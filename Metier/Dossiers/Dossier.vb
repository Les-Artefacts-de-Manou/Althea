' -------------------------------------------------------------------------------------------------
' Classe      : Dossier
' Projet      : Althéa
' Version     : V1.2.1
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Représente un dossier de suivi clinique rattaché à un patient et à un domaine métier.
'
' Responsabilités :
' - Contenir les données d'identité du dossier (patient, domaine, statut, dates, référent, modalités)
' - Porter les 7 zones cliniques enrichies (RTF + texte) : motif, historique, antécédents,
'   diagnostic, traitements en cours, objectifs, historique d'archive
' - Exposer les libellés joints (domaine, statut, thérapeute traitant) pour l'affichage
' - Fournir des helpers de présentation simples (code statut = en pause / clôturé / archivé)
'
' Table source : dossiers
'
' Important   :
' - Un patient peut posséder plusieurs dossiers (un par domaine de suivi) — cf. D-Q5
' - Chaque zone clinique est stockée en double format xxx_rtf / xxx_txt (ADR-15)
' - historique_archive n'est renseigné qu'au passage en Archivé (lecture seule) — cf. D-DOS-4
'
' Architecture :
' - Classe de données (DTO-like) sans logique métier complexe
' - Ne contient aucun accès base de données
' - Ne contient aucune logique UI
'
' Dépendances :
' - Utilisée par GestionDossiers (mapping depuis DataReader)
' - Utilisée par QueryDossiers (requêtes SQL)
' - Utilisée par les UserControls dossiers (UC_DossierFiche, DossierZoneEditor)
' - Codes de statut : CodesSysteme.StatutsDossier (une seule définition)
'
' Historique  :
' - V1.1.0 (17/10/2026) : EstEnPause (couleur de la pastille de statut) ; codes de statut repris
'   de GestionDossiers au lieu d'être répétés.
' - V1.2.0 (20/10/2026) : LogoDomaine (logo du domaine dans le bandeau du dossier).
' - V1.2.1 (24/10/2026, audit C.3) : codes de statut lus dans CodesSysteme.StatutsDossier.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class Dossier

#Region "Propriétés — Identité"

    ' Identifiant unique du dossier (clé primaire, généré par la base via seq_dossiers)
    Public Property IdDossier As Long

    ' Code technique du dossier (généré automatiquement, ex. 'DO000012', non modifiable)
    Public Property CodeDossier As String

    ' Identifiant du patient auquel le dossier est rattaché (FK patients, obligatoire)
    Public Property IdPatient As Long

    ' Identifiant du domaine métier du dossier (FK ref_domaines, obligatoire)
    Public Property IdDomaine As Long

    ' Libellé du domaine (non stocké : alimenté par jointure pour l'affichage)
    Public Property LibelleDomaine As String

    ' Fichier logo du domaine (non stocké : jointure ; vide si aucun) — bandeau du dossier
    Public Property LogoDomaine As String = String.Empty

    ' Identifiant du statut courant du dossier (FK ref_statuts_dossier, obligatoire)
    Public Property IdStatutDossier As Long

    ' Code du statut courant (non stocké : alimenté par jointure ; 'ACTIF', 'PAUSE', 'CLOTURE', 'ARCHIVE')
    Public Property CodeStatutDossier As String

    ' Libellé du statut courant (non stocké : alimenté par jointure pour l'affichage)
    Public Property LibelleStatutDossier As String

    ' Date d'ouverture du dossier (obligatoire)
    Public Property DateOuverture As Date

    ' Date de clôture du dossier (obligatoire au passage en Clôturé, NULL sinon)
    Public Property DateCloture As Date?

    ' Date d'archivage du dossier (renseignée au passage en Archivé, NULL sinon)
    Public Property DateArchivage As Date?

    ' Modalités de paiement du suivi (optionnel)
    Public Property ModalitesPaiement As String

    ' Identifiant du thérapeute référent du dossier (FK therapeutes, optionnel)
    Public Property IdTherapeuteTraitant As Long?

    ' Nom affiché du thérapeute référent (non stocké : alimenté par jointure « Prénom Nom »)
    Public Property TherapeuteTraitant As String

    ' Date de création du dossier (gérée par la base)
    Public Property DateCreation As DateTime

    ' Date de dernière modification du dossier (gérée par la base)
    Public Property DateModification As DateTime

#End Region

#Region "Propriétés — Zones cliniques (RTF + texte)"

    ' Zone 1 : Motif du suivi (RTF)
    Public Property MotifSuiviRtf As String

    ' Zone 1 : Motif du suivi (texte brut, pour recherche/affichage simple)
    Public Property MotifSuiviTxt As String

    ' Zone 2 : Historique (RTF)
    Public Property HistoriqueRtf As String

    ' Zone 2 : Historique (texte brut)
    Public Property HistoriqueTxt As String

    ' Zone 3 : Antécédents (RTF)
    Public Property AntecedentsRtf As String

    ' Zone 3 : Antécédents (texte brut)
    Public Property AntecedentsTxt As String

    ' Zone 4 : Diagnostic (RTF)
    Public Property DiagnosticRtf As String

    ' Zone 4 : Diagnostic (texte brut)
    Public Property DiagnosticTxt As String

    ' Zone 5 : Traitements en cours (RTF)
    Public Property TraitementsEnCoursRtf As String

    ' Zone 5 : Traitements en cours (texte brut)
    Public Property TraitementsEnCoursTxt As String

    ' Zone 6 : Objectifs du suivi (RTF)
    Public Property ObjectifsSuiviRtf As String

    ' Zone 6 : Objectifs du suivi (texte brut)
    Public Property ObjectifsSuiviTxt As String

    ' Zone 7 : Historique d'archive (RTF) — renseigné à l'archivage, lecture seule (D-DOS-4)
    Public Property HistoriqueArchiveRtf As String

    ' Zone 7 : Historique d'archive (texte brut)
    Public Property HistoriqueArchiveTxt As String

#End Region

#Region "Helpers"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstArchive
    ' Version    : V1.0.0
    ' Date       : 21/07/2026
    '
    ' Rôle       :
    ' Indique si le dossier est au statut « Archivé » (terminal, lecture seule).
    '
    ' Retour     :
    ' - Boolean : True si le code statut vaut 'ARCHIVE', False sinon
    '
    ' Remarques  :
    ' - S'appuie sur CodeStatutDossier (issu de la jointure ref_statuts_dossier)
    ' - Conditionne la visibilité de la zone historique_archive (D-DOS-4)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstArchive As Boolean
        Get
            Return String.Equals(CodeStatutDossier, CodesSysteme.StatutsDossier.Archive, StringComparison.OrdinalIgnoreCase)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstCloture
    ' Version    : V1.0.0
    ' Date       : 21/07/2026
    '
    ' Rôle       :
    ' Indique si le dossier est au statut « Clôturé ».
    '
    ' Retour     :
    ' - Boolean : True si le code statut vaut 'CLOTURE', False sinon
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstCloture As Boolean
        Get
            Return String.Equals(CodeStatutDossier, CodesSysteme.StatutsDossier.Cloture, StringComparison.OrdinalIgnoreCase)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstEnPause
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Indique si le dossier est au statut « En pause » (suivi suspendu, toujours modifiable).
    '
    ' Retour     :
    ' - Boolean : True si le code statut vaut 'PAUSE', False sinon
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstEnPause As Boolean
        Get
            Return String.Equals(CodeStatutDossier, CodesSysteme.StatutsDossier.Pause, StringComparison.OrdinalIgnoreCase)
        End Get
    End Property

#End Region

End Class
