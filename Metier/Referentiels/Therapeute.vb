' -------------------------------------------------------------------------------------------------
' Classe      : Therapeute
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 16/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Représente un thérapeute (intervenant professionnel) issu de la table therapeutes.
' Entité riche partagée comme référentiel : référencée par dossiers.id_therapeute_traitant
' et par autres_suivis_patient.id_therapeute.
'
' Responsabilités :
' - Contenir l'identité, la spécialité, les coordonnées et l'adresse d'un thérapeute
' - Porter le commentaire libre (texte) et l'indicateur d'activité (soft-delete)
'
' Table source : therapeutes
'
' Important   :
' - Classe de données (DTO-like) sans logique métier complexe
' - Ne contient aucun accès base de données
' - Ne contient aucune logique UI
' - code_therapeute est généré par la base (colonne STORED) : lecture seule, jamais inséré/modifié
' - La colonne actif est ajoutée manuellement à la table (pas de migration) — D-Therapeutes
'
' Dépendances :
' - Utilisée par GestionTherapeutes (mapping depuis DataReader)
' - Utilisée par QueryTherapeutes (requêtes SQL)
' - Utilisée par l'écran UC_Therapeutes et le formulaire TherapeuteEdition
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class Therapeute

#Region "Propriétés"

    ' Identifiant unique du thérapeute (clé primaire, généré par la base via seq_therapeutes)
    Public Property IdTherapeute As Long

    ' Code technique du thérapeute (généré automatiquement, ex. 'TH000001', non modifiable)
    Public Property CodeTherapeute As String

    ' Nom de famille du thérapeute (obligatoire)
    Public Property Nom As String

    ' Prénom du thérapeute (optionnel)
    Public Property Prenom As String

    ' Spécialité / profession du thérapeute (optionnelle)
    ' TODO (idée à creuser, décision à prendre) : mettre la spécialité en table (référentiel ref_*)
    ' pour unifier la saisie (éviter les variantes orthographiques). Si OK : créer la table côté base,
    ' remplacer ce champ texte par un id_specialite (FK), et adapter TherapeuteEdition (combo + [+]).
    ' Voir Docs\Todo\ToDo.md § Thérapeutes.
    Public Property Specialite As String

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

    '  ' Commentaire libre (texte brut)
    Public Property Commentaire_txt As String

    '  ' Commentaire libre (texte enrichi RTF)
    Public Property Commentaire_rtf As String

    ' Indicateur d'activité : True = actif, False = désactivé (soft-delete)
    Public Property Actif As Boolean

    ' Date de création du thérapeute
    Public Property DateCreation As DateTime

    ' Date de dernière modification du thérapeute
    Public Property DateModification As DateTime

#End Region

#Region "Helpers"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : NomComplet
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Rôle       :
    ' Retourne le nom complet du thérapeute au format "Prénom NOM" pour l'affichage.
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
    ' Propriété  : LibelleAffichage
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Rôle       :
    ' Retourne un libellé lisible "Prénom NOM (spécialité)" pour les listes déroulantes.
    '
    ' Retour     :
    ' - String : Nom complet suivi de la spécialité entre parenthèses si renseignée
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property LibelleAffichage As String
        Get
            If String.IsNullOrWhiteSpace(Specialite) Then
                Return NomComplet
            End If

            Return $"{NomComplet} ({Specialite.Trim()})"
        End Get
    End Property

#End Region

End Class
