' -------------------------------------------------------------------------------------------------
' Module      : CheminsPatientHelper
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 11/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Reconstruit de façon déterministe les chemins physiques liés à un patient à partir de son
' code patient, sans jamais stocker de chemin absolu en base (D-Q4 / D-Q13).
'
' Responsabilités :
' - Lire la racine des données métier depuis tec_parametres (clé PATH_GENERAL)
' - Lire le sous-dossier des documents depuis tec_parametres (clé PATH_DOCUMENT)
' - Construire le dossier d'un patient : {PATH_GENERAL}/{PATH_DOCUMENT}/{code_patient}
' - Construire le chemin complet d'un fichier patient à partir de son nom seul
' - Fournir le nom de fichier déterministe de la photo d'identité (nom fixe, sans timestamp)
' - Garantir l'existence du dossier patient (création à la demande)
' - Formater un code patient à partir de son identifiant (règle PA + 6 chiffres)
'
' Arborescence (référence métier §8.1) :
' {PATH_GENERAL}/                 → ex. D:\Althea_Data
'   {PATH_DOCUMENT}/              → ex. Documents
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
' - GestionParametres (lecture de PATH_GENERAL et PATH_DOCUMENT)
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

    ' Clé du paramètre tec_parametres contenant la racine des données métier.
    Private Const CleRacineDonnees As String = "PATH_GENERAL"

    ' Clé du paramètre tec_parametres contenant le sous-dossier des documents.
    Private Const CleDossierDocuments As String = "PATH_DOCUMENT"

    ' Préfixe fixe du fichier photo d'identité (nom déterministe, sans timestamp, §8.2).
    Private Const PrefixePhotoIdentite As String = "Identite"

    ' Extension par défaut de la photo d'identité si aucune n'est fournie.
    Private Const ExtensionPhotoParDefaut As String = ".jpg"

    ' Préfixe du code patient (règle métier : PA + identifiant sur 6 chiffres, ex. PA000003).
    Private Const PrefixeCodePatient As String = "PA"

    ' Format de l'identifiant patient dans le code patient (6 chiffres zéro-paddés, ex. 000003).
    Private Const FormatIdentifiant As String = "D6"

#End Region

#Region "Racine"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetRacineDonnees
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Retourne la racine des données métier lue depuis tec_parametres (clé PATH_GENERAL).
    '
    ' Retour     :
    ' - String : Chemin racine configuré (ex : "C:\...\Althea_Data")
    '
    ' Exceptions :
    ' - InvalidOperationException : si la clé PATH_GENERAL est absente ou vide
    ' -------------------------------------------------------------------------------------------------
    Public Function GetRacineDonnees() As String

        Dim racine As String = GestionParametres.GetValeurParametre(CleRacineDonnees)

        If String.IsNullOrWhiteSpace(racine) Then
            Throw New InvalidOperationException(
                "La racine des données métier (paramètre '" & CleRacineDonnees &
                "') n'est pas configurée. Vérifiez les paramètres de l'application.")
        End If

        Return racine.Trim()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetSousDossierDocuments
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Rôle       :
    ' Retourne le nom du sous-dossier des documents lu depuis tec_parametres (clé PATH_DOCUMENT).
    '
    ' Retour     :
    ' - String : Nom du sous-dossier des documents (ex : "Documents")
    '
    ' Exceptions :
    ' - InvalidOperationException : si la clé PATH_DOCUMENT est absente ou vide
    ' -------------------------------------------------------------------------------------------------
    Public Function GetSousDossierDocuments() As String

        Dim sousDossier As String = GestionParametres.GetValeurParametre(CleDossierDocuments)

        If String.IsNullOrWhiteSpace(sousDossier) Then
            Throw New InvalidOperationException(
                "Le sous-dossier des documents (paramètre '" & CleDossierDocuments &
                "') n'est pas configuré. Vérifiez les paramètres de l'application.")
        End If

        Return sousDossier.Trim()

    End Function

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
    ' Retourne le dossier racine des documents : {PATH_GENERAL}/{PATH_DOCUMENT}.
    '
    ' Retour     :
    ' - String : Chemin du dossier des documents (ex : "D:\Althea_Data\Documents")
    ' -------------------------------------------------------------------------------------------------
    Public Function GetDossierDocuments() As String
        Return Path.Combine(GetRacineDonnees(), GetSousDossierDocuments())
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetDossierPatient
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Rôle       :
    ' Retourne le dossier d'un patient donné : {PATH_GENERAL}/{PATH_DOCUMENT}/{code_patient}.
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

#End Region

#Region "Photo d'identité"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetNomFichierPhotoIdentite
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Construit le nom de fichier déterministe de la photo d'identité (nom fixe + extension).
    '
    ' Paramètres :
    ' - extension : Extension du fichier source (ex : ".jpg", "png", ".PNG"...) ; ".jpg" par défaut
    '
    ' Retour     :
    ' - String : Nom de fichier de la photo d'identité (ex : "Identite.jpg")
    '
    ' Remarques  :
    ' - Nom FIXE (sans timestamp) car la photo d'identité remplace la précédente (§8.2)
    ' - L'extension est normalisée en minuscules et préfixée d'un point si nécessaire
    ' - C'est cette valeur qui est stockée dans patients.photo_fichier (nom seul, D-Q4)
    ' -------------------------------------------------------------------------------------------------
    Public Function GetNomFichierPhotoIdentite(Optional extension As String = ExtensionPhotoParDefaut) As String

        Dim ext As String = If(extension, String.Empty).Trim()

        If ext.Length = 0 Then
            ext = ExtensionPhotoParDefaut
        ElseIf Not ext.StartsWith(".", StringComparison.Ordinal) Then
            ext = "." & ext
        End If

        Return PrefixePhotoIdentite & ext.ToLowerInvariant()

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

#End Region

End Module
