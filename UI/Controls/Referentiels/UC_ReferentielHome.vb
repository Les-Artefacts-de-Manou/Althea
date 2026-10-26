' -------------------------------------------------------------------------------------------------
' UserControl : UC_ReferentielHome
' Projet      : Althéa
' Version     : V1.7.0
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Hub d'accueil de la section Référentiels (tables ref_*). Présente une tuile par référentiel
' et oriente vers l'écran de gestion correspondant (UC_Pays, UC_Therapeutes, etc.).
'
' Responsabilités :
' - Afficher l'interface d'accueil de la section Référentiels
' - Gérer l'affichage et l'état des boutons selon le rôle utilisateur (SuperUser, Admin)
' - Proposer l'accès aux écrans de gestion des référentiels (Domaines en V1, autres à venir)
' - Gérer l'élévation temporaire des droits d'accès (bouton "Élever accès")
' - Gérer le retour au rôle de base après élévation (bouton "Retour rôle de base")
' - Afficher le rôle courant et l'état de l'élévation (lblRoleCourant, lblElevation)
' - Naviguer vers les écrans de référentiels par la fenêtre principale (Ouvrir)
'
' Remarques   :
' - Chargé dynamiquement dans le panneau central de Home via le mécanisme de navigation
' - Implémente IContextAwareUserControl pour injection du contexte UI partagé
' - Aucune logique métier ni accès direct à la base de données (tout passe par les couches inférieures)
' - La gestion des référentiels est réservée aux rôles SuperUser et Admin (comme l'Administration)
' - Le retour au rôle de base peut déclencher un retour automatique à l'Accueil si le rôle < SuperUser
'
' Dépendances :
' - UserSession (session utilisateur avec élévation)
' - UtilisateurApplication (utilisateur connecté avec RoleMaxElevation)
' - UserControlContext (contexte UI injecté par Home)
' - IFenetrePrincipale (fenêtre principale lue dans le contexte : navigation, fil d'Ariane, bandeau,
'   élévation)
' - ElevationAcces (Form élévation d'accès)
' - UC_ReferentielBase et ses dérivés (écrans de référentiels)
' - UtilsButtons (thématisation des boutons)
' - GestionLog (journalisation)
'
' Interface   :
' - IContextAwareUserControl : Pour injection du contexte UI partagé
'
' Historique  :
' - V1.1.1 (17/10/2026) : tuiles Natures séance (natureSeance_normal) et Prix séance
'   (tarifSeance_normal) sur leurs vrais fichiers ; sans survol ni état désactivé jusque-là.
' - V1.1.2 (18/10/2026) : commentaires à jour (ancien UC_Domaines supprimé, UI-8).
' - V1.2.0 (19/10/2026, éditeur temps 2) : tuile « Textes types » (UC_TextesTypes), groupe Dossier.
' - V1.3.0 (19/10/2026) : tuile « Usages des textes types » (UC_UsagesTextesTypes).
' - V1.4.0 (20/10/2026, v2.46.0) : tuile « Latéralités » (UC_Lateralites) dans le groupe Patient ; pour
'   lui faire place, Civilités rejoint la première ligne (à côté de Pays) et Types de rendez-vous
'   le groupe Séances.
' - V1.5.0 (20/10/2026) : tuile « Domaines » retirée (décision de Joëlle) : Domaines s'ouvre par le
'   menu principal de la Home.
' - V1.6.0 (23/10/2026, v2.55.0) : tuile « Spécialités » (UC_Specialites) dans le groupe Cabinet, à
'   côté de Civilités ; icône des thérapeutes en attendant une icône dédiée.
' - V1.7.0 (25/10/2026, audit C.4 étape 3, ADR-55) : fenêtre principale lue dans le contexte
'   (IFenetrePrincipale) ; ouverture des écrans par Ouvrir (un seul endroit) ; élévation demandée
'   à la fenêtre principale (DemanderElevation) au lieu d'un bloc recopié.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_ReferentielHome
    Implements IContextAwareUserControl

#Region "Variables privées"

    'Injecté par Home via SetContext() (implémentation IContextAwareUserControl)
    Private _context As UserControlContext

    ' Fourni au constructeur, contient CurrentRole et IsElevated
    Private ReadOnly _userSession As UserSession

    'Fourni au constructeur, contient RoleMaxElevation
    Private ReadOnly _utilisateur As UtilisateurApplication

#End Region

#Region "Constructeurs"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version      : V1.0.0
    ' Date         : 09/06/2026
    '
    ' Rôle         :
    ' Constructeur par défaut requis par le WinForms Designer.
    '
    ' Remarques    :
    ' - Ne pas supprimer : obligatoire pour le Designer
    ' - N'initialise pas _userSession ni _utilisateur (seront Nothing)
    ' - En production, utiliser le constructeur surchargé New(userSession, utilisateur)
    '
    ' Exceptions   :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub New()

        InitializeComponent()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version      : V1.0.0
    ' Date         : 09/06/2026
    '
    ' Rôle         :
    ' Initialise UC_ReferentielHome avec la session utilisateur et les données métier de l'utilisateur.
    '
    ' Paramètres   :
    ' - userSession : Session utilisateur courante (contient CurrentRole, IsElevated)
    ' - utilisateur : Données métier de l'utilisateur connecté (contient RoleMaxElevation)
    '
    ' Remarques    :
    ' - Constructeur utilisé en production par Home lors du chargement du UserControl
    ' - _context sera injecté ultérieurement via SetContext()
    '
    ' Exceptions   :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(
        userSession As UserSession,
        utilisateur As UtilisateurApplication
    )

        InitializeComponent()

        _userSession = userSession
        _utilisateur = utilisateur

    End Sub

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetContext
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Injecte le contexte UI partagé fourni par Home (implémentation IContextAwareUserControl).
    '
    ' Paramètres :
    ' - context : Instance de UserControlContext partagée entre Home et tous les UserControls
    '
    ' Remarques  :
    ' - Appelé automatiquement par Home après instanciation du UserControl
    ' - Permet d'accéder aux contrôles partagés : StatusStrip, ErrorProvider, ToolTip
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(
        context As UserControlContext
    ) Implements IContextAwareUserControl.SetContext

        _context = context

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetStatus
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Met à jour le message de la barre d'état via le contexte partagé.
    '
    ' Paramètres :
    ' - message : Texte à afficher dans la StatusStrip de Home
    '
    ' Remarques  :
    ' - Si _context est Nothing, l'appel est ignoré sans erreur
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub SetStatus(
        message As String
    )

        If _context IsNot Nothing Then
            _context.SetStatus(message)
        End If

    End Sub

#End Region

#Region "Gestion des droits"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AppliquerDroitsUtilisateur
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Configure l'affichage et l'état des boutons selon le rôle utilisateur courant et l'état d'élévation.
    '
    ' Responsabilités :
    ' - Afficher le rôle courant et l'état d'élévation (lblRoleCourant, lblElevation)
    ' - Activer/désactiver les tuiles de référentiels selon le rôle (SuperUser ou Admin : autorisés)
    ' - Activer/désactiver le bouton "Élever accès" selon le RoleMaxElevation de l'utilisateur
    ' - Activer/désactiver le bouton "Retour rôle de base" selon l'état d'élévation
    ' - Gérer le cas où _userSession est Nothing (désactiver tout)
    '
    ' Remarques  :
    ' - Appelée au chargement du UserControl (UC_ReferentielHome_Load)
    ' - Appelée après élévation ou retour au rôle de base pour rafraîchir l'affichage
    ' - Stratégie : SuperUser ou Admin -> tuiles disponibles, autres -> rien
    ' - Les référentiels non encore implémentés restent désactivés (à venir)
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerDroitsUtilisateur()

        If _userSession Is Nothing Then

            DesactiverTousLesReferentiels()

            btnEleverAcces.Enabled = False
            btnRetourRoleBase.Enabled = False

            lblRoleCourant.Text =
                "Rôle courant : inconnu"

            lblElevation.Text =
                "Aucune session active"

            SetStatus("Session utilisateur absente.")

            Return

        End If

        lblRoleCourant.Text =
            $"Rôle courant : {_userSession.CurrentRole}"

        If _userSession.IsElevated Then
            lblElevation.Text = "Élévation active"
        Else
            lblElevation.Text = "Élévation inactive"
        End If

        If _utilisateur IsNot Nothing Then
            btnEleverAcces.Enabled =
                _userSession.CurrentRole < _utilisateur.RoleMaxElevation
        Else
            btnEleverAcces.Enabled = False
        End If

        btnRetourRoleBase.Enabled =
            _userSession.IsElevated

        If _userSession.IsSuperUserOrAdmin() Then

            ActiverReferentielsDisponibles()

            SetStatus("Gestion des référentiels.")

        Else

            DesactiverTousLesReferentiels()

            SetStatus("Accès référentiels non autorisé.")

        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ActiverReferentielsDisponibles
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Active les tuiles de tous les référentiels gérés.
    '
    ' Remarques  :
    ' - Appelée uniquement pour les rôles SuperUser et Admin
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub ActiverReferentielsDisponibles()

        btnOrganismes.Enabled = True
        btnLiensPatient.Enabled = True
        btnRolesIntervenant.Enabled = True
        btnSituationsFamiliales.Enabled = True
        btnStatutsDossier.Enabled = True
        btnStatutsSeance.Enabled = True
        btnTypesDocuments.Enabled = True
        btnTypesRendezVous.Enabled = True
        btnRoleLegal.Enabled = True
        btnTherapeutes.Enabled = True
        btnModesPaiement.Enabled = True
        btnPeriodicites.Enabled = True
        btnTextesTypes.Enabled = True
        btnUsagesTextes.Enabled = True
        btnPays.Enabled = True
        btnCivilites.Enabled = True
        btnSpecialites.Enabled = True
        btnLateralites.Enabled = True
        btnPrixSeance.Enabled = True
        btnNaturesSeance.Enabled = True

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DesactiverTousLesReferentiels
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Désactive toutes les tuiles de référentiels.
    '
    ' Remarques  :
    ' - Appelée pour les rôles inférieurs à SuperUser ou si _userSession est Nothing
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub DesactiverTousLesReferentiels()

        btnOrganismes.Enabled = False
        btnLiensPatient.Enabled = False
        btnRolesIntervenant.Enabled = False
        btnSituationsFamiliales.Enabled = False
        btnStatutsDossier.Enabled = False
        btnStatutsSeance.Enabled = False
        btnTypesDocuments.Enabled = False
        btnTypesRendezVous.Enabled = False
        btnRoleLegal.Enabled = False
        btnTherapeutes.Enabled = False
        btnModesPaiement.Enabled = False
        btnPeriodicites.Enabled = False
        btnTextesTypes.Enabled = False
        btnUsagesTextes.Enabled = False
        btnPays.Enabled = False
        btnCivilites.Enabled = False
        btnLateralites.Enabled = False
        btnSpecialites.Enabled = False
        btnPrixSeance.Enabled = False
        btnNaturesSeance.Enabled = False

    End Sub

#End Region

#Region "Initialisation"

    Private Sub InitialiserToolTips()

        If _context Is Nothing Then Exit Sub

        _context.SetToolTip(btnEleverAcces, "Élever les droits de l'utilisateur.")
        _context.SetToolTip(btnRetourRoleBase, "Retourner au rôle de base.")
        _context.SetToolTip(btnOrganismes, "Gérer les organismes de remboursement (ex. INAMI) et les mutualités.")
        _context.SetToolTip(btnLiensPatient, "Gérer les liens patients.")
        _context.SetToolTip(btnRolesIntervenant, "Gérer les rôles des intervenants.")
        _context.SetToolTip(btnSituationsFamiliales, "Gérer les situations familiales.")
        _context.SetToolTip(btnStatutsDossier, "Gérer les statuts des dossiers.")
        _context.SetToolTip(btnStatutsSeance, "Gérer les statuts des séances.")
        _context.SetToolTip(btnTypesDocuments, "Gérer les types de documents.")
        _context.SetToolTip(btnTypesRendezVous, "Gérer les types de rendez-vous.")
        _context.SetToolTip(btnRoleLegal, "Gérer les rôles légaux.")
        _context.SetToolTip(btnTherapeutes, "Gérer les thérapeutes.")
        _context.SetToolTip(btnModesPaiement, "Gérer les modes de paiement.")
        _context.SetToolTip(btnPeriodicites, "Gérer les périodicités d'encodage / de facturation des organismes.")
        _context.SetToolTip(btnTextesTypes, "Gérer les textes types à insérer dans les notes, l'anamnèse et les zones du dossier.")
        _context.SetToolTip(btnUsagesTextes, "Nommer et ordonner les éditeurs où un texte type peut être proposé.")
        _context.SetToolTip(btnPays, "Gérer les pays proposés dans les adresses.")
        _context.SetToolTip(btnCivilites, "Gérer les civilités proposées lors de la saisie d'une personne.")
        _context.SetToolTip(btnLateralites, "Gérer les latéralités proposées dans la fiche patient.")
        _context.SetToolTip(btnSpecialites, "Gérer les spécialités proposées dans la fiche Thérapeute.")
        _context.SetToolTip(btnPrixSeance, "Gérer les tarifs de séance.")
        _context.SetToolTip(btnNaturesSeance, "Gérer les natures de séance.")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UC_ReferentielHome_Load
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Initialise l'affichage du UserControl au chargement.
    '
    ' Responsabilités :
    ' - Appeler AppliquerDroitsUtilisateur() pour configurer l'état des boutons selon le rôle
    ' - Thématiser les tuiles de référentiels via UtilsButtons.InitLargeIconButton()
    ' - Thématiser les boutons standards (Élever accès, Retour rôle de base) via UtilsButtons.InitStandardButton()
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Premier événement déclenché lors du chargement du UserControl dans Home
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_ReferentielHome_Load(
        sender As Object,
        e As EventArgs
    ) Handles MyBase.Load

        AppliquerDroitsUtilisateur()

        'Boutons Tuiles
        UtilsButtons.InitLargeIconButton(btnOrganismes)
        UtilsButtons.InitLargeIconButton(btnLiensPatient)
        UtilsButtons.InitLargeIconButton(btnRolesIntervenant)
        UtilsButtons.InitLargeIconButton(btnSituationsFamiliales)
        UtilsButtons.InitLargeIconButton(btnStatutsDossier)
        UtilsButtons.InitLargeIconButton(btnStatutsSeance)
        UtilsButtons.InitLargeIconButton(btnTypesDocuments)
        UtilsButtons.InitLargeIconButton(btnTypesRendezVous)
        UtilsButtons.InitLargeIconButton(btnRoleLegal)
        UtilsButtons.InitLargeIconButton(btnTherapeutes)
        UtilsButtons.InitLargeIconButton(btnModesPaiement)
        UtilsButtons.InitLargeIconButton(btnPeriodicites)
        UtilsButtons.InitLargeIconButton(btnTextesTypes)
        UtilsButtons.InitLargeIconButton(btnUsagesTextes)
        UtilsButtons.InitLargeIconButton(btnPays)
        UtilsButtons.InitLargeIconButton(btnCivilites)
        UtilsButtons.InitLargeIconButton(btnLateralites)
        UtilsButtons.InitLargeIconButton(btnSpecialites)
        UtilsButtons.InitLargeIconButton(btnPrixSeance)
        UtilsButtons.InitLargeIconButton(btnNaturesSeance)

        'Boutons Standards
        UtilsButtons.InitStandardButton(btnEleverAcces)
        UtilsButtons.InitStandardButton(btnRetourRoleBase)

        InitialiserToolTips()

    End Sub

#End Region

#Region "Actions référentiels"

    ' Ouvre un écran dans la fenêtre principale (fil d'Ariane « Référentiels > … », audit C.4, ADR-55).
    ' Sans fenêtre principale (écran hébergé ailleurs que dans Home), la vue est libérée et rien ne
    ' s'ouvre, comme lorsque Home était introuvable.
    Private Sub Ouvrir(vue As UserControl, titre As String)

        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale

        If fenetre Is Nothing Then
            vue.Dispose()
            Return
        End If

        fenetre.NavigateToReferentielView(vue, titre)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnOrganismes_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des organismes (UC_Organismes).
    ' - (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnOrganismes_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnOrganismes.Click

        Ouvrir(New UC_Organismes(), "Organismes")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnLiensPatient_Click
    ' Version    : V1.0.1
    ' Date       : 25/10/2026
    '
    ' Rôle       :
    ' Ouvre l'écran de gestion du référentiel des liens patient (UserControl UC_LiensPatient).
    '
    ' Responsabilités :
    ' - Lire la fenêtre principale dans le contexte (_context.FenetrePrincipale)
    ' - Naviguer vers UC_LiensPatient par la fenêtre principale (Ouvrir)
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon)
    '
    ' Exceptions :
    ' - Aucune (rien ne s'ouvre sans fenêtre principale)
    ' - V1.0.1 (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnLiensPatient_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnLiensPatient.Click

        Ouvrir(New UC_LiensPatient(), "Liens patient")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnRoleLegal_Click
    ' Version    : V1.0.1
    ' Date       : 25/10/2026
    '
    ' Rôle       :
    ' Ouvre l'écran de gestion du référentiel des rôles légaux des contacts (UserControl UC_RoleLegal).
    '
    ' Responsabilités :
    ' - Lire la fenêtre principale dans le contexte (_context.FenetrePrincipale)
    ' - Naviguer vers UC_RoleLegal par la fenêtre principale (Ouvrir)
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon)
    '
    ' Exceptions :
    ' - Aucune (rien ne s'ouvre sans fenêtre principale)
    ' - V1.0.1 (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnRoleLegal_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnRoleLegal.Click

        Ouvrir(New UC_RoleLegal(), "Rôles légaux")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnTherapeutes_Click
    ' Version    : V1.0.1
    ' Date       : 25/10/2026
    '
    ' Rôle       :
    ' Ouvre l'écran de gestion du référentiel des thérapeutes (UserControl UC_Therapeutes).
    '
    ' Responsabilités :
    ' - Lire la fenêtre principale dans le contexte (_context.FenetrePrincipale)
    ' - Naviguer vers UC_Therapeutes par la fenêtre principale (Ouvrir)
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon)
    '
    ' Exceptions :
    ' - Aucune (rien ne s'ouvre sans fenêtre principale)
    ' - V1.0.1 (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnTherapeutes_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnTherapeutes.Click

        Ouvrir(New UC_Therapeutes(), "Thérapeutes")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnRolesIntervenant_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des rôles intervenant (UC_RolesIntervenant).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' - (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnRolesIntervenant_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnRolesIntervenant.Click

        Ouvrir(New UC_RolesIntervenant(), "Rôles intervenant")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnSituationsFamiliales_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des situations familiales (UC_SituationsFamiliales).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' - (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnSituationsFamiliales_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnSituationsFamiliales.Click

        Ouvrir(New UC_SituationsFamiliales(), "Situations familiales")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnStatutsDossier_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des statuts de dossier (UC_StatutsDossier).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' - (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnStatutsDossier_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnStatutsDossier.Click

        Ouvrir(New UC_StatutsDossier(), "Statuts dossier")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnStatutsSeance_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des statuts de séance (UC_StatutsSeance).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' - (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnStatutsSeance_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnStatutsSeance.Click

        Ouvrir(New UC_StatutsSeance(), "Statuts séance")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnTypesDocuments_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des types de documents (UC_TypesDocuments).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' - (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnTypesDocuments_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnTypesDocuments.Click

        Ouvrir(New UC_TypesDocuments(), "Types documents")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnTypesRendezVous_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des types de rendez-vous (UC_TypesRendezVous).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' - (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnTypesRendezVous_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnTypesRendezVous.Click

        Ouvrir(New UC_TypesRendezVous(), "Types rendez-vous")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnModesPaiement_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des modes de paiement (UC_ModesPaiement).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' - (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnModesPaiement_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnModesPaiement.Click

        Ouvrir(New UC_ModesPaiement(), "Modes paiement")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnPeriodicites_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des périodicités d'encodage (UC_Periodicites).
    ' - (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnPeriodicites_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnPeriodicites.Click

        Ouvrir(New UC_Periodicites(), "Périodicités")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnTextesTypes_Click
    ' Version    : V1.0.1
    ' Date       : 25/10/2026
    ' Rôle       : Ouvre l'écran de gestion des textes types (UC_TextesTypes).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' - V1.0.1 (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnTextesTypes_Click(sender As Object, e As EventArgs) Handles btnTextesTypes.Click

        Ouvrir(New UC_TextesTypes(), "Textes types")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnUsagesTextes_Click
    ' Version    : V1.0.1
    ' Date       : 25/10/2026
    ' Rôle       : Ouvre l'écran des usages des textes types (UC_UsagesTextesTypes).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' - V1.0.1 (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnUsagesTextes_Click(sender As Object, e As EventArgs) Handles btnUsagesTextes.Click

        Ouvrir(New UC_UsagesTextesTypes(), "Usages des textes types")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnPays_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des pays (UC_Pays).
    ' - (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnPays_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnPays.Click

        Ouvrir(New UC_Pays(), "Pays")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnCivilites_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des civilités (UC_Civilites).
    ' - (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnCivilites_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnCivilites.Click

        Ouvrir(New UC_Civilites(), "Civilités")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnLateralites_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des latéralités (UC_Lateralites, v2.46.0).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' - (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnLateralites_Click(sender As Object, e As EventArgs) Handles btnLateralites.Click

        Ouvrir(New UC_Lateralites(), "Latéralités")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnSpecialites_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des spécialités (UC_Specialites, v2.55.0).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' - (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnSpecialites_Click(sender As Object, e As EventArgs) Handles btnSpecialites.Click

        Ouvrir(New UC_Specialites(), "Spécialités")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnPrixSeance_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des tarifs de séance (UC_PrixSeance).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' - (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnPrixSeance_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnPrixSeance.Click

        Ouvrir(New UC_PrixSeance(), "Prix séance")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnNaturesSeance_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des natures de séance (UC_NaturesSeance).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' - (25/10/2026, audit C.4, ADR-55) : ouverture par Ouvrir (fenêtre principale du contexte).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnNaturesSeance_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnNaturesSeance.Click

        Ouvrir(New UC_NaturesSeance(), "Natures séance")

    End Sub

#End Region

#Region "Gestion élévation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnRetourRoleBase_Click
    ' Version    : V1.0.1
    ' Date       : 25/10/2026
    '
    ' Rôle       :
    ' Annule l'élévation temporaire et revient au rôle utilisateur de base.
    '
    ' Responsabilités :
    ' - Vérifier que _userSession est valide et que l'élévation est active
    ' - Appeler UserSession.ResetElevation() pour revenir au rôle de base
    ' - Journaliser le retour via GestionLog (catégorie Security, niveau Rapide)
    ' - Rafraîchir l'affichage via AppliquerDroitsUtilisateur() et le bandeau (IFenetrePrincipale.UpdateConnectedUserDisplay)
    ' - Si le rôle de base ne permet plus l'accès (< SuperUser), retourner automatiquement à l'Accueil
    ' - Afficher les erreurs via MessageBox
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Bouton activé uniquement si IsElevated est True
    ' - Si le rôle de base < SuperUser, retour automatique à l'accueil (IFenetrePrincipale.NavigateToAccueil)
    '
    ' Exceptions :
    ' - Exception : Loguée via GestionLog ; MessageBox affiché à l'utilisateur
    ' - V1.0.1 (25/10/2026, audit C.4, ADR-55) : fenêtre principale lue dans le contexte
    '   (IFenetrePrincipale) au lieu de Home.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnRetourRoleBase_Click_1(sender As Object, e As EventArgs) Handles btnRetourRoleBase.Click

        Try

            If _userSession Is Nothing Then
                Return
            End If

            If Not _userSession.IsElevated Then

                SetStatus("Aucune élévation active.")

                Return

            End If

            _userSession.ResetElevation()

            EcrireLog(
                $"Retour au rôle de base ({_userSession.UserName}, rôle={_userSession.CurrentRole}).",
                LogLevel.Rapide,
                LogCategory.Security
            )

            Dim fenetre As IFenetrePrincipale =
                _context?.FenetrePrincipale

            If fenetre IsNot Nothing Then
                fenetre.UpdateConnectedUserDisplay()
            End If

            AppliquerDroitsUtilisateur()

            SetStatus("Retour au rôle de base effectué.")

            If _userSession.CurrentRole < AppRole.SuperUser Then

                If fenetre IsNot Nothing Then
                    fenetre.NavigateToAccueil()
                End If

            End If

        Catch ex As Exception

            EcrireLog(
                "Erreur btnRetourRoleBase_Click.",
                LogLevel.Succinct,
                LogCategory.Security,
                ex
            )

            DialogChoix.Erreur(
                "Erreur lors du retour au rôle de base.",
                "Erreur"
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnEleverAcces_Click
    ' Version    : V2.0.0
    ' Date       : 25/10/2026
    '
    ' Rôle       :
    ' Demande une élévation temporaire d'accès (rôle supérieur) à la fenêtre principale.
    '
    ' Responsabilités :
    ' - Vérifier que _userSession est valide
    ' - Demander l'élévation par la fenêtre principale du contexte (IFenetrePrincipale.DemanderElevation :
    '   fil d'Ariane temporaire, fenêtre ElevationAcces, bandeau rafraîchi, journal de sécurité)
    ' - Réappliquer les droits de l'écran (AppliquerDroitsUtilisateur) et annoncer le rôle courant
    ' - Afficher les erreurs via DialogChoix
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Bouton activé uniquement si CurrentRole < RoleMaxElevation
    ' - Si l'élévation est annulée ou refusée, aucun changement n'est appliqué
    ' - V2.0.0 (25/10/2026, audit C.4, ADR-55, décision de Joëlle) : le bloc d'élévation, écrit ici et
    '   dans Home, est réuni dans la fenêtre principale (DemanderElevation) ; ElevationAcces journalise
    '   l'élévation accordée.
    '
    ' Exceptions :
    ' - Exception : Loguée via GestionLog ; message d'erreur affiché à l'utilisateur
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEleverAcces_Click_1(sender As Object, e As EventArgs) Handles btnEleverAcces.Click

        Try

            If _userSession Is Nothing Then
                SetStatus("Session utilisateur absente.")
                Return
            End If

            Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale

            If fenetre Is Nothing Then
                SetStatus("Fenêtre principale indisponible.")
                Return
            End If

            If Not fenetre.DemanderElevation(fenetre.BuildReferentielContexte("Élévation d'accès")) Then
                SetStatus("Élévation annulée ou refusée.")
                Return
            End If

            AppliquerDroitsUtilisateur()

            SetStatus($"Rôle courant : {_userSession.CurrentRole}")

        Catch ex As Exception

            EcrireLog(
                "Erreur btnEleverAcces_Click.",
                LogLevel.Succinct,
                LogCategory.Security,
                ex
            )

            DialogChoix.Erreur(
                "Erreur lors de l'élévation d'accès.",
                "Erreur"
            )

        End Try

    End Sub

#End Region

End Class
