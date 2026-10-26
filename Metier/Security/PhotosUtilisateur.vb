' -------------------------------------------------------------------------------------------------
' Module      : PhotosUtilisateur
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 12/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Porte la règle d'affichage de la photo d'un compte utilisateur (décision du 12/10/2026) :
' 1. compte lié à un praticien qui a une photo → la photo de la fiche praticien ;
' 2. sinon → la photo propre du compte (sec_utilisateurs.photo_fichier) ;
' 3. à défaut → aucune (l'interface affiche les initiales).
'
' Remarques   :
' - Un seul endroit pour cette règle : bandeau de la Home et fiche utilisateur s'appuient dessus.
' - L'image retournée appartient à l'appelant (à libérer quand elle est remplacée).
'
' Dépendances :
' - GestionCabinet (praticien lié), CheminsCabinetHelper (photo du praticien)
' - GestionUtilisateurs, CheminsUtilisateurHelper (photo propre)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module PhotosUtilisateur

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : PraticienLie
    ' Rôle     : Praticien dont ce compte est le compte de travail, ou Nothing.
    ' -------------------------------------------------------------------------------------------------
    Public Function PraticienLie(idUtilisateur As Long) As Praticien
        If idUtilisateur <= 0 Then Return Nothing
        Return GestionCabinet.GetPraticienParUtilisateur(CULng(idUtilisateur))
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ChargerPhotoAffichee
    ' Rôle     : Photo à afficher pour un compte selon la règle ci-dessus ; Nothing si aucune.
    '
    ' Paramètres :
    ' - idUtilisateur : compte concerné
    ' - photoFichier  : photo propre déjà connue de l'appelant ; si Nothing, elle est relue en base
    '                   (cas du compte connecté, dont l'objet en mémoire peut être ancien)
    ' -------------------------------------------------------------------------------------------------
    Public Function ChargerPhotoAffichee(idUtilisateur As Long, Optional photoFichier As String = Nothing) As Drawing.Image

        If idUtilisateur <= 0 Then Return Nothing

        Dim praticien As Praticien = PraticienLie(idUtilisateur)
        If praticien IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(praticien.CheminPhoto) Then
            Dim photoPraticien = CheminsCabinetHelper.ChargerImage(praticien.IdPraticien, praticien.CheminPhoto)
            If photoPraticien IsNot Nothing Then Return photoPraticien
        End If

        Dim nom As String = If(photoFichier, GestionUtilisateurs.GetPhotoUtilisateur(idUtilisateur))
        Return CheminsUtilisateurHelper.ChargerPhoto(idUtilisateur, nom)

    End Function

End Module
