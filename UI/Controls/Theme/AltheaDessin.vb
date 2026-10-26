' -------------------------------------------------------------------------------------------------
' Module     : AltheaDessin
' Projet     : Althéa
' Version    : V1.2.0
' Date       : 20/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Outils de dessin partagés par les contrôles du kit Althéa (AltheaBouton, AltheaTuile,
' AltheaCarte, AltheaBarreActions, AltheaTitreSection).
'
' Responsabilités :
' - Construire des contours à coins arrondis (GraphicsPath)
' - Assombrir, éclaircir ou mélanger des couleurs de la charte
' - Dessiner une image avec une opacité donnée (état désactivé)
' - Dessiner une ombre douce sous un rectangle arrondi
' - Dessiner un titre de section (texte + trait fin)
'
' Remarques  :
' - Les couleurs de base viennent de UITheme ; seules les teintes d'état (désactivé) sont
'   définies ici, dérivées de la charte.
' - Aucune image : tout est dessiné, donc net à toutes les tailles et à toutes les échelles.
' - V1.2.0 (20/10/2026) : CouleurDesactiveTexteBouton, OpaciteIconeDesactivee (boutons inactifs lisibles).
' - V1.1.0 (16/10/2026) : FLAGS_TEXTE_SOUS_ENFANTS, pour le texte qu'un parent dessine sous ses
'   enfants (titre de carte, de barre locale, d'en-tête).
'
' Dépendances :
' - UITheme
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

Friend Module AltheaDessin

#Region "Texte dessiné par un parent"

    ' -------------------------------------------------------------------------------------------------
    ' Constante  : FLAGS_TEXTE_SOUS_ENFANTS
    ' Rôle       : Options à ajouter à TextRenderer.DrawText pour un texte qu'un conteneur dessine
    '              (titre d'AltheaCarte, d'AltheaBarreOutils, textes d'AltheaEnTete).
    '
    ' Remarques  :
    ' - Les contrôles à fond recopié (AltheaFondParent : étapes, pastilles, listes, dates…) font
    '   redessiner leur parent dans leur propre tampon, décalé par TranslateTransform.
    '   TextRenderer ignore ce décalage par défaut : sans ces options, le titre du parent
    '   réapparaissait en fantôme au coin de l'enfant (vitrine du kit, 16/10/2026).
    ' -------------------------------------------------------------------------------------------------
    Friend Const FLAGS_TEXTE_SOUS_ENFANTS As TextFormatFlags =
        TextFormatFlags.PreserveGraphicsTranslateTransform Or TextFormatFlags.PreserveGraphicsClipping

#End Region

#Region "Teintes d'état"

    ' Fond d'un contrôle désactivé (gris-vert très pâle dérivé de ColorGrisVertClair).
    Friend ReadOnly CouleurDesactiveFond As Color = Color.FromArgb(233, 236, 232)

    ' Texte d'un contrôle désactivé.
    Friend ReadOnly CouleurDesactiveTexte As Color = Color.FromArgb(154, 170, 160)

    ' Texte et opacité de l'icône d'un bouton désactivé : plus foncés que CouleurDesactiveTexte,
    ' presque illisible sur le fond pâle d'un bouton (relevé sur la vitrine, 16/10/2026) ; le bouton
    ' reste visiblement inactif (contraste d'environ 3,9 pour 1 au lieu de 2).
    Friend ReadOnly CouleurDesactiveTexteBouton As Color = Color.FromArgb(108, 120, 113)
    Friend Const OpaciteIconeDesactivee As Single = 0.55F

    ' Bordure d'un bouton « Clair » désactivé.
    Friend ReadOnly CouleurDesactiveBordure As Color = Color.FromArgb(207, 216, 210)

#End Region

#Region "Couleurs"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : Assombrir
    ' Rôle       : Ajoute une part de noir à une couleur (facteur 0 = inchangée, 1 = noir).
    ' -------------------------------------------------------------------------------------------------
    Friend Function Assombrir(couleur As Color, facteur As Single) As Color

        Dim f As Single = 1.0F - Math.Max(0.0F, Math.Min(1.0F, facteur))

        Return Color.FromArgb(couleur.A,
                              CInt(couleur.R * f),
                              CInt(couleur.G * f),
                              CInt(couleur.B * f))

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : Melanger
    ' Rôle       : Mélange deux couleurs (t = 0 : couleur1, t = 1 : couleur2).
    ' -------------------------------------------------------------------------------------------------
    Friend Function Melanger(couleur1 As Color, couleur2 As Color, t As Single) As Color

        Dim k As Single = Math.Max(0.0F, Math.Min(1.0F, t))

        Return Color.FromArgb(CInt(couleur1.A + (CInt(couleur2.A) - CInt(couleur1.A)) * k),
                              CInt(couleur1.R + (CInt(couleur2.R) - CInt(couleur1.R)) * k),
                              CInt(couleur1.G + (CInt(couleur2.G) - CInt(couleur1.G)) * k),
                              CInt(couleur1.B + (CInt(couleur2.B) - CInt(couleur1.B)) * k))

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : AvecAlpha
    ' Rôle       : Renvoie la couleur avec une transparence donnée (0 à 255).
    ' -------------------------------------------------------------------------------------------------
    Friend Function AvecAlpha(couleur As Color, alpha As Integer) As Color

        Return Color.FromArgb(Math.Max(0, Math.Min(255, alpha)), couleur.R, couleur.G, couleur.B)

    End Function

