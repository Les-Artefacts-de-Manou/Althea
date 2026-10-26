' -------------------------------------------------------------------------------------------------
' Classe      : QuotaInamiInfo
' Projet      : Althéa
' Version     : V2.0.0
' Date        : 20/09/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Objet métier décrivant l'état d'un quota de prestations paramétré pour un dossier.
'
' Responsabilités :
' - Porter les compteurs consommés PAR CATÉGORIE de quota et les plafonds de la règle datée
' - Exposer les propriétés calculées de seuil (alerte, quota atteint, dépassement)
' - Fournir un libellé synthétique destiné au bandeau de la fiche dossier
'
' Important   :
' - Aucune logique d'accès aux données : objet de transport pur
' - Les catégories, plafonds, seuils et durée de période proviennent de ref_categories_quota,
'   ref_quotas et ref_quotas_plafonds ; aucune catégorie n'est codée en dur.
' - Sans règle applicable, les compteurs restent affichés mais aucun plafond n'est supposé :
'   l'anomalie est journalisée par GestionQuotaInami.
' - Les noms historiques « Inami » restent des noms techniques de compatibilité et ne pilotent pas
'   l'assujettissement : celui-ci provient de ref_domaines.soumis_quota.
'
' Dépendances :
' - QuotaInamiRegle / QuotaPlafond (référentiel daté des plafonds et seuils).
' - GestionQuotaInami (calcule et alimente l'état).
' - UC_DossierFiche et SeanceEditor (affichent les informations de contrôle).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

' -------------------------------------------------------------------------------------------------
' Classe      : QuotaPeriodeInami
' Version     : V2.0.0
' Date        : 20/09/2026
'
' Rôle        :
' Décrit une période de quota : sa numérotation, ses bornes et ses compteurs par catégorie.
'
' Remarques   :
' - Une période s'ouvre à la première séance consommante et court pendant la durée paramétrée
'   dans la règle en vigueur à son ouverture (ref_quotas.duree_periode_mois).
' -------------------------------------------------------------------------------------------------
Public Class QuotaPeriodeInami

    ' Numéro d'ordre de la période dans le dossier (1 = première période ouverte)
    Public Property Numero As Integer

    ' Date de la 1re séance ayant ouvert la période
    Public Property DateDebut As Date

    ' Durée de la période, figée à son ouverture
    Public Property DureeMois As Integer = QuotaInamiInfo.DUREE_PERIODE_MOIS_DEFAUT

    ' Règle applicable à la période (Nothing si non paramétrée)
    Public Property Regle As QuotaInamiRegle

    ' Séances consommées par catégorie de quota
    Public Property Compteurs As New Dictionary(Of ULong, Integer)

    ' Fin exclusive de la période : une séance à cette date ouvre la période suivante
    Public ReadOnly Property DateFin As Date
        Get
            Return DateDebut.AddMonths(DureeMois)
        End Get
    End Property

    ' Dernier jour effectivement couvert par la période (borne affichable à l'utilisateur)
    Public ReadOnly Property DateDernierJour As Date
        Get
            Return DateFin.AddDays(-1)
        End Get
    End Property

    Public ReadOnly Property NbTotal As Integer
        Get
            Return Compteurs.Values.Sum()
        End Get
    End Property

    Public Function NbDe(idCategorieQuota As ULong) As Integer
        Dim nb As Integer = 0
        Compteurs.TryGetValue(idCategorieQuota, nb)
        Return nb
    End Function

    ' Incrémente le compteur de la catégorie et retourne le rang obtenu
    Public Function Incrementer(idCategorieQuota As ULong) As Integer
        Dim nb As Integer = NbDe(idCategorieQuota) + 1
        Compteurs(idCategorieQuota) = nb
        Return nb
    End Function

End Class

