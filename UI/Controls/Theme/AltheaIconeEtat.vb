' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaIconeEtat
' Projet     : Althéa
' Version    : V1.1.0
' Date       : 16/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Icône d'état du kit Althéa (Information, Avertissement, Erreur, Question, Succès, Attente),
' dessinée en code : rond à fond pâle, contour et signe dans la couleur du type (maquette
' « DialogChoix — icônes », style 2, retenu le 14/10/2026).
'
' Remarques  :
' - Remplace les GIF animés de DialogChoix : net à toutes les tailles, couleurs de la charte, un
'   seul endroit à modifier. Utilisable ailleurs (notes d'aide, encadrés d'attention).
' - Le dessin est fait sur une grille de 48 unités, mise à l'échelle de la taille du contrôle.
' - Attente : l'arc tourne (minuterie) tant que le contrôle est visible.
' - Teintes(valeurEtat) donne les trois couleurs d'un type (base, pâle, foncée), toutes dérivées des
'   couleurs de UITheme : l'en-tête de DialogChoix les réutilise.
'
' - V1.1.0 (16/10/2026) : dessin partagé (Dessiner) et CreerImage, pour afficher l'icône là où
'   une Image est attendue (icône d'AltheaEnTete dans DialogChoix).
'
' Dépendances :
' - UITheme, AltheaDessin
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

' Type d'état représenté par l'icône.
Public Enum EtatIcone
    Information = 0
    Avertissement = 1
    Erreur = 2
    Question = 3
    Succes = 4
    Attente = 5
End Enum

<DesignerCategory("Code")>
<ToolboxItem(True)>
<Description("Icône d'état Althéa : rond pâle, contour et signe dans la couleur du type.")>
Public Class AltheaIconeEtat
    Inherits Control

#Region "Constantes"

    ' Grille de dessin (les coordonnées des signes sont exprimées sur 48 unités).
    Private Const GRILLE As Single = 48.0F

    ' Rotation de l'arc d'attente : pas (degrés) et intervalle (ms).
    Private Const PAS_ROTATION As Single = 12.0F
    Private Const INTERVALLE_ROTATION As Integer = 40

#End Region

#Region "Variables privées"

    Private _etat As EtatIcone = EtatIcone.Information
    Private _angle As Single
    Private ReadOnly _minuterie As New System.Windows.Forms.Timer() With {.Interval = INTERVALLE_ROTATION}

#End Region

#Region "Initialisation"

    Public Sub New()

        SetStyle(ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or
                 ControlStyles.SupportsTransparentBackColor, True)

        BackColor = Color.Transparent
        Size = New Size(32, 32)
        TabStop = False

        AddHandler _minuterie.Tick, AddressOf Minuterie_Tick

    End Sub

#End Region

#Region "Propriétés publiques"

    <Category("Althéa")>
    <Description("Type d'état représenté (signe et couleur).")>
    <DefaultValue(GetType(EtatIcone), "Information")>
    Public Property Etat As EtatIcone
        Get
            Return _etat
        End Get
        Set(value As EtatIcone)
            _etat = value
            ActualiserMinuterie()
            Invalidate()
        End Set
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Teintes
    ' Rôle     : Couleurs d'un type d'état : Base (contour), Pale (fond), Foncee (signe, titre).
    '            Toutes dérivées de la charte (UITheme), aucune valeur propre.
    ' -------------------------------------------------------------------------------------------------
    Public Shared Function Teintes(valeurEtat As EtatIcone) As (Base As Color, Pale As Color, Foncee As Color)

        Select Case valeurEtat
            Case EtatIcone.Attente
                Return (UITheme.ColorSaugeClair, UITheme.ColorBeigeClair, UITheme.ColorSaugeFonce)
            Case Else
                Dim base As Color = CouleurBase(valeurEtat)
                Return (base, AltheaDessin.Melanger(base, Color.White, 0.85F), AltheaDessin.Assombrir(base, 0.3F))
        End Select

    End Function

    Private Shared Function CouleurBase(valeurEtat As EtatIcone) As Color

        Select Case valeurEtat
            Case EtatIcone.Avertissement : Return UITheme.ColorJauneModere
            Case EtatIcone.Erreur : Return AltheaDessin.Assombrir(UITheme.ColorRoseFonce, 0.08F)
            Case EtatIcone.Question : Return UITheme.ColorMauveFonce
            Case EtatIcone.Succes : Return UITheme.ColorSaugeFonce
            Case Else : Return UITheme.ColorBleuMoyen
        End Select

    End Function

#End Region

#Region "Animation de l'attente"

    Private Sub ActualiserMinuterie()
        _minuterie.Enabled = _etat = EtatIcone.Attente AndAlso Visible AndAlso Not DesignMode
    End Sub

    Protected Overrides Sub OnVisibleChanged(e As EventArgs)
        MyBase.OnVisibleChanged(e)
        ActualiserMinuterie()
    End Sub

    Private Sub Minuterie_Tick(sender As Object, e As EventArgs)
        _angle = (_angle + PAS_ROTATION) Mod 360.0F
        Invalidate()
    End Sub

#End Region

