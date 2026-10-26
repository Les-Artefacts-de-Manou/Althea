' -------------------------------------------------------------------------------------------------
' Module      : QueryPaiements
' Projet      : Althéa
' Version     : V1.12.1
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table paiements et à sa liaison lia_paiements_seances.
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL des paiements
' - Sélectionner la liste des paiements d'un dossier (via les séances affectées)
' - Sélectionner la liste des paiements affectés à une séance, ou d'une période (recherche)
' - Insérer un paiement, créer la liaison paiement↔séance, supprimer un paiement
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_paiement et code_paiement sont générés par la base : jamais fournis par l'application
' - En V1 (D-PAY-1), un paiement saisi est affecté à UNE séance (montant_affecte = paiements.montant).
'   L'utilisation d'un avoir diminue ensuite l'affectation source. Des réaffectations faites avant le
'   21/10/2026 ont pu répartir un paiement sur plusieurs séances (une ligne de liaison par séance).
'
' Historique  :
' - V1.1.0 (17/10/2026) : SelectAvoirsDisponiblesPatient retient aussi les séances reportées
'   (encaissement non réaffecté = avoir, comme partout ailleurs).
' - V1.2.0 (17/10/2026) : SelectAvoirsDisponiblesPatient retient aussi le trop-perçu d'une séance
'   encore due (montant baissé sous l'encaissé).
' - V1.3.0 (21/10/2026, audit B.1) : SelectPaiementsEncaissesParSeance rend la part encore affectée à
'   la séance ; SelectAffectationsParPaiement et SelectMontantUtiliseEnAvoir ajoutés ; une seule
'   ligne de dû par séance (SelectSyntheseDuSeance désigne la ligne gardée, UpdateDuSeance et
'   AnnulerAutresPaiementsEnAttenteParSeance remplacent les mises à jour « toutes lignes »
'   ReactiverPaiementsAnnulesParSeance et UpdateMontantPaiementsEnAttenteParSeance) ;
'   SelectIdsPaiementsEnAttenteParSeance, inutilisée, supprimée.
' - V1.4.0 (21/10/2026, audit B.2) : la réaffectation est remplacée par l'utilisation de l'avoir
'   (décision de Joëlle) ; SelectPaiementsEncaissesParSeance, SelectAffectationsParPaiement et
'   SelectResteDuSeance, qui ne servaient qu'à elle, sont supprimées.
' - V1.5.0 (21/10/2026, audit B.4) : reste dû et réglé lus dans la vue v_seances_solde
'   (SelectPaiementsParDossier, SelectPaiementsParSeance, SelectSeancesNonSoldeesParDossier,
'   SelectSyntheseDuSeance) ; une absence non facturable n'est plus proposée à l'encaissement et
'   une séance non due n'affiche plus de reste dû.
' - V1.6.0 (21/10/2026, audit B.5, migration v2.50.0) : le mode d'un paiement est relié au référentiel
'   par son identifiant (id_mode_paiement) ; les listes lisent son libellé et son caractère « avoir »
'   par jointure ; SelectEstReglementParAvoir ajoutée.
' - V1.7.0 (22/10/2026, audit B.5, migration v2.51.0) : l'annulation d'un encaissement est écrite
'   dans ses colonnes (date, utilisateur, motif) au lieu d'une mention ajoutée au commentaire ; les
'   listes les lisent, avec le nom de l'utilisateur par jointure ; SelectIdsSeancesParPaiement
'   (requête qui était écrite dans GestionPaiements).
' - V1.8.0 (22/10/2026, audit B.5, migration v2.52.0) : chaque utilisation d'un avoir est reliée à
'   l'affectation encaissée qui l'a financée (InsertUtilisationAvoir, SelectUtilisationsAvoirParDossier
'   / ParSeance) ; DiminuerAffectationAvoir n'écrit plus de mention dans le commentaire.
' - V1.9.0 (22/10/2026, audit C.2) : SelectEncaissementSeance (contrôle d'un encaissement par
'   GestionPaiements.CreatePaiement).
' - V1.10.0 (22/10/2026) : SelectSeancesNonSoldeesParDossier supprimée (liste de la fenêtre Paiement,
'   retirée avec le bouton « Nouveau paiement »).
' - V1.11.0 (22/10/2026) : SelectSeancesDuDesaligne (contrôle et réalignement des dus en attente,
'   État du système).
' - V1.12.0 (23/10/2026, recherche transversale R3, ADR-52) : SelectPaiementsBase (colonnes communes
'   des listes, + horaires et avoir de la séance, dossier, patient) ; SelectPaiementsParPeriode et
'   SelectUtilisationsAvoirParPeriode (paiements d'une période).
' - V1.12.1 (24/10/2026, audit C.3) : statuts de paiement écrits dans le SQL depuis
'   CodesSysteme.StatutsPaiement (STATUT_PAYE…), plus en clair.
'
' Dépendances :
' - Utilisé par GestionPaiements pour les opérations sur paiements / lia_paiements_seances
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryPaiements

    ' Codes écrits dans le SQL : ceux de CodesSysteme (audit C.3), jamais une saisie — la
    ' chaîne est bâtie sur une constante du programme.
    Private Const STATUT_PAYE As String = CodesSysteme.StatutsPaiement.Paye
    Private Const STATUT_EN_ATTENTE As String = CodesSysteme.StatutsPaiement.EnAttente
    Private Const STATUT_ANNULE As String = CodesSysteme.StatutsPaiement.Annule

