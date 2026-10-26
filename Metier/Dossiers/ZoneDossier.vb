' -------------------------------------------------------------------------------------------------
' Fichier     : ZoneDossier.vb
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 08/07/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Décrit les 7 zones cliniques d'un dossier de suivi (brique B4 « Suivi clinique »).
'
' Responsabilités :
' - Enum ZoneDossier : identifie chaque zone de façon typée (clé de paramétrage de l'éditeur).
' - Class ZoneDossierInfo : métadonnées d'une zone (libellé, colonnes RTF/TXT, lecture seule).
' - Class ContenuZoneDossier : transporte le contenu (RTF + texte brut) d'une zone entre couches.
' - Module ZonesDossier : registre central (whitelist) reliant chaque zone à ses métadonnées.
'
' Important   :
' - Les noms de colonnes proviennent EXCLUSIVEMENT de ce registre (whitelist) : aucune valeur
'   utilisateur n'est jamais concaténée dans une requête SQL (protection anti-injection).
' - Règle double colonne ADR-15 : chaque zone stocke un format riche (xxx_rtf) et un texte brut
'   (xxx_txt, pour recherche/aperçu rapide demandé par l'utilisatrice).
' - La zone HistoriqueArchive est en lecture seule (D-DOS-4) : renseignée à l'archivage (B6).
'
' Dépendances :
' - Utilisé par QueryDossiers (construction SQL ciblée) et GestionDossiers (lecture/écriture zone).
' - Utilisé par l'UI (UC_DossierFiche onglet Suivi clinique, modale DossierZoneEditor).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

' -------------------------------------------------------------------------------------------------
' Énumération : ZoneDossier
' Version     : V1.1.0
' Date        : 08/07/2026
'
' Rôle        :
' Identifie de façon typée chacune des sept zones cliniques d'un dossier.
'
' Remarques   :
' - Sert de clé au registre ZonesDossier et ne doit pas être remplacée par une chaîne libre.
' -------------------------------------------------------------------------------------------------
Public Enum ZoneDossier
    MotifSuivi
    Historique
    Antecedents
    Diagnostic
    TraitementsEnCours
    ObjectifsSuivi
    HistoriqueArchive
End Enum

' -------------------------------------------------------------------------------------------------
' Classe      : ZoneDossierInfo
' Version     : V1.1.0
' Date        : 08/07/2026
'
' Rôle        :
' Porte les métadonnées d'une zone clinique : libellé UI, colonnes de stockage et lecture seule.
'
' Important   :
' - DTO interne au registre, sans accès à la base ni logique UI.
' -------------------------------------------------------------------------------------------------
Public Class ZoneDossierInfo

    ' Zone concernée
    Public Property Zone As ZoneDossier

    ' Libellé affiché dans l'UI (bouton du suivi clinique, titre de la modale)
    Public Property Libelle As String

    ' Nom de la colonne RTF en base (issu de la whitelist, jamais d'une saisie utilisateur)
    Public Property ColonneRtf As String

    ' Nom de la colonne texte brut en base (issu de la whitelist)
    Public Property ColonneTxt As String

    ' Zone en lecture seule (True pour l'historique d'archive, D-DOS-4)
    Public Property LectureSeule As Boolean

End Class

' -------------------------------------------------------------------------------------------------
' Classe      : ContenuZoneDossier
' Version     : V1.1.0
' Date        : 08/07/2026
'
' Rôle        :
' Transporte le contenu d'une zone clinique en format riche et texte brut entre les couches.
'
' Important   :
' - DTO sans accès à la base ni logique métier.
' -------------------------------------------------------------------------------------------------
Public Class ContenuZoneDossier

    ' Contenu RTF de la zone (peut être Nothing/vide)
    Public Property Rtf As String

    ' Texte brut de la zone (peut être Nothing/vide) — sert à l'aperçu rapide et à la recherche
    Public Property Txt As String

End Class

' -------------------------------------------------------------------------------------------------
' Module      : ZonesDossier
' Version     : V1.1.0
' Date        : 08/07/2026
'
' Rôle        :
' Registre central (whitelist) des métadonnées des zones cliniques.
'
' Important   :
' - Seul ce registre autorise les noms de colonnes injectés dans les requêtes ciblées.
' -------------------------------------------------------------------------------------------------
Public Module ZonesDossier

