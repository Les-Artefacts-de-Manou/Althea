' -------------------------------------------------------------------------------------------------
' Module      : ArborescenceDonnees
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 20/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Décrit à un seul endroit l'arborescence du dossier de données : la racine, seul élément
' paramétrable (PATH_GENERAL), et les sous-dossiers fixes utilisés par les helpers de chemins.
'
' Arborescence :
' {PATH_GENERAL}/                 racine choisie à l'installation (ex. D:\Althea_Data)
'   Documents/                    documents des patients et des dossiers
'     {code_patient}/             ex. PA000003 — photo d'identité, exports (CheminsPatientHelper)
'       {code_dossier}/           ex. DO000012 — fichiers du dossier (CheminsDossierHelper)
'   Cabinet/                      logo du cabinet (CheminsCabinetHelper)
'     Praticien_{id}/             photo, signature, cachet du praticien
'   Utilisateurs/
'     Utilisateur_{id}/           photo propre d'un compte (CheminsUtilisateurHelper)
'   Domaines/
'     Domaine_{id}/               logo facultatif du domaine (CheminsDomaineHelper)
'
' Remarques   :
' - Ces noms sont la structure du programme, pas des réglages : la base stocke des chemins
'   relatifs qui les contiennent (documents.chemin_relatif = « Documents/PA…/DO…/fichier ») et les
'   photos sont retrouvées en recalculant leur dossier. Les changer rendrait des fichiers
'   introuvables : ils ne sont donc pas paramétrables (décision du 12/10/2026 ; le paramètre
'   PATH_DOCUMENT a été supprimé en v2.37.0).
' - Les chemins enregistrés restent relatifs à la racine : déplacer tout le dossier de données
'   (autre disque, autre machine) ne demande que de modifier PATH_GENERAL.
' - Un nouveau sous-dossier s'ajoute ici.
' - V1.1.0 (20/10/2026) : Domaines/Domaine_{id}/ (logos des domaines, migration v2.45.0).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module ArborescenceDonnees

    Public Const SousDossierDocuments As String = "Documents"
    Public Const SousDossierCabinet As String = "Cabinet"
    Public Const PrefixeDossierPraticien As String = "Praticien_"
    Public Const SousDossierUtilisateurs As String = "Utilisateurs"
    Public Const PrefixeDossierUtilisateur As String = "Utilisateur_"
    Public Const SousDossierDomaines As String = "Domaines"
    Public Const PrefixeDossierDomaine As String = "Domaine_"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetRacineDonnees
    ' Version    : V1.1.0 (reprise de CheminsPatientHelper, V1.0.0 du 11/06/2026)
    ' Date       : 12/10/2026
    '
    ' Rôle       :
    ' Retourne la racine du dossier de données (paramètre PATH_GENERAL).
    '
    ' Exceptions :
    ' - InvalidOperationException : paramètre absent ou vide (aucun repli possible : écrire des
    '   fichiers patients dans un dossier par défaut serait pire qu'une erreur claire)
    ' -------------------------------------------------------------------------------------------------
    Public Function GetRacineDonnees() As String

        Dim racine As String = GestionParametres.GetValeurParametre(ClesParametres.CheminRacineDonnees)

        If String.IsNullOrWhiteSpace(racine) Then
            Throw New InvalidOperationException(
                "La racine des données métier (paramètre '" & ClesParametres.CheminRacineDonnees &
                "') n'est pas configurée. Vérifiez les paramètres de l'application.")
        End If

        Return racine.Trim()

    End Function

End Module
