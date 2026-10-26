' -------------------------------------------------------------------------------------------------
' Classe     : AltheaFondFenetre
' Projet     : Althéa
' Version    : V1.3.0
' Date       : 16/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Fond des fenêtres du kit Althéa : beige clair uni et petite feuille dessinée dans le coin
' haut droit. Remplace l'image étirée Fond_1000x800_Feuille4 (décision de Joëlle, 14/10/2026).
'
' Responsabilités :
' - Poser le fond uni (UITheme.ColorBeigeClair) et retirer l'image de fond
' - Dessiner la feuille par le code (contour, nervure, tige), aux couleurs sauge de la charte
'
' Utilisation :
' - Dans le constructeur d'une fenêtre, après InitializeComponent : AltheaFondFenetre.Appliquer(Me)
' - Fenêtre dont tout le contenu est posé sur un panneau racine (pnlForm) : Appliquer(pnlForm),
'   sinon le panneau opaque cacherait la feuille (V1.1.0)
' - V1.2.0 (15/10/2026) : appliqué à une fenêtre, colore aussi sa barre de titre Windows
'   (AltheaBarreTitre) : une seule ligne donne à la fenêtre tout l'habillage du kit.
' - V1.3.0 (16/10/2026) : LargeurZone devient publique, pour qu'une fenêtre dimensionnée par le
'   code (DialogChoix) laisse la place de la feuille à droite de son titre.
'
' Remarques  :
' - La feuille est dessinée dans l'évènement Paint de la surface : les contrôles transparents
'   posés dessus (AltheaEnTete) la laissent voir, car WinForms repeint le parent sous eux.
' - Taille proportionnelle à la mise à l'échelle Windows (DeviceDpi) ; aucune image, donc nette.
' - Classe sans état : un seul jeu de gestionnaires partagés, ajoutés une fois par fenêtre.
'
' Dépendances :
' - UITheme, AltheaDessin
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Drawing.Drawing2D

Public NotInheritable Class AltheaFondFenetre

#Region "Constantes"

    ' Feuille de référence (à 100 %) : dessinée le long de l'axe X, base en (0, 0), pointe en
    ' (LongueurFeuille, 0) ; la largeur sert aux courbes du contour et aux nervures.
    Private Const LongueurFeuille As Single = 50.0F
    Private Const LargeurFeuille As Single = 20.0F

    ' « Petite » feuille : léger agrandissement de la feuille de référence.
    Private Const EchelleFeuille As Single = 1.15F

    ' Orientation : base dans le coin haut droit, pointe vers le bas à gauche.
    Private Const AngleFeuille As Single = 150.0F

    ' Position de la base par rapport au coin haut droit de la zone cliente (à 100 %).
    Private Const MargeDroite As Single = 16.0F
    Private Const MargeHaut As Single = 14.0F

    ' Zone à repeindre autour de la feuille (à 100 %), comptée depuis le coin haut droit.
    ' Publique : place à laisser libre à droite d'un titre (DialogChoix), à multiplier par DeviceDpi / 96.
    Public Const LargeurZone As Integer = 84
    Private Const HauteurZone As Integer = 60

    ' Opacités (0 à 255) appliquées aux couleurs sauge de la charte.
    Private Const AlphaRemplissage As Integer = 95
    Private Const AlphaContour As Integer = 140
    Private Const AlphaNervure As Integer = 170
    Private Const AlphaNervuresSecondaires As Integer = 110

#End Region

#Region "Initialisation"

    ' Classe utilitaire : pas d'instance.
    Private Sub New()
    End Sub

#End Region

#Region "Méthodes publiques"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Appliquer
    ' Version    : V1.2.0
    ' Date       : 15/10/2026
    '
    ' Rôle       :
    ' Donne à une fenêtre le fond du kit : beige clair uni, feuille dessinée dans le coin haut droit.
    '
    ' Paramètres :
    ' - surface : la fenêtre, ou son panneau racine s'il la recouvre entièrement (sans effet si Nothing)
    '
    ' Remarques  :
    ' - Peut être appelée plusieurs fois sans doubler le dessin (gestionnaires retirés puis ajoutés).
    ' -------------------------------------------------------------------------------------------------
    Public Shared Sub Appliquer(surface As Control)

        If surface Is Nothing Then Return

        surface.BackgroundImage = Nothing
        surface.BackColor = UITheme.ColorBeigeClair

        RemoveHandler surface.Paint, AddressOf Surface_Paint
        AddHandler surface.Paint, AddressOf Surface_Paint

        RemoveHandler surface.Resize, AddressOf Surface_Resize
        AddHandler surface.Resize, AddressOf Surface_Resize

        ' Barre de titre Windows aux couleurs du kit (Windows 11).
        Dim fenetre As Form = TryCast(surface, Form)
        If fenetre IsNot Nothing Then AltheaBarreTitre.Appliquer(fenetre)

    End Sub

#End Region

