' -------------------------------------------------------------------------------------------------
' Module      : GestionSauvegardes
' Projet      : Althéa
' Version     : V1.3.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Sauvegarde complète de l'application (lot 30, étape P2) : la base ET le dossier de données,
' dans une seule archive ZIP chiffrée AES-256 (ouvrable avec 7-Zip et le mot de passe).
'
' Contenu d'une archive Althea_AAAAMMJJ_HHMMSS.zip :
'   base/althea.sql   dump complet de la base (mariadb-dump : tables, séquences, vues,
'                     déclencheurs, routines ; pas d'événements planifiés, inexistants)
'   donnees/…         tout le dossier de données (PATH_GENERAL : Documents, Cabinet, Utilisateurs)
'   manifeste.json    date, versions de l'application et du schéma, machine, compte, volumes
'
' Responsabilités :
' - Contrôler les prérequis : droits, dossier de sauvegarde (hors dossier de données), mot de
'   passe défini, outil mariadb-dump trouvé, racine des données accessible
' - Produire l'archive sous un nom provisoire puis la renommer une fois complète
' - Journaliser chaque tentative dans tec_sauvegardes ; appliquer la rotation
' - Gérer le mot de passe des archives (définition réservée à l'Admin, vérification)
' - Fournir l'historique et l'information de rappel
'
' Sécurité :
' - Le dump passe directement de mariadb-dump à l'archive chiffrée : la base n'est jamais écrite
'   en clair sur le disque.
' - Le mot de passe de la base est transmis à mariadb-dump par un fichier d'options temporaire
'   dans le dossier temporaire du compte Windows, supprimé aussitôt ; jamais sur la ligne de
'   commande (visible des autres programmes).
' - Le mot de passe des archives n'est jamais journalisé ni stocké en base (MotDePasseSauvegarde).
'
' Remarques   :
' - Fonctions longues et synchrones : l'écran les lance en arrière-plan (Task.Run) et reçoit
'   l'avancement par IProgress.
' - La restauration (GestionRestaurations, étape P3) lit la même archive ; les outils MariaDB sont
'   trouvés et alimentés par OutilsMariaDB, commun aux deux.
'
' Dépendances :
' - ICSharpCode.SharpZipLib (ZIP AES), GestionParametres / ClesParametres, ArborescenceDonnees,
'   ConfigManager / CryptoManagerDPAPI (connexion), MotDePasseSauvegarde, QuerySauvegardes, OutilsMariaDB
' - V1.2.0 (22/10/2026, audit C.1) : DefinirDossierSauvegarde, Sauvegarder, MessageRappel et
'   DefinirMotDePasse reçoivent un ContexteOperation (Autorisation.Exiger / Possede).
' - V1.3.0 (23/10/2026, audit C.2 étape 4) : règle du mot de passe des archives lue dans
'   PolitiqueMotDePasse (longueur LongueurMinSauvegarde) : elle était recopiée ici.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.Json
Imports System.Text.RegularExpressions
Imports ICSharpCode.SharpZipLib.Zip
Imports MySqlConnector

Public Module GestionSauvegardes

#Region "Constantes"

    ' Nom des archives : préfixe + horodatage ; la rotation ne touche qu'aux fichiers de ce motif.
    Public Const PrefixeArchive As String = "Althea_"
    Private Const FormatHorodatage As String = "yyyyMMdd_HHmmss"
    Private Const ExtensionArchive As String = ".zip"
    Private Const ExtensionProvisoire As String = ".tmp"
    Friend ReadOnly MotifArchive As New Regex("^Althea_\d{8}_\d{6}\.zip$", RegexOptions.IgnoreCase)

    ' Organisation interne de l'archive (relue par la restauration, P3).
    Public Const EntreeDump As String = "base/althea.sql"
    Public Const PrefixeEntreesDonnees As String = "donnees/"
    Public Const EntreeManifeste As String = "manifeste.json"
    Public Const VersionFormatArchive As Integer = 1

    ' Chiffrement WinZip AES-256 ; compression moyenne (documents souvent déjà compressés).
    Private Const TailleCleAes As Integer = 256
    Private Const NiveauCompression As Integer = 5

    ' Fichiers « propriétaire » créés par Word/Excel à côté d'un document ouvert (~$nom.docx) :
    ' simples verrous temporaires, sans contenu utile, qu'il ne faut surtout pas restaurer.
    Private Const PrefixeFichierVerrouOffice As String = "~$"
    Private Const TailleTamponCopie As Integer = 81920

    ' Règle de sécurité du mot de passe des archives (plus exigeante que celle des comptes, 10 :
    ' il protège toutes les données, y compris hors de l'application). Texte affiché par l'écran.
    ' La règle elle-même est PolitiqueMotDePasse (LongueurMinSauvegarde, audit C.2).
    Public ReadOnly Property DescriptionReglesMotDePasse As String
        Get
            Return PolitiqueMotDePasse.Description(PolitiqueMotDePasse.LongueurMinSauvegarde) &
                   " Une phrase de plusieurs mots est idéale."
        End Get
    End Property

    ' Une archive provisoire plus ancienne vient d'une sauvegarde interrompue : supprimée.
    Private Const HeuresAvantNettoyageProvisoire As Integer = 24

    ' Longueur maximale d'un message d'erreur journalisé.
    Private Const LongueurMaxMessage As Integer = 1000

