' -------------------------------------------------------------------------------------------------
' Classe   : ReferentielLigne
' Projet   : Althéa
' Version  : V1.1.0
' Date     : 18/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Modèle de présentation générique pour une ligne de référentiel (tables ref_*).
' Permet d'alimenter de façon uniforme la grille de UC_ReferentielBase, quel que soit
' le référentiel concerné (domaines, types de séance, statuts, etc.).
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Elle est produite par les UserControls dérivés (ex. UC_Pays) à partir de leur
'   propre modèle métier (ex. Domaine) pour être affichée par la base commune.
' - IdReferentiel permet de retrouver l'élément métier d'origine lors des actions (modifier, activer).
' - V1.1.0 (18/10/2026, UI-9.4) : état de la ligne (Etat, LibelleEtat) pour la colonne « État » de la
'   liste — une ligne normale ne porte rien, seules les exceptions (système, inactive) se voient.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

''' <summary>
''' État d'une ligne de référentiel tel que la liste le montre (UI-9.4, 18/10/2026). Une ligne
''' désactivée l'est d'abord : c'est ce qui change son usage, système ou non.
''' </summary>
Public Enum EtatReferentielLigne
    Normal
    Systeme
    Inactif
End Enum

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
    ' un montant associé (ex. ref_prix_seance.montant). Nothing pour les autres.
    Public Property Tarif As Decimal?

    ' Information complémentaire affichée en liste (optionnel) : texte libre propre au
    ' référentiel (ex. durée en mois d'une périodicité, périodicité d'un organisme).
    Public Property Complement As String

    ' Indique si l'élément est une ligne système, utilisée par l'application : il ne peut donc ni
    ' changer de code ni être désactivé.
    Public Property EstSysteme As Boolean

    ' État affiché dans la liste : inactif d'abord, puis système, sinon normal (V1.1.0).
    Public ReadOnly Property Etat As EtatReferentielLigne
        Get
            If Not Actif Then Return EtatReferentielLigne.Inactif
            Return If(EstSysteme, EtatReferentielLigne.Systeme, EtatReferentielLigne.Normal)
        End Get
    End Property

    ' Libellé de l'état (colonne « État », triable) ; vide pour une ligne normale (V1.1.0).
    Public ReadOnly Property LibelleEtat As String
        Get
            Select Case Etat
                Case EtatReferentielLigne.Inactif : Return "Inactif"
                Case EtatReferentielLigne.Systeme : Return "Système"
                Case Else : Return String.Empty
            End Select
        End Get
    End Property

#End Region

End Class
