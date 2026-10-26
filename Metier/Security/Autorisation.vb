' -------------------------------------------------------------------------------------------------
' Module      : Autorisation
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' LA garde des droits côté métier (audit C.1) : une méthode métier sensible commence par
' Autorisation.Exiger, quel que soit l'écran qui l'appelle. Masquer ou griser un bouton ne protège
' rien ; c'est ici que le refus est décidé.
'
' Responsabilités :
' - Refuser une opération si le rôle courant est insuffisant (Exiger)
' - Refuser une opération tracée si aucun utilisateur n'est identifié (ExigerUtilisateur)
' - Dire si un rôle est atteint, sans refuser (Possede)
'
' Important   :
' - Le refus est une UnauthorizedAccessException dont le message est rédigé pour l'utilisatrice :
'   l'écran l'affiche tel quel (DialogChoix), sans détail technique.
' - Le refus n'est pas journalisé ici : l'appelant journalise s'il y a lieu (identifiants seulement).
'
' Dépendances :
' - ContexteOperation, AppRole
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module Autorisation

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Exiger
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Refuse l'opération si le contexte manque ou si son rôle courant est inférieur au rôle exigé.
    '
    ' Paramètres :
    ' - contexte     : qui fait l'opération
    ' - roleMinimum  : rôle exigé (la hiérarchie User < SuperUser < Admin s'applique)
    ' - messageRefus : message affiché à l'utilisatrice en cas de refus
    ' -------------------------------------------------------------------------------------------------
    Public Sub Exiger(contexte As ContexteOperation, roleMinimum As AppRole, messageRefus As String)

        If contexte Is Nothing OrElse contexte.RoleCourant < roleMinimum Then
            Throw New UnauthorizedAccessException(messageRefus)
        End If

    End Sub

    ' Vrai si le contexte atteint le rôle exigé (choix d'affichage ou de message, sans refus).
    Public Function Possede(contexte As ContexteOperation, roleMinimum As AppRole) As Boolean
        Return contexte IsNot Nothing AndAlso contexte.RoleCourant >= roleMinimum
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ExigerUtilisateur
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Refuse une opération dont l'auteur est enregistré (annulation, utilisation d'avoir…) si aucun
    ' utilisateur n'est identifié.
    ' -------------------------------------------------------------------------------------------------
    Public Sub ExigerUtilisateur(contexte As ContexteOperation)

        If contexte Is Nothing OrElse contexte.IdUtilisateur <= 0 Then
            Throw New UnauthorizedAccessException("Utilisateur non identifié : reconnectez-vous, puis recommencez.")
        End If

    End Sub

End Module
