' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaBarreOutils
' Projet     : Althéa
' Version    : V1.1.0
' Date       : 16/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Barre locale du kit Althéa : bande d'outils posée en haut d'une carte, au-dessus de la grille
' (ou de l'éditeur, de l'image) sur laquelle ses boutons agissent.
'
' Responsabilités :
' - Afficher, facultativement, le nom de la liste à gauche (Mauve foncé, gras)
' - Souligner la bande d'un trait fin qui la sépare du contenu
'
' Remarques  :
' - Décision du 10/10/2026 (maquette « Barres locales », version B) : les boutons qui agissent
'   sur une seule liste ne doivent pas ressembler à la barre d'actions de l'écran
'   (AltheaBarreActions). Ils utilisent les variantes Outil / OutilPrincipal / OutilDanger
'   d'AltheaBouton : sans cadre ni ombre.
' - Se pose en Dock = Top dans une AltheaCarte qui contient aussi la grille (Dock = Fill) : la
'   carte matérialise ce à quoi les boutons appartiennent.
' - Titre vide par défaut : dans un onglet, le nom de la liste répéterait celui de l'onglet.
' - Les boutons sont placés dans le Designer, après le titre éventuel (MargeTitre).
' - V1.1.0 (16/10/2026) : titre dessiné avec AltheaDessin.FLAGS_TEXTE_SOUS_ENFANTS (pas de titre
'   fantôme sous un contrôle à fond recopié posé dans la barre).
'
' Dépendances :
' - UITheme
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel

<DesignerCategory("Code")>
<ToolboxItem(True)>
<Description("Barre locale Althéa : outils d'une liste, en haut de sa carte.")>
Public Class AltheaBarreOutils
    Inherits Panel

#Region "Variables privées"

    Private _titre As String = String.Empty
    Private _couleurTrait As Color = UITheme.ColorBeige
    Private _policeTitre As Font = New Font("Calibri", 11.0F, FontStyle.Bold)

#End Region

#Region "Initialisation"

    Public Sub New()

        SetStyle(ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or
                 ControlStyles.SupportsTransparentBackColor, True)

        MyBase.BackColor = Color.Transparent
        Dock = DockStyle.Top
        Padding = New Padding(10, 6, 10, 6)
        Size = New Size(600, 42)

    End Sub

#End Region

#Region "Propriétés publiques"

    <Category("Althéa")>
    <Description("Nom de la liste affiché à gauche (vide = pas de titre).")>
    <DefaultValue("")>
    Public Property Titre As String
        Get
            Return _titre
        End Get
        Set(value As String)
            _titre = If(value, String.Empty)
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Couleur du trait qui sépare la barre du contenu (Transparent = pas de trait).")>
    Public Property CouleurTrait As Color
        Get
            Return _couleurTrait
        End Get
        Set(value As Color)
            _couleurTrait = value
            Invalidate()
        End Set
    End Property

    ' Largeur occupée par le titre (marge comprise) : les boutons se placent au-delà.
    <Browsable(False)>
    Public ReadOnly Property MargeTitre As Integer
        Get
            If _titre.Length = 0 Then Return Padding.Left
            Return Padding.Left + TextRenderer.MeasureText(_titre, _policeTitre).Width + 12
        End Get
    End Property

    <Browsable(False)>
    <EditorBrowsable(EditorBrowsableState.Never)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Overrides Property BackColor As Color
        Get
            Return Color.Transparent
        End Get
        Set(value As Color)
            ' Ignoré volontairement : la barre laisse voir la carte.
        End Set
    End Property

#End Region

#Region "Sérialisation Designer"

    Private Function ShouldSerializeCouleurTrait() As Boolean
        Return _couleurTrait <> UITheme.ColorBeige
    End Function

    Private Sub ResetCouleurTrait()
        CouleurTrait = UITheme.ColorBeige
    End Sub

#End Region

#Region "Dessin"

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        MyBase.OnPaint(e)

        If _titre.Length > 0 Then
            TextRenderer.DrawText(e.Graphics, _titre, _policeTitre,
                                  New Rectangle(Padding.Left, 0, Math.Max(0, Width - Padding.Horizontal), Height - 1),
                                  If(Enabled, UITheme.ColorMauveFonce, AltheaDessin.CouleurDesactiveTexte),
                                  TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or
                                  TextFormatFlags.SingleLine Or TextFormatFlags.NoPrefix Or
                                  AltheaDessin.FLAGS_TEXTE_SOUS_ENFANTS)
        End If

        If _couleurTrait.A > 0 Then
            Using stylo As New Pen(_couleurTrait, 1.0F)
                e.Graphics.DrawLine(stylo, 0, Height - 1, Width, Height - 1)
            End Using
        End If

    End Sub

#End Region

End Class
