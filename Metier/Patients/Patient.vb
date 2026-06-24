' -------------------------------------------------------------------------------------------------
' Classe      : Patient
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 11/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Représente un patient (la personne suivie), indépendamment de ses dossiers.
'
' Responsabilités :
' - Contenir les données d'identité, de contact et administratives d'un patient
' - Porter l'alerte enrichie (RTF + texte) et la photo d'identité (nom de fichier seul)
' - Porter l'anamnèse enrichie (RTF + texte)
' - Fournir des helpers de présentation simples (NomComplet, AAlerte)
'
' Table source : patients
'
' Important   :
' - Classe de données (DTO-like) sans logique métier complexe
' - Ne contient aucun accès base de données
' - Ne contient aucune logique UI
'
' Dépendances :
' - Utilisée par GestionPatients (mapping depuis DataReader)
' - Utilisée par QueryPatients (requêtes SQL)
' - Utilisée par les UserControls patients (UC_PatientHome, UC_PatientFiche)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class Patient

#Region "Propriétés"

    ' Identifiant unique du patient (clé primaire, généré par la base via seq_patients)
    Public Property IdPatient As Long

    ' Code technique du patient (généré automatiquement, ex. 'PA000001', non modifiable)
    Public Property CodePatient As String

    ' Nom de famille du patient (obligatoire)
    Public Property Nom As String

    ' Prénom du patient (obligatoire)
    Public Property Prenom As String

    ' Date de naissance (optionnelle)
    Public Property DateNaissance As Date?

    ' Numéro d'identification de sécurité sociale (NISS, unique si renseigné)
    Public Property Niss As String

    ' Latéralité (droitier, gaucher, ambidextre, ...)
    Public Property Lateralite As String

    ' Première ligne d'adresse
    Public Property AdresseLigne1 As String

    ' Seconde ligne d'adresse (complément)
    Public Property AdresseLigne2 As String

    ' Code postal
    Public Property CodePostal As String

    ' Localité / commune
    Public Property Localite As String

    ' Pays
    Public Property Pays As String

    ' Téléphone de contact
    Public Property Telephone As String

    ' Adresse e-mail de contact
    Public Property Email As String

    ' Mutualité / assurance santé
    Public Property Mutualite As String

    ' Situation familiale (FK ref_situations_familiales, optionnelle)
    Public Property IdSituationFamiliale As Long?

    ' Nom seul du fichier photo d'identité ; le chemin est reconstruit (D-Q13)
    Public Property PhotoFichier As String

    ' Alerte patient au format RTF (formatage conservé) (D-Q12)
    Public Property AlerteRtf As String

    ' Alerte patient en texte brut (pour la recherche et l'affichage simple) (D-Q12)
    Public Property AlerteTxt As String

    ' Anamnèse patient au format RTF (formatage conservé) (D-Q19)
    Public Property AnamneseRtf As String

    ' Anamnèse patient en texte brut (pour la recherche et l'affichage simple) (D-Q19)
    Public Property AnamneseTxt As String

    ' Date de création de la fiche patient
    Public Property DateCreation As DateTime

    ' Date de dernière modification de la fiche patient
    Public Property DateModification As DateTime

#End Region

#Region "Helpers"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : NomComplet
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Retourne le nom complet du patient au format "Prénom NOM" pour l'affichage.
    '
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
    '
    ' Rôle       :
    ' Indique si le patient porte une alerte (basé sur le texte brut de l'alerte).
    '
    ' Retour     :
    ' - Boolean : True si AlerteTxt contient du texte, False sinon
    '
    ' Utilisé par :
    ' - UI (affichage de l'indicateur d'alerte dans la liste et le bandeau patient)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property AAlerte As Boolean
        Get
            Return Not String.IsNullOrWhiteSpace(AlerteTxt)
        End Get
    End Property

#End Region

End Class
