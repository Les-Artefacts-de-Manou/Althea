' -------------------------------------------------------------------------------------------------
' Classe      : DossierListeItem
' Projet      : Althéa
' Version     : V1.0.2
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Modèle de présentation léger pour la liste des dossiers d'un patient (onglet Dossiers
' de UC_PatientFiche).
'
' Responsabilités :
' - Porter les données minimales d'un dossier pour l'affichage en grille
' - Exposer les libellés joints (domaine, statut) et le référent pour un affichage direct
' - Fournir un aperçu du motif de suivi (texte brut)
'
' Remarques   :
' - Cette classe ne contient aucun accès base de données.
' - Elle est produite par GestionDossiers à partir de la requête SelectDossiersParPatient.
' - Volontairement distincte de Dossier (vue allégée, sans les zones cliniques RTF).
' - V1.0.1 (17/10/2026) : codes de statut repris de GestionDossiers.CODE_STATUT_*.
' - V1.0.2 (24/10/2026, audit C.3) : codes de statut lus dans CodesSysteme.StatutsDossier.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class DossierListeItem

#Region "Propriétés"

    ' Identifiant unique du dossier (clé primaire)
    Public Property IdDossier As Long

    ' Code technique du dossier (ex. 'DO000012')
    Public Property CodeDossier As String

    ' Identifiant du patient auquel le dossier est rattaché
    Public Property IdPatient As Long

    ' Identifiant du domaine métier du dossier
    Public Property IdDomaine As Long

    ' Libellé du domaine (issu de la jointure ref_domaines)
    Public Property LibelleDomaine As String

    ' Identifiant du statut courant du dossier
    Public Property IdStatutDossier As Long

    ' Code du statut courant (ex. 'ACTIF', 'CLOTURE', 'ARCHIVE') — utile pour l'icône/couleur
    Public Property CodeStatutDossier As String

    ' Libellé du statut courant (issu de la jointure ref_statuts_dossier)
    Public Property LibelleStatutDossier As String

    ' Date d'ouverture du dossier
    Public Property DateOuverture As Date

    ' Date de clôture du dossier (NULL si le dossier n'est pas clôturé)
    Public Property DateCloture As Date?

    ' Nom affiché du thérapeute référent (« Prénom Nom », issu de la jointure therapeutes)
    Public Property TherapeuteTraitant As String

    ' Aperçu du motif de suivi (texte brut)
    Public Property MotifSuiviTxt As String

    ' Date de dernière modification du dossier
    Public Property DateModification As DateTime

#End Region

#Region "Helpers"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstArchive
    ' Version    : V1.0.1
    ' Date       : 17/10/2026
    ' Rôle       : Indique si le dossier est au statut « Archivé » (lecture seule, terminal)
    ' Retour     :
    ' - Boolean : True si le code statut vaut 'ARCHIVE', False sinon
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstArchive As Boolean
        Get
            Return String.Equals(CodeStatutDossier, CodesSysteme.StatutsDossier.Archive, StringComparison.OrdinalIgnoreCase)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstCloture
    ' Version    : V1.0.1
    ' Date       : 17/10/2026
    ' Rôle       : Indique si le dossier est au statut « Clôturé »
    ' Retour     :
    ' - Boolean : True si le code statut vaut 'CLOTURE', False sinon
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstCloture As Boolean
        Get
            Return String.Equals(CodeStatutDossier, CodesSysteme.StatutsDossier.Cloture, StringComparison.OrdinalIgnoreCase)
        End Get
    End Property

#End Region

End Class
