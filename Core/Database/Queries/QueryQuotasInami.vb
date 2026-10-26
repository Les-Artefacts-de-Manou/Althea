' -------------------------------------------------------------------------------------------------
' Module      : QueryQuotasInami
' Projet      : Althéa
' Version     : V1.3.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table référentielle ref_quotas, qui paramètre les
' plafonds de prestations remboursées par domaine conventionné.
'
' Responsabilités :
' - Sélectionner les règles de quota (toutes, ou pour un domaine donné)
' - Résoudre la règle applicable à un domaine à une date donnée
' - Détecter un chevauchement de périodes de validité avant enregistrement
' - Insérer, mettre à jour, désactiver et supprimer une règle
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères), aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_quota est généré par la base (AUTO_INCREMENT) : jamais fourni par l'application
' - Le plafond TOTAL n'est pas stocké : il se déduit de la somme des deux plafonds
'
' Historique  :
' - V1.3.0 (22/10/2026, audit B.6) : SelectQuotaInamiParDossier remplacée par SelectIdDomaineDossier
'   (règle lue par domaine).
' - V1.2.0 (21/10/2026, audit B.2) : ArreterRegleOuverte (arrêt de la règle précédente à l'ajout
'   d'une règle, seulement si elle est encore ouverte).
'
' Dépendances :
' - Utilisé par GestionQuotasInami pour les opérations CRUD sur ref_quotas
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryQuotasInami

#Region "SELECT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectQuotasInamiTous
    '
    ' Rôle       :
    ' Liste complète des règles paramétrées, pour l'écran d'administration.
    '
    ' Remarques  :
    ' - Le libellé du domaine est joint : l'écran doit afficher un nom lisible, pas un identifiant.
    ' - Tri par domaine puis par période décroissante : la règle la plus récente apparaît en tête,
    '   c'est celle que l'utilisateur consulte ou modifie le plus souvent.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectQuotasInamiTous As String
        Get
            Return "
SELECT
    q.id_quota,
    q.id_domaine,
    d.code_domaine,
    d.libelle_domaine,
    q.seuil_alerte_total,
    q.duree_periode_mois,
    q.date_debut_validite,
    q.date_fin_validite,
    q.actif
FROM ref_quotas q
INNER JOIN ref_domaines d ON d.id_domaine = q.id_domaine
ORDER BY d.ordre_affichage, d.libelle_domaine, q.date_debut_validite DESC;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectQuotaInamiApplicable
    '
    ' Rôle       :
    ' Retourne la règle de quota applicable à un domaine à une date donnée.
    '
    ' Paramètres SQL :
    ' - @id_domaine : domaine du dossier évalué
    ' - @date_reference : date à laquelle la règle doit s'appliquer (début de la période de quota)
    '
    ' Remarques  :
    ' - La date de référence est celle du DÉBUT de la période de quota évaluée, et non la date du
    '   jour : une période ouverte sous l'ancienne convention doit se terminer sous cette convention,
    '   sinon les compteurs déjà communiqués au patient changeraient rétroactivement.
    ' - date_fin_validite NULL signifie « toujours en vigueur ».
    ' - LIMIT 1 avec tri décroissant : si plusieurs règles se chevauchaient malgré le contrôle métier,
    '   la plus récente l'emporte plutôt que de faire échouer le calcul.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectQuotaInamiApplicable As String
        Get
            Return "
SELECT
    q.id_quota,
    q.id_domaine,
    d.code_domaine,
    d.libelle_domaine,
    q.seuil_alerte_total,
    q.duree_periode_mois,
    q.date_debut_validite,
    q.date_fin_validite,
    q.actif
FROM ref_quotas q
INNER JOIN ref_domaines d ON d.id_domaine = q.id_domaine
WHERE q.id_domaine = @id_domaine
  AND q.actif = 1
  AND q.date_debut_validite <= @date_reference
  AND (q.date_fin_validite IS NULL OR q.date_fin_validite >= @date_reference)
ORDER BY q.date_debut_validite DESC
LIMIT 1;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectIdDomaineDossier
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Domaine ACTUEL d'un dossier : la règle de quota applicable est ensuite lue (et gardée en
    ' mémoire) par domaine.
    '
    ' Paramètres SQL :
    ' - @id_dossier : dossier évalué
    '
    ' Remarques  :
    ' - Remplace SelectQuotaInamiParDossier (22/10/2026, audit B.6) : la règle était gardée en mémoire
    '   par dossier ; un dossier qui changeait de domaine gardait la règle de l'ancien.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectIdDomaineDossier As String
        Get
            Return "
