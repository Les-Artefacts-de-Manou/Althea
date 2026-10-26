' -------------------------------------------------------------------------------------------------
' Module      : CheminsCabinetHelper
' Projet      : Althéa
' Version     : V1.2.0
' Date        : 12/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Gère le stockage des images du cabinet et des praticiens (logo, photo, cachet, signature),
' sur le même principe que la photo patient : la base ne stocke que le nom du fichier.
'
' Arborescence :
' {PATH_GENERAL}/
'   Cabinet/
'     Logo.png
'     Praticien_{id}/
'       Photo.jpg | Cachet.png | Signature.png
'
' Remarques   :
' - Noms fixes : un nouvel import remplace le précédent.
' - Aucune dépendance UI.
' - Noms des dossiers : ArborescenceDonnees (structure fixe du dossier de données).
' - Copie, enregistrement et chargement passent par ImagesHelper (import d'image unifié, 11/10/2026).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.IO

Public Module CheminsCabinetHelper

    Public Const NomLogo As String = "Logo"
    Public Const NomPhoto As String = "Photo"
    Public Const NomCachet As String = "Cachet"
    Public Const NomSignature As String = "Signature"

    Public Function GetDossierCabinet() As String
        Return Path.Combine(ArborescenceDonnees.GetRacineDonnees(), ArborescenceDonnees.SousDossierCabinet)
    End Function

    Public Function GetDossierPraticien(idPraticien As ULong) As String
        Return Path.Combine(GetDossierCabinet(), ArborescenceDonnees.PrefixeDossierPraticien & idPraticien.ToString())
    End Function

    ''' <summary>Chemin complet d'un fichier cabinet (idPraticien = 0 : dossier du cabinet).</summary>
    Public Function GetCheminFichier(idPraticien As ULong, nomFichier As String) As String
        If String.IsNullOrWhiteSpace(nomFichier) Then Return String.Empty
        Return Path.Combine(GetDossier(idPraticien), nomFichier)
    End Function

    ''' <summary>
    ''' Copie une image source sous un nom fixe (remplace les versions précédentes de même nom
    ''' quelle que soit l'extension) et retourne le nom de fichier à stocker en base.
    ''' </summary>
    Public Function ImporterImage(idPraticien As ULong, nomFixe As String, cheminSource As String) As String
        Return ImagesHelper.CopierSousNomFixe(GetDossier(idPraticien), nomFixe, cheminSource)
    End Function

    ''' <summary>Enregistre une image générée (PNG) sous un nom fixe et retourne le nom de fichier.</summary>
    Public Function EnregistrerImagePng(idPraticien As ULong, nomFixe As String, image As Drawing.Image) As String
        Return ImagesHelper.EnregistrerPngSousNomFixe(GetDossier(idPraticien), nomFixe, image)
    End Function

    ''' <summary>Charge une image sans verrouiller le fichier ; Nothing si absente ou illisible.</summary>
    Public Function ChargerImage(idPraticien As ULong, nomFichier As String) As Drawing.Image
        Return ImagesHelper.ChargerSansVerrou(GetCheminFichier(idPraticien, nomFichier))
    End Function

    ' Dossier de rangement : celui du cabinet (idPraticien = 0) ou celui du praticien.
    Private Function GetDossier(idPraticien As ULong) As String
        Return If(idPraticien = 0UL, GetDossierCabinet(), GetDossierPraticien(idPraticien))
    End Function

End Module
