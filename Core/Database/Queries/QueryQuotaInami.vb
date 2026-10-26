' -------------------------------------------------------------------------------------------------
' Module      : QueryQuotaInami
' Projet      : Althéa
' Version     : V2.0.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL de consommation des quotas de prestations par dossier.
'
' Responsabilités :
' - Donner la catégorie de quota consommée par une nature de séance dans le domaine d'un dossier
'
' Important   :
' - Contient uniquement du SQL (chaînes de caractères), aucun accès base de données ici
' - Le discriminant est lia_natures_seance_domaines.id_categorie_quota pour le couple (nature,
'   domaine du dossier) ; une liaison sans catégorie ne consomme jamais le quota.
' - Les statuts consommants sont explicitement documentés par la requête ; les plafonds et seuils
'   restent définis dans ref_quotas, jamais dans ce module.
'
' Historique  :
' - V2.0.0 (22/10/2026, audit B.6) : SelectDateDebutPeriodeInami, SelectCompteursQuotaInami et leur
'   filtre FILTRE_CONSOMMATION supprimés : ils ne calculaient que la première période ; le quota se
'   calcule désormais à un seul endroit (GestionQuotaInami.CalculerQuota), sur les séances lues par
'   GestionSeances (même règle de consommation : ref_statuts_seance.consomme_quota, date de clôture).
'
' Dépendances :
' - Utilisé par GestionQuotaInami
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryQuotaInami

#Region "SELECT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCategorieQuotaParNatureDossier
    ' Version    : V2.0.0
    ' Date       : 19/09/2026
    '
    ' Rôle       :
    ' Retourne la catégorie de quota du rattachement nature / domaine du dossier, afin de déterminer
    ' si la séance en cours d'encodage consomme le quota et, le cas échéant, lequel.
    '
    ' Paramètres SQL :
    ' - @id_nature_seance : Identifiant de la nature de séance
    ' - @id_dossier       : Dossier dont le domaine détermine la catégorie
    '
    ' Retour     :
    ' - Une ligne, colonne id_categorie_quota (NULL ou aucune ligne si hors quota)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCategorieQuotaParNatureDossier As String
        Get
            Return "
SELECT lia.id_categorie_quota
FROM dossiers d
INNER JOIN lia_natures_seance_domaines lia ON lia.id_domaine = d.id_domaine
WHERE d.id_dossier = @id_dossier
  AND lia.id_nature_seance = @id_nature_seance;
"
        End Get
    End Property

#End Region

End Module
