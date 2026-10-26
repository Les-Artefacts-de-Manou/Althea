' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaListe
' Projet     : Althéa
' Version    : V1.3.0
' Date       : 24/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Liste déroulante du kit Althéa : champ arrondi blanc, étiquette intégrée (« Suivi : En cours »)
' et flèche sauge, dessinés par le code.
'
' Responsabilités :
' - Dessiner le champ fermé (normal, survolé, focus, désactivé) avec son étiquette
' - Dessiner les éléments de la liste ouverte aux couleurs du kit
' - Laisser un écran dessiner lui-même ses éléments (icônes…) quand il le demande
'
' Remarques  :
' - Hérite de ComboBox : Items, DataSource, SelectedIndex, SelectedIndexChanged et le code
'   existant restent inchangés. La migration d'un écran se limite au type déclaré.
' - Le champ fermé est entièrement repeint sur WM_PAINT, dans un tampon : la ComboBox Windows ne
'   sait ni arrondir ses coins ni afficher une étiquette, et ignore BackColor = Transparent.
' - DessinItemsPersonnalise = True : l'événement DrawItem de l'écran dessine les éléments, y
'   compris l'élément sélectionné dans le champ fermé (après l'étiquette).
' - Prévue pour DropDownList (liste fermée, sans saisie libre), le cas de tous les filtres.
' - Désactivée (V1.1.0, 14/10/2026) : dessinée comme un champ en lecture seule — valeur lisible
'   sur fond beige (UITheme.ColorChampLectureSeule), flèche estompée.
' - V1.2.0 (21/10/2026) : un élément ElementListePastille avec une teinte s'affiche en pastille
'   colorée, dans la liste ouverte et dans le champ fermé (filtres aux couleurs des pastilles,
'   demande de Joëlle) ; les autres éléments de la liste s'alignent sur le texte des pastilles.
' - V1.3.0 (24/10/2026) : la liste ouverte s'élargit à son plus long élément (libellés suivis de
'   leur code lisibles en entier au moment du choix).
'
' Dépendances :
' - UITheme, AltheaDessin, AltheaFondParent
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.Runtime.InteropServices

<DesignerCategory("Code")>
<ToolboxItem(True)>
<Description("Liste déroulante Althéa : champ arrondi avec étiquette intégrée.")>
Public Class AltheaListe
    Inherits ComboBox

#Region "API Windows"

    Private Const WM_PAINT As Integer = &HF

    <StructLayout(LayoutKind.Sequential)>
    Private Structure RECT
        Public Left, Top, Right, Bottom As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure PAINTSTRUCT
        Public hdc As IntPtr
        Public fErase As Integer
        Public rcPaint As RECT
        Public fRestore As Integer
        Public fIncUpdate As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=32)>
        Public rgbReserved As Byte()
    End Structure

    <DllImport("user32.dll")>
    Private Shared Function BeginPaint(hWnd As IntPtr, ByRef lpPaint As PAINTSTRUCT) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Private Shared Function EndPaint(hWnd As IntPtr, ByRef lpPaint As PAINTSTRUCT) As Boolean
    End Function

#End Region

#Region "Constantes"

    Private Const Rayon As Single = 8.0F
    Private Const MargeInterne As Integer = 10
    Private Const LargeurFleche As Integer = 22

#End Region

#Region "Variables privées"

    Private _etiquette As String = String.Empty
    Private _dessinItemsPersonnalise As Boolean
    Private _survolee As Boolean
    Private _policeEtiquette As Font = New Font("Calibri", 9.75F, FontStyle.Bold)

#End Region

#Region "Initialisation"

    Public Sub New()

        DropDownStyle = ComboBoxStyle.DropDownList
        FlatStyle = FlatStyle.Flat
        DrawMode = DrawMode.OwnerDrawFixed
        ItemHeight = 24
        IntegralHeight = False
        MaxDropDownItems = 12
        Font = New Font("Calibri", 11.0F)
        ForeColor = UITheme.ColorGrisFonce
        BackColor = Color.White

    End Sub

#End Region

#Region "Propriétés publiques"

    <Category("Althéa")>
    <Description("Étiquette affichée dans le champ, avant la valeur (vide = pas d'étiquette).")>
    <DefaultValue("")>
    Public Property Etiquette As String
        Get
            Return _etiquette
        End Get
        Set(value As String)
            _etiquette = If(value, String.Empty)
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("True : l'écran dessine les éléments lui-même (événement DrawItem).")>
    <DefaultValue(False)>
    Public Property DessinItemsPersonnalise As Boolean
        Get
            Return _dessinItemsPersonnalise
        End Get
        Set(value As Boolean)
            _dessinItemsPersonnalise = value
            Invalidate()
        End Set
    End Property

#End Region

#Region "Dessin du champ fermé"

    Protected Overrides Sub WndProc(ByRef m As Message)

        If m.Msg = WM_PAINT AndAlso DropDownStyle = ComboBoxStyle.DropDownList Then
            PeindreChampFerme(m.HWnd)
            m.Result = IntPtr.Zero
            Return
        End If

        MyBase.WndProc(m)

    End Sub

    Private Sub PeindreChampFerme(hWnd As IntPtr)

        Dim ps As New PAINTSTRUCT With {.rgbReserved = New Byte(31) {}}
        Dim hdc As IntPtr = BeginPaint(hWnd, ps)

        Try
            Using tampon As BufferedGraphics = BufferedGraphicsManager.Current.Allocate(hdc, ClientRectangle)
                DessinerChamp(tampon.Graphics)
                tampon.Render()
            End Using
        Finally
            EndPaint(hWnd, ps)
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerChamp
    ' Version    : V1.0.0
    ' Date       : 10/10/2026
    '
    ' Rôle       :
    ' Fond du parent aux coins, champ arrondi, étiquette, valeur sélectionnée et flèche.
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub DessinerChamp(g As Graphics)

        AltheaFondParent.Peindre(Me, New PaintEventArgs(g, ClientRectangle))

        g.SmoothingMode = SmoothingMode.AntiAlias

        Dim focus As Boolean = Focused OrElse DroppedDown
        Dim epaisseur As Single = If(focus, 2.0F, 1.0F)
        Dim rect As New RectangleF(epaisseur / 2.0F, epaisseur / 2.0F, Width - epaisseur, Height - epaisseur)

        Dim bordure As Color
        If Not Enabled Then
            bordure = UITheme.ColorBeigeFonce
        ElseIf focus Then
            bordure = UITheme.ColorSauge
        ElseIf _survolee Then
            bordure = UITheme.ColorSaugeClair
        Else
            bordure = UITheme.ColorBeigeFonce
        End If

        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(rect, Rayon)
            Using pinceau As New SolidBrush(If(Enabled, Color.White, UITheme.ColorChampLectureSeule))
                g.FillPath(pinceau, chemin)
            End Using
            Using stylo As New Pen(bordure, epaisseur)
                g.DrawPath(stylo, chemin)
            End Using
        End Using

        g.SmoothingMode = SmoothingMode.None

        Dim x As Integer = MargeInterne
        Dim zoneUtile As New Rectangle(0, 0, Width - LargeurFleche, Height)

        If _etiquette.Length > 0 Then
            Dim largeur As Integer = TextRenderer.MeasureText(g, _etiquette, _policeEtiquette, Size.Empty, TextFormatFlags.NoPadding).Width
            TextRenderer.DrawText(g, _etiquette, _policeEtiquette,
                                  New Rectangle(x, 0, Math.Max(0, zoneUtile.Right - x), Height),
                                  UITheme.ColorSaugeFonce,
                                  TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.SingleLine Or TextFormatFlags.NoPadding)
            x += largeur + 8
        End If

        Dim zoneValeur As New Rectangle(x, 0, Math.Max(0, zoneUtile.Right - x), Height)
        ' Désactivée = lecture seule : la valeur reste lisible, seul le fond se teinte.
        Dim couleurTexte As Color = ForeColor

        If _dessinItemsPersonnalise AndAlso SelectedIndex >= 0 Then
            ' L'écran dessine la valeur comme dans la liste (icône comprise), sur fond blanc.
            Dim zoneItem As New Rectangle(zoneValeur.X - 2, (Height - ItemHeight) \ 2, zoneValeur.Width, ItemHeight)
            MyBase.OnDrawItem(New DrawItemEventArgs(g, Font, zoneItem, SelectedIndex,
                                                    DrawItemState.ComboBoxEdit Or DrawItemState.NoFocusRect,
                                                    couleurTexte, If(Enabled, Color.White, UITheme.ColorChampLectureSeule)))
        ElseIf SelectedIndex >= 0 Then
            Dim element As ElementListePastille = TryCast(SelectedItem, ElementListePastille)
            If element IsNot Nothing AndAlso element.Teinte.HasValue Then
                ' Pastille colorée ; décalée à gauche pour que son texte suive l'étiquette.
                Dim zonePastille As New Rectangle(zoneValeur.X - UtilsDataGrid.RetraitTextePastille + 9, 0,
                                                  zoneValeur.Width + UtilsDataGrid.RetraitTextePastille - 9, Height)
                UtilsDataGrid.DessinerPastilleZone(g, zonePastille, element.Libelle, element.Teinte.Value, element.Contour)
            Else
                TextRenderer.DrawText(g, GetItemText(SelectedItem), Font, zoneValeur, couleurTexte,
                                      TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.SingleLine Or
                                      TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPadding)
            End If
        End If

        DessinerFleche(g, New Rectangle(Width - LargeurFleche - 4, 0, LargeurFleche, Height))

    End Sub

    Private Sub DessinerFleche(g As Graphics, zone As Rectangle)

        g.SmoothingMode = SmoothingMode.AntiAlias

        Dim cx As Single = zone.Left + zone.Width / 2.0F
        Dim cy As Single = zone.Top + zone.Height / 2.0F

        Using stylo As New Pen(If(Enabled, UITheme.ColorSaugeFonce, UITheme.ColorSaugeClair), 1.8F)
            stylo.StartCap = LineCap.Round
            stylo.EndCap = LineCap.Round
            stylo.LineJoin = LineJoin.Round
            g.DrawLines(stylo, {New PointF(cx - 4.5F, cy - 2.0F), New PointF(cx, cy + 2.5F), New PointF(cx + 4.5F, cy - 2.0F)})
        End Using

    End Sub

#End Region

#Region "Dessin de la liste ouverte"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : OnDrawItem
    ' Version    : V1.0.0
    ' Date       : 10/10/2026
    '
    ' Rôle       :
    ' Éléments de la liste ouverte : fond Gris-vert clair pour l'élément pointé, texte Gris foncé.
    '
    ' Remarques  :
    ' - DessinItemsPersonnalise : l'événement DrawItem de l'écran prend la main.
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub OnDrawItem(e As DrawItemEventArgs)

        If _dessinItemsPersonnalise Then
            MyBase.OnDrawItem(e)
            Return
        End If

        If e.Index < 0 Then Return

        Dim selectionne As Boolean = (e.State And DrawItemState.Selected) = DrawItemState.Selected

        Using pinceau As New SolidBrush(If(selectionne, UITheme.ColorGrisVertClair, Color.White))
            e.Graphics.FillRectangle(pinceau, e.Bounds)
        End Using

        Dim zone As Rectangle = e.Bounds
        zone.Inflate(-6, 0)

        Dim element As ElementListePastille = TryCast(Items(e.Index), ElementListePastille)
        If element IsNot Nothing AndAlso element.Teinte.HasValue Then
            UtilsDataGrid.DessinerPastilleZone(e.Graphics, zone, element.Libelle, element.Teinte.Value, element.Contour)
            Return
        End If

        ' Dans une liste qui contient des pastilles, le texte simple s'aligne sur le leur.
        If ContientDesPastilles() Then
            zone = New Rectangle(zone.X + UtilsDataGrid.RetraitTextePastille, zone.Y,
                                 Math.Max(0, zone.Width - UtilsDataGrid.RetraitTextePastille), zone.Height)
        End If

        TextRenderer.DrawText(e.Graphics, GetItemText(Items(e.Index)), Font, zone,
                              If(selectionne, UITheme.ColorSaugeTresFonce, UITheme.ColorGrisFonce),
                              TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.SingleLine Or
                              TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPadding)

    End Sub

    ' Au moins un élément affiché en pastille (ElementListePastille avec teinte).
    Private Function ContientDesPastilles() As Boolean
        For Each item As Object In Items
            Dim element As ElementListePastille = TryCast(item, ElementListePastille)
            If element IsNot Nothing AndAlso element.Teinte.HasValue Then Return True
        Next
        Return False
    End Function

#End Region

#Region "Événements"

    Protected Overrides Sub OnMouseEnter(e As EventArgs)
        _survolee = True
        Invalidate()
        MyBase.OnMouseEnter(e)
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        _survolee = False
        Invalidate()
        MyBase.OnMouseLeave(e)
    End Sub

    Protected Overrides Sub OnGotFocus(e As EventArgs)
        Invalidate()
        MyBase.OnGotFocus(e)
    End Sub

    Protected Overrides Sub OnLostFocus(e As EventArgs)
        Invalidate()
        MyBase.OnLostFocus(e)
    End Sub

    ' La liste ouverte prend la largeur de son plus long élément (jamais moins que le champ) : un
    ' libellé long — nom suivi de son code, par exemple — se lit en entier au moment du choix.
    ' Les éléments dessinés par l'écran (DessinItemsPersonnalise) gardent la largeur du champ.
    Protected Overrides Sub OnDropDown(e As EventArgs)

        If Not _dessinItemsPersonnalise Then
            Dim retrait As Integer = If(ContientDesPastilles(), UtilsDataGrid.RetraitTextePastille, 0)
            Dim largeurMax As Integer = Width
            For Each item As Object In Items
                Dim largeur As Integer = TextRenderer.MeasureText(GetItemText(item), Font).Width + retrait + 12 +
                                         SystemInformation.VerticalScrollBarWidth
                largeurMax = Math.Max(largeurMax, largeur)
            Next
            DropDownWidth = largeurMax
        End If

        MyBase.OnDropDown(e)

    End Sub

    Protected Overrides Sub OnDropDownClosed(e As EventArgs)
        Invalidate()
        MyBase.OnDropDownClosed(e)
    End Sub

    Protected Overrides Sub OnSelectedIndexChanged(e As EventArgs)
        Invalidate()
        MyBase.OnSelectedIndexChanged(e)
    End Sub

    Protected Overrides Sub OnEnabledChanged(e As EventArgs)
        Invalidate()
        MyBase.OnEnabledChanged(e)
    End Sub

#End Region

End Class
