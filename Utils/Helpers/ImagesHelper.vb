' -------------------------------------------------------------------------------------------------
' Module      : ImagesHelper
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 20/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Import d'image unifié (sans interface) : formats acceptés, contrôle qu'un fichier est réellement
' une image, copie sous un nom fixe et chargement sans verrou. Partagé par la photo patient, les
' images du cabinet et du praticien, et demain le logo de domaine.
'
' Responsabilités :
' - Tenir la liste unique des formats d'image acceptés
' - Vérifier qu'un fichier choisi est une image lisible (et pas seulement une extension)
' - Copier une image sous un nom fixe en supprimant ses versions précédentes, quelle que soit leur
'   extension ; enregistrer de même une image générée (PNG)
' - Charger une image en mémoire sans garder le fichier verrouillé
'
' Remarques   :
' - Aucune dépendance UI : les dialogues et les PictureBox relèvent de UtilsImages (Utils/UI).
' - Les règles de rangement (dossier, nom fixe) restent chez les helpers de chemins
'   (CheminsPatientHelper, CheminsCabinetHelper) : ce module ne connaît que des dossiers.
' - Le fichier choisi est copié tel quel (CopierSousNomFixe) ; seul le logo d'un domaine est réduit
'   avant d'être enregistré (Reduire, choix de Joëlle du 20/10/2026).
' - V1.1.0 (20/10/2026) : Reduire / ReduireDans (cadre largeur × hauteur) ; nouvelle image écrite avant de retirer l'ancienne (audit § 4.5) ;
'   SupprimerImage public (retrait d'un logo).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.IO

Public Module ImagesHelper

