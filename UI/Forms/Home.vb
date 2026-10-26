' -------------------------------------------------------------------------------------------------
' Form       : Home
' Projet     : Althéa
' Version    : V1.22.0
' Date       : 25/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Form principale et Shell applicatif de l''application Althéa.
' Centralise l''authentification, la navigation, les composants UI partagés, et l''hébergement
' des UserControls métier dans une zone de contenu dynamique.
'
' Responsabilités :
' - Gérer l''authentification initiale via Login.vb au démarrage
' - Gérer le changement de mot de passe obligatoire (MustChangePassword)
' - Créer et maintenir la session utilisateur (UserSession) pendant toute la durée de vie de l''application
' - Héberger les composants UI partagés (StatusStrip, ToolTip, ErrorProvider, lblContexte)
' - Fournir un contexte UI partagé (UserControlContext) à tous les UserControls via IContextAwareUserControl
' - Gérer le menu principal (pnlMenu) avec état visuel des boutons (normal/hover/sélectionné)
' - Charger dynamiquement les UserControls métier dans pnlContent via NavigationManager
' - Afficher le contexte de navigation hiérarchique (lblContexte : "Accueil", "Administration > Paramètres", etc.)
' - Gérer l''élévation temporaire de privilèges pour accéder aux zones réservées (Administration)
' - Afficher l''utilisateur connecté, son rôle courant, et l''état d''élévation dans le header (lblUtilisateurConnecte)
' - Synchroniser le menu, le contenu, et le StatusStrip lors de chaque navigation
' - Journaliser les actions de navigation et d''élévation via GestionLog
' - Permettre la navigation de retour depuis les UserControls enfants (NavigateToAccueil, NavigateToAdminView)
'
' Remarques  :
' - Home est le nœud central de l''application : tous les UserControls métier sont chargés dans pnlContent
' - Home ne contient aucune logique métier ni accès direct à la base de données
' - L''architecture est organisée en panneaux :
'   * pnlMenu (gauche)    : boutons de navigation principale
'   * pnlHeader (haut)    : contexte utilisateur (lblUtilisateurConnecte) et contexte de navigation (lblContexte)
'   * pnlContent (centre) : zone dynamique où sont chargés les UserControls
'   * stsStatus (bas)     : StatusStrip partagé (stsLabelStatus)
' - Le mécanisme de navigation est centralisé via NavigationManager (instancié dans Home_Load)
' - Les UserControls implémentant IContextAwareUserControl reçoivent automatiquement le contexte via SetContext()
' - L''élévation de privilèges est temporaire et limitée à la session en cours (gérée par UserSession)
' - Le DialogResult n''est jamais utilisé (Home est la Form principale, pas une modale)
' - Les écrans et fenêtres naviguent par le contrat IFenetrePrincipale (UserControlContext.FenetrePrincipale),
'   que Home implémente (audit C.4, ADR-55) : ils ne connaissent plus la classe Home.
'
' Dépendances :
' - Login (authentification initiale au démarrage)
' - ChangePassword (changement de mot de passe obligatoire)
' - ElevationAcces (élévation temporaire de privilèges)
' - UserSession (session utilisateur, rôle courant, état d''élévation)
' - UtilisateurApplication (utilisateur authentifié)
' - UserControlContext (contexte UI partagé pour les UserControls)
' - NavigationManager (chargement centralisé des UserControls)
' - UC_Accueil (UserControl d''accueil chargé par défaut)
' - UC_AdminHome (UserControl administration)
' - UtilsButtons (thématisation et gestion des boutons du menu)
' - GestionLog (journalisation des actions de navigation et d''élévation)
'
' Historique :
' - V1.13.1 (18/10/2026) : AttacherFeuillageCoupe, commentaire à jour (ancien fond supprimé, UI-8).
' - V1.13.2 (18/10/2026) : exemples des commentaires sur un référentiel existant (UC_Pays).
' - V1.14.0 (20/10/2026) : logo du cabinet lisible : réduit dans un cadre 180 × 40 (au lieu d'un carré
'   de 30 px), centré sur la ligne du nom.
' - V1.15.0 (20/10/2026) : le journal de navigation note le nom du bouton du menu, plus le texte du
'   fil d'Ariane qui pouvait contenir le nom d'un patient (audit B3).
' - V1.16.1 (21/10/2026) : fin de l'élévation — seules les fenêtres modales visibles la retardent.
' - V1.16.0 (20/10/2026) : fin de l'élévation après inactivité (audit A.5, décision de Joëlle) —
'   activité notée par SuiviActivite, contrôle toutes les 30 s, durée SECURITE_ELEVATION_MINUTES ;
'   message, puis retour à l'accueil si un écran d'administration ou des référentiels est ouvert.
' - V1.17.0 (22/10/2026, audit C.1) : les opérations sensibles reçoivent le ContexteOperation de l'écran (utilisateur + rôle
'   courant), vérifié par le métier (Autorisation).
' - V1.17.1 (23/10/2026, v2.55.0) : la création d'un thérapeute depuis la fiche Intervenant transmet Home
'   à TherapeuteEdition (bouton [+] des spécialités).
' - V1.18.0 (23/10/2026, recherche transversale R1, ADR-52, décision de Joëlle) : bouton « Recherche »
'   du menu, sous Patients (UC_RechercheHome) ; NavigateDepuisRecherche ouvre un élément trouvé en
'   gardant la recherche dans la mini-pile (Retour y ramène, critères et résultats compris).
' - V1.19.0 (23/10/2026, recherche transversale R2) : NavigateToRecherche (bouton du menu, retour
'   d'une recherche spécialisée) ; NavigateDepuisRecherche reçoit le contexte de retour (« Recherche >
'   Séances »).
' - V1.20.0 (24/10/2026, Debug seulement) : FiltreCaptureEcran installé au chargement — captures
'   d'écran au clavier pour les illustrations de la documentation.
' - V1.21.0 (25/10/2026, audit C.4, ADR-55) : Home implémente IFenetrePrincipale (navigation,
'   référentiels en fenêtre, fil d'Ariane, bandeau) et se fournit dans le UserControlContext.
' - V1.22.0 (25/10/2026, audit C.4 étape 3, ADR-55) : élévation réunie — DemanderElevation (contrat,
'   hubs), ObtenirElevationPourZone (zones réservées), OuvrirFenetreElevation (seul endroit où
'   ElevationAcces s'ouvre, avec le contexte UI).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class Home
    Implements IFenetrePrincipale

