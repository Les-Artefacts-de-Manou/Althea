' -------------------------------------------------------------------------------------------------
' Classe     : AltheaBarreTitre
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 15/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Barre de titre Windows aux couleurs du kit : fond beige clair (celui des fenêtres), titre sauge
' très foncé, bordure sauge clair. Remplace la couleur d'accentuation de Windows (souvent bleue)
' qui tranchait avec l'application (décision de Joëlle, 15/10/2026).
'
' Utilisation :
' - AltheaBarreTitre.Appliquer(Me) dans le constructeur ou au chargement d'une fenêtre ;
'   AltheaFondFenetre.Appliquer(Me) l'appelle déjà pour les fenêtres reprises.
'
' Remarques  :
' - On demande seulement à Windows (DWM) de peindre sa propre barre dans nos couleurs : déplacement,
'   agrandissement, ancrage, boutons réduire / agrandir / fermer, barre des tâches et accessibilité
'   restent ceux de Windows. Rien n'est redessiné par l'application.
' - Disponible à partir de Windows 11 (build 22000). Sur un Windows plus ancien, rien n'est demandé
'   et la barre garde l'aspect choisi par Windows : ce n'est pas une erreur.
' - Appliqué à chaque création du handle de la fenêtre (évènement HandleCreated) : WinForms recrée
'   parfois ce handle (changement de certaines propriétés), les couleurs sont alors reposées.
' - Couleurs lues dans UITheme : aucune valeur codée ici.
'
' Dépendances :
' - UITheme, GestionLog
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Runtime.InteropServices

Public NotInheritable Class AltheaBarreTitre

#Region "Constantes"

    ' Attributs de fenêtre DWM (dwmapi.h, Windows 11).
    Private Const DWMWA_BORDER_COLOR As Integer = 34
    Private Const DWMWA_CAPTION_COLOR As Integer = 35
    Private Const DWMWA_TEXT_COLOR As Integer = 36

    ' Première version de Windows 11, qui accepte les couleurs de barre de titre.
    Private Const BuildWindows11 As Integer = 22000

#End Region

#Region "Appels système"

    <DllImport("dwmapi.dll")>
    Private Shared Function DwmSetWindowAttribute(hwnd As IntPtr,
                                                  attribut As Integer,
                                                  ByRef valeur As Integer,
                                                  taille As Integer) As Integer
    End Function

#End Region

#Region "Initialisation"

    ' Classe utilitaire : pas d'instance.
    Private Sub New()
    End Sub

#End Region

#Region "Méthodes publiques"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Appliquer
    ' Version    : V1.0.0
    ' Date       : 15/10/2026
    '
    ' Rôle       :
    ' Donne à la barre de titre d'une fenêtre les couleurs du kit, dès la création de son handle
    ' (donc avant le premier affichage) et à chaque recréation.
    '
    ' Paramètres :
    ' - fenetre : fenêtre concernée (sans effet si Nothing)
    '
    ' Remarques  :
    ' - Peut être appelée plusieurs fois sans effet de bord (gestionnaire retiré puis ajouté).
    ' -------------------------------------------------------------------------------------------------
    Public Shared Sub Appliquer(fenetre As Form)

        If fenetre Is Nothing Then Return

        RemoveHandler fenetre.HandleCreated, AddressOf Fenetre_HandleCreated
        AddHandler fenetre.HandleCreated, AddressOf Fenetre_HandleCreated

        If fenetre.IsHandleCreated Then Teinter(fenetre)

    End Sub

#End Region

#Region "Mise en couleur"

    Private Shared Sub Fenetre_HandleCreated(sender As Object, e As EventArgs)

        Dim fenetre As Form = TryCast(sender, Form)
        If fenetre IsNot Nothing Then Teinter(fenetre)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Teinter
    ' Rôle       : Pose les trois couleurs de la barre (fond, titre, bordure) sur Windows 11 et plus.
    ' -------------------------------------------------------------------------------------------------
    Private Shared Sub Teinter(fenetre As Form)

        If Not OperatingSystem.IsWindowsVersionAtLeast(10, 0, BuildWindows11) Then Return

        Dim reussi As Boolean =
            DefinirCouleur(fenetre.Handle, DWMWA_CAPTION_COLOR, UITheme.ColorBeigeClair) And
            DefinirCouleur(fenetre.Handle, DWMWA_TEXT_COLOR, UITheme.ColorSaugeTresFonce) And
            DefinirCouleur(fenetre.Handle, DWMWA_BORDER_COLOR, UITheme.ColorSaugeClair)

        ' Refus inattendu sur Windows 11 : la fenêtre reste utilisable avec la barre de Windows.
        ' Tracé pour diagnostic, sans gêner l'utilisateur.
        If Not reussi Then
            GestionLog.EcrireLog(
                $"AltheaBarreTitre : couleurs de barre de titre refusées par Windows ({fenetre.Name}).",
                GestionLog.LogLevel.Complet,
                GestionLog.LogCategory.UI)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : DefinirCouleur
    ' Rôle       : Transmet une couleur à DWM au format COLORREF (0x00BBGGRR).
    ' Retour     : True si Windows l'a acceptée.
    ' -------------------------------------------------------------------------------------------------
    Private Shared Function DefinirCouleur(hwnd As IntPtr, attribut As Integer, couleur As Color) As Boolean

        Dim colorRef As Integer = CInt(couleur.R) Or (CInt(couleur.G) << 8) Or (CInt(couleur.B) << 16)

        Return DwmSetWindowAttribute(hwnd, attribut, colorRef, Marshal.SizeOf(Of Integer)()) = 0

    End Function

#End Region

End Class
