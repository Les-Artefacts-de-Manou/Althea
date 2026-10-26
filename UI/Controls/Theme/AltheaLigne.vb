' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaLigne
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 11/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Ligne cliquable du kit Althéa : icône en pastille, titre, aperçu sur une ligne et action
' (« Ouvrir ») à droite. Toute la ligne réagit au survol et au clic (maquette « Suivi clinique A »).
'
' Remarques  :
' - Hérite de Button : Click, Tag, Text (= titre), Enabled, focus clavier et touche Entrée/Espace
'   restent ceux d'un bouton ; les écrans existants qui manipulent des Button n'ont rien à changer.
' - Image = icône (blanche, posée sur la pastille Sauge).
' - Description vide : TexteVide est affiché en gris italique.
' - ButtonBase se déclare « Opaque » : désactivé pour laisser voir le parent (cf. AltheaBouton).
'
' Dépendances :
' - UITheme, AltheaDessin
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

<DesignerCategory("Code")>
<ToolboxItem(True)>
<Description("Ligne cliquable Althéa : icône, titre, aperçu et action.")>
Public Class AltheaLigne
    Inherits Button

#Region "Constantes de dessin"

    Private Const MARGE As Integer = 10
    Private Const TAILLE_PASTILLE As Integer = 36
    ' Taille native des icônes de zones (Assets\Boutons_suivi) : pas de mise à l'échelle floue.
    Private Const TAILLE_ICONE As Integer = 24
    Private Const ECART As Integer = 14

#End Region

#Region "Variables privées"

    Private _description As String = String.Empty
    Private _texteVide As String = "Vide"
    Private _texteAction As String = "Ouvrir"
    Private _largeurTitre As Integer = 180
    Private _separateur As Boolean = True
    Private _survol As Boolean
    Private _enfonce As Boolean

    Private ReadOnly _policeDescription As New Font("Calibri", 10.5F)
    Private ReadOnly _policeVide As New Font("Calibri", 10.5F, FontStyle.Italic)

#End Region

#Region "Initialisation"

    Public Sub New()

        SetStyle(ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or
                 ControlStyles.SupportsTransparentBackColor, True)
        SetStyle(ControlStyles.Opaque, False)

        FlatStyle = FlatStyle.Flat
        FlatAppearance.BorderSize = 0
        UseVisualStyleBackColor = False
        MyBase.BackColor = Color.Transparent
        Font = New Font("Calibri", 11.25F, FontStyle.Bold)
        ForeColor = UITheme.ColorSaugeTresFonce
        Cursor = Cursors.Hand
        Size = New Size(600, 54)

    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            _policeDescription.Dispose()
            _policeVide.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#End Region

