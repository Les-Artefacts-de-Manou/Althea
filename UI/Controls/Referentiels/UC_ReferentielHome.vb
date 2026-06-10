' -------------------------------------------------------------------------------------------------
' UserControl : UC_ReferentielHome
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 09/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Hub d'accueil de la section Référentiels (tables ref_*). Présente une tuile par référentiel
' et oriente vers l'écran de gestion correspondant (UC_Domaines, etc.).
'
' Responsabilités :
' - Afficher l'interface d'accueil de la section Référentiels
' - Gérer l'affichage et l'état des boutons selon le rôle utilisateur (SuperUser, Admin)
' - Proposer l'accès aux écrans de gestion des référentiels (Domaines en V1, autres à venir)
' - Gérer l'élévation temporaire des droits d'accès (bouton "Élever accès")
' - Gérer le retour au rôle de base après élévation (bouton "Retour rôle de base")
' - Afficher le rôle courant et l'état de l'élévation (lblRoleCourant, lblElevation)
' - Naviguer vers les écrans de référentiels via Home.NavigateToReferentielView()
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
' - Home (Form parente : navigation, contexte, affichage utilisateur)
' - ElevationAcces (Form élévation d'accès)
' - UC_Domaines (UserControl gestion des domaines)
' - UtilsButtons (thématisation des boutons)
' - GestionLog (journalisation)
'
' Interface   :
' - IContextAwareUserControl : Pour injection du contexte UI partagé
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

        btnDomaines.Enabled = True
        btnLiensPatient.Enabled = True
        btnRolesIntervenant.Enabled = True
        btnSituationsFamiliales.Enabled = True
        btnStatutsDossier.Enabled = True
        btnStatutsSeance.Enabled = True
        btnTypesDocuments.Enabled = True
        btnTypesRendezVous.Enabled = True
        btnTypesSeance.Enabled = True

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

        btnDomaines.Enabled = False
        btnLiensPatient.Enabled = False
        btnRolesIntervenant.Enabled = False
        btnSituationsFamiliales.Enabled = False
        btnStatutsDossier.Enabled = False
        btnStatutsSeance.Enabled = False
        btnTypesDocuments.Enabled = False
        btnTypesRendezVous.Enabled = False
        btnTypesSeance.Enabled = False

    End Sub

#End Region

#Region "Initialisation"

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
        UtilsButtons.InitLargeIconButton(btnDomaines)
        UtilsButtons.InitLargeIconButton(btnLiensPatient)
        UtilsButtons.InitLargeIconButton(btnRolesIntervenant)
        UtilsButtons.InitLargeIconButton(btnSituationsFamiliales)
        UtilsButtons.InitLargeIconButton(btnStatutsDossier)
        UtilsButtons.InitLargeIconButton(btnStatutsSeance)
        UtilsButtons.InitLargeIconButton(btnTypesDocuments)
        UtilsButtons.InitLargeIconButton(btnTypesRendezVous)
        UtilsButtons.InitLargeIconButton(btnTypesSeance)

        'Boutons Standards
        UtilsButtons.InitStandardButton(btnEleverAcces)
        UtilsButtons.InitStandardButton(btnRetourRoleBase)

    End Sub

#End Region

