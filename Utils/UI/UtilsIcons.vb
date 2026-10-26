' -------------------------------------------------------------------------------------------------
' Module      : UtilsIcons
' Projet      : Althéa
' Version     : V1.2.1
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Classe utilitaire centralisant l'accès aux icônes d'état de l'application.
'
' Responsabilités :
' - Fournir les icônes de statut de dossier et de patient sans dossier (dessinées), utilisées par
'   les filtres de la grille Patients
' - Éviter de redessiner une icône déjà créée
'
' Important   :
' - Les Fonctions sont Shared pour un accès global sans instanciation
'
' - V1.0.2 (17/10/2026) : IconStatutDossierDominant compare aux codes de GestionDossiers.
' - V1.1.0 (17/10/2026) : icônes de statut de dossier dessinées (style AltheaIconeEtat, couleurs de
'   la pastille de statut) ; IconPatientActif, IconPatientEnCours et IconPatientNonEnCours retirés
'   (plus utilisés depuis la nouvelle grille Patients).
' - V1.2.0 (18/10/2026, UI-9.4) : IconOK, IconOFF, IconLock et IconNo retirés avec les PNG
'   Assets/Tech_Ico : les états des grilles sont des pastilles (UtilsPastilles) depuis UI-9.
' - V1.2.1 (24/10/2026, audit C.3) : codes de statut de dossier lus dans
'   CodesSysteme.StatutsDossier.
'
' Dépendances :
' - CodesSysteme.StatutsDossier (codes de statut de dossier)
' - System.Drawing (Image), UITheme (couleurs des icônes dessinées)
'
' Usage :
' - UtilsIcons.IconStatutDossierDominant(code, taille) : icône d'un statut de dossier
' - UtilsIcons.IconPatientSansDossier(taille) : icône « patient sans dossier »
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UtilsIcons