#Region "Propriétés publiques"

    <Category("Althéa")>
    <Description("Aperçu affiché après le titre, sur une ligne (tronqué si nécessaire).")>
    <DefaultValue("")>
    Public Property Description As String
        Get
            Return _description
        End Get
        Set(value As String)
            _description = If(value, String.Empty)
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Texte affiché à la place de l'aperçu lorsque Description est vide.")>
    <DefaultValue("Vide")>
    Public Property TexteVide As String
        Get
            Return _texteVide
        End Get
        Set(value As String)
            _texteVide = If(value, String.Empty)
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Libellé de l'action, à droite (vide = pas d'action affichée).")>
    <DefaultValue("Ouvrir")>
    Public Property TexteAction As String
        Get
            Return _texteAction
        End Get
        Set(value As String)
            _texteAction = If(value, String.Empty)
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Largeur réservée au titre, pour aligner les aperçus d'une ligne à l'autre.")>
    <DefaultValue(180)>
    Public Property LargeurTitre As Integer
        Get
            Return _largeurTitre
        End Get
        Set(value As Integer)
            _largeurTitre = Math.Max(40, value)
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Trace un filet sous la ligne (False pour la dernière ligne d'une liste).")>
    <DefaultValue(True)>
    Public Property Separateur As Boolean
        Get
            Return _separateur
        End Get
        Set(value As Boolean)
            _separateur = value
            Invalidate()
        End Set
    End Property

    ' Toujours transparent : la ligne laisse voir la carte qui la contient.
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

    ''' <summary>Abscisse où commence l'aperçu (sert d'ancrage à une fenêtre d'aperçu détaillé).</summary>
    <Browsable(False)>
    Public ReadOnly Property DebutDescription As Integer
        Get
            Return MARGE + TAILLE_PASTILLE + ECART + _largeurTitre + ECART
        End Get
    End Property

#End Region

#Region "Dessin"

    Protected Overrides Sub OnPaint(pevent As PaintEventArgs)

        Dim g As Graphics = pevent.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.InterpolationMode = InterpolationMode.HighQualityBicubic

        Dim actif As Boolean = Enabled
        Dim survole As Boolean = _survol AndAlso actif

        ' Fond de survol (ou d'appui) sur toute la ligne.
        If survole OrElse (Focused AndAlso ShowFocusCues) Then
            Dim teinte As Color = If(_enfonce AndAlso survole,
                                     AltheaDessin.Melanger(UITheme.ColorGrisVertClair, UITheme.ColorSaugeClair, 0.5F),
                                     UITheme.ColorGrisVertClair)
            Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(New RectangleF(0.5F, 1.5F, Width - 1.0F, Height - 3.0F), 10.0F)
                Using pinceau As New SolidBrush(teinte)
                    g.FillPath(pinceau, chemin)
                End Using
                If Focused AndAlso ShowFocusCues AndAlso Not survole Then
                    Using stylo As New Pen(UITheme.ColorSaugeClair)
                        g.DrawPath(stylo, chemin)
                    End Using
                End If
            End Using
        ElseIf _separateur Then
            Using stylo As New Pen(UITheme.ColorBeige)
                g.DrawLine(stylo, MARGE, Height - 1, Width - MARGE, Height - 1)
            End Using
        End If

        ' Pastille et icône.
        Dim yPastille As Integer = (Height - TAILLE_PASTILLE) \ 2
        Dim pastille As New RectangleF(MARGE, yPastille, TAILLE_PASTILLE, TAILLE_PASTILLE)
        Dim couleurPastille As Color = If(Not actif, AltheaDessin.CouleurDesactiveBordure,
                                          If(survole, UITheme.ColorSaugeFonce, UITheme.ColorSauge))
        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(pastille, 9.0F)
            Using pinceau As New SolidBrush(couleurPastille)
                g.FillPath(pinceau, chemin)
            End Using
        End Using
        If Image IsNot Nothing Then
            Dim decalage As Integer = (TAILLE_PASTILLE - TAILLE_ICONE) \ 2
            AltheaDessin.DessinerImage(g, Image,
                                       New Rectangle(MARGE + decalage, yPastille + decalage, TAILLE_ICONE, TAILLE_ICONE),
                                       If(actif, 1.0F, 0.6F))
        End If

        ' Titre.
        Dim xTitre As Integer = MARGE + TAILLE_PASTILLE + ECART
        Dim couleurTitre As Color = If(actif, ForeColor, AltheaDessin.CouleurDesactiveTexte)
        TextRenderer.DrawText(g, Text, Font, New Rectangle(xTitre, 0, _largeurTitre, Height), couleurTitre,
                              TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis Or
                              TextFormatFlags.SingleLine Or TextFormatFlags.NoPrefix)

        ' Action à droite.
        Dim largeurAction As Integer = 0
        If Not String.IsNullOrEmpty(_texteAction) Then
            Dim tailleAction As Size = TextRenderer.MeasureText(_texteAction, Font)
            largeurAction = tailleAction.Width + 16
            Dim zoneAction As New Rectangle(Width - MARGE - largeurAction, (Height - 30) \ 2, largeurAction, 30)
            If survole Then
                Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(zoneAction, 8.0F)
                    Using pinceau As New SolidBrush(UITheme.ColorBlanc)
                        g.FillPath(pinceau, chemin)
                    End Using
                End Using
            End If
            TextRenderer.DrawText(g, _texteAction, Font, zoneAction, couleurTitre,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or
                                  TextFormatFlags.SingleLine Or TextFormatFlags.NoPrefix)
            largeurAction += ECART
        End If

        ' Aperçu.
        Dim xDescription As Integer = DebutDescription
        Dim largeurDescription As Integer = Width - xDescription - MARGE - largeurAction
        If largeurDescription > 20 Then
            Dim vide As Boolean = String.IsNullOrWhiteSpace(_description)
            TextRenderer.DrawText(g, If(vide, _texteVide, _description), If(vide, _policeVide, _policeDescription),
                                  New Rectangle(xDescription, 0, largeurDescription, Height),
                                  If(Not actif, AltheaDessin.CouleurDesactiveTexte,
                                     If(vide, UITheme.ColorGrisDiscret, UITheme.ColorGrisFonce)),
                                  TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis Or
                                  TextFormatFlags.SingleLine Or TextFormatFlags.NoPrefix)
        End If

    End Sub

#End Region

#Region "Événements souris et état"

    Protected Overrides Sub OnMouseEnter(e As EventArgs)
        _survol = True
        Invalidate()
        MyBase.OnMouseEnter(e)
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        _survol = False
        _enfonce = False
        Invalidate()
        MyBase.OnMouseLeave(e)
    End Sub

    Protected Overrides Sub OnMouseDown(mevent As MouseEventArgs)
        If mevent.Button = MouseButtons.Left Then
            _enfonce = True
            Invalidate()
        End If
        MyBase.OnMouseDown(mevent)
    End Sub

    Protected Overrides Sub OnMouseUp(mevent As MouseEventArgs)
        _enfonce = False
        Invalidate()
        MyBase.OnMouseUp(mevent)
    End Sub

    Protected Overrides Sub OnEnabledChanged(e As EventArgs)
        If Not Enabled Then
            _survol = False
            _enfonce = False
        End If
        Invalidate()
        MyBase.OnEnabledChanged(e)
    End Sub

    Protected Overrides Sub OnGotFocus(e As EventArgs)
        Invalidate()
        MyBase.OnGotFocus(e)
    End Sub

    Protected Overrides Sub OnLostFocus(e As EventArgs)
        Invalidate()
        MyBase.OnLostFocus(e)
    End Sub

#End Region

End Class
