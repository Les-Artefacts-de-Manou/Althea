' -------------------------------------------------------------------------------------------------
' Module      : QuerySeances
' Projet      : Althéa
' Version     : V1.6.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table seances.
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL des séances
' - Sélectionner la liste des séances d'un dossier (avec libellés type/statut + montant réglé)
' - Sélectionner une séance complète par identifiant (avec notes/évolution RTF/TXT)
' - Insérer une séance, mettre à jour son identité, ses notes/évolution et son statut
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_seance et code_seance sont générés par la base : jamais fournis par l'application
' - Les jointures sont en LEFT JOIN pour rester robustes même si un libellé venait à manquer
' - Le montant réglé par séance est agrégé depuis lia_paiements_seances (sous-requête)
'
' Historique  :
' - V1.1.0 (21/10/2026, audit B.2) : UpdateStatutSeanceSiInchange (report : la séance d'origine
'   n'est marquée reportée que si elle est encore planifiée).
' - V1.2.0 (21/10/2026, audit B.3, ADR-47) : BasculerPlanifieesEchues supprimée (une lecture ne
'   modifie plus de statut).
' - V1.3.0 (21/10/2026, audit B.4) : SelectSeancesParDossier et SelectSeancesParDate lisent le solde
'   dans la vue v_seances_solde (réglé, facturable, reste dû, avoir) : une seule définition.
' - V1.4.0 (22/10/2026, audit B.6, migration v2.53.0) : SelectSeancesParDossier et SelectSeancesParDate
'   rendent est_absence (case « Absence non excusée » du statut).
' - V1.5.0 (23/10/2026, recherche transversale R2, ADR-52) : SelectSeancesParPeriode (séances d'une
'   période) ; colonnes communes avec SelectSeancesParDate dans SelectSeancesPlanning (+ code du
'   dossier, identifiant et code du patient).
' - V1.5.1 (24/10/2026, audit C.3) : statut « encaissé » écrit dans le SQL depuis
'   CodesSysteme.StatutsPaiement, plus en clair.
' - V1.6.0 (24/10/2026, audit C.3 étape 2, v2.57.0) : listes des séances : case « Non tenue, libère
'   le créneau » du statut (est_non_tenue).
'
' Dépendances :
' - Utilisé par GestionSeances pour les opérations sur seances
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QuerySeances

    ' Codes écrits dans le SQL : ceux de CodesSysteme (audit C.3), jamais une saisie — la
    ' chaîne est bâtie sur une constante du programme.
    Private Const STATUT_PAYE As String = CodesSysteme.StatutsPaiement.Paye

#Region "SELECT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectSeancesParDossier
    ' Version    : V1.0.0
    ' Date       : 14/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour charger la liste des séances d'un dossier (onglet Séances)
    '
    ' Rôle       :
    ' Retourne les séances rattachées à un dossier avec les libellés du type et du statut, ainsi
    ' que le montant déjà réglé (agrégé depuis lia_paiements_seances), triées par date décroissante.
    '
    ' Paramètres SQL :
    ' - @id_dossier : Identifiant du dossier
    '
    ' Utilisé par :
    ' - GestionSeances.GetSeancesParDossier
    '
    ' Remarques  :
    ' - Vue allégée (SeanceListeItem) : n'inclut pas les colonnes RTF notes/évolution
    ' - Réglé, facturable, reste dû et avoir viennent de la vue v_seances_solde (21/10/2026, audit B.4)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectSeancesParDossier As String
        Get
            Return "
