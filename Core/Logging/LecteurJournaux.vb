' -------------------------------------------------------------------------------------------------
' Module      : LecteurJournaux
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 13/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Lecture des journaux écrits par GestionLog, pour l'écran Journaux (Administration).
'
' Responsabilités :
' - Lister les fichiers journaux du dossier (%APPDATA%\Althea\Logs), du plus récent au plus ancien
' - Lire un fichier et le découper en entrées : horodatage, niveau, catégorie, message, exception,
'   pile d'appels, en-têtes de session
'
' Remarques   :
' - Lecture seule ; le fichier du jour est ouvert en lecture partagée (GestionLog continue d'écrire).
' - Le format d'une ligne vient de GestionLog (constantes) : une seule définition.
' - Une ligne qui ne commence pas par un horodatage prolonge l'entrée précédente (message écrit
'   sur plusieurs lignes) ; les lignes de tirets encadrant les en-têtes de session sont ignorées.
'
' Dépendances :
' - GestionLog (dossier, format des lignes)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions

Public Module LecteurJournaux

    ' « 2026-10-13 15:22:12 [Succinct] [Process] message… »
    Private ReadOnly MotifEntree As New Regex("^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}) \[(\w+)\] \[(\w+)\] ?(.*)$")
    ' « 2026-10-13 15:14:40 [SESSION] Démarrage application | Machine=… | User=… »
    Private ReadOnly MotifSession As New Regex("^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}) " & Regex.Escape(GestionLog.MarqueurSession) & " ?(.*)$")
    Private ReadOnly MotifSeparateur As New Regex("^-{10,}$")

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ListerFichiers
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Fichiers journaux présents (Althea_AAAA-MM-JJ.log), du plus récent au plus ancien.
    ' -------------------------------------------------------------------------------------------------
    Public Function ListerFichiers() As List(Of FichierJournal)

        Dim resultat As New List(Of FichierJournal)
        Dim dossier As String = GestionLog.GetDossierJournaux()
        If Not Directory.Exists(dossier) Then Return resultat

        For Each info In New DirectoryInfo(dossier).GetFiles(GestionLog.PrefixeFichierJournal & "*" & GestionLog.ExtensionFichierJournal)
            Dim partieDate As String = Path.GetFileNameWithoutExtension(info.Name).Substring(GestionLog.PrefixeFichierJournal.Length)
            Dim jour As DateTime
            If DateTime.TryParseExact(partieDate, GestionLog.FormatDateFichierJournal, CultureInfo.InvariantCulture,
                                      DateTimeStyles.None, jour) Then
                resultat.Add(New FichierJournal With {.Chemin = info.FullName, .Jour = jour, .TailleOctets = info.Length})
            End If
        Next

        Return resultat.OrderByDescending(Function(f) f.Jour).ToList()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : Lire
    ' Version    : V1.0.0
    ' Date       : 13/10/2026
    '
    ' Rôle       :
    ' Lit un fichier journal et retourne ses entrées dans l'ordre du fichier.
    '
    ' Exceptions :
    ' - IOException / UnauthorizedAccessException : fichier illisible (à afficher par l'appelant)
    ' -------------------------------------------------------------------------------------------------
    Public Function Lire(cheminFichier As String) As List(Of EntreeJournal)

        Dim entrees As New List(Of EntreeJournal)
        Dim numero As Integer = 0

        Using flux As New FileStream(cheminFichier, FileMode.Open, FileAccess.Read, FileShare.ReadWrite Or FileShare.Delete),
              lecteur As New StreamReader(flux, Encoding.UTF8, detectEncodingFromByteOrderMarks:=True)

            Dim ligne As String = lecteur.ReadLine()
            While ligne IsNot Nothing
                numero += 1
                Dim entree As EntreeJournal = LireLigne(ligne, numero)

                If entree IsNot Nothing Then
                    entrees.Add(entree)
                ElseIf ligne.Length > 0 AndAlso Not MotifSeparateur.IsMatch(ligne) AndAlso entrees.Count > 0 Then
                    ' Suite d'un message écrit sur plusieurs lignes.
                    Dim precedente As EntreeJournal = entrees(entrees.Count - 1)
                    precedente.Message &= Environment.NewLine & ligne
                End If

                ligne = lecteur.ReadLine()
            End While

        End Using

        Return entrees

    End Function

    ' Une ligne qui commence une entrée (normale ou en-tête de session), sinon Nothing.
    Friend Function LireLigne(ligne As String, numero As Integer) As EntreeJournal

        Dim m As Match = MotifSession.Match(ligne)
        If m.Success Then
            Return New EntreeJournal With {
                .NumeroLigne = numero,
                .Horodatage = LireHorodatage(m.Groups(1).Value),
                .EstSession = True,
                .Niveau = String.Empty,
                .Categorie = String.Empty,
                .Message = m.Groups(2).Value
            }
        End If

        m = MotifEntree.Match(ligne)
        If Not m.Success Then Return Nothing

        Dim entree As New EntreeJournal With {
            .NumeroLigne = numero,
            .Horodatage = LireHorodatage(m.Groups(1).Value),
            .Niveau = m.Groups(2).Value,
            .Categorie = m.Groups(3).Value
        }
        DecouperMessage(m.Groups(4).Value, entree)
        Return entree

    End Function

    Private Function LireHorodatage(texte As String) As DateTime?
        Dim valeur As DateTime
        If DateTime.TryParseExact(texte, GestionLog.FormatHorodatageLigne, CultureInfo.InvariantCulture,
                                  DateTimeStyles.None, valeur) Then
            Return valeur
        End If
        Return Nothing
    End Function

    ' « message | EX: … | STACK: … | INNER: … | INNER_STACK: … » : le message s'arrête au premier
    ' « | EX: » ; le reste est le détail de l'exception (piles remises sur plusieurs lignes).
    Private Sub DecouperMessage(texte As String, entree As EntreeJournal)

        Dim positionException As Integer = texte.IndexOf(GestionLog.SeparateurException, StringComparison.Ordinal)
        If positionException < 0 Then
            entree.Message = texte
            entree.Exception = String.Empty
            entree.DetailTechnique = String.Empty
            Return
        End If

        entree.Message = texte.Substring(0, positionException)
        Dim reste As String = texte.Substring(positionException + GestionLog.SeparateurException.Length)

        Dim finException As Integer = reste.IndexOf(" | ", StringComparison.Ordinal)
        entree.Exception = If(finException < 0, reste, reste.Substring(0, finException))

        entree.DetailTechnique = reste _
            .Replace(GestionLog.SeparateurPile, Environment.NewLine & Environment.NewLine & "Pile d'appels :" & Environment.NewLine) _
            .Replace(GestionLog.SeparateurPileInterne, Environment.NewLine & Environment.NewLine & "Pile d'appels (cause) :" & Environment.NewLine) _
            .Replace(GestionLog.SeparateurInterne, Environment.NewLine & Environment.NewLine & "Cause : ") _
            .Replace(" ⇢ ", Environment.NewLine)

    End Sub

End Module
