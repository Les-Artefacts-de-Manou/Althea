' -------------------------------------------------------------------------------------------------
' Module      : CheminsDossierHelper
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 12/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Reconstruit de façon déterministe les chemins physiques liés à un dossier de suivi à partir
' du code patient et du code dossier, sans jamais stocker de chemin absolu en base (D-Q4 / D-DOS-3).
'
' Responsabilités :
' - Construire le dossier physique d'un dossier de suivi :
'   {PATH_GENERAL}/Documents/{code_patient}/{code_dossier}
' - Construire le chemin complet d'un fichier de dossier à partir de son nom seul
' - Garantir l'existence du dossier de suivi (création à la demande)
' - Formater un code dossier à partir de son identifiant (règle DO + 6 chiffres)
'
' Arborescence (référence métier §8 du plan Dossiers) :
' {PATH_GENERAL}/                     → ex. C:\Users\Joelle\OneDrive\Althea_Data
'   Documents/                        → sous-dossier fixe (ArborescenceDonnees)
'     {code_patient}/                → ex. PA000003
'       {code_dossier}/             → ex. DO000012
'         suivi_DO000012_motif_*.pdf → exports documentaires niveau dossier (zones cliniques)
'
' Remarques   :
' - S'appuie sur ArborescenceDonnees (racine, sous-dossier Documents) et CheminsPatientHelper (dossier patient)
' - La DB ne stocke aucun chemin ; il est toujours recalculable depuis (code_patient, code_dossier)
' - Le dossier de suivi est nommé par son code_dossier (DO000012), plus parlant que l'id
' - Aucune dépendance UI : module utilitaire pur côté système de fichiers
'
' Dépendances :
' - CheminsPatientHelper (construction du dossier patient parent)
' - System.IO (Path, Directory)
'
' Imports :
' - System.IO
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.IO

Public Module CheminsDossierHelper

#Region "Constantes"

    ' Préfixe du code dossier (règle métier : DO + identifiant sur 6 chiffres, ex. DO000012).
    Private Const PrefixeCodeDossier As String = "DO"

    ' Format de l'identifiant dossier dans le code dossier (6 chiffres zéro-paddés, ex. 000012).
    Private Const FormatIdentifiant As String = "D6"

#End Region

#Region "Code dossier"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : FormaterCodeDossier
    ' Version    : V1.0.0
    ' Date       : 21/07/2026
    '
    ' Rôle       :
    ' Construit le code dossier déterministe à partir de son identifiant (règle DO + 6 chiffres).
    '
    ' Paramètres :
    ' - idDossier : Identifiant du dossier (Long)
    '
    ' Retour     :
    ' - String : Code dossier (ex : "DO000012")
    '
    ' Remarques  :
    ' - Reproduit la colonne générée dossiers.code_dossier = concat('DO', lpad(id_dossier, 6, '0'))
    ' - Utile en création quand seul l'id fraîchement généré est connu (pas encore relu en base)
    ' -------------------------------------------------------------------------------------------------
    Public Function FormaterCodeDossier(idDossier As Long) As String
        Return PrefixeCodeDossier & idDossier.ToString(FormatIdentifiant)
    End Function

#End Region

