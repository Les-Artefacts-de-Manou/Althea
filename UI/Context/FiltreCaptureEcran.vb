' -------------------------------------------------------------------------------------------------
' Classe      : FiltreCaptureEcran
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Outil de documentation, en Debug seulement : capture d'écran au clavier, en PNG à la résolution
' réelle de l'application, pour les illustrations de Docs/Illustrations/Doc_UI (décision de
' Joëlle du 24/10/2026, après un essai de capture par l'écran jugé trop flou et aux couleurs
' fausses).
'
' Raccourcis :
' - Ctrl+Maj+F8 : le contenu — l'écran affiché dans Home (son UserControl), ou la fenêtre active
'   (dialogue, fiche) ; fichier « <Nom de la classe>.png ».
' - Ctrl+Maj+F7 : la fenêtre active entière (menu et bandeau de Home compris) ;
'   fichier « <Nom de la classe>_fenetre.png ».
'
' Remarques   :
' - Fichiers écrits dans Docs/Illustrations/Doc_UI/_captures du dépôt (dossier retrouvé en
'   remontant depuis l'exécutable jusqu'à Althea.vbproj ; à défaut, Images\Althea_captures) ;
'   une capture remplace la précédente du même écran. Les illustrations définitives sont choisies
'   et renommées ensuite : le dossier _captures n'est pas suivi par git.
' - PrintWindow (PW_RENDERFULLCONTENT) dessine la fenêtre elle-même, même si une autre la
'   recouvre, éditeurs de texte compris ; repli sur une copie de l'écran si l'appel échoue.
' - F12 est évité : Windows le réserve au débogueur.
' - Filtre de messages de l'application (comme SuiviActivite) : seuls les deux raccourcis sont
'   consommés ; aucune donnée lue, journal : le nom du fichier seulement.
' - Compilé en Debug seulement (#If DEBUG) : absent de la version livrée.
'
' Dépendances :
' - Home (pnlContent), GestionLog
' -------------------------------------------------------------------------------------------------

#If DEBUG Then

Option Strict On
Option Explicit On

Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices

Friend NotInheritable Class FiltreCaptureEcran
    Implements IMessageFilter

#Region "Constantes et API Windows"

    Private Const WM_KEYDOWN As Integer = &H100
    Private Const WM_SYSKEYDOWN As Integer = &H104
    Private Const PW_RENDERFULLCONTENT As UInteger = 2
    Private Const DWMWA_EXTENDED_FRAME_BOUNDS As Integer = 9

    Private Const TOUCHE_CONTENU As Keys = Keys.F8
    Private Const TOUCHE_FENETRE As Keys = Keys.F7
    Private Const FICHIER_PROJET As String = "Althea.vbproj"
    Private Const DOSSIER_CAPTURES As String = "Docs\Illustrations\Doc_UI\_captures"

    <StructLayout(LayoutKind.Sequential)>
    Private Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

    <DllImport("user32.dll")>
    Private Shared Function PrintWindow(hwnd As IntPtr, hdc As IntPtr, flags As UInteger) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function GetWindowRect(hwnd As IntPtr, ByRef rect As RECT) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("dwmapi.dll")>
    Private Shared Function DwmGetWindowAttribute(hwnd As IntPtr, attribut As Integer, ByRef valeur As RECT, taille As Integer) As Integer
    End Function

#End Region

#Region "Raccourcis"

    Public Function PreFilterMessage(ByRef m As Message) As Boolean Implements IMessageFilter.PreFilterMessage

        If m.Msg <> WM_KEYDOWN AndAlso m.Msg <> WM_SYSKEYDOWN Then Return False
        If (Control.ModifierKeys And (Keys.Control Or Keys.Shift)) <> (Keys.Control Or Keys.Shift) Then Return False

        Dim touche As Keys = CType(CInt(m.WParam.ToInt64() And &HFFFF), Keys)

        Select Case touche
            Case TOUCHE_CONTENU
                Capturer(fenetreEntiere:=False)
                Return True
            Case TOUCHE_FENETRE
                Capturer(fenetreEntiere:=True)
                Return True
        End Select

        Return False

    End Function

#End Region

#Region "Capture"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Capturer
    ' Rôle      : Capture la fenêtre active (ou l'écran affiché dans Home) et l'enregistre en PNG ;
    '             un bip confirme. Une erreur est journalisée, sans fenêtre (outil de test).
    ' -------------------------------------------------------------------------------------------------
    Private Shared Sub Capturer(fenetreEntiere As Boolean)

        Dim fenetre As Form = Form.ActiveForm
        If fenetre Is Nothing Then Return

        Dim cible As Control = fenetre
        Dim nom As String = fenetre.GetType().Name

        If fenetreEntiere Then
            nom &= "_fenetre"
        Else
            Dim homeForm As Home = TryCast(fenetre, Home)
            Dim vue As Control = If(homeForm Is Nothing, Nothing,
                                    homeForm.pnlContent.Controls.OfType(Of UserControl)().FirstOrDefault())
            If vue IsNot Nothing Then
                cible = vue
                nom = vue.GetType().Name
            End If
        End If

        Try
            Dim chemin As String = Path.Combine(DossierCaptures(), nom & ".png")
            Directory.CreateDirectory(Path.GetDirectoryName(chemin))

            Using image As Bitmap = CapturerControle(fenetre, cible)
                image.Save(chemin, ImageFormat.Png)
            End Using

            System.Media.SystemSounds.Asterisk.Play()
            GestionLog.EcrireLog($"Capture d'écran (Debug) : {Path.GetFileName(chemin)}.",
                                 GestionLog.LogLevel.Rapide, GestionLog.LogCategory.UI)

        Catch ex As Exception
            System.Media.SystemSounds.Hand.Play()
            GestionLog.EcrireLog("Capture d'écran (Debug) impossible.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : CapturerControle
    ' Rôle      : Image de la fenêtre dessinée par elle-même (PrintWindow), recadrée sur la cible :
    '             la fenêtre visible (sans les bordures invisibles de Windows) ou un contrôle.
    ' Remarques : le cadre visible donné par DWM n'est retenu que s'il tient dans le rectangle de
    '             la fenêtre (sinon, mise à l'échelle Windows : coordonnées non comparables) ; à
    '             défaut, la fenêtre est prise avec ses bordures invisibles.
    ' -------------------------------------------------------------------------------------------------
    Private Shared Function CapturerControle(fenetre As Form, cible As Control) As Bitmap

        Dim rf As RECT
        GetWindowRect(fenetre.Handle, rf)
        Dim largeur As Integer = rf.Right - rf.Left
        Dim hauteur As Integer = rf.Bottom - rf.Top

        ' Zone voulue, en coordonnées d'écran.
        Dim zone As Rectangle
        If cible Is fenetre Then
            Dim visible As RECT
            Dim rectFenetre As Rectangle = Rectangle.FromLTRB(rf.Left, rf.Top, rf.Right, rf.Bottom)
            zone = rectFenetre
            If DwmGetWindowAttribute(fenetre.Handle, DWMWA_EXTENDED_FRAME_BOUNDS, visible, Marshal.SizeOf(GetType(RECT))) = 0 Then
                Dim cadreVisible As Rectangle = Rectangle.FromLTRB(visible.Left, visible.Top, visible.Right, visible.Bottom)
                If rectFenetre.Contains(cadreVisible) Then zone = cadreVisible
            End If
        Else
            zone = cible.RectangleToScreen(cible.ClientRectangle)
        End If
        zone.Offset(-rf.Left, -rf.Top)
        zone.Intersect(New Rectangle(0, 0, largeur, hauteur))

        Using complete As New Bitmap(largeur, hauteur, PixelFormat.Format32bppArgb)

            Dim reussi As Boolean
            Using g As Graphics = Graphics.FromImage(complete)
                Dim hdc As IntPtr = g.GetHdc()
                Try
                    reussi = PrintWindow(fenetre.Handle, hdc, PW_RENDERFULLCONTENT)
                Finally
                    g.ReleaseHdc(hdc)
                End Try
            End Using

            If Not reussi Then
                ' Repli : copie de l'écran (la fenêtre doit alors être visible).
                Dim copie As New Bitmap(zone.Width, zone.Height, PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(copie)
                    g.CopyFromScreen(rf.Left + zone.Left, rf.Top + zone.Top, 0, 0, zone.Size)
                End Using
                Return copie
            End If

            Return complete.Clone(zone, PixelFormat.Format32bppArgb)

        End Using

    End Function

    ' Dossier des captures : Docs/Illustrations/Doc_UI/_captures du dépôt, retrouvé en remontant
    ' depuis l'exécutable ; à défaut, Images\Althea_captures.
    Private Shared Function DossierCaptures() As String

        Dim dossier As DirectoryInfo = New DirectoryInfo(AppContext.BaseDirectory)
        While dossier IsNot Nothing
            If File.Exists(Path.Combine(dossier.FullName, FICHIER_PROJET)) Then
                Return Path.Combine(dossier.FullName, DOSSIER_CAPTURES)
            End If
            dossier = dossier.Parent
        End While

        Return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Althea_captures")

    End Function

#End Region

End Class

#End If
