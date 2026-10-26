' -------------------------------------------------------------------------------------------------
' Module      : CheminsPatientHelper
' Projet      : Althéa
' Version     : V1.3.0
' Date        : 12/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Reconstruit de façon déterministe les chemins physiques liés à un patient à partir de son
' code patient, sans jamais stocker de chemin absolu en base (D-Q4 / D-Q13).
'
' Responsabilités :
' - Construire le dossier d'un patient : {PATH_GENERAL}/Documents/{code_patient}
' - Construire le chemin complet d'un fichier patient à partir de son nom seul
' - Importer la photo d'identité sous son nom fixe (Identite + extension d'origine)
' - Garantir l'existence du dossier patient (création à la demande)
' - Formater un code patient à partir de son identifiant (règle PA + 6 chiffres)
'
' Arborescence (référence métier §8.1) :
' {PATH_GENERAL}/                 → ex. D:\Althea_Data
'   Documents/                    → sous-dossier fixe (ArborescenceDonnees)
'     {code_patient}/            → ex. PA000003
'       Identite.jpg            → photo d'identité (nom fixe, remplace la précédente)
'       anamnese_PA000003_*.pdf → exports documentaires niveau patient
'
' Remarques   :
' - La DB ne stocke que le NOM du fichier (patients.photo_fichier), jamais le chemin (D-Q4)
' - Le chemin est toujours recalculable depuis (code_patient, nom de fichier)
' - Le dossier du patient est nommé par son code_patient (PA000003), plus parlant que l'id
' - La photo d'identité garde un nom FIXE (Identite + extension) car elle remplace la précédente (§8.2)
' - Aucune dépendance UI : module utilitaire pur côté système de fichiers
'
' Dépendances :
' - ArborescenceDonnees (racine du dossier de données et sous-dossier Documents)
' - System.IO (Path, Directory)
'
' Imports :
' - System.IO
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.IO

Public Module CheminsPatientHelper

#Region "Constantes"

    ' Préfixe fixe du fichier photo d'identité (nom déterministe, sans timestamp, §8.2).
    Private Const PrefixePhotoIdentite As String = "Identite"

    ' Préfixe du code patient (règle métier : PA + identifiant sur 6 chiffres, ex. PA000003).
    Private Const PrefixeCodePatient As String = "PA"

    ' Format de l'identifiant patient dans le code patient (6 chiffres zéro-paddés, ex. 000003).
    Private Const FormatIdentifiant As String = "D6"

#End Region

#Region "Code patient"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : FormaterCodePatient
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Rôle       :
    ' Construit le code patient déterministe à partir de son identifiant (règle PA + 6 chiffres).
    '
    ' Paramètres :
    ' - idPatient : Identifiant du patient (Long)
    '
    ' Retour     :
    ' - String : Code patient (ex : "PA000003")
    '
    ' Remarques  :
    ' - Reproduit la colonne générée patients.code_patient = concat('PA', lpad(id_patient, 6, '0'))
    ' - Utile en création quand seul l'id fraîchement généré est connu (pas encore relu en base)
    ' -------------------------------------------------------------------------------------------------
    Public Function FormaterCodePatient(idPatient As Long) As String
        Return PrefixeCodePatient & idPatient.ToString(FormatIdentifiant)
    End Function

#End Region

#Region "Dossiers patient"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetDossierDocuments
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Rôle       :
    ' Retourne le dossier racine des documents : {PATH_GENERAL}/Documents.
    '
    ' Retour     :
    ' - String : Chemin du dossier des documents (ex : "D:\Althea_Data\Documents")
    ' -------------------------------------------------------------------------------------------------
    Public Function GetDossierDocuments() As String
        Return Path.Combine(ArborescenceDonnees.GetRacineDonnees(), ArborescenceDonnees.SousDossierDocuments)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetDossierPatient
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Rôle       :
    ' Retourne le dossier d'un patient donné : {PATH_GENERAL}/Documents/{code_patient}.
    '
    ' Paramètres :
    ' - codePatient : Code patient (ex : "PA000003")
    '
    ' Retour     :
    ' - String : Chemin du dossier du patient
    '
    ' Remarques  :
    ' - Le dossier est nommé par le code patient (PA000003), plus parlant que l'id (D-Q préférence)
    ' - Ne crée pas le dossier (voir AssurerDossierPatient)
    ' -------------------------------------------------------------------------------------------------
    Public Function GetDossierPatient(codePatient As String) As String

        If String.IsNullOrWhiteSpace(codePatient) Then
            Throw New ArgumentException(
                "Le code patient est obligatoire pour construire son dossier.", NameOf(codePatient))
        End If

        Return Path.Combine(GetDossierDocuments(), codePatient.Trim())

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : AssurerDossierPatient
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Rôle       :
    ' Garantit l'existence du dossier du patient et le retourne (le crée si nécessaire).
    '
    ' Paramètres :
    ' - codePatient : Code patient (ex : "PA000003")
    '
    ' Retour     :
    ' - String : Chemin du dossier du patient (existant)
    '
    ' Remarques  :
    ' - À appeler avant d'écrire un fichier (photo, document) dans le dossier patient
    ' -------------------------------------------------------------------------------------------------
    Public Function AssurerDossierPatient(codePatient As String) As String

        Dim dossier As String = GetDossierPatient(codePatient)

        If Not Directory.Exists(dossier) Then
            Directory.CreateDirectory(dossier)
        End If

        Return dossier

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetCheminRelatifFichierPatient
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Construit le chemin RELATIF (à PATH_GENERAL) d'un fichier de niveau patient, tel que stocké
    ' dans documents.chemin_relatif : Documents/{code_patient}/{nom_fichier}.
    '
    ' Paramètres :
    ' - codePatient : Code patient (ex : "PA000003")
    ' - nomFichier  : Nom du fichier (ex : "anamnese_PA000003_20260609_101500.pdf")
    '
    ' Retour     :
    ' - String : Chemin relatif (ex : "Documents\PA000003\anamnese_PA000003_...pdf")
    '
    ' Remarques  :
    ' - Ne contient jamais PATH_GENERAL : le chemin reste valable si la racine change de machine (D-Q4)
    ' - Reconstruction du chemin absolu : Path.Combine(ArborescenceDonnees.GetRacineDonnees(), cheminRelatif)
    ' -------------------------------------------------------------------------------------------------
    Public Function GetCheminRelatifFichierPatient(codePatient As String, nomFichier As String) As String

        If String.IsNullOrWhiteSpace(codePatient) Then
            Throw New ArgumentException(
                "Le code patient est obligatoire pour construire le chemin relatif.", NameOf(codePatient))
        End If

        If String.IsNullOrWhiteSpace(nomFichier) Then
            Throw New ArgumentException(
                "Le nom de fichier est obligatoire pour construire le chemin relatif.", NameOf(nomFichier))
        End If

        Return Path.Combine(ArborescenceDonnees.SousDossierDocuments, codePatient.Trim(), nomFichier.Trim())

    End Function

#End Region

#Region "Photo d'identité"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ImporterPhotoIdentite
    ' Version    : V1.0.0
    ' Date       : 11/10/2026
    '
    ' Rôle       :
    ' Copie la photo choisie dans le dossier du patient sous le nom fixe « Identite » (extension
    ' d'origine conservée), en remplaçant toute photo d'identité précédente.
    '
    ' Paramètres :
    ' - codePatient  : Code patient (ex : "PA000003")
    ' - cheminSource : Fichier image choisi (déjà contrôlé par l'appelant)
    '
    ' Retour     :
    ' - String : Nom de fichier à stocker dans patients.photo_fichier (ex : "Identite.png")
    '
    ' Remarques  :
    ' - Nom FIXE (sans timestamp) car la photo d'identité remplace la précédente (§8.2).
    ' - La copie et la suppression des anciennes versions (Identite.*) passent par ImagesHelper
    '   (import d'image unifié, 11/10/2026).
    ' -------------------------------------------------------------------------------------------------
    Public Function ImporterPhotoIdentite(codePatient As String, cheminSource As String) As String

        Dim dossier As String = AssurerDossierPatient(codePatient)
        Return ImagesHelper.CopierSousNomFixe(dossier, PrefixePhotoIdentite, cheminSource)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetCheminFichierPatient
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Reconstruit le chemin complet d'un fichier patient à partir de son nom seul.
    '
    ' Paramètres :
    ' - codePatient : Code patient (ex : "PA000003")
    ' - nomFichier  : Nom du fichier stocké en base (ex : "Identite.jpg")
    '
    ' Retour     :
    ' - String : Chemin complet du fichier, ou Nothing si nomFichier est vide
    '
    ' Remarques  :
    ' - Retourne Nothing quand aucun fichier n'est associé (ex : patient sans photo → placeholder UI)
    ' - Ne vérifie pas l'existence physique du fichier (responsabilité de l'appelant)
    ' -------------------------------------------------------------------------------------------------
    Public Function GetCheminFichierPatient(codePatient As String, nomFichier As String) As String

        If String.IsNullOrWhiteSpace(nomFichier) Then
            Return Nothing
        End If

        Return Path.Combine(GetDossierPatient(codePatient), nomFichier.Trim())

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ChargerImagePatient
    ' Version    : V1.0.0
    ' Date       : 11/10/2026
    '
    ' Rôle       :
    ' Charge en mémoire une image du dossier d'un patient (photo d'identité notamment), sans garder
    ' le fichier verrouillé.
    '
    ' Paramètres :
    ' - codePatient : Code patient (ex : "PA000003")
    ' - nomFichier  : Nom du fichier seul (patients.photo_fichier)
    '
    ' Retour     :
    ' - Image : copie en mémoire, ou Nothing si aucun fichier, fichier absent ou illisible
    '
    ' Remarques  :
    ' - Délègue à ImagesHelper.ChargerSansVerrou (qui journalise un fichier illisible) ; l'appelant
    '   libère l'image reçue.
    ' - Utilisée par la fiche Patient et le récapitulatif de la fiche Dossier.
    ' -------------------------------------------------------------------------------------------------
    Public Function ChargerImagePatient(codePatient As String, nomFichier As String) As Drawing.Image

        If String.IsNullOrWhiteSpace(codePatient) OrElse String.IsNullOrWhiteSpace(nomFichier) Then
            Return Nothing
        End If

        Return ImagesHelper.ChargerSansVerrou(GetCheminFichierPatient(codePatient, nomFichier))

    End Function

#End Region

End Module