#Region "Dossiers de suivi"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetDossierDuDossier
    ' Version    : V1.0.0
    ' Date       : 21/07/2026
    '
    ' Rôle       :
    ' Retourne le dossier physique d'un dossier de suivi donné :
    ' {PATH_GENERAL}/Documents/{code_patient}/{code_dossier}.
    '
    ' Paramètres :
    ' - codePatient : Code patient (ex : "PA000003")
    ' - codeDossier : Code dossier (ex : "DO000012")
    '
    ' Retour     :
    ' - String : Chemin du dossier de suivi
    '
    ' Remarques  :
    ' - S'appuie sur CheminsPatientHelper.GetDossierPatient (qui valide déjà le code patient)
    ' - Le dossier est nommé par le code dossier (DO000012), plus parlant que l'id
    ' - Ne crée pas le dossier (voir AssurerDossierDuDossier)
    ' -------------------------------------------------------------------------------------------------
    Public Function GetDossierDuDossier(codePatient As String, codeDossier As String) As String

        If String.IsNullOrWhiteSpace(codeDossier) Then
            Throw New ArgumentException(
                "Le code dossier est obligatoire pour construire son dossier physique.", NameOf(codeDossier))
        End If

        Return Path.Combine(CheminsPatientHelper.GetDossierPatient(codePatient), codeDossier.Trim())

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : AssurerDossierDuDossier
    ' Version    : V1.0.0
    ' Date       : 21/07/2026
    '
    ' Rôle       :
    ' Garantit l'existence du dossier physique du dossier de suivi et le retourne (le crée si besoin).
    '
    ' Paramètres :
    ' - codePatient : Code patient (ex : "PA000003")
    ' - codeDossier : Code dossier (ex : "DO000012")
    '
    ' Retour     :
    ' - String : Chemin du dossier de suivi (existant)
    '
    ' Remarques  :
    ' - À appeler avant d'écrire un fichier (export PDF/Word d'une zone clinique) dans le dossier
    ' - Directory.CreateDirectory crée aussi l'arborescence parente (dossier patient) si nécessaire
    ' -------------------------------------------------------------------------------------------------
    Public Function AssurerDossierDuDossier(codePatient As String, codeDossier As String) As String

        Dim dossier As String = GetDossierDuDossier(codePatient, codeDossier)

        If Not Directory.Exists(dossier) Then
            Directory.CreateDirectory(dossier)
        End If

        Return dossier

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetCheminFichierDossier
    ' Version    : V1.0.0
    ' Date       : 21/07/2026
    '
    ' Rôle       :
    ' Reconstruit le chemin complet d'un fichier de dossier de suivi à partir de son nom seul.
    '
    ' Paramètres :
    ' - codePatient : Code patient (ex : "PA000003")
    ' - codeDossier : Code dossier (ex : "DO000012")
    ' - nomFichier  : Nom du fichier (ex : "suivi_DO000012_motif_20260721_184741.pdf")
    '
    ' Retour     :
    ' - String : Chemin complet du fichier, ou Nothing si nomFichier est vide
    '
    ' Remarques  :
    ' - Jumeau de CheminsPatientHelper.GetCheminFichierPatient, un niveau plus bas
    ' - Ne vérifie pas l'existence physique du fichier (responsabilité de l'appelant)
    ' -------------------------------------------------------------------------------------------------
    Public Function GetCheminFichierDossier(codePatient As String, codeDossier As String, nomFichier As String) As String

        If String.IsNullOrWhiteSpace(nomFichier) Then
            Return Nothing
        End If

        Return Path.Combine(GetDossierDuDossier(codePatient, codeDossier), nomFichier.Trim())

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetCheminRelatifFichierDossier
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Construit le chemin RELATIF (à PATH_GENERAL) d'un fichier de niveau dossier, tel que stocké
    ' dans documents.chemin_relatif : Documents/{code_patient}/{code_dossier}/{nom_fichier}.
    '
    ' Paramètres :
    ' - codePatient : Code patient (ex : "PA000003")
    ' - codeDossier : Code dossier (ex : "DO000012")
    ' - nomFichier  : Nom du fichier (ex : "suivi_DO000012_motif_20260721_184741.pdf")
    '
    ' Retour     :
    ' - String : Chemin relatif (ex : "Documents\PA000003\DO000012\suivi_...pdf")
    '
    ' Remarques  :
    ' - Ne contient jamais PATH_GENERAL : reste valable si la racine change de machine (D-Q4)
    ' - Reconstruction du chemin absolu : Path.Combine(ArborescenceDonnees.GetRacineDonnees(), cheminRelatif)
    ' -------------------------------------------------------------------------------------------------
    Public Function GetCheminRelatifFichierDossier(codePatient As String, codeDossier As String, nomFichier As String) As String

        If String.IsNullOrWhiteSpace(codePatient) Then
            Throw New ArgumentException(
                "Le code patient est obligatoire pour construire le chemin relatif.", NameOf(codePatient))
        End If

        If String.IsNullOrWhiteSpace(codeDossier) Then
            Throw New ArgumentException(
                "Le code dossier est obligatoire pour construire le chemin relatif.", NameOf(codeDossier))
        End If

        If String.IsNullOrWhiteSpace(nomFichier) Then
            Throw New ArgumentException(
                "Le nom de fichier est obligatoire pour construire le chemin relatif.", NameOf(nomFichier))
        End If

        Return Path.Combine(
            ArborescenceDonnees.SousDossierDocuments,
            codePatient.Trim(),
            codeDossier.Trim(),
            nomFichier.Trim())

    End Function

#End Region

End Module
