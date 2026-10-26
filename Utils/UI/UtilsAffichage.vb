' -------------------------------------------------------------------------------------------------
' Module      : UtilsAffichage
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 16/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Gel et reprise de l'affichage d'une zone de l'écran, pour qu'une construction en plusieurs
' étapes (chargement d'un écran, de ses données, placement de ses panneaux) n'apparaisse qu'une
' fois terminée, d'un seul coup, au lieu de se mettre en place sous les yeux de l'utilisateur.
'
' Responsabilités :
' - Suspendre le dessin d'une zone et de tout son contenu (SuspendreDessin)
' - Le reprendre et redessiner la zone entière en une fois (ReprendreDessin)
'
' Utilisation :
' UtilsAffichage.SuspendreDessin(zone)
' Try
'     ' construction, chargement, placement…
' Finally
'     UtilsAffichage.ReprendreDessin(zone)
' End Try
'
' Remarques   :
' - Repose sur le message Windows WM_SETREDRAW, prévu pour cet usage. Toujours appeler
'   ReprendreDessin dans un Finally : une zone restée gelée ne se redessinerait plus.
' - SuspendLayout / ResumeLayout ne suffisent pas : ils retardent le placement des contrôles,
'   pas leur dessin ; Windows redessine les parties déjà créées pendant un chargement long.
' - Sans effet sur une zone dont la fenêtre n'est pas encore créée (rien n'y est dessiné).
' - Première utilisation : NavigationManager.Navigate (16/10/2026).
'
' Dépendances :
' - user32.dll (SendMessage, RedrawWindow)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Runtime.InteropServices

Public Module UtilsAffichage

#Region "Appels système"

    Private Const WM_SETREDRAW As Integer = &HB

    ' Options de RedrawWindow : invalider, effacer le fond, inclure le cadre et tous les enfants.
    Private Const RDW_INVALIDATE As UInteger = &H1UI
    Private Const RDW_ERASE As UInteger = &H4UI
    Private Const RDW_ALLCHILDREN As UInteger = &H80UI
    Private Const RDW_FRAME As UInteger = &H400UI

    <DllImport("user32.dll")>
    Private Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Private Function RedrawWindow(hWnd As IntPtr, lprcUpdate As IntPtr, hrgnUpdate As IntPtr, flags As UInteger) As Boolean
    End Function

#End Region

#Region "Méthodes publiques"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SuspendreDessin
    ' Version    : V1.0.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Gèle l'affichage d'une zone et de tout ce qu'elle contient jusqu'à ReprendreDessin.
    '
    ' Paramètres :
    ' - zone : contrôle à geler (sans effet si Nothing ou si sa fenêtre n'existe pas encore)
    ' -------------------------------------------------------------------------------------------------
    Public Sub SuspendreDessin(zone As Control)

        If zone Is Nothing OrElse Not zone.IsHandleCreated Then Return

        SendMessage(zone.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ReprendreDessin
    ' Version    : V1.0.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Rétablit l'affichage d'une zone gelée et la redessine entièrement, contenu compris, en une fois.
    '
    ' Paramètres :
    ' - zone : contrôle gelé par SuspendreDessin (sans effet si Nothing ou sans fenêtre)
    ' -------------------------------------------------------------------------------------------------
    Public Sub ReprendreDessin(zone As Control)

        If zone Is Nothing OrElse Not zone.IsHandleCreated Then Return

        SendMessage(zone.Handle, WM_SETREDRAW, New IntPtr(1), IntPtr.Zero)
        RedrawWindow(zone.Handle, IntPtr.Zero, IntPtr.Zero, RDW_ERASE Or RDW_FRAME Or RDW_INVALIDATE Or RDW_ALLCHILDREN)

    End Sub

#End Region

End Module
