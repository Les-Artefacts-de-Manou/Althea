' -------------------------------------------------------------------------------------------------
' Module      : QueryTarification
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL nécessaires à la résolution du tarif d'une séance à sa création
' (contexte tarifaire du dossier et flag « dû » du statut).
'
' Responsabilités :
' - Fournir le contexte tarifaire d'un dossier : comportements du domaine (tarif_convention,
'   no_show_facturable) et montant du tarif patient rattaché (ref_prix_seance via
'   dossiers.id_prix_seance)
' - Fournir le tarif conventionné applicable à un domaine à une date donnée
'   (lia_prix_seance_domaines, historisée)
' - Fournir le montant d'un tarif de ref_prix_seance identifié par son code (repli uniquement)
' - Fournir le flag « dû » (est_du) et la case « absence non excusée » (est_absence) d'un statut
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères) ; aucun accès base de données ici
' - Module statique (propriétés/const ReadOnly partagées)
' - V1.1.0 (22/10/2026, audit B.6, v2.53.0) : SelectEstAbsenceParStatut.
'
' Dépendances :
' - Utilisé par GestionTarification
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryTarification

#Region "Constantes et requêtes tarifaires"

    ' -------------------------------------------------------------------------------------------------
    ' Constante  : CODE_PRIX_INAMI
    ' Version    : V2.0.0
    '
    ' Rôle       :
    ' Code du tarif conventionné historique dans ref_prix_seance.
    '
    ' ATTENTION  :
    ' Cette constante n'est PLUS le pivot de résolution du tarif conventionné : elle ne sert que
    ' de REPLI lorsque aucun tarif n'est rattaché au domaine dans lia_prix_seance_domaines.
    ' Conformément à la règle de pilotage par les référentiels (Rules.md §6), le tarif doit être
    ' rattaché au domaine : un code global ne permettrait pas à deux domaines conventionnés
    ' d'avoir des montants distincts. Ne pas réintroduire d'usage direct.
    ' -------------------------------------------------------------------------------------------------
    Public Const CODE_PRIX_INAMI As String = "INAMI"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectContexteTarifaireDossier
    ' Version    : V2.0.0
    ' Date       : 19/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Contexte tarifaire d'un dossier (comportements du domaine + tarif patient)
    '
    ' Paramètres SQL :
    ' - @id_dossier : Identifiant du dossier
    '
    ' Colonnes retournées :
    ' - id_domaine           : Domaine du dossier, nécessaire à la résolution du tarif conventionné
    ' - tarif_convention     : 1 si le tarif provient du domaine et non du tarif patient
    ' - no_show_facturable   : 0 si un no-show ne doit pas être facturé au patient
    ' - soumis_quota         : 1 si les séances du domaine sont décomptées d'un quota
    ' - montant_prix_patient : montant du tarif patient rattaché, ou NULL si non renseigné
    '
    ' Utilisé par :
    ' - GestionTarification.GetContexteTarifaireDossier
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectContexteTarifaireDossier As String
        Get
            Return "
SELECT
    dom.id_domaine           AS id_domaine,
    dom.tarif_convention     AS tarif_convention,
    dom.no_show_facturable   AS no_show_facturable,
    dom.soumis_quota         AS soumis_quota,
    pp.montant               AS montant_prix_patient
FROM dossiers d
INNER JOIN ref_domaines dom ON dom.id_domaine = d.id_domaine
LEFT JOIN ref_prix_seance pp ON pp.id_prix_seance = d.id_prix_seance
WHERE d.id_dossier = @id_dossier;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectTarifConventionParDomaine
    ' Version    : V1.0.0
    ' Date       : 19/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Montant du tarif conventionné applicable à un domaine à une date donnée
    '
    ' Paramètres SQL :
    ' - @id_domaine     : Domaine du dossier
    ' - @date_reference : Date de la séance (et non la date du jour)
    '
    ' Remarques  :
    ' - La résolution se fait à la DATE DE LA SÉANCE : une révision tarifaire ne doit pas
    '   modifier rétroactivement le montant d'une séance déjà encodée.
    ' - Tri par date de début décroissante : si plusieurs tarifs se chevauchent à tort, le plus
    '   récemment ouvert l'emporte. Le chevauchement reste une anomalie de paramétrage, détectée
    '   et journalisée par la couche métier.
    '
    ' Utilisé par :
    ' - GestionTarification.GetTarifConventionDomaine
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectTarifConventionParDomaine As String
        Get
            Return "
