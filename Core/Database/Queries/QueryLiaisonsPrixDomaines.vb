' -------------------------------------------------------------------------------------------------
' Module      : QueryLiaisonsPrixDomaines
' Projet      : Althéa
' Version     : V1.1.0
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table de liaison lia_prix_seance_domaines, qui rattache
' un tarif de séance à un domaine sur une période de validité (D-POR-4).
'
' Responsabilités :
' - Sélectionner les rattachements d'un tarif ou d'un domaine, avec les libellés joints
' - Détecter les chevauchements de période pour un même couple tarif / domaine
' - Détecter la présence d'un autre tarif conventionné actif sur la même période pour un domaine
' - Insérer, mettre à jour, clôturer, activer/désactiver et supprimer un rattachement
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères) : aucun accès base de données ici
' - La liaison est HISTORISÉE : une évolution de tarif se traduit par la clôture de la période
'   en cours puis la création d'une nouvelle liaison, jamais par l'écrasement de l'historique
' - date_fin_validite à NULL signifie « toujours en vigueur »
'
' Dépendances :
' - Utilisé par GestionLiaisonsPrixDomaines
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : UpdateLiaison, plus appelée par le métier, supprimée.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryLiaisonsPrixDomaines

#Region "SELECT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectLiaisonsParPrix
    ' Type       : String (ReadOnly)
    '
    ' Description : Rattachements d'un tarif à ses domaines, périodes comprises
    '
    ' Paramètres SQL :
    ' - @idPrixSeance : Identifiant du tarif de séance
    '
    ' Utilisé par :
    ' - GestionLiaisonsPrixDomaines.GetLiaisonsParPrix
    '
    ' Remarques  :
    ' - Inclut les rattachements désactivés : le filtrage éventuel est fait par l'appelant
    ' - Tri antéchronologique afin de présenter la période courante en premier
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectLiaisonsParPrix As String
        Get
            Return "
SELECT
    lia.id_lia_prix_seance_domaine,
    lia.id_prix_seance,
    lia.id_domaine,
    dom.code_domaine,
    dom.libelle_domaine,
    ps.code_prix_seance,
    ps.libelle_prix_seance,
    ps.montant,
    lia.date_debut_validite,
    lia.date_fin_validite,
    lia.actif
FROM lia_prix_seance_domaines lia
INNER JOIN ref_domaines dom ON dom.id_domaine = lia.id_domaine
INNER JOIN ref_prix_seance ps ON ps.id_prix_seance = lia.id_prix_seance
WHERE lia.id_prix_seance = @idPrixSeance
ORDER BY dom.ordre_affichage, dom.libelle_domaine, lia.date_debut_validite DESC;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectLiaisonsParDomaine
    ' Type       : String (ReadOnly)
    '
    ' Description : Rattachements d'un domaine à ses tarifs conventionnés, périodes comprises
    '
    ' Paramètres SQL :
    ' - @idDomaine : Identifiant du domaine
    '
    ' Utilisé par :
    ' - GestionLiaisonsPrixDomaines.GetLiaisonsParDomaine
    '
    ' Remarques  :
    ' - Alimente l'onglet « Tarifs conventionnés » de l'écran des domaines
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectLiaisonsParDomaine As String
        Get
            Return "
SELECT
    lia.id_lia_prix_seance_domaine,
    lia.id_prix_seance,
    lia.id_domaine,
    dom.code_domaine,
    dom.libelle_domaine,
    ps.code_prix_seance,
    ps.libelle_prix_seance,
    ps.montant,
    lia.date_debut_validite,
    lia.date_fin_validite,
    lia.actif
FROM lia_prix_seance_domaines lia
INNER JOIN ref_domaines dom ON dom.id_domaine = lia.id_domaine
INNER JOIN ref_prix_seance ps ON ps.id_prix_seance = lia.id_prix_seance
WHERE lia.id_domaine = @idDomaine
ORDER BY lia.date_debut_validite DESC, ps.libelle_prix_seance;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountChevauchementLiaison
    ' Type       : String (ReadOnly)
    '
    ' Description : Comptage des rattachements du même couple tarif / domaine dont la période
    '               chevauche celle proposée
    '
    ' Paramètres SQL :
    ' - @idPrixSeance : Identifiant du tarif
    ' - @idDomaine    : Identifiant du domaine
    ' - @dateDebut    : Premier jour de la période proposée
    ' - @dateFin      : Dernier jour de la période proposée (NULL si toujours en vigueur)
    ' - @idExclu      : Identifiant du rattachement à exclure (0 en création)
    '
    ' Utilisé par :
    ' - GestionLiaisonsPrixDomaines.PeriodeChevauche
    '
    ' Remarques  :
    ' - IFNULL(..., '9999-12-31') traite « toujours en vigueur » comme une borne très lointaine
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountChevauchementLiaison As String
        Get
            Return "
