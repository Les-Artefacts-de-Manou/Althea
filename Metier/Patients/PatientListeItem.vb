' -------------------------------------------------------------------------------------------------
' Classe      : PatientListeItem
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 11/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Modèle de présentation léger pour la liste/recherche de patients (UC_PatientHome).
' Contient les colonnes affichées dans la grille ainsi que les compteurs de dossiers.
'
' Responsabilités :
' - Porter les données minimales d'un patient pour l'affichage en liste
' - Indiquer le nombre total de dossiers et de dossiers actifs
' - Fournir des helpers de présentation (NomComplet, AAlerte, APhoto)
'
' Remarques   :
' - Cette classe ne contient aucun accès base de données.
' - Elle est produite par GestionPatients à partir de la requête SelectPatientsListe.
' - Volontairement distincte de Patient (vue allégée, enrichie des compteurs de dossiers).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class PatientListeItem

#Region "Propriétés"

    ' Identifiant unique du patient (clé primaire)
    Public Property IdPatient As Long

    ' Code technique du patient (ex. 'PA000001')
    Public Property CodePatient As String

    ' Nom de famille du patient
    Public Property Nom As String

    ' Prénom du patient
    Public Property Prenom As String

    ' Date de naissance (optionnelle)
    Public Property DateNaissance As Date?

    ' Numéro d'identification (NISS)
    Public Property Niss As String

    ' Téléphone de contact
    Public Property Telephone As String

    ' Adresse e-mail de contact
    Public Property Email As String

    ' Alerte patient en texte brut (pour affichage rapide de l'indicateur)
    Public Property AlerteTxt As String

    ' Nom seul du fichier photo d'identité (ou Nothing si aucune photo)
    Public Property PhotoFichier As String

    ' Nombre total de dossiers du patient
    Public Property NbDossiers As Integer

    ' Nombre de dossiers actifs (ni clôturés ni archivés)
    Public Property NbDossiersActifs As Integer

    ' Indique si le patient a un suivi en cours (True) ou clôturé/archivé (False)
    Public Property SuiviEnCours As Boolean

    ' Date de dernière modification de la fiche patient
    Public Property DateModification As DateTime

#End Region

#Region "Helpers"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : NomComplet
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    ' Rôle       : Fournit le nom complet du patient au format "Prénom NOM" pour l'affichage en liste
    ' Retour     :
    ' - String : Concaténation lisible du prénom et du nom (sans espaces superflus)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property NomComplet As String
        Get
            Return $"{Prenom} {Nom}".Trim()
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : AAlerte
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    ' Rôle       : Indique si le patient porte une alerte (basé sur le texte brut)
    ' Retour     :
    ' - Boolean : True si AlerteTxt n'est pas vide ou null, sinon False
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property AAlerte As Boolean
        Get
            Return Not String.IsNullOrWhiteSpace(AlerteTxt)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : APhoto
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    ' Rôle       : Indique si le patient possède une photo d'identité
    ' Retour     :
    ' - Boolean : True si PhotoFichier n'est pas vide ou null, sinon False
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property APhoto As Boolean
        Get
            Return Not String.IsNullOrWhiteSpace(PhotoFichier)
        End Get
    End Property

#End Region

End Class