#End Region

#Region "Dossier de sauvegarde"

    ''' <summary>Dossier des archives (paramètre SAUVEGARDE_DOSSIER) ; vide s'il n'est pas choisi.</summary>
    Public Function GetDossierSauvegarde() As String
        Return If(GestionParametres.GetValeurParametre(ClesParametres.SauvegardeDossier), String.Empty).Trim()
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ValiderDossierSauvegarde
    ' Version    : V1.0.0
    ' Date       : 12/10/2026
    '
    ' Rôle       :
    ' Vérifie qu'un dossier peut recevoir les sauvegardes. Retourne Nothing s'il convient, sinon le
    ' message à afficher.
    '
    ' Remarques  :
    ' - Refusé à l'intérieur du dossier de données (ou égal à lui) : les archives seraient
    '   sauvegardées dans les archives suivantes et partiraient avec le disque en cas de panne.
    ' - Utilisée par l'écran Paramètres et avant chaque sauvegarde.
    ' -------------------------------------------------------------------------------------------------
    Public Function ValiderDossierSauvegarde(chemin As String) As String

        If String.IsNullOrWhiteSpace(chemin) Then Return "Le dossier des sauvegardes n'est pas encore choisi."
        If Not Path.IsPathFullyQualified(chemin) Then Return "Le dossier des sauvegardes doit être un chemin complet."

        Dim racine As String
        Try
            racine = ArborescenceDonnees.GetRacineDonnees()
        Catch ex As InvalidOperationException
            Return "La racine du dossier de données n'est pas configurée (Paramètres › PATHS)."
        End Try

        If EstDansDossier(chemin, racine) Then
            Return "Le dossier des sauvegardes ne peut pas se trouver dans le dossier de données." & Environment.NewLine &
                   "Choisissez un autre emplacement, idéalement sur un autre disque."
        End If

        Return Nothing

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : DefinirDossierSauvegarde
    ' Version    : V1.0.0
    ' Date       : 12/10/2026
    '
    ' Rôle       :
    ' Enregistre le dossier des archives (SAUVEGARDE_DOSSIER) depuis l'écran Sauvegardes.
    ' Retourne Nothing si c'est fait, sinon le message de refus.
    '
    ' Remarques  :
    ' - SuperUser et Admin (comme la sauvegarde) ; la règle « hors dossier de données » s'applique.
    ' -------------------------------------------------------------------------------------------------
    Public Function DefinirDossierSauvegarde(contexte As ContexteOperation, chemin As String) As String

        Autorisation.Exiger(contexte, AppRole.SuperUser, "Seuls le SuperUser et l'administrateur peuvent choisir le dossier des sauvegardes.")

        Dim refus As String = ValiderDossierSauvegarde(chemin)
        If refus IsNot Nothing Then Return refus

        Dim modeAcces As ModeAccesParametres =
            If(contexte.RoleCourant = AppRole.Admin, ModeAccesParametres.Admin, ModeAccesParametres.SuperUser)

        If Not GestionParametres.UpdateValeurParametreParCle(ClesParametres.SauvegardeDossier, chemin.Trim(), modeAcces) Then
            Return "Le paramètre du dossier des sauvegardes est introuvable ou inactif (migration v2.38.0 appliquée ?)."
        End If

        GestionLog.EcrireLog($"Dossier des sauvegardes défini : {chemin.Trim()}.",
                             GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Process)
        Return Nothing

    End Function

    ' True si chemin est le dossier racine lui-même ou l'un de ses sous-dossiers.
    Friend Function EstDansDossier(chemin As String, racine As String) As Boolean
        Dim c As String = Path.TrimEndingDirectorySeparator(Path.GetFullPath(chemin)) & Path.DirectorySeparatorChar
        Dim r As String = Path.TrimEndingDirectorySeparator(Path.GetFullPath(racine)) & Path.DirectorySeparatorChar
        Return c.StartsWith(r, StringComparison.OrdinalIgnoreCase)
    End Function

#End Region

#Region "Outil mariadb-dump"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : TrouverOutilDump
    ' Version    : V1.1.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Chemin de mariadb-dump (recherche commune : OutilsMariaDB.TrouverOutil). Nothing si introuvable.
    ' -------------------------------------------------------------------------------------------------
    Public Function TrouverOutilDump() As String
        Return OutilsMariaDB.TrouverOutil(OutilsMariaDB.NomsOutilDump)
    End Function

#End Region

#Region "Sauvegarde"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : Sauvegarder
    ' Version    : V1.1.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Produit une archive complète (base + dossier de données), la journalise et applique la
    ' rotation.
    '
    ' Paramètres :
    ' - contexte    : qui lance la sauvegarde (SuperUser ou Admin, élévation comprise ; compte au journal)
    ' - progression : avancement pour l'écran (peut être Nothing)
    ' - avecRotation : False pour la sauvegarde de sécurité d'une restauration (la rotation
    '   pourrait supprimer l'archive en cours de restauration si elle est la plus ancienne)
    '
    ' Retour     :
    ' - ResultatSauvegarde : réussite ou message d'échec compréhensible
    '
    ' Remarques  :
    ' - Un prérequis manquant (dossier, mot de passe, outil) n'est pas une tentative : message
    '   sans ligne de journal. Une tentative commencée est toujours journalisée (réussie / échec).
    ' - En cas d'échec, l'archive provisoire est supprimée : aucune archive incomplète ne garde
    '   le nom d'une sauvegarde.
    ' - Une erreur de rotation n'invalide pas la sauvegarde (journalisée seulement).
    ' -------------------------------------------------------------------------------------------------
    Public Function Sauvegarder(contexte As ContexteOperation,
                                progression As IProgress(Of ProgressionSauvegarde),
                                Optional avecRotation As Boolean = True) As ResultatSauvegarde

        Autorisation.Exiger(contexte, AppRole.SuperUser, "Seuls le SuperUser et l'administrateur peuvent lancer une sauvegarde.")
        Dim utilisateur As UtilisateurApplication = contexte.Utilisateur

        ' --- Prérequis
        Dim dossier As String = GetDossierSauvegarde()
        Dim messageDossier As String = ValiderDossierSauvegarde(dossier)
        If messageDossier IsNot Nothing Then Return Echec(messageDossier)

        If Not Directory.Exists(dossier) Then
            Return Echec($"Le dossier des sauvegardes « {dossier} » est inaccessible (disque débranché ?).")
        End If

        Dim motDePasse As String = MotDePasseSauvegarde.Obtenir()
        If String.IsNullOrEmpty(motDePasse) Then
            Return Echec("Le mot de passe des sauvegardes n'est pas défini sur ce poste." & Environment.NewLine &
                         "L'administrateur doit le définir (carte « Mot de passe »).")
        End If

        Dim outilDump As String = TrouverOutilDump()
        If outilDump Is Nothing Then
            Return Echec("L'outil mariadb-dump est introuvable." & Environment.NewLine &
                         "Indiquez le dossier bin de MariaDB dans Paramètres › SAUVEGARDE.")
        End If

        Dim racine As String = ArborescenceDonnees.GetRacineDonnees()
        If Not Directory.Exists(racine) Then
            Return Echec($"Le dossier de données « {racine} » est introuvable.")
        End If

        Dim config As LocalDbConfig = ConfigManager.LoadLocalDbConfig()
        If config Is Nothing OrElse Not config.IsConfigurationValid Then
            Return Echec("La configuration de connexion à la base est incomplète.")
        End If

        ' --- Tentative
        Dim horodatage As String = DateTime.Now.ToString(FormatHorodatage)
        Dim nomArchive As String = PrefixeArchive & horodatage & ExtensionArchive
        Dim cheminArchive As String = Path.Combine(dossier, nomArchive)
        Dim cheminProvisoire As String = cheminArchive & ExtensionProvisoire
        Dim versionSchema As String = LireVersionSchema()
        Dim versionApplication As String = LireVersionApplication()

        Dim idJournal As ULong = JournaliserDebut(utilisateur, nomArchive, dossier, versionSchema, versionApplication)

        GestionLog.EcrireLog($"Sauvegarde lancée par {If(utilisateur?.LoginUtilisateur, "?")} : {nomArchive}.",
                             GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Process)

        Try
            Dim fichiers As List(Of String) = Directory.EnumerateFiles(racine, "*", SearchOption.AllDirectories) _
                .Where(Function(f) Not Path.GetFileName(f).StartsWith(PrefixeFichierVerrouOffice, StringComparison.Ordinal)) _
                .ToList()
            Dim tailleDonnees As Long = 0

            Using flux As New FileStream(cheminProvisoire, FileMode.CreateNew, FileAccess.Write, FileShare.None),
                  zip As New ZipOutputStream(flux)

                zip.SetLevel(NiveauCompression)
                zip.Password = motDePasse

                Signaler(progression, "Export de la base…", 2)
                EcrireDump(zip, outilDump, config)

                For i As Integer = 0 To fichiers.Count - 1
                    Signaler(progression, $"Copie des fichiers ({i + 1} / {fichiers.Count})…", 10 + CInt(85L * i \ Math.Max(fichiers.Count, 1)))
                    tailleDonnees += EcrireFichier(zip, racine, fichiers(i))
                Next

                Signaler(progression, "Finalisation…", 96)
                EcrireManifeste(zip, config.DatabaseName, versionSchema, versionApplication, utilisateur,
                                fichiers.Count, tailleDonnees)

                zip.Finish()
            End Using

            File.Move(cheminProvisoire, cheminArchive)

            Dim taille As Long = New FileInfo(cheminArchive).Length
            JournaliserFin(idJournal, StatutSauvegarde.Reussie, taille, fichiers.Count, Nothing)

            Dim nbSupprimees As Integer = 0
            If avecRotation Then
                Signaler(progression, "Rotation des anciennes sauvegardes…", 98)
                nbSupprimees = AppliquerRotation(dossier)
            End If

            GestionLog.EcrireLog($"Sauvegarde réussie : {nomArchive} ({taille} octets, {fichiers.Count} fichiers, {nbSupprimees} ancienne(s) archive(s) supprimée(s)).",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Process)

            Signaler(progression, "Sauvegarde terminée.", 100)

            Return New ResultatSauvegarde With {
                .Reussie = True,
                .CheminArchive = cheminArchive,
                .TailleOctets = taille,
                .NbFichiers = fichiers.Count,
                .NbArchivesSupprimees = nbSupprimees,
                .Message = "Sauvegarde terminée."
            }

        Catch ex As Exception

            SupprimerSansErreur(cheminProvisoire)
            JournaliserFin(idJournal, StatutSauvegarde.Echec, Nothing, Nothing, Tronquer(ex.Message))

            GestionLog.EcrireLog($"Sauvegarde en échec : {nomArchive}.", GestionLog.LogLevel.Succinct,
                                 GestionLog.LogCategory.Process, ex)

            Return Echec("La sauvegarde a échoué : " & ex.Message)

        End Try

    End Function

    Private Function Echec(message As String) As ResultatSauvegarde
        Return New ResultatSauvegarde With {.Reussie = False, .Message = message}
    End Function

    Private Sub Signaler(progression As IProgress(Of ProgressionSauvegarde), etape As String, pourcentage As Integer)
        progression?.Report(New ProgressionSauvegarde With {.Etape = etape, .Pourcentage = Math.Max(0, Math.Min(100, pourcentage))})
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : EcrireDump
    ' Version    : V1.1.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Lance mariadb-dump et copie sa sortie directement dans l'entrée chiffrée base/althea.sql.
    '
    ' Remarques  :
    ' - --defaults-extra-file doit être la première option (exigence de l'outil).
    ' - --single-transaction : instantané cohérent des tables InnoDB sans les verrouiller.
    ' - --skip-add-locks : pas de LOCK TABLES dans le dump. Indispensable : à la restauration, un
    '   LOCK TABLES sur une table dont une colonne vaut DEFAULT nextval(seq_…) échoue (« Table
    '   'seq_…' was not locked with LOCK TABLES ») — constaté en test le 12/10/2026.
    ' - Pas de --events : la base n'a aucun événement planifié, et l'option exige le droit EVENT que
    '   le compte de l'application n'a pas (moindre privilège) — constaté le 12/10/2026 (erreur 1044).
    '   À réintroduire, avec ce droit, si un événement MariaDB est un jour créé.
    ' - Sans --databases : pas de CREATE DATABASE / USE, la restauration choisit la base cible.
    ' - La sortie d'erreur est lue en parallèle (sinon blocage si elle se remplit).
    ' -------------------------------------------------------------------------------------------------
    Private Sub EcrireDump(zip As ZipOutputStream, outilDump As String, config As LocalDbConfig)

        Dim fichierOptions As String = Nothing

        Try
            fichierOptions = OutilsMariaDB.CreerFichierOptions(config.Host, config.Port, config.UserName,
                                                               CryptoManagerDPAPI.Unprotect(config.EncryptedPassword))

            Dim infos As New ProcessStartInfo(outilDump) With {
                .UseShellExecute = False,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .CreateNoWindow = True
            }
            infos.ArgumentList.Add("--defaults-extra-file=" & fichierOptions)
            infos.ArgumentList.Add("--single-transaction")
            infos.ArgumentList.Add("--skip-add-locks")
            infos.ArgumentList.Add("--routines")
            infos.ArgumentList.Add("--triggers")
            infos.ArgumentList.Add("--hex-blob")
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

                zip.PutNextEntry(New ZipEntry(EntreeDump) With {.DateTime = DateTime.Now, .AESKeySize = TailleCleAes})
                processus.StandardOutput.BaseStream.CopyTo(zip)
                zip.CloseEntry()

                processus.WaitForExit()

                If processus.ExitCode <> 0 Then
                    Dim detail As String
                    SyncLock erreurs
                        detail = erreurs.ToString().Trim()
                    End SyncLock
                    Throw New InvalidOperationException(
                        $"l'export de la base a échoué (mariadb-dump, code {processus.ExitCode}) : {Tronquer(detail)}")
                End If

            End Using

        Finally
            OutilsMariaDB.SupprimerFichierOptions(fichierOptions)
        End Try

    End Sub

    ' Copie un fichier du dossier de données dans donnees/… ; retourne le nombre d'octets copiés.
    '
    ' Un fichier ouvert dans un autre programme peut être illisible ou se lire vide (constaté le
    ' 12/10/2026 : document ouvert dans Word, 0 octet lu au lieu de 7 227). La sauvegarde ne doit
    ' jamais être incomplète en silence : on compte les octets réellement lus et, s'ils ne
    ' correspondent pas à la taille du fichier, on échoue avec un message qui nomme le fichier.
    ' La taille n'est volontairement pas annoncée à l'entrée ZIP : sinon SharpZipLib lève lui-même
    ' une erreur peu lisible (« size was 0, but … was expected ») que la fermeture de l'archive
    ' relance ensuite, masquant notre message.
    Private Function EcrireFichier(zip As ZipOutputStream, racine As String, cheminFichier As String) As Long

        Dim relatif As String = Path.GetRelativePath(racine, cheminFichier)
        Dim info As New FileInfo(cheminFichier)
        Dim octetsLus As Long = 0

        Try
            ' Ouvert avant de créer l'entrée : un fichier verrouillé ne laisse pas d'entrée orpheline.
            Using source As New FileStream(cheminFichier, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)

                zip.PutNextEntry(New ZipEntry(ZipEntry.CleanName(PrefixeEntreesDonnees & relatif.Replace(Path.DirectorySeparatorChar, "/"c))) With {
                    .DateTime = info.LastWriteTime,
                    .AESKeySize = TailleCleAes
                })

                Dim tampon(TailleTamponCopie - 1) As Byte
                Dim lus As Integer = source.Read(tampon, 0, tampon.Length)
                While lus > 0
                    zip.Write(tampon, 0, lus)
                    octetsLus += lus
                    lus = source.Read(tampon, 0, tampon.Length)
                End While

            End Using
            zip.CloseEntry()

        Catch ex As IOException
            Throw New IOException(MessageFichierIllisible(relatif), ex)
        End Try

        If octetsLus <> info.Length Then
            Throw New IOException(MessageFichierIllisible(relatif) &
                                  $" ({octetsLus} octet(s) lu(s) sur {info.Length}).")
        End If

        Return octetsLus

    End Function

    Private Function MessageFichierIllisible(relatif As String) As String
        Return $"le fichier « {relatif} » n'a pas pu être lu entièrement. Il est sans doute ouvert dans " &
               "un autre programme (Word, lecteur PDF…) : fermez-le puis relancez la sauvegarde"
    End Function

    Private Sub EcrireManifeste(zip As ZipOutputStream, nomBase As String, versionSchema As String,
                                versionApplication As String, utilisateur As UtilisateurApplication,
                                nbFichiers As Integer, tailleDonnees As Long)

        Dim manifeste As New Dictionary(Of String, Object) From {
            {"application", "Althéa"},
            {"formatArchive", VersionFormatArchive},
            {"dateSauvegarde", DateTime.Now.ToString("o")},
            {"versionApplication", versionApplication},
            {"versionSchema", versionSchema},
            {"base", nomBase},
            {"machine", Environment.MachineName},
            {"utilisateur", If(utilisateur?.LoginUtilisateur, String.Empty)},
            {"nbFichiersDonnees", nbFichiers},
            {"tailleDonneesOctets", tailleDonnees},
            {"entreeBase", EntreeDump},
            {"prefixeDonnees", PrefixeEntreesDonnees}
        }

        Dim octets As Byte() = JsonSerializer.SerializeToUtf8Bytes(manifeste, New JsonSerializerOptions With {.WriteIndented = True})

        zip.PutNextEntry(New ZipEntry(EntreeManifeste) With {.DateTime = DateTime.Now, .Size = octets.Length, .AESKeySize = TailleCleAes})
        zip.Write(octets, 0, octets.Length)
        zip.CloseEntry()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : AppliquerRotation
    ' Version    : V1.0.0
    ' Date       : 12/10/2026
    '
    ' Rôle       :
    ' Garde les N archives les plus récentes (SAUVEGARDE_NB_CONSERVEES) et supprime les archives
    ' provisoires abandonnées. Retourne le nombre d'archives supprimées.
    '
    ' Remarques  :
    ' - Ne touche qu'aux fichiers au motif exact Althea_AAAAMMJJ_HHMMSS.zip : un autre fichier du
    '   dossier n'est jamais supprimé.
    ' - Le nom porte l'horodatage : l'ordre alphabétique inverse est l'ordre chronologique inverse.
    ' -------------------------------------------------------------------------------------------------
    Private Function AppliquerRotation(dossier As String) As Integer

        Dim nbConservees As Integer = GestionParametres.GetEntier(ClesParametres.SauvegardeNbConservees,
                                                                  ClesParametres.DefautSauvegardeNbConservees)
        Dim repertoire As New DirectoryInfo(dossier)
        Dim nbSupprimees As Integer = 0

        Dim archives = repertoire.GetFiles(PrefixeArchive & "*" & ExtensionArchive) _
            .Where(Function(f) MotifArchive.IsMatch(f.Name)) _
            .OrderByDescending(Function(f) f.Name, StringComparer.OrdinalIgnoreCase) _
            .ToList()

        For Each ancienne In archives.Skip(Math.Max(nbConservees, 1))
            Try
                ancienne.Delete()
                nbSupprimees += 1
                GestionLog.EcrireLog($"Rotation : ancienne sauvegarde supprimée ({ancienne.Name}).",
                                     GestionLog.LogLevel.Rapide, GestionLog.LogCategory.Process)
            Catch ex As Exception
                GestionLog.EcrireLog($"Rotation : suppression impossible de {ancienne.Name}.",
                                     GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Process, ex)
            End Try
        Next

        Dim limite As DateTime = DateTime.Now.AddHours(-HeuresAvantNettoyageProvisoire)
        For Each provisoire In repertoire.GetFiles(PrefixeArchive & "*" & ExtensionArchive & ExtensionProvisoire)
            If provisoire.LastWriteTime < limite Then SupprimerSansErreur(provisoire.FullName)
        Next

        Return nbSupprimees

    End Function

    Private Sub SupprimerSansErreur(chemin As String)
        Try
            If File.Exists(chemin) Then File.Delete(chemin)
        Catch ex As Exception
            GestionLog.EcrireLog($"Sauvegarde : fichier provisoire non supprimé ({chemin}).",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Process, ex)
        End Try
    End Sub

    Friend Function Tronquer(texte As String) As String
        If String.IsNullOrEmpty(texte) Then Return texte
        Return If(texte.Length <= LongueurMaxMessage, texte, texte.Substring(0, LongueurMaxMessage) & "…")
    End Function

    Friend Function LireVersionApplication() As String
        Dim version = Reflection.Assembly.GetEntryAssembly()?.GetName().Version
        Return If(version?.ToString(), String.Empty)
    End Function

    Friend Function LireVersionSchema() As String
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QuerySauvegardes.SelectVersionSchema, conn)
                Dim resultat As Object = cmd.ExecuteScalar()
                Return If(resultat Is Nothing OrElse Convert.IsDBNull(resultat), String.Empty, resultat.ToString())
            End Using
        End Using
    End Function

#End Region

#Region "Journal"

    Private Function JournaliserDebut(utilisateur As UtilisateurApplication, nomArchive As String, dossier As String,
                                      versionSchema As String, versionApplication As String) As ULong
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QuerySauvegardes.InsertDebut, conn)
                cmd.Parameters.AddWithValue("@statut", StatutsSauvegarde.CodeEnCours)
                cmd.Parameters.AddWithValue("@nom_fichier", nomArchive)
                cmd.Parameters.AddWithValue("@dossier", dossier)
                cmd.Parameters.AddWithValue("@version_schema", DbHelper.ValeurOuDBNull(versionSchema))
                cmd.Parameters.AddWithValue("@version_application", DbHelper.ValeurOuDBNull(versionApplication))
                cmd.Parameters.AddWithValue("@id_utilisateur",
                                            If(utilisateur IsNot Nothing AndAlso utilisateur.IdUtilisateur > 0,
                                               CObj(utilisateur.IdUtilisateur), DBNull.Value))
                cmd.Parameters.AddWithValue("@login", DbHelper.ValeurOuDBNull(utilisateur?.LoginUtilisateur))
                cmd.Parameters.AddWithValue("@machine", Environment.MachineName)
                Return Convert.ToUInt64(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    ' Ne lève pas d'exception : une erreur de journal ne doit pas masquer le résultat réel.
    Private Sub JournaliserFin(idJournal As ULong, statut As StatutSauvegarde, taille As Long?,
                               nbFichiers As Integer?, message As String)
        Try
            Using conn = DatabaseManager.OpenConnection()
                Using cmd As New MySqlCommand(QuerySauvegardes.UpdateFin, conn)
                    cmd.Parameters.AddWithValue("@id", idJournal)
                    cmd.Parameters.AddWithValue("@statut", StatutsSauvegarde.VersCode(statut))
                    cmd.Parameters.AddWithValue("@taille", If(taille.HasValue, CObj(taille.Value), DBNull.Value))
                    cmd.Parameters.AddWithValue("@nb_fichiers", If(nbFichiers.HasValue, CObj(nbFichiers.Value), DBNull.Value))
                    cmd.Parameters.AddWithValue("@message", DbHelper.ValeurOuDBNull(message))
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            GestionLog.EcrireLog($"Journal des sauvegardes : clôture impossible de la ligne {idJournal}.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database, ex)
        End Try
    End Sub

    ''' <summary>Dernières sauvegardes, de la plus récente à la plus ancienne.</summary>
    Public Function GetHistorique(Optional limite As Integer = 50) As List(Of SauvegardeJournal)

        Dim resultat As New List(Of SauvegardeJournal)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QuerySauvegardes.SelectHistorique, conn)
                cmd.Parameters.AddWithValue("@limite", Math.Max(limite, 1))
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        resultat.Add(New SauvegardeJournal With {
                            .IdSauvegarde = reader.GetUInt64("id_sauvegarde"),
                            .DateDebut = reader.GetDateTime("date_debut"),
                            .DateFin = DbHelper.LireDateNullable(reader, "date_fin"),
                            .Statut = StatutsSauvegarde.DepuisCode(reader.GetString("statut_sauvegarde")),
                            .NomFichier = DbHelper.LireString(reader, "nom_fichier"),
                            .DossierSauvegarde = DbHelper.LireString(reader, "dossier_sauvegarde"),
                            .TailleOctets = If(reader.IsDBNull("taille_octets"), CType(Nothing, Long?), Convert.ToInt64(reader("taille_octets"))),
                            .NbFichiers = If(reader.IsDBNull("nb_fichiers"), CType(Nothing, Integer?), Convert.ToInt32(reader("nb_fichiers"))),
                            .VersionSchema = DbHelper.LireString(reader, "version_schema"),
                            .LoginUtilisateur = DbHelper.LireString(reader, "login_utilisateur"),
                            .Machine = DbHelper.LireString(reader, "machine"),
                            .MessageErreur = DbHelper.LireString(reader, "message_erreur")
                        })
                    End While
                End Using
            End Using
        End Using

        Return resultat

    End Function

    ''' <summary>Date de fin de la dernière sauvegarde réussie ; Nothing s'il n'y en a aucune.</summary>
    Public Function GetDateDerniereReussie() As DateTime?
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QuerySauvegardes.SelectDateDerniereReussie, conn)
                cmd.Parameters.AddWithValue("@statut", StatutsSauvegarde.CodeReussie)
                Dim resultat As Object = cmd.ExecuteScalar()
                If resultat Is Nothing OrElse Convert.IsDBNull(resultat) Then Return Nothing
                Return Convert.ToDateTime(resultat)
            End Using
        End Using
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : MessageRappel
    ' Version    : V1.0.0
    ' Date       : 12/10/2026
    '
    ' Rôle       :
    ' Texte du rappel à afficher à l'ouverture, ou Nothing si aucun rappel n'est nécessaire
    ' (rôle insuffisant, ou dernière sauvegarde réussie récente — SAUVEGARDE_RAPPEL_JOURS).
    '
    ' Remarques  :
    ' - Ne lève pas d'exception : un rappel impossible à calculer ne doit pas gêner l'ouverture.
    ' -------------------------------------------------------------------------------------------------
    Public Function MessageRappel(contexte As ContexteOperation) As String

        If Not Autorisation.Possede(contexte, AppRole.SuperUser) Then Return Nothing

        Try
            Dim delai As Integer = GestionParametres.GetEntier(ClesParametres.SauvegardeRappelJours,
                                                               ClesParametres.DefautSauvegardeRappelJours)
            Dim derniere As DateTime? = GetDateDerniereReussie()

            If Not derniere.HasValue Then
                Return "Aucune sauvegarde n'a encore été faite." & Environment.NewLine &
                       "Pensez à sauvegarder la base et les documents (Administration › Sauvegardes)."
            End If

            Dim jours As Integer = CInt(Math.Floor((DateTime.Now - derniere.Value).TotalDays))
            If jours <= delai Then Return Nothing

            Return $"La dernière sauvegarde date de {jours} jours (le {derniere.Value:dd/MM/yyyy})." & Environment.NewLine &
                   "Pensez à sauvegarder (Administration › Sauvegardes)."

        Catch ex As Exception
            GestionLog.EcrireLog("Rappel de sauvegarde : calcul impossible.", GestionLog.LogLevel.Succinct,
                                 GestionLog.LogCategory.Database, ex)
            Return Nothing
        End Try

    End Function

#End Region

#Region "Mot de passe des archives"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DefinirMotDePasse
    ' Version    : V1.1.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Définit ou remplace le mot de passe des archives (Admin, élévation comprise).
    '
    ' Remarques  :
    ' - Mot de passe PROPRE aux sauvegardes, commun au cabinet : ce n'est pas un mot de passe de
    '   connexion (ceux-ci ne sont conservés qu'en empreinte et ne peuvent pas chiffrer une archive ;
    '   ils changent et diffèrent d'un compte à l'autre, alors qu'une archive doit rester lisible).
    ' - Règles : DescriptionReglesMotDePasse.
    ' - Les archives déjà produites restent chiffrées avec l'ancien mot de passe (l'écran le dit).
    ' - Journalisé sans jamais écrire le mot de passe.
    ' - V1.1.0 (23/10/2026, audit C.2) : règle de PolitiqueMotDePasse (LongueurMinSauvegarde).
    ' -------------------------------------------------------------------------------------------------
    Public Sub DefinirMotDePasse(contexte As ContexteOperation, motDePasse As String, confirmation As String)

        Autorisation.Exiger(contexte, AppRole.Admin, "Seul l'administrateur peut définir le mot de passe des sauvegardes.")
        Dim utilisateur As UtilisateurApplication = contexte.Utilisateur

        If Not PolitiqueMotDePasse.EstConforme(motDePasse, PolitiqueMotDePasse.LongueurMinSauvegarde) Then
            Throw New ArgumentException("Le mot de passe ne respecte pas les règles :" & Environment.NewLine &
                                        DescriptionReglesMotDePasse)
        End If

        If Not String.Equals(motDePasse, confirmation, StringComparison.Ordinal) Then
            Throw New ArgumentException("Les deux saisies du mot de passe ne correspondent pas.")
        End If

        Dim remplacement As Boolean = MotDePasseSauvegarde.EstDefini()
        MotDePasseSauvegarde.Enregistrer(motDePasse)

        GestionLog.EcrireLog($"Mot de passe des sauvegardes {If(remplacement, "modifié", "défini")} par {If(utilisateur?.LoginUtilisateur, "?")}.",
                             GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security)

    End Sub

    ''' <summary>Compare une saisie au mot de passe enregistré (comparaison à temps constant).</summary>
    Public Function VerifierMotDePasse(saisie As String) As Boolean
        Dim attendu As String = MotDePasseSauvegarde.Obtenir()
        If String.IsNullOrEmpty(attendu) OrElse saisie Is Nothing Then Return False
        Return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(saisie), Encoding.UTF8.GetBytes(attendu))
    End Function

#End Region

End Module