#Region "SELECT"

    ' -------------------------------------------------------------------------------------------------
    ' Constante  : SelectPaiementsBase (privée)
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Colonnes et jointures communes des lignes de paiement (une ligne par affectation à une séance) :
    ' le paiement, sa séance, son solde, son mode, son annulation, son dossier et son patient. Les
    ' listes du dossier, d'une séance et de la recherche n'ajoutent que leur filtre et leur ordre ; un
    ' seul mapping (GestionPaiements.MapListeItem).
    '
    ' Remarques  :
    ' - Jointure via lia_paiements_seances puis seances (paiement toujours lié à une séance en V1).
    ' - La nature de la séance provient du référentiel ref_natures_seance : elle situe le paiement dans
    '   son contexte clinique sans qu'il faille ouvrir la séance.
    ' - reste_du_seance et avoir_seance viennent de la vue v_seances_solde (calculés, jamais stockés) :
    '   ils reflètent toujours l'état réel, y compris après des règlements partiels successifs.
    ' - Sorti le 23/10/2026 (recherche R3) de SelectPaiementsParDossier et SelectPaiementsParSeance,
    '   qui en étaient deux copies ; ajout des horaires et de l'avoir de la séance, du dossier et du
    '   patient.
    ' -------------------------------------------------------------------------------------------------
    Private Const SelectPaiementsBase As String = "
SELECT
    p.id_paiement,
    p.code_paiement,
    p.date_paiement,
    p.montant,
    p.id_mode_paiement,
    mp.libelle_mode_paiement AS mode_paiement,
    COALESCE(mp.est_avoir, 0) AS mode_est_avoir,
    p.reference_paiement,
    s.id_seance,
    s.code_seance,
    s.date_seance,
    s.heure_debut,
    s.heure_fin,
    s.tarif_seance,
    ss.est_du AS seance_est_due,
    ss.code_statut_seance,
    so.code_seance AS code_seance_origine,
    srep.code_seance AS code_seance_report,
    ns.libelle_nature_seance,
    lps.montant_affecte,
    p.statut_paiement,
    p.commentaire,
    p.date_annulation,
    p.motif_annulation,
    ua.nom_affichage AS auteur_annulation,
    p.date_modification,
    vs.reste_du AS reste_du_seance,
    vs.montant_avoir AS avoir_seance,
    d.id_dossier,
    d.code_dossier,
    pat.id_patient,
    pat.code_patient,
    pat.nom AS nom_patient,
    pat.prenom AS prenom_patient
FROM paiements p
INNER JOIN lia_paiements_seances lps ON lps.id_paiement = p.id_paiement
INNER JOIN seances s ON s.id_seance = lps.id_seance
INNER JOIN v_seances_solde vs ON vs.id_seance = s.id_seance
INNER JOIN ref_statuts_seance ss ON ss.id_statut_seance = s.id_statut_seance
INNER JOIN dossiers d ON d.id_dossier = s.id_dossier
INNER JOIN patients pat ON pat.id_patient = d.id_patient
LEFT JOIN ref_modes_paiement mp ON mp.id_mode_paiement = p.id_mode_paiement
LEFT JOIN sec_utilisateurs ua ON ua.id_utilisateur = p.id_utilisateur_annulation
LEFT JOIN seances so ON so.id_seance = s.id_seance_origine
LEFT JOIN seances srep ON srep.id_seance_origine = s.id_seance
LEFT JOIN ref_natures_seance ns ON ns.id_nature_seance = s.id_nature_seance
"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectPaiementsParDossier
    ' Version    : V1.1.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Paiements affectés aux séances d'un dossier (onglet Paiements), par date de paiement décroissante.
    '
    ' Paramètres SQL :
    ' - @id_dossier : Identifiant du dossier
    '
    ' Utilisé par :
    ' - GestionPaiements.GetPaiementsParDossier
    '
    ' Remarques  :
    ' - V1.1.0 (23/10/2026) : colonnes et jointures dans SelectPaiementsBase.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectPaiementsParDossier As String
        Get
            Return SelectPaiementsBase & "WHERE s.id_dossier = @id_dossier
