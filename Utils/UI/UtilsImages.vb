' -------------------------------------------------------------------------------------------------
' Module      : UtilsImages
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 11/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Partie interface de l'import d'image unifié : choix du fichier (avec contrôle et message commun),
' confirmation avant de remplacer une image existante, remplacement d'une image affichée.
'
' Responsabilités :
' - Ouvrir le même dialogue de choix d'image partout et refuser un fichier qui n'est pas une image
' - Demander confirmation avant d'écraser une image existante (décision du 11/10/2026)
' - Remplacer l'image d'un PictureBox en libérant la précédente
'
' Dépendances :
' - ImagesHelper (formats, contrôle du fichier), DialogChoix (messages)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module UtilsImages

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : ChoisirFichierImage
    ' Version   : V1.0.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Ouvre le dialogue de choix d'une image et vérifie le fichier retenu.
    '
    ' Paramètres :
    ' - proprietaire : Fenêtre ou contrôle appelant (pour centrer le dialogue)
    ' - titre        : Titre du dialogue (« Logo du cabinet », « Photo d'identité »…)
    '
    ' Retour    :
    ' - String : chemin du fichier valide, ou Nothing (abandon ou fichier refusé, message affiché)
    ' -------------------------------------------------------------------------------------------------
    Public Function ChoisirFichierImage(proprietaire As IWin32Window, titre As String) As String

        Using dlg As New OpenFileDialog With {
            .Title = titre,
            .Filter = ImagesHelper.FiltreDialogue,
            .CheckFileExists = True,
            .Multiselect = False
        }
            If dlg.ShowDialog(proprietaire) <> DialogResult.OK Then Return Nothing

            If Not ImagesHelper.EstImageValide(dlg.FileName) Then
                DialogChoix.Erreur("Ce fichier n'est pas une image lisible ou son format n'est pas pris en charge." &
                                   Environment.NewLine & $"Formats acceptés : {ImagesHelper.LibelleFormats}.")
                Return Nothing
            End If

            Return dlg.FileName
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : ConfirmerRemplacement
    ' Version   : V1.0.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Demande confirmation avant de remplacer une image existante ; sans image existante, accepte.
    '
    ' Paramètres :
    ' - nomFichierExistant : nom actuellement enregistré (vide = pas d'image, rien à confirmer)
    ' - libelle            : désignation de l'image (« la photo d'identité », « le logo du cabinet »…)
    ' -------------------------------------------------------------------------------------------------
    Public Function ConfirmerRemplacement(nomFichierExistant As String, libelle As String) As Boolean

        If String.IsNullOrWhiteSpace(nomFichierExistant) Then Return True

        Return DialogChoix.Confirmer($"Une image existe déjà pour {libelle}." & Environment.NewLine &
                                     "Voulez-vous la remplacer par la nouvelle ?",
                                     "Remplacer l'image") = DialogResult.Yes

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplacerImage
    ' Version   : V1.0.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Affiche une nouvelle image dans un PictureBox et libère la précédente (handles GDI+).
    ' -------------------------------------------------------------------------------------------------
    Public Sub RemplacerImage(pic As PictureBox, nouvelleImage As Drawing.Image)

        If pic Is Nothing Then Return
        Dim ancienne As Drawing.Image = pic.Image
        pic.Image = nouvelleImage
        If ancienne IsNot Nothing AndAlso ancienne IsNot nouvelleImage Then ancienne.Dispose()

    End Sub

End Module