' -------------------------------------------------------------------------------------------------
' Classe      : QuotaCategorieEtat
'
' Rôle        :
' Ligne d'affichage : consommation d'une catégorie face à son plafond, pour la période courante.
' -------------------------------------------------------------------------------------------------
Public Class QuotaCategorieEtat

    Public Property IdCategorieQuota As ULong

    Public Property LibelleCategorie As String = String.Empty

    Public Property Nb As Integer

    ' Nothing si la règle ne plafonne pas la catégorie (ou si aucune règle n'est paramétrée)
    Public Property Plafond As Integer?

    Public ReadOnly Property Epuise As Boolean
        Get
            Return Plafond.HasValue AndAlso Nb >= Plafond.Value
        End Get
    End Property

    Public ReadOnly Property Depasse As Boolean
        Get
            Return Plafond.HasValue AndAlso Nb > Plafond.Value
        End Get
    End Property

    Public ReadOnly Property LibelleCompteur As String
        Get
            Return If(Plafond.HasValue, $"{Nb} / {Plafond.Value}", $"{Nb} / ?")
        End Get
    End Property

End Class

Public Class QuotaInamiInfo

#Region "Valeurs de repli"

    ' Durée de période appliquée en repli documenté lorsqu'aucune règle n'est résolue (journalisé).
    Public Const DUREE_PERIODE_MOIS_DEFAUT As Integer = 12

#End Region

#Region "Propriétés"

    ' Règle applicable à la période courante ; Nothing si aucune règle n'est paramétrée.
    Public Property Regle As QuotaInamiRegle

    ' Indique si le dossier relève d'un domaine soumis à quota (ref_domaines.soumis_quota).
    Public Property EstDomaineInami As Boolean

    ' Date de la première séance consommante de la période en cours.
    Public Property DateDebutPeriode As Date?

    ' Séances consommées par catégorie sur la période en cours
    Public Property Compteurs As New Dictionary(Of ULong, Integer)

    ' Libellés des catégories rencontrées dans les séances (complète ceux de la règle)
    Public Property LibellesCategories As New Dictionary(Of ULong, String)

    ' Détail de toutes les périodes ouvertes dans le dossier, dans l'ordre chronologique.
    Public Property Periodes As New List(Of QuotaPeriodeInami)

    ' Numéro de la période en cours (0 si aucune séance consommante n'est encodée)
    Public Property NumeroPeriodeCourante As Integer

#End Region