ORDER BY p.date_paiement DESC, p.id_paiement DESC;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectPaiementsParSeance
    ' Version    : V1.1.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Paiements affectés à une séance, par date de paiement décroissante.
    '
    ' Paramètres SQL :
    ' - @id_seance : Identifiant de la séance
    '
    ' Utilisé par :
    ' - GestionPaiements.GetPaiementsParSeance
    '
    ' Remarques  :
    ' - V1.1.0 (23/10/2026) : colonnes et jointures dans SelectPaiementsBase.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectPaiementsParSeance As String
        Get
            Return SelectPaiementsBase & "WHERE lps.id_seance = @id_seance
ORDER BY p.date_paiement DESC, p.id_paiement DESC;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectPaiementsParPeriode
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Lignes de paiement d'une période, tous patients (recherche transversale R3, ADR-52) : la date
    ' d'un mouvement est celle de l'encaissement, ou celle de la séance pour un paiement encore
    ' attendu (PaiementListeItem.DateMouvement, même règle que le relevé du dossier).
    '
    ' Paramètres SQL :
    ' - @du, @au : bornes de la période (dates, incluses)
    ' - @limite  : nombre maximal de lignes (l'appelant en demande une de plus que son plafond)
    '
    ' Utilisé par :
    ' - GestionPaiements.GetPaiementsParPeriode
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectPaiementsParPeriode As String
        Get
            Return SelectPaiementsBase & "WHERE COALESCE(p.date_paiement, s.date_seance) BETWEEN @du AND @au
ORDER BY COALESCE(p.date_paiement, s.date_seance) DESC, p.id_paiement DESC
LIMIT @limite;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriétés : SelectUtilisationsAvoirParDossier / SelectUtilisationsAvoirParSeance
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Utilisations d'avoir (lia_paiements_avoirs, v2.52.0) qui touchent un dossier ou une séance, d'un
    ' côté ou de l'autre : encaissement source sur ce dossier / cette séance, ou règlement par avoir
    ' fait sur ce dossier / cette séance (un avoir peut servir dans un autre dossier du patient).
    '
    ' Paramètres SQL :
    ' - @id_dossier (ParDossier) ou @id_seance (ParSeance)
    '
    ' Utilisé par :
    ' - GestionPaiements.GetPaiementsParDossier / GetPaiementsParSeance (AttacherUtilisationsAvoir)
    '
    ' Remarques  :
    ' - Un règlement par avoir n'a qu'une liaison (sa séance réglée) : la jointure lr ne double rien.
    ' - auteur : nom affiché, lu ici ; seul l'identifiant est enregistré.
    ' -------------------------------------------------------------------------------------------------
    Private Const SelectUtilisationsAvoirBase As String = "
SELECT
    la.id_paiement_avoir,
    src.id_paiement AS id_paiement_source,
    ps.code_paiement AS code_paiement_source,
    src.id_seance AS id_seance_source,
    ss.code_seance AS code_seance_source,
    sr.code_seance AS code_seance_reglee,
    la.montant,
    la.date_utilisation,
    u.nom_affichage AS auteur
FROM lia_paiements_avoirs la
INNER JOIN lia_paiements_seances src ON src.id_lia_paiements_seances = la.id_lia_paiements_seances_source
INNER JOIN paiements ps ON ps.id_paiement = src.id_paiement
INNER JOIN seances ss ON ss.id_seance = src.id_seance
INNER JOIN lia_paiements_seances lr ON lr.id_paiement = la.id_paiement_avoir
INNER JOIN seances sr ON sr.id_seance = lr.id_seance
LEFT JOIN sec_utilisateurs u ON u.id_utilisateur = la.id_utilisateur
"

    Public ReadOnly Property SelectUtilisationsAvoirParDossier As String
        Get
            Return SelectUtilisationsAvoirBase & "WHERE ss.id_dossier = @id_dossier OR sr.id_dossier = @id_dossier
ORDER BY la.date_utilisation, la.id_lia_paiements_avoirs;
"
        End Get
    End Property

    ' Utilisations d'avoir dont l'encaissement source ou le règlement par avoir tombe dans la période
    ' (même date de mouvement que SelectPaiementsParPeriode ; recherche R3).
    Public ReadOnly Property SelectUtilisationsAvoirParPeriode As String
        Get
            Return SelectUtilisationsAvoirBase & "WHERE COALESCE(ps.date_paiement, ss.date_seance) BETWEEN @du AND @au
   OR EXISTS (SELECT 1 FROM paiements pr
              WHERE pr.id_paiement = la.id_paiement_avoir
                AND COALESCE(pr.date_paiement, sr.date_seance) BETWEEN @du AND @au)
ORDER BY la.date_utilisation, la.id_lia_paiements_avoirs;
"
        End Get
    End Property

    Public ReadOnly Property SelectUtilisationsAvoirParSeance As String
        Get
            Return SelectUtilisationsAvoirBase & "WHERE src.id_seance = @id_seance OR lr.id_seance = @id_seance
ORDER BY la.date_utilisation, la.id_lia_paiements_avoirs;
"
        End Get
    End Property

#End Region

