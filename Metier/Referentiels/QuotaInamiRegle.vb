' -------------------------------------------------------------------------------------------------
' Classe      : QuotaInamiRegle
' Projet      : Althéa
' Version     : V1.2.0
' Date        : 18/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Représente une règle de quota annuel de prestations remboursées, telle que paramétrée dans la
' table référentielle ref_quotas pour un domaine conventionné.
'
' Contexte métier :
' Les plafonds de séances remboursées sont fixés par la convention INAMI. Ils sont susceptibles
' d'évoluer, et un autre domaine conventionné pourrait relever de plafonds différents. Ils sont
' donc paramétrés plutôt que codés en dur, et historisés par période de validité afin qu'un quota
' passé reste recalculable avec les règles qui s'appliquaient à l'époque.
'
' Remarques   :
' - Le plafond total n'est pas stocké : il se déduit de la somme des deux plafonds, ce qui interdit
'   toute incohérence entre le total et le détail.
' - Aucun accès base de données : objet de transport pur.
' - V1.2.0 (18/10/2026, UI-9.4) : état en famille (Etat, EtatRegleQuota) pour la pastille de la grille
'   des règles ; LibelleEtat en découle, ses textes sont inchangés.
'
' Dépendances :
' - Alimenté par GestionQuotasInami depuis QueryQuotasInami
' - Consommé par QuotaInamiInfo et GestionQuotaInami pour l'évaluation des compteurs
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

''' <summary>État d'une règle de quota à la date du jour (UI-9.4, 18/10/2026).</summary>
Public Enum EtatRegleQuota
    EnVigueur
    AVenir
    Terminee
    Desactivee
End Enum

Public Class QuotaInamiRegle

#Region "Propriétés"

    ' Identifiant de la règle (0 tant qu'elle n'est pas enregistrée)
    Public Property IdQuotaInami As ULong

    ' Domaine conventionné auquel la règle s'applique
    Public Property IdDomaine As ULong

    ' Code court du domaine (issu de la jointure, lecture seule côté écran)
    Public Property CodeDomaine As String = String.Empty

    ' Libellé du domaine (issu de la jointure) — affiché dans la grille d'administration
    Public Property LibelleDomaine As String = String.Empty

    ' Plafonds par catégorie de quota du domaine (ref_quotas_plafonds) — nombre libre
    Public Property Plafonds As New List(Of QuotaPlafond)

    ' Total à partir duquel l'utilisateur est averti de l'approche du plafond
    Public Property SeuilAlerteTotal As Integer

    ' Durée de la période glissante ouverte à la 1re séance consommante
    Public Property DureePeriodeMois As Integer = 12

    ' Premier jour d'application de la règle
    Public Property DateDebutValidite As Date

    ' Dernier jour d'application ; Nothing tant que la règle est en vigueur
    Public Property DateFinValidite As Date?

    ' Règle utilisable ; une règle désactivée est conservée pour l'historique mais n'est plus appliquée
    Public Property Actif As Boolean = True

#End Region

#Region "Helpers"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : PlafondTotal
    '
    ' Rôle       :
    ' Nombre total de séances remboursées sur une période.
    '
    ' Remarques  :
    ' - Calculé et non stocké : le total ne peut ainsi jamais diverger du détail par prestation.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property PlafondTotal As Integer
        Get
            Return Plafonds.Sum(Function(p) p.Plafond)
        End Get
    End Property

    ' Plafond d'une catégorie ; 0 si la catégorie n'est pas plafonnée par la règle
    Public Function PlafondDe(idCategorieQuota As ULong) As Integer
        Dim p As QuotaPlafond = Plafonds.FirstOrDefault(Function(x) x.IdCategorieQuota = idCategorieQuota)
        Return If(p Is Nothing, 0, p.Plafond)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstEnVigueur
    '
    ' Rôle       :
    ' Indique si la règle s'applique à la date du jour.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstEnVigueur As Boolean
        Get
            Return EstApplicableLe(Date.Today)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : LibelleEtat
    '
    ' Rôle       :
    ' État lisible de la règle à la date du jour : Désactivée, En vigueur, À venir ou Terminée
    ' (remplace « Hors période », qui ne disait pas si la règle était passée ou future).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property LibelleEtat As String
        Get
            Select Case Etat
                Case EtatRegleQuota.Desactivee : Return "Désactivée"
                Case EtatRegleQuota.EnVigueur : Return "En vigueur"
                Case EtatRegleQuota.AVenir : Return "À venir"
                Case Else : Return "Terminée"
            End Select
        End Get
    End Property

    ' Famille de l'état à la date du jour (V1.2.0) : même règle que LibelleEtat.
    Public ReadOnly Property Etat As EtatRegleQuota
        Get
            If Not Actif Then Return EtatRegleQuota.Desactivee
            If EstEnVigueur Then Return EtatRegleQuota.EnVigueur
            If DateDebutValidite.Date > Date.Today Then Return EtatRegleQuota.AVenir
            Return EtatRegleQuota.Terminee
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : EstApplicableLe
    '
    ' Rôle       :
    ' Indique si la règle s'applique à une date donnée.
    '
    ' Paramètres :
    ' - dateReference : date à évaluer
    '
    ' Remarques  :
    ' - Une règle désactivée n'est applicable à aucune date.
    ' - DateFinValidite vide signifie « toujours en vigueur ».
    ' -------------------------------------------------------------------------------------------------
    Public Function EstApplicableLe(dateReference As Date) As Boolean

        If Not Actif Then Return False
        If dateReference.Date < DateDebutValidite.Date Then Return False
        If DateFinValidite.HasValue AndAlso dateReference.Date > DateFinValidite.Value.Date Then Return False

        Return True

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : LibellePeriodeValidite
    '
    ' Rôle       :
    ' Libellé de la période de validité destiné à la grille d'administration.
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
    ' Propriété  : LibellePlafonds
    '
    ' Rôle       :
    ' Résumé compact des plafonds, du type « Soutien 10 + Traitement 20 = 30 (12 mois) ».
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property LibellePlafonds As String
        Get
            If Plafonds.Count = 0 Then Return "Aucun plafond"

            Dim detail As String = String.Join(" + ",
                Plafonds.OrderBy(Function(p) p.OrdreAffichage).Select(Function(p) $"{p.LibelleCategorie} {p.Plafond}"))

            Return $"{detail} = {PlafondTotal} ({DureePeriodeMois} mois)"
        End Get
    End Property

#End Region

End Class
