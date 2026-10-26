' -------------------------------------------------------------------------------------------------
' Classe      : Patient
' Projet      : Althéa
' Version     : V1.4.0
' Date        : 23/10/2026
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
' - V1.3.0 (20/10/2026, v2.46.0) : IdLateralite (référentiel ref_lateralites) à la place du texte
'   Lateralite.
' - V1.4.0 (23/10/2026, audit C.2) : TypeIdentifiantNiss (non enregistré, transmis par l'écran) ;
'   NISS enregistré normalisé.
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

    ' Numéro d'identification de sécurité sociale (NISS, unique si renseigné), enregistré sous sa
    ' forme normalisée (UtilsNiss.Normaliser : sans séparateurs ; migration v2.54.0)
    Public Property Niss As String

    ' Type de l'identifiant choisi dans la fiche (UtilsNiss.TYPE_BE / TYPE_FR / TYPE_LU / TYPE_AUTRE).
    ' NON ENREGISTRÉ (décision de Joëlle, 23/10/2026) : l'écran le transmet pour que le métier juge
    ' le format du NISS (GestionPatients.ValiderPatient) ; au chargement, il est déduit du numéro.
    ' Vide : déduit du numéro (UtilsNiss.DeduireType).
    Public Property TypeIdentifiantNiss As String

    ' Latéralité → ref_lateralites (v2.46.0 ; remplace le texte libre « lateralite »)
    Public Property IdLateralite As Long?

    ' Première ligne d'adresse
    Public Property AdresseLigne1 As String

    ' Seconde ligne d'adresse (complément)
    Public Property AdresseLigne2 As String

    ' Code postal
    Public Property CodePostal As String

    ' Localité / commune
    Public Property Localite As String

    ' Pays de l'adresse (FK ref_pays, optionnelle)
    Public Property IdPays As ULong?

    ' Code ISO du pays (non stocké : lu par sous-requête) ; clé des règles de validation
    Public Property CodePays As String

    ' Civilité (FK ref_civilites, optionnelle)
    Public Property IdCivilite As ULong?

    ' Téléphone de contact
    Public Property Telephone As String

    ' Adresse e-mail de contact
    Public Property Email As String

    ' Mutualité (ref_organismes de type MUTUALITE, optionnelle)
    Public Property IdOrganismeMutualite As ULong?

    ' Numéro d'affiliation auprès de la mutualité
    Public Property NumeroAffiliation As String

    ' Codes titulaire CT1 / CT2 (3 chiffres chacun)
    Public Property CodeTitulaire1 As String
    Public Property CodeTitulaire2 As String

    ' Situation familiale (FK ref_situations_familiales, optionnelle)
    Public Property IdSituationFamiliale As Long?

    ' Tarif par défaut du patient (FK prix_seance, optionnelle) pour tracer
    ' l'historique tarifaire (anciens 50, semi-anciens 55, nouveaux 60).
    ' NULL = utiliser le tarif courant en vigueur.
    Public Property IdPrixSeanceDefaut As ULong?

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

    ' Activation manuelle du patient (indépendante des dossiers) : True = actif, False = inactif
    Public Property IsActif As Boolean

    ' Date de la dernière activation du patient (optionnelle)
    Public Property DateActivation As Date?

    ' Date de la dernière désactivation du patient (optionnelle)
    Public Property DateDesactivation As Date?

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

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CalculerAge
    ' Version    : V1.0.0
    ' Date       : 11/10/2026
    '
    ' Rôle       :
    ' Calcule l'âge en années révolues à une date de référence donnée.
    '
    ' Paramètres :
    ' - dateNaissance : Date de naissance
    ' - reference     : Date de référence (généralement aujourd'hui)
    '
    ' Retour     :
    ' - Integer : Âge en années révolues (peut être négatif si date future)
    '
    ' Remarques  :
    ' - Partagée : utilisée par la fiche Patient (validation) et le récapitulatif de la fiche
    '   Dossier ; auparavant fonction privée de UC_PatientFiche.
    ' -------------------------------------------------------------------------------------------------
    Public Shared Function CalculerAge(dateNaissance As Date, reference As Date) As Integer

        Dim age As Integer = reference.Year - dateNaissance.Year

        If dateNaissance.Date > reference.AddYears(-age) Then
            age -= 1
        End If

        Return age

    End Function

#End Region

End Class