#Region "INSERT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : InsertPaiement
    ' Version    : V1.0.0
    ' Date       : 15/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour insérer un nouveau paiement
    '
    ' Rôle       :
    ' Insère une nouvelle ligne dans paiements ; id_paiement et code_paiement sont générés par la
    ' base (seq_paiements + colonne GENERATED).
    '
    ' Paramètres SQL :
    ' - @date_paiement, @montant, @id_mode_paiement, @reference_paiement, @commentaire
    '
    ' Utilisé par :
    ' - GestionPaiements.CreatePaiement
    '
    ' Remarques  :
    ' - L'ID généré est récupéré séparément via SELECT LASTVAL(seq_paiements) sur la même connexion
    ' - date_creation et date_modification sont gérées par la base (DEFAULT current_timestamp())
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property InsertPaiement As String
        Get
            Return $"
INSERT INTO paiements (
    date_paiement,
    montant,
    id_mode_paiement,
    reference_paiement,
    commentaire,
    statut_paiement
) VALUES (
    @date_paiement,
    @montant,
    @id_mode_paiement,
    @reference_paiement,
    @commentaire,
    '{STATUT_PAYE}'
);
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : InsertPaiementEnAttente
    ' Version    : V1.0.0
    ' Date       : 17/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour créer un paiement « en attente » (dû) associé à une séance
    '
    ' Rôle       :
    ' Insère une ligne de paiement non encaissée (date_paiement NULL, statut_paiement = 'EN_ATTENTE')
    ' dont le montant correspond au tarif de la séance à régler.
    '
    ' Paramètres SQL :
    ' - @montant : Montant dû (tarif de la séance)
    '
    ' Utilisé par :
    ' - GestionPaiements.CreatePaiementEnAttente (à la création d'une séance)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property InsertPaiementEnAttente As String
        Get
            Return $"
INSERT INTO paiements (
    date_paiement,
    montant,
    statut_paiement
) VALUES (
    NULL,
    @montant,
    '{STATUT_EN_ATTENTE}'
);
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : InsertLiaisonPaiementSeance
    ' Version    : V1.0.0
    ' Date       : 15/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour affecter un paiement à une séance (liaison N-N)
    '
    ' Rôle       :
    ' Insère la ligne de liaison entre un paiement et une séance dans lia_paiements_seances avec
    ' le montant affecté.
    '
    ' Paramètres SQL :
    ' - @id_paiement, @id_seance, @montant_affecte
    '
    ' Utilisé par :
    ' - GestionPaiements.CreatePaiement (dans la même transaction que InsertPaiement)
    '
    ' Remarques  :
    ' - En V1, un seul appel par paiement (1 paiement ↔ 1 séance, D-PAY-1)
    ' - L'unicité (id_paiement, id_seance) est garantie par uq_lia_paiement_seance
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property InsertLiaisonPaiementSeance As String
        Get
            Return "