SELECT ps.montant
FROM lia_prix_seance_domaines lia
INNER JOIN ref_prix_seance ps ON ps.id_prix_seance = lia.id_prix_seance
WHERE lia.id_domaine = @id_domaine
  AND lia.actif = 1
  AND ps.actif = 1
  AND lia.date_debut_validite <= @date_reference
  AND (lia.date_fin_validite IS NULL OR lia.date_fin_validite >= @date_reference)
ORDER BY lia.date_debut_validite DESC
LIMIT 1;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountTarifsConventionApplicables
    ' Version    : V1.0.0
    ' Date       : 19/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Nombre de tarifs conventionnés simultanément applicables à un domaine
    '
    ' Paramètres SQL :
    ' - @id_domaine     : Domaine contrôlé
    ' - @date_reference : Date évaluée
    '
    ' Remarques  :
    ' - Doit retourner 0 ou 1. Une valeur supérieure révèle un chevauchement de périodes : le
    '   montant facturé deviendrait arbitraire, l'anomalie doit être journalisée.
    '
    ' Utilisé par :
    ' - GestionTarification.GetTarifConventionDomaine (contrôle de cohérence)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountTarifsConventionApplicables As String
        Get
            Return "
SELECT COUNT(*)
FROM lia_prix_seance_domaines lia
INNER JOIN ref_prix_seance ps ON ps.id_prix_seance = lia.id_prix_seance
WHERE lia.id_domaine = @id_domaine
  AND lia.actif = 1
  AND ps.actif = 1
  AND lia.date_debut_validite <= @date_reference
  AND (lia.date_fin_validite IS NULL OR lia.date_fin_validite >= @date_reference);
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectMontantParCodePrix
    ' Version    : V1.0.0
    ' Date       : 15/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Montant courant d'un tarif de ref_prix_seance identifié par son code
    '
    ' Paramètres SQL :
    ' - @code_prix_seance : Code technique du tarif (ex. 'INAMI')
    '
    ' Utilisé par :
    ' - GestionTarification.GetMontantParCodePrix (résolution du tarif INAMI)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectMontantParCodePrix As String
        Get
            Return "
SELECT montant
FROM ref_prix_seance
WHERE code_prix_seance = @code_prix_seance
  AND actif = 1
LIMIT 1;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectEstDuParStatut
    ' Version    : V1.0.0
    ' Date       : 15/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Flag « dû » (est_du) d'un statut de séance
    '
    ' Paramètres SQL :
    ' - @id_statut_seance : Identifiant du statut de séance
    '
    ' Utilisé par :
    ' - GestionTarification.StatutEstDu
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectEstDuParStatut As String
        Get
            Return "
SELECT est_du
FROM ref_statuts_seance
WHERE id_statut_seance = @id_statut_seance
LIMIT 1;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectEstAbsenceParStatut
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Description : Case « absence non excusée » (est_absence, v2.53.0) d'un statut de séance
    '
    ' Paramètres SQL :
    ' - @id_statut_seance : Identifiant du statut de séance
    '
    ' Utilisé par :
    ' - GestionTarification.StatutEstAbsence
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectEstAbsenceParStatut As String
        Get
            Return "
SELECT est_absence
FROM ref_statuts_seance
WHERE id_statut_seance = @id_statut_seance
LIMIT 1;
"
        End Get
    End Property

#End Region

End Module