SELECT id_domaine
FROM dossiers
WHERE id_dossier = @id_dossier;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountChevauchementQuotaInami
    '
    ' Rôle       :
    ' Compte les règles d'un même domaine dont la période recouvre celle que l'on s'apprête à
    ' enregistrer.
    '
    ' Paramètres SQL :
    ' - @id_domaine  : domaine concerné
    ' - @date_debut  : début de la période saisie
    ' - @date_fin    : fin de la période saisie (NULL si toujours en vigueur)
    ' - @id          : identifiant de la règle en cours d'édition, exclu du contrôle (0 en création)
    '
    ' Remarques  :
    ' - Le chevauchement ne peut pas être exprimé en contrainte SQL déclarative : ce contrôle est
    '   indispensable, faute de quoi deux règles concurrentes rendraient le plafond applicable
    '   indéterminé.
    ' - Deux intervalles se chevauchent si chacun commence avant la fin de l'autre, les bornes NULL
    '   étant traitées comme « infini ».
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountChevauchementQuotaInami As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_quotas
WHERE id_domaine = @id_domaine
  AND id_quota <> @id
  AND date_debut_validite <= IFNULL(@date_fin, '9999-12-31')
  AND IFNULL(date_fin_validite, '9999-12-31') >= @date_debut;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectPlafondsParQuota
    '
    ' Rôle       :
    ' Plafonds par catégorie d'une ou plusieurs règles (@ids_quota injectés en liste d'entiers).
    '
    ' Remarques  :
    ' - Le marqueur {IDS} est remplacé par une liste d'identifiants numériques produite par le code
    '   (jamais par une saisie utilisateur).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectPlafondsParQuotas As String
        Get
            Return "
SELECT
    p.id_quota,
    p.id_categorie_quota,
    c.code_categorie,
    c.libelle_categorie,
    c.ordre_affichage,
    p.plafond
FROM ref_quotas_plafonds p
INNER JOIN ref_categories_quota c ON c.id_categorie_quota = p.id_categorie_quota
WHERE p.id_quota IN ({IDS})
ORDER BY p.id_quota, c.ordre_affichage, c.libelle_categorie;
"
        End Get
    End Property

    Public ReadOnly Property SelectIdDernierQuota As String
        Get
            Return "SELECT LAST_INSERT_ID();"
        End Get
    End Property

#End Region

#Region "INSERT"

    Public ReadOnly Property InsertPlafondQuota As String
        Get
            Return "
INSERT INTO ref_quotas_plafonds (id_quota, id_categorie_quota, plafond)
VALUES (@id_quota, @id_categorie_quota, @plafond);"
        End Get
    End Property

    Public ReadOnly Property SupprimerPlafondsQuota As String
        Get
            Return "
DELETE FROM ref_quotas_plafonds
WHERE id_quota = @id_quota;"
        End Get
    End Property

    Public ReadOnly Property InsertQuotaInami As String
        Get
            Return "
INSERT INTO ref_quotas
(
    id_domaine,
    seuil_alerte_total,
    duree_periode_mois,
    date_debut_validite,
    date_fin_validite,
    actif
)
VALUES
(
    @id_domaine,
    @seuil_alerte_total,
    @duree_periode_mois,
    @date_debut,
    @date_fin,
    @actif
);"
        End Get
    End Property

#End Region

#Region "UPDATE"

    Public ReadOnly Property UpdateQuotaInami As String
        Get
            Return "
UPDATE ref_quotas
SET id_domaine = @id_domaine,
    seuil_alerte_total = @seuil_alerte_total,
    duree_periode_mois = @duree_periode_mois,
    date_debut_validite = @date_debut,
    date_fin_validite = @date_fin,
    actif = @actif
WHERE id_quota = @id;"
        End Get
    End Property

    Public ReadOnly Property DefinirActivationQuotaInami As String
        Get
            Return "
UPDATE ref_quotas
SET actif = @actif
WHERE id_quota = @id;"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateFinValiditeQuota
    ' Rôle       : Change seulement la date de fin d'une règle (rouvrir la règle précédente quand la
    '              règle qui l'avait arrêtée est supprimée). @date_fin NULL = règle ouverte.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateFinValiditeQuota As String
        Get
            Return "
UPDATE ref_quotas
SET date_fin_validite = @date_fin
WHERE id_quota = @id;"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : ArreterRegleOuverte
    ' Version    : V1.0.0
    ' Date       : 21/10/2026
    ' Rôle       : Arrête une règle à la date donnée, seulement si elle est encore ouverte et active
    '              (aucune ligne modifiée = elle a changé entre-temps). Utilisée par
    '              GestionQuotasInami.AjouterRegleEnChaine, dans la transaction de l'ajout.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property ArreterRegleOuverte As String
        Get
            Return "
UPDATE ref_quotas
SET date_fin_validite = @date_fin
WHERE id_quota = @id
  AND date_fin_validite IS NULL
  AND actif = 1;"
        End Get
    End Property

#End Region

#Region "Contrôles"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountSeancesSurPeriodeRegle
    ' Rôle       : Nombre de séances du domaine sur la période d'une règle ; une règle qui en compte
    '              a servi au décompte des quotas et ne peut plus être supprimée.
    ' Remarques  : Même critère que pour l'historique des tarifs (QueryLiaisonsPrixDomaines).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountSeancesSurPeriodeRegle As String
        Get
            Return "
SELECT COUNT(*)
FROM seances s
INNER JOIN dossiers d ON d.id_dossier = s.id_dossier
WHERE d.id_domaine = @idDomaine
AND s.date_seance >= @dateDebut
AND s.date_seance <= IFNULL(@dateFin, '9999-12-31');"
        End Get
    End Property

#End Region

#Region "DELETE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SupprimerQuotaInami
    '
    ' Remarques  :
    ' - La suppression physique reste possible : une règle de quota n'est référencée par aucune autre
    '   table. Elle est cependant déconseillée dès lors que la règle a servi à calculer des quotas,
    '   car l'historique de facturation ne serait plus reconstituable.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SupprimerQuotaInami As String
        Get
            Return "
DELETE FROM ref_quotas
WHERE id_quota = @id;"
        End Get
    End Property

#End Region

End Module