INSERT INTO lia_paiements_seances (
    id_paiement,
    id_seance,
    montant_affecte
) VALUES (
    @id_paiement,
    @id_seance,
    @montant_affecte
);
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : InsertUtilisationAvoir
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Relie un règlement par avoir à l'affectation encaissée qu'il a diminuée (lia_paiements_avoirs,
    ' v2.52.0), avec le montant pris et l'utilisateur ; la date est posée par la base.
    '
    ' Paramètres SQL :
    ' - @id_paiement_avoir, @id_lia_source, @montant, @id_utilisateur
    '
    ' Utilisé par :
    ' - GestionPaiements.ImputerAvoir (même transaction que la diminution et le règlement)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property InsertUtilisationAvoir As String
        Get
            Return "
INSERT INTO lia_paiements_avoirs (id_paiement_avoir, id_lia_paiements_seances_source, montant, id_utilisateur)
VALUES (@id_paiement_avoir, @id_lia_source, @montant, @id_utilisateur);
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : AnnulerPaiementsNonEncaissesParSeance
    ' Version    : V1.0.0
    ' Date       : 17/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour annuler les paiements non encaissés d'une séance
    '
    ' Rôle       :
    ' Bascule en statut 'ANNULE' avec un montant nul tous les paiements 'EN_ATTENTE' rattachés à une
    ' séance devenue non due (maladie, report, annulation praticien...). La ligne est conservée pour
    ' garder la traçabilité, contrairement à une suppression.
    '
    ' Paramètres SQL :
    ' - @id_seance : Identifiant de la séance concernée
    '
    ' Utilisé par :
    ' - GestionPaiements.AnnulerPaiementsNonEncaissesParSeance
    '
    ' Remarques  :
    ' - Les paiements déjà encaissés (statut 'PAYE') ne sont jamais touchés.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property AnnulerPaiementsNonEncaissesParSeance As String
        Get
            Return $"
UPDATE paiements p
INNER JOIN lia_paiements_seances lps ON lps.id_paiement = p.id_paiement
SET p.statut_paiement = '{STATUT_ANNULE}',
    p.montant         = 0,
    lps.montant_affecte = 0
WHERE lps.id_seance = @id_seance
  AND p.statut_paiement = '{STATUT_EN_ATTENTE}';
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : AnnulerAutresPaiementsEnAttenteParSeance
    ' Version    : V1.0.0
    ' Date       : 21/10/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Annule les lignes de dû en double d'une séance
    '
    ' Rôle       :
    ' Une séance n'a qu'une ligne de dû. Annule (montant 0, trace conservée) toutes ses lignes
    ' 'EN_ATTENTE' sauf celle qui est gardée : des doublons ont pu naître avant la règle « une seule
    ' ligne » (réactivation de tous les dûs annulés à chaque changement de statut).
    '
    ' Paramètres SQL :
    ' - @id_seance         : Identifiant de la séance concernée
    ' - @id_paiement_garde : Ligne de dû conservée
    '
    ' Utilisé par :
    ' - GestionPaiements.ResynchroniserDuSeance (même transaction que UpdateDuSeance)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property AnnulerAutresPaiementsEnAttenteParSeance As String
        Get
            Return $"
UPDATE paiements p
INNER JOIN lia_paiements_seances lps ON lps.id_paiement = p.id_paiement
SET p.statut_paiement = '{STATUT_ANNULE}',
    p.montant         = 0,
    lps.montant_affecte = 0
WHERE lps.id_seance = @id_seance
  AND p.statut_paiement = '{STATUT_EN_ATTENTE}'
  AND p.id_paiement <> @id_paiement_garde;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : AnnulerPaiementEncaisse
    ' Version    : V2.0.0
    ' Date       : 22/10/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL d'annulation tracée d'un paiement encaissé
    '
    ' Rôle       :
    ' Bascule un paiement 'PAYE' en 'ANNULE' sans le supprimer : la date, le montant et le mode
    ' d'origine sont conservés ; la date, l'utilisateur et le motif de l'annulation sont écrits dans
    ' leurs colonnes et l'affectation est remise à zéro.
    '
    ' Paramètres SQL :
    ' - @id_paiement    : Identifiant du paiement à annuler
    ' - @id_utilisateur : Utilisateur qui annule (sec_utilisateurs)
    ' - @motif          : Motif saisi
    '
    ' Utilisé par :
    ' - GestionPaiements.AnnulerPaiementEncaisse
    '
    ' Remarques  :
    ' - Aucune ligne modifiée si le paiement n'est plus encaissé (écran périmé) : refus.
    ' - V2.0.0 (22/10/2026, audit B.5, v2.51.0) : la mention « [Annulé le … par … : motif] » n'est
    '   plus ajoutée au commentaire.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property AnnulerPaiementEncaisse As String
        Get
            Return $"
UPDATE paiements p
LEFT JOIN lia_paiements_seances lps ON lps.id_paiement = p.id_paiement
SET p.statut_paiement = '{STATUT_ANNULE}',
    p.date_annulation = NOW(),
    p.id_utilisateur_annulation = @id_utilisateur,
    p.motif_annulation = @motif,
    lps.montant_affecte = 0
WHERE p.id_paiement = @id_paiement
  AND p.statut_paiement = '{STATUT_PAYE}';
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateDuSeance
    ' Version    : V1.0.0
    ' Date       : 21/10/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Met LA ligne de dû d'une séance au reste dû
    '
    ' Rôle       :
    ' Place en 'EN_ATTENTE', au montant fourni, la ligne de dû désignée : soit la ligne en attente
    ' existante (réalignement après un changement de tarif ou un encaissement), soit un dû annulé
    ' sans date (séance qui redevient due). Une seule ligne est touchée.
    '
    ' Paramètres SQL :
    ' - @id_paiement : Ligne de dû gardée (désignée par SelectSyntheseDuSeance)
    ' - @id_seance   : Séance concernée (garde-fou : la ligne doit lui appartenir)
    ' - @montant     : Reste dû
    '
    ' Utilisé par :
    ' - GestionPaiements.ResynchroniserDuSeance
    '
    ' Remarques  :
    ' - Remplace UpdateMontantPaiementsEnAttenteParSeance et ReactiverPaiementsAnnulesParSeance, qui
    '   touchaient toutes les lignes de la séance (dû multiplié après plusieurs changements de statut).
    ' - Un encaissement (PAYE) ou une annulation tracée (ANNULE avec date) n'est jamais touché.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateDuSeance As String
        Get
            Return $"
UPDATE paiements p
INNER JOIN lia_paiements_seances lps ON lps.id_paiement = p.id_paiement
SET p.statut_paiement = '{STATUT_EN_ATTENTE}',
    p.montant         = @montant,
    lps.montant_affecte = @montant
WHERE p.id_paiement = @id_paiement
  AND lps.id_seance = @id_seance
  AND (p.statut_paiement = '{STATUT_EN_ATTENTE}'
       OR (p.statut_paiement = '{STATUT_ANNULE}' AND p.date_paiement IS NULL));
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectSeancesDuDesaligne
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Rôle       :
    ' Séances dont le montant « en attente » (somme des lignes EN_ATTENTE) diffère du reste dû de la
    ' vue v_seances_solde : la règle unique veut une seule ligne en attente égale au reste dû (0 s'il
    ' n'y a plus rien à payer). Ces écarts viennent de données antérieures aux corrections du lot B
    ' (report ou réaffectation d'avant le 21/10/2026) que rien n'a resynchronisées depuis.
    '
    ' Utilisé par :
    ' - GestionPaiements.CompterDusDesalignes, RealignerDusDesalignes (État du système)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectSeancesDuDesaligne As String
        Get
            Return $"
