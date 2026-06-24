' -------------------------------------------------------------------------------------------------
' Classe      : FamilleContact
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 13/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Représente un contact de l'entourage d'un patient (famille, représentant légal, personne
' autorisée, contact d'urgence) issu de la table famille_contacts.
'
' Responsabilités :
' - Contenir l'identité, les coordonnées et l'adresse d'un contact lié au patient
' - Porter le lien de parenté/relation (FK ref_liens_patient) et le rôle légal (FK ref_role_legal)
' - Porter le commentaire enrichi (RTF + texte) (D-Q7bis)
'
' Table source : famille_contacts
'
' Important   :
' - Classe de données (DTO-like) sans logique métier complexe
' - Ne contient aucun accès base de données
' - Ne contient aucune logique UI
'
' Dépendances :
' - Utilisée par GestionFamilleContacts (mapping depuis DataReader)
' - Utilisée par QueryFamilleContacts (requêtes SQL)
' - Utilisée par l'onglet Famille/Contacts de UC_PatientFiche et le formulaire ContactEdition
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class FamilleContact

#Region "Propriétés"

    ' Identifiant unique du contact (clé primaire, généré par la base via seq_famille_contacts)
    Public Property IdFamilleContact As Long

    ' Code technique du contact (généré automatiquement, ex. 'FC000001', non modifiable)
    Public Property CodeFamilleContact As String

    ' Identifiant du patient auquel le contact est rattaché (FK patients)
    Public Property IdPatient As Long

    ' Lien de parenté/relation avec le patient (FK ref_liens_patient)
    Public Property IdLienPatient As ULong

    ' Libellé du lien (non stocké : alimenté par jointure pour l'affichage en liste)
    Public Property LibelleLienPatient As String

    ' Nom de famille du contact (obligatoire)
    Public Property Nom As String

    ' Prénom du contact (obligatoire)
    Public Property Prenom As String

    ' Date de naissance du contact (optionnelle)
    Public Property DateNaissance As Date?

    ' Téléphone de contact
    Public Property Telephone As String

    ' Adresse e-mail de contact
    Public Property Email As String

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

    ' Rôle légal unique du contact (FK ref_role_legal, obligatoire) — D-Q18
    Public Property IdRoleLegal As ULong

    ' Libellé du rôle légal (non stocké : alimenté par jointure pour l'affichage en liste)
    Public Property LibelleRoleLegal As String

    ' Ordre d'affichage dans la liste des contacts du patient
    Public Property OrdreAffichage As Integer

    ' Commentaire au format RTF (formatage conservé) (D-Q7bis)
    Public Property CommentaireRtf As String

    ' Commentaire en texte brut (pour la recherche et l'affichage simple) (D-Q7bis)
    Public Property CommentaireTxt As String

    ' Date de création du contact
    Public Property DateCreation As DateTime

    ' Date de dernière modification du contact
    Public Property DateModification As DateTime

#End Region

#Region "Helpers"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : NomComplet
    ' Version    : V1.0.0
    ' Date       : 13/06/2026
    '
    ' Rôle       :
    ' Retourne le nom complet du contact au format "Prénom NOM" pour l'affichage.
    '
    ' Retour     :
    ' - String : Concaténation lisible du prénom et du nom (sans espaces superflus)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property NomComplet As String
        Get
            Return $"{Prenom} {Nom}".Trim()
        End Get
    End Property

#End Region

End Class