SELECT
    s.id_seance,
    s.code_seance,
    s.id_dossier,
    s.id_prix_seance,
    ps.libelle_prix_seance,
    s.id_nature_seance,
    ns.libelle_nature_seance,
    lia.id_categorie_quota,
    cq.libelle_categorie AS libelle_categorie_quota,
    s.id_statut_seance,
    ss.code_statut_seance,
    ss.libelle_statut_seance,
    ss.est_du,
    s.id_seance_origine,
    so.code_seance AS code_seance_origine,
    s.date_seance,
    s.heure_debut,
    s.heure_fin,
    s.duree_minutes,
    s.tarif_seance,
    vs.montant_regle,
    vs.est_facturable,
    vs.reste_du,
    vs.montant_avoir,
    COALESCE(dom.soumis_quota, 0)       AS soumis_quota,
    COALESCE(dom.no_show_facturable, 1) AS no_show_facturable,
    CASE WHEN COALESCE(ss.consomme_quota, 0) = 1
              AND (d.date_cloture IS NULL OR s.date_seance <= d.date_cloture)
         THEN 1 ELSE 0 END             AS consomme_quota,
    COALESCE(ss.est_absence, 0)         AS est_absence,
    COALESCE(ss.est_non_tenue, 0)       AS est_non_tenue,
    s.notes_seance_txt,
    s.date_modification
FROM seances s
INNER JOIN v_seances_solde vs ON vs.id_seance = s.id_seance
LEFT JOIN ref_prix_seance ps ON ps.id_prix_seance = s.id_prix_seance
LEFT JOIN ref_natures_seance ns ON ns.id_nature_seance = s.id_nature_seance
LEFT JOIN ref_statuts_seance ss ON ss.id_statut_seance = s.id_statut_seance
LEFT JOIN seances so ON so.id_seance = s.id_seance_origine
LEFT JOIN dossiers d ON d.id_dossier = s.id_dossier
LEFT JOIN ref_domaines dom ON dom.id_domaine = d.id_domaine
LEFT JOIN lia_natures_seance_domaines lia ON lia.id_nature_seance = s.id_nature_seance
                                        AND lia.id_domaine = d.id_domaine
LEFT JOIN ref_categories_quota cq ON cq.id_categorie_quota = lia.id_categorie_quota
WHERE s.id_dossier = @id_dossier
ORDER BY s.date_seance DESC, s.id_seance DESC;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectSeancesPlanning (privée)
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Colonnes et jointures communes aux séances « tous dossiers confondus » (planning du jour,
    ' recherche des séances d'une période) : la séance, son solde, son patient, son dossier et son
    ' domaine. Chaque requête n'ajoute que son filtre et son ordre ; un seul mapping
    ' (GestionSeances.MapPlanningItem).
    ' -------------------------------------------------------------------------------------------------
    Private ReadOnly Property SelectSeancesPlanning As String
        Get
            Return "
SELECT
    s.id_seance,
    s.code_seance,
    s.id_dossier,
    s.id_prix_seance,
    ps.libelle_prix_seance,
    s.id_nature_seance,
    ns.libelle_nature_seance,
    s.id_statut_seance,
    ss.code_statut_seance,
    ss.libelle_statut_seance,
    ss.est_du,
    s.date_seance,
    s.heure_debut,
    s.heure_fin,
    s.duree_minutes,
    s.tarif_seance,
    vs.montant_regle,
    vs.est_facturable,
    vs.reste_du,
    vs.montant_avoir,
    s.notes_seance_txt,
    s.date_modification,
    d.code_dossier,
    p.id_patient,
    p.code_patient,
    p.nom AS nom_patient,
    p.prenom AS prenom_patient,
    dom.libelle_domaine,
    COALESCE(dom.soumis_quota, 0)       AS soumis_quota,
    COALESCE(dom.no_show_facturable, 1) AS no_show_facturable,
    COALESCE(ss.est_absence, 0)         AS est_absence,
    COALESCE(ss.est_non_tenue, 0)       AS est_non_tenue