SELECT s.id_seance
FROM seances s
INNER JOIN v_seances_solde vs ON vs.id_seance = s.id_seance
LEFT JOIN lia_paiements_seances lps ON lps.id_seance = s.id_seance
LEFT JOIN paiements p ON p.id_paiement = lps.id_paiement
GROUP BY s.id_seance, vs.reste_du
HAVING COALESCE(SUM(CASE WHEN p.statut_paiement = '{STATUT_EN_ATTENTE}' THEN lps.montant_affecte END), 0) <> vs.reste_du
ORDER BY s.id_seance;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectEncaissementSeance
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Rôle       :
    ' Ce qu'il faut savoir d'une séance pour juger un encaissement : code du statut (une séance
    ' reportée ne s'encaisse pas), statut dû et reste dû.
    '
    ' Paramètres SQL :
    ' - @id_seance : Séance à encaisser
    '
    ' Utilisé par :
    ' - GestionPaiements.CreatePaiement (LireResteEncaissable, dans sa transaction)
    '
    ' Remarques  :
    ' - est_du et reste_du viennent de la vue v_seances_solde (une seule définition, ADR-48).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectEncaissementSeance As String
        Get
            Return "
SELECT ss.code_statut_seance,
       vs.est_du,
       vs.reste_du
FROM seances s
INNER JOIN ref_statuts_seance ss ON ss.id_statut_seance = s.id_statut_seance
INNER JOIN v_seances_solde vs ON vs.id_seance = s.id_seance
WHERE s.id_seance = @id_seance;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectSyntheseDuSeance
    ' Version    : V1.2.0
    ' Date       : 21/10/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Synthèse financière d'une séance (tarif, statut dû, encaissé, lignes de dû)
    '
    ' Rôle       :
    ' Fournit en une requête les éléments nécessaires pour réaligner la ligne « en attente » d'une
    ' séance sur son reste dû réel (tarif moins encaissements PAYE).
    '
    ' Paramètres SQL :
    ' - @id_seance : Identifiant de la séance
    '
    ' Utilisé par :
    ' - GestionPaiements.ResynchroniserDuSeance
    '
    ' Remarques  :
    ' - V1.1.0 : désigne la ligne de dû à garder, la plus récente (id_en_attente, à défaut
    '   id_du_annule) ; nb_dus_annules, devenu inutile, est retiré.
    ' - V1.2.0 (21/10/2026, audit B.4) : facturable et reste dû lus dans v_seances_solde ; une absence
    '   non facturable n'a plus de dû en attente (est_du seul la comptait).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectSyntheseDuSeance As String
        Get
            Return $"
SELECT s.tarif_seance,
       vs.est_facturable,
       vs.montant_regle AS montant_encaisse,
       vs.reste_du,
       (
           SELECT COUNT(*)
           FROM lia_paiements_seances lps
           INNER JOIN paiements p ON p.id_paiement = lps.id_paiement
           WHERE lps.id_seance = s.id_seance
             AND p.statut_paiement = '{STATUT_EN_ATTENTE}'
       ) AS nb_en_attente,
       (
           SELECT MAX(p.id_paiement)
           FROM lia_paiements_seances lps
           INNER JOIN paiements p ON p.id_paiement = lps.id_paiement
           WHERE lps.id_seance = s.id_seance
             AND p.statut_paiement = '{STATUT_EN_ATTENTE}'
       ) AS id_en_attente,
       (
           SELECT MAX(p.id_paiement)
           FROM lia_paiements_seances lps
           INNER JOIN paiements p ON p.id_paiement = lps.id_paiement
           WHERE lps.id_seance = s.id_seance
             AND p.statut_paiement = '{STATUT_ANNULE}'
             AND p.date_paiement IS NULL
       ) AS id_du_annule
FROM seances s
INNER JOIN v_seances_solde vs ON vs.id_seance = s.id_seance
WHERE s.id_seance = @id_seance;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectIdsSeancesParPaiement
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Séances servies par un paiement (une seule en V1 ; plusieurs pour un paiement réparti par une
    ' réaffectation d'avant le 21/10/2026).
    '
    ' Paramètres SQL :
    ' - @id_paiement : Paiement concerné
    '
    ' Utilisé par :
    ' - GestionPaiements.AnnulerPaiementEncaisse (dû des séances réaligné après l'annulation)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectIdsSeancesParPaiement As String
        Get
            Return "
