' -------------------------------------------------------------------------------------------------
' Module      : CheminsDomaineHelper
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 20/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Rangement du logo facultatif des domaines, sur le même principe que les images du cabinet : la
' base ne stocke que le nom du fichier (ref_domaines.logo_fichier).
'
' Arborescence :
' {PATH_GENERAL}/
'   Domaines/
'     Domaine_{id}/
'       Logo.png
'
' Remarques   :
' - Dans le dossier de données (et non dans Assets, remplacé à chaque déploiement) : le logo est
'   compris dans les sauvegardes. Dossier nommé par l'identifiant : le libellé peut changer.
' - Logo enregistré en PNG réduit (TailleMaxLogo, proportions et transparence gardées) : léger et
'   net en miniature (choix de Joëlle, 20/10/2026).
' - Nom fixe : un nouvel import remplace le précédent. Aucune dépendance UI.
' - Copie, enregistrement et chargement passent par ImagesHelper.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.IO

Public Module CheminsDomaineHelper

    Public Const NomLogo As String = "Logo"

    ' Plus grand côté d'un logo enregistré (pixels) : assez pour la synthèse, léger pour les listes.
    Public Const TailleMaxLogo As Integer = 256

    Public Function GetDossierDomaine(idDomaine As ULong) As String
        Return Path.Combine(ArborescenceDonnees.GetRacineDonnees(),
                            ArborescenceDonnees.SousDossierDomaines,
                            ArborescenceDonnees.PrefixeDossierDomaine & idDomaine.ToString())
    End Function

    ' Réduit l'image, l'enregistre en PNG sous le nom fixe et retourne le nom de fichier à stocker.
    Public Function EnregistrerLogo(idDomaine As ULong, image As Drawing.Image) As String

        ArgumentNullException.ThrowIfNull(image)

        Using reduite As Drawing.Bitmap = ImagesHelper.Reduire(image, TailleMaxLogo)
            Return ImagesHelper.EnregistrerPngSousNomFixe(GetDossierDomaine(idDomaine), NomLogo, reduite)
        End Using

    End Function

    ' Charge le logo sans verrouiller le fichier ; Nothing s'il n'y en a pas ou s'il est illisible.
    Public Function ChargerLogo(idDomaine As ULong, nomFichier As String) As Drawing.Image
        If idDomaine = 0UL OrElse String.IsNullOrWhiteSpace(nomFichier) Then Return Nothing
        Return ImagesHelper.ChargerSansVerrou(Path.Combine(GetDossierDomaine(idDomaine), nomFichier))
    End Function

    ' Retire le logo du domaine (toutes ses versions).
    Public Sub SupprimerLogo(idDomaine As ULong)
        ImagesHelper.SupprimerImage(GetDossierDomaine(idDomaine), NomLogo)
    End Sub

    ' Retire le dossier entier d'un domaine supprimé de la base.
    Public Sub SupprimerDossier(idDomaine As ULong)
        Dim dossier As String = GetDossierDomaine(idDomaine)
        If Directory.Exists(dossier) Then Directory.Delete(dossier, recursive:=True)
    End Sub

End Module
