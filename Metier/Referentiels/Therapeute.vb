' -------------------------------------------------------------------------------------------------
' Classe      : Therapeute
' Projet      : Althéa
' Version     : V1.5.0
' Date        : 24/10/2026
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
' - Libellé de l'état pour la colonne « État » de la liste (LibelleEtat, V1.2.0, 18/10/2026, UI-9.4)
' - Adresse complète sur une ligne pour l'affichage (AdresseUneLigne, V1.3.0, 23/10/2026)
' - Spécialité par son identifiant (IdSpecialite → ref_specialites, V1.4.0, 23/10/2026, v2.55.0)
' - Code dans le libellé des listes, après le nom (LibelleAffichage, V1.5.0, 24/10/2026)
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

    ' Spécialité (optionnelle) → ref_specialites (v2.55.0 ; une seule par thérapeute, décision de Joëlle)
    Public Property IdSpecialite As ULong?

    ' Libellé de la spécialité, lu dans le référentiel (affichage, recherche, copie figée de l'intervenant) ;
    ' jamais écrit : seul IdSpecialite l'est
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

    ' Pays de l'adresse (FK ref_pays, optionnelle)
    Public Property IdPays As ULong?

    ' Code ISO du pays (non stocké : lu par sous-requête) ; clé des règles de validation
    Public Property CodePays As String

    ' Civilité (FK ref_civilites, optionnelle)
    Public Property IdCivilite As ULong?

    ' Libellé de la civilité (non stocké : lu par sous-requête, repris dans la trace d'un intervenant)
    Public Property LibelleCivilite As String

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

    ' État affiché dans la liste (colonne triable) : « Inactif », ou rien pour un thérapeute actif —
    ' seules les exceptions se voient (V1.2.0).
    Public ReadOnly Property LibelleEtat As String
        Get
            Return If(Actif, String.Empty, "Inactif")
        End Get
    End Property

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
    ' Version    : V1.1.0
    ' Date       : 24/10/2026
    '
    ' Rôle       :
    ' Retourne un libellé lisible "Prénom NOM (spécialité) · TH000003" pour les listes déroulantes.
    '
    ' Remarques  :
    ' - V1.1.0 (24/10/2026) : le code suit le nom (code toujours visible, Rules § 15) : on choisit
    '   un thérapeute par son nom, la frappe d'une lettre doit encore sauter au bon nom (décision de
    '   Joëlle, 24/10/2026). Sans code (entrée « (sélectionner…) »), le nom seul.
    '
    ' Retour     :
    ' - String : Nom complet, spécialité entre parenthèses si renseignée, puis le code
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property LibelleAffichage As String
        Get
            Dim libelle As String = If(String.IsNullOrWhiteSpace(Specialite),
                                       NomComplet,
                                       $"{NomComplet} ({Specialite.Trim()})")

            Return If(String.IsNullOrWhiteSpace(CodeTherapeute), libelle, $"{libelle} · {CodeTherapeute}")
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : AdresseUneLigne
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Retourne l'adresse sur une ligne, parties renseignées seulement, séparées par une virgule
    ' (ex. « Rue des Lilas 4, Bâtiment B, 1000 Bruxelles »).
    '
    ' Remarques  :
    ' - Affichée en lecture seule dans la fiche Intervenant (adresse actuelle du thérapeute choisi).
    ' - Le pays n'y figure pas : l'adresse sert à situer le cabinet, pas à libeller un courrier.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property AdresseUneLigne As String
        Get
            Dim ville As String = $"{If(CodePostal, String.Empty).Trim()} {If(Localite, String.Empty).Trim()}".Trim()

            Dim parties As IEnumerable(Of String) =
                {AdresseLigne1, AdresseLigne2, ville}.
                Where(Function(partie) Not String.IsNullOrWhiteSpace(partie)).
                Select(Function(partie) partie.Trim())

            Return String.Join(", ", parties)
        End Get
    End Property

#End Region

End Class