SELECT lps.id_seance
FROM lia_paiements_seances lps
WHERE lps.id_paiement = @id_paiement;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectEstReglementParAvoir
    ' Version    : V1.0.0
    ' Date       : 21/10/2026
    '
    ' Rôle       :
    ' Indique (1 / 0) si un paiement règle sa séance avec un avoir : son mode est marqué est_avoir
    ' dans le référentiel, actif ou non (un mode « Avoir » désactivé ou renommé après coup ne doit
    ' pas rendre ses règlements annulables).
    '
    ' Paramètres SQL :
    ' - @id_paiement : Paiement concerné
    '
    ' Utilisé par :
    ' - GestionPaiements.AnnulerPaiementEncaisse
    '
    ' Remarques  :
    ' - Remplace la comparaison du libellé (audit B.5, 21/10/2026) ; aucune ligne si le paiement
    '   n'existe pas.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectEstReglementParAvoir As String
        Get
            Return "
SELECT COALESCE(mp.est_avoir, 0) AS est_avoir
FROM paiements p
LEFT JOIN ref_modes_paiement mp ON mp.id_mode_paiement = p.id_mode_paiement
WHERE p.id_paiement = @id_paiement;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectMontantUtiliseEnAvoir
    ' Version    : V1.0.0
    ' Date       : 21/10/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Part d'un encaissement déjà consommée comme avoir
    '
    ' Rôle       :
    ' Un encaissement est affecté en totalité à la saisie ; seule l'utilisation d'un avoir diminue
    ' ensuite ses affectations (DiminuerAffectationAvoir). L'écart entre le montant encaissé et la
    ' somme de ses affectations est donc ce qui a servi à régler d'autres séances.
    '
    ' Paramètres SQL :
    ' - @id_paiement : Paiement concerné
    '
    ' Utilisé par :
    ' - GestionPaiements.GetMontantUtiliseEnAvoir
    '
    ' Remarques  :
    ' - Aucune ligne si le paiement n'est pas encaissé (PAYE).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectMontantUtiliseEnAvoir As String
        Get
            Return $"
SELECT p.montant - COALESCE(SUM(lps.montant_affecte), 0) AS montant_utilise
FROM paiements p
LEFT JOIN lia_paiements_seances lps ON lps.id_paiement = p.id_paiement
WHERE p.id_paiement = @id_paiement
  AND p.statut_paiement = '{STATUT_PAYE}'
GROUP BY p.id_paiement, p.montant;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectAvoirsDisponiblesPatient
    ' Version    : V1.2.0
    ' Date       : 17/10/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Sources d'avoir du patient propriétaire d'une séance (tous dossiers confondus)
    '
    ' Rôle       :
    ' Liste les sources d'avoir, de la séance la plus ancienne à la plus récente : les affectations
    ' encaissées (PAYE) portées par des séances non dues, et le trop-perçu des séances encore dues
    ' (réglé > montant). Même définition que patients.montant_avoir (QueryPatients) et que
    ' SeanceListeItem.AvoirSeance.
    '
    ' Remarques  :
    ' - V1.1.0 : les séances reportées ne sont plus exclues. Leur crédit n'est déplacé que si
    '   l'utilisateur valide la réaffectation ; sinon il reste sur la séance reportée et l'application
    '   annonce qu'il est « conservé en avoir » — il doit donc pouvoir être réutilisé.
    ' - V1.2.0 : séance due avec trop-perçu → une seule source, sa dernière affectation encaissée,
    '   plafonnée au trop-perçu (colonne montant_affecte = montant disponible). Diminuer cette
    '   affectation ne peut donc jamais entamer ce que la séance doit. Si le trop-perçu dépasse cette
    '   affectation (plusieurs petits paiements), le solde est proposé à l'utilisation suivante.
    '
    ' Paramètres SQL :
    ' - @id_seance : Séance cible de l'encaissement (sert à retrouver le patient ; elle est exclue)
    '
    ' Utilisé par :
    ' - GestionPaiements.GetAvoirsDisponiblesPourSeance
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectAvoirsDisponiblesPatient As String
        Get
            Return $"
SELECT lps.id_lia_paiements_seances,
       lps.id_paiement,
       p.code_paiement,
       lps.id_seance,
       s.code_seance,
       CASE WHEN ss.est_du = 0 THEN lps.montant_affecte
            ELSE LEAST(lps.montant_affecte, r.regle - s.tarif_seance)
       END AS montant_affecte
