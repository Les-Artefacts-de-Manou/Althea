' -------------------------------------------------------------------------------------------------
' Module      : CheminsUtilisateurHelper
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 12/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Gère le stockage de la photo propre des comptes utilisateurs, sur le même principe que la photo
' patient et les images du cabinet : la base ne stocke que le nom du fichier.
'
' Arborescence :
' {PATH_GENERAL}/
'   Utilisateurs/
'     Utilisateur_{id}/
'       Photo.jpg
'
' Remarques   :
' - Nom fixe : un nouvel import remplace le précédent (quelle que soit l'extension).
' - Rangement par identifiant (et non par login) : le login peut changer.
' - Aucune dépendance UI ; copie et chargement passent par ImagesHelper.
' - Noms des dossiers : ArborescenceDonnees.
' - Un compte lié à un praticien affiche la photo du praticien (règle portée par PhotosUtilisateur).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.IO

Public Module CheminsUtilisateurHelper

    ' Nom fixe de la photo (l'extension suit le fichier importé)
    Public Const NomPhoto As String = "Photo"

    Public Function GetDossierUtilisateur(idUtilisateur As Long) As String
        Return Path.Combine(ArborescenceDonnees.GetRacineDonnees(), ArborescenceDonnees.SousDossierUtilisateurs,
                            ArborescenceDonnees.PrefixeDossierUtilisateur & idUtilisateur.ToString())
    End Function

    ''' <summary>
    ''' Copie la photo choisie sous le nom fixe du compte (remplace la précédente) et retourne le
    ''' nom de fichier à stocker en base.
    ''' </summary>
    Public Function ImporterPhoto(idUtilisateur As Long, cheminSource As String) As String
        Return ImagesHelper.CopierSousNomFixe(GetDossierUtilisateur(idUtilisateur), NomPhoto, cheminSource)
    End Function

    ''' <summary>Charge la photo propre du compte sans verrouiller le fichier ; Nothing si absente ou illisible.</summary>
    Public Function ChargerPhoto(idUtilisateur As Long, nomFichier As String) As Drawing.Image
        If String.IsNullOrWhiteSpace(nomFichier) Then Return Nothing
        Return ImagesHelper.ChargerSansVerrou(Path.Combine(GetDossierUtilisateur(idUtilisateur), nomFichier))
    End Function

End Module