#Region "Formats"

    ' Extensions acceptées partout (photo patient, cabinet, praticien, logos).
    Private ReadOnly ExtensionsAcceptees As String() = {".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff"}

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : FiltreDialogue
    ' Rôle      : Filtre « Images » d'un OpenFileDialog, construit depuis la liste des formats acceptés.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property FiltreDialogue As String
        Get
            Dim motifs As String = String.Join(";", ExtensionsAcceptees.Select(Function(ext) "*" & ext))
            Return $"Images ({motifs})|{motifs}"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : LibelleFormats
    ' Rôle      : Liste lisible des formats pour les messages (« JPG, PNG, BMP, GIF, TIFF »).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property LibelleFormats As String
        Get
            Return "JPG, PNG, BMP, GIF, TIFF"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : EstImageValide
    ' Version   : V1.0.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Indique si un fichier peut être importé comme image : extension acceptée ET contenu réellement
    ' lisible comme image.
    '
    ' Paramètres :
    ' - chemin : Chemin complet du fichier choisi
    '
    ' Retour    :
    ' - Boolean : True si le fichier est une image lisible d'un format accepté
    '
    ' Remarques :
    ' - L'extension seule ne suffit pas : un fichier renommé ou abîmé serait enregistré puis
    '   resterait invisible. On l'ouvre donc réellement (sans verrou durable).
    ' -------------------------------------------------------------------------------------------------
    Public Function EstImageValide(chemin As String) As Boolean

        If String.IsNullOrWhiteSpace(chemin) OrElse Not File.Exists(chemin) Then Return False

        Dim extension As String = Path.GetExtension(chemin).ToLowerInvariant()
        If Array.IndexOf(ExtensionsAcceptees, extension) < 0 Then Return False

        Try
            Using fs As New FileStream(chemin, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                Using img As Drawing.Image = Drawing.Image.FromStream(fs, useEmbeddedColorManagement:=False, validateImageData:=True)
                    Return img.Width > 0 AndAlso img.Height > 0
                End Using
            End Using
        Catch ex As Exception
            GestionLog.EcrireLog($"Fichier refusé à l'import d'image (illisible) : {Path.GetFileName(chemin)}",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
            Return False
        End Try

    End Function

#End Region

#Region "Copie et chargement"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : CopierSousNomFixe
    ' Version   : V1.1.0
    ' Date      : 20/10/2026
    '
    ' Rôle      :
    ' Copie une image dans un dossier sous un nom fixe (« Identite », « Logo »…), en conservant son
    ' extension, après avoir supprimé les versions précédentes de ce nom quelle que soit leur extension.
    '
    ' Paramètres :
    ' - dossier      : Dossier de destination (créé si nécessaire)
    ' - nomFixe      : Nom sans extension
    ' - cheminSource : Fichier image choisi
    '
    ' Retour    :
    ' - String : Nom du fichier écrit (ex. "Logo.png"), à stocker en base (nom seul)
    '
    ' Remarques :
    ' - Les versions précédentes sont toutes retirées : Logo.png ne remplacerait pas un ancien Logo.jpg.
    ' - Une ancienne version verrouillée fait échouer l'import (exception) plutôt que de laisser
    '   deux fichiers de même nom fixe.
    ' - V1.1.0 (20/10/2026) : écrite à part avant de retirer l'ancienne version (Remplacer, audit § 4.5).
    ' -------------------------------------------------------------------------------------------------
    Public Function CopierSousNomFixe(dossier As String, nomFixe As String, cheminSource As String) As String

        If Not File.Exists(cheminSource) Then
            Throw New FileNotFoundException("Fichier image introuvable.", cheminSource)
        End If

        Dim nomFichier As String = nomFixe & Path.GetExtension(cheminSource).ToLowerInvariant()
        Remplacer(dossier, nomFixe, nomFichier, Sub(temporaire) File.Copy(cheminSource, temporaire))
        Return nomFichier

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : EnregistrerPngSousNomFixe
    ' Version   : V1.1.0
    ' Date      : 20/10/2026
    '
    ' Rôle      :
    ' Enregistre une image produite par l'application (cachet généré, logo réduit) en PNG sous un
    ' nom fixe, à la place des versions précédentes de ce nom.
    '
    ' Retour    :
    ' - String : Nom du fichier écrit (« nomFixe.png »)
    ' - V1.1.0 (20/10/2026) : écrit d'abord à part, l'ancienne version n'est retirée qu'ensuite
    '   (Remplacer).
    ' -------------------------------------------------------------------------------------------------
    Public Function EnregistrerPngSousNomFixe(dossier As String, nomFixe As String, image As Drawing.Image) As String

        Dim nomFichier As String = nomFixe & ".png"
        Remplacer(dossier, nomFixe, nomFichier, Sub(temporaire) image.Save(temporaire, Drawing.Imaging.ImageFormat.Png))
        Return nomFichier

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Remplacer
    ' Version   : V1.0.0
    ' Date      : 20/10/2026
    '
    ' Rôle      :
    ' Écrit la nouvelle image dans un fichier temporaire du même dossier, puis retire les versions
    ' précédentes du nom fixe et met la nouvelle à leur place.
    '
    ' Remarques :
    ' - Audit du 18/10/2026 (§ 4.5) : l'ancienne image était supprimée avant la copie ; une copie ratée
    '   (fichier source illisible, disque plein) faisait perdre les deux.
    ' - Le nom temporaire (« ~Logo-….tmp ») ne répond pas au motif « Logo.* » des versions.
    ' -------------------------------------------------------------------------------------------------
    Private Sub Remplacer(dossier As String, nomFixe As String, nomFichier As String, ecrire As Action(Of String))

        Directory.CreateDirectory(dossier)
        Dim temporaire As String = Path.Combine(dossier, $"~{nomFixe}-{Guid.NewGuid():N}.tmp")

        Try
            ecrire(temporaire)
            SupprimerImage(dossier, nomFixe)
            File.Move(temporaire, Path.Combine(dossier, nomFichier))
        Finally
            If File.Exists(temporaire) Then File.Delete(temporaire)
        End Try

    End Sub

    ' Supprime toutes les versions d'un nom fixe (« Logo.* ») ; rien si le dossier n'existe pas.
    Public Sub SupprimerImage(dossier As String, nomFixe As String)

        If Not Directory.Exists(dossier) Then Return

        For Each ancien As String In Directory.GetFiles(dossier, nomFixe & ".*")
            File.Delete(ancien)
        Next

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : Reduire
    ' Version   : V1.0.0
    ' Date      : 20/10/2026
    '
    ' Rôle      :
    ' Copie de l'image ramenée à « tailleMax » pixels pour son plus grand côté, proportions et
    ' transparence gardées, en haute qualité ; une image déjà plus petite est copiée telle quelle.
    '
    ' Retour    :
    ' - Bitmap : nouvelle image (à libérer par l'appelant)
    ' -------------------------------------------------------------------------------------------------
    Public Function Reduire(image As Drawing.Image, tailleMax As Integer) As Drawing.Bitmap
        Return ReduireDans(image, tailleMax, tailleMax)
    End Function

    ' Même réduction, dans un cadre largeur × hauteur : un logo en longueur garde toute la largeur
    ' permise au lieu d'être tassé dans un carré.
    Public Function ReduireDans(image As Drawing.Image, largeurMax As Integer, hauteurMax As Integer) As Drawing.Bitmap

        ArgumentNullException.ThrowIfNull(image)
        If largeurMax <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(largeurMax))
        If hauteurMax <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(hauteurMax))

        Dim echelle As Double = Math.Min(1.0, Math.Min(largeurMax / CDbl(image.Width), hauteurMax / CDbl(image.Height)))
        Dim largeur As Integer = Math.Max(1, CInt(Math.Round(image.Width * echelle)))
        Dim hauteur As Integer = Math.Max(1, CInt(Math.Round(image.Height * echelle)))

        Dim resultat As New Drawing.Bitmap(largeur, hauteur, Drawing.Imaging.PixelFormat.Format32bppArgb)
        Using g As Drawing.Graphics = Drawing.Graphics.FromImage(resultat)
            g.Clear(Drawing.Color.Transparent)
            g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = Drawing.Drawing2D.PixelOffsetMode.HighQuality
            g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighQuality
            g.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality
            Using attributs As New Drawing.Imaging.ImageAttributes()
                ' Évite le liseré clair sur les bords (pixels du bord répétés plutôt que transparents).
                attributs.SetWrapMode(Drawing.Drawing2D.WrapMode.TileFlipXY)
                g.DrawImage(image, New Drawing.Rectangle(0, 0, largeur, hauteur), 0, 0, image.Width, image.Height,
                            Drawing.GraphicsUnit.Pixel, attributs)
            End Using
        End Using

        Return resultat

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : ChargerSansVerrou
    ' Version   : V1.0.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Charge une image en mémoire et relâche aussitôt le fichier (il peut ensuite être remplacé).
    '
    ' Paramètres :
    ' - chemin : Chemin complet du fichier
    '
    ' Retour    :
    ' - Image : copie en mémoire (à libérer par l'appelant), Nothing si absent ou illisible
    ' -------------------------------------------------------------------------------------------------
    Public Function ChargerSansVerrou(chemin As String) As Drawing.Image

        If String.IsNullOrWhiteSpace(chemin) OrElse Not File.Exists(chemin) Then Return Nothing

        Try
            Using fs As New FileStream(chemin, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                Using img As Drawing.Image = Drawing.Image.FromStream(fs)
                    Return New Drawing.Bitmap(img)
                End Using
            End Using
        Catch ex As Exception
            GestionLog.EcrireLog($"Image illisible : {Path.GetFileName(chemin)}",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
            Return Nothing
        End Try

    End Function

#End Region

End Module