#Region "Dessin"

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        MyBase.OnPaint(e)

        Dim cote As Single = Math.Min(ClientSize.Width, ClientSize.Height) - 1
        If cote <= 4 Then Return

        Dim zone As New RectangleF((ClientSize.Width - cote) / 2.0F, (ClientSize.Height - cote) / 2.0F, cote, cote)
        Dessiner(e.Graphics, zone, _etat, _angle)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Dessiner
    ' Version    : V1.1.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Dessine l'icône d'un état dans un carré : rond à fond pâle, contour et signe dans la couleur
    ' du type. Utilisée par le contrôle et par CreerImage.
    '
    ' Paramètres :
    ' - g          : surface de dessin
    ' - zone       : carré où dessiner l'icône
    ' - valeurEtat : type d'état
    ' - angle      : rotation de l'arc d'attente, en degrés (ignorée pour les autres états)
    ' -------------------------------------------------------------------------------------------------
    Public Shared Sub Dessiner(g As Graphics, zone As RectangleF, valeurEtat As EtatIcone, Optional angle As Single = 0.0F)

        Dim cote As Single = Math.Min(zone.Width, zone.Height)
        If cote <= 4 Then Return

        Dim teinte = Teintes(valeurEtat)
        Dim echelle As Single = cote / GRILLE
        Dim sauvegarde As GraphicsState = g.Save()

        Try
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.TranslateTransform(zone.X + (zone.Width - cote) / 2.0F, zone.Y + (zone.Height - cote) / 2.0F)
            g.ScaleTransform(echelle, echelle)

            ' Rond : fond pâle, contour dans la couleur du type.
            Using fond As New SolidBrush(teinte.Pale)
                g.FillEllipse(fond, 2.5F, 2.5F, 43.0F, 43.0F)
            End Using
            Using contour As New Pen(teinte.Base, 2.5F)
                g.DrawEllipse(contour, 2.5F, 2.5F, 43.0F, 43.0F)
            End Using

            DessinerSigne(g, teinte.Foncee, valeurEtat, angle)

        Finally
            g.Restore(sauvegarde)
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CreerImage
    ' Version    : V1.1.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Icône d'un état sous forme d'image à fond transparent, pour un contrôle qui attend une Image
    ' (icône d'AltheaEnTete dans DialogChoix, par exemple).
    '
    ' Paramètres :
    ' - valeurEtat : type d'état
    ' - cote       : côté de l'image, en pixels
    '
    ' Retour     : image à libérer par l'appelant (Dispose).
    '
    ' Remarques  :
    ' - Image fixe : l'arc de l'état Attente n'y tourne pas (utiliser le contrôle pour l'animation).
    ' -------------------------------------------------------------------------------------------------
    Public Shared Function CreerImage(valeurEtat As EtatIcone, cote As Integer) As Bitmap

        Dim image As New Bitmap(Math.Max(1, cote), Math.Max(1, cote))

        Using g As Graphics = Graphics.FromImage(image)
            g.Clear(Color.Transparent)
            Dessiner(g, New RectangleF(0.0F, 0.0F, image.Width - 1, image.Height - 1), valeurEtat)
        End Using

        Return image

    End Function

    ' Signe du type, sur la grille de 48 unités (mêmes tracés que la maquette).
    Private Shared Sub DessinerSigne(g As Graphics, couleur As Color, valeurEtat As EtatIcone, angle As Single)

        Using trait As New Pen(couleur, 4.0F) With {.StartCap = LineCap.Round, .EndCap = LineCap.Round, .LineJoin = LineJoin.Round},
              pinceau As New SolidBrush(couleur)

            Select Case valeurEtat

                Case EtatIcone.Information
                    g.DrawLine(trait, 24.0F, 21.0F, 24.0F, 34.0F)
                    DessinerPoint(g, pinceau, 24.0F, 14.0F, 2.6F)

                Case EtatIcone.Avertissement
                    trait.Width = 4.5F
                    g.DrawLine(trait, 24.0F, 12.0F, 24.0F, 27.0F)
                    DessinerPoint(g, pinceau, 24.0F, 35.0F, 2.8F)

                Case EtatIcone.Erreur
                    trait.Width = 4.2F
                    g.DrawLine(trait, 16.5F, 16.5F, 31.5F, 31.5F)
                    g.DrawLine(trait, 31.5F, 16.5F, 16.5F, 31.5F)

                Case EtatIcone.Question
                    ' Arc de cercle (centre 24 ; 18,5, rayon 6) puis courbe vers la tige.
                    Using chemin As New GraphicsPath()
                        chemin.AddArc(18.0F, 12.5F, 12.0F, 12.0F, 180.0F, 244.3F)
                        chemin.AddBezier(26.6F, 23.9F, 24.9F, 24.7F, 24.0F, 26.0F, 24.0F, 27.9F)
                        chemin.AddLine(24.0F, 27.9F, 24.0F, 29.0F)
                        g.DrawPath(trait, chemin)
                    End Using
                    DessinerPoint(g, pinceau, 24.0F, 35.5F, 2.6F)

                Case EtatIcone.Succes
                    trait.Width = 4.2F
                    g.DrawLines(trait, {New PointF(14.5F, 24.5F), New PointF(21.0F, 31.0F), New PointF(33.5F, 18.0F)})

                Case EtatIcone.Attente
                    ' Arc ouvert (rayon 13) qui tourne autour du centre.
                    g.DrawArc(trait, 11.0F, 11.0F, 26.0F, 26.0F, 270.0F + angle, 288.0F)

            End Select

        End Using

    End Sub

    Private Shared Sub DessinerPoint(g As Graphics, pinceau As Brush, x As Single, y As Single, rayon As Single)
        g.FillEllipse(pinceau, x - rayon, y - rayon, 2 * rayon, 2 * rayon)
    End Sub

#End Region

#Region "Libération"

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            _minuterie.Stop()
            _minuterie.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#End Region

End Class
