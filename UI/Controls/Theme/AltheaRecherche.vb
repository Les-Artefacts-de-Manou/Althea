' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaRecherche
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 10/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Champ de recherche du kit Althéa : champ arrondi blanc avec loupe et texte d'aide, contour
' sauge quand il a le focus.
'
' Responsabilités :
' - Dessiner le champ (normal, survolé, focus, désactivé) et la loupe
' - Héberger une TextBox sans bordure et relayer son texte et ses touches au code de l'écran
'
' Remarques  :
' - Contrôle composé : une TextBox Windows ne sait ni arrondir ses coins ni afficher d'icône.
' - Ce que les écrans utilisent est relayé : Text, TextChanged, KeyDown / KeyPress / KeyUp (les
'   mêmes arguments que la TextBox, donc e.SuppressKeyPress fonctionne), Clear(), SelectAll(),
'   TexteAide (PlaceholderText) et MaxLength.
' - Le focus donné au contrôle passe à la TextBox interne.
'
' Dépendances :
' - UITheme, AltheaDessin, AltheaFondParent
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

<DesignerCategory("Code")>
<ToolboxItem(True)>
<DefaultEvent("TextChanged")>
<Description("Champ de recherche Althéa : champ arrondi avec loupe.")>
Public Class AltheaRecherche
    Inherits UserControl

#Region "Constantes"

    Private Const Rayon As Single = 9.0F
    Private Const MargeGauche As Integer = 12
    Private Const TailleLoupe As Integer = 16
    Private Const EcartLoupeTexte As Integer = 8

#End Region

#Region "Variables privées"

    Private ReadOnly _saisie As New TextBox()
    Private _survolee As Boolean

#End Region

#Region "Initialisation"

    Public Sub New()

        SetStyle(ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or
                 ControlStyles.SupportsTransparentBackColor, True)

        MyBase.BackColor = Color.Transparent
        Font = New Font("Calibri", 11.0F)
        Size = New Size(320, 32)
        Padding = New Padding(0)

        _saisie.BorderStyle = BorderStyle.None
        _saisie.BackColor = Color.White
        _saisie.ForeColor = UITheme.ColorGrisFonce
        _saisie.Font = Font

        AddHandler _saisie.TextChanged, AddressOf Saisie_TextChanged
        AddHandler _saisie.KeyDown, AddressOf Saisie_KeyDown
        AddHandler _saisie.KeyPress, AddressOf Saisie_KeyPress
        AddHandler _saisie.KeyUp, AddressOf Saisie_KeyUp
        AddHandler _saisie.GotFocus, AddressOf Saisie_FocusChange
        AddHandler _saisie.LostFocus, AddressOf Saisie_FocusChange
        AddHandler _saisie.MouseEnter, AddressOf Saisie_MouseEnter
        AddHandler _saisie.MouseLeave, AddressOf Saisie_MouseLeave

        Controls.Add(_saisie)
        PlacerSaisie()

    End Sub

#End Region

#Region "Propriétés publiques"

    <Browsable(True)>
    <EditorBrowsable(EditorBrowsableState.Always)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    <Category("Althéa")>
    <Description("Texte saisi.")>
    Public Overrides Property Text As String
        Get
            Return _saisie.Text
        End Get
        Set(value As String)
            _saisie.Text = value
        End Set
    End Property

    <Category("Althéa")>
    <Description("Texte d'aide affiché tant que le champ est vide.")>
    <DefaultValue("")>
    Public Property TexteAide As String
        Get
            Return _saisie.PlaceholderText
        End Get
        Set(value As String)
            _saisie.PlaceholderText = If(value, String.Empty)
        End Set
    End Property

    <Category("Althéa")>
    <Description("Nombre maximal de caractères (0 = valeur par défaut de la TextBox).")>
    <DefaultValue(32767)>
    Public Property MaxLength As Integer
        Get
            Return _saisie.MaxLength
        End Get
        Set(value As Integer)
            _saisie.MaxLength = value
        End Set
    End Property

    ' Le fond reste transparent : seuls les coins laissent voir le parent.
    <Browsable(False)>
    <EditorBrowsable(EditorBrowsableState.Never)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Overrides Property BackColor As Color
        Get
            Return Color.Transparent
        End Get
        Set(value As Color)
            ' Ignoré volontairement.
        End Set
    End Property

#End Region

#Region "Méthodes publiques"

    Public Sub Clear()
        _saisie.Clear()
    End Sub

    Public Sub SelectAll()
        _saisie.SelectAll()
    End Sub

#End Region