#Region "Variables privées"

    ' Liste centralisée des boutons du menu principal (pnlMenu).
    Private ReadOnly _homeMenuButtons As New List(Of Button)

    ' Gestionnaire de navigation centralisé pour charger les UserControls métier dans pnlContent.
    Private _navManager As NavigationManager

    ' Session utilisateur courante maintenant l'état d'authentification, le rôle actif, et l'élévation temporaire.
    Private _userSession As UserSession

    ' Contexte UI partagé fourni à tous les UserControls via IContextAwareUserControl.SetContext().
    Private _uiContext As UserControlContext

    ' Utilisateur authentifié retourné par Login.vb au démarrage de l'application.
    Private _authenticatedUser As UtilisateurApplication

    ' Bouton du menu principal de l'écran affiché (fin de l'élévation : quitter une zone réservée).
    Private _boutonMenuCourant As Button

    ' Fin de l'élévation après inactivité (audit A.5) : activité notée sur toute l'application,
    ' contrôlée régulièrement.
    Private Const INTERVALLE_CONTROLE_ELEVATION_MS As Integer = 30000
    Private _suiviActivite As SuiviActivite
    Private WithEvents _minuterieElevation As System.Windows.Forms.Timer
    Private _finElevationEnCours As Boolean
    Private _finElevationReportee As Boolean
    Private _elevationSurveillee As DateTime?

    ' Feuillage décoratif du coin haut droit, dessiné à taille réelle sur pnlForm (jamais étiré).
    Private ReadOnly _feuillageHome As Image = My.Resources.Resources.Home_Feuillage_CoinHautDroit

    ' Surfaces de fond des UserControls hébergés dans pnlContent (clé = UserControl, valeur = surface),
    ' sur lesquelles la suite du feuillage est dessinée.
    Private ReadOnly _surfacesFeuillage As New Dictionary(Of Control, Control)

#End Region

#Region "Propriétés publiques"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : AuthenticatedUser
    ' Type       : UtilisateurApplication (ReadOnly)
    ' Version    : V1.0.0
    ' Date       : 12/05/2026
    '
    ' Rôle       :
    ' Retourne l'utilisateur authentifié courant récupéré depuis Login.vb au démarrage.
    '
    ' Retour     :
    ' - UtilisateurApplication : Instance de l'utilisateur authentifié avec toutes ses propriétés
    '
    ' Remarques  :
    ' - Propriété ReadOnly exposée publiquement pour permettre aux UserControls d'accéder aux informations utilisateur
    ' - Utilisée notamment par UC_AdminHome pour passer l'utilisateur authentifié lors de l'élévation
    ' - Ne retourne jamais Nothing après authentification réussie (Home se ferme si login échoue)
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property AuthenticatedUser As UtilisateurApplication
        Get
            Return _authenticatedUser
        End Get
    End Property

#End Region

#Region "Initialisation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Home_Load
    ' Version    : V1.5.0
    ' Date       : 07/05/2026
    '
    ' Rôle       :
    ' Initialise la Form Home : authentification, session utilisateur, contexte UI partagé,
    ' changement de mot de passe obligatoire, et chargement du UserControl d'accueil par défaut.
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement
    ' - e      : Arguments de l'événement Load
    '
    ' Remarques  :
    ' - Séquence d'initialisation :
    '   1. Initialisation des boutons du menu principal (InitializeHomeMenuButtons)
    '   2. Affichage de Login.vb en modal et authentification utilisateur
    '   3. Si authentification échouée (DialogResult <> OK) : fermeture de l'application
    '   4. Création de la UserSession avec les informations de l'utilisateur authentifié
    '   5. Affichage de l'utilisateur connecté dans le header (UpdateConnectedUserDisplay)
    '   6. Création du contexte UI partagé (UserControlContext) pour injection dans les UserControls
    '   7. Si MustChangePassword = True : affichage de ChangePassword.vb en modal
    '   8. Si changement de mot de passe refusé : fermeture de l'application avec log sécurité
    '   9. Initialisation du NavigationManager avec pnlContent et _uiContext
    '   10. Chargement de UC_Accueil par défaut avec bouton Accueil sélectionné
    ' - Les boutons du menu Home ont un comportement spécifique : normal / hover / sélectionné
    ' - Le bouton sélectionné reste visuellement actif jusqu'au choix d'un autre menu
    ' - Home est la Form principale : si elle se ferme, l'application se termine
    '
    ' Exceptions :
    ' - Aucune gestion explicite (erreurs propagées au niveau global)
    ' -------------------------------------------------------------------------------------------------
    Private Sub Home_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        If _navManager IsNot Nothing AndAlso Not _navManager.PeutQuitterVueCourante() Then
            e.Cancel = True
            Return
        End If

        If _suiviActivite IsNot Nothing Then Application.RemoveMessageFilter(_suiviActivite)
        _minuterieElevation?.Stop()

    End Sub

    Private Sub Home_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Barre de titre Windows aux couleurs du kit (Windows 11).
        AltheaBarreTitre.Appliquer(Me)

        ' Initialisation du menu principal
        InitializeHomeMenuButtons()

        ' Connexion utilisateur
        Dim authenticatedUser As UtilisateurApplication

        Using frmLogin As New Login()

            Dim result As DialogResult =
        frmLogin.ShowDialog(Me)

            If result <> DialogResult.OK Then

                Close()

                Return

            End If

            authenticatedUser =
        frmLogin.AuthenticatedUser

        End Using

        _authenticatedUser =
    authenticatedUser

        ' Création session utilisateur
        _userSession = New UserSession(
    authenticatedUser.LoginUtilisateur,
    authenticatedUser.NomAffichage,
    authenticatedUser.RoleUtilisateur
)
        ' Affichage de l'utilisateur connecté dans le header
        UpdateConnectedUserDisplay()

        ' Initialisation du contexte partagé pour les UserControls
        _uiContext = New UserControlContext(
            stsLabelStatus,
            ttMain,
            errProvider,
            lblContexte,
            _userSession,
            _authenticatedUser,
            Me
        )

        ' Demande de changement de mot de passe 
        If authenticatedUser.MustChangePassword Then

            Using frmChangePassword As New ChangePassword(
        authenticatedUser)

                Dim result As DialogResult =
            frmChangePassword.ShowDialog(Me)

                If result <> DialogResult.OK Then

                    GestionLog.EcrireLog(
                $"Application fermée : changement mot de passe obligatoire refusé ({authenticatedUser.LoginUtilisateur}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security
            )
                    Close()

                    Return

                End If

            End Using

        End If

        ' Le bandeau et la zone de contenu sont transparents : ils doivent dessiner eux-mêmes
        ' la portion de feuillage qu'ils recouvrent (voir AttacherFeuillage).
        ActiverDoubleTampon(pnlForm)
        AttacherFeuillage(pnlHeader)
        AttacherFeuillage(pnlContent)

        RafraichirBandeauCabinet()

        ' Initialisation du gestionnaire de navigation centralisé
        _navManager = New NavigationManager(pnlContent, _uiContext)
        NavigateTo(New UC_Accueil(), btnAccueil, "Accueil")

        ' Fin de l'élévation après inactivité (audit A.5).
        _suiviActivite = New SuiviActivite(_userSession)
        Application.AddMessageFilter(_suiviActivite)
#If DEBUG Then
        ' Outil de documentation : Ctrl+Maj+F8 / F7 (voir FiltreCaptureEcran).
        Application.AddMessageFilter(New FiltreCaptureEcran())
#End If
        _minuterieElevation = New System.Windows.Forms.Timer(components) With {.Interval = INTERVALLE_CONTROLE_ELEVATION_MS}
        _minuterieElevation.Start()

        '  _uiContext.SetStatus("Application prête")
        '  _uiContext.SetHeader("Accueil")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Home_Shown
    ' Version    : V1.0.0
    ' Date       : 12/10/2026
    '
    ' Rôle       :
    ' Une fois la fenêtre affichée, rappelle de sauvegarder si la dernière sauvegarde réussie est
    ' trop ancienne (SAUVEGARDE_RAPPEL_JOURS) — SuperUser et Admin seulement.
    '
    ' Remarques  :
    ' - Shown plutôt que Load : le rappel apparaît sur la fenêtre principale, après la connexion.
    ' - GestionSauvegardes.MessageRappel ne lève pas d'exception.
    ' -------------------------------------------------------------------------------------------------
    Private Sub Home_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        If _userSession Is Nothing Then Return

        Dim rappel As String = GestionSauvegardes.MessageRappel(New ContexteOperation(_authenticatedUser, _userSession.CurrentRole))
        If rappel Is Nothing Then Return

        _uiContext?.SetStatus("Pensez à faire une sauvegarde.")
        DialogChoix.Information(rappel, "Althéa - Sauvegarde")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateConnectedUserDisplay
    ' Version    : V1.1.0
    ' Date       : 12/05/2026
    '
    ' Rôle       :
    ' Met à jour l'affichage de l'utilisateur connecté dans le header (lblUtilisateurConnecte)
    ' avec le nom, le rôle courant, et l'état d'élévation temporaire.
    '
    ' Paramètres :
    ' - Aucun
    '
    ' Remarques  :
    ' - Appelée dans Home_Load après création de la UserSession
    ' - Appelée après chaque élévation de privilèges (btnAdmin_Click) pour refléter le changement de rôle
    ' - Appelée par UC_AdminHome après retour au rôle de base (btnRetourRoleBase_Click)
    ' - Affichage (UI, 11/10/2026) : pastille AltheaPuceUtilisateur — nom (partie du nom d'affichage
    '   avant « - »), précision (partie après), rôle courant, mention « Élévation » si active
    ' - Si _userSession est Nothing (cas anormal) : affiche « - » sans erreur
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub UpdateConnectedUserDisplay() Implements IFenetrePrincipale.UpdateConnectedUserDisplay

        If _userSession Is Nothing Then
            lblUtilisateurConnecte.Nom = "-"
            lblUtilisateurConnecte.Precision = String.Empty
            lblUtilisateurConnecte.Role = String.Empty
            lblUtilisateurConnecte.Eleve = False
            RemplacerPhotoUtilisateur(Nothing)
            Return
        End If

        ' Le nom d'affichage est saisi librement (« Joëlle - Administratrice ») : la partie après le
        ' premier « - » devient la seconde ligne de la pastille.
        Dim nomAffichage As String = If(_userSession.DisplayName, String.Empty)
        Dim position As Integer = nomAffichage.IndexOf(" - ", StringComparison.Ordinal)

        lblUtilisateurConnecte.Nom = If(position > 0, nomAffichage.Substring(0, position), nomAffichage)
        lblUtilisateurConnecte.Precision = If(position > 0, nomAffichage.Substring(position + 3), String.Empty)
        lblUtilisateurConnecte.Role = _userSession.CurrentRole.ToString()
        lblUtilisateurConnecte.Eleve = _userSession.IsElevated

    End Sub

#End Region

#Region "Initialisation UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InitializeHomeMenuButtons
    ' Version    : V1.0.0
    ' Date       : 25/04/2026
    '
    ' Rôle       :
    ' Initialise la liste centralisée des boutons du menu principal (_homeMenuButtons)
    ' et applique leur style standard via UtilsButtons.InitHomeMenuButton().
    '
    ' Paramètres :
    ' - Aucun
    '
    ' Remarques  :
    ' - Appelée dans Home_Load avant l'authentification
    ' - Ajoute les 8 boutons du menu principal : btnAccueil, btnPatients, btnRecherche, btnDomaines,
    '   btnAgenda, btnDocuments, btnReferentiels, btnAdmin
    ' - Chaque bouton doit avoir un Tag défini dans le Designer pour le système de navigation
    ' - Exemples de Tags : "accueil", "patients", "domaines", "agenda", "documents", "referentiels", "admin"
    ' - UtilsButtons.InitHomeMenuButton() configure les styles normal/hover/sélectionné pour chaque bouton
    ' - La liste _homeMenuButtons est utilisée par SetSelectedHomeMenuButton() pour gérer l'état visuel
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitializeHomeMenuButtons()

        _homeMenuButtons.Clear()

        _homeMenuButtons.AddRange(New Button() {
        btnAccueil,
        btnPatients,
        btnRecherche,
        btnDomaines,
        btnAgenda,
        btnDocuments,
        btnReferentiels,
        btnAdmin
    })

        For Each btn As Button In _homeMenuButtons
            UtilsButtons.InitHomeMenuButton(btn)
        Next

    End Sub

#End Region

#Region "Habillage du shell"

    ' Cadre du logo du cabinet dans le bandeau (pixels) : en longueur, comme la plupart des logos.
    Private Const LARGEUR_LOGO_CABINET As Integer = 180
    Private Const HAUTEUR_LOGO_CABINET As Integer = 40

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : RafraichirBandeauCabinet
    ' Version    : V1.1.0
    ' Date       : 20/10/2026
    '
    ' Rôle       :
    ' Affiche dans le bandeau le logo et le nom du cabinet, puis le praticien (maquette « Home A »).
    '
    ' Remarques  :
    ' - Praticien : celui lié au compte connecté, sinon le praticien principal du cabinet.
    ' - Appelée au chargement et par UC_Cabinet après un enregistrement (le bandeau suit les données).
    ' - Une erreur de lecture est journalisée et laisse le bandeau vide : elle ne bloque pas l'application.
    ' - V1.1.0 (20/10/2026) : logo réduit dans un cadre 180 × 40, contrôle à sa taille (lisible).
    ' -------------------------------------------------------------------------------------------------
    Public Sub RafraichirBandeauCabinet() Implements IFenetrePrincipale.RafraichirBandeauCabinet

        Dim cabinet As Cabinet = Nothing
        Dim praticien As Praticien = Nothing

        Try
            cabinet = GestionCabinet.GetCabinetActif()
            If _authenticatedUser IsNot Nothing AndAlso _authenticatedUser.IdUtilisateur > 0 Then
                praticien = GestionCabinet.GetPraticienParUtilisateur(CULng(_authenticatedUser.IdUtilisateur))
            End If
            If praticien Is Nothing Then praticien = GestionCabinet.GetPraticienPrincipal()
        Catch ex As Exception
            GestionLog.EcrireLog("Bandeau de la Home : lecture du cabinet impossible.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
        End Try

        ' Logo réduit en haute qualité dans un cadre en longueur, puis le contrôle prend sa taille :
        ' dans le carré de 30 px d'origine, un logo large devenait illisible (relevé de Joëlle).
        Dim logo As Image = Nothing
        Try
            If cabinet IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cabinet.CheminLogo) Then
                Using source As Image = CheminsCabinetHelper.ChargerImage(0UL, cabinet.CheminLogo)
                    If source IsNot Nothing Then logo = ImagesHelper.ReduireDans(source, LARGEUR_LOGO_CABINET, HAUTEUR_LOGO_CABINET)
                End Using
            End If
        Catch ex As Exception
            GestionLog.EcrireLog("Bandeau de la Home : logo du cabinet illisible.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
        End Try

        UtilsImages.RemplacerImage(picLogoCabinet, logo)
        picLogoCabinet.Visible = logo IsNot Nothing
        If logo IsNot Nothing Then
            picLogoCabinet.Size = logo.Size
            ' Centré sur la ligne du nom du cabinet.
            picLogoCabinet.Top = lblCabinet.Top + lblCabinet.Height \ 2 - logo.Height \ 2
        End If

        lblCabinet.Text = If(cabinet Is Nothing, String.Empty, If(cabinet.NomCabinet, String.Empty).Trim())

        Dim textePraticien As String = String.Empty
        If praticien IsNot Nothing Then
            textePraticien = $"{praticien.Prenom} {praticien.Nom}".Trim()
            If Not String.IsNullOrWhiteSpace(praticien.TitreProfessionnel) Then
                textePraticien &= $" · {praticien.TitreProfessionnel.Trim()}"
            End If
        End If
        lblPraticien.Text = textePraticien

        PlacerBandeauCabinet()

        ' La photo du compte connecté peut venir de la fiche praticien : on la recharge avec le bandeau.
        RafraichirPhotoUtilisateur()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RafraichirPhotoUtilisateur
    ' Rôle      : Recharge la photo de la pastille utilisateur (règle PhotosUtilisateur : photo du
    '             praticien lié, sinon photo propre du compte, sinon initiales).
    ' Remarques : Appelée avec le bandeau du cabinet (connexion, enregistrement dans Cabinet) et
    '             après la fiche d'un utilisateur (UC_Utilisateurs).
    ' -------------------------------------------------------------------------------------------------
    Public Sub RafraichirPhotoUtilisateur() Implements IFenetrePrincipale.RafraichirPhotoUtilisateur

        Dim photo As Image = Nothing

        Try
            If _authenticatedUser IsNot Nothing AndAlso _authenticatedUser.IdUtilisateur > 0 Then
                photo = PhotosUtilisateur.ChargerPhotoAffichee(_authenticatedUser.IdUtilisateur)
            End If
        Catch ex As Exception
            GestionLog.EcrireLog("Bandeau de la Home : lecture de la photo de l'utilisateur impossible.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
        End Try

        RemplacerPhotoUtilisateur(photo)

    End Sub

    ' Remplace la photo de la pastille et libère la précédente (le contrôle ne la possède pas).
    Private Sub RemplacerPhotoUtilisateur(photo As Image)
        Dim ancienne As Image = lblUtilisateurConnecte.Photo
        lblUtilisateurConnecte.Photo = photo
        ancienne?.Dispose()
    End Sub

    ' Aligne logo, nom du cabinet, séparateur et praticien selon la largeur réelle des textes.
    Private Sub PlacerBandeauCabinet()

        Const Ecart As Integer = 8
        Dim x As Integer = picLogoCabinet.Left

        If picLogoCabinet.Visible Then x = picLogoCabinet.Right + Ecart

        lblCabinet.Left = x
        Dim aCabinet As Boolean = lblCabinet.Text.Length > 0
        Dim aPraticien As Boolean = lblPraticien.Text.Length > 0
        If aCabinet Then x = lblCabinet.Right + Ecart

        lblCabinetSeparateur.Visible = aCabinet AndAlso aPraticien
        If lblCabinetSeparateur.Visible Then
            lblCabinetSeparateur.Left = x
            x = lblCabinetSeparateur.Right + Ecart
        End If

        lblPraticien.Left = x

    End Sub

    ' Filet séparant les écrans du menu des actions personnelles (Outils / Admin, mot de passe).
    Private Sub pnlMenu_Paint(sender As Object, e As PaintEventArgs) Handles pnlMenu.Paint

        Dim y As Integer = btnAdmin.Top - 8
        Using stylo As New Pen(UITheme.ColorBeigeFonce)
            e.Graphics.DrawLine(stylo, btnAdmin.Left + 4, y, btnAdmin.Right - 4, y)
        End Using

    End Sub

    ' Composition de la fenetre par le système : tous les contrôles enfants sont dessines hors
    ' écran puis affiches en une seule fois. Indispensable ici, ou de nombreux contrles
    ' transparents se superposent au fond et au feuillage.
    Private Const WS_EX_COMPOSITED As Integer = &H2000000

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : CreateParams
    ' Version    : V1.0.0
    ' Date       : 10/10/2026
    '
    ' Rôle       :
    ' Active la composition de la fenetre (WS_EX_COMPOSITED) pour supprimer le scintillement
    ' lors du chargement d'un écran dans pnlContent.
    '
    ' Remarques  :
    ' - Sans elle, chaque contrôle transparent declenche le redessin de son parent : avec une
    '   vingtaine de tuiles et de cartes, l'ecran se reconstruit visiblement a l'ouverture.
    ' - Si une grille (DataGridView) devenait lente au defilement, il faudrait restreindre la
    '   composition au seul panneau de contenu plutot qu'a toute la fenetre.
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get

            Dim parametres As CreateParams = MyBase.CreateParams
            parametres.ExStyle = parametres.ExStyle Or WS_EX_COMPOSITED
            Return parametres

        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ZoneFeuillage
    ' Version    : V1.0.0
    ' Date       : 10/10/2026
    '
    ' Rôle       :
    ' Emplacement du feuillage sur une surface donnée, dans les coordonnées de cette surface.
    '
    ' Paramètres :
    ' - surface : pnlForm de la Home, ou surface d'un UserControl hébergé
    '
    ' Retour     :
    ' - Rectangle : zone occupée par le feuillage, ou Rectangle.Empty s'il n'y a rien à dessiner
    '
    ' Remarques  :
    ' - Le feuillage reste calé sur le bord droit du pnlForm de la Home ; pour une surface
    '   hébergée, la position est simplement décalée de l'origine de cette surface.
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Function ZoneFeuillage(surface As Control) As Rectangle

        If _feuillageHome Is Nothing OrElse surface Is Nothing OrElse Not surface.IsHandleCreated Then
            Return Rectangle.Empty
        End If

        Dim origine As Point = Point.Empty

        If Not ReferenceEquals(surface, pnlForm) Then
            origine = pnlForm.PointToClient(surface.PointToScreen(Point.Empty))
        End If

        Return New Rectangle(pnlForm.ClientSize.Width - _feuillageHome.Width - origine.X,
                             -origine.Y,
                             _feuillageHome.Width,
                             _feuillageHome.Height)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerFeuillage
    ' Version    : V1.0.0
    ' Date       : 10/10/2026
    '
    ' Rôle       :
    ' Dessine le feuillage sur une surface, uniquement si la zone à repeindre le touche.
    '
    ' Paramètres :
    ' - e       : contexte de dessin
    ' - surface : surface sur laquelle dessiner
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub DessinerFeuillage(e As PaintEventArgs, surface As Control)

        Dim zone As Rectangle = ZoneFeuillage(surface)

        ' Un contrôle transparent fait repeindre son parent : inutile de redessiner le
        ' feuillage quand la zone concernée ne le touche pas.
        If zone.IsEmpty OrElse Not e.ClipRectangle.IntersectsWith(zone) Then Return

        e.Graphics.DrawImage(_feuillageHome, zone)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InvaliderFeuillage
    ' Version    : V1.0.0
    ' Date       : 10/10/2026
    '
    ' Rôle       :
    ' Demande le redessin de la seule zone du feuillage, sans toucher aux contrôles enfants.
    '
    ' Paramètres :
    ' - surface : surface portant le feuillage
    '
    ' Remarques  :
    ' - **Ne jamais utiliser Invalidate(True) ici.** Les enfants ne dessinent pas le feuillage,
    '   et les inclure repeint toute l'arborescence de l'écran. Sur un écran chargé (grille,
    '   filtres, barre de boutons), un redimensionnement survenant pendant ce repeint relance
    '   le cycle : le feuillage se met alors à clignoter sans fin (constaté le 10/10/2026 sur
    '   l'onglet Séances de UC_DossierFiche).
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub InvaliderFeuillage(surface As Control)

        Dim zone As Rectangle = ZoneFeuillage(surface)

        If zone.IsEmpty Then Return

        surface.Invalidate(zone)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : pnlForm_Paint
    ' Version    : V1.0.0
    ' Date       : 09/10/2026
    '
    ' Rôle       :
    ' Dessine le feuillage décoratif (Home_Feuillage_CoinHautDroit) collé au bord droit de pnlForm,
    ' à sa taille réelle, quelle que soit la largeur de la fenêtre.
    '
    ' Paramètres :
    ' - sender : pnlForm
    ' - e      : contexte de dessin
    '
    ' Remarques  :
    ' - Le fond Home_Fond_2560x1440 est en BackgroundImageLayout = None : il est rogné, jamais étiré.
    ' - Le feuillage ne peut pas faire partie du fond : ancré en haut à gauche, il serait coupé
    '   dès que la fenêtre est moins large que 2560 px.
    ' - pnlHeader et pnlContent étant transparents, le feuillage apparaît derrière eux.
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub pnlForm_Paint(sender As Object, e As PaintEventArgs) Handles pnlForm.Paint

        DessinerFeuillage(e, pnlForm)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : pnlForm_Resize
    ' Version    : V1.0.0
    ' Date       : 09/10/2026
    '
    ' Rôle       :
    ' Redessine pnlForm et ses panneaux transparents lors d'un redimensionnement, afin que le
    ' feuillage suive le bord droit de la fenêtre.
    '
    ' Paramètres :
    ' - sender : pnlForm
    ' - e      : arguments standard
    '
    ' Remarques  :
    ' - Seule la zone du feuillage est invalidée, sans les enfants (voir InvaliderFeuillage).
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub pnlForm_Resize(sender As Object, e As EventArgs) Handles pnlForm.Resize

        InvaliderFeuillage(pnlForm)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : pnlContent_ControlAdded / pnlContent_ControlRemoved
    ' Version    : V1.0.0
    ' Date       : 09/10/2026
    '
    ' Rôle       :
    ' Branche (ou débranche) la suite du feuillage sur chaque UserControl chargé dans pnlContent.
    '
    ' Paramètres :
    ' - sender : pnlContent
    ' - e      : contrôle ajouté ou retiré (le UserControl métier)
    '
    ' Remarques  :
    ' - NavigationManager ajoute le UserControl via Controls.Add et le libère via Dispose :
    '   ces deux événements couvrent donc toute la navigation.
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub pnlContent_ControlAdded(sender As Object, e As ControlEventArgs) Handles pnlContent.ControlAdded

        AttacherFeuillageCoupe(e.Control)

    End Sub

    Private Sub pnlContent_ControlRemoved(sender As Object, e As ControlEventArgs) Handles pnlContent.ControlRemoved

        DetacherFeuillageCoupe(e.Control)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AttacherFeuillageCoupe
    ' Version    : V1.0.1
    ' Date       : 18/10/2026
    '
    ' Rôle       :
    ' Prépare la surface de fond d'un UserControl hébergé pour qu'elle affiche la suite exacte
    ' du feuillage de la Home (« feuille coupée »), à l'endroit où le UserControl le recouvre.
    '
    ' Paramètres :
    ' - uc : UserControl ajouté dans pnlContent
    '
    ' Remarques  :
    ' - Surface = le pnlForm du UserControl (Dock Fill) s'il existe, sinon le UserControl lui-même.
    ' - Toute image de fond posée dans le Designer est retirée à l'exécution : la Home fournit la
    '   suite du feuillage, parfaitement alignée et sans étirement (l'ancien fond étiré
    '   Fond_1000x770_FeuilleCoupee1 a été supprimé au lot UI-8, 18/10/2026).
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub AttacherFeuillageCoupe(uc As Control)

        If uc Is Nothing OrElse _surfacesFeuillage.ContainsKey(uc) Then Return

        Dim surface As Control = uc
        Dim trouves As Control() = uc.Controls.Find("pnlForm", False)
        If trouves.Length > 0 AndAlso trouves(0).Dock = DockStyle.Fill Then
            surface = trouves(0)
        End If

        surface.BackgroundImage = Nothing
        AttacherFeuillage(surface)
        _surfacesFeuillage(uc) = surface

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AttacherFeuillage
    ' Version    : V1.0.0
    ' Date       : 10/10/2026
    '
    ' Rôle       :
    ' Fait dessiner à une surface la portion de feuillage qu'elle recouvre.
    '
    ' Paramètres :
    ' - surface : panneau transparent posé sur pnlForm (bandeau, zone de contenu, surface d'un UC)
    '
    ' Remarques  :
    ' - **Indispensable pour tout panneau transparent posé sur pnlForm.** La transparence de
    '   WinForms ne reproduit que le *fond* du parent (sa couleur et son image) : elle ignore
    '   ce que le parent dessine dans son événement Paint. Un panneau transparent efface donc
    '   le feuillage en se repeignant, et celui-ci se met à clignoter (constaté le 10/10/2026
    '   sur le bandeau de la Home). Chaque surface doit dessiner sa propre portion.
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub AttacherFeuillage(surface As Control)

        If surface Is Nothing Then Return

        ActiverDoubleTampon(surface)

        AddHandler surface.Paint, AddressOf Surface_Paint
        AddHandler surface.Resize, AddressOf Surface_Resize

        InvaliderFeuillage(surface)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ActiverDoubleTampon
    ' Version    : V1.0.0
    ' Date       : 10/10/2026
    '
    ' Rôle       :
    ' Active le double tampon d'un contrôle qui ne l'expose pas (Panel, UserControl…).
    '
    ' Paramètres :
    ' - controle : surface sur laquelle un dessin est effectué dans Paint
    '
    ' Remarques  :
    ' - `Control.DoubleBuffered` est une propriété protégée : hors d'une classe dérivée, elle
    '   n'est accessible que par réflexion. C'est l'usage établi pour un contrôle du Designer.
    ' - Sans double tampon, une surface transparente qui dessine dans Paint laisse voir
    '   l'instant où son fond est effacé avant que le dessin ne soit refait : dès que des
    '   repeints se succèdent (grille, filtres, sélecteurs de date), le dessin clignote.
    '   Avec, l'effacement et le redessin sont composés hors écran.
    ' - Un échec de réflexion (version de .NET) est sans gravité : le dessin reste correct,
    '   seul le confort visuel est en jeu.
    '
    ' Exceptions :
    ' - Aucune (toute erreur de réflexion est absorbée)
    ' -------------------------------------------------------------------------------------------------
    Private Shared Sub ActiverDoubleTampon(controle As Control)

        If controle Is Nothing Then Return

        Try

            Dim propriete As Reflection.PropertyInfo =
                GetType(Control).GetProperty("DoubleBuffered",
                                             Reflection.BindingFlags.Instance Or
                                             Reflection.BindingFlags.NonPublic)

            propriete?.SetValue(controle, True, Nothing)

        Catch
            ' Volontairement ignoré : voir Remarques.
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DetacherFeuillageCoupe
    ' Version    : V1.0.0
    ' Date       : 09/10/2026
    '
    ' Rôle       :
    ' Retire les gestionnaires de dessin posés par AttacherFeuillageCoupe.
    '
    ' Paramètres :
    ' - uc : UserControl retiré de pnlContent
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub DetacherFeuillageCoupe(uc As Control)

        Dim surface As Control = Nothing
        If uc Is Nothing OrElse Not _surfacesFeuillage.TryGetValue(uc, surface) Then Return

        RemoveHandler surface.Paint, AddressOf Surface_Paint
        RemoveHandler surface.Resize, AddressOf Surface_Resize
        _surfacesFeuillage.Remove(uc)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Surface_Paint
    ' Version    : V1.0.0
    ' Date       : 09/10/2026
    '
    ' Rôle       :
    ' Dessine sur la surface du UserControl la portion du feuillage qu'il recouvre, en reprenant
    ' exactement la position du feuillage de la Home : le motif paraît continu.
    '
    ' Paramètres :
    ' - sender : surface du UserControl (pnlForm ou UserControl)
    ' - e      : contexte de dessin
    '
    ' Remarques  :
    ' - La position est calculée par rapport au pnlForm de la Home : elle reste juste quels que
    '   soient le Padding de pnlContent, la taille de la fenêtre ou la mise à l'échelle Windows.
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub Surface_Paint(sender As Object, e As PaintEventArgs)

        DessinerFeuillage(e, TryCast(sender, Control))

    End Sub

    Private Sub Surface_Resize(sender As Object, e As EventArgs)

        InvaliderFeuillage(TryCast(sender, Control))

    End Sub

#End Region

#Region "Fin de l'élévation après inactivité"

    ' Contrôle périodique : une élévation sans activité depuis SECURITE_ELEVATION_MINUTES prend fin.
    Private Sub MinuterieElevation_Tick(sender As Object, e As EventArgs) Handles _minuterieElevation.Tick

        If _finElevationEnCours OrElse _userSession Is Nothing OrElse Not _userSession.IsElevated Then Return

        ' Une fenêtre modale est ouverte par-dessus la Home (fiche utilisateur, séance…) : on attend sa
        ' fermeture, l'écran qui l'a ouverte ne doit pas disparaître sous elle. Seules les fenêtres
        ' modales VISIBLES comptent : une fenêtre seulement cachée restait dans OpenForms et pouvait
        ' bloquer la fin de l'élévation (relevé de Joëlle, 21/10/2026).
        Dim minutes As Integer =
            GestionParametres.GetEntier(ClesParametres.SecuriteElevationMinutes, ClesParametres.DefautElevationMinutes)

        ' Une ligne par élévation, au premier contrôle : la surveillance tourne, et avec quelle durée.
        If Not Nullable.Equals(_elevationSurveillee, _userSession.ElevationDateTime) Then
            _elevationSurveillee = _userSession.ElevationDateTime
            GestionLog.EcrireLog($"Élévation surveillée : fin après {minutes} min sans activité ({_userSession.UserName}).",
                                 GestionLog.LogLevel.Rapide, GestionLog.LogCategory.Security)
        End If

        If (DateTime.Now - _userSession.LastActivityDateTime).TotalMinutes < minutes Then Return

        If FenetreModaleOuverte() Then
            ' Trace une seule fois par report : sans elle, une fin d'élévation qui n'arrive pas serait
            ' impossible à expliquer.
            If Not _finElevationReportee Then
                GestionLog.EcrireLog("Fin de l'élévation reportée : une fenêtre est ouverte par-dessus la Home.",
                                     GestionLog.LogLevel.Complet, GestionLog.LogCategory.Security)
                _finElevationReportee = True
            End If
            Return
        End If
        _finElevationReportee = False

        TerminerElevationParInactivite(minutes)

    End Sub

    Private Function FenetreModaleOuverte() As Boolean
        For Each fenetre As Form In Application.OpenForms
            If fenetre IsNot Me AndAlso fenetre.Visible AndAlso fenetre.Modal Then Return True
        Next
        Return False
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : TerminerElevationParInactivite
    ' Version    : V1.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       :
    ' Ramène la session au rôle de base, le dit à l'utilisateur, et quitte une zone réservée
    ' (Administration, Référentiels) pour l'accueil.
    '
    ' Remarques  :
    ' - Décision de Joëlle (20/10/2026) : fin après inactivité, pas après une durée fixe.
    ' - Garde _finElevationEnCours : la minuterie continue de battre pendant le message.
    ' - Pas pendant qu'une fenêtre est ouverte par-dessus la Home : fin au contrôle suivant sa fermeture.
    ' - Le retour à l'accueil passe par la navigation habituelle : des modifications non enregistrées
    '   déclenchent la question Enregistrer / Quitter / Rester (l'enregistrement d'une action
    '   réservée sera alors refusé par le métier : il faut d'abord se réélever).
    ' -------------------------------------------------------------------------------------------------
    Private Sub TerminerElevationParInactivite(minutes As Integer)

        _finElevationEnCours = True
        Try
            Dim roleEleve As AppRole = _userSession.CurrentRole
            _userSession.ResetElevation()

            GestionLog.EcrireLog(
                $"Élévation terminée après {minutes} min d'inactivité ({_userSession.UserName} : {roleEleve} → {_userSession.CurrentRole}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security)

            UpdateConnectedUserDisplay()
            stsLabelStatus.Text = "Élévation terminée (inactivité)."

            DialogChoix.Information(
                $"Vos droits élevés ({roleEleve}) ont pris fin après {minutes} minutes sans activité : " &
                $"vous êtes revenu(e) au rôle {_userSession.CurrentRole}." & Environment.NewLine & Environment.NewLine &
                "Pour une action réservée, élevez de nouveau vos droits (cadenas).",
                "Fin de l'élévation")

            If _boutonMenuCourant Is btnAdmin OrElse _boutonMenuCourant Is btnReferentiels Then
                NavigateToAccueil()
            End If
        Finally
            _finElevationEnCours = False
        End Try

    End Sub

#End Region

#Region "Navigation menu principal"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SelectHomeMenuButton
    ' Version    : V1.0.1
    ' Date       : 20/10/2026
    '
    ' Rôle       :
    ' Définit le bouton actif du menu principal, met à jour l'état visuel, affiche le statut,
    ' et journalise la sélection.
    '
    ' Paramètres :
    ' - selectedButton : Bouton à marquer comme sélectionné (Button)
    ' - statusText     : Texte à afficher dans la barre de statut (String)
    '
    ' Remarques  :
    ' - Appelée par NavigateTo() et par les handlers de clic simples (btnPatients_Click, etc.)
    ' - Utilise UtilsButtons.SetSelectedHomeMenuButton() pour gérer l'état visuel de tous les boutons
    ' - Le bouton sélectionné reste visuellement actif jusqu'au choix d'un autre menu
    ' - Met à jour stsLabelStatus.Text avec le contexte fourni
    ' - Journalise la sélection dans GestionLog (niveau Rapide, catégorie UI) : le NOM du bouton,
    '   jamais le texte affiché, qui peut contenir le nom d'un patient (fil d'Ariane ; audit B3).
    ' - V1.0.1 (20/10/2026) : nom du bouton au lieu du texte de statut dans le journal (B3, RGPD).
    ' - Ne charge pas de UserControl (c'est le rôle de NavigateTo())
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub SelectHomeMenuButton(selectedButton As Button, statusText As String)

        UtilsButtons.SetSelectedHomeMenuButton(selectedButton, _homeMenuButtons)
        _boutonMenuCourant = selectedButton

        stsLabelStatus.Text = statusText

        GestionLog.EcrireLog(
        $"Menu Home sélectionné : {selectedButton?.Name}",
        GestionLog.LogLevel.Rapide,
        GestionLog.LogCategory.UI)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnAccueil_Click
    ' Version    : V1.1.0
    ' Date       : 13/05/2026
    '
    ' Rôle       :
    ' Retourne à l'accueil via NavigateToAccueil().
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement (Object)
    ' - e      : Arguments de l'événement Click (EventArgs)
    '
    ' Remarques  :
    ' - Utilisé pour revenir à l'écran d'accueil depuis n'importe quelle vue
    ' - NavigateToAccueil() charge UC_Accueil, sélectionne btnAccueil, et met à jour le contexte
    ' - Utilisé notamment après une perte d'accès à une zone réservée
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAccueil_Click(
    sender As Object,
    e As EventArgs
) Handles btnAccueil.Click

        NavigateToAccueil()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnPatients_Click
    ' Version    : V1.1.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Active l'environnement Patients en chargeant l'écran d'accueil des patients (UC_PatientHome).
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement (Object)
    ' - e      : Arguments de l'événement Click (EventArgs)
    '
    ' Remarques  :
    ' - Charge une nouvelle instance de UC_PatientHome via NavigateTo() avec btnPatients sélectionné
    ' - NavigateTo() injecte automatiquement le contexte UI partagé (IContextAwareUserControl)
    '
    ' Exceptions :
    ' - Aucune gestion explicite (erreurs propagées par NavigateTo)
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnPatients_Click(sender As Object, e As EventArgs) Handles btnPatients.Click

        NavigateTo(
            New UC_PatientHome(),
            btnPatients,
            "Patients"
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnRecherche_Click
    ' Version    : V1.1.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Ouvre le hub Recherche (UC_RechercheHome, recherche transversale, ADR-52) : NavigateToRecherche.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnRecherche_Click(sender As Object, e As EventArgs) Handles btnRecherche.Click
        NavigateToRecherche()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnDomaines_Click
    ' Version    : V1.0.0
    ' Date       : 25/04/2026
    '
    ' Rôle       :
    ' Active l'environnement Domaines (en attente d'implémentation complète).
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement (Object)
    ' - e      : Arguments de l'événement Click (EventArgs)
    '
    ' Remarques  :
    ' - Actuellement : sélectionne uniquement le bouton et met à jour le statut
    ' - À terme : devra charger le UserControl de gestion des domaines via NavigateTo()
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnDomaines_Click(sender As Object, e As EventArgs) Handles btnDomaines.Click

        NavigateTo(
            New UC_DomainesHome(),
            btnDomaines,
            "Domaines"
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnAgenda_Click
    ' Version    : V1.0.0
    ' Date       : 25/04/2026
    '
    ' Rôle       :
    ' Active l'environnement Agenda (en attente d'implémentation complète).
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement (Object)
    ' - e      : Arguments de l'événement Click (EventArgs)
    '
    ' Remarques  :
    ' - Actuellement : sélectionne uniquement le bouton et met à jour le statut
    ' - À terme : devra charger le UserControl de gestion de l'agenda via NavigateTo()
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAgenda_Click(sender As Object, e As EventArgs) Handles btnAgenda.Click

        SelectHomeMenuButton(btnAgenda, "Agenda")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnDocuments_Click
    ' Version    : V1.0.0
    ' Date       : 25/04/2026
    '
    ' Rôle       :
    ' Active l'environnement Documents (en attente d'implémentation complète).
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement (Object)
    ' - e      : Arguments de l'événement Click (EventArgs)
    '
    ' Remarques  :
    ' - Actuellement : sélectionne uniquement le bouton et met à jour le statut
    ' - À terme : devra charger le UserControl de gestion des documents via NavigateTo()
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnDocuments_Click(sender As Object, e As EventArgs) Handles btnDocuments.Click

        SelectHomeMenuButton(btnDocuments, "Documents")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnReferentiels_Click
    ' Version    : V1.2.0
    ' Date       : 25/10/2026
    '
    ' Rôle       :
    ' Ouvre l'espace Référentiels (UC_ReferentielHome) avec contrôle d'accès et élévation de privilèges si nécessaire.
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement (Object)
    ' - e      : Arguments de l'événement Click (EventArgs)
    '
    ' Remarques  :
    ' - Vérifie d'abord si l'utilisateur a les droits d'accès via PeutAccederReferentielHome()
    ' - Si droits insuffisants : propose une élévation temporaire de privilèges via ElevationAcces
    ' - Si l'utilisateur refuse l'élévation : annulation avec message de statut
    ' - Si l'élévation échoue : retour sans chargement de UC_ReferentielHome
    ' - Après élévation réussie : mise à jour de l'affichage utilisateur (UpdateConnectedUserDisplay)
    ' - Charge UC_ReferentielHome via NavigateTo() avec le contexte "Référentiels"
    ' - En cas d'erreur : log (Succinct/UI) + MessageBox d'erreur
    '
    ' Exceptions :
    ' - Toutes les exceptions sont capturées, journalisées, et affichées à l'utilisateur
    ' - V1.2.0 (25/10/2026, audit C.4, ADR-55) : élévation par ObtenirElevationPourZone (bloc commun
    '   aux quatre zones réservées).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnReferentiels_Click(sender As Object, e As EventArgs) Handles btnReferentiels.Click

        Try

            If Not PeutAccederReferentielHome() AndAlso Not ObtenirElevationPourZone("Accès Référentiels annulé.") Then
                Return
            End If

            NavigateTo(
            New UC_ReferentielHome(_userSession, _authenticatedUser),
            btnReferentiels,
            "Référentiels"
        )

        Catch ex As Exception

            GestionLog.EcrireLog(
            "Erreur btnReferentiels_Click.",
            GestionLog.LogLevel.Succinct,
            GestionLog.LogCategory.UI,
            ex
        )

            DialogChoix.Erreur(
            "Erreur lors de l'ouverture des référentiels.",
            "Erreur"
        )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnAdmin_Click
    ' Version    : V1.6.0
    ' Date       : 25/10/2026
    '
    ' Rôle       :
    ' Ouvre l'espace Administration (UC_AdminHome) avec contrôle d'accès et élévation de privilèges si nécessaire.
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement (Object)
    ' - e      : Arguments de l'événement Click (EventArgs)
    '
    ' Remarques  :
    ' - Vérifie d'abord si l'utilisateur a les droits d'accès via PeutAccederAdminHome()
    ' - Si droits insuffisants : propose une élévation temporaire de privilèges via ElevationAcces
    ' - Si l'utilisateur refuse l'élévation : annulation avec message de statut
    ' - Si l'élévation échoue : retour sans chargement de UC_AdminHome
    ' - Après élévation réussie : mise à jour de l'affichage utilisateur (UpdateConnectedUserDisplay)
    ' - Charge UC_AdminHome via NavigateTo() avec le contexte "Administration"
    ' - En cas d'erreur : log (Succinct/UI) + MessageBox d'erreur
    '
    ' Exceptions :
    ' - Toutes les exceptions sont capturées, journalisées, et affichées à l'utilisateur
    ' - V1.6.0 (25/10/2026, audit C.4, ADR-55) : élévation par ObtenirElevationPourZone (bloc commun
    '   aux quatre zones réservées).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAdmin_Click(
    sender As Object,
    e As EventArgs
) Handles btnAdmin.Click

        Try

            If Not PeutAccederAdminHome() AndAlso Not ObtenirElevationPourZone("Accès Administration annulé.") Then
                Return
            End If

            NavigateTo(
            New UC_AdminHome(_userSession, _authenticatedUser),
            btnAdmin,
            "Administration"
        )

        Catch ex As Exception

            GestionLog.EcrireLog(
            "Erreur btnAdmin_Click.",
            GestionLog.LogLevel.Succinct,
            GestionLog.LogCategory.UI,
            ex
        )

            DialogChoix.Erreur(
            "Erreur lors de l'ouverture de l'administration.",
            "Erreur"
        )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnChangerMotDePasse_Click
    ' Version    : V1.0.0
    ' Date       : 01/06/2026
    '
    ' Rôle       :
    ' Ouvre la Form ChangePassword en mode UserChange pour permettre à l'utilisateur de changer son mot de passe.
    '
    ' Paramètres :
    ' - sender : Objet source de l'événement (Object)
    ' - e      : Arguments de l'événement Click (EventArgs)
    '
    ' Remarques  :
    ' - Accessible à tous les utilisateurs authentifiés
    ' - N'affecte pas la navigation (pas de sélection de bouton menu)
    ' - En cas d'erreur : log (Succinct/UI) + MessageBox
    '
    ' Exceptions :
    ' - Toutes les exceptions sont capturées et journalisées
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnChangerMotDePasse_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnChangerMotDePasse.Click

        Try

            Using frmChangePassword As New ChangePassword(
                _authenticatedUser,
                ModeChangePassword.UserChange
            )

                frmChangePassword.ShowDialog(Me)

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur btnChangerMotDePasse_Click.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Information(
                "Erreur lors de l'ouverture du changement de mot de passe.",
                "Erreur"
            )

        End Try

    End Sub

#End Region

#Region "Navigation - Chargement des vues"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : NavigateTo
    ' Version    : V1.3.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Navigue vers un UserControl métier et synchronise le menu, le contenu (pnlContent),
    ' le contexte de navigation (lblContexte), et le StatusStrip.
    '
    ' Paramètres :
    ' - view           : UserControl à charger dans pnlContent (UserControl)
    ' - selectedButton : Bouton du menu à marquer comme sélectionné (Button)
    ' - statusText     : Texte de contexte à afficher dans lblContexte et stsLabelStatus (String)
    '
    ' Remarques  :
    ' - Séquence de navigation :
    '   1. SelectHomeMenuButton(selectedButton, statusText) : mise à jour de l'état visuel du menu
    '   2. _navManager.Navigate(view) : chargement du UserControl dans pnlContent (décharge l'ancien, charge le nouveau)
    '   3. SetContexte(statusText) : mise à jour de lblContexte et stsLabelStatus
    ' - Le contexte est appliqué APRES la navigation pour éviter qu'un UserControl ne l'écrase pendant son chargement
    ' - _navManager injecte automatiquement _uiContext aux UserControls implémentant IContextAwareUserControl
    ' - Utilisée par tous les handlers de navigation : btnAccueil_Click, btnAdmin_Click, NavigateToAccueil, etc.
    ' - Méthode centrale de navigation : toute navigation doit passer par ici pour garantir la cohérence UI
    ' - Réinitialise l'historique de navigation (mini-pile D-Q15) : un clic sur le menu principal est une
    '   navigation "racine" qui repart d'une chaîne de retour propre
    '
    ' Exceptions :
    ' - Aucune gestion explicite (erreurs propagées par NavigationManager)
    ' -------------------------------------------------------------------------------------------------
    Private Sub NavigateTo(
    view As UserControl,
    selectedButton As Button,
    statusText As String
)

        If Not _navManager.PeutQuitterVueCourante() Then
            Return
        End If

        _navManager.ClearHistory()

        ' La pastille du fil d'Ariane (domaine du dossier) appartient à l'écran quitté.
        lblContexte.Pastille = String.Empty

        SelectHomeMenuButton(
        selectedButton,
        statusText
    )

        _navManager.Navigate(view)

        SetContexte(statusText)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : NavigateToAdminView
    ' Version    : V1.1.0
    ' Date       : 13/05/2026
    '
    ' Rôle       :
    ' Navigue vers un écran d'administration (sous-UserControl de UC_AdminHome) avec contexte hiérarchique.
    '
    ' Paramètres :
    ' - view       : UserControl d'administration à charger (ex: UC_Parametres) (UserControl)
    ' - statusText : Texte du sous-contexte (ex: "Paramètres") (String)
    '
    ' Remarques  :
    ' - Appelée publiquement par les UserControls d'administration (ex: UC_AdminHome.btnParametres_Click)
    ' - Construit un contexte hiérarchique via BuildAdminContexte() : "Administration > [statusText]"
    ' - Navigue via NavigateTo() avec btnAdmin sélectionné pour maintenir la cohérence du menu
    ' - Permet de naviguer entre les différents écrans d'administration sans repasser par UC_AdminHome
    ' - Le contexte hiérarchique indique clairement la position dans l'arborescence : "Administration > Connexion DB", etc.
    '
    ' Exceptions :
    ' - Aucune gestion explicite (erreurs propagées par NavigateTo)
    ' -------------------------------------------------------------------------------------------------
    Public Sub NavigateToAdminView(
    view As UserControl,
    statusText As String
) Implements IFenetrePrincipale.NavigateToAdminView

        Dim contexte As String =
        BuildAdminContexte(statusText)

        NavigateTo(
        view,
        btnAdmin,
        contexte
    )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : NavigateToReferentielView
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Navigue vers un écran de référentiel (sous-UserControl de UC_ReferentielHome) avec contexte hiérarchique.
    '
    ' Paramètres :
    ' - view       : UserControl de référentiel à charger (ex: UC_Pays) (UserControl)
    ' - statusText : Texte du sous-contexte (ex: "Domaines") (String)
    '
    ' Remarques  :
    ' - Appelée publiquement par les UserControls de référentiels (ex: UC_ReferentielHome.btnDomaines_Click)
    ' - Construit un contexte hiérarchique via BuildReferentielContexte() : "Référentiels > [statusText]"
    ' - Navigue via NavigateTo() avec btnReferentiels sélectionné pour maintenir la cohérence du menu
    ' - Permet de naviguer entre les différents écrans de référentiels sans repasser par UC_ReferentielHome
    '
    ' Exceptions :
    ' - Aucune gestion explicite (erreurs propagées par NavigateTo)
    ' -------------------------------------------------------------------------------------------------
    Public Sub NavigateToReferentielView(
    view As UserControl,
    statusText As String
) Implements IFenetrePrincipale.NavigateToReferentielView

        Dim contexte As String =
        BuildReferentielContexte(statusText)

        NavigateTo(
        view,
        btnReferentiels,
        contexte
    )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : NavigateToAccueil
    ' Version    : V1.0.0
    ' Date       : 12/05/2026
    '
    ' Rôle       :
    ' Retourne vers l'écran d'accueil (UC_Accueil) avec contexte "Accueil".
    '
    ' Paramètres :
    ' - Aucun
    '
    ' Remarques  :
    ' - Appelée publiquement par les UserControls pour revenir à l'accueil
    ' - Utilisée notamment après perte d'accès à un écran réservé (ex: UC_AdminHome après retour au rôle de base)
    ' - Navigue via NavigateTo() avec btnAccueil sélectionné et contexte "Accueil"
    ' - Charge une nouvelle instance de UC_Accueil à chaque appel (pas de réutilisation)
    '
    ' Exceptions :
    ' - Aucune gestion explicite (erreurs propagées par NavigateTo)
    ' -------------------------------------------------------------------------------------------------
    Public Sub NavigateToAccueil() Implements IFenetrePrincipale.NavigateToAccueil

        NavigateTo(
        New UC_Accueil(),
        btnAccueil,
        "Accueil"
    )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : NavigateToPatients
    ' Version    : V1.0.0
    ' Date       : 12/06/2026
    '
    ' Rôle       :
    ' Retourne vers l'écran d'accueil des patients (UC_PatientHome) avec contexte "Patients".
    '
    ' Paramètres :
    ' - Aucun
    '
    ' Remarques  :
    ' - Appelée publiquement par les UserControls patients (ex : UC_PatientFiche) pour revenir
    '   à la liste lorsque l'historique de navigation est vide
    ' - Charge une nouvelle instance de UC_PatientHome à chaque appel (pas de réutilisation)
    '
    ' Exceptions :
    ' - Aucune gestion explicite (erreurs propagées par NavigateTo)
    ' -------------------------------------------------------------------------------------------------
    Public Sub NavigateToPatients() Implements IFenetrePrincipale.NavigateToPatients

        NavigateTo(
        New UC_PatientHome(),
        btnPatients,
        "Patients"
    )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : NavigateToRecherche
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Ouvre le hub Recherche (bouton du menu ; Retour d'une recherche spécialisée quand la mini-pile
    ' est vide).
    ' -------------------------------------------------------------------------------------------------
    Public Sub NavigateToRecherche() Implements IFenetrePrincipale.NavigateToRecherche

        NavigateTo(
            New UC_RechercheHome(),
            btnRecherche,
            OuvertureResultat.CONTEXTE_RECHERCHE
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : NavigateDepuisRecherche
    ' Version    : V1.1.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Ouvre l'écran d'un élément trouvé par la recherche (fiche patient, hub dossier) en empilant la
    ' recherche dans la mini-pile (D-Q15) : Retour la retrouve, critères et résultats compris.
    '
    ' Paramètres :
    ' - vue               : Écran déjà instancié et initialisé par l'appelant (UserControl)
    ' - contexte          : Libellé de contexte à afficher (String)
    ' - creerEcranCourant : Fabrique recréant la recherche à l'identique au retour (Func(Of UserControl))
    ' - contexteRetour    : Libellé de contexte de la recherche, réaffiché au retour (String,
    '                       « Recherche » par défaut ; « Recherche > Séances »…)
    '
    ' Remarques  :
    ' - Appelée par OuvertureResultat et par le hub Recherche (ouverture d'une recherche spécialisée,
    '   ADR-52) ; encapsule btnRecherche.
    ' - V1.1.0 (23/10/2026, recherche R2) : contexteRetour (recherches spécialisées).
    ' -------------------------------------------------------------------------------------------------
    Public Sub NavigateDepuisRecherche(
        vue As UserControl,
        contexte As String,
        creerEcranCourant As Func(Of UserControl),
        Optional contexteRetour As String = OuvertureResultat.CONTEXTE_RECHERCHE
    ) Implements IFenetrePrincipale.NavigateDepuisRecherche

        NavigateAvecHistorique(
            vue,
            btnRecherche,
            contexte,
            New NavigationEntry(creerEcranCourant, btnRecherche, contexteRetour)
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : NavigateToPatientFiche
    ' Version    : V1.0.0
    ' Date       : 12/06/2026
    '
    ' Rôle       :
    ' Ouvre la fiche patient (UC_PatientFiche) en empilant l'écran patients courant dans la
    ' mini-pile de navigation (D-Q15) pour permettre un retour à la liste avec son filtre restauré.
    '
    ' Paramètres :
    ' - fiche             : Fiche patient déjà instanciée et initialisée (mode défini par l'appelant) (UserControl)
    ' - contexte          : Libellé de contexte hiérarchique à afficher (ex : "Patients > Nouveau patient") (String)
    ' - creerEcranCourant : Fabrique recréant l'écran patients courant à l'identique au retour
    '                       (capture le filtre par closure) (Func(Of UserControl))
    '
    ' Remarques  :
    ' - Appelée publiquement par UC_PatientHome ; encapsule btnPatients (privé) et la construction
    '   du NavigationEntry pour rester cohérent avec le menu principal
    ' - Le bouton de menu resélectionné au retour reste btnPatients
    '
    ' Exceptions :
    ' - Aucune gestion explicite (erreurs propagées par NavigateAvecHistorique)
    ' -------------------------------------------------------------------------------------------------
    Public Sub NavigateToPatientFiche(
        fiche As UserControl,
        contexte As String,
        creerEcranCourant As Func(Of UserControl)
    ) Implements IFenetrePrincipale.NavigateToPatientFiche

        Dim etapeCourante As New NavigationEntry(
            creerEcranCourant,
            btnPatients,
            "Patients"
        )

        NavigateAvecHistorique(
            fiche,
            btnPatients,
            contexte,
            etapeCourante
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : NavigateToDossierFiche
    ' Version    : V1.0.0
    ' Date       : 21/07/2026
    '
    ' Rôle       :
    ' Ouvre le hub d'un dossier (UC_DossierFiche) en empilant l'écran patient courant dans la
    ' mini-pile de navigation (D-Q15), pour permettre un retour à la fiche patient restaurée.
    '
    ' Paramètres :
    ' - hub               : Hub dossier déjà instancié et initialisé (mode défini par l'appelant) (UserControl)
    ' - contexte          : Libellé de contexte hiérarchique à afficher (ex : "Patients > Dupont Jean > DO000012") (String)
    ' - creerEcranCourant : Fabrique recréant la fiche patient courante à l'identique au retour
    '                       (capture le patient et l'onglet Dossiers par closure) (Func(Of UserControl))
    '
    ' Remarques  :
    ' - Jumeau de NavigateToPatientFiche, un niveau plus bas ; encapsule btnPatients (privé)
    ' - Le bouton de menu resélectionné au retour reste btnPatients (parcours patient)
    '
    ' Exceptions :
    ' - Aucune gestion explicite (erreurs propagées par NavigateAvecHistorique)
    ' -------------------------------------------------------------------------------------------------
    Public Sub NavigateToDossierFiche(
        hub As UserControl,
        contexte As String,
        creerEcranCourant As Func(Of UserControl)
    ) Implements IFenetrePrincipale.NavigateToDossierFiche

        Dim etapeCourante As New NavigationEntry(
            creerEcranCourant,
            btnPatients,
            "Patients"
        )

        NavigateAvecHistorique(
            hub,
            btnPatients,
            contexte,
            etapeCourante
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : NavigateAvecHistorique
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Navigue vers une vue métier en empilant l'écran COURANT dans la mini-pile de navigation (D-Q15),
    ' afin de permettre un retour ultérieur via NavigateRetour().
    '
    ' Paramètres :
    ' - nouvelleVue    : UserControl à charger dans pnlContent (UserControl)
    ' - boutonMenu     : Bouton du menu à marquer comme sélectionné (Button)
    ' - contexte       : Libellé de contexte à afficher (lblContexte / stsLabelStatus) (String)
    ' - etapeCourante  : Étape décrivant l'écran actuel (fabrique + menu + contexte + filtre) à empiler
    '                    pour pouvoir y revenir (NavigationEntry)
    '
    ' Remarques  :
    ' - Appelée par les UserControls métier pour enchaîner PatientHome → Fiche → Dossier...
    ' - etapeCourante.CreerVue doit recréer l'écran courant à l'identique (filtre restauré par closure)
    ' - Synchronise menu + contexte comme NavigateTo(), tout en alimentant l'historique
    '
    ' Exceptions :
    ' - Aucune gestion explicite (erreurs propagées par NavigationManager)
    ' -------------------------------------------------------------------------------------------------
    Public Sub NavigateAvecHistorique(
        nouvelleVue As UserControl,
        boutonMenu As Button,
        contexte As String,
        etapeCourante As NavigationEntry
    )

        If Not _navManager.PeutQuitterVueCourante() Then
            Return
        End If

        SelectHomeMenuButton(boutonMenu, contexte)

        _navManager.NavigateAndPush(nouvelleVue, etapeCourante)

        SetContexte(contexte)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : NavigateRetour
    ' Type       : Boolean
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Revient à l'écran précédent de la mini-pile de navigation (D-Q15) et restaure son menu et
    ' son contexte. Le dernier filtre de recherche est rétabli via la fabrique de l'étape.
    '
    ' Retour     :
    ' - True  : si un retour a eu lieu (une étape précédente existait)
    ' - False : si l'historique était vide (aucune action)
    '
    ' Remarques  :
    ' - Appelée par les UserControls métier (bouton "Retour") pour remonter la chaîne de navigation
    ' - NavigationManager recrée la vue précédente ; Home resélectionne le bouton et réaffiche le contexte
    '
    ' Exceptions :
    ' - Aucune gestion explicite (erreurs propagées par NavigationManager)
    ' -------------------------------------------------------------------------------------------------
    Public Function NavigateRetour() As Boolean Implements IFenetrePrincipale.NavigateRetour

        If Not _navManager.PeutQuitterVueCourante() Then
            Return True
        End If

        Dim etapePrecedente As NavigationEntry = _navManager.NavigateBack()

        If etapePrecedente Is Nothing Then
            Return False
        End If

        If etapePrecedente.MenuButton IsNot Nothing Then
            SelectHomeMenuButton(etapePrecedente.MenuButton, etapePrecedente.Contexte)
        End If

        SetContexte(etapePrecedente.Contexte)

        Return True

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : OuvrirReferentielModal
    ' Type       : Boolean
    ' Version    : V1.1.0
    ' Date       : 25/10/2026
    '
    ' Rôle       :
    ' Ouvre un écran de référentiel existant (UC_Ref<X>) dans une fenêtre modale générique
    ' (ReferentielModalHost), pour permettre l'ajout d'une valeur de référentiel « en contexte »
    ' depuis un bouton [+] près d'un combo (D-Q17). Réutilise le contrôle de droit et l'élévation
    ' déjà en place pour l'espace Référentiels (même mécanique que btnReferentiels_Click).
    '
    ' Paramètres :
    ' - vueReferentiel : UserControl de référentiel à héberger (ex : New UC_Pays()) (UserControl)
    ' - titre          : Titre de la fenêtre modale (ex : "Domaines") (String)
    '
    ' Retour     :
    ' - True  : si le modal a été affiché (droits suffisants ou élévation acceptée)
    ' - False : si l'accès a été refusé / annulé (aucun modal affiché)
    '
    ' Remarques  :
    ' - Aucune nouvelle UI par référentiel : on réutilise les UC_Ref<X> existants (pas de duplication)
    ' - Le contrôle de droit + élévation est centralisé ici (l'hôte modal ne gère pas la sécurité)
    ' - L'appelant peut utiliser le retour True pour rafraîchir le combo associé après fermeture
    '
    ' Exceptions :
    ' - Toutes les exceptions sont capturées, journalisées (Succinct/UI) et signalées à l'utilisateur
    ' - V1.1.0 (25/10/2026, audit C.4, ADR-55) : élévation par ObtenirElevationPourZone (bloc commun
    '   aux quatre zones réservées).
    ' -------------------------------------------------------------------------------------------------
    Public Function OuvrirReferentielModal(
        vueReferentiel As UserControl,
        titre As String
    ) As Boolean Implements IFenetrePrincipale.OuvrirReferentielModal

        Try

            If Not PeutAccederReferentielHome() AndAlso Not ObtenirElevationPourZone("Ajout de référentiel annulé.") Then
                Return False
            End If

            Using frmHost As New ReferentielModalHost(
                vueReferentiel,
                titre,
                _userSession,
                _authenticatedUser
            )

                frmHost.ShowDialog(Me)

            End Using

            Return True

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur OuvrirReferentielModal.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur(
                "Erreur lors de l'ouverture du référentiel.",
                "Erreur"
            )

            Return False

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : DemanderElevation
    ' Version    : V1.0.0
    ' Date       : 25/10/2026
    '
    ' Rôle       :
    ' Élévation demandée par un écran (bouton « Élever l'accès » des hubs Administration et
    ' Référentiels) : fil d'Ariane temporaire pendant la fenêtre, puis fenêtre d'élévation.
    '
    ' Paramètres :
    ' - contexteTemporaire : fil d'Ariane affiché pendant la fenêtre (« Administration > Élévation d'accès »)
    '
    ' Retour     :
    ' - True si l'élévation est accordée (bandeau rafraîchi) ; False si elle est annulée ou refusée.
    '
    ' Remarques  :
    ' - Audit C.4 (ADR-55, décision de Joëlle du 25/10/2026) : l'élévation était écrite six fois
    '   (quatre dans Home, une dans chaque hub) ; elle passe désormais par OuvrirFenetreElevation.
    ' - ElevationAcces journalise elle-même l'élévation accordée (« Session élevée : … »).
    ' -------------------------------------------------------------------------------------------------
    Public Function DemanderElevation(contexteTemporaire As String) As Boolean Implements IFenetrePrincipale.DemanderElevation

        Dim contextePrecedent As String = PushContexteTemporaire(contexteTemporaire)

        Try
            Return OuvrirFenetreElevation()
        Finally
            SetContexte(contextePrecedent)
        End Try

    End Function

    ' Zone réservée (menu Administration ou Référentiels, référentiel ou thérapeute en fenêtre) : la
    ' question d'abord, puis la fenêtre d'élévation. False si l'utilisateur renonce (messageAbandon dans
    ' la barre d'état) ou si l'élévation est refusée.
    Private Function ObtenirElevationPourZone(messageAbandon As String) As Boolean

        Dim reponse As DialogResult =
            DialogChoix.Confirmer(
                "Cette zone nécessite une élévation de privilèges." &
                Environment.NewLine &
                Environment.NewLine &
                "Souhaitez-vous ouvrir une session élevée ?",
                "Élévation requise")

        If reponse = DialogResult.No Then
            stsLabelStatus.Text = messageAbandon
            Return False
        End If

        If Not OuvrirFenetreElevation() Then
            stsLabelStatus.Text = "Élévation refusée."
            Return False
        End If

        Return True

    End Function

    ' Fenêtre d'élévation, seul endroit où elle s'ouvre ; True si l'élévation est accordée, le bandeau
    ' (rôle courant) est alors rafraîchi.
    Private Function OuvrirFenetreElevation() As Boolean

        Using frmElevation As New ElevationAcces(_userSession, _authenticatedUser)

            frmElevation.SetContext(_uiContext)

            If frmElevation.ShowDialog(Me) <> DialogResult.OK Then
                Return False
            End If

        End Using

        UpdateConnectedUserDisplay()
        Return True

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : OuvrirCreationTherapeuteModal
    ' Type       : Therapeute
    ' Version    : V1.2.0
    ' Date       : 25/10/2026
    '
    ' Rôle       :
    ' Ouvre directement la Form modale de création d'un thérapeute (TherapeuteEdition), sans passer
    ' par l'écran de liste UC_Therapeutes. Réutilise le contrôle de droit + élévation déjà en place
    ' pour l'espace Référentiels (même mécanique que OuvrirReferentielModal). Destinée au bouton [+]
    ' « Ajouter un thérapeute » d'IntervenantEdition : retour immédiat à l'appelant avec le thérapeute
    ' nouvellement créé, prêt à être sélectionné dans le combo.
    '
    ' Paramètres :
    ' - Aucun
    '
    ' Retour     :
    ' - Le thérapeute créé (si l'enregistrement a réussi)
    ' - Nothing : si l'accès a été refusé, l'élévation annulée ou la saisie annulée
    '
    ' Remarques  :
    ' - Le contrôle de droit + élévation est centralisé ici (comme OuvrirReferentielModal)
    ' - Le contexte UI partagé est injecté à la Form si elle implémente IContextAwareForm
    '
    ' Exceptions :
    ' - Toutes les exceptions sont capturées, journalisées (Succinct/UI) et signalées à l'utilisateur
    ' - V1.1.0 (23/10/2026, v2.55.0) : TherapeuteEdition reçoit Home (bouton [+] des spécialités).
    ' - V1.2.0 (25/10/2026, audit C.4, ADR-55) : élévation par ObtenirElevationPourZone (bloc commun
    '   aux quatre zones réservées).
    ' -------------------------------------------------------------------------------------------------
    Public Function OuvrirCreationTherapeuteModal() As Therapeute Implements IFenetrePrincipale.OuvrirCreationTherapeuteModal

        Try

            If Not PeutAccederReferentielHome() AndAlso Not ObtenirElevationPourZone("Ajout de thérapeute annulé.") Then
                Return Nothing
            End If

            Using frm As New TherapeuteEdition(Nothing, Me)

                Dim contextAwareForm As IContextAwareForm = TryCast(frm, IContextAwareForm)
                If contextAwareForm IsNot Nothing Then
                    contextAwareForm.SetContext(_uiContext)
                End If

                If frm.ShowDialog(Me) = DialogResult.OK Then
                    Return frm.TherapeuteEnregistre
                End If

            End Using

            Return Nothing

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur OuvrirCreationTherapeuteModal.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur(
                "Erreur lors de l'ouverture de la création d'un thérapeute.",
                "Erreur"
            )

            Return Nothing

        End Try

    End Function

#End Region

#Region "Gestion des accès"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : PeutAccederAdminHome
    ' Type       : Boolean
    ' Version    : V1.0.0
    ' Date       : 11/05/2026
    '
    ' Rôle       :
    ' Vérifie si l'utilisateur courant possède les droits d'accès à l'espace Administration (UC_AdminHome).
    '
    ' Paramètres :
    ' - Aucun
    '
    ' Retour     :
    ' - True : Si le rôle courant est SuperUser ou Admin (via _userSession.IsSuperUserOrAdmin())
    ' - False : Si _userSession est Nothing ou si le rôle est insuffisant
    '
    ' Remarques  :
    ' - Appelée dans btnAdmin_Click avant de charger UC_AdminHome
    ' - Si retourne False : propose une élévation de privilèges via ElevationAcces
    ' - Vérifie le rôle COURANT (_userSession.CurrentRole), pas le rôle de base
    ' - Après élévation réussie, retournera True car CurrentRole aura changé
    ' - La vérification passe par UserSession.IsSuperUserOrAdmin() qui utilise l'enum AppRole
    '
    ' Exceptions :
    ' - Aucune (retourne False en cas de session invalide)
    ' -------------------------------------------------------------------------------------------------
    Private Function PeutAccederAdminHome() As Boolean

        If _userSession Is Nothing Then

            Return False

        End If

        Return _userSession.IsSuperUserOrAdmin()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : PeutAccederReferentielHome
    ' Type       : Boolean
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Vérifie si l'utilisateur courant possède les droits d'accès à l'espace Référentiels (UC_ReferentielHome).
    '
    ' Paramètres :
    ' - Aucun
    '
    ' Retour     :
    ' - True : Si le rôle courant est SuperUser ou Admin (via _userSession.IsSuperUserOrAdmin())
    ' - False : Si _userSession est Nothing ou si le rôle est insuffisant
    '
    ' Remarques  :
    ' - Appelée dans btnReferentiels_Click avant de charger UC_ReferentielHome
    ' - Si retourne False : propose une élévation de privilèges via ElevationAcces
    ' - Vérifie le rôle COURANT (_userSession.CurrentRole), pas le rôle de base
    ' - La gestion des référentiels est réservée aux mêmes rôles que l'Administration (SuperUser / Admin)
    '
    ' Exceptions :
    ' - Aucune (retourne False en cas de session invalide)
    ' -------------------------------------------------------------------------------------------------
    Private Function PeutAccederReferentielHome() As Boolean

        If _userSession Is Nothing Then

            Return False

        End If

        Return _userSession.IsSuperUserOrAdmin()

    End Function

#End Region

#Region "Gestion du contexte"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : BuildAdminContexte
    ' Type       : String
    ' Version    : V1.1.0
    ' Date       : 13/05/2026
    '
    ' Rôle       :
    ' Construit un libellé de contexte hiérarchique pour les écrans issus de l'administration.
    '
    ' Paramètres :
    ' - sousContexte : Nom du sous-écran d'administration (ex: "Paramètres", "Connexion DB") (String)
    '
    ' Retour     :
    ' - "Administration" : Si sousContexte est vide ou null
    ' - "Administration > [sousContexte]" : Si sousContexte est fourni (Trim() appliqué)
    '
    ' Remarques  :
    ' - Appelée publiquement par NavigateToAdminView() pour construire le contexte hiérarchique
    ' - Le format hiérarchique permet de visualiser la navigation : "Administration > Paramètres > Détail"
    ' - Trim() est appliqué pour éviter les espaces indésirables dans le rendu final
    ' - Peut être appelée par les UserControls pour construire des contextes multi-niveaux
    '
    ' Exceptions :
    ' - Aucune (gère les chaînes vides/null sans erreur)
    ' -------------------------------------------------------------------------------------------------
    Public Function BuildAdminContexte(
        sousContexte As String
    ) As String Implements IFenetrePrincipale.BuildAdminContexte

        If String.IsNullOrWhiteSpace(sousContexte) Then

            Return "Administration"

        End If

        Return $"Administration > {sousContexte.Trim()}"

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : BuildReferentielContexte
    ' Type       : String
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Construit un libellé de contexte hiérarchique pour les écrans issus des référentiels.
    '
    ' Paramètres :
    ' - sousContexte : Nom du sous-écran de référentiel (ex: "Domaines") (String)
    '
    ' Retour     :
    ' - "Référentiels" : Si sousContexte est vide ou null
    ' - "Référentiels > [sousContexte]" : Si sousContexte est fourni (Trim() appliqué)
    '
    ' Remarques  :
    ' - Appelée publiquement par NavigateToReferentielView() pour construire le contexte hiérarchique
    ' - Le format hiérarchique permet de visualiser la navigation : "Référentiels > Domaines"
    ' - Trim() est appliqué pour éviter les espaces indésirables dans le rendu final
    '
    ' Exceptions :
    ' - Aucune (gère les chaînes vides/null sans erreur)
    ' -------------------------------------------------------------------------------------------------
    Public Function BuildReferentielContexte(
        sousContexte As String
    ) As String Implements IFenetrePrincipale.BuildReferentielContexte

        If String.IsNullOrWhiteSpace(sousContexte) Then

            Return "Référentiels"

        End If

        Return $"Référentiels > {sousContexte.Trim()}"

    End Function


    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetContexte
    ' Version    : V1.2.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Met à jour le contexte de navigation affiché dans lblContexte (header) et stsLabelStatus (barre de statut).
    '
    ' Paramètres :
    ' - contexte : Libellé du contexte à afficher (ex: "Accueil", "Administration > Paramètres") (String)
    '
    ' Remarques  :
    ' - Appelée par NavigateTo() après chaque navigation pour synchroniser l'affichage
    ' - Appelée publiquement par les UserControls pour mettre à jour le contexte dynamiquement
    ' - lblContexte (Label dans pnlHeader) : indique "où je suis" dans l'application
    ' - stsLabelStatus (ToolStripStatusLabel) : affiche l'état courant associé au contexte
    ' - Les deux contrôles affichent le même texte pour cohérence visuelle
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContexte(
        contexte As String
    ) Implements IFenetrePrincipale.SetContexte

        lblContexte.Text = contexte
        stsLabelStatus.Text = contexte

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : PushContexteTemporaire
    ' Type       : String
    ' Version    : V1.0.0
    ' Date       : 13/05/2026
    '
    ' Rôle       :
    ' Applique temporairement un contexte avant ouverture d'une Form modale et retourne le contexte précédent.
    '
    ' Paramètres :
    ' - contexteTemporaire : Libellé du contexte temporaire à afficher (String)
    '
    ' Retour     :
    ' - Contexte précédent (lblContexte.Text) à restaurer après fermeture de la modale
    '
    ' Remarques  :
    ' - Utilisée avant l'ouverture de Forms modales (ConfigurationConnexion, ElevationAcces, etc.)
    ' - Pattern typique : contexte = PushContexteTemporaire("...") > ShowDialog() > RestoreContexte(contexte)
    ' - Sauvegarde le contexte actuel (lblContexte.Text) avant de le remplacer
    ' - Appelle SetContexte() pour mettre à jour lblContexte et stsLabelStatus
    ' - Le contexte retourné doit être passé à RestoreContexte() après fermeture de la modale
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Function PushContexteTemporaire(
        contexteTemporaire As String
    ) As String Implements IFenetrePrincipale.PushContexteTemporaire

        Dim contextePrecedent As String =
            lblContexte.Text

        SetContexte(contexteTemporaire)

        Return contextePrecedent

    End Function


    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : RestoreContexte
    ' Version    : V1.0.0
    ' Date       : 13/05/2026
    '
    ' Rôle       :
    ' Restaure un contexte précédemment sauvegardé via PushContexteTemporaire().
    '
    ' Paramètres :
    ' - contextePrecedent : Contexte à restaurer (String)
    '
    ' Remarques  :
    ' - Utilisée après fermeture de Forms modales pour revenir au contexte précédent
    ' - Pattern typique : contexte = PushContexteTemporaire("...") > ShowDialog() > RestoreContexte(contexte)
    ' - Si contextePrecedent est vide ou null : retourne sans erreur (protection)
    ' - Appelle SetContexte() pour mettre à jour lblContexte et stsLabelStatus
    '
    ' Exceptions :
    ' - Aucune (gère les chaînes vides/null sans erreur)
    ' -------------------------------------------------------------------------------------------------
    Public Sub RestoreContexte(
        contextePrecedent As String
    )

        If String.IsNullOrWhiteSpace(contextePrecedent) Then

            Return

        End If

        SetContexte(contextePrecedent)

    End Sub

#End Region

End Class