FROM seances s
INNER JOIN v_seances_solde vs ON vs.id_seance = s.id_seance
LEFT JOIN ref_prix_seance ps ON ps.id_prix_seance = s.id_prix_seance
LEFT JOIN ref_natures_seance ns ON ns.id_nature_seance = s.id_nature_seance
LEFT JOIN ref_statuts_seance ss ON ss.id_statut_seance = s.id_statut_seance
LEFT JOIN dossiers d ON d.id_dossier = s.id_dossier
LEFT JOIN patients p ON p.id_patient = d.id_patient
LEFT JOIN ref_domaines dom ON dom.id_domaine = d.id_domaine"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectSeancesParDate
    ' Version    : V1.1.0
    ' Date       : 23/10/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour charger toutes les séances planifiées à une date donnée,
    ' tous dossiers/patients confondus (un seul thérapeute = un seul planning global).
    '
    ' Rôle       :
    ' Alimente la grille de consultation du planning du jour (PlanningJourViewer) et le contrôle
    ' métier de chevauchement de créneaux (GestionSeances.ChercherChevauchement).
    '
    ' Paramètres SQL :
    ' - @date_seance : Date des séances à charger (format Date, sans heure)
    '
    ' Utilisé par :
    ' - GestionSeances.GetSeancesParDate, LireSeancesParDate
    '
    ' Remarques  :
    ' - Inclut le nom/prénom du patient et le libellé du domaine du dossier pour l'affichage.
    ' - Ne filtre pas les statuts ici : le filtrage (ex. exclusion des séances annulées) est
    '   effectué côté métier (GestionSeances) selon le contexte d'utilisation.
    ' - V1.1.0 (23/10/2026, recherche transversale R2) : colonnes et jointures dans
    '   SelectSeancesPlanning, partagées avec SelectSeancesParPeriode ; rend aussi le code du dossier,
    '   l'identifiant et le code du patient.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectSeancesParDate As String
        Get
            Return SelectSeancesPlanning & "
WHERE s.date_seance = @date_seance
ORDER BY s.heure_debut ASC, s.id_seance ASC;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectSeancesParPeriode
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Séances d'une période (bornes incluses), tous dossiers et patients confondus, dans l'ordre du
    ' calendrier : recherche transversale R2 (ADR-52).
    '
    ' Paramètres SQL :
    ' - @du, @au   : bornes de la période (dates, incluses)
    ' - @limite    : nombre maximal de lignes rendues (l'appelant demande une ligne de plus que son
    '                plafond pour savoir si la liste est tronquée)
    '
    ' Utilisé par :
    ' - GestionSeances.GetSeancesParPeriode
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectSeancesParPeriode As String
        Get
            Return SelectSeancesPlanning & "
WHERE s.date_seance BETWEEN @du AND @au
ORDER BY s.date_seance ASC, s.heure_debut ASC, s.id_seance ASC
LIMIT @limite;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectSeanceById
    ' Version    : V1.0.0
    ' Date       : 14/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour charger une séance complète depuis son identifiant
    '
    ' Rôle       :
    ' Retourne toutes les colonnes d'une séance (identité + notes/évolution RTF/TXT) pour
    ' l'affichage/édition, enrichies des libellés type/statut.
    '
    ' Paramètres SQL :
    ' - @id_seance : Identifiant de la séance à charger
    '
    ' Utilisé par :
    ' - GestionSeances.GetSeance
    '
    ' Remarques  :
    ' - LIMIT 1 pour garantir un seul résultat
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectSeanceById As String
        Get
            Return "
SELECT
    s.id_seance,
    s.code_seance,
    s.id_dossier,
    s.id_prix_seance,
    ps.libelle_prix_seance,
    s.id_nature_seance,
    ns.libelle_nature_seance,
    s.id_statut_seance,
    ss.code_statut_seance,
    ss.libelle_statut_seance,
    s.date_seance,
    s.heure_debut,
    s.heure_fin,
    s.duree_minutes,
    s.tarif_seance,
    s.notes_seance_rtf,
    s.notes_seance_txt,
    s.evolution_rtf,
    s.evolution_txt,
    s.date_creation,
    s.date_modification
FROM seances s
LEFT JOIN ref_prix_seance ps ON ps.id_prix_seance = s.id_prix_seance
LEFT JOIN ref_natures_seance ns ON ns.id_nature_seance = s.id_nature_seance
LEFT JOIN ref_statuts_seance ss ON ss.id_statut_seance = s.id_statut_seance
WHERE s.id_seance = @id_seance
LIMIT 1;
"
        End Get
    End Property

#End Region