#Region "Registre et accès aux zones"

    ' Registre immuable : la seule source autorisée des noms de colonnes des zones cliniques.
    Private ReadOnly _registre As New Dictionary(Of ZoneDossier, ZoneDossierInfo) From {
        {ZoneDossier.MotifSuivi, New ZoneDossierInfo With {
            .Zone = ZoneDossier.MotifSuivi,
            .Libelle = "Motif du suivi",
            .ColonneRtf = "motif_suivi_rtf",
            .ColonneTxt = "motif_suivi_txt",
            .LectureSeule = False}},
        {ZoneDossier.Historique, New ZoneDossierInfo With {
            .Zone = ZoneDossier.Historique,
            .Libelle = "Historique",
            .ColonneRtf = "historique_rtf",
            .ColonneTxt = "historique_txt",
            .LectureSeule = False}},
        {ZoneDossier.Antecedents, New ZoneDossierInfo With {
            .Zone = ZoneDossier.Antecedents,
            .Libelle = "Antécédents",
            .ColonneRtf = "antecedents_rtf",
            .ColonneTxt = "antecedents_txt",
            .LectureSeule = False}},
        {ZoneDossier.Diagnostic, New ZoneDossierInfo With {
            .Zone = ZoneDossier.Diagnostic,
            .Libelle = "Diagnostic",
            .ColonneRtf = "diagnostic_rtf",
            .ColonneTxt = "diagnostic_txt",
            .LectureSeule = False}},
        {ZoneDossier.TraitementsEnCours, New ZoneDossierInfo With {
            .Zone = ZoneDossier.TraitementsEnCours,
            .Libelle = "Traitements en cours",
            .ColonneRtf = "traitements_en_cours_rtf",
            .ColonneTxt = "traitements_en_cours_txt",
            .LectureSeule = False}},
        {ZoneDossier.ObjectifsSuivi, New ZoneDossierInfo With {
            .Zone = ZoneDossier.ObjectifsSuivi,
            .Libelle = "Objectifs du suivi",
            .ColonneRtf = "objectifs_suivi_rtf",
            .ColonneTxt = "objectifs_suivi_txt",
            .LectureSeule = False}},
        {ZoneDossier.HistoriqueArchive, New ZoneDossierInfo With {
            .Zone = ZoneDossier.HistoriqueArchive,
            .Libelle = "Historique d'archive",
            .ColonneRtf = "historique_archive_rtf",
            .ColonneTxt = "historique_archive_txt",
            .LectureSeule = True}}
    }

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetInfo
    ' Version    : V1.1.0
    ' Date       : 08/07/2026
    '
    ' Rôle       :
    ' Retourne les métadonnées de la whitelist pour une zone clinique donnée.
    '
    ' Paramètres :
    ' - zone : Zone clinique demandée.
    '
    ' Retour     :
    ' - ZoneDossierInfo : Métadonnées de la zone, jamais Nothing pour une valeur répertoriée.
    '
    ' Remarques  :
    ' - Lève ArgumentOutOfRangeException si la zone n’est pas répertoriée (garde-fou).
    ' -------------------------------------------------------------------------------------------------
    Public Function GetInfo(zone As ZoneDossier) As ZoneDossierInfo

        Dim info As ZoneDossierInfo = Nothing

        If Not _registre.TryGetValue(zone, info) Then
            Throw New ArgumentOutOfRangeException(NameOf(zone), $"Zone de dossier non répertoriée : {zone}.")
        End If

        Return info

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetZonesEditables
    ' Version    : V1.1.0
    ' Date       : 08/07/2026
    '
    ' Rôle       :
    ' Retourne, dans l’ordre d’affichage, les zones éditables hors historique d’archive.
    '
    ' Retour     :
    ' - List(Of ZoneDossierInfo) : Liste ordonnée des zones modifiables.
    ' -------------------------------------------------------------------------------------------------
    Public Function GetZonesEditables() As List(Of ZoneDossierInfo)

        Return New List(Of ZoneDossierInfo) From {
            GetInfo(ZoneDossier.MotifSuivi),
            GetInfo(ZoneDossier.Historique),
            GetInfo(ZoneDossier.Antecedents),
            GetInfo(ZoneDossier.Diagnostic),
            GetInfo(ZoneDossier.TraitementsEnCours),
            GetInfo(ZoneDossier.ObjectifsSuivi)
        }

    End Function

#End Region

End Module