#End Region

#Region "Formes"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CheminArrondi
    ' Rôle       : Contour d'un rectangle à coins arrondis. L'appelant libère le GraphicsPath.
    ' -------------------------------------------------------------------------------------------------
    Friend Function CheminArrondi(rect As RectangleF, rayon As Single) As GraphicsPath

        Dim chemin As New GraphicsPath()
        Dim r As Single = Math.Max(0.0F, Math.Min(rayon, Math.Min(rect.Width, rect.Height) / 2.0F))

        If r < 0.5F OrElse rect.Width <= 0 OrElse rect.Height <= 0 Then
            chemin.AddRectangle(rect)
            Return chemin
        End If

        Dim d As Single = r * 2.0F

        chemin.AddArc(rect.X, rect.Y, d, d, 180, 90)
        chemin.AddArc(rect.Right - d, rect.Y, d, d, 270, 90)
        chemin.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90)
        chemin.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90)
        chemin.CloseFigure()

        Return chemin

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerOmbre
    ' Rôle       : Ombre douce sous un rectangle arrondi, par couches successives très légères.
    '
    ' Paramètres :
    ' - couleur   : couleur de l'ombre, son alpha fixe l'intensité au plus près du bord
    ' - taille    : étalement de l'ombre en pixels
    ' - decalageY : décalage vers le bas (lumière venant du haut)
    ' -------------------------------------------------------------------------------------------------
    Friend Sub DessinerOmbre(g As Graphics,
                             rect As RectangleF,
                             rayon As Single,
                             couleur As Color,
                             taille As Integer,
                             decalageY As Single)

        If taille <= 0 OrElse couleur.A = 0 Then Return

        ' Un seul pinceau pour toutes les couches : sa couleur change d'une passe a l'autre.
        ' Ce dessin est repete pour chaque tuile d'un ecran, l'allocation compte.
        Using pinceau As New SolidBrush(couleur)

            For i As Integer = taille To 1 Step -1

                Dim alpha As Integer = CInt(couleur.A * (taille - i + 1) / (taille * 2.0F))
                Dim r As New RectangleF(rect.X - i * 0.5F,
                                        rect.Y + decalageY - i * 0.25F,
                                        rect.Width + i,
                                        rect.Height + i * 0.5F)

                pinceau.Color = AvecAlpha(couleur, alpha)

                Using chemin As GraphicsPath = CheminArrondi(r, rayon + i * 0.5F)
                    g.FillPath(pinceau, chemin)
                End Using

            Next

        End Using

    End Sub

#End Region

#Region "Images et textes"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerImage
    ' Rôle       : Dessine une image dans un rectangle, avec une opacité (1 = normale).
    ' -------------------------------------------------------------------------------------------------
    Friend Sub DessinerImage(g As Graphics, img As Image, dest As Rectangle, opacite As Single)

        If img Is Nothing Then Return

        If opacite >= 0.999F Then
            g.DrawImage(img, dest)
            Return
        End If

        Using attributs As New ImageAttributes()

            Dim matrice As New ColorMatrix()
            matrice.Matrix33 = Math.Max(0.0F, Math.Min(1.0F, opacite))
            attributs.SetColorMatrix(matrice)

            g.DrawImage(img, dest, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attributs)

        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerTitreSection
    ' Rôle       : Dessine un titre de section suivi d'un trait fin jusqu'au bord droit.
    '
    ' Remarques  :
    ' - Utilisé par AltheaCarte (propriété Titre) et AltheaTitreSection.
    ' -------------------------------------------------------------------------------------------------
    Friend Sub DessinerTitreSection(g As Graphics,
                                    texte As String,
                                    police As Font,
                                    couleurTexte As Color,
                                    couleurTrait As Color,
                                    zone As Rectangle)

        If String.IsNullOrEmpty(texte) OrElse zone.Width <= 0 Then Return

        Dim flags As TextFormatFlags = TextFormatFlags.Left Or
                                       TextFormatFlags.VerticalCenter Or
                                       TextFormatFlags.SingleLine Or
                                       TextFormatFlags.NoPadding Or
                                       TextFormatFlags.EndEllipsis Or
                                       FLAGS_TEXTE_SOUS_ENFANTS

        Dim taille As Size = TextRenderer.MeasureText(g, texte, police, zone.Size, flags)

        TextRenderer.DrawText(g, texte, police, zone, couleurTexte, flags)

        Dim xTrait As Integer = zone.X + taille.Width + 12
        Dim yTrait As Integer = zone.Y + zone.Height \ 2

        If xTrait < zone.Right Then
            Using stylo As New Pen(couleurTrait, 1.0F)
                g.DrawLine(stylo, xTrait, yTrait, zone.Right, yTrait)
            End Using
        End If

    End Sub

#End Region

End Module
