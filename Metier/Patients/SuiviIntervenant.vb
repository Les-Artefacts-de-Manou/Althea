' -------------------------------------------------------------------------------------------------
' Classe      : SuiviIntervenant
' Projet      : Althéa
' Version     : V1.2.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Représente un intervenant du réseau de suivi d'un patient (médecin traitant, psychiatre,
' logopède, adresseur/orienteur, autre thérapie…) issu de la table de liaison N-N
' autres_suivis_patient (D-Q1bis, Option A actée).
'
' Responsabilités :
' - Porter le rôle de l'intervenant (FK ref_roles_intervenant) — le rôle est porté par la liaison,
'   un même thérapeute pouvant être « adresseur » pour l'un et « médecin traitant » pour l'autre
' - Porter l'identité texte libre de l'intervenant (nom professionnel, spécialité, lieu)
' - Porter la période de suivi (date_debut, date_fin) et le commentaire enrichi (RTF + texte, D-Q7bis)
' - Dire si le suivi est terminé (EstTerminee, V1.2.0, 23/10/2026, audit C.2) : règle unique pour la
'   liste de l'onglet Intervenants (lignes grisées)
'
' Table source : autres_suivis_patient
'
' Important   :
' - Classe de données (DTO-like) sans logique métier complexe
' - Ne contient aucun accès base de données
' - Ne contient aucune logique UI
' - id_therapeute référence le thérapeute choisi ; nom professionnel, spécialité et lieu en sont une
'   copie figée à l'enregistrement (trace)
'
' Dépendances :
' - Utilisée par GestionSuivisIntervenants (mapping depuis DataReader)
' - Utilisée par QuerySuivisIntervenants (requêtes SQL)
' - Utilisée par l'onglet Intervenants de UC_PatientFiche et le formulaire IntervenantEdition
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class SuiviIntervenant

#Region "Propriétés"

    ' Identifiant unique du suivi (clé primaire, généré par la base via seq_autres_suivis_patient)
    Public Property IdAutreSuiviPatient As Long

    ' Code technique du suivi (généré automatiquement, ex. 'AS000001', non modifiable)
    Public Property CodeAutreSuiviPatient As String

    ' Identifiant du patient auquel l'intervenant est rattaché (FK patients)
    Public Property IdPatient As Long

    ' Rôle de l'intervenant dans le suivi (FK ref_roles_intervenant, optionnel)
    Public Property IdRoleIntervenant As ULong?

    ' Libellé du rôle (non stocké : alimenté par jointure pour l'affichage en liste)
    Public Property LibelleRoleIntervenant As String

    ' Identifiant du thérapeute connu au référentiel (FK therapeutes, optionnel)
    ' Remarque : laissé NULL en V1 (saisie texte libre) — à brancher avec la gestion des thérapeutes
    Public Property IdTherapeute As ULong?

    ' Civilité reprise du thérapeute choisi (trace, FK ref_civilites, optionnelle)
    Public Property IdCivilite As ULong?

    ' Libellé de la civilité (non stocké : lu par sous-requête pour l'affichage)
    Public Property LibelleCivilite As String

    ' Nom de l'intervenant en texte libre (ex. « Dr Dupont »)
    Public Property NomProfessionnel As String

    ' Spécialité de l'intervenant (ex. « Neuropédiatrie »)
    Public Property Specialite As String

    ' Lieu d'exercice de l'intervenant (ex. « CHU de Liège »)
    Public Property Lieu As String

    ' Date de début du suivi (optionnelle)
    Public Property DateDebut As Date?

    ' Date de fin du suivi (optionnelle ; NULL = suivi toujours en cours)
    Public Property DateFin As Date?

    ' Commentaire au format RTF (formatage conservé) (D-Q7bis)
    Public Property CommentaireRtf As String

    ' Commentaire en texte brut (pour la recherche et l'affichage simple) (D-Q7bis)
    Public Property CommentaireTxt As String

    ' Date de création du suivi
    Public Property DateCreation As DateTime

    ' Date de dernière modification du suivi
    Public Property DateModification As DateTime

#End Region

#Region "Helpers"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : LibelleAffichage
    ' Version    : V1.0.0
    ' Date       : 15/06/2026
    '
    ' Rôle       :
    ' Retourne une désignation lisible de l'intervenant pour l'affichage (nom professionnel,
    ' complété de la spécialité si elle est renseignée).
    '
    ' Retour     :
    ' - String : Désignation lisible (ex. « Dr Dupont (Neuropédiatrie) »)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property LibelleAffichage As String
        Get

            Dim nom As String = If(NomProfessionnel, String.Empty).Trim()

            If String.IsNullOrWhiteSpace(Specialite) Then
                Return nom
            End If

            Return $"{nom} ({Specialite.Trim()})".Trim()

        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstTerminee
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Vrai si le suivi a une date de fin déjà passée (avant aujourd'hui) ; un suivi qui finit
    ' aujourd'hui, ou sans date de fin, est en cours.
    '
    ' Remarques  :
    ' - Audit C.2 : la liste de l'onglet Intervenants relisait la cellule Date de fin pour griser la
    '   ligne ; la règle vit désormais ici.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstTerminee As Boolean
        Get
            Return DateFin.HasValue AndAlso DateFin.Value.Date < Date.Today
        End Get
    End Property

#End Region

End Class