FROM lia_paiements_seances lps
INNER JOIN paiements p ON p.id_paiement = lps.id_paiement
INNER JOIN seances s ON s.id_seance = lps.id_seance
INNER JOIN dossiers d ON d.id_dossier = s.id_dossier
INNER JOIN ref_statuts_seance ss ON ss.id_statut_seance = s.id_statut_seance
INNER JOIN (
    SELECT lr.id_seance,
           SUM(lr.montant_affecte) AS regle,
           MAX(lr.id_lia_paiements_seances) AS id_lia_derniere
    FROM lia_paiements_seances lr
    INNER JOIN paiements pr ON pr.id_paiement = lr.id_paiement
    WHERE pr.statut_paiement = '{STATUT_PAYE}'
      AND lr.montant_affecte > 0
    GROUP BY lr.id_seance
) r ON r.id_seance = s.id_seance
WHERE d.id_patient = (
          SELECT d2.id_patient
          FROM seances s2
          INNER JOIN dossiers d2 ON d2.id_dossier = s2.id_dossier
          WHERE s2.id_seance = @id_seance
      )
  AND lps.id_seance <> @id_seance
  AND p.statut_paiement = '{STATUT_PAYE}'
  AND lps.montant_affecte > 0
  AND (ss.est_du = 0
       OR (r.regle > s.tarif_seance AND lps.id_lia_paiements_seances = r.id_lia_derniere))
ORDER BY s.date_seance, lps.id_lia_paiements_seances;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DiminuerAffectationAvoir
    ' Version    : V2.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Diminue l'affectation source d'un avoir consommé. Le garde-fou montant_affecte >= @montant évite
    ' toute consommation double.
    '
    ' Paramètres SQL :
    ' - @id_lia, @montant
    '
    ' Remarques  :
    ' - V2.0.0 (22/10/2026, audit B.5, v2.52.0) : l'utilisation n'est plus tracée dans le commentaire
    '   du paiement d'origine ; elle est enregistrée dans lia_paiements_avoirs (InsertUtilisationAvoir).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property DiminuerAffectationAvoir As String
        Get
            Return $"
UPDATE lia_paiements_seances lps
INNER JOIN paiements p ON p.id_paiement = lps.id_paiement
SET lps.montant_affecte = lps.montant_affecte - @montant
WHERE lps.id_lia_paiements_seances = @id_lia
  AND p.statut_paiement = '{STATUT_PAYE}'
  AND lps.montant_affecte >= @montant;
"
        End Get
    End Property

#End Region

#Region "DELETE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DeleteLiaisonsParPaiement
    ' Version    : V1.0.0
    ' Date       : 15/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour supprimer les liaisons d'un paiement
    '
    ' Rôle       :
    ' Supprime toutes les lignes de lia_paiements_seances rattachées à un paiement (préalable à
    ' la suppression du paiement lui-même, contrainte FK).
    '
    ' Paramètres SQL :
    ' - @id_paiement : Identifiant du paiement
    '
    ' Utilisé par :
    ' - GestionPaiements.DeletePaiement (dans la même transaction que DeletePaiement)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property DeleteLiaisonsParPaiement As String
        Get
            Return "
DELETE FROM lia_paiements_seances
WHERE id_paiement = @id_paiement;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DeletePaiement
    ' Version    : V1.0.0
    ' Date       : 15/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour supprimer un paiement
    '
    ' Rôle       :
    ' Supprime définitivement un paiement (après suppression de ses liaisons).
    '
    ' Paramètres SQL :
    ' - @id_paiement : Identifiant du paiement à supprimer
    '
    ' Utilisé par :
    ' - GestionPaiements.DeletePaiement
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property DeletePaiement As String
        Get
            Return "
DELETE FROM paiements
WHERE id_paiement = @id_paiement;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectEtatPaiementsParSeance
    ' Version    : V1.0.0
    ' Date       : 17/09/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Synthèse du nombre de paiements par état pour une séance
    '
    ' Rôle       :
    ' Retourne, pour une séance, le nombre de paiements encaissés, en attente et annulés. Permet à la
    ' couche métier de décider s'il faut créer, réactiver ou annuler le dû sans charger les lignes.
    '
    ' Paramètres SQL :
    ' - @id_seance : Identifiant de la séance concernée
    '
    ' Utilisé par :
    ' - GestionPaiements.GetEtatPaiementsParSeance
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectEtatPaiementsParSeance As String
        Get
            Return $"
SELECT
    SUM(CASE WHEN p.statut_paiement = '{STATUT_PAYE}'       THEN 1 ELSE 0 END) AS nb_payes,
    SUM(CASE WHEN p.statut_paiement = '{STATUT_EN_ATTENTE}' THEN 1 ELSE 0 END) AS nb_en_attente,
    SUM(CASE WHEN p.statut_paiement = '{STATUT_ANNULE}'     THEN 1 ELSE 0 END) AS nb_annules
FROM paiements p
INNER JOIN lia_paiements_seances lps ON lps.id_paiement = p.id_paiement
WHERE lps.id_seance = @id_seance;
"
        End Get
    End Property

#End Region

End Module
