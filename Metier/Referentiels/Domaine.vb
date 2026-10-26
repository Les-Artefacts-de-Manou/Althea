' -------------------------------------------------------------------------------------------------
' Classe   : Domaine
' Projet   : Althéa
' Version  : V1.1.1
' Date     : 25/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente un domaine de prise en charge issu de la table référentielle ref_domaines
' (ex. Psychologie, Graphothérapie, ...).
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Elle sert de modèle métier entre la couche DB et l'interface de gestion du référentiel.
' - id_domaine est généré par la base (AUTO_INCREMENT).
' - code_domaine est saisi par l'utilisateur (3 lettres majuscules) et doit être unique.
' - TarifConvention, NoShowFacturable et SoumisQuota portent les règles de portabilité
'   (D-POR-1 à D-POR-3) : aucun comportement métier ne doit être codé en dur à partir du code
'   du domaine, tout est piloté par ces indicateurs référentiels.
' - V1.1.0 (20/10/2026, v2.45.0) : LogoFichier (logo facultatif, CheminsDomaineHelper).
' - V1.1.1 (25/10/2026, audit D-1) : Option Strict On.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class Domaine

#Region "Propriétés"

    ' Identifiant unique du domaine (clé primaire, généré par la base)
    Public Property IdDomaine As ULong

    ' Code court du domaine (3 lettres majuscules, unique)
    Public Property CodeDomaine As String

    ' Libellé affiché dans l'interface utilisateur (unique)
    Public Property LibelleDomaine As String

    ' Indique si le domaine est actif (0 = désactivé, 1 = actif)
    Public Property Actif As Boolean

    ' Ordre d'affichage dans l'interface utilisateur
    Public Property OrdreAffichage As Integer

    ' Tarif résolu via lia_prix_seance_domaines (circuit conventionné) et non via
    ' le tarif patient du dossier — D-POR-1
    Public Property TarifConvention As Boolean

    ' No-show facturé au patient (False pour un circuit conventionné qui ne rembourse pas) — D-POR-2
    Public Property NoShowFacturable As Boolean = True

    ' Domaine soumis à un quota périodique paramétré dans ref_quotas — D-POR-3
    Public Property SoumisQuota As Boolean

    ' Organisme de remboursement rattaché (facultatif : Nothing = aucun organisme)
    Public Property IdOrganisme As ULong?

    ' Libellé de l'organisme rattaché (lecture seule, issu de la jointure ; vide si aucun)
    Public Property LibelleOrganisme As String = String.Empty

    ' Nom du fichier logo (« Logo.png »), vide si aucun ; fichier rangé par CheminsDomaineHelper
    Public Property LogoFichier As String = String.Empty

#End Region

#Region "Propriétés calculées"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : EstConventionne
    ' Rôle      : Indique si le domaine relève d'un circuit conventionné, c'est-à-dire si son tarif
    '             est fixé par une convention plutôt que par le tarif patient du dossier.
    ' Remarque  : Sert de raccourci d'affichage ; la règle métier reste portée par TarifConvention.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstConventionne As Boolean
        Get
            Return TarifConvention
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : LibelleComportement
    ' Rôle      : Résumé lisible des règles portées par le domaine, destiné aux grilles et info-bulles.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property LibelleComportement As String
        Get
            Dim elements As New List(Of String)()

            If TarifConvention Then elements.Add("Tarif conventionné")
            If SoumisQuota Then elements.Add("Soumis à quota")
            If Not NoShowFacturable Then elements.Add("No-show non facturable")

            If elements.Count = 0 Then Return "Domaine privé standard"

            Return String.Join(" · ", elements)
        End Get
    End Property

#End Region

End Class