#Region "INSERT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : InsertSeance
    ' Version    : V1.0.0
    ' Date       : 14/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour insérer une nouvelle séance
    '
    ' Rôle       :
    ' Insère une nouvelle ligne dans seances ; id_seance et code_seance sont générés par la base
    ' (seq_seances + colonne GENERATED). Le statut initial est fourni par l'appelant (forcé Planifiée).
    '
    ' Paramètres SQL :
    ' - @id_dossier, @id_type_seance, @id_statut_seance, @date_seance, @heure_debut, @heure_fin,
    '   @duree_minutes, @tarif_seance
    '
    ' Utilisé par :
    ' - GestionSeances.CreateSeance
    '
    ' Remarques  :
    ' - id_seance et code_seance sont générés par la base : jamais fournis par l'application
    ' - Les notes/évolution ne sont pas renseignées à la création (restent NULL, éditées ensuite)
    ' - date_creation et date_modification sont gérées par la base (DEFAULT current_timestamp())
    ' - L'ID généré est récupéré séparément via SELECT LASTVAL(seq_seances)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property InsertSeance As String
        Get
            Return "
INSERT INTO seances (
    id_dossier,
    id_prix_seance,
    id_nature_seance,
    id_statut_seance,
    id_seance_origine,
    date_seance,
    heure_debut,
    heure_fin,
    duree_minutes,
    tarif_seance
) VALUES (
    @id_dossier,
    @id_prix_seance,
    @id_nature_seance,
    @id_statut_seance,
    @id_seance_origine,
    @date_seance,
    @heure_debut,
    @heure_fin,
    @duree_minutes,
    @tarif_seance
);
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateSeance
    ' Version    : V1.0.0
    ' Date       : 14/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour mettre à jour l'identité d'une séance existante
    '
    ' Rôle       :
    ' Met à jour les colonnes éditables d'identité (type, date, horaires, durée, tarif) d'une
    ' séance identifiée par @id_seance.
    '
    ' Paramètres SQL :
    ' - @id_type_seance, @date_seance, @heure_debut, @heure_fin, @duree_minutes, @tarif_seance, @id_seance
    '
    ' Utilisé par :
    ' - GestionSeances.UpdateSeance
    '
    ' Remarques  :
    ' - Ne modifie ni id_seance, ni code_seance, ni id_dossier
    ' - Le statut n'est pas modifié ici : il relève de UpdateStatutSeance
    ' - date_modification est également mise à jour par ON UPDATE de la table (doublon inoffensif)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateSeance As String
        Get
            Return "
UPDATE seances
SET
    id_prix_seance = @id_prix_seance,
    id_nature_seance = @id_nature_seance,
    date_seance = @date_seance,
    heure_debut = @heure_debut,
    heure_fin = @heure_fin,
    duree_minutes = @duree_minutes,
    tarif_seance = @tarif_seance,
    date_modification = current_timestamp()
WHERE id_seance = @id_seance;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateNotesSeance
    ' Version    : V1.0.0
    ' Date       : 14/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour mettre à jour de façon ciblée les notes et l'évolution d'une séance
    '
    ' Rôle       :
    ' Met à jour uniquement les colonnes notes/évolution (RTF + TXT) sans réécrire les autres
    ' colonnes (enregistrement localisé, pattern UpdateZoneDossier).
    '
    ' Paramètres SQL :
    ' - @notes_seance_rtf, @notes_seance_txt, @evolution_rtf, @evolution_txt, @id_seance
    '
    ' Utilisé par :
    ' - GestionSeances.UpdateNotesSeance
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateNotesSeance As String
        Get
            Return "
UPDATE seances
SET
    notes_seance_rtf = @notes_seance_rtf,
    notes_seance_txt = @notes_seance_txt,
    evolution_rtf = @evolution_rtf,
    evolution_txt = @evolution_txt,
    date_modification = current_timestamp()
