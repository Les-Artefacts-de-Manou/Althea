' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaPuceUtilisateur
' Projet     : Althéa
' Version    : V1.1.0
' Date       : 12/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Pastille de l'utilisateur connecté, à droite du bandeau de la Home (maquette « Home A ») :
' photo (ou, à défaut, initiales dans un rond Mauve), nom et précision sur deux lignes, rôle en
' pastille, mention « Élévation » en terre quand une élévation d'accès est active.
'
' Remarques  :
' - La largeur s'ajuste au contenu ; ancré à droite, le contrôle garde son bord droit en place.
' - Aucune donnée n'est lue ici : Home renseigne Nom, Precision, Role, Eleve et Photo.
' - Photo : l'image n'appartient pas au contrôle (l'appelant la libère quand il la remplace) ;
'   elle est recadrée au centre pour remplir le rond.
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
<Description("Pastille de l'utilisateur connecté : initiales, nom, rôle.")>
Public Class AltheaPuceUtilisateur
    Inherits Control

#Region "Constantes"

    Private Const MARGE As Integer = 6
    Private Const ECART As Integer = 10
    Private Const FLAGS As TextFormatFlags = TextFormatFlags.NoPadding Or TextFormatFlags.SingleLine Or TextFormatFlags.NoPrefix

#End Region

#Region "Variables privées"

    Private _nom As String = String.Empty
    Private _precision As String = String.Empty
    Private _role As String = String.Empty
    Private _eleve As Boolean
    Private _photo As Image

    Private ReadOnly _policeNom As New Font("Calibri", 11.25F, FontStyle.Bold)
    Private ReadOnly _policePrecision As New Font("Calibri", 9.75F)
    Private ReadOnly _policePastille As New Font("Calibri", 9.0F, FontStyle.Bold)
    Private ReadOnly _policeInitiales As New Font("Calibri", 10.5F, FontStyle.Bold)

#End Region

#Region "Initialisation"

    Public Sub New()

        SetStyle(ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or
                 ControlStyles.SupportsTransparentBackColor, True)

        BackColor = Color.Transparent
        Size = New Size(240, 46)
        TabStop = False

    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            _policeNom.Dispose()
            _policePrecision.Dispose()
            _policePastille.Dispose()
            _policeInitiales.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#End Region

#Region "Propriétés publiques"

    <Category("Althéa")>
    <DefaultValue("")>
    Public Property Nom As String
        Get
            Return _nom
        End Get
        Set(value As String)
            _nom = If(value, String.Empty).Trim()
            AjusterLargeur()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Seconde ligne, sous le nom (fonction, précision).")>
    <DefaultValue("")>
    Public Property Precision As String
        Get
            Return _precision
        End Get
        Set(value As String)
            _precision = If(value, String.Empty).Trim()
            AjusterLargeur()
        End Set
    End Property

    <Category("Althéa")>
    <DefaultValue("")>
    Public Property Role As String
        Get
            Return _role
        End Get
        Set(value As String)
            _role = If(value, String.Empty).Trim()
            AjusterLargeur()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Élévation d'accès active : ajoute la mention « Élévation ».")>
    <DefaultValue(False)>
    Public Property Eleve As Boolean
        Get
            Return _eleve
        End Get
        Set(value As Boolean)
            _eleve = value
            AjusterLargeur()
        End Set
    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    <Description("Photo affichée dans le rond à la place des initiales (Nothing = initiales).")>
    Public Property Photo As Image
        Get
            Return _photo
        End Get
        Set(value As Image)
            _photo = value
            Invalidate()
        End Set
    End Property

#End Region

#Region "Mise en page"

    Private ReadOnly Property TailleRond As Integer
        Get
            Return Math.Max(16, Height - 2 * MARGE)
        End Get
    End Property

    Private Function LargeurPastille(texte As String) As Integer
        If String.IsNullOrEmpty(texte) Then Return 0
        Return TextRenderer.MeasureText(texte, _policePastille, Size.Empty, FLAGS).Width + 16
    End Function

    Private Function LargeurVoulue() As Integer

        Dim texte As Integer = Math.Max(TextRenderer.MeasureText(_nom, _policeNom, Size.Empty, FLAGS).Width,
                                        TextRenderer.MeasureText(_precision, _policePrecision, Size.Empty, FLAGS).Width)
        Dim pastilles As Integer = LargeurPastille(_role) + If(_eleve, LargeurPastille("Élévation") + 4, 0)
        Return MARGE + TailleRond + ECART + texte + If(pastilles > 0, ECART + pastilles, 0) + 14

    End Function

    ' Garde le bord droit en place quand le contrôle est ancré à droite (bandeau de la Home).
    Private Sub AjusterLargeur()

        Dim largeur As Integer = LargeurVoulue()
        If (Anchor And AnchorStyles.Right) = AnchorStyles.Right AndAlso (Anchor And AnchorStyles.Left) <> AnchorStyles.Left Then
            Bounds = New Rectangle(Right - largeur, Top, largeur, Height)
        Else
            Width = largeur
        End If
        Invalidate()

    End Sub