#Region "Propriétés calculées"

    Public ReadOnly Property DureePeriodeMois As Integer
        Get
            Return If(Regle IsNot Nothing, Regle.DureePeriodeMois, DUREE_PERIODE_MOIS_DEFAUT)
        End Get
    End Property

    Public ReadOnly Property DateFinPeriode As Date?
        Get
            If Not DateDebutPeriode.HasValue Then Return Nothing
            Return DateDebutPeriode.Value.AddMonths(DureePeriodeMois)
        End Get
    End Property

    Public ReadOnly Property ARegle As Boolean
        Get
            Return Regle IsNot Nothing
        End Get
    End Property

    Public ReadOnly Property NbTotal As Integer
        Get
            Return Compteurs.Values.Sum()
        End Get
    End Property

    Public ReadOnly Property PlafondTotal As Integer
        Get
            Return If(Regle IsNot Nothing, Regle.PlafondTotal, 0)
        End Get
    End Property

    Public ReadOnly Property SeuilAlerteTotal As Integer
        Get
            Return If(Regle IsNot Nothing, Regle.SeuilAlerteTotal, 0)
        End Get
    End Property

    Public Function NbDe(idCategorieQuota As ULong) As Integer
        Dim nb As Integer = 0
        Compteurs.TryGetValue(idCategorieQuota, nb)
        Return nb
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : Categories
    '
    ' Rôle       :
    ' État de chaque catégorie pour la période courante : catégories plafonnées par la règle
    ' (dans l'ordre d'affichage), puis catégories consommées mais non plafonnées.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property Categories As List(Of QuotaCategorieEtat)
        Get
            Dim result As New List(Of QuotaCategorieEtat)

            If Regle IsNot Nothing Then
                For Each p In Regle.Plafonds.OrderBy(Function(x) x.OrdreAffichage)
                    result.Add(New QuotaCategorieEtat With {
                        .IdCategorieQuota = p.IdCategorieQuota,
                        .LibelleCategorie = p.LibelleCategorie,
                        .Nb = NbDe(p.IdCategorieQuota),
                        .Plafond = p.Plafond
                    })
                Next
            End If

            For Each kv In Compteurs
                If result.Any(Function(c) c.IdCategorieQuota = kv.Key) Then Continue For
                Dim libelle As String = String.Empty
                LibellesCategories.TryGetValue(kv.Key, libelle)
                result.Add(New QuotaCategorieEtat With {
                    .IdCategorieQuota = kv.Key,
                    .LibelleCategorie = If(String.IsNullOrEmpty(libelle), "Catégorie", libelle),
                    .Nb = kv.Value,
                    .Plafond = Nothing
                })
            Next

            Return result
        End Get
    End Property

    Public ReadOnly Property ResteTotal As Integer
        Get
            Return Math.Max(0, PlafondTotal - NbTotal)
        End Get
    End Property

    ' Le plafond global est atteint ou dépassé (jamais vrai sans règle paramétrée)
    Public ReadOnly Property QuotaAtteint As Boolean
        Get
            Return ARegle AndAlso NbTotal >= PlafondTotal
        End Get
    End Property

    ' Le quota global est dépassé : les séances excédentaires sont à charge du patient
    Public ReadOnly Property QuotaDepasse As Boolean
        Get
            Return ARegle AndAlso NbTotal > PlafondTotal
        End Get
    End Property

    ' Zone de pré-alerte : on approche du plafond sans l'avoir encore atteint
    Public ReadOnly Property EnAlerte As Boolean
        Get
            Return ARegle AndAlso NbTotal >= SeuilAlerteTotal AndAlso Not QuotaAtteint
        End Get
    End Property

    ' Libellé de la période affichée dans le bandeau, du type « P1 : 19/09/26 au 18/09/27 ».
    Public ReadOnly Property LibellePeriodeCourante As String
        Get
            If Not EstDomaineInami OrElse Not DateDebutPeriode.HasValue Then Return String.Empty

            Return String.Format("P{0} : {1} au {2}",
                NumeroPeriodeCourante,
                DateDebutPeriode.Value.ToString("dd/MM/yy"),
                DateFinPeriode.Value.AddDays(-1).ToString("dd/MM/yy"))
        End Get
    End Property

    ' Libellé compact des catégories, du type « Soutien 3 / 10 · Traitement 5 / 20 ».
    Public ReadOnly Property LibelleCompteurs As String
        Get
            Dim etats = Categories
            If etats.Count = 0 Then Return "Aucune catégorie de quota"
            Return String.Join("   ·   ", etats.Select(Function(c) $"{c.LibelleCategorie} {c.LibelleCompteur}"))
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : LibelleSynthese
    '
    ' Rôle       :
    ' Récapitulatif complet destiné à l'info-bulle du bandeau de la fiche dossier.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property LibelleSynthese As String
        Get
            If Not EstDomaineInami Then Return String.Empty

            Dim texte As New System.Text.StringBuilder()

            If Not ARegle Then
                texte.AppendLine("Aucune règle de quota paramétrée pour ce domaine à cette date.")
            End If

            If Not DateDebutPeriode.HasValue Then
                texte.Append("Quota : aucune séance consommante encodée")
                Return texte.ToString()
            End If

            For Each periode In Periodes
                If texte.Length > 0 Then texte.AppendLine()

                Dim plafondPeriode As Integer = If(periode.Regle IsNot Nothing, periode.Regle.PlafondTotal, 0)
                Dim detail As String = String.Join(", ",
                    periode.Compteurs.Select(Function(kv)
                                                 Dim libelle As String = String.Empty
                                                 LibellesCategories.TryGetValue(kv.Key, libelle)
                                                 Dim plafond As String = If(periode.Regle IsNot Nothing,
                                                     periode.Regle.PlafondDe(kv.Key).ToString(), "?")
                                                 Return $"{libelle} {kv.Value}/{plafond}"
                                             End Function))

                texte.AppendFormat("{0}Période {1} (du {2} au {3}) : {4}/{5} séances — {6}",
                    If(periode.Numero = NumeroPeriodeCourante, "► ", ""),
                    periode.Numero,
                    periode.DateDebut.ToString("dd/MM/yyyy"),
                    periode.DateDernierJour.ToString("dd/MM/yyyy"),
                    periode.NbTotal,
                    If(periode.Regle IsNot Nothing, plafondPeriode.ToString(), "?"),
                    detail)
            Next

            Return texte.ToString()
        End Get
    End Property

#End Region

End Class