WHERE id_seance = @id_seance;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateStatutSeance
    ' Version    : V1.0.0
    ' Date       : 14/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour changer le statut d'une séance
    '
    ' Rôle       :
    ' Met à jour le statut courant de la séance (ex. Planifiée → Réalisée / Annulée...) sans toucher
    ' aux autres colonnes.
    '
    ' Paramètres SQL :
    ' - @id_statut_seance : Identifiant du statut cible (résolu depuis ref_statuts_seance)
    ' - @id_seance        : Identifiant de la séance concernée
    '
    ' Utilisé par :
    ' - GestionSeances.ChangerStatutSeance
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateStatutSeance As String
        Get
            Return "
UPDATE seances
SET
    id_statut_seance = @id_statut_seance,
    date_modification = current_timestamp()
WHERE id_seance = @id_seance;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateStatutSeanceSiInchange
    ' Version    : V1.0.0
    ' Date       : 21/10/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Changement de statut conditionné au statut attendu
    '
    ' Rôle       :
    ' Change le statut d'une séance seulement si elle a encore le statut attendu. Aucune ligne
    ' modifiée = la séance a changé entre la lecture et l'écriture (autre fenêtre, autre poste).
    '
    ' Paramètres SQL :
    ' - @id_statut_seance  : Statut cible
    ' - @id_seance         : Séance concernée
    ' - @id_statut_attendu : Statut que la séance doit encore avoir
    '
    ' Utilisé par :
    ' - GestionSeances.ReporterSeance
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateStatutSeanceSiInchange As String
        Get
            Return "
UPDATE seances
SET
    id_statut_seance = @id_statut_seance,
    date_modification = current_timestamp()
WHERE id_seance = @id_seance
  AND id_statut_seance = @id_statut_attendu;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DeleteSeance
    ' Version    : V1.0.0
    ' Date       : 14/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour supprimer une séance.
    '
    ' Rôle       :
    ' Supprime définitivement une séance. Les affectations de paiements liées (lia_paiements_seances)
    ' sont supposées gérées par contrainte ON DELETE ou traitées en amont par la couche métier.
    '
    ' Paramètres SQL :
    ' - @id_seance : Identifiant de la séance à supprimer
    '
    ' Utilisé par :
    ' - GestionSeances.DeleteSeance
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property DeleteSeance As String
        Get
            Return "
DELETE FROM seances
WHERE id_seance = @id_seance;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectBlocagesSuppressionSeance
    ' Version    : V1.0.0
    ' Date       : 25/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Rôle       :
    ' Compte ce qui interdit la suppression d'une séance : paiements ayant été encaissés (date de
    ' paiement renseignée, même annulés ensuite) et séances issues d'un report de celle-ci.
    '
    ' Paramètres SQL :
    ' - @id_seance : Identifiant de la séance à supprimer
    '
    ' Utilisé par :
    ' - GestionSeances.DeleteSeance
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectBlocagesSuppressionSeance As String
        Get
            Return $"
SELECT
    (SELECT COUNT(*)
       FROM paiements p
       INNER JOIN lia_paiements_seances lps ON lps.id_paiement = p.id_paiement
      WHERE lps.id_seance = @id_seance
        AND (p.statut_paiement = '{STATUT_PAYE}' OR p.date_paiement IS NOT NULL)) AS nb_encaisses,
    (SELECT COUNT(*)
       FROM seances s
      WHERE s.id_seance_origine = @id_seance) AS nb_reports;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectPaiementsNonEncaissesParSeance
    ' Version    : V1.0.0
    ' Date       : 25/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Rôle       :
    ' Liste les paiements jamais encaissés (dû en attente ou dû annulé) d'une séance, supprimés avec
    ' elle car ils n'ont pas d'existence propre.
    '
    ' Utilisé par :
    ' - GestionSeances.DeleteSeance
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectPaiementsNonEncaissesParSeance As String
        Get
            Return $"
SELECT p.id_paiement
FROM paiements p
INNER JOIN lia_paiements_seances lps ON lps.id_paiement = p.id_paiement
WHERE lps.id_seance = @id_seance
  AND p.statut_paiement <> '{STATUT_PAYE}'
  AND p.date_paiement IS NULL;
"
        End Get
    End Property

#End Region

End Module