#Region "Actions référentiels"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnDomaines_Click
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Ouvre l'écran de gestion du référentiel des domaines (UserControl UC_Domaines).
    '
    ' Responsabilités :
    ' - Récupérer la Form parente Home via FindForm()
    ' - Naviguer vers UC_Domaines via Home.NavigateToReferentielView()
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon)
    '
    ' Exceptions :
    ' - Aucune (gestion silencieuse si Home est introuvable)
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnDomaines_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnDomaines.Click

        Dim homeForm = TryCast(FindForm(), Home)

        If homeForm Is Nothing Then
            Return
        End If

        homeForm.NavigateToReferentielView(
            New UC_Domaines(),
            "Domaines"
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnLiensPatient_Click
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Ouvre l'écran de gestion du référentiel des liens patient (UserControl UC_LiensPatient).
    '
    ' Responsabilités :
    ' - Récupérer la Form parente Home via FindForm()
    ' - Naviguer vers UC_LiensPatient via Home.NavigateToReferentielView()
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon)
    '
    ' Exceptions :
    ' - Aucune (gestion silencieuse si Home est introuvable)
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnLiensPatient_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnLiensPatient.Click

        Dim homeForm = TryCast(FindForm(), Home)

        If homeForm Is Nothing Then
            Return
        End If

        homeForm.NavigateToReferentielView(
            New UC_LiensPatient(),
            "Liens patient"
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnRolesIntervenant_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des rôles intervenant (UC_RolesIntervenant).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnRolesIntervenant_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnRolesIntervenant.Click

        Dim homeForm = TryCast(FindForm(), Home)

        If homeForm Is Nothing Then
            Return
        End If

        homeForm.NavigateToReferentielView(
            New UC_RolesIntervenant(),
            "Rôles intervenant"
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnSituationsFamiliales_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des situations familiales (UC_SituationsFamiliales).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnSituationsFamiliales_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnSituationsFamiliales.Click

        Dim homeForm = TryCast(FindForm(), Home)

        If homeForm Is Nothing Then
            Return
        End If

        homeForm.NavigateToReferentielView(
            New UC_SituationsFamiliales(),
            "Situations familiales"
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnStatutsDossier_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des statuts de dossier (UC_StatutsDossier).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnStatutsDossier_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnStatutsDossier.Click

        Dim homeForm = TryCast(FindForm(), Home)

        If homeForm Is Nothing Then
            Return
        End If

        homeForm.NavigateToReferentielView(
            New UC_StatutsDossier(),
            "Statuts dossier"
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnStatutsSeance_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des statuts de séance (UC_StatutsSeance).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnStatutsSeance_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnStatutsSeance.Click

        Dim homeForm = TryCast(FindForm(), Home)

        If homeForm Is Nothing Then
            Return
        End If

        homeForm.NavigateToReferentielView(
            New UC_StatutsSeance(),
            "Statuts séance"
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnTypesDocuments_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des types de documents (UC_TypesDocuments).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnTypesDocuments_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnTypesDocuments.Click

        Dim homeForm = TryCast(FindForm(), Home)

        If homeForm Is Nothing Then
            Return
        End If

        homeForm.NavigateToReferentielView(
            New UC_TypesDocuments(),
            "Types documents"
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnTypesRendezVous_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des types de rendez-vous (UC_TypesRendezVous).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnTypesRendezVous_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnTypesRendezVous.Click

        Dim homeForm = TryCast(FindForm(), Home)

        If homeForm Is Nothing Then
            Return
        End If

        homeForm.NavigateToReferentielView(
            New UC_TypesRendezVous(),
            "Types rendez-vous"
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnTypesSeance_Click
    ' Rôle       : Ouvre l'écran de gestion du référentiel des types de séance (UC_TypesSeance).
    ' Remarques  : Accès réservé aux rôles SuperUser et Admin (tuile désactivée sinon).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnTypesSeance_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnTypesSeance.Click

        Dim homeForm = TryCast(FindForm(), Home)

        If homeForm Is Nothing Then
            Return
        End If

        homeForm.NavigateToReferentielView(
            New UC_TypesSeance(),
            "Types séance"
        )

    End Sub

#End Region

#Region "Gestion élévation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnRetourRoleBase_Click
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Annule l'élévation temporaire et revient au rôle utilisateur de base.
    '
    ' Responsabilités :
    ' - Vérifier que _userSession est valide et que l'élévation est active
    ' - Appeler UserSession.ResetElevation() pour revenir au rôle de base
    ' - Journaliser le retour via GestionLog (catégorie Security, niveau Rapide)
    ' - Rafraîchir l'affichage via AppliquerDroitsUtilisateur() et Home.UpdateConnectedUserDisplay()
    ' - Si le rôle de base ne permet plus l'accès (< SuperUser), retourner automatiquement à l'Accueil
    ' - Afficher les erreurs via MessageBox
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Bouton activé uniquement si IsElevated est True
    ' - Si le rôle de base < SuperUser, navigation automatique vers Home.NavigateToAccueil()
    '
    ' Exceptions :
    ' - Exception : Loguée via GestionLog ; MessageBox affiché à l'utilisateur
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnRetourRoleBase_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnRetourRoleBase.Click

        Try

            If _userSession Is Nothing Then
                Return
            End If

            If Not _userSession.IsElevated Then

                SetStatus("Aucune élévation active.")

                Return

            End If

            _userSession.ResetElevation()

            GestionLog.EcrireLog(
                $"Retour au rôle de base ({_userSession.UserName}, rôle={_userSession.CurrentRole}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Security
            )

            Dim homeForm As Home =
                TryCast(FindForm(), Home)

            If homeForm IsNot Nothing Then
                homeForm.UpdateConnectedUserDisplay()
            End If

            AppliquerDroitsUtilisateur()

            SetStatus("Retour au rôle de base effectué.")

            If _userSession.CurrentRole < AppRole.SuperUser Then

                If homeForm IsNot Nothing Then
                    homeForm.NavigateToAccueil()
                End If

            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur btnRetourRoleBase_Click.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
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
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Ouvre la Form d'élévation d'accès (ElevationAcces) pour demander temporairement un rôle supérieur.
    '
    ' Responsabilités :
    ' - Vérifier que _userSession est valide
    ' - Récupérer la Form parente Home via FindForm()
    ' - Pousser un contexte temporaire dans Home via PushContexteTemporaire()
    ' - Ouvrir ElevationAcces en mode modal avec injection du contexte UI et de la session utilisateur
    ' - Restaurer le contexte précédent après fermeture de la Form
    ' - Rafraîchir l'affichage via AppliquerDroitsUtilisateur() et Home.UpdateConnectedUserDisplay()
    ' - Journaliser l'élévation via GestionLog (catégorie Security, niveau Rapide)
    ' - Afficher les erreurs via MessageBox
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement
    '
    ' Remarques  :
    ' - Bouton activé uniquement si CurrentRole < RoleMaxElevation
    ' - Si l'élévation est annulée ou refusée, aucun changement n'est appliqué
    '
    ' Exceptions :
    ' - Exception : Loguée via GestionLog ; MessageBox affiché à l'utilisateur
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEleverAcces_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnEleverAcces.Click

        Try

            If _userSession Is Nothing Then

                SetStatus("Session utilisateur absente.")

                Return

            End If

            Dim homeForm As Home =
                TryCast(FindForm(), Home)

            If homeForm Is Nothing Then

                SetStatus("Impossible de retrouver Home.")

                Return

            End If

            Dim contextePrecedent As String =
                homeForm.PushContexteTemporaire(
                    homeForm.BuildReferentielContexte("Élévation d'accès")
                )

            Try

                Using frmElevation As New ElevationAcces(
                    _userSession,
                    homeForm.AuthenticatedUser
                )

                    frmElevation.SetContext(_context)

                    Dim result As DialogResult =
                        frmElevation.ShowDialog(homeForm)

                    If result <> DialogResult.OK Then

                        SetStatus("Élévation annulée ou refusée.")

                        Return

                    End If

                End Using

            Finally

                homeForm.SetContexte(contextePrecedent)

            End Try

            AppliquerDroitsUtilisateur()

            homeForm.UpdateConnectedUserDisplay()

            GestionLog.EcrireLog(
                $"Élévation appliquée depuis ReferentielHome ({_userSession.UserName} -> {_userSession.CurrentRole}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Security
            )

            SetStatus($"Rôle courant : {_userSession.CurrentRole}")

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur btnEleverAcces_Click.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
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
