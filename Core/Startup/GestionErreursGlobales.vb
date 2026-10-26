' -------------------------------------------------------------------------------------------------
' Module      : GestionErreursGlobales
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 20/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Dernier filet pour les erreurs que rien n'a interceptées : elles sont journalisées (détail
' technique) et l'utilisateur reçoit un message clair, sans pile .NET ni nom de table.
'
' Remarques   :
' - Branché dans Program.Main, avant tout le reste (audit § 3.3 : une erreur non interceptée
'   affichait la pile .NET et n'était pas journalisée).
' - Erreur de l'interface (Application.ThreadException) : message, puis l'application continue —
'   la dernière action n'a peut-être pas abouti, l'utilisateur est invité à la vérifier.
' - Erreur hors de l'interface (AppDomain.UnhandledException) : .NET va fermer l'application ; on
'   journalise et on prévient, rien de plus n'est possible.
' - Garde contre la réentrance : si l'affichage du message échoue lui-même, repli sur une boîte
'   Windows simple, jamais une boucle d'erreurs.
' - Le message ne contient aucun détail technique (règle de sécurité du projet) : le détail est au
'   journal.
'
' Dépendances :
' - GestionLog, DialogChoix
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Threading

Public Module GestionErreursGlobales

    Private Const TITRE_MESSAGE As String = "Erreur inattendue"

    ' 1 pendant l'affichage d'un message (Interlocked : une erreur peut arriver d'un autre thread).
    Private _messageEnCours As Integer

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Installer
    ' Version    : V1.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       :
    ' Branche les deux filets. À appeler dans Main, avant la création de toute fenêtre.
    ' -------------------------------------------------------------------------------------------------
    Public Sub Installer()

        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException)
        AddHandler Application.ThreadException, AddressOf SurErreurInterface
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf SurErreurFatale

    End Sub

    Private Sub SurErreurInterface(sender As Object, e As ThreadExceptionEventArgs)

        GestionLog.EcrireException("Erreur non interceptée (interface).", e.Exception,
                                   GestionLog.LogLevel.Succinct, GestionLog.LogCategory.General)

        Prevenir("Une erreur inattendue s'est produite. Votre dernière action n'a peut-être pas abouti : " &
                 "vérifiez-la, puis continuez votre travail." & Environment.NewLine & Environment.NewLine &
                 "Si l'erreur se répète, notez ce que vous faisiez à ce moment-là. " &
                 "Le détail technique est dans le journal (Administration › Journaux).")

    End Sub

    Private Sub SurErreurFatale(sender As Object, e As UnhandledExceptionEventArgs)

        GestionLog.EcrireException("Erreur non interceptée (hors interface, fermeture).",
                                   TryCast(e.ExceptionObject, Exception),
                                   GestionLog.LogLevel.Succinct, GestionLog.LogCategory.General)

        If e.IsTerminating Then
            Prevenir("Une erreur grave oblige Althéa à se fermer. Les données déjà enregistrées ne sont pas " &
                     "perdues ; relancez Althéa et vérifiez votre dernière saisie." & Environment.NewLine & Environment.NewLine &
                     "Le détail technique est dans le journal (Administration › Journaux).")
        End If

    End Sub

    ' Affiche le message du kit ; boîte Windows simple si celui-ci ne peut pas s'afficher.
    Private Sub Prevenir(message As String)

        If Interlocked.Exchange(_messageEnCours, 1) = 1 Then Return

        Try
            DialogChoix.Erreur(message, TITRE_MESSAGE)
        Catch
            Try
                MessageBox.Show(message, TITRE_MESSAGE, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Catch
                ' Plus aucun moyen d'afficher : l'erreur est déjà au journal.
            End Try
        Finally
            Interlocked.Exchange(_messageEnCours, 0)
        End Try

    End Sub

End Module
