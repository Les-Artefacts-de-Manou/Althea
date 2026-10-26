' -------------------------------------------------------------------------------------------------
' Module      : LogosDomaines
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 20/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Miniatures des logos de domaine pour l'affichage (bandeau du dossier, liste des domaines,
' convention du domaine), lues une fois puis gardées en mémoire.
'
' Remarques   :
' - Une miniature par domaine et par taille, calculée en haute qualité (ImagesHelper.Reduire) :
'   une liste qui se redessine ne relit pas le disque.
' - Les images rendues sont partagées : l'appelant ne les libère pas (pas de
'   UtilsImages.RemplacerImage sur une PictureBox qui en affiche une).
' - Oublier(idDomaine) après un changement de logo ; les miniatures retirées ne sont pas libérées
'   (un écran peut encore les afficher) : le ramasse-miettes s'en charge.
' - Une lecture impossible (fichier absent, dossier de données non configuré) est journalisée une
'   fois et donne « pas de logo ».
'
' - V1.1.0 (20/10/2026) : miniature dans un cadre largeur × hauteur (logos en longueur lisibles).
'
' Dépendances :
' - CheminsDomaineHelper, ImagesHelper, GestionLog
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module LogosDomaines

    ' Clé « id|fichier|largeurxhauteur » → miniature (Nothing = pas de logo lisible, mémorisé aussi).
    Private ReadOnly _miniatures As New Dictionary(Of String, Image)(StringComparer.OrdinalIgnoreCase)

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : Miniature
    ' Version    : V1.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       : Logo du domaine réduit pour tenir dans largeurMax × hauteurMax (proportions
    '              gardées) ; Nothing si le domaine n'a pas de logo ou s'il est illisible.
    ' Remarques  : Un cadre et non un carré : les logos sont souvent en longueur (relevé de Joëlle,
    '              20/10/2026 : dans un carré de 18 px, illisibles).
    ' -------------------------------------------------------------------------------------------------
    Public Function Miniature(idDomaine As ULong, logoFichier As String, largeurMax As Integer, hauteurMax As Integer) As Image

        If idDomaine = 0UL OrElse String.IsNullOrWhiteSpace(logoFichier) OrElse largeurMax <= 0 OrElse hauteurMax <= 0 Then Return Nothing

        Dim cle As String = $"{idDomaine}|{logoFichier}|{largeurMax}x{hauteurMax}"
        Dim existante As Image = Nothing
        If _miniatures.TryGetValue(cle, existante) Then Return existante

        Dim mini As Image = Nothing
        Try
            Using logo As Image = CheminsDomaineHelper.ChargerLogo(idDomaine, logoFichier)
                If logo IsNot Nothing Then mini = ImagesHelper.ReduireDans(logo, largeurMax, hauteurMax)
            End Using
        Catch ex As Exception
            GestionLog.EcrireException($"Logo du domaine {idDomaine} illisible.", ex,
                                       GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI)
        End Try

        _miniatures(cle) = mini
        Return mini

    End Function

    ' Logo du domaine changé ou retiré : ses miniatures seront recalculées.
    Public Sub Oublier(idDomaine As ULong)

        Dim prefixe As String = idDomaine.ToString() & "|"
        For Each cle As String In _miniatures.Keys.Where(Function(k) k.StartsWith(prefixe, StringComparison.Ordinal)).ToList()
            _miniatures.Remove(cle)
        Next

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Dessiner
    ' Version    : V1.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       : Dessine une miniature dans une zone (listes dessinées par l'écran), calée à gauche
    '              et centrée en hauteur ; rien si image est Nothing.
    ' -------------------------------------------------------------------------------------------------
    Public Sub Dessiner(g As Graphics, image As Image, zone As Rectangle)

        If image Is Nothing Then Return

        Dim x As Integer = zone.Left
        Dim y As Integer = zone.Top + (zone.Height - image.Height) \ 2
        g.DrawImage(image, x, y, image.Width, image.Height)

    End Sub

End Module
