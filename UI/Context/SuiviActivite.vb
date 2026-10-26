' -------------------------------------------------------------------------------------------------
' Classe      : SuiviActivite
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 20/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Note l'activité de l'utilisateur (touche, clic, molette) dans la session, pour la fin de
' l'élévation après inactivité (audit A.5, décision de Joëlle : tant qu'on travaille, l'élévation
' reste).
'
' Remarques   :
' - Filtre de messages de l'application (IMessageFilter) : il voit passer les messages de TOUTES
'   les fenêtres d'Althéa, sans rien intercepter (PreFilterMessage rend toujours False).
' - Seuls les gestes volontaires comptent : un simple mouvement de souris ne prolonge pas
'   l'élévation.
' - Installé par Home (Application.AddMessageFilter) ; aucune donnée lue ni journalisée.
'
' Dépendances :
' - UserSession (Touch)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public NotInheritable Class SuiviActivite
    Implements IMessageFilter

    ' Messages Windows des gestes volontaires
    Private Const WM_KEYDOWN As Integer = &H100
    Private Const WM_SYSKEYDOWN As Integer = &H104
    Private Const WM_LBUTTONDOWN As Integer = &H201
    Private Const WM_RBUTTONDOWN As Integer = &H204
    Private Const WM_MOUSEWHEEL As Integer = &H20A

    Private ReadOnly _session As UserSession

    Public Sub New(session As UserSession)
        _session = session
    End Sub

    Public Function PreFilterMessage(ByRef m As Message) As Boolean Implements IMessageFilter.PreFilterMessage

        Select Case m.Msg
            Case WM_KEYDOWN, WM_SYSKEYDOWN, WM_LBUTTONDOWN, WM_RBUTTONDOWN, WM_MOUSEWHEEL
                _session.Touch()
        End Select

        Return False

    End Function

End Class
