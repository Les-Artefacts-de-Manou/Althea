' -------------------------------------------------------------------------------------------------
' Classe   : LiaisonNatureDomaine
' Projet   : Althéa
' Version  : V1.1.0
' Date     : 18/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente le rattachement d'une nature de séance à un domaine, issu de la table de liaison
' lia_natures_seance_domaines (D-POR-5). Ce rattachement détermine les natures proposées à la
' saisie d'une séance selon le domaine du dossier.
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Contrairement au rattachement tarif / domaine, cette liaison n'est PAS historisée :
'   une nature est proposée ou non pour un domaine, sans période de validité.
' - ActifNature reflète l'état de la nature dans son propre référentiel : une nature désactivée
'   ne sera pas proposée même si le rattachement est actif.
' - V1.1.0 (18/10/2026, UI-9.4) : état en famille (Etat, EtatLiaisonNature) pour la pastille de la
'   grille ; LibelleEtat en découle, ses textes sont inchangés.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

''' <summary>État d'un rattachement nature / domaine, de la saisie au retrait (UI-9.4, 18/10/2026).</summary>
Public Enum EtatLiaisonNature
    ''' <summary>Proposée à la saisie d'une séance.</summary>
    Proposee
    ''' <summary>Rattachement actif mais nature désactivée dans son référentiel : pas proposée.</summary>
    NatureDesactivee
    ''' <summary>Rattachement retiré du domaine.</summary>
    RetireeDuDomaine
End Enum

Public Class LiaisonNatureDomaine

#Region "Propriétés"

    ' Identifiant du rattachement (clé primaire, généré par la base ; 0 avant enregistrement)
    Public Property IdLiaison As ULong

    ' Identifiant de la nature de séance rattachée
    Public Property IdNatureSeance As ULong

    ' Identifiant du domaine rattaché
    Public Property IdDomaine As ULong

    ' Code de la nature (issu de la jointure, lecture seule à l'écran)
    Public Property CodeNatureSeance As String

    ' Libellé de la nature (issu de la jointure, lecture seule à l'écran)
    Public Property LibelleNatureSeance As String

    ' Catégorie de quota consommée par la nature dans ce domaine (Nothing = hors quota)
    Public Property IdCategorieQuota As ULong?

    ' Libellé de la catégorie (issu de la jointure, vide si hors quota)
    Public Property LibelleCategorieQuota As String

    ' État de la nature dans le référentiel ref_natures_seance
    Public Property ActifNature As Boolean

    ' Code du domaine (issu de la jointure, lecture seule à l'écran)
    Public Property CodeDomaine As String

    ' Libellé du domaine (issu de la jointure, lecture seule à l'écran)
    Public Property LibelleDomaine As String

    ' Rattachement actif (retrait de la liste de saisie sans perte d'historique)
    Public Property Actif As Boolean = True

#End Region

#Region "Propriétés calculées"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : EstProposeASaisie
    ' Rôle      : Indique si la nature sera effectivement proposée lors de la saisie d'une séance.
    ' Remarques : Le rattachement ET la nature doivent être actifs.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstProposeASaisie As Boolean
        Get
            Return Actif AndAlso ActifNature
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : ConsommeQuota
    ' Rôle      : Indique si la nature consomme un quota dans ce domaine.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property ConsommeQuota As Boolean
        Get
            Return IdCategorieQuota.HasValue
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : LibellePrestation
    ' Rôle      : Libellé lisible de la catégorie de quota pour l'affichage en grille.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property LibellePrestation As String
        Get
            If Not ConsommeQuota Then Return "Hors quota"
            Return If(String.IsNullOrWhiteSpace(LibelleCategorieQuota), "(catégorie inconnue)", LibelleCategorieQuota)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : LibelleEtat
    ' Rôle      : État lisible du rattachement, en distinguant la cause d'une non-proposition.
    ' Remarques : L'utilisateur doit comprendre pourquoi une nature n'apparaît pas à la saisie.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property LibelleEtat As String
        Get
            Select Case Etat
                Case EtatLiaisonNature.RetireeDuDomaine : Return "Retirée du domaine"
                Case EtatLiaisonNature.NatureDesactivee : Return "Nature désactivée"
                Case Else : Return "Proposée"
            End Select
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : Etat
    ' Rôle      : Famille de l'état du rattachement (V1.1.0) : le retrait du domaine prime sur l'état
    '             de la nature.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property Etat As EtatLiaisonNature
        Get
            If Not Actif Then Return EtatLiaisonNature.RetireeDuDomaine
            If Not ActifNature Then Return EtatLiaisonNature.NatureDesactivee
            Return EtatLiaisonNature.Proposee
        End Get
    End Property

#End Region

End Class