#Region "Icônes de statut de dossier (dessinées)"

    ' Icônes déjà dessinées, par code de statut et taille : chacune n'est créée qu'une fois.
    Private Shared ReadOnly _iconesStatut As New Dictionary(Of String, Image)

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : IconPatientSansDossier (Shared)
    ' Version   : V2.0.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Icône neutre d'un patient sans aucun dossier : cercle pointillé gris et tiret.
    '
    ' Remarques :
    ' - V2.0.0 : dessinée (IconStatutDossierDominant) ; remplace l'octogone « ! » magenta, qui
    '   ressemblait à une alerte alors qu'un patient sans dossier est une situation normale.
    ' -------------------------------------------------------------------------------------------------
    Public Shared Function IconPatientSansDossier(Optional cote As Integer = 20) As Image
        Return IconStatutDossierDominant(Nothing, cote)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : IconStatutDossierDominant (Shared)
    ' Version   : V2.0.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Icône d'état du dossier dominant d'un patient, dessinée dans le style des icônes d'état du kit
    ' (AltheaIconeEtat : rond à fond pâle, contour et signe dans la couleur de l'état) :
    ' - ACTIF   : lecture (▶) vert-de-gris ;
    ' - PAUSE   : pause (‖) ocre ;
    ' - CLOTURE : coche gris foncé ;
    ' - ARCHIVE : boîte d'archive gris foncé ;
    ' - (aucun) : cercle pointillé et tiret gris (sans dossier).
    '
    ' Paramètres :
    ' - codeStatut : Code du statut dominant, ou Nothing / vide si le patient n'a pas de dossier
    ' - cote       : Taille de l'icône en pixels (20 par défaut)
    '
    ' Remarques :
    ' - V2.0.0 (maquette « Icônes d'état », 17/10/2026) : remplace les PNG actif / pause / cloture /
    '   archive / dossier_manquant, de styles et de couleurs disparates ; couleurs reprises de la
    '   pastille de statut (UITheme). Les PNG restent dans les ressources jusqu'au nettoyage UI-8.
    ' - Code inconnu : traité comme « sans dossier ».
    ' -------------------------------------------------------------------------------------------------
    Public Shared Function IconStatutDossierDominant(codeStatut As String, Optional cote As Integer = 20) As Image

        Dim code As String = If(codeStatut, String.Empty).Trim().ToUpperInvariant()
        Select Case code
            Case CodesSysteme.StatutsDossier.Actif, CodesSysteme.StatutsDossier.Pause,
                 CodesSysteme.StatutsDossier.Cloture, CodesSysteme.StatutsDossier.Archive
            Case Else
                code = String.Empty
        End Select

        Dim taille As Integer = Math.Max(8, cote)
        Dim cle As String = $"{code}|{taille}"
        Dim icone As Image = Nothing

        If Not _iconesStatut.TryGetValue(cle, icone) Then
            icone = DessinerIconeStatut(code, taille)
            _iconesStatut(cle) = icone
        End If

        Return icone

    End Function

    ' Dessine l'icône d'un statut dans un repère de 20 x 20 (celui de la maquette), mis à l'échelle.
    Private Shared Function DessinerIconeStatut(code As String, cote As Integer) As Bitmap

        Dim image As New Bitmap(cote, cote)

        Using g As Graphics = Graphics.FromImage(image)

            g.Clear(Color.Transparent)
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.ScaleTransform(cote / 20.0F, cote / 20.0F)

            Dim cercle As New RectangleF(1.4F, 1.4F, 17.2F, 17.2F)

            Select Case code

                Case CodesSysteme.StatutsDossier.Actif
                    DessinerRond(g, cercle, UITheme.ColorGrisVertClair, UITheme.ColorSauge)
                    Using pinceau As New SolidBrush(UITheme.ColorSaugeTresFonce)
                        g.FillPolygon(pinceau, {New PointF(8.2F, 6.4F), New PointF(14.0F, 10.0F), New PointF(8.2F, 13.6F)})
                    End Using

                Case CodesSysteme.StatutsDossier.Pause
                    DessinerRond(g, cercle, UITheme.StatutPauseBack, UITheme.ColorJauneModere)
                    Using pinceau As New SolidBrush(UITheme.StatutPauseFore)
                        g.FillRectangle(pinceau, 7.0F, 6.3F, 2.2F, 7.4F)
                        g.FillRectangle(pinceau, 10.8F, 6.3F, 2.2F, 7.4F)
                    End Using

                Case CodesSysteme.StatutsDossier.Cloture
                    DessinerRond(g, cercle, UITheme.ColorGrisClair, UITheme.ColorGrisMoyenFonce)
                    Using stylo As New Pen(UITheme.ColorGrisFonce, 1.9F) With {
                        .StartCap = Drawing2D.LineCap.Round, .EndCap = Drawing2D.LineCap.Round,
                        .LineJoin = Drawing2D.LineJoin.Round}
                        g.DrawLines(stylo, {New PointF(6.3F, 10.3F), New PointF(8.8F, 12.8F), New PointF(13.7F, 7.6F)})
                    End Using

                Case CodesSysteme.StatutsDossier.Archive
                    DessinerRond(g, cercle, UITheme.ColorGrisClair, UITheme.ColorGrisMoyenFonce)
                    Using stylo As New Pen(UITheme.ColorGrisFonce, 1.3F) With {
                        .StartCap = Drawing2D.LineCap.Round, .EndCap = Drawing2D.LineCap.Round}
                        g.DrawRectangle(stylo, 5.7F, 6.0F, 8.6F, 2.5F)
                        g.DrawLines(stylo, {New PointF(6.5F, 8.5F), New PointF(6.5F, 13.6F), New PointF(13.5F, 13.6F), New PointF(13.5F, 8.5F)})
                        g.DrawLine(stylo, 8.8F, 10.6F, 11.2F, 10.6F)
                    End Using

                Case Else
                    Using stylo As New Pen(UITheme.ColorGrisMoyenFonce, 1.3F) With {.DashPattern = {1.7F, 1.5F}}
                        g.DrawEllipse(stylo, cercle)
                    End Using
                    Using stylo As New Pen(UITheme.ColorGrisDiscret, 1.6F) With {
                        .StartCap = Drawing2D.LineCap.Round, .EndCap = Drawing2D.LineCap.Round}
                        g.DrawLine(stylo, 7.0F, 10.0F, 13.0F, 10.0F)
                    End Using

            End Select

        End Using

        Return image

    End Function

    ' Rond de l'icône : fond pâle et contour dans la couleur de l'état.
    Private Shared Sub DessinerRond(g As Graphics, cercle As RectangleF, fond As Color, contour As Color)
        Using pinceau As New SolidBrush(fond)
            g.FillEllipse(pinceau, cercle)
        End Using
        Using stylo As New Pen(contour, 1.4F)
            g.DrawEllipse(stylo, cercle)
        End Using
    End Sub

#End Region

End Class