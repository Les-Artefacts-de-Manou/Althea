' -------------------------------------------------------------------------------------------------
' Classe      : UC_DossierFiche
' Projet      : Althéa
' Version     : V1.24.0
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Hub d'un dossier de suivi clinique, ouvert depuis l'onglet Dossiers de UC_PatientFiche.
' Réplique le pattern UC_PatientHome → UC_PatientFiche un niveau plus bas (D-DOS-1).
'
' Responsabilités :
' - Recevoir le contexte UI partagé (IContextAwareUserControl)
' - S'initialiser en consultation d'un dossier existant ou en création d'un nouveau dossier
' - Charger le dossier au Load et renseigner le bandeau d'identité (code, domaine, statut, dates, référent)
' - Afficher le récapitulatif patient (UI-5) : quatre blocs Patient / Dossier / Finances / Quotas INAMI,
'   toujours visibles quel que soit l'onglet actif
' - Offrir un retour vers la fiche patient (mini-pile de navigation D-Q15)
'
' Sous-onglets (D-DOS-1) :
' - Suivi clinique : cœur clinique (7 zones) — édition par zone via DossierZoneEditor (B4)
' - Documents / Séances : coquilles « à venir » (relèvent des Lots 2 et 3)
'
' Important   :
' - Persistance complète : création (CreerDossier → GestionDossiers.CreateDossier), mise à jour
'   d'identité (MettreAJourDossier → UpdateDossier) et cycle de vie (DossierStatutEditor → B6).
' - Aucun accès base directe : délègue à GestionDossiers (couche métier).
' - V1.3.0 (14/10/2026, UI-7) : domaine en AltheaListe, date d'ouverture et filtres « du / au »
'   (séances, paiements) en AltheaDate ; carte des paramètres en consultation lisible.
' - V1.4.0 (15/10/2026) : « Modifier » montré seulement sur l'onglet Fiche Dossier (AfficherBoutonModifier).
' - V1.5.0 (17/10/2026, maquette « Grille des séances ») : 10 colonnes au lieu de 15 ; statut et paiement en
'   pastilles colorées par famille (UtilsPastilles), solde unique (− dû, + avoir), lignes non dues
'   atténuées, ligne « Aujourd'hui » entre séances à venir et passées, résumé cliquable au-dessus de la
'   grille (pastilles qui choisissent le filtre) ; code, période et quota en infobulle.
' - V1.6.0 (17/10/2026, maquette « Paiements — A ») : grille des paiements en relevé chronologique
'   (Date, Mouvement, Pour, Mode, Montant, État en pastille), fin du regroupement par séance, tri par
'   en-tête rétabli, filtre « À régler » ; résumé cliquable commun aux deux grilles (ResumeFiltre).
' - V1.6.1 (17/10/2026) : montant de ligne = montant affecté à la séance (paiement réparti, avoir
'   utilisé) ; compte rendu affiché après la réaffectation d'un paiement au report ; bouton
'   « Annuler l'encaissement » (au lieu de « Supprimer paiement ») ; « Encaisser séance » inactif sur
'   une séance soldée.
' - V1.6.2 (17/10/2026) : trop-perçu d'une séance due compté en avoir (paiements : filtre Avoirs,
'   mention « avoir 10 € » ; RenseignerEtatSeances).
' - V1.6.3 (17/10/2026) : le relevé masque les dus en attente clos automatiquement (une ligne
'   « Paiement annulé » apparaissait après chaque encaissement).
' - V1.6.4 (17/10/2026) : colonne « Code » dans la grille des séances ; les fenêtres ouvertes sur une
'   séance, un paiement ou le dossier reçoivent son code (en-tête, rappel de la séance).
' - V1.6.5 (17/10/2026) : refus d'annuler un règlement par avoir annoncé par un message.
' - V1.7.0 (17/10/2026) : bandeau Finances — avoir du patient (tous dossiers) avec la part du
'   dossier, montants dus du dossier suivis du total du patient ; un seul calcul
'   (GestionSeances.CalculerSoldesPatient), fin du calcul par codes de statut en dur.
' - V1.8.0 (17/10/2026) : clôture refusée tant que des séances sont à venir, montants à régler
'   signalés dans la confirmation (GestionDossiers.ControlerCloture) ; pastille de statut colorée
'   selon l'état et bandeau « consultation » sous l'en-tête d'un dossier clôturé ou archivé ; codes
'   de statut repris de GestionDossiers.
' - V1.8.1 (17/10/2026) : ResumeFiltre sorti dans UI/Controls/Communs (partagé avec la grille Patients).
' - V1.9.0 (19/10/2026) : grilles Séances et Paiements paginées (UC_Pagination) ; les séances s'ouvrent
'   sur la page de la ligne « Aujourd'hui », les paiements sur la plus récente.
' - V1.10.0 (19/10/2026) : modifications non enregistrées — quitter l'écran (Retour, menu de Home,
'   fermeture d'Althéa : IUnsavedChangesAware) propose Enregistrer / Quitter sans enregistrer / Rester,
'   Annuler demande « Abandonner / Continuer la saisie » (GardeModifications ; ADR-43).
' - V1.10.1 (19/10/2026) : le patient est transmis à la zone clinique (en-tête des documents).
' - V1.11.0 (20/10/2026, v2.45.0) : logo du domaine dans le bandeau (à droite du bloc Dossier, cadre
'   120 × 52, lisible) et devant chaque domaine de la liste « Domaine » de la fiche (création
'   comprise) ; la convention le montre aussi
'   (UC_SyntheseDomaine). Liste des domaines : membres posés avant la source.
' - V1.12.0 (20/10/2026) : modalités de paiement proposées lues dans Paramètres (v2.46.0, elles
'   étaient écrites dans le code) ; boutons Séances et Paiements inactifs : la raison s'affiche au survol
'   (RaisonDossierFige, DefinirActionSeance ; AltheaBouton.DefinirDisponibilite).
'
' Dépendances :
' - GestionDossiers (lecture du dossier)
' - Home (navigation retour)
' - UserControlContext / IContextAwareUserControl (contexte UI partagé)
' - V1.13.0 (20/10/2026) : codes des statuts de séance lus dans GestionSeances.CODE_STATUT_* (une seule définition ; audit B9).
' - V1.13.1 (20/10/2026) : annulation d'un encaissement — rôle courant transmis au métier, qui vérifie le droit (audit A.3).
' - V1.14.0 (21/10/2026) : un encaissement dont une partie a servi d'avoir n'est plus annulable ; le refus est annoncé avant la saisie du motif (audit B.1).
' - V1.14.1 (21/10/2026) : séance reportée — la mention du statut donne la date et le code de la remplaçante (demande de Joëlle).
' - V1.14.2 (21/10/2026) : report — refus métier affiché tel quel, échec annoncé « rien n'a été modifié » (report d'un seul tenant, audit B.2).
' - V1.15.0 (21/10/2026) : après le report d'une séance encaissée, proposition d'utiliser l'avoir (DialogUtilisationAvoir) au lieu de réaffecter l'encaissement (audit B.2, décision de Joëlle).
' - V1.16.0 (21/10/2026) : séances « À confirmer » (planifiées passées, ADR-47) — filtre et pastille du résumé.
' - V1.17.0 (21/10/2026) : filtres des séances (demandes de Joëlle) — « Aujourd'hui » et « Reportées » ajoutés, ordre revu ; « À venir » ne garde que les séances dues à venir (comme la pastille du résumé, qui en comptait moins que la liste n'en montrait) ; filtres des séances et des paiements aux couleurs des pastilles.
' - V1.17.1 (21/10/2026, audit B.4) : la séance reconstituée depuis un paiement (séance filtrée) reprend
'   le reste dû lu en base, qui n'est plus recalculé par SeanceListeItem.
' - V1.17.2 (21/10/2026, audit B.5) : règlement par avoir reconnu par le référentiel
'   (PaiementListeItem.EstReglementParAvoir), plus par le libellé du mode.
' - V1.17.3 (22/10/2026, audit B.5) : l'annulation d'un encaissement transmet l'identifiant de
'   l'utilisateur (plus son nom affiché).
' - V1.17.4 (22/10/2026, audit B.5, v2.52.0) : de même pour l'utilisation d'un avoir ; l'infobulle
'   d'un règlement par avoir dit d'où vient l'argent (PaiementListeItem.MentionOrigineAvoir).
' - V1.17.5 (22/10/2026, audit B.6) : après un changement de domaine du dossier, les séances sont
'   relues (quota, catégories, tarifs du nouveau domaine) : le bloc Quotas restait sur l'ancien.
' - V1.17.6 (22/10/2026, audit C.1) : annulation d'un encaissement et utilisation d'un avoir reçoivent
'   le ContexteOperation de l'écran (utilisateur + rôle courant).
' - V1.18.0 (22/10/2026, audit C.1, étape 2) : la raison d'un dossier figé vient du métier
'   (GestionDossiers.RaisonDossierFige, qui refuse aussi l'écriture) ; dossier archivé : « Modifier »
'   la fiche inactif avec sa raison (décision de Joëlle) ; refus et échec de l'enregistrement de la
'   fiche affichés dans une fenêtre ; utilisation d'un avoir : refus affiché tel quel, échec sans
'   détail technique.
' - V1.19.0 (22/10/2026, audit C.2) : reporter, supprimer et encaisser une séance lisent les règles
'   du métier (SeanceListeItem.EstReportable, EstSupprimable, EstEncaissable / RaisonNonEncaissable) ;
'   SeanceReportable et SeanceEncaissable supprimées.
' - V1.20.0 (22/10/2026, relevés de Joëlle au test P-93, décisions de Joëlle) : une action sur une
'   ligne exige que l'utilisatrice l'ait choisie (UtilsDataGrid.ElementSelectionne : la ligne
'   courante posée d'office au chargement ne compte plus) ; bouton « Nouveau paiement » de l'onglet
'   Paiements retiré — il encaissait la séance sélectionnée dans l'onglet Séances, invisible depuis
'   Paiements, et sans proposer l'avoir : on encaisse par « Encaisser séance » ; colonne « Solde »
'   retirée (la pastille Paiement dit déjà « À régler », « Reste » et « Avoir »).
'   Bandeau (demande de Joëlle) : « 0 € ici · 68,00 € tous dossiers » ; nouvelle ligne « Prévu »
'   (reste à payer des séances à venir) ; l'avoir passe à la ligne suivante.
' - V1.21.0 (23/10/2026, recherche transversale R1, ADR-52) : ouverture ciblée — le hub peut s'ouvrir
'   sur l'onglet Séances ou Paiements, la ligne demandée sélectionnée sur sa page (CiblerAuChargement,
'   appelé par la recherche avant la navigation). Relevé de Joëlle : colonne « Code » (PAI…) dans la
'   grille des paiements — le code d'un paiement ne se voyait nulle part.
' - V1.22.0 (23/10/2026, recherche transversale R2) : axes du filtre des séances, filtrage et résumé
'   sortis dans FiltresSeances (partagés avec la recherche des séances d'une période), sans
'   changement de comportement ; constantes FiltreSeance* remplacées par AxeFiltreSeance ;
'   CorrigerBornes déplacée dans AltheaDate (partagée).
' - V1.23.0 (23/10/2026, recherche transversale R3) : lignes du relevé, axes du filtre, filtrage et
'   résumé des paiements sortis dans FiltresPaiements, infobulle d'une ligne dans
'   PaiementListeItem.InfobulleLigne (partagés avec la recherche des paiements), sans changement de
'   comportement ; constantes FiltrePaiement* remplacées par AxeFiltrePaiement.
' - V1.23.1 (24/10/2026, audit C.3) : codes de statut de dossier lus dans
'   CodesSysteme.StatutsDossier.
' - V1.23.2 (25/10/2026, audit C.4, ADR-55) : la fenêtre principale est lue dans le contexte
'   (_context.FenetrePrincipale, IFenetrePrincipale) ; plus aucune référence à Home.
' - V1.24.0 (25/10/2026, audit C.5, décision de Joëlle) : fiche répartie en fichiers partiels, un
'   par onglet, code déplacé tel quel : UC_DossierFiche.ZonesCliniques.vb (Suivi clinique),
'   .Seances.vb, .Paiements.vb ; ici le reste (chargement, bandeau, modes, édition, statut,
'   navigation, garde).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_DossierFiche
    Implements IContextAwareUserControl, IUnsavedChangesAware

#Region "Types"

    ' -------------------------------------------------------------------------------------------------
    ' Énumération : ModeFiche
    ' Rôle        : Décrit le mode courant du hub dossier (pattern UC_PatientFiche, D-Q14).
    ' -------------------------------------------------------------------------------------------------
    Public Enum ModeFiche
        Consultation
        Creation
        Modification
    End Enum

    ' Onglet sur lequel ouvrir le hub (ouverture ciblée, recherche transversale R1).
    Public Enum OngletDossier
        Fiche
        Seances
        Paiements
    End Enum

#End Region

#Region "Garde des modifications"

    ' Photo des paramètres du dossier en création ou en modification (créée au chargement).
    Private _garde As GardeModifications

#End Region

#Region "Variables privées"

    ' Bandeau « consultation » (dossier clôturé ou archivé) actuellement affiché. Tenu ici plutôt que
    ' lu sur Visible, qui vaut False tant que le contrôle n'est pas à l'écran.
    Private _bandeauConsultationAffiche As Boolean

    ' Injecté par Home via SetContext() (implémentation IContextAwareUserControl)
    Private _context As UserControlContext

    ' Dossier actuellement chargé dans le hub (Nothing tant qu'aucun dossier n'est chargé en création)
    Private _dossier As Dossier

    ' Mode courant du hub (consultation par défaut)
    Private _mode As ModeFiche = ModeFiche.Consultation

    ' Identifiant du dossier en cours de consultation (0 = création, aucun id encore attribué)
    Private _idDossier As Long = 0

    ' Identifiant du patient propriétaire du dossier (obligatoire, y compris en création)
    Private _idPatient As Long = 0

    ' Patient propriétaire du dossier (affiché dans le bandeau et le fil d'Ariane)
    Private _patient As Patient

    ' Domaine du dossier : porte l'organisme et l'assujettissement au quota (bloc Quotas du récapitulatif)
    Private _domaineDossier As Domaine

    ' Logo du domaine : cadre dans le bandeau (à droite du bloc Dossier, sur les lignes Statut à
    ' Clôture) et dans la liste « Domaine » (hauteur d'une ligne). Des cadres et non des carrés :
    ' les logos sont souvent en longueur (relevé de Joëlle, 20/10/2026).
    Private Const LARGEUR_LOGO_BANDEAU As Integer = 120
    Private Const HAUTEUR_LOGO_BANDEAU As Integer = 52
    Private Const LARGEUR_LOGO_LISTE As Integer = 56
    Private Const HAUTEUR_LOGO_LISTE As Integer = 18

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SetContext
    ' Version   : V1.0.0
    ' Date      : 21/07/2026
    '
    ' Rôle      :
    ' Reçoit le contexte UI partagé injecté par Home.
    '
    ' Paramètres :
    ' - context : Contexte UI partagé de l'application.
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(context As UserControlContext) _
        Implements IContextAwareUserControl.SetContext

        _context = context

    End Sub

#End Region

#Region "Initialisation publique"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserPourDossier
    ' Version   : V1.0.0
    ' Date      : 21/07/2026
    '
    ' Rôle      :
    ' Prépare le hub pour la consultation d'un dossier existant.
    '
    ' Paramètres :
    ' - idDossier : Identifiant du dossier à charger
    '
    ' Remarques :
    ' - À appeler par l'appelant (UC_PatientFiche) avant la navigation.
    ' - Le chargement effectif a lieu au Load du UserControl.
    ' -------------------------------------------------------------------------------------------------
    Public Sub InitialiserPourDossier(idDossier As Long)

        _idDossier = idDossier
        _mode = ModeFiche.Consultation

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserPourNouveauDossier
    ' Version   : V1.0.0
    ' Date      : 21/07/2026
    '
    ' Rôle      :
    ' Prépare le hub pour la création d'un nouveau dossier rattaché à un patient.
    '
    ' Paramètres :
    ' - idPatient : Identifiant du patient propriétaire du futur dossier
    '
    ' Remarques :
    ' - La saisie est persistée par CreerDossier (GestionDossiers.CreateDossier) à l'enregistrement.
    ' -------------------------------------------------------------------------------------------------
    Public Sub InitialiserPourNouveauDossier(idPatient As Long)

        _idPatient = idPatient
        _idDossier = 0
        _mode = ModeFiche.Creation

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : CiblerAuChargement
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Demande d'ouvrir le hub sur un onglet, avec une séance ou un paiement sélectionné sur sa page.
    '
    ' Paramètres :
    ' - onglet     : onglet à afficher
    ' - idSeance   : séance à sélectionner (onglet Séances), 0 = aucune
    ' - idPaiement : paiement à sélectionner (onglet Paiements), 0 = aucun
    '
    ' Remarques :
    ' - À appeler après InitialiserPourDossier, avant la navigation (recherche transversale, ADR-52).
    ' - La sélection est posée une fois l'onglet affiché (une grille masquée perd sa sélection quand
    '   elle devient visible) ; si les filtres de l'onglet cachent la ligne, la barre d'état le dit.
    ' -------------------------------------------------------------------------------------------------
    Public Sub CiblerAuChargement(onglet As OngletDossier, Optional idSeance As Long = 0, Optional idPaiement As Long = 0)

        _ongletCible = onglet
        _idSeanceCible = idSeance
        _idPaiementCible = idPaiement

    End Sub

#End Region

#Region "Cycle de vie"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : UC_DossierFiche_Load
    ' Version   : V1.2.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Charge le dossier (si consultation) et renseigne le bandeau à l'ouverture du hub.
    ' - V1.1.0 (19/10/2026) : crée la garde des modifications (GardeModifications).
    ' - V1.2.0 (23/10/2026, recherche R1) : ouverture ciblée (AppliquerCible) après le chargement.
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_DossierFiche_Load(
        sender As Object,
        e As EventArgs
    ) Handles MyBase.Load

        If DesignMode Then
            Return
        End If

        ' Photo des paramètres du dossier en création ou en modification (ADR-43).
        _garde = New GardeModifications(carteParametres)

        InitialiserBoutons()
        ConstruireZonesCliniques()
        ChargerDomaines()
        ChargerDossier()

        ' Ouverture ciblée (recherche) : une fois le hub affiché.
        If _ongletCible.HasValue AndAlso _dossier IsNot Nothing Then
            BeginInvoke(New Action(AddressOf AppliquerCible))
        End If

        ' Home réécrit lblContexte après le chargement de la vue : on réapplique le fil d'Ariane détaillé ensuite.
        BeginInvoke(New Action(Sub() RafraichirContexteNavigation()))

        AddHandler Me.Disposed, Sub()
                                    If _apercuPopup IsNot Nothing Then
                                        _apercuPopup.Dispose()
                                        _apercuPopup = Nothing
                                    End If
                                    RemplacerPhotoRecap(Nothing)
                                End Sub

    End Sub

#End Region

#Region "Chargement"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserBoutons
    ' Version   : V1.0.0
    ' Date      : 21/07/2026
    '
    ' Rôle      :
    ' Applique le style standard aux boutons d'action et branche les info-bulles.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserBoutons()

        If _context IsNot Nothing Then
            _context.SetToolTip(btnModifier, "Passer la fiche dossier en modification.")
            _context.SetToolTip(btnEnregistrer, "Enregistrer les modifications de la fiche dossier.")
            _context.SetToolTip(btnAnnuler, "Annuler les modifications en cours.")
            _context.SetToolTip(btnChangerStatut, "Changer le statut du dossier.")
            _context.SetToolTip(btnFermer, "Revenir à la liste des patients.")

            _context.SetToolTip(btnNouvelleSeance, "Créer une nouvelle séance dans ce dossier.")
            _context.SetToolTip(btnModifierSeance, "Modifier la séance sélectionnée.")
            _context.SetToolTip(btnReporterSeance, "Reporter la séance sélectionnée à une autre date.")
            _context.SetToolTip(btnSupprimerSeance, "Supprimer la séance sélectionnée.")
            _context.SetToolTip(btnEncaisserSeance, "Encaisser la séance sélectionnée.")

            _context.SetToolTip(btnSupprimerPaiement, "Annuler l'encaissement sélectionné (conservé et tracé, motif obligatoire).")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerDomaines
    ' Version   : V1.1.0
    ' Date      : 20/10/2026
    '
    ' Rôle      :
    ' Charge les domaines actifs dans la combo de la fiche dossier.
    '
    ' Remarques :
    ' - En modification d'un dossier existant, on inclut le domaine courant même s'il est inactif,
    '   afin de ne pas perdre la valeur enregistrée.
    ' - V1.1.0 (20/10/2026) : membres posés avant DataSource (ordre recommandé, ToDo) ; liste dessinée avec les logos.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerDomaines()

        Try

            Dim domaines As List(Of Domaine) = GestionDomaines.GetDomainesActifs()

            ' Membres avant la source : sinon la sélection posée par DataSource lit un objet entier.
            cboDomaine.DisplayMember = NameOf(Domaine.LibelleDomaine)
            cboDomaine.ValueMember = NameOf(Domaine.IdDomaine)
            cboDomaine.DataSource = domaines
            cboDomaine.SelectedIndex = -1

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur chargement des domaines (UC_DossierFiche).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors du chargement des domaines.")
            End If

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerDossier
    ' Version   : V1.0.0
    ' Date      : 21/07/2026
    '
    ' Rôle      :
    ' Charge le dossier depuis la couche métier (mode consultation) et met à jour le bandeau.
    '
    ' Remarques :
    ' - En création, aucun dossier n'existe encore : le bandeau reste en mode « Nouveau dossier ».
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerDossier()

        If _mode = ModeFiche.Creation OrElse _idDossier <= 0 Then
            _dossier = Nothing
            RemplirBandeau()
            RemplirFicheDossier()
            AppliquerMode(ModeFiche.Creation)
            Return
        End If

        Try

            _dossier = GestionDossiers.GetDossier(_idDossier)

            If _dossier Is Nothing Then

                If _context IsNot Nothing Then
                    _context.SetStatus("Dossier introuvable.")
                End If

                RemplirBandeau()
                RemplirFicheDossier()
                Return

            End If

            _idPatient = _dossier.IdPatient
            RemplirBandeau()
            RemplirFicheDossier()
            AppliquerMode(ModeFiche.Consultation)
            InitialiserGrilleSeances()
            InitialiserGrillePaiements()
            ChargerSeances()

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur chargement du dossier (UC_DossierFiche, id={_idDossier}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors du chargement du dossier.")
            End If

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplirBandeau
    ' Version   : V1.2.0
    ' Date      : 20/10/2026
    '
    ' Rôle      :
    ' Renseigne les libellés du bandeau d'identité à partir du dossier chargé (ou du mode création).
    '
    ' Remarques :
    ' - Les valeurs absentes (date de clôture) sont affichées par un tiret cadratin.
    ' - V1.1.0 : état du dossier rendu visible (AfficherEtatDossier).
    ' - V1.2.0 (20/10/2026) : logo du domaine à droite du bloc Dossier (AfficherLogoBandeau).
    ' -------------------------------------------------------------------------------------------------
    Private Sub RemplirBandeau()

        Const Absent As String = "—"

        RemplirIdentitePatient()

        ChargerDomaineDossier()

        If _dossier Is Nothing Then

            enTete.Text = "Nouveau dossier"
            lblDomaineValeur.Text = Absent
            AfficherLogoBandeau(0, Nothing)
            lblStatutValeur.Text = Absent
            lblDateOuvertureValeur.Text = Absent
            lblDateClotureValeur.Text = Absent
            btnChangerStatut.Visible = False
            AfficherEtatDossier()
            RafraichirContexteNavigation()
            Return

        End If

        enTete.Text = _dossier.CodeDossier
        lblDomaineValeur.Text = If(String.IsNullOrWhiteSpace(_dossier.LibelleDomaine), Absent, _dossier.LibelleDomaine)
        AfficherLogoBandeau(_dossier.IdDomaine, _dossier.LogoDomaine)
        lblStatutValeur.Text = If(String.IsNullOrWhiteSpace(_dossier.LibelleStatutDossier), Absent, _dossier.LibelleStatutDossier)
        lblDateOuvertureValeur.Text = _dossier.DateOuverture.ToShortDateString()
        lblDateClotureValeur.Text = If(_dossier.DateCloture.HasValue, _dossier.DateCloture.Value.ToShortDateString(), Absent)
        AfficherEtatDossier()

        ' Le bouton de changement de statut n'est proposé que pour un dossier persisté et hors édition.
        btnChangerStatut.Visible = _idDossier > 0 AndAlso _mode = ModeFiche.Consultation

        ' Charger le tarif patient et les montants dus
        ChargerTarifEtMontantsDus()

        RafraichirContexteNavigation()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherEtatDossier
    ' Version   : V1.0.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Rend l'état du dossier visible d'emblée (demande de Joëlle du 17/10/2026) : pastille de statut
    ' colorée (UtilsPastilles.TeinteStatutDossier) et, pour un dossier clôturé ou archivé, bandeau
    ' d'information sous l'en-tête qui dit ce qui reste possible.
    '
    ' Remarques :
    ' - Le bandeau s'ajoute à la hauteur de la zone d'en-tête : le récapitulatif garde la sienne.
    ' - Clôturé : séances et paiements sont figés (EstDossierArchive), la fiche et le suivi clinique
    '   restent modifiables ; Archivé : tout est figé, sans retour possible.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherEtatDossier()

        Dim teinte As UtilsPastilles.TeintePastille = UtilsPastilles.TeinteStatutDossier(_dossier)
        lblStatutValeur.BackColor = teinte.Fond
        lblStatutValeur.ForeColor = teinte.Texte

        Dim message As String = MessageConsultation()
        Dim afficher As Boolean = Not String.IsNullOrEmpty(message)

        If afficher Then
            lblDossierConsultation.BackColor = AltheaIconeEtat.Teintes(EtatIcone.Information).Pale
            lblDossierConsultation.ForeColor = AltheaIconeEtat.Teintes(EtatIcone.Information).Foncee
            lblDossierConsultation.Text = message
        End If

        If afficher <> _bandeauConsultationAffiche Then
            pnlBandeau.Height += If(afficher, pnlDossierConsultation.Height, -pnlDossierConsultation.Height)
            pnlDossierConsultation.Visible = afficher
            _bandeauConsultationAffiche = afficher
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : MessageConsultation
    ' Version  : V1.0.0
    ' Date     : 17/10/2026
    '
    ' Rôle     :
    ' Texte du bandeau d'un dossier clôturé ou archivé (vide pour un dossier actif ou en pause).
    ' -------------------------------------------------------------------------------------------------
    Private Function MessageConsultation() As String

        If _dossier Is Nothing Then
            Return String.Empty
        End If

        If _dossier.EstArchive Then
            Dim le As String = If(_dossier.DateArchivage.HasValue, $" le {_dossier.DateArchivage.Value:dd/MM/yyyy}", String.Empty)
            Return $"Dossier archivé{le} — consultation seulement (archivage définitif)."
        End If

        If _dossier.EstCloture Then
            Dim le As String = If(_dossier.DateCloture.HasValue, $" le {_dossier.DateCloture.Value:dd/MM/yyyy}", String.Empty)
            Return $"Dossier clôturé{le} — séances et paiements en consultation seulement. " &
                   $"Pour le rouvrir : « {btnChangerStatut.Text} »."
        End If

        Return String.Empty

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplirIdentitePatient
    ' Version   : V1.1.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Affiche le nom et le code du patient propriétaire dans l'en-tête du dossier, afin de ne
    ' jamais perdre la notion du patient lors de la navigation dans le hub, puis renseigne le
    ' bloc « Patient » du récapitulatif (photo, naissance, alerte).
    '
    ' Remarques :
    ' - Le patient n'est relu en base que si l'identifiant a changé.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RemplirIdentitePatient()

        Try

            If _idPatient > 0 AndAlso (_patient Is Nothing OrElse _patient.IdPatient <> _idPatient) Then
                _patient = GestionPatients.GetPatientById(_idPatient)
            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur chargement du patient du dossier (UC_DossierFiche, idPatient={_idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            _patient = Nothing

        End Try

        RemplirRecapPatient()

        If _patient Is Nothing Then
            enTete.SousTitre = "Patient : —"
            Return
        End If

        Dim code As String = If(String.IsNullOrWhiteSpace(_patient.CodePatient), String.Empty, $" ({_patient.CodePatient})")
        enTete.SousTitre = $"Patient : {_patient.NomComplet}{code}"

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ConstruireContexteNavigation
    ' Version  : V1.1.0
    ' Date     : 11/10/2026
    '
    ' Rôle     :
    ' Construit le fil d'Ariane du hub : « Patients > {patient} ({code}) > {dossier} > {onglet} » ; le
    ' domaine est transmis à part et affiché en pastille (RafraichirContexteNavigation).
    '
    ' Retour   :
    ' - String : Fil d'Ariane courant (sans action).
    ' -------------------------------------------------------------------------------------------------
    Private Function ConstruireContexteNavigation() As String

        Dim segmentPatient As String = "Patient"

        If _patient IsNot Nothing Then
            segmentPatient = _patient.NomComplet
            If Not String.IsNullOrWhiteSpace(_patient.CodePatient) Then
                segmentPatient &= $" ({_patient.CodePatient})"
            End If
        End If

        ' Le domaine n'est plus dans le texte : il s'affiche en pastille à la fin du fil (Home, UI).
        Dim segmentDossier As String = If(_dossier IsNot Nothing, _dossier.CodeDossier, "Nouveau dossier")

        Dim contexte As String = $"Patients > {segmentPatient} > {segmentDossier}"

        If tabDossier.SelectedTab IsNot Nothing Then
            contexte &= $" > {tabDossier.SelectedTab.Text}"
        End If

        If _mode = ModeFiche.Modification Then
            contexte &= " > Modification"
        ElseIf _mode = ModeFiche.Creation Then
            contexte &= " > Création"
        End If

        Return contexte

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RafraichirContexteNavigation
    ' Version   : V1.0.0
    ' Date      : 01/10/2026
    '
    ' Rôle      :
    ' Met à jour lblContexte (Home) avec le fil d'Ariane du hub, éventuellement complété d'une action.
    '
    ' Paramètres :
    ' - action : Action en cours (ex. « Nouvelle séance »), ajoutée en dernier segment (facultatif).
    '
    ' Remarques :
    ' - N'écrit pas dans stsStatus : celle-ci porte les messages d'action.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RafraichirContexteNavigation(Optional action As String = Nothing)

        If _context Is Nothing Then
            Return
        End If

        Dim contexte As String = ConstruireContexteNavigation()

        If Not String.IsNullOrWhiteSpace(action) Then
            contexte &= $" > {action}"
        End If

        _context.SetHeader(contexte, _dossier?.LibelleDomaine)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SignalerAction
    ' Version   : V1.0.0
    ' Date      : 01/10/2026
    '
    ' Rôle      :
    ' Signale l'ouverture d'une action (modale) : fil d'Ariane complété de l'action et message
    ' explicite dans la barre de statut.
    '
    ' Paramètres :
    ' - action  : Segment ajouté au fil d'Ariane
    ' - message : Message affiché dans stsStatus
    ' -------------------------------------------------------------------------------------------------
    Private Sub SignalerAction(action As String, message As String)

        RafraichirContexteNavigation(action)
        _context?.SetStatus(message)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : tabDossier_SelectedIndexChanged
    ' Version   : V1.1.0
    ' Date      : 15/10/2026
    '
    ' Rôle      :
    ' Met à jour le fil d'Ariane, la barre de statut et le bouton « Modifier » à chaque changement
    ' d'onglet du hub.
    ' -------------------------------------------------------------------------------------------------
    Private Sub tabDossier_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles tabDossier.SelectedIndexChanged

        AfficherBoutonModifier()
        RafraichirContexteNavigation()

        If _context IsNot Nothing AndAlso tabDossier.SelectedTab IsNot Nothing Then
            Dim code As String = If(_dossier IsNot Nothing, $" du dossier {_dossier.CodeDossier}", String.Empty)
            _context.SetStatus($"Onglet « {tabDossier.SelectedTab.Text} »{code}.")
        End If

    End Sub

    '     -------------------------------------------------------------------------------------------------
    '     Fonction : ObtenirLibelleTarif
    '     Version   : V1.0.0
    '     Date      : 21/07/2026
    '
    '     Rôle      :
    '     Récupère le libellé d'un tarif de séance par son identifiant.
    '
    '     Paramètres :
    '     - idPrixSeance : Identifiant du tarif à rechercher
    '
    '     Retour    :
    '     Libellé du tarif (montant + unité) ou chaîne vide si non trouvé.
    '     -------------------------------------------------------------------------------------------------
    Private Function ObtenirLibelleTarif(idPrixSeance As ULong) As String

        Try
            Dim tarifsList As List(Of PrixSeance) = GestionPrixSeance.GetPrixSeanceActifs()
            Dim tarif As PrixSeance = tarifsList.FirstOrDefault(Function(t) CULng(t.IdPrixSeance) = idPrixSeance)

            If tarif IsNot Nothing Then
                Return String.Format("{0} € ({1})", tarif.Montant.ToString("F2"), tarif.LibellePrixSeance)
            End If

        Catch ex As Exception
            GestionLog.EcrireException("ObtenirLibelleTarif", ex)
        End Try

        Return ""

    End Function

    '     -------------------------------------------------------------------------------------------------
    '     Procédure : ChargerTarifEtMontantsDus
    '     Version   : V2.1.0
    '     Date      : 22/10/2026
    '
    '     Rôle      :
    '     Charge et affiche le tarif par défaut du patient, les montants dus, le prévu et l'avoir.
    '
    '     Remarques :
    '     - V2.0.0 (décision de Joëlle du 17/10/2026) : l'avoir est celui du patient, tous dossiers
    '       confondus (il sert dans n'importe lequel de ses dossiers), avec la part de ce dossier ;
    '       les montants dus sont ceux du dossier, suivis du total du patient s'il diffère. Calcul
    '       unique GestionSeances.CalculerSoldesPatient (règles des grilles : séance due et passée),
    '       qui remplace l'ancien calcul par codes de statut écrits en dur (REALISEE / ABSENTE).
    '     - Détail par dossier en infobulle.
    '     - Affichage des montants au format monétaire (€ / montant local).
    '     - Un montant dû non nul est mis en évidence avec la couleur sémantique « Dû » (UITheme).
    '     - Le patient est celui déjà chargé par RemplirIdentitePatient (pas de relecture en base).
    '     - V2.1.0 (22/10/2026, demande de Joëlle) : libellés explicites (« 0 € ici · 68,00 € tous
    '       dossiers » au lieu de « 0 € · patient : 68,00 € ») ; ligne « Prévu » : reste à payer des
    '       séances dues pas encore passées (payées en partie ou pas du tout), qui n'est pas un dû —
    '       il n'apparaissait que dans le résumé de l'onglet Séances.
    '     -------------------------------------------------------------------------------------------------
    Private Sub ChargerTarifEtMontantsDus()

        Const Absent As String = "—"

        Try

            ' Réinitialiser les affichages
            lblTarifPatientValeur.Text = Absent
            lblMontantsDusValeur.Text = Absent
            lblPrevuValeur.Text = Absent
            MettreEnEvidenceMontantDu(False)
            lblAvoirLegende.Visible = False
            lblAvoirValeur.Visible = False

            ' Charger le tarif du patient
            If _patient IsNot Nothing AndAlso _patient.IdPrixSeanceDefaut.HasValue Then
                ' Récupérer le libellé du tarif (à partir de ref_prix_seance)
                Dim tarifLibelle As String = ObtenirLibelleTarif(_patient.IdPrixSeanceDefaut.Value)
                If Not String.IsNullOrWhiteSpace(tarifLibelle) Then
                    lblTarifPatientValeur.Text = tarifLibelle
                End If
            End If

            ' Montants dus et avoir (uniquement en consultation de dossier persisté)
            If _idDossier > 0 Then
                Dim soldes As List(Of SoldeDossier) = GestionSeances.CalculerSoldesPatient(_idPatient)
                Dim ici As SoldeDossier = soldes.FirstOrDefault(Function(s) s.IdDossier = _idDossier)

                ' Montants dus et prévu : ce dossier, puis le total du patient s'il diffère.
                Dim duIci As Decimal = If(ici Is Nothing, 0D, ici.MontantExigible)
                lblMontantsDusValeur.Text = MontantIciEtPatient(duIci, soldes.Sum(Function(s) s.MontantExigible))
                MettreEnEvidenceMontantDu(duIci > 0)
                _context?.SetToolTip(lblMontantsDusValeur, DetailParDossier(soldes, Function(s) s.MontantExigible, "Montants dus (séances passées)"))

                lblPrevuValeur.Text = MontantIciEtPatient(If(ici Is Nothing, 0D, ici.MontantPrevu), soldes.Sum(Function(s) s.MontantPrevu))
                _context?.SetToolTip(lblPrevuValeur, DetailParDossier(soldes, Function(s) s.MontantPrevu, "Prévu (séances à venir, pas encore dues)"))

                ' Avoir du patient (tous dossiers), affiché seulement s'il existe ; la part de ce
                ' dossier est précisée quand elle diffère du total.
                Dim avoirIci As Decimal = If(ici Is Nothing, 0D, ici.Avoir)
                Dim avoirPatient As Decimal = soldes.Sum(Function(s) s.Avoir)
                Dim aAvoir As Boolean = avoirPatient > 0D
                lblAvoirLegende.Visible = aAvoir
                lblAvoirValeur.Visible = aAvoir
                If aAvoir Then
                    Dim precision As String
                    If avoirIci = avoirPatient Then
                        precision = String.Empty
                    ElseIf avoirIci = 0D Then
                        precision = If(soldes.Where(Function(s) s.Avoir > 0D).Count() > 1, " (autres dossiers)", " (autre dossier)")
                    Else
                        precision = $" dont {MontantBandeau(avoirIci)} ici"
                    End If
                    lblAvoirValeur.Text = MontantBandeau(avoirPatient) & precision
                    _context?.SetToolTip(lblAvoirValeur, DetailParDossier(soldes, Function(s) s.Avoir, "Avoir du patient"))
                End If
            End If

        Catch ex As Exception
            GestionLog.EcrireException("ChargerTarifEtMontantsDus", ex)
            lblTarifPatientValeur.Text = Absent
            lblMontantsDusValeur.Text = Absent
            lblPrevuValeur.Text = Absent
            MettreEnEvidenceMontantDu(False)
        End Try

    End Sub

    ' Montant de ce dossier, suivi du total du patient (tous ses dossiers) s'il diffère :
    ' « 0 € ici · 68,00 € tous dossiers ».
    Private Shared Function MontantIciEtPatient(ici As Decimal, patient As Decimal) As String
        If patient = ici Then Return MontantBandeau(ici)
        Return $"{MontantBandeau(ici)} ici · {MontantBandeau(patient)} tous dossiers"
    End Function

    ' Montant du bandeau : « 120,00 € », « 0 € » quand il n'y a rien.
    Private Shared Function MontantBandeau(montant As Decimal) As String
        Return If(montant > 0D, montant.ToString("F2") & " €", "0 €")
    End Function

    ' Infobulle du bandeau : une ligne par dossier concerné (« DO000028 : 15,00 € »).
    Private Shared Function DetailParDossier(soldes As List(Of SoldeDossier),
                                             montant As Func(Of SoldeDossier, Decimal),
                                             titre As String) As String
        Dim lignes As List(Of String) = soldes.
            Where(Function(s) montant(s) > 0D).
            Select(Function(s) $"{s.CodeDossier} : {MontantBandeau(montant(s))}").
            ToList()
        If lignes.Count = 0 Then Return String.Empty
        Return titre & " par dossier :" & Environment.NewLine & String.Join(Environment.NewLine, lignes)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreEnEvidenceMontantDu
    ' Version   : V1.0.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Applique (ou retire) la mise en évidence « somme exigible » sur la ligne Montants dus.
    ' -------------------------------------------------------------------------------------------------
    Private Sub MettreEnEvidenceMontantDu(estDu As Boolean)

        lblMontantsDusValeur.ForeColor = If(estDu, UITheme.StatutDuFore, UITheme.ColorGrisFonce)
        If lblMontantsDusValeur.Font.Bold <> estDu Then
            lblMontantsDusValeur.Font = New Font(lblMontantsDusValeur.Font, If(estDu, FontStyle.Bold, FontStyle.Regular))
        End If
        lblMontantsDusLegende.ForeColor = If(estDu, UITheme.StatutDuFore, UITheme.ColorSaugeFonce)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherQuotaInami
    ' Version   : V1.1.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Affiche, pour un dossier relevant du circuit INAMI (Realism), le nombre de séances de soutien
    ' et de traitement consommées sur la période annuelle en cours.
    '
    ' Paramètres :
    ' - quota : État du quota, ou Nothing si aucune séance n'est chargée
    '
    ' Remarques :
    ' - L'assujettissement vient du domaine (soumis_quota) et non de la présence de séances : un
    '   dossier encore vide d'un domaine à quota l'affiche donc déjà comme tel.
    ' - Le titre du bloc nomme l'organisme dont dépend le domaine (« QUOTAS — INAMI ») : les quotas
    '   sont imposés par un organisme, l'INAMI n'étant aujourd'hui qu'un cas parmi d'autres possibles.
    ' - Hors quota, le bloc affiche « Domaine sans quota » et masque compteurs et jauge.
    ' - La jauge visualise le total face au plafond global ; elle reste vide sans règle paramétrée.
    ' - Le code couleur alerte l'utilisateur à l'approche du plafond puis à son dépassement, car
    '   les séances excédentaires ne sont plus remboursées et passent à charge du patient.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherQuotaInami(quota As QuotaInamiInfo)

        Dim soumisQuota As Boolean =
            If(_domaineDossier IsNot Nothing, _domaineDossier.SoumisQuota, quota IsNot Nothing AndAlso quota.EstDomaineInami)
        Dim visible As Boolean = soumisQuota AndAlso quota IsNot Nothing AndAlso quota.EstDomaineInami

        lblRecapTitreQuotas.Text = TitreBlocQuotas(soumisQuota)
        lblQuotaAucun.Text = If(soumisQuota, "Aucune séance comptabilisée", "Domaine sans quota")

        lblQuotaSoutienLegende.Visible = visible
        lblQuotaSoutienValeur.Visible = visible
        lblQuotaTraitementLegende.Visible = visible
        lblQuotaTraitementValeur.Visible = visible
        lblQuotaPeriodeInami.Visible = visible
        jaugeQuota.Visible = visible
        lblQuotaAucun.Visible = Not visible

        If Not visible Then Return

        ' Ligne 1 : une entrée par catégorie paramétrée ; ligne 2 : total face au plafond global.
        Dim etats As List(Of QuotaCategorieEtat) = quota.Categories

        lblQuotaSoutienLegende.Text = "Quota"
        lblQuotaSoutienValeur.Text = quota.LibelleCompteurs
        lblQuotaTraitementLegende.Text = "Total"
        lblQuotaTraitementValeur.Text =
            If(quota.ARegle, $"{quota.NbTotal} / {quota.PlafondTotal}", $"{quota.NbTotal} / ? (aucune règle)")

        jaugeQuota.Maximum = If(quota.ARegle, quota.PlafondTotal, 0)
        jaugeQuota.SeuilAlerte = quota.SeuilAlerteTotal
        jaugeQuota.Valeur = quota.NbTotal

        ' Les compteurs ne valent que pour une période donnée : sans ce rappel, l'utilisateur ne peut
        ' pas relier les chiffres du bandeau aux séances visibles dans la grille, qui peuvent relever
        ' de la période suivante.
        If String.IsNullOrEmpty(quota.LibellePeriodeCourante) Then
            lblQuotaPeriodeInami.Text = "Aucune séance remboursée"
        Else
            lblQuotaPeriodeInami.Text = quota.LibellePeriodeCourante
        End If

        ' Rouge dès qu'une catégorie ou le plafond global est atteint.
        lblQuotaSoutienValeur.ForeColor = CouleurQuota(etats.Any(Function(c) c.Epuise), quota.EnAlerte)
        lblQuotaTraitementValeur.ForeColor = CouleurQuota(quota.QuotaAtteint OrElse Not quota.ARegle, quota.EnAlerte)

        Dim infobulle As String = quota.LibelleSynthese
        If _context IsNot Nothing Then
            _context.SetToolTip(lblQuotaSoutienValeur, infobulle)
            _context.SetToolTip(lblQuotaTraitementValeur, infobulle)
            _context.SetToolTip(lblQuotaPeriodeInami, infobulle)
            _context.SetToolTip(jaugeQuota, infobulle)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : TitreBlocQuotas
    ' Version   : V1.0.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Compose le titre du bloc Quotas : « QUOTAS — <ORGANISME> » pour un domaine à quota rattaché à
    ' un organisme, « QUOTAS » sinon.
    ' -------------------------------------------------------------------------------------------------
    Private Function TitreBlocQuotas(soumisQuota As Boolean) As String

        Const Titre As String = "QUOTAS"

        If Not soumisQuota OrElse _domaineDossier Is Nothing OrElse
           String.IsNullOrWhiteSpace(_domaineDossier.LibelleOrganisme) Then
            Return Titre
        End If

        Return $"{Titre} — {_domaineDossier.LibelleOrganisme.Trim().ToUpperInvariant()}"

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerDomaineDossier
    ' Version   : V1.0.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Charge le domaine du dossier (organisme, soumission au quota) pour le bloc Quotas.
    '
    ' Remarques :
    ' - Lu par identifiant et non dans la liste des domaines actifs : un dossier ancien peut relever
    '   d'un domaine désactivé depuis.
    ' - En cas d'échec, le bloc retombe sur l'information portée par les séances (journalisé).
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerDomaineDossier()

        _domaineDossier = Nothing
        If _dossier Is Nothing OrElse _dossier.IdDomaine <= 0 Then Return

        Try
            _domaineDossier = GestionDomaines.GetDomaineParId(CULng(_dossier.IdDomaine))
        Catch ex As Exception
            GestionLog.EcrireLog(
                $"Erreur chargement du domaine du dossier (UC_DossierFiche, idDomaine={_dossier.IdDomaine}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : CouleurQuota
    ' Version   : V1.0.0
    ' Date      : 19/09/2026
    '
    ' Rôle      :
    ' Détermine la couleur d'affichage d'un compteur de quota INAMI selon son niveau de criticité.
    ' -------------------------------------------------------------------------------------------------
    Private Function CouleurQuota(plafondAtteint As Boolean, enAlerte As Boolean) As Color

        If plafondAtteint Then
            Return Color.FromArgb(192, 0, 0)
        ElseIf enAlerte Then
            Return Color.FromArgb(200, 100, 0)
        Else
            Return SystemColors.ControlText
        End If

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplirFicheDossier
    ' Version   : V1.1.0
    ' Date      : 20/10/2026
    '
    ' Rôle      :
    ' Renseigne les contrôles de saisie de l'onglet « Fiche Dossier » à partir du dossier chargé,
    ' ou applique les valeurs par défaut en création.
    '
    ' Remarques :
    ' - Modalités de paiement proposées en création : paramètre DOSSIER_MODALITE_PAIEMENT_DEFAUT
    '   (« À la séance » à la livraison ; texte libre, modifiable dans le dossier).
    ' - V1.1.0 (20/10/2026) : modalités de paiement proposées : paramètre DOSSIER_MODALITE_PAIEMENT_DEFAUT.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RemplirFicheDossier()

        If _dossier Is Nothing Then

            cboDomaine.SelectedIndex = -1
            dtpDateOuverture.Valeur = Date.Today
            txtModalitesPaiement.Text = GestionParametres.GetTexte(ClesParametres.DossierModalitePaiementDefaut,
                                                                   ClesParametres.DefautDossierModalitePaiement)
            RemplirApercusZones()
            Return

        End If

        SelectionnerDomaine(_dossier.IdDomaine)
        dtpDateOuverture.Valeur = _dossier.DateOuverture
        txtModalitesPaiement.Text = _dossier.ModalitesPaiement
        RemplirApercusZones()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SelectionnerDomaine
    ' Version   : V1.0.0
    ' Date      : 21/07/2026
    '
    ' Rôle      :
    ' Sélectionne dans la combo le domaine correspondant à l'identifiant fourni.
    '
    ' Paramètres :
    ' - idDomaine : Identifiant du domaine à sélectionner
    ' -------------------------------------------------------------------------------------------------
    Private Sub SelectionnerDomaine(idDomaine As Long)

        Dim domaines As List(Of Domaine) = TryCast(cboDomaine.DataSource, List(Of Domaine))

        If domaines Is Nothing Then
            Return
        End If

        Dim index As Integer = domaines.FindIndex(
            Function(d) Convert.ToInt64(d.IdDomaine) = idDomaine)

        cboDomaine.SelectedIndex = index

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : cboDomaine_SelectedIndexChanged
    ' Version   : V1.0.0
    ' Date      : 06/10/2026
    '
    ' Rôle      :
    ' Affiche la synthèse du domaine choisi (tarifs, natures, quota) dans l'encadré de droite, afin
    ' que l'utilisateur choisisse le domaine du dossier en connaissance de cause.
    ' -------------------------------------------------------------------------------------------------
    Private Sub cboDomaine_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboDomaine.SelectedIndexChanged

        ucSyntheseDomaine.AfficherDomaine(TryCast(cboDomaine.SelectedItem, Domaine), inclureCompletude:=False)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : cboDomaine_DrawItem
    ' Version   : V1.0.0
    ' Date      : 20/10/2026
    '
    ' Rôle      : Logo du domaine (miniature) devant son libellé, dans la liste ouverte et le champ.
    ' Remarques : La place du logo est gardée pour un domaine qui n'en a pas : les libellés restent
    '             alignés. Couleurs du kit comme cboFiltreSuivi (UC_PatientHome).
    ' -------------------------------------------------------------------------------------------------
    Private Sub cboDomaine_DrawItem(sender As Object, e As DrawItemEventArgs) Handles cboDomaine.DrawItem

        If e.Index < 0 Then Return

        Dim champFerme As Boolean = (e.State And DrawItemState.ComboBoxEdit) = DrawItemState.ComboBoxEdit
        Dim pointe As Boolean = Not champFerme AndAlso (e.State And DrawItemState.Selected) = DrawItemState.Selected

        Using pinceau As New SolidBrush(If(champFerme, e.BackColor, If(pointe, UITheme.ColorGrisVertClair, Color.White)))
            e.Graphics.FillRectangle(pinceau, e.Bounds)
        End Using

        Const marge As Integer = 4
        Dim item As Object = cboDomaine.Items(e.Index)
        Dim dom As Domaine = TryCast(item, Domaine)
        Dim zoneLogo As New Rectangle(e.Bounds.Left + marge, e.Bounds.Top + (e.Bounds.Height - HAUTEUR_LOGO_LISTE) \ 2,
                                      LARGEUR_LOGO_LISTE, HAUTEUR_LOGO_LISTE)

        If dom IsNot Nothing Then
            LogosDomaines.Dessiner(e.Graphics, LogosDomaines.Miniature(dom.IdDomaine, dom.LogoFichier, LARGEUR_LOGO_LISTE, HAUTEUR_LOGO_LISTE), zoneLogo)
        End If

        Dim xTexte As Integer = zoneLogo.Right + marge
        Dim zoneTexte As New Rectangle(xTexte, e.Bounds.Top, Math.Max(0, e.Bounds.Right - xTexte - 2), e.Bounds.Height)
        Dim couleur As Color = If(champFerme, e.ForeColor, If(pointe, UITheme.ColorSaugeTresFonce, UITheme.ColorGrisFonce))

        TextRenderer.DrawText(e.Graphics, cboDomaine.GetItemText(item), cboDomaine.Font, zoneTexte, couleur,
                              TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.SingleLine Or
                              TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPadding)

    End Sub

    ' Logo du domaine dans le bandeau : à droite du bloc Dossier, centré sous « Changer le statut… »
    ' (dont le libellé est centré dans le bouton), à sa taille réelle (cadre 120 × 52) ; masqué sans logo.
    Private Sub AfficherLogoBandeau(idDomaine As Long, logoFichier As String)

        Dim logo As Image = If(idDomaine > 0,
                               LogosDomaines.Miniature(CULng(idDomaine), logoFichier, LARGEUR_LOGO_BANDEAU, HAUTEUR_LOGO_BANDEAU),
                               Nothing)
        picLogoDomaine.Image = logo
        picLogoDomaine.Visible = logo IsNot Nothing
        If logo Is Nothing Then Return

        picLogoDomaine.Size = logo.Size
        picLogoDomaine.Location = New Point(btnChangerStatut.Left + (btnChangerStatut.Width - logo.Width) \ 2, lblStatutValeur.Top)
        picLogoDomaine.BringToFront()

    End Sub

#End Region

#Region "Récapitulatif patient"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplirRecapPatient
    ' Version   : V1.0.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Renseigne le bloc « Patient » du récapitulatif : photo (ou initiales), nom, date de naissance
    ' avec l'âge, et la note d'alerte de la fiche patient lorsqu'elle existe.
    '
    ' Remarques :
    ' - L'alerte reprend AlerteTxt tel quel ; le texte complet reste lisible en info-bulle quand
    '   il est tronqué par l'ellipse du libellé.
    ' - Sans photo, picRecapPhoto_Paint dessine les initiales sur le fond beige (cf. maquette RecapA).
    ' -------------------------------------------------------------------------------------------------
    Private Sub RemplirRecapPatient()

        If _patient Is Nothing Then
            RemplacerPhotoRecap(Nothing)
            lblRecapNaissance.Text = "—"
            lblRecapAlerte.Visible = False
            Return
        End If

        RemplacerPhotoRecap(CheminsPatientHelper.ChargerImagePatient(_patient.CodePatient, _patient.PhotoFichier))

        Dim naissance As String
        If _patient.DateNaissance.HasValue Then
            Dim age As Integer = Patient.CalculerAge(_patient.DateNaissance.Value, Date.Today)
            naissance = $"Né(e) le {_patient.DateNaissance.Value:dd/MM/yyyy} · {age} an{If(age > 1, "s", String.Empty)}"
        Else
            naissance = "Date de naissance non renseignée"
        End If
        lblRecapNaissance.Text = $"{_patient.NomComplet}{Environment.NewLine}{naissance}"

        lblRecapAlerte.Visible = _patient.AAlerte
        If _patient.AAlerte Then
            lblRecapAlerte.Text = $"Alerte : {_patient.AlerteTxt.Trim()}"
            If _context IsNot Nothing Then
                _context.SetToolTip(lblRecapAlerte, _patient.AlerteTxt.Trim())
            End If
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplacerPhotoRecap
    ' Version   : V1.0.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Remplace l'image du récapitulatif en libérant la précédente.
    '
    ' Remarques :
    ' - ChargerImagePatient renvoie une copie en mémoire (fichier non verrouillé) : c'est à nous de
    '   la libérer, sinon chaque rechargement du bandeau laisserait un Bitmap orphelin.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RemplacerPhotoRecap(nouvelleImage As Image)

        UtilsImages.RemplacerImage(picRecapPhoto, nouvelleImage)
        ' Sans photo, les initiales sont redessinées (picRecapPhoto_Paint).
        picRecapPhoto.Invalidate()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : picRecapPhoto_Paint
    ' Version   : V1.0.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Dessine les initiales du patient lorsque aucune photo n'est disponible.
    ' -------------------------------------------------------------------------------------------------
    Private Sub picRecapPhoto_Paint(sender As Object, e As PaintEventArgs) Handles picRecapPhoto.Paint

        If picRecapPhoto.Image IsNot Nothing OrElse _patient Is Nothing Then Return

        Dim initiales As String =
            (InitialeDe(_patient.Prenom) & InitialeDe(_patient.Nom)).ToUpperInvariant()
        If initiales.Length = 0 Then Return

        Using police As New Font(lblRecapNaissance.Font.FontFamily, 13.0F, FontStyle.Bold)
            TextRenderer.DrawText(
                e.Graphics, initiales, police, picRecapPhoto.ClientRectangle, UITheme.ColorMauveFonce,
                TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.NoPadding)
        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : InitialeDe
    ' Version   : V1.0.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Retourne la première lettre d'un prénom ou d'un nom (chaîne vide si absent).
    ' -------------------------------------------------------------------------------------------------
    Private Shared Function InitialeDe(texte As String) As String
        Return If(String.IsNullOrWhiteSpace(texte), String.Empty, texte.Trim().Substring(0, 1))
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : tblRecap_CellPaint
    ' Version   : V1.0.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Trace un filet vertical discret entre les quatre blocs du récapitulatif.
    '
    ' Remarques :
    ' - Un CellBorderStyle du TableLayoutPanel encadrerait aussi l'extérieur et le haut/bas des
    '   cellules : on ne veut que les séparations internes, comme dans la maquette.
    ' -------------------------------------------------------------------------------------------------
    Private Sub tblRecap_CellPaint(sender As Object, e As TableLayoutCellPaintEventArgs) Handles tblRecap.CellPaint

        If e.Column >= tblRecap.ColumnCount - 1 Then Return

        Dim x As Integer = e.CellBounds.Right - 1
        Using stylo As New Pen(UITheme.ColorBeige)
            e.Graphics.DrawLine(stylo, x, e.CellBounds.Top + 6, x, e.CellBounds.Bottom - 6)
        End Using

    End Sub

#End Region

#Region "Modes"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerMode
    ' Version   : V1.3.0
    ' Date      : 19/10/2026
    '
    ' Rôle      :
    ' Applique le mode courant (consultation / création / modification) : active ou non la saisie
    ' et ajuste la visibilité des boutons d'action.
    '
    ' Paramètres :
    ' - mode : Mode à appliquer
    ' - V1.3.0 (19/10/2026) : photo des paramètres en création / modification (GardeModifications).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerMode(mode As ModeFiche)

        _mode = mode

        Dim editable As Boolean = mode = ModeFiche.Creation OrElse mode = ModeFiche.Modification

        ' Consultation lisible (Rules) : la carte des paramètres reste lisible hors saisie.
        UtilsControls.DefinirLectureSeule(carteParametres, Not editable)

        AfficherBoutonModifier()
        btnEnregistrer.Visible = editable
        btnAnnuler.Visible = editable

        ' Le changement de statut n'est possible qu'en consultation d'un dossier déjà persisté.
        btnChangerStatut.Visible = mode = ModeFiche.Consultation AndAlso _dossier IsNot Nothing AndAlso _idDossier > 0

        ' Les zones cliniques ne sont éditables que pour un dossier déjà persisté (pas en création).
        ActiverBoutonsZones(_dossier IsNot Nothing AndAlso _idDossier > 0)

        ' En création, l'onglet Fiche Dossier est prioritaire pour la saisie
        If mode = ModeFiche.Creation Then
            tabDossier.SelectedTab = tabPageFicheDossier
        End If

        ' Création / modification : photo des champs une fois remplis ; consultation : rien à perdre.
        If editable Then
            _garde?.PhotographierApresChargement()
        Else
            _garde?.Oublier()
        End If

        RafraichirContexteNavigation()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherBoutonModifier
    ' Version   : V1.1.0
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' « Modifier » ne concerne que la fiche du dossier (onglet Fiche Dossier) : il n'est montré qu'en
    ' consultation et sur cet onglet. Les autres onglets ont leurs propres actions (barres locales
    ' Séances et Paiements, lignes du Suivi clinique, Documents).
    '
    ' Remarques :
    ' - En saisie, Enregistrer et Annuler restent visibles sur tous les onglets : une modification en
    '   cours de la fiche doit toujours pouvoir être enregistrée ou abandonnée.
    ' - V1.1.0 (22/10/2026, audit C.1) : dossier archivé → bouton inactif, raison au survol.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherBoutonModifier()

        btnModifier.Visible = _mode = ModeFiche.Consultation AndAlso
                              tabDossier.SelectedTab Is tabPageFicheDossier

        Dim raison As String = If(_dossier Is Nothing, String.Empty,
            GestionDossiers.RaisonDossierFige(_dossier.CodeStatutDossier, GestionDossiers.ElementDossier.FicheEtZones))
        btnModifier.DefinirDisponibilite(raison.Length = 0, raison)

    End Sub

#End Region

#Region "Édition"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnModifier_Click
    ' Version   : V1.0.0
    ' Date      : 21/07/2026
    '
    ' Rôle      :
    ' Passe la fiche dossier en mode modification.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnModifier_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnModifier.Click

        AppliquerMode(ModeFiche.Modification)
        _context?.SetStatus($"Modification de la fiche du dossier {_dossier?.CodeDossier}.")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAnnuler_Click
    ' Version   : V1.1.0
    ' Date      : 19/10/2026
    '
    ' Rôle      :
    ' Annule la saisie en cours : recharge les valeurs d'origine (modification) ou revient à la
    ' fiche patient (création).
    ' - V1.1.0 (19/10/2026) : en modification, Abandonner / Continuer la saisie s'il y a des modifications.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAnnuler_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnAnnuler.Click

        If _mode = ModeFiche.Creation Then
            RetournerFichePatient()
            Return
        End If

        If _garde IsNot Nothing AndAlso Not _garde.ConfirmerAnnulation("de la fiche du dossier") Then
            Return
        End If

        RemplirFicheDossier()
        AppliquerMode(ModeFiche.Consultation)
        _context?.SetStatus("Modifications du dossier annulées.")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnEnregistrer_Click
    ' Version   : V1.1.0
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' Valide la saisie puis crée (mode création) ou met à jour (mode modification) le dossier.
    ' - V1.1.0 (22/10/2026, audit C.1) : refus du métier (dossier archivé) et échec affichés dans une
    '   fenêtre (ils ne restaient que dans la barre d'état).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEnregistrer_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnEnregistrer.Click

        If Not ValiderSaisie() Then
            Return
        End If

        Try

            If _mode = ModeFiche.Creation Then
                CreerDossier()
            Else
                MettreAJourDossier()
            End If

        Catch exRefus As InvalidOperationException

            ' Refus métier (message rédigé pour l'utilisatrice).
            DialogChoix.Information(exRefus.Message, "Enregistrement impossible")
            _context?.SetStatus("Enregistrement du dossier refusé.")

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur enregistrement du dossier (UC_DossierFiche, id={_idDossier}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur(
                "Le dossier n'a pas pu être enregistré ; rien n'a été modifié." & Environment.NewLine &
                "Vérifiez la connexion à la base de données, puis cliquez à nouveau sur Enregistrer.",
                "Enregistrement impossible")
            _context?.SetStatus("Erreur lors de l'enregistrement du dossier.")

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ValiderSaisie
    ' Version  : V1.0.0
    ' Date     : 21/07/2026
    '
    ' Rôle     :
    ' Vérifie que les champs obligatoires sont renseignés avant enregistrement.
    '
    ' Retour   :
    ' - Boolean : True si la saisie est valide, False sinon (message d'état positionné).
    ' -------------------------------------------------------------------------------------------------
    Private Function ValiderSaisie() As Boolean

        _context?.ClearError(cboDomaine)

        If cboDomaine.SelectedValue Is Nothing OrElse cboDomaine.SelectedIndex < 0 Then

            If _context IsNot Nothing Then
                _context.SetError(cboDomaine, "Le domaine est obligatoire.")
                _context.SetStatus("Veuillez sélectionner un domaine.")
            End If

            cboDomaine.Focus()
            Return False

        End If

        Return True

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : CreerDossier
    ' Version   : V1.0.0
    ' Date      : 21/07/2026
    '
    ' Rôle      :
    ' Crée un nouveau dossier à partir de la saisie ; le statut est forcé à Actif par la couche
    ' métier et le code est généré par la base.
    ' -------------------------------------------------------------------------------------------------
    Private Sub CreerDossier()

        Dim nouveau As New Dossier With {
            .IdPatient = _idPatient,
            .IdDomaine = Convert.ToInt64(cboDomaine.SelectedValue),
            .DateOuverture = dtpDateOuverture.Valeur.Value,
            .ModalitesPaiement = txtModalitesPaiement.Text.Trim()
        }

        Dim idCree As Long = GestionDossiers.CreateDossier(nouveau)

        If idCree <= 0 Then

            If _context IsNot Nothing Then
                _context.SetStatus("La création du dossier a échoué.")
            End If

            Return

        End If

        _idDossier = idCree
        _dossier = GestionDossiers.GetDossier(idCree)
        _idPatient = _dossier.IdPatient
        _mode = ModeFiche.Consultation

        RemplirBandeau()
        RemplirFicheDossier()
        AppliquerMode(ModeFiche.Consultation)

        ' Le dossier vient de passer de Création à Consultation : les grilles n'ont jamais été
        ' initialisées (ChargerDossier sortait avant en mode Création). On les initialise ici pour
        ' que le style et le tri soient présents dès l'ajout de la première séance ou du premier paiement.
        InitialiserGrilleSeances()
        InitialiserGrillePaiements()
        ChargerSeances()

        If _context IsNot Nothing Then
            _context.SetStatus($"Dossier {_dossier.CodeDossier} créé.")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreAJourDossier
    ' Version   : V1.1.0
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' Met à jour l'identité du dossier courant à partir de la saisie.
    '
    ' Remarques :
    ' - V1.1.0 (22/10/2026, audit B.6, test TQ-25) : si le domaine change, les séances sont relues
    '   (soumission au quota, catégories des natures, règle) ; le bloc Quotas du bandeau gardait
    '   l'ancien domaine jusqu'à la réouverture du dossier.
    ' -------------------------------------------------------------------------------------------------
    Private Sub MettreAJourDossier()

        Dim domaineChange As Boolean = _dossier.IdDomaine <> Convert.ToInt64(cboDomaine.SelectedValue)

        _dossier.IdDomaine = Convert.ToInt64(cboDomaine.SelectedValue)
        _dossier.DateOuverture = dtpDateOuverture.Valeur.Value
        _dossier.ModalitesPaiement = txtModalitesPaiement.Text.Trim()

        GestionDossiers.UpdateDossier(_dossier)

        _dossier = GestionDossiers.GetDossier(_idDossier)
        _mode = ModeFiche.Consultation

        RemplirBandeau()
        RemplirFicheDossier()
        AppliquerMode(ModeFiche.Consultation)

        If domaineChange Then
            ChargerSeances()
        End If

        If _context IsNot Nothing Then
            _context.SetStatus($"Dossier {_dossier.CodeDossier} mis à jour.")
        End If

    End Sub

#End Region

#Region "Cycle de vie (statut)"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnChangerStatut_Click
    ' Version   : V1.2.0
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' Ouvre la modale de changement de statut (cycle de vie, brique B6), applique la transition
    ' retenue via la couche métier, puis recharge le dossier pour rafraîchir le bandeau et les zones.
    '
    ' Remarques :
    ' - Ne fait rien si aucun dossier n'est chargé.
    ' - Une confirmation utilisateur est demandée avant d'appliquer la transition.
    ' - V1.1.0 (décision du 17/10/2026) : la clôture est refusée tant que des séances sont à venir
    '   (message qui les cite et dit quoi faire) ; s'il reste des montants à régler, la confirmation
    '   les signale dans un encadré et le bouton dit « Clôturer quand même ». Un refus métier de
    '   GestionDossiers (InvalidOperationException) est montré tel quel : son message s'adresse à
    '   l'utilisatrice.
    ' - V1.2.0 (22/10/2026, audit C.1) : après la transition, « Modifier » et les boutons des
    '   paiements sont réalignés eux aussi (seuls ceux des séances l'étaient).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnChangerStatut_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnChangerStatut.Click

        If _dossier Is Nothing OrElse _idDossier <= 0 Then
            Return
        End If

        Try

            Using editeur As New DossierStatutEditor(_dossier.CodeStatutDossier, _dossier.LibelleStatutDossier, _dossier.CodeDossier)

                editeur.SetContext(_context)
                SignalerAction("Changement de statut", $"Changement de statut du dossier {_dossier.CodeDossier} (actuel : {_dossier.LibelleStatutDossier}).")

                If editeur.ShowDialog(FindForm()) <> DialogResult.OK Then
                    RafraichirContexteNavigation()
                    _context?.SetStatus("Changement de statut abandonné.")
                    Return
                End If

                Dim codeCible As String = editeur.CodeStatutCible
                Dim dateCloture As Date? = editeur.DateClotureChoisie

                If String.IsNullOrWhiteSpace(codeCible) Then
                    Return
                End If

                ' Clôture : contrôlée avant toute confirmation (séances à venir = refus ; montants à
                ' régler = avertissement).
                Dim avertissementMontants As String = String.Empty

                If String.Equals(codeCible, CodesSysteme.StatutsDossier.Cloture, StringComparison.OrdinalIgnoreCase) Then

                    Dim controle As ControleClotureDossier = GestionDossiers.ControlerCloture(_idDossier)

                    If controle.EstBloquant Then
                        DialogChoix.Information(controle.MessageBlocage, $"Clôture impossible — {_dossier.CodeDossier}")
                        _context?.SetStatus($"Clôture du dossier {_dossier.CodeDossier} refusée : des séances sont encore à venir.")
                        Return
                    End If

                    If controle.MontantARegler > 0D Then
                        avertissementMontants = controle.AvertissementMontants
                    End If

                End If

                Dim reponse As DialogResult

                If String.Equals(codeCible, CodesSysteme.StatutsDossier.Archive, StringComparison.OrdinalIgnoreCase) Then
                    ' Message spécifique pour l'archivage : transition définitive (dossier ensuite en lecture seule).
                    reponse = DialogChoix.Confirmer(
                        "Vous êtes sur le point d'ARCHIVER définitivement ce dossier." & Environment.NewLine & Environment.NewLine &
                        "Un dossier archivé reste consultable mais ne peut plus être modifié ni réouvert." & Environment.NewLine & Environment.NewLine &
                        "Confirmez-vous cet archivage définitif ?",
                        "Archivage définitif")

                ElseIf Not String.IsNullOrEmpty(avertissementMontants) Then
                    reponse = DialogChoix.Confirmer(
                        $"Confirmez-vous la clôture du dossier {_dossier.CodeDossier} ?",
                        "Clôturer le dossier", "Clôturer quand même", "Annuler",
                        VarianteBouton.Ocre, avertissementMontants)

                Else
                    reponse = DialogChoix.Confirmer(
                        $"Confirmez-vous le changement de statut du dossier de « {_dossier.LibelleStatutDossier} » vers le nouveau statut sélectionné ?",
                        "Changer le statut")
                End If

                If reponse <> DialogResult.Yes Then
                    Return
                End If

                GestionDossiers.ChangerStatutDossier(_idDossier, _dossier.CodeStatutDossier, codeCible, dateCloture)

            End Using

            ' Rechargement complet pour refléter le nouveau statut et les dates (clôture / archivage).
            _dossier = GestionDossiers.GetDossier(_idDossier)
            RemplirBandeau()
            RemplirApercusZones()

            ' Clôture et archivage figent séances, paiements (et la fiche, archivé) : on réaligne les boutons.
            RafraichirEtatBoutonsSeances()
            RafraichirEtatBoutonsPaiements()
            AfficherBoutonModifier()

            If _context IsNot Nothing Then
                _context.SetStatus($"Statut du dossier {_dossier.CodeDossier} mis à jour : {_dossier.LibelleStatutDossier}.")
            End If

        Catch ex As InvalidOperationException

            ' Refus métier (message rédigé pour l'utilisatrice) : il doit se voir, pas seulement dans
            ' la barre d'état.
            DialogChoix.Information(ex.Message, "Changement de statut impossible")
            _context?.SetStatus("Changement de statut refusé.")

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur changement de statut du dossier (UC_DossierFiche, id={_idDossier}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur("Le changement de statut a échoué. Veuillez réessayer.")

        End Try

    End Sub

#End Region

#Region "Résumés cliquables (séances, paiements)"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreAJourResumeSeances
    ' Version   : V1.2.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Reconstruit le résumé au-dessus de la grille des séances, calculé sur toutes les séances du
    ' dossier (pas seulement celles affichées) : nombre de séances à venir, réalisées, absences, non
    ' dues ; montants à régler, prévus et en avoir.
    '
    ' Remarques :
    ' - Une famille ou un montant nul n'a pas de pastille : le résumé ne montre que ce qui existe.
    ' - V1.1.0 : pastilles construites par ResumeFiltre, partagé avec la grille des paiements.
    ' - V1.2.0 (23/10/2026) : résumé construit par FiltresSeances.MettreAJourResume (partagé).
    ' -------------------------------------------------------------------------------------------------
    Private Sub MettreAJourResumeSeances()

        If _resumeSeances Is Nothing Then Return

        FiltresSeances.MettreAJourResume(_resumeSeances, _seances)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreAJourResumePaiements
    ' Version   : V1.2.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Reconstruit le résumé au-dessus de la grille des paiements, calculé sur tout le dossier : total
    ' encaissé, total en attente, avoir disponible et reste à régler.
    '
    ' Remarques :
    ' - Avoir et reste à régler viennent des séances (SeanceListeItem.AvoirSeance / MontantExigible) :
    '   mêmes montants que le résumé de l'onglet Séances et que l'avoir du bandeau du dossier.
    ' - « En attente » et « Reste à régler » se recoupent : un paiement attendu pour une séance passée
    '   compte dans les deux (il est attendu, et en retard).
    ' - Les montants sont ceux affectés aux séances du dossier : un paiement réparti ou un avoir
    '   réutilisé n'est compté qu'une fois (l'avoir utilisé quitte la ligne d'origine).
    ' - V1.2.0 (23/10/2026) : résumé construit par FiltresPaiements.MettreAJourResume (partagé).
    ' -------------------------------------------------------------------------------------------------
    Private Sub MettreAJourResumePaiements()

        If _resumePaiements Is Nothing Then Return

        FiltresPaiements.MettreAJourResume(_resumePaiements, _paiements, _seances)

    End Sub

    ' Infobulle des pastilles de résumé (ResumeFiltre ne connaît pas le contexte de l'écran).
    Private Sub DefinirInfobulle(controle As Control, texte As String)
        _context?.SetToolTip(controle, texte)
    End Sub

#End Region

#Region "Navigation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnFermer_Click
    ' Version   : V1.0.0
    ' Date      : 21/07/2026
    '
    ' Rôle      :
    ' Revient à la fiche patient via la mini-pile de navigation (D-Q15).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnFermer_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnFermer.Click

        RetournerFichePatient()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RetournerFichePatient
    ' Version   : V1.0.1
    ' Date      : 25/10/2026
    '
    ' Rôle      :
    ' Demande à Home de revenir à l'écran précédent (la fiche patient) ; à défaut, recharge la
    ' liste des patients.
    ' - V1.0.1 (25/10/2026, audit C.4, ADR-55) : fenêtre principale lue dans le contexte
    '   (IFenetrePrincipale) au lieu de TryCast(FindForm(), Home).
    ' -------------------------------------------------------------------------------------------------
    Private Sub RetournerFichePatient()

        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale

        If fenetre Is Nothing Then
            Return
        End If

        If Not fenetre.NavigateRetour() Then
            fenetre.NavigateToPatients()
        End If

    End Sub



#End Region

#Region "Ouverture ciblée (recherche transversale)"

    ' Onglet et ligne demandés par CiblerAuChargement (Nothing / 0 : aucun).
    Private _ongletCible As OngletDossier?
    Private _idSeanceCible As Long
    Private _idPaiementCible As Long

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerCible
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Affiche l'onglet demandé et sélectionne la séance ou le paiement demandé, puis oublie la cible.
    '
    ' Remarques :
    ' - Appelée après le chargement (BeginInvoke) : l'onglet est affiché d'abord, la liste est
    '   réappliquée (page de la ligne), la ligne est ensuite sélectionnée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerCible()

        If Not _ongletCible.HasValue OrElse IsDisposed Then Return

        Try

            Select Case _ongletCible.Value

                Case OngletDossier.Seances
                    tabDossier.SelectedTab = tabPageSeances
                    If _idSeanceCible > 0 Then
                        AppliquerFiltresSeances()
                        If Not SelectionnerLigne(dgvSeances, Function(o) TypeOf o Is SeanceListeItem AndAlso DirectCast(o, SeanceListeItem).IdSeance = _idSeanceCible) Then
                            _context?.SetStatus("La séance recherchée n'est pas dans la liste affichée (filtres de l'onglet).")
                        End If
                        RafraichirEtatBoutonsSeances()
                    End If

                Case OngletDossier.Paiements
                    tabDossier.SelectedTab = tabPagePaiements
                    If _idPaiementCible > 0 Then
                        AppliquerFiltresPaiements()
                        If Not SelectionnerLigne(dgvPaiements, Function(o) TypeOf o Is PaiementListeItem AndAlso DirectCast(o, PaiementListeItem).IdPaiement = _idPaiementCible) Then
                            _context?.SetStatus("Le paiement recherché n'est pas dans la liste affichée (filtres de l'onglet).")
                        End If
                        RafraichirEtatBoutonsPaiements()
                    End If

                Case Else
                    tabDossier.SelectedTab = tabPageFicheDossier

            End Select

        Catch ex As Exception

            GestionLog.EcrireLog($"Erreur ouverture ciblée du dossier (UC_DossierFiche, id={_idDossier}).",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)

        Finally

            _ongletCible = Nothing
            _idSeanceCible = 0
            _idPaiementCible = 0

        End Try

    End Sub

    ' Sélectionne la première ligne affichée qui répond au critère ; False si aucune.
    Private Shared Function SelectionnerLigne(dgv As DataGridView, critere As Func(Of Object, Boolean)) As Boolean

        For Each ligne As DataGridViewRow In dgv.Rows
            If ligne.Visible AndAlso critere(ligne.DataBoundItem) Then
                Dim premiereColonne As DataGridViewColumn = dgv.Columns.GetFirstColumn(DataGridViewElementStates.Visible)
                If premiereColonne IsNot Nothing Then dgv.CurrentCell = ligne.Cells(premiereColonne.Index)
                dgv.ClearSelection()
                ligne.Selected = True
                Return True
            End If
        Next

        Return False

    End Function

#End Region

#Region "Modifications non enregistrées"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : PeutQuitter
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Avant de quitter l'écran (Home ou fenêtre hôte) : rien à perdre, ou Enregistrer /
    '              Quitter sans enregistrer / Rester, comme partout dans l'application (ADR-43).
    ' -------------------------------------------------------------------------------------------------
    Public Function PeutQuitter() As Boolean Implements IUnsavedChangesAware.PeutQuitter
        Return _garde Is Nothing OrElse _garde.PeutQuitter(If(_mode = ModeFiche.Creation, "du nouveau dossier", "de la fiche du dossier"), AddressOf EnregistrerParLeBouton)
    End Function

    ' « Enregistrer » avant de quitter : même chemin que le bouton (validation, messages) ; True si
    ' l'enregistrement a abouti (l'écran est revenu en consultation).
    Private Function EnregistrerParLeBouton() As Boolean
        btnEnregistrer.PerformClick()
        Return _mode = ModeFiche.Consultation
    End Function

#End Region

End Class