#Region "Dessin"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Surface_Paint
    ' Rôle       : Dessine la feuille quand la zone à repeindre la touche.
    ' -------------------------------------------------------------------------------------------------
    Private Shared Sub Surface_Paint(sender As Object, e As PaintEventArgs)

        Dim surface As Control = TryCast(sender, Control)
        If surface Is Nothing Then Return

        Dim echelle As Single = surface.DeviceDpi / 96.0F
        If Not e.ClipRectangle.IntersectsWith(ZoneFeuille(surface.ClientSize, echelle)) Then Return

        Dim baseFeuille As New PointF(surface.ClientSize.Width - MargeDroite * echelle, MargeHaut * echelle)
        DessinerFeuille(e.Graphics, baseFeuille, echelle * EchelleFeuille)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Surface_Resize
    ' Rôle       : La feuille suit le bord droit : la surface est repeinte après un redimensionnement.
    ' -------------------------------------------------------------------------------------------------
    Private Shared Sub Surface_Resize(sender As Object, e As EventArgs)

        DirectCast(sender, Control).Invalidate()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ZoneFeuille
    ' Rôle       : Rectangle occupé par la feuille (et sa tige) dans la zone cliente.
    ' -------------------------------------------------------------------------------------------------
    Private Shared Function ZoneFeuille(tailleClient As Size, echelle As Single) As Rectangle

        Dim largeur As Integer = CInt(Math.Ceiling(LargeurZone * echelle))
        Dim hauteur As Integer = CInt(Math.Ceiling(HauteurZone * echelle))

        Return New Rectangle(tailleClient.Width - largeur, 0, largeur, hauteur)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerFeuille
    ' Version    : V1.0.0
    ' Date       : 14/10/2026
    '
    ' Rôle       :
    ' Dessine une feuille stylisée : contour en deux courbes, remplissage sauge translucide,
    ' nervure centrale prolongée par la tige, trois paires de nervures secondaires.
    '
    ' Paramètres :
    ' - g           : surface de dessin
    ' - baseFeuille : point d'attache de la feuille (départ de la tige)
    ' - echelle     : facteur appliqué à la feuille de référence
    ' -------------------------------------------------------------------------------------------------
    Private Shared Sub DessinerFeuille(g As Graphics, baseFeuille As PointF, echelle As Single)

        Dim sauvegarde As GraphicsState = g.Save()

        Try

            g.SmoothingMode = SmoothingMode.AntiAlias
            g.TranslateTransform(baseFeuille.X, baseFeuille.Y)
            g.RotateTransform(AngleFeuille)
            g.ScaleTransform(echelle, echelle)

            Const L As Single = LongueurFeuille
            Const W As Single = LargeurFeuille

            ' Contour légèrement dissymétrique : plus naturel qu'une ellipse.
            Using contour As New GraphicsPath()

                contour.AddBezier(0.0F, 0.0F, L * 0.22F, -W * 0.62F, L * 0.68F, -W * 0.58F, L, 0.0F)
                contour.AddBezier(L, 0.0F, L * 0.74F, W * 0.52F, L * 0.3F, W * 0.62F, 0.0F, 0.0F)
                contour.CloseFigure()

                Using remplissage As New SolidBrush(AltheaDessin.AvecAlpha(UITheme.ColorSauge, AlphaRemplissage))
                    g.FillPath(remplissage, contour)
                End Using

                Using trait As New Pen(AltheaDessin.AvecAlpha(UITheme.ColorSaugeFonce, AlphaContour), 1.0F)
                    g.DrawPath(trait, contour)
                End Using

            End Using

            ' Tige (dépasse de la base) et nervure centrale, d'un seul trait.
            Using trait As New Pen(AltheaDessin.AvecAlpha(UITheme.ColorSaugeFonce, AlphaNervure), 1.1F)
                trait.StartCap = LineCap.Round
                trait.EndCap = LineCap.Round
                g.DrawBezier(trait, -9.0F, 1.5F, -3.0F, 0.5F, L * 0.4F, -1.0F, L * 0.9F, 0.0F)
            End Using

            ' Nervures secondaires, orientées vers la pointe ; longueur limitée à la demi-largeur locale.
            Using trait As New Pen(AltheaDessin.AvecAlpha(UITheme.ColorSaugeFonce, AlphaNervuresSecondaires), 0.9F)
                For Each nervure As (Depart As Single, Ecart As Single) In {(10.0F, 6.0F), (20.0F, 6.0F), (30.0F, 4.3F)}
                    For Each cote As Single In {-1.0F, 1.0F}
                        Dim y As Single = cote * nervure.Ecart
                        g.DrawBezier(trait,
                                     nervure.Depart, 0.0F,
                                     nervure.Depart + 3.0F, y * 0.55F,
                                     nervure.Depart + 6.0F, y * 0.85F,
                                     nervure.Depart + 8.0F, y)
                    Next
                Next
            End Using

        Finally
            g.Restore(sauvegarde)
        End Try

    End Sub

#End Region

End Class