#End Region

#Region "Dessin"

    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        AltheaFondParent.Peindre(Me, e)
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias

        Dim cadre As New RectangleF(0.5F, 0.5F, Width - 1.0F, Height - 1.0F)
        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(cadre, cadre.Height / 2.0F)
            Using pinceau As New SolidBrush(Color.FromArgb(180, UITheme.ColorBlanc))
                g.FillPath(pinceau, chemin)
            End Using
            Using stylo As New Pen(UITheme.ColorBeigeFonce)
                g.DrawPath(stylo, chemin)
            End Using
        End Using

        ' Rond : photo recadrée si elle existe, sinon initiales.
        Dim rond As New Rectangle(MARGE, MARGE, TailleRond, TailleRond)
        If _photo IsNot Nothing Then
            DessinerPhoto(g, rond)
        Else
            Using pinceau As New SolidBrush(UITheme.ColorMauveFonce)
                g.FillEllipse(pinceau, rond)
            End Using
            TextRenderer.DrawText(g, Initiales(), _policeInitiales, rond, UITheme.ColorBlanc,
                                  FLAGS Or TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        End If

        ' Nom et précision.
        Dim x As Integer = rond.Right + ECART
        Dim hNom As Integer = TextRenderer.MeasureText(_nom, _policeNom, Size.Empty, FLAGS).Height
        Dim hPrecision As Integer = If(_precision.Length > 0, TextRenderer.MeasureText(_precision, _policePrecision, Size.Empty, FLAGS).Height, 0)
        Dim y As Integer = (Height - hNom - hPrecision) \ 2
        TextRenderer.DrawText(g, _nom, _policeNom, New Point(x, y), UITheme.ColorGrisFonce, FLAGS)
        If hPrecision > 0 Then
            TextRenderer.DrawText(g, _precision, _policePrecision, New Point(x, y + hNom), UITheme.ColorSaugeFonce, FLAGS)
        End If

        Dim largeurTexte As Integer = Math.Max(TextRenderer.MeasureText(_nom, _policeNom, Size.Empty, FLAGS).Width,
                                               TextRenderer.MeasureText(_precision, _policePrecision, Size.Empty, FLAGS).Width)
        x += largeurTexte + ECART

        ' Pastilles du rôle et de l'élévation.
        x = DessinerPastille(g, _role, UITheme.ColorBeige, UITheme.ColorMauveFonce, x)
        If _eleve Then
            DessinerPastille(g, "Élévation", AltheaDessin.Melanger(UITheme.ColorBlanc, UITheme.ColorRougeMoyenClair, 0.25F),
                             AltheaDessin.Assombrir(UITheme.ColorRougeMoyenClair, 0.35F), x + 4)
        End If

    End Sub

    ' Photo recadrée au centre (remplissage du rond, proportions conservées) et liseré beige.
    Private Sub DessinerPhoto(g As Graphics, rond As Rectangle)

        Dim echelle As Single = Math.Max(rond.Width / CSng(_photo.Width), rond.Height / CSng(_photo.Height))
        Dim largeur As Single = _photo.Width * echelle
        Dim hauteur As Single = _photo.Height * echelle
        Dim cible As New RectangleF(rond.X + (rond.Width - largeur) / 2.0F, rond.Y + (rond.Height - hauteur) / 2.0F, largeur, hauteur)

        Using chemin As New GraphicsPath()
            chemin.AddEllipse(rond)
            Dim etat As GraphicsState = g.Save()
            g.SetClip(chemin)
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.DrawImage(_photo, cible)
            g.Restore(etat)
        End Using

        Using stylo As New Pen(UITheme.ColorBeigeFonce)
            g.DrawEllipse(stylo, rond)
        End Using

    End Sub

    Private Function DessinerPastille(g As Graphics, texte As String, fond As Color, couleur As Color, x As Integer) As Integer

        If String.IsNullOrEmpty(texte) Then Return x

        Dim largeur As Integer = LargeurPastille(texte)
        Dim cadre As New Rectangle(x, (Height - 20) \ 2, largeur, 20)
        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(cadre, 10.0F)
            Using pinceau As New SolidBrush(fond)
                g.FillPath(pinceau, chemin)
            End Using
        End Using
        TextRenderer.DrawText(g, texte, _policePastille, cadre, couleur,
                              FLAGS Or TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        Return cadre.Right

    End Function

    Private Function Initiales() As String

        Dim mots As String() = _nom.Split({" "c, "-"c}, StringSplitOptions.RemoveEmptyEntries)
        If mots.Length = 0 Then Return "?"
        Dim resultat As String = mots(0).Substring(0, 1)
        If mots.Length > 1 Then resultat &= mots(1).Substring(0, 1)
        Return resultat.ToUpperInvariant()

    End Function

#End Region

End Class