#Region "Mise en page"

    ' TextBox centrée verticalement, après la loupe, sur toute la largeur restante.
    Private Sub PlacerSaisie()

        Dim x As Integer = MargeGauche + TailleLoupe + EcartLoupeTexte
        Dim h As Integer = _saisie.PreferredHeight
        _saisie.SetBounds(x, Math.Max(0, (Height - h) \ 2), Math.Max(10, Width - x - MargeGauche), h)

    End Sub

    Protected Overrides Sub OnLayout(e As LayoutEventArgs)
        MyBase.OnLayout(e)
        PlacerSaisie()
    End Sub

    Protected Overrides Sub OnFontChanged(e As EventArgs)
        MyBase.OnFontChanged(e)
        _saisie.Font = Font
        PlacerSaisie()
    End Sub

#End Region

#Region "Dessin"

    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        AltheaFondParent.Peindre(Me, e)
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : OnPaint
    ' Version    : V1.0.0
    ' Date       : 10/10/2026
    '
    ' Rôle       :
    ' Champ arrondi blanc, contour selon l'état, loupe à gauche.
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias

        Dim focus As Boolean = _saisie.Focused
        Dim epaisseur As Single = If(focus, 2.0F, 1.0F)
        Dim rect As New RectangleF(epaisseur / 2.0F, epaisseur / 2.0F, Width - epaisseur, Height - epaisseur)

        Dim bordure As Color
        If Not Enabled Then
            bordure = AltheaDessin.CouleurDesactiveBordure
        ElseIf focus Then
            bordure = UITheme.ColorSauge
        ElseIf _survolee Then
            bordure = UITheme.ColorSaugeClair
        Else
            bordure = UITheme.ColorBeigeFonce
        End If

        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(rect, Rayon)
            Using pinceau As New SolidBrush(If(Enabled, Color.White, AltheaDessin.CouleurDesactiveFond))
                g.FillPath(pinceau, chemin)
            End Using
            Using stylo As New Pen(bordure, epaisseur)
                g.DrawPath(stylo, chemin)
            End Using
        End Using

        DessinerLoupe(g, New RectangleF(MargeGauche, (Height - TailleLoupe) / 2.0F, TailleLoupe, TailleLoupe))

    End Sub

    Private Sub DessinerLoupe(g As Graphics, zone As RectangleF)

        Using stylo As New Pen(If(Enabled, UITheme.ColorSaugeFonce, AltheaDessin.CouleurDesactiveTexte), 1.8F)
            stylo.StartCap = LineCap.Round
            stylo.EndCap = LineCap.Round
            Dim diametre As Single = zone.Width * 0.62F
            g.DrawEllipse(stylo, zone.Left, zone.Top, diametre, diametre)
            g.DrawLine(stylo, zone.Left + diametre * 0.86F, zone.Top + diametre * 0.86F, zone.Right - 0.5F, zone.Bottom - 0.5F)
        End Using

    End Sub

#End Region

#Region "Relais de la TextBox"

    Private Sub Saisie_TextChanged(sender As Object, e As EventArgs)
        OnTextChanged(e)
    End Sub

    Private Sub Saisie_KeyDown(sender As Object, e As KeyEventArgs)
        OnKeyDown(e)
    End Sub

    Private Sub Saisie_KeyPress(sender As Object, e As KeyPressEventArgs)
        OnKeyPress(e)
    End Sub

    Private Sub Saisie_KeyUp(sender As Object, e As KeyEventArgs)
        OnKeyUp(e)
    End Sub

    Private Sub Saisie_FocusChange(sender As Object, e As EventArgs)
        Invalidate()
    End Sub

    Private Sub Saisie_MouseEnter(sender As Object, e As EventArgs)
        _survolee = True
        Invalidate()
    End Sub

    Private Sub Saisie_MouseLeave(sender As Object, e As EventArgs)
        _survolee = ClientRectangle.Contains(PointToClient(Cursor.Position))
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseEnter(e As EventArgs)
        _survolee = True
        Invalidate()
        MyBase.OnMouseEnter(e)
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        _survolee = ClientRectangle.Contains(PointToClient(Cursor.Position))
        Invalidate()
        MyBase.OnMouseLeave(e)
    End Sub

    ' Un clic n'importe où dans le champ (loupe, marges) donne le focus à la saisie.
    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        _saisie.Focus()
        MyBase.OnMouseDown(e)
    End Sub

    Protected Overrides Sub OnEnter(e As EventArgs)
        _saisie.Focus()
        MyBase.OnEnter(e)
    End Sub

    Protected Overrides Sub OnEnabledChanged(e As EventArgs)
        _saisie.BackColor = If(Enabled, Color.White, AltheaDessin.CouleurDesactiveFond)
        Invalidate()
        MyBase.OnEnabledChanged(e)
    End Sub

#End Region

End Class
