' -------------------------------------------------------------------------------------------------
' Classe   : LiaisonPrixDomaine
' Projet   : Althéa
' Version  : V1.1.0
' Date     : 18/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente le rattachement d'un tarif de séance à un domaine sur une période de validité,
' issu de la table de liaison lia_prix_seance_domaines (D-POR-4).
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - La liaison est historisée : plusieurs rattachements successifs peuvent exister pour un même
'   couple tarif / domaine, chacun portant sa propre période.
' - DateFinValidite à Nothing signifie « toujours en vigueur ».
' - V1.1.0 (18/10/2026, UI-9.4) : état en famille (Etat, EtatLiaisonPrix) pour la pastille de la
'   grille ; LibelleEtat en découle, ses textes sont inchangés.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

''' <summary>État d'un rattachement tarif / domaine à la date du jour (UI-9.4, 18/10/2026).</summary>
Public Enum EtatLiaisonPrix
    EnVigueur
    AVenir
    Cloture
    Desactive
End Enum

Public Class LiaisonPrixDomaine

#Region "Propriétés"

    ' Identifiant du rattachement (clé primaire, généré par la base ; 0 avant enregistrement)
    Public Property IdLiaison As ULong

    ' Identifiant du tarif de séance rattaché
    Public Property IdPrixSeance As ULong

    ' Identifiant du domaine rattaché
    Public Property IdDomaine As ULong

    ' Code du domaine (issu de la jointure, lecture seule à l'écran)
    Public Property CodeDomaine As String

    ' Libellé du domaine (issu de la jointure, lecture seule à l'écran)
    Public Property LibelleDomaine As String

    ' Code du tarif (issu de la jointure, lecture seule à l'écran)
    Public Property CodePrixSeance As String

    ' Libellé du tarif (issu de la jointure, lecture seule à l'écran)
    Public Property LibellePrixSeance As String

    ' Montant du tarif rattaché (issu de la jointure, lecture seule à l'écran)
    Public Property Montant As Decimal

    ' Premier jour d'application du tarif pour ce domaine
    Public Property DateDebutValidite As Date

    ' Dernier jour d'application ; Nothing tant que le tarif est en vigueur
    Public Property DateFinValidite As Date?

    ' Rattachement actif (désactivation sans perte d'historique)
    Public Property Actif As Boolean = True

#End Region

#Region "Propriétés calculées"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : EstEnVigueur
    ' Rôle      : Indique si le rattachement s'applique aujourd'hui.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstEnVigueur As Boolean
        Get
            Return Actif AndAlso EstApplicableLe(Date.Today)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : EstApplicableLe
    ' Rôle     : Indique si le rattachement couvre la date de référence fournie.
    ' Paramètre: dateReference - date de la séance ou date du jour
    ' -------------------------------------------------------------------------------------------------
    Public Function EstApplicableLe(dateReference As Date) As Boolean

        If dateReference < DateDebutValidite Then Return False

        If DateFinValidite.HasValue AndAlso dateReference > DateFinValidite.Value Then Return False

        Return True

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : LibellePeriodeValidite
    ' Rôle      : Période affichable dans les grilles, en clair pour l'utilisateur.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property LibellePeriodeValidite As String
        Get
            If DateFinValidite.HasValue Then
                Return $"Du {DateDebutValidite:dd/MM/yyyy} au {DateFinValidite.Value:dd/MM/yyyy}"
            End If

            Return $"Depuis le {DateDebutValidite:dd/MM/yyyy}"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : MontantAffichage
    ' Rôle      : Montant formaté pour l'affichage en grille.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property MontantAffichage As String
        Get
            Return Montant.ToString("C2")
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : LibelleEtat
    ' Rôle      : État lisible du rattachement (en vigueur, à venir, clôturé, désactivé).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property LibelleEtat As String
        Get
            Select Case Etat
                Case EtatLiaisonPrix.Desactive : Return "Désactivé"
                Case EtatLiaisonPrix.AVenir : Return "À venir"
                Case EtatLiaisonPrix.Cloture : Return "Clôturé"
                Case Else : Return "En vigueur"
            End Select
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : Etat
    ' Rôle      : Famille de l'état du rattachement à la date du jour (V1.1.0).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property Etat As EtatLiaisonPrix
        Get
            If Not Actif Then Return EtatLiaisonPrix.Desactive
            If Date.Today < DateDebutValidite Then Return EtatLiaisonPrix.AVenir
            If DateFinValidite.HasValue AndAlso Date.Today > DateFinValidite.Value Then Return EtatLiaisonPrix.Cloture
            Return EtatLiaisonPrix.EnVigueur
        End Get
    End Property

#End Region

End Class