SELECT COUNT(*)
FROM lia_prix_seance_domaines
WHERE id_prix_seance = @idPrixSeance
AND id_domaine = @idDomaine
AND id_lia_prix_seance_domaine <> @idExclu
AND date_debut_validite <= IFNULL(@dateFin, '9999-12-31')
AND IFNULL(date_fin_validite, '9999-12-31') >= @dateDebut;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountAutreTarifActifDomaine
    ' Type       : String (ReadOnly)
    '
    ' Description : Comptage des rattachements ACTIFS d'un autre tarif au même domaine sur une
    '               période qui chevauche celle proposée
    '
    ' Paramètres SQL :
    ' - @idPrixSeance : Identifiant du tarif proposé (exclu du comptage)
    ' - @idDomaine    : Identifiant du domaine
    ' - @dateDebut    : Premier jour de la période proposée
    ' - @dateFin      : Dernier jour de la période proposée (NULL si toujours en vigueur)
    ' - @idExclu      : Identifiant du rattachement à exclure (0 en création)
    '
    ' Utilisé par :
    ' - GestionLiaisonsPrixDomaines.AutreTarifActifSurPeriode
    '
    ' Remarques  :
    ' - Deux tarifs conventionnés actifs simultanément sur un même domaine rendraient la
    '   résolution du tarif ambiguë : ce contrôle permet d'avertir l'utilisateur.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountAutreTarifActifDomaine As String
        Get
            Return "
SELECT COUNT(*)
FROM lia_prix_seance_domaines
WHERE id_domaine = @idDomaine
AND id_prix_seance <> @idPrixSeance
AND id_lia_prix_seance_domaine <> @idExclu
AND actif = 1
AND date_debut_validite <= IFNULL(@dateFin, '9999-12-31')
AND IFNULL(date_fin_validite, '9999-12-31') >= @dateDebut;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountSeancesSurPeriodeLiaison
    ' Type       : String (ReadOnly)
    '
    ' Description : Comptage des séances déjà enregistrées sur la période d'un rattachement
    '
    ' Paramètres SQL :
    ' - @idDomaine : Identifiant du domaine
    ' - @dateDebut : Premier jour de la période
    ' - @dateFin   : Dernier jour de la période (NULL si toujours en vigueur)
    '
    ' Utilisé par :
    ' - GestionLiaisonsPrixDomaines.PeriodeComporteSeances
    '
    ' Remarques  :
    ' - Sert à prévenir l'utilisateur avant la suppression d'un historique tarifaire déjà utilisé
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountSeancesSurPeriodeLiaison As String
        Get
            Return "
SELECT COUNT(*)
FROM seances s
INNER JOIN dossiers d ON d.id_dossier = s.id_dossier
WHERE d.id_domaine = @idDomaine
AND s.date_seance >= @dateDebut
AND s.date_seance <= IFNULL(@dateFin, '9999-12-31');
"
        End Get
    End Property

#End Region

#Region "INSERT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : InsertLiaison
    ' Type       : String (ReadOnly)
    '
    ' Description : Insertion d'un rattachement tarif / domaine sur une période
    '
    ' Paramètres SQL :
    ' - @idPrixSeance : Identifiant du tarif
    ' - @idDomaine    : Identifiant du domaine
    ' - @dateDebut    : Premier jour d'application
    ' - @dateFin      : Dernier jour d'application (NULL si toujours en vigueur)
    ' - @actif        : Rattachement actif (1) ou désactivé (0)
    '
    ' Utilisé par :
    ' - GestionLiaisonsPrixDomaines.InsertLiaison
    '
    ' Remarques  :
    ' - id_lia_prix_seance_domaine est généré par la base (AUTO_INCREMENT)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property InsertLiaison As String
        Get
            Return "
INSERT INTO lia_prix_seance_domaines
(
    id_prix_seance,
    id_domaine,
    date_debut_validite,
    date_fin_validite,
    actif
)
VALUES
(
    @idPrixSeance,
    @idDomaine,
    @dateDebut,
    @dateFin,
    @actif
);"
        End Get
    End Property

#End Region

#Region "UPDATE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : CloturerLiaison
    ' Type       : String (ReadOnly)
    '
    ' Description : Clôture d'un rattachement à une date donnée
    '
    ' Paramètres SQL :
    ' - @id      : Identifiant du rattachement
    ' - @dateFin : Dernier jour d'application
    '
    ' Utilisé par :
    ' - GestionLiaisonsPrixDomaines.CloturerLiaison
    '
    ' Remarques  :
    ' - Préserve l'historique : la ligne reste en base avec sa période fermée
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property CloturerLiaison As String
        Get
            Return "
        UPDATE lia_prix_seance_domaines
        SET date_fin_validite = @dateFin
        WHERE id_lia_prix_seance_domaine = @id"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DefinirActivationLiaison
    ' Type       : String (ReadOnly)
    '
    ' Description : Activation ou désactivation d'un rattachement (soft-delete)
    '
    ' Paramètres SQL :
    ' - @id    : Identifiant du rattachement
    ' - @actif : Nouvel état (1 ou 0)
    '
    ' Utilisé par :
    ' - GestionLiaisonsPrixDomaines.DefinirActivationLiaison
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property DefinirActivationLiaison As String
        Get
            Return "
        UPDATE lia_prix_seance_domaines
        SET actif = @actif
        WHERE id_lia_prix_seance_domaine = @id"
        End Get
    End Property

#End Region

#Region "DELETE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SupprimerLiaison
    ' Type       : String (ReadOnly)
    '
    ' Description : Suppression physique d'un rattachement
    '
    ' Paramètres SQL :
    ' - @id : Identifiant du rattachement à supprimer
    '
    ' Utilisé par :
    ' - GestionLiaisonsPrixDomaines.SupprimerLiaison
    '
    ' Remarques  :
    ' - À réserver aux rattachements créés par erreur et non encore utilisés par des séances ;
    '   sinon privilégier la clôture ou la désactivation pour préserver l'historique.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SupprimerLiaison As String
        Get
            Return "
        DELETE FROM lia_prix_seance_domaines
        WHERE id_lia_prix_seance_domaine = @id"
        End Get
    End Property

#End Region

End Module
