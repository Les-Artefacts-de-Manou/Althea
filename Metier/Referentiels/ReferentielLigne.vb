' -------------------------------------------------------------------------------------------------
' Classe   : ReferentielLigne
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 09/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Modèle de présentation générique pour une ligne de référentiel (tables ref_*).
' Permet d'alimenter de façon uniforme la grille de UC_ReferentielBase, quel que soit
' le référentiel concerné (domaines, types de séance, statuts, etc.).
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Elle est produite par les UserControls dérivés (ex. UC_Domaines) à partir de leur
'   propre modèle métier (ex. Domaine) pour être affichée par la base commune.
' - IdReferentiel permet de retrouver l'élément métier d'origine lors des actions (modifier, activer).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class ReferentielLigne

#Region "Propriétés"

    ' Identifiant technique de l'élément de référentiel (clé primaire en base)
    Public Property IdReferentiel As ULong

    ' Code court de l'élément (unique au sein du référentiel)
    Public Property Code As String

    ' Libellé affiché de l'élément (unique au sein du référentiel)
    Public Property Libelle As String

    ' Ordre d'affichage de l'élément dans les listes
    Public Property OrdreAffichage As Integer

    ' Indique si l'élément est actif (False = désactivé / soft-delete)
    Public Property Actif As Boolean

    ' Tarif par défaut (optionnel) : utilisé uniquement par les référentiels possédant
    ' un montant associé (ex. ref_types_seance.tarif_defaut). Nothing pour les autres.
    Public Property Tarif As Decimal?

#End Region

End Class
