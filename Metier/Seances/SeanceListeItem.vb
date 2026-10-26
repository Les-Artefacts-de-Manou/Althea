' -------------------------------------------------------------------------------------------------
' Classe      : SeanceListeItem
' Projet      : Althéa
' Version     : V1.11.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Modèle de présentation léger pour la liste des séances d'un dossier (onglet Séances
' de UC_DossierFiche).
'
' Responsabilités :
' - Porter les données minimales d'une séance pour l'affichage en grille
' - Exposer les libellés joints (type, statut) pour un affichage direct
' - Fournir un aperçu des notes (texte brut) et le tarif figé
'
' Remarques   :
' - Cette classe ne contient aucun accès base de données.
' - Elle est produite par GestionSeances à partir de la requête SelectSeancesParDossier.
' - Volontairement distincte de Seance (vue allégée, sans les notes/évolution RTF).
' - Le champ MontantRegle (somme des affectations de paiements) est alimenté par jointure
'   sur lia_paiements_seances : il permet d'afficher le reste dû par séance (suivi financier).
' - V1.1.0 (17/10/2026, grille des séances lisible) : familles d'affichage du statut et du paiement,
'   textes des pastilles, solde et mention de report ; EtatPaiement et AvoirAffichage, remplacés,
'   sont retirés.
' - V1.1.1 (17/10/2026) : MontantCourt partagée (Friend) avec PaiementListeItem ; la famille de
'   paiement sert aussi à la pastille « État » de la grille des paiements.
' - V1.2.0 (17/10/2026, décision de Joëlle « A ») : le trop-perçu d'une séance encore due (montant
'   baissé sous l'encaissé) est un avoir (AvoirSeance) ; pastille « Réglée · avoir 10 € ».
' - V1.2.1 (17/10/2026) : LibelleReaffectation commence par le code de la séance.
' - V1.2.2 (20/10/2026) : codes des statuts de séance lus dans GestionSeances.CODE_STATUT_* (une seule définition ; audit B9).
' - V1.3.0 (21/10/2026) : la mention d'une séance reportée donne aussi le code de la remplaçante
'   (« → 31/10 · S000045 », demande de Joëlle) : CodeSeanceReport.
' - V1.4.0 (21/10/2026) : LibelleReaffectation devient LibelleChoixSeance (choix de la séance à
'   régler avec un avoir) et montre le reste dû.
' - V1.5.0 (21/10/2026, audit B.3, ADR-47, décision de Joëlle) : une séance planifiée dont l'heure est
'   passée n'est plus présumée réalisée : EstAConfirmer, famille AConfirmer, pastille « À confirmer » ;
'   StatutEffectifCode supprimée.
' - V1.6.0 (21/10/2026, audit B.4) : ResteDu, AvoirSeance et le caractère facturable viennent de la
'   vue v_seances_solde (une seule définition, en base) au lieu d'être recalculés ici.
' - V1.7.0 (22/10/2026, audit B.6, v2.53.0) : EstNoShow lit la case « Absence non excusée » du statut
'   (StatutEstAbsence).
' - V1.8.0 (22/10/2026, audit C.2) : EstReportable, EstSupprimable, EstEncaissable et
'   RaisonNonEncaissable, sorties de UC_DossierFiche ; elles lisent les règles de GestionSeances, que
'   les écritures appliquent aussi.
' - V1.9.0 (22/10/2026, décision de Joëlle) : Solde supprimée avec la colonne de l'onglet Séances
'   (elle répétait la pastille « Paiement »).
' - V1.10.0 (23/10/2026, recherche R3) : EstMomentPasse (règle de EstPassee, partagée avec les lignes
'   de paiement de la recherche).
' - V1.10.1 (24/10/2026, audit C.3) : codes de statut lus dans CodesSysteme.StatutsSeance.
' - V1.11.0 (24/10/2026, audit C.3 étape 2, v2.57.0) : StatutNonTenu (case « Non tenue, libère le
'   créneau » du statut).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class SeanceListeItem

#Region "Propriétés"

    ' Identifiant unique de la séance (clé primaire)
    Public Property IdSeance As Long

    ' Code technique de la séance (ex. 'SE000012')
    Public Property CodeSeance As String

    ' Identifiant du dossier auquel la séance est rattachée
    Public Property IdDossier As Long

    ' Identifiant du tarif de la séance
    Public Property IdPrixSeance As Long

    ' Libellé du tarif de la séance (issu de la jointure ref_prix_seance)
    Public Property LibellePrixSeance As String

    ' Identifiant de la nature de la séance
    Public Property IdNatureSeance As Long

    ' Libellé de la nature de la séance (issu de la jointure ref_natures_seance)
    Public Property LibelleNatureSeance As String

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : IdCategorieQuota / LibelleCategorieQuota
    ' Version    : V2.0.0
    ' Date       : 20/09/2026
    '
    ' Rôle       :
    ' Catégorie de quota consommée par la nature de la séance (0 / vide si hors quota).
    '
    ' Remarques  :
    ' - Alimenté depuis lia_natures_seance_domaines.id_categorie_quota (domaine du dossier)
    '   et ref_categories_quota.
    ' -------------------------------------------------------------------------------------------------
    Public Property IdCategorieQuota As ULong

    Public Property LibelleCategorieQuota As String

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : RangQuotaInami
    ' Version    : V1.0.0
    ' Date       : 19/09/2026
    '
    ' Rôle       :
    ' Numéro d'ordre de la séance dans le quota INAMI de sa catégorie (1re, 2e, 3e séance de soutien
    ' ou de traitement), sur la période glissante d'un an en cours.
    '
    ' Remarques  :
    ' - Valeur 0 tant que la séance ne consomme pas le quota (nature neutre, No show, annulation).
    ' - Calculé par la couche UI après chargement, car il dépend de l'ensemble des séances du dossier.
    ' -------------------------------------------------------------------------------------------------
    Public Property RangQuotaInami As Integer

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : NumeroPeriodeInami
    ' Version    : V1.0.0
    ' Date       : 19/09/2026
    '
    ' Rôle       :
    ' Numéro de la période annuelle de quota à laquelle la séance se rattache (1 = 1re période ouverte).
    '
    ' Remarques  :
    ' - Deux périodes peuvent coexister dans la grille (période en cours et période suivante déjà
    '   amorcée) : sans ce numéro, les rangs « 1 S / 1 T » réapparaissent sans explication visible.
    ' - Valeur 0 tant que la séance ne consomme pas le quota.
    ' -------------------------------------------------------------------------------------------------
    Public Property NumeroPeriodeInami As Integer

    ' Identifiant du statut courant de la séance
    Public Property IdStatutSeance As Long

    ' Code du statut courant (ex. 'PLANIFIEE', 'REALISEE') — utile pour l'icône/couleur
    Public Property CodeStatutSeance As String

    ' Libellé du statut courant (issu de la jointure ref_statuts_seance)
    Public Property LibelleStatutSeance As String

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SoumisQuota
    ' Version    : V2.0.0
    ' Date       : 19/09/2026
    '
    ' Rôle       :
    ' Indique que la séance relève d'un dossier dont le domaine décompte les séances d'un quota
    ' conventionné. Alimentée depuis ref_domaines.soumis_quota via la jointure sur le dossier.
    '
    ' Remarques  :
    ' - Remplace l'ancien drapeau EstDomaineInami, qui confondait trois règles distinctes et nommait
    '   un organisme particulier alors que d'autres conventions peuvent exister.
    ' -------------------------------------------------------------------------------------------------
    Public Property SoumisQuota As Boolean

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : NoShowFacturable
    ' Version    : V1.0.0
    ' Date       : 19/09/2026
    '
    ' Rôle       :
    ' Indique qu'une absence non prévenue peut être facturée au patient pour ce domaine.
    ' Alimentée depuis ref_domaines.no_show_facturable.
    ' -------------------------------------------------------------------------------------------------
    Public Property NoShowFacturable As Boolean

    ' Date de la séance
    Public Property DateSeance As Date

    ' Heure de début de la séance (NULL si non renseignée)
    Public Property HeureDebut As TimeSpan?

    ' Heure de fin de la séance (NULL si non renseignée) — sert à savoir si la séance est terminée
    Public Property HeureFin As TimeSpan?

    ' Durée de la séance en minutes (NULL si non renseignée)
    Public Property DureeMinutes As Integer?

    ' Tarif de la séance, figé à la création
    Public Property TarifSeance As Decimal

    ' Montant déjà réglé pour cette séance (somme des montants affectés via lia_paiements_seances)
    Public Property MontantRegle As Decimal

    ' Indique si le statut courant rend la séance « due » (facturable) — ref_statuts_seance.est_du
    Public Property EstDuStatut As Boolean

    ' Indique si la séance entre dans le décompte du quota : statut paramétré consommant
    ' (ref_statuts_seance.consomme_quota) et séance non postérieure à la clôture du dossier.
    Public Property StatutConsommeQuota As Boolean

    ' Identifiant de la séance dont celle-ci est issue par report (NULL si séance ordinaire)
    Public Property IdSeanceOrigine As Long?

    ' Code de la séance d'origine (non stocké : alimenté par jointure, ex. 'SE000036')
    Public Property CodeSeanceOrigine As String

    ' Aperçu des notes de séance (texte brut)
    Public Property NotesSeanceTxt As String

    ' Date de dernière modification de la séance
    Public Property DateModification As DateTime

#End Region

#Region "Helpers"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstIssueDeReport
    ' Rôle       : Indique que cette séance est le résultat du report d'une autre séance.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstIssueDeReport As Boolean
        Get
            Return IdSeanceOrigine.HasValue AndAlso IdSeanceOrigine.Value > 0
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : OrigineAffichage
    ' Rôle       : Libellé affiché dans la colonne « Origine » de la grille des séances.
    '              Vide pour une séance ordinaire, « Report de SExxxxxx » pour une séance reportée.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property OrigineAffichage As String
        Get
            If Not EstIssueDeReport Then
                Return String.Empty
            End If

            If String.IsNullOrEmpty(CodeSeanceOrigine) Then
                Return "Report d'une séance"
            End If

            Return $"Report de {CodeSeanceOrigine}"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : HorairesAffichage
    ' Rôle       : Libellé affiché dans la colonne « Horaires » de la grille des séances.
    '              « 09:00 - 09:45 » si début et fin connus, « 09:00 » si seul le début l'est,
    '              vide si aucun horaire n'est renseigné.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property HorairesAffichage As String
        Get
            If Not HeureDebut.HasValue Then
                Return String.Empty
            End If

            If Not HeureFin.HasValue Then
                Return HeureDebut.Value.ToString("hh\:mm")
            End If

            Return HeureDebut.Value.ToString("hh\:mm") & " - " & HeureFin.Value.ToString("hh\:mm")
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstNonFacturableInami
    ' Version    : V2.0.0
    ' Date       : 19/09/2026
    '
    ' Rôle       :
    ' Indique une séance qui ne peut être facturée à personne : absence non prévenue dans un domaine
    ' dont la convention ne rembourse pas les absences.
    '
    ' Remarques  :
    ' - La règle vient désormais de ref_domaines.no_show_facturable et non d'un drapeau codé en dur.
    ' - Le montant ne bascule pas sur le patient : la prestation n'a pas eu lieu et la convention ne
    '   prévoit pas de report de charge. Le montant dû est donc nul.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstNonFacturableInami As Boolean
        Get
            ' Règle tenue par la vue v_seances_solde (est_facturable) : due par son statut mais non
            ' facturable = absence non excusée d'un domaine qui ne facture pas les absences.
            Return EstDuStatut AndAlso Not EstFacturable
        End Get
    End Property

    ' Séance due et facturable (v_seances_solde.est_facturable, 21/10/2026 audit B.4).
    Public Property EstFacturable As Boolean

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : ConsommeQuotaInami
    ' Version    : V1.1.0
    ' Date       : 19/09/2026
    '
    ' Rôle       :
    ' Indique si la séance consomme effectivement une unité du quota annuel INAMI.
    '
    ' Remarques  :
    ' - Seules les natures porteuses d'un code de prestation (soutien / traitement) sont comptées.
    ' - Les statuts consommants sont paramétrés (ref_statuts_seance.consomme_quota) ; une séance
    '   postérieure à la clôture du dossier ne consomme plus le quota.
    ' - Cas du report : l'ancienne séance passe en « Reportée » (-1) et la séance replanifiée est
    '   créée en « Planifiée » (+1), le quota reste donc juste.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property ConsommeQuotaInami As Boolean
        Get
            If Not SoumisQuota Then Return False
            If IdCategorieQuota = 0UL Then Return False

            Return StatutConsommeQuota
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : LibelleQuotaInami
    ' Version    : V1.0.0
    ' Date       : 19/09/2026
    '
    ' Rôle       :
    ' Libellé compact affiché dans la grille des séances : « 1 S », « 2 S » pour le soutien,
    ' « 1 T », « 2 T » pour le traitement.
    '
    ' Retour     :
    ' - Chaîne vide si la séance ne consomme pas le quota (nature neutre ou séance non honorée)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property LibelleQuotaInami As String
        Get
            If Not ConsommeQuotaInami OrElse RangQuotaInami <= 0 Then Return String.Empty

            Return $"{RangQuotaInami} {CategorieQuota.AbreviationDe(LibelleCategorieQuota)}"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : LibellePeriodeInami
    ' Version    : V1.0.0
    ' Date       : 19/09/2026
    '
    ' Rôle       :
    ' Libellé compact de la période affiché dans la grille : « P1 », « P2 ».
    '
    ' Retour     :
    ' - Chaîne vide si la séance ne consomme pas le quota
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property LibellePeriodeInami As String
        Get
            If Not ConsommeQuotaInami OrElse NumeroPeriodeInami <= 0 Then Return String.Empty
            Return $"P{NumeroPeriodeInami}"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : ResteDu
    ' Version    : V1.0.0
    ' Date       : 14/09/2026
    '
    ' Rôle       :
    ' Montant restant dû pour la séance (tarif figé - montant déjà réglé), borné à 0 minimum.
    '
    ' Retour     :
    ' - Decimal : reste à payer sur la séance
    '
    ' Remarques  :
    ' - Le solde patient/dossier est toujours calculé, jamais stocké (règle actée).
    ' - V2.0.0 (21/10/2026, audit B.4) : lu dans la vue v_seances_solde (reste_du), une seule
    '   définition en base : 0 pour un statut non dû et pour une absence non facturable, sinon
    '   max(0, montant - réglé). Il était recalculé ici et ailleurs avec des résultats différents.
    ' -------------------------------------------------------------------------------------------------
    Public Property ResteDu As Decimal

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstSoldee
    ' Version    : V1.0.0
    ' Date       : 14/09/2026
    '
    ' Rôle       :
    ' Indique si la séance est intégralement réglée (reste dû nul).
    '
    ' Retour     :
    ' - Boolean : True si MontantRegle couvre TarifSeance, False sinon
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstSoldee As Boolean
        Get
            Return MontantRegle >= TarifSeance
        End Get
    End Property

    ' Libellé de la pastille d'une séance planifiée dont l'heure est passée (ADR-47).
    Public Const LIBELLE_A_CONFIRMER As String = "À confirmer"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstAConfirmer
    ' Version    : V1.0.0
    ' Date       : 21/10/2026
    '
    ' Rôle       :
    ' Séance encore « Planifiée » dont l'heure est passée : son issue (réalisée, absence, report…) n'a
    ' pas été confirmée. Calcul pur, rien n'est écrit en base (ADR-47).
    '
    ' Remarques  :
    ' - Remplace StatutEffectifCode, qui présumait une telle séance réalisée ; la bascule en base au
    '   chargement de la liste (BasculerPlanifieesEchues) est supprimée.
    ' - Pointage : choisir le statut dans la fenêtre Séance (plus tard, l'Agenda).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstAConfirmer As Boolean
        Get
            Return String.Equals(CodeStatutSeance, CodesSysteme.StatutsSeance.Planifiee, StringComparison.OrdinalIgnoreCase) AndAlso EstPassee
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstPassee
    ' Version    : V1.1.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Indique si la séance est déjà passée (date + heure de fin/début dépassées par rapport à maintenant).
    '
    ' Remarques  :
    ' - V1.1.0 (23/10/2026) : règle dans EstMomentPasse, partagée avec la recherche des paiements.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstPassee As Boolean
        Get
            Return EstMomentPasse(DateSeance, HeureDebut, HeureFin)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : EstMomentPasse
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Règle « séance passée » : l'heure de fin (à défaut, de début) est dépassée ; sans horaire, le
    ' jour entier doit être passé. Partagée par toute ligne qui connaît la date et les horaires d'une
    ' séance (liste des séances, lignes de paiement de la recherche).
    ' -------------------------------------------------------------------------------------------------
    Public Shared Function EstMomentPasse(dateSeance As Date, heureDebut As TimeSpan?, heureFin As TimeSpan?) As Boolean
        Dim reference As Date = dateSeance.Date
        If heureFin.HasValue Then
            reference = reference.Add(heureFin.Value)
        ElseIf heureDebut.HasValue Then
            reference = reference.Add(heureDebut.Value)
        Else
            reference = reference.AddDays(1)
        End If
        Return reference <= Date.Now
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstDue
    ' Version    : V1.0.0
    ' Date       : 17/09/2026
    '
    ' Rôle       :
    ' Indique si la séance génère un montant dû non soldé (statut facturable ET reste dû > 0).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstDue As Boolean
        Get
            Return EstDuStatut AndAlso ResteDu > 0D
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Actions possibles sur la séance (audit C.2) : les boutons de l'onglet Séances les lisent ; les
    ' règles sont celles de GestionSeances, que ReporterSeance, DeleteSeance et CreatePaiement
    ' appliquent aussi.
    ' -------------------------------------------------------------------------------------------------

    ' Seule une séance encore planifiée se reporte (GestionSeances.StatutPermetReport).
    Public ReadOnly Property EstReportable As Boolean
        Get
            Return GestionSeances.StatutPermetReport(CodeStatutSeance)
        End Get
    End Property

    ' Une séance reportée garde la trace du report (GestionSeances.StatutPermetSuppression). Un
    ' encaissement ou un report issu de la séance bloquent aussi la suppression : DeleteSeance les lit
    ' en base.
    Public ReadOnly Property EstSupprimable As Boolean
        Get
            Return GestionSeances.StatutPermetSuppression(CodeStatutSeance)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : RaisonNonEncaissable
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Pourquoi la séance ne peut pas recevoir d'encaissement (vide si elle le peut) : elle doit être
    ' due (statut est_du, non reportée) et pas soldée. Encaisser une séance annulée, en maladie ou
    ' reportée créerait un faux avoir.
    '
    ' Remarques  :
    ' - Reprend SeanceEncaissable et les deux refus d'OuvrirPaiementPourSeance (UC_DossierFiche).
    ' - GestionPaiements.CreatePaiement applique la même règle à l'écriture.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property RaisonNonEncaissable As String
        Get
            If Not EstDuStatut OrElse Not GestionSeances.StatutPermetEncaissement(CodeStatutSeance) Then
                Return $"La séance {CodeSeance} n'est pas due ({LibelleStatutSeance}) : elle ne peut pas être encaissée."
            End If

            If ResteDu <= 0D Then
                Return $"La séance {CodeSeance} est déjà soldée : aucun encaissement supplémentaire n'est possible."
            End If

            Return String.Empty
        End Get
    End Property

    ' La séance peut recevoir un encaissement (RaisonNonEncaissable vide).
    Public ReadOnly Property EstEncaissable As Boolean
        Get
            Return RaisonNonEncaissable.Length = 0
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstNoShow
    ' Version    : V2.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Indique une absence du patient non prévenue. La séance reste due (selon son statut), et ce cas
    ' doit être visuellement distingué d'un simple impayé : il traduit un comportement à surveiller.
    '
    ' Remarques  :
    ' - V2.0.0 (22/10/2026, audit B.6, v2.53.0) : lue dans la case « Absence non excusée » du statut
    '   (ref_statuts_seance.est_absence) au lieu de comparer le code à ABSENTE.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstNoShow As Boolean
        Get
            Return StatutEstAbsence
        End Get
    End Property

    ' Case « Absence non excusée » du statut (ref_statuts_seance.est_absence, migration v2.53.0).
    Public Property StatutEstAbsence As Boolean

    ' Case « Non tenue, libère le créneau » du statut (ref_statuts_seance.est_non_tenue, v2.57.0) :
    ' la séance n'occupe pas son horaire (GestionSeances.OccupeSonCreneau).
    Public Property StatutNonTenu As Boolean

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : AvoirSeance
    ' Version    : V1.1.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Montant encaissé qui ne couvre aucune créance de la séance, donc disponible en avoir :
    ' - séance non due (annulée, maladie, reportée) : tout le réglé ;
    ' - séance due : la part du réglé qui dépasse son montant (trop-perçu après une baisse du montant
    '   au cadenas).
    ' Ce crédit reste réutilisable pour un encaissement ultérieur.
    '
    ' Retour     :
    ' - Decimal : montant en avoir (0 si rien n'est encaissé en trop).
    '
    ' Remarques  :
    ' - Calculé, jamais stocké (règle de conception : calculer plutôt que stocker le dérivable).
    ' - V1.1.0 : le trop-perçu d'une séance due est compté (décision du 17/10/2026) ; auparavant
    '   une séance due n'avait jamais d'avoir, alors que la confirmation de baisse du montant en
    '   annonçait un.
    ' - V2.0.0 (21/10/2026, audit B.4) : lu dans la vue v_seances_solde (montant_avoir), comme la
    '   grille Patients ; SelectAvoirsDisponiblesPatient suit la même définition.
    ' -------------------------------------------------------------------------------------------------
    Public Property AvoirSeance As Decimal

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : MontantPrevisionnel
    ' Version    : V1.0.0
    ' Date       : 17/09/2026
    '
    ' Rôle       :
    ' Montant attendu pour une séance due mais pas encore réalisée. Il s'agit d'une prévision, pas
    ' d'une créance : l'utilisateur ne doit pas la confondre avec un impayé.
    '
    ' Retour     :
    ' - Decimal : Reste dû si la séance est due et à venir, sinon 0
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property MontantPrevisionnel As Decimal
        Get
            If Not EstDuStatut OrElse EstPassee Then
                Return 0D
            End If

            Return ResteDu
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : MontantExigible
    ' Version    : V1.0.0
    ' Date       : 17/09/2026
    '
    ' Rôle       :
    ' Montant réellement réclamable : la séance est due et a déjà eu lieu (ou constitue une absence
    ' non prévenue). C'est cette valeur, et elle seule, qui constitue une dette du patient.
    '
    ' Retour     :
    ' - Decimal : Reste dû si la séance est due et passée, sinon 0
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property MontantExigible As Decimal
        Get
            If Not EstDuStatut OrElse Not EstPassee Then
                Return 0D
            End If

            Return ResteDu
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : FamilleStatut
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Famille d'affichage du statut (couleur de la pastille de la grille des séances), déduite des
    ' propriétés du statut et de la date, jamais d'une liste de codes : un statut ajouté au
    ' référentiel se range de lui-même.
    '
    ' Remarques  :
    ' - Absence non excusée : statut ABSENTE (ligne système, déjà distinguée par EstNoShow).
    ' - Non due : statut sans dû (annulée, malade, reportée…), passée ou à venir.
    ' - À confirmer : séance planifiée dont l'heure est passée (21/10/2026, ADR-47).
    ' - Réalisée / à venir : séance due, selon qu'elle est passée ou non.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property FamilleStatut As FamilleStatutSeance
        Get
            If EstNoShow Then Return FamilleStatutSeance.AbsenceNonExcusee
            If Not EstDuStatut Then Return FamilleStatutSeance.NonDue
            If EstAConfirmer Then Return FamilleStatutSeance.AConfirmer
            Return If(EstPassee, FamilleStatutSeance.Realisee, FamilleStatutSeance.AVenir)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : LibelleStatutAffichage
    ' Rôle       : Texte de la pastille de statut : libellé du référentiel, sauf une séance planifiée
    '              déjà passée, affichée « À confirmer » (calcul pur, sans écriture en base ; elle
    '              était présumée « Réalisée » jusqu'au 21/10/2026, ADR-47).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property LibelleStatutAffichage As String
        Get
            If EstAConfirmer Then
                Return LIBELLE_A_CONFIRMER
            End If
            Return LibelleStatutSeance
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DateReport
    ' Rôle       : Date de la séance qui remplace celle-ci quand elle a été reportée. Renseignée par
    '              l'écran à partir de la liste du dossier (la requête ne la lit pas) ; Nothing sinon.
    ' -------------------------------------------------------------------------------------------------
    Public Property DateReport As Date?

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : CodeSeanceReport
    ' Rôle       : Code de la séance qui remplace celle-ci quand elle a été reportée ; renseigné par
    '              l'écran avec DateReport. Nothing sinon.
    ' -------------------------------------------------------------------------------------------------
    Public Property CodeSeanceReport As String

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : MentionStatut
    ' Rôle       : Petite mention grise à droite de la pastille de statut : « → 31/10 · S000045 »
    '              (date et code de la remplaçante) pour une séance reportée, « report de S000041 »
    '              pour la séance qui la remplace ; vide sinon.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property MentionStatut As String
        Get
            If DateReport.HasValue Then
                Return If(String.IsNullOrEmpty(CodeSeanceReport),
                          $"→ {DateReport.Value:dd/MM}",
                          $"→ {DateReport.Value:dd/MM} · {CodeSeanceReport}")
            End If
            If EstIssueDeReport Then
                Return If(String.IsNullOrEmpty(CodeSeanceOrigine), "report", $"report de {CodeSeanceOrigine}")
            End If
            Return String.Empty
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : FamillePaiement
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Famille d'affichage de l'état de paiement (couleur de la pastille « Paiement »).
    '
    ' Remarques  :
    ' - Une séance due mais pas encore réalisée est « prévue » et non « à régler » : tant qu'elle n'a
    '   pas eu lieu, aucun montant n'est réclamable (règle reprise de l'ancien EtatPaiement).
    ' - Absence non prévenue non facturable (convention) : non due.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property FamillePaiement As FamillePaiementSeance
        Get
            If EstNonFacturableInami Then Return FamillePaiementSeance.NonDue
            If Not EstDuStatut Then
                Return If(AvoirSeance > 0D, FamillePaiementSeance.Avoir, FamillePaiementSeance.NonDue)
            End If
            If EstSoldee Then Return FamillePaiementSeance.Reglee
            Return If(EstPassee, FamillePaiementSeance.ARegler, FamillePaiementSeance.Prevue)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : PaiementAffichage
    ' Rôle       : Texte de la pastille « Paiement » : « Réglée », « Payée d'avance », « À régler · 60 € »,
    '              « Reste · 20 € », « Prévue · 60 € », « Avoir · 60 € », « Non due »,
    '              « Non facturée (convention) » ; « Réglée · avoir 10 € » pour un trop-perçu (V1.2.0).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property PaiementAffichage As String
        Get
            If EstNonFacturableInami Then Return "Non facturée (convention)"
            Select Case FamillePaiement
                Case FamillePaiementSeance.Avoir
                    Return $"Avoir · {MontantCourt(AvoirSeance)}"
                Case FamillePaiementSeance.NonDue
                    Return "Non due"
                Case FamillePaiementSeance.Reglee
                    Dim texte As String = If(EstPassee, "Réglée", "Payée d'avance")
                    Return If(AvoirSeance > 0D, $"{texte} · avoir {MontantCourt(AvoirSeance)}", texte)
                Case FamillePaiementSeance.ARegler
                    Return If(MontantRegle > 0D, $"Reste · {MontantCourt(ResteDu)}", $"À régler · {MontantCourt(ResteDu)}")
                Case Else
                    Return If(MontantRegle > 0D, $"Prévue · reste {MontantCourt(ResteDu)}", $"Prévue · {MontantCourt(ResteDu)}")
            End Select
        End Get
    End Property

    ' « 60 € » pour un montant rond, « 62,50 € » sinon (pastilles : court et lisible). Partagée avec
    ' PaiementListeItem (pastille « État » de la grille des paiements).
    Friend Shared Function MontantCourt(montant As Decimal) As String
        Dim format As String = If(montant = Decimal.Truncate(montant), "N0", "N2")
        Return montant.ToString(format, Globalization.CultureInfo.CurrentCulture) & " €"
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : LibelleChoixSeance
    ' Version    : V2.0.0
    ' Date       : 21/10/2026
    '
    ' Rôle       :
    ' Libellé lisible d'une séance à régler avec un avoir (code, date, heure, nature et reste dû),
    ' affiché dans la liste de choix de DialogUtilisationAvoir.
    '
    ' Remarques  :
    ' - V2.0.0 (21/10/2026) : ancien LibelleReaffectation ; le reste dû remplace le tarif (c'est ce
    '   que l'avoir peut régler).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property LibelleChoixSeance As String
        Get
            Dim heure As String = If(HeureDebut.HasValue,
                                     $" {HeureDebut.Value:hh\:mm}",
                                     String.Empty)

            Dim code As String = If(String.IsNullOrWhiteSpace(CodeSeance), String.Empty, $"{CodeSeance} · ")

            Return $"{code}{DateSeance:dd/MM/yyyy}{heure} - {LibelleNatureSeance} (reste dû {ResteDu:N2} €)"
        End Get
    End Property

#End Region

End Class

' -------------------------------------------------------------------------------------------------
' Enum     : FamilleStatutSeance / FamillePaiementSeance
' Version  : V1.1.0
' Date     : 17/10/2026
'
' Rôle :
' Familles d'affichage de la grille des séances (couleur des pastilles Statut et Paiement) ;
' les couleurs elles-mêmes sont dans UtilsPastilles (couche UI).
'
' Remarques :
' - V1.1.0 : FamillePaiementSeance sert aussi à la pastille « État » de la grille des paiements
'   (PaiementListeItem.FamilleEtat) : un paiement annulé ou reporté y prend la famille NonDue (gris).
' -------------------------------------------------------------------------------------------------
Public Enum FamilleStatutSeance
    AVenir
    Realisee
    AbsenceNonExcusee
    NonDue
    ' Planifiée dont l'heure est passée, issue pas encore confirmée (21/10/2026, ADR-47).
    AConfirmer
End Enum

Public Enum FamillePaiementSeance
    Reglee
    ARegler
    Prevue
    Avoir
    NonDue
End Enum
