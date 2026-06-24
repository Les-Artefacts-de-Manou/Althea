' -------------------------------------------------------------------------------------------------
' Formulaire : UtilisateurEdition
' Projet     : Althéa
' Version    : V1.3.0
' Date       : 05/06/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Form modale permettant la consultation, la création et la modification
' d'un utilisateur applicatif.
'
' Responsabilités :
' - Afficher les informations détaillées d'un utilisateur
' - Gérer les rôles et droits applicatifs
' - Gérer les états de sécurité utilisateur
' - Permettre la création/modification utilisateur
' - Permettre les actions de sécurité administratives
' - Assurer une expérience utilisateur claire et sécurisée
'
' Fonctionnement :
' - Mode Création
' - Mode Modification
'
' Remarques :
' - Implémente IContextAwareForm
' - Utilisée uniquement depuis UC_Utilisateurs
' - Toute logique métier passe par GestionUtilisateurs
' - Aucun accès DB direct depuis l'UI
' - Aucun accès direct aux hash/salt depuis l'interface
'
' Sécurité :
' - Le login utilisateur est non modifiable en mode édition
' - Les mots de passe sont gérés exclusivement via les mécanismes sécurisés
' - Les actions critiques nécessitent un profil Admin
'
' Dépendances :
' - GestionUtilisateurs
' - GestionAuthentification
' - UtilisateurApplication
' - UserControlContext
' - UtilsControls
' - UtilsValidation
' - GestionLog
' - UITheme
' - PasswordSecurityHelper
'
' Remarques :
' - Ouverture modale via UC_Utilisateurs
' - Retour DialogResult.OK après sauvegarde réussie
' - En mode création, un mot de passe temporaire est généré et affiché à l'utilisateur
' - En mode modif, le mot de passe temporaire est caché
' -------------------------------------------------------------------------------------------------

' Enumération pour définir le mode d'édition de l'utilisateur (création ou modification)

Option Strict On
Option Explicit On

' Enumération pour définir le mode d'édition de l'utilisateur (création ou modification)
Public Enum ModeUtilisateurEdition
    Creation = 0
    Modification = 1
    Consultation = 2
End Enum

Public Class UtilisateurEdition
    Implements IContextAwareForm

#Region "Variables"

    ' Contexte UI partagé
    Private _context As UserControlContext

    ' Mode d'édition de l'utilisateur (création ou mod
    Private _mode As ModeUtilisateurEdition

    ' Identifiant utilisateur en mode modification
    Private _idUtilisateur As Long

    ' Instance de l'utilisateur en cours d'édition
    Private _utilisateur As UtilisateurApplication

#End Region

#Region "Constructeurs"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version      : V1.0.0
    ' Date         : 23/05/2026
    '
    ' Rôle         :
    ' Initialise la Form d'édition utilisateur en mode création ou modification.
    '
    ' Paramètres   :
    ' - mode          : Mode d'ouverture de la Form.
    ' - idUtilisateur : Identifiant utilisateur en mode modification.
    '
    ' Remarques    :
    ' - idUtilisateur vaut 0 en mode création.
    ' - La Form reste modale et retourne DialogResult.OK après sauvegarde réussie.
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(
        mode As ModeUtilisateurEdition,
        Optional idUtilisateur As Long = 0
    )

        InitializeComponent()

        _mode = mode
        _idUtilisateur = idUtilisateur

    End Sub

    Public Sub New()

        InitializeComponent()

        _mode = ModeUtilisateurEdition.Creation
        _idUtilisateur = 0

    End Sub

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SetContext
    ' Version   : V1.0.0
    ' Date      : 23/05/2026
    '
    ' Rôle      :
    ' Reçoit le contexte UI partagé fourni par Home.
    '
    ' Paramètres :
    ' - context : Contexte UI partagé.

    ' Remarques :
    ' - Obligatoire pour accéder au StatusStrip, ToolTip, ErrorProvider et contexte Home.
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(
        context As UserControlContext
    ) Implements IContextAwareForm.SetContext

        _context = context

    End Sub

#End Region

#Region "Chargement Form"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : UtilisateurEdition_Load
    ' Version   : V1.0.0
    ' Date      : 23/05/2026
    '
    ' Rôle      :
    ' Initialise la Form UtilisateurEdition selon le mode d'ouverture.

    ' Remarques :
    ' - La Form reste modale et retourne DialogResult.OK après sauvegarde réussie.
    ' -------------------------------------------------------------------------------------------------
    Private Sub UtilisateurEdition_Load(
        sender As Object,
        e As EventArgs
    ) Handles MyBase.Load

        InitialiserBoutons()
        InitialiserToolTips()
        InitialiserCombos()
        InitialiserMode()
        ChargerUtilisateur()

        Me.txtLoginUtilisateur.SelectionStart = 0
        Me.txtLoginUtilisateur.Focus()

        If _mode = ModeUtilisateurEdition.Modification Then
            txtLoginUtilisateur.BackColor = UITheme.ColorBeigeClair
            txtLoginUtilisateur.ForeColor = UITheme.ColorSaugeTresFonce
            txtLoginUtilisateur.BorderStyle = BorderStyle.None
        End If

    End Sub

#End Region

#Region "Initialisation UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserBoutons
    ' Version   : V1.0.0
    ' Date      : 23/05/2026
    '
    ' Rôle      :
    ' Initialise les boutons standards de la Form.

    ' Remarques :
    ' - Les boutons sont initialisés avec le style standard défini dans UtilsButtons.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserBoutons()

        UtilsButtons.InitStandardButton(btnEnregistrer)
        UtilsButtons.InitStandardButton(btnAnnuler)
        UtilsButtons.InitStandardButton(btnDeverrouiller)
        UtilsButtons.InitStandardButton(btnResetPassword)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserToolTips
    ' Version   : V1.0.0
    ' Date      : 23/05/2026
    '
    ' Rôle      :
    ' Initialise les aides contextuelles de la Form.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserToolTips()

        DefinirToolTip(btnEnregistrer, "Enregistrer les modifications.")
        DefinirToolTip(btnAnnuler, "Fermer sans enregistrer.")
        DefinirToolTip(btnDeverrouiller, "Déverrouiller le compte utilisateur.")
        DefinirToolTip(btnResetPassword, "Réinitialiser le mot de passe utilisateur.")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DefinirToolTip
    ' Version   : V1.0.0
    ' Date      : 01/07/2026
    '
    ' Rôle      :
    ' Définit une infobulle en privilégiant le contexte partagé de Home (_context) pour harmoniser
    ' le comportement ; repli sur le ToolTip local (ttMain) si aucun contexte n'est injecté.
    ' -------------------------------------------------------------------------------------------------
    Private Sub DefinirToolTip(control As Control, message As String)

        If _context IsNot Nothing Then
            _context.SetToolTip(control, message)
        Else
            ttMain.SetToolTip(control, message)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserCombos
    ' Version   : V1.0.0
    ' Date      : 23/05/2026
    '
    ' Rôle      :
    ' Initialise les ComboBox de sélection des rôles avec les valeurs de l'énumération AppRole.

    ' Remarques :
    ' - Les ComboBox sont initialisées avec le style standard de la Form.
    ' - Les rôles sont affichés sous forme de texte (User, SuperUser, Admin).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserCombos()

        cboRoleUtilisateur.Items.Clear()
        cboRoleUtilisateur.Items.Add(AppRole.User.ToString())
        cboRoleUtilisateur.Items.Add(AppRole.SuperUser.ToString())
        cboRoleUtilisateur.Items.Add(AppRole.Admin.ToString())

        cboRoleMaxElevation.Items.Clear()
        cboRoleMaxElevation.Items.Add(AppRole.User.ToString())
        cboRoleMaxElevation.Items.Add(AppRole.SuperUser.ToString())
        cboRoleMaxElevation.Items.Add(AppRole.Admin.ToString())

    End Sub

#End Region

#Region "Événements UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnEnregistrer_Click
    ' Version   : V1.0.0
    ' Date      : 17/05/2026
    '
    ' Rôle      :
    ' - Enregistre les modifications ou crée un nouvel utilisateur.
    ' - Valide les champs du formulaire avant enregistrement.
    '
    ' Remarques :
    ' - Cette procédure gère à la fois la création et la modification des utilisateurs.
    ' - En cas de création, un mot de passe temporaire est généré et copié dans le presse-papiers.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEnregistrer_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnEnregistrer.Click

        Try

            ' Validation des champs obligatoires
            If Not ValiderFormulaire() Then
                Return
            End If

            ' Récupération des valeurs du formulaire
            Dim login As String = txtLoginUtilisateur.Text.Trim()
            Dim nomAffichage As String = txtNomAffichage.Text.Trim()
            Dim roleUtilisateur As AppRole = DirectCast(
                [Enum].Parse(GetType(AppRole), cboRoleUtilisateur.SelectedItem.ToString()),
                AppRole
            )
            Dim roleMaxElevation As AppRole = DirectCast(
                [Enum].Parse(GetType(AppRole), cboRoleMaxElevation.SelectedItem.ToString()),
                AppRole
            )
            Dim actif As Boolean = chkActif.Checked
            Dim mustChangePassword As Boolean = chkMustChangePassword.Checked

            Select Case _mode

                Case ModeUtilisateurEdition.Creation

                    ' Génération d'un mot de passe temporaire sécurisé
                    Dim passwordInitial As String = GestionUtilisateurs.GenererMotDePasseTemporaire()

                    ' Appel CreateUtilisateur
                    Dim idUtilisateur As Long = GestionUtilisateurs.CreateUtilisateur(
                        login,
                        nomAffichage,
                        passwordInitial,
                        roleUtilisateur,
                        roleMaxElevation,
                        actif,
                        True ' mustChangePassword = True par défaut
                    )

                    GestionLog.EcrireLog(
                        $"[DEBUG] UtilisateurEdition : idUtilisateur retourné = {idUtilisateur}",
                        GestionLog.LogLevel.Complet,
                        GestionLog.LogCategory.UI
                    )

                    If idUtilisateur > 0 Then

                        ' Copier le mot de passe dans le presse-papiers pour faciliter la communication
                        Try
                            Clipboard.SetText(passwordInitial)
                        Catch ex As Exception
                            ' Ignorer les erreurs de presse-papiers (non critique)
                        End Try

                        DialogChoix.Succes(
                            $"L'utilisateur '{nomAffichage}' a été créé avec succès." & vbCrLf & vbCrLf &
                            $"Mot de passe temporaire : {passwordInitial}" & vbCrLf & vbCrLf &
                            "✓ Le mot de passe a été copié dans le presse-papiers." & vbCrLf &
                            "✓ L'utilisateur devra le changer à sa première connexion.",
                            "Utilisateur créé avec succès"
                        )

                        DialogResult = DialogResult.OK
                        Close()

                    Else

                        DialogChoix.Erreur(
                            "Erreur lors de la création de l'utilisateur. Consultez les logs.",
                            "Erreur"
                        )

                    End If

                Case ModeUtilisateurEdition.Modification

                    ' Appel UpdateUtilisateur
                    Dim succes As Boolean = GestionUtilisateurs.UpdateUtilisateur(
                        _idUtilisateur,
                        nomAffichage,
                        roleUtilisateur,
                        roleMaxElevation,
                        actif,
                        mustChangePassword,
                        _context.AuthenticatedUser
                    )

                    If succes Then

                        DialogChoix.Succes(
                            $"L'utilisateur '{nomAffichage}' a été modifié avec succès.",
                            "Succès"
                        )

                        DialogResult = DialogResult.OK
                        Close()

                    Else

                        DialogChoix.Erreur(
                            "Erreur lors de la modification de l'utilisateur. Consultez les logs.",
                            "Erreur"
                        )

                    End If

            End Select

        Catch ex As UnauthorizedAccessException

            MessageBox.Show(
                ex.Message,
                "Accès refusé",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            )

        Catch ex As ArgumentException

            DialogChoix.Avertissement(
                ex.Message,
                "Validation"
            )

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur btnEnregistrer_Click.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
                ex
            )

            DialogChoix.Erreur(
                "Une erreur inattendue s'est produite. Consultez les logs.",
                "Erreur"
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAnnuler_Click
    ' Version   : V1.0.0
    ' Date      : 23/05/2026
    '
    ' Rôle      :
    ' Ferme la Form sans enregistrer.

    ' Remarques :
    ' - La Form reste modale et retourne DialogResult.Cancel après fermeture.
    ' - Aucune validation n'est effectuée lors de l'annulation.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAnnuler_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnAnnuler.Click

        DialogResult = DialogResult.Cancel
        Close()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnResetPassword_Click
    ' Version   : V1.1.0
    ' Date      : 17/05/2026
    '
    ' Rôle      :
    ' Ouvre la Form ChangePassword en mode AdminReset pour réinitialiser le mot de passe de l'utilisateur.
    '
    ' Remarques :
    ' - Seule la réinitialisation du mot de passe est gérée ici, sans modification des autres champs.
    ' - La Form reste modale et retourne DialogResult.OK si la réinitialisation est effectuée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnResetPassword_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnResetPassword.Click

        Try

            ' Charger l'utilisateur complet
            Dim utilisateur As UtilisateurApplication = GestionUtilisateurs.GetUtilisateurPourEdition(_idUtilisateur)

            If utilisateur IsNot Nothing Then

                ' Ouvrir ChangePassword en mode AdminReset
                Using frmChange As New ChangePassword(utilisateur, ModeChangePassword.AdminReset)

                    Dim result As DialogResult = frmChange.ShowDialog(Me)

                    If result = DialogResult.OK Then

                        DialogChoix.Succes(
                            $"Le mot de passe de '{utilisateur.NomAffichage}' a été réinitialisé avec succès." & vbCrLf & vbCrLf &
                            "L'utilisateur devra le changer à sa prochaine connexion.",
                            "Mot de passe réinitialisé"
                        )

                        ' Recharger les données pour refléter les changements
                        ChargerUtilisateur()

                    End If

                End Using

            Else

                DialogChoix.Erreur(
                    "Impossible de charger les informations de l'utilisateur.",
                    "Erreur"
                )

            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur btnResetPassword_Click.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
                ex
            )

            DialogChoix.Erreur(
                "Une erreur inattendue s'est produite. Consultez les logs.",
                "Erreur"
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnDeverrouiller_Click
    ' Version   : V1.0.0
    ' Date      : 17/05/2026
    '
    ' Rôle      :
    ' Déverrouille le compte utilisateur bloqué.

    ' Remarques :   
    ' - Seule la déverrouillage du compte est gérée ici, sans modification des autres champs.
    ' - La Form reste modale et retourne DialogResult.OK si la déverrouillage est effectué.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnDeverrouiller_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnDeverrouiller.Click

        Try

            ' Confirmation de l'action
            Dim result As DialogResult = DialogChoix.Confirmer(
                $"Voulez-vous vraiment déverrouiller le compte de l'utilisateur '{txtNomAffichage.Text}' ?",
                "Confirmation"
            )

            If result <> DialogResult.Yes Then
                Return
            End If

            ' Appel de la méthode métier
            Dim succes As Boolean = GestionUtilisateurs.DeverrouillerUtilisateur(
                _idUtilisateur,
                _context.AuthenticatedUser
            )

            If succes Then

                DialogChoix.Succes(
                    $"Le compte de l'utilisateur '{txtNomAffichage.Text}' a été déverrouillé avec succès.",
                    "Succès"
                )

                ' Recharger les données pour refléter les changements
                ChargerUtilisateur()

            Else

                DialogChoix.Erreur(
                    "Erreur lors du déverrouillage du compte. Consultez les logs.",
                    "Erreur"
                )

            End If

        Catch ex As UnauthorizedAccessException

            DialogChoix.Avertissement(
                ex.Message,
                "Accès refusé"
            )

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur btnDeverrouiller_Click.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
                ex
            )

            DialogChoix.Erreur(
                "Une erreur inattendue s'est produite. Consultez les logs.",
                "Erreur"
            )

        End Try

    End Sub

#End Region

#Region "Gestion modes UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserMode
    ' Version   : V1.1.0
    ' Date      : 23/05/2026
    '
    ' Rôle      :
    ' Initialise l'état de la Form selon le mode d'ouverture.
    ' - Configure les champs, boutons et titres en fonction du mode (Création, Modification, Consultation).
    ' 
    ' Remarques :
    ' - La Form reste modale et retourne DialogResult.OK si la sauvegarde est effectuée.
    ' - La Form reste modale et retourne DialogResult.Cancel si la sauvegarde est annulée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserMode()

        Select Case _mode

            Case ModeUtilisateurEdition.Creation

                AppliquerModeCreation()

            Case ModeUtilisateurEdition.Modification

                AppliquerModeModification()

            Case ModeUtilisateurEdition.Consultation

                AppliquerModeConsultation()

        End Select

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerModeCreation
    ' Version   : V1.0.0
    ' Date      : 23/05/2026
    '
    ' Rôle      :
    ' Configure l'interface pour la création d'un utilisateur.
    '
    ' Remarques :
    ' - Les champs liés à l'identifiant et à la sécurité sont en lecture seule ou masqués.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerModeCreation()

        lblTitreForm.Text = "Nouvel utilisateur"
        Text = "Althéa - Nouvel utilisateur"

        txtCodeUtilisateur.ReadOnly = True
        txtDernierLogin.ReadOnly = True
        txtNbEchecsLogin.ReadOnly = True
        txtDateVerrouillage.ReadOnly = True
        txtDateCreation.ReadOnly = True
        txtDateModification.ReadOnly = True

        txtCodeUtilisateur.Text = String.Empty
        txtDernierLogin.Text = "Jamais"
        txtNbEchecsLogin.Text = "0"
        txtDateVerrouillage.Text = String.Empty
        txtDateCreation.Text = String.Empty
        txtDateModification.Text = String.Empty

        chkCompteVerrouille.Checked = False
        chkCompteVerrouille.Enabled = False

        btnDeverrouiller.Enabled = False
        btnResetPassword.Enabled = False ' Pas de reset avant la création

        txtLoginUtilisateur.ReadOnly = False

        chkMustChangePassword.Checked = True

        If _context IsNot Nothing Then

            _context.SetHeader(
            "Administration > Utilisateurs > Création"
        )

            _context.SetStatus(
            "Création d'un nouvel utilisateur."
        )

        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerModeModification
    ' Version   : V1.0.0
    ' Date      : 23/05/2026
    '
    ' Rôle      :
    ' Configure l'interface pour la modification d'un utilisateur.
    '
    ' Remarques :
    ' - Le login utilisateur est en lecture seule (non modifiable).
    ' - Les autres champs peuvent être modifiés selon les permissions de l'utilisateur.
    ' -------------------------------------------------------------------------------------------------

    Private Sub AppliquerModeModification()

        lblTitreForm.Text = "Modification utilisateur"
        Text = "Althéa - Modification utilisateur"

        txtLoginUtilisateur.ReadOnly = True

        chkCompteVerrouille.Enabled = False

        btnDeverrouiller.Enabled =
            chkCompteVerrouille.Checked

        If _context IsNot Nothing Then

            _context.SetHeader(
                "Administration > Utilisateurs > Modification"
            )

            _context.SetStatus(
                $"Modification utilisateur ID {_idUtilisateur}."
            )

        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerModeConsultation
    ' Version   : V1.0.0
    ' Date      : 01/06/2026
    '
    ' Rôle      :
    ' Configure l'interface pour la consultation d'un utilisateur (SuperUser).
    '
    ' Remarques :
    ' - Tous les champs sont en lecture seule
    ' - Seuls les boutons admin (Déverrouiller, Reset Password) sont accessibles
    ' - Bouton Enregistrer masqué
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerModeConsultation()

        lblTitreForm.Text = "Consultation utilisateur"
        Text = "Althéa - Consultation utilisateur"

        ' Tous les champs en lecture seule
        txtLoginUtilisateur.ReadOnly = True
        txtNomAffichage.ReadOnly = True
        txtCodeUtilisateur.ReadOnly = True
        txtDernierLogin.ReadOnly = True
        txtNbEchecsLogin.ReadOnly = True
        txtDateVerrouillage.ReadOnly = True
        txtDateCreation.ReadOnly = True
        txtDateModification.ReadOnly = True

        cboRoleUtilisateur.Enabled = False
        cboRoleMaxElevation.Enabled = False
        chkActif.Enabled = False
        chkMustChangePassword.Enabled = False
        chkCompteVerrouille.Enabled = False

        ' Seuls les boutons admin restent actifs
        btnEnregistrer.Visible = False
        btnDeverrouiller.Enabled = False ' Sera activé si compte verrouillé après chargement
        btnResetPassword.Enabled = True

        If _context IsNot Nothing Then

            _context.SetHeader(
                "Administration > Utilisateurs > Consultation"
            )

            _context.SetStatus(
                $"Consultation utilisateur ID {_idUtilisateur}."
            )

        End If

    End Sub

#End Region

#Region "Chargement données"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerUtilisateur
    ' Version   : V1.1.0
    ' Date      : 01/06/2026
    '
    ' Rôle      :
    ' Charge l'utilisateur à modifier/consulter et alimente les contrôles de la Form.
    '
    ' Remarques :
    ' - Utilisée en mode Modification et Consultation.
    ' - Aucun accès direct à la base depuis l'UI.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerUtilisateur()

        If _mode <> ModeUtilisateurEdition.Modification AndAlso _mode <> ModeUtilisateurEdition.Consultation Then
            Return
        End If

        _utilisateur =
            GestionUtilisateurs.GetUtilisateurPourEdition(_idUtilisateur)

        If _utilisateur Is Nothing Then

            If _context IsNot Nothing Then
                _context.SetStatus("Utilisateur introuvable.")
            End If

            DialogResult = DialogResult.Cancel
            Close()
            Return

        End If

        AlimenterControlesDepuisUtilisateur()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AlimenterControlesDepuisUtilisateur
    ' Version   : V1.0.0
    ' Date      : 23/05/2026
    '
    ' Rôle      :
    ' Alimente les contrôles de la Form depuis l'objet utilisateur chargé.
    '
    ' Remarques :
    ' - Formate les dates et gère les valeurs nullables pour l'affichage.
    ' - Aucun accès direct à la base depuis l'UI.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AlimenterControlesDepuisUtilisateur()

        txtCodeUtilisateur.Text = _utilisateur.CodeUtilisateur
        txtLoginUtilisateur.Text = _utilisateur.LoginUtilisateur
        txtNomAffichage.Text = _utilisateur.NomAffichage

        cboRoleUtilisateur.SelectedItem =
            _utilisateur.RoleUtilisateur.ToString()

        cboRoleMaxElevation.SelectedItem =
            _utilisateur.RoleMaxElevation.ToString()

        chkActif.Checked = _utilisateur.Actif
        chkMustChangePassword.Checked = _utilisateur.MustChangePassword

        txtNbEchecsLogin.Text =
            _utilisateur.NbEchecsLogin.ToString()

        chkCompteVerrouille.Checked =
            _utilisateur.CompteVerrouille

        txtDateVerrouillage.Text =
            If(
                _utilisateur.DateVerrouillage.HasValue,
                _utilisateur.DateVerrouillage.Value.ToString("dd/MM/yyyy HH:mm"),
                String.Empty
            )

        txtDernierLogin.Text =
            If(
                _utilisateur.DernierLogin.HasValue,
                _utilisateur.DernierLogin.Value.ToString("dd/MM/yyyy HH:mm"),
                "Jamais"
            )

        txtDateCreation.Text =
            _utilisateur.DateCreation.ToString("dd/MM/yyyy HH:mm")

        txtDateModification.Text =
            _utilisateur.DateModification.ToString("dd/MM/yyyy HH:mm")

        btnDeverrouiller.Enabled =
            _utilisateur.CompteVerrouille

    End Sub

#End Region

#Region "Validation"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : ValiderFormulaire
    ' Version   : V1.0.0
    ' Date      : 17/05/2026
    '
    ' Rôle      :
    ' Valide les champs du formulaire avant enregistrement.
    '
    ' Retour    :
    ' - Boolean : True si tous les champs sont valides, False sinon
    '
    ' Remarques :
    ' - Valide les champs obligatoires (login, nom affichage, rôles).
    ' -------------------------------------------------------------------------------------------------
    Private Function ValiderFormulaire() As Boolean

        ' Validation login
        If String.IsNullOrWhiteSpace(txtLoginUtilisateur.Text) Then
            DialogChoix.Avertissement(
                "Le login est obligatoire.",
                "Validation"
            )
            txtLoginUtilisateur.Focus()
            Return False
        End If

        ' Validation nom affichage
        If String.IsNullOrWhiteSpace(txtNomAffichage.Text) Then
            DialogChoix.Avertissement(
                "Le nom d'affichage est obligatoire.",
                "Validation"
            )
            txtNomAffichage.Focus()
            Return False
        End If

        ' Validation rôle utilisateur
        If cboRoleUtilisateur.SelectedItem Is Nothing Then
            DialogChoix.Avertissement(
                "Le rôle utilisateur est obligatoire.",
                "Validation"
            )
            cboRoleUtilisateur.Focus()
            Return False
        End If

        ' Validation rôle max élévation
        If cboRoleMaxElevation.SelectedItem Is Nothing Then
            DialogChoix.Avertissement(
                "Le rôle d'élévation maximale est obligatoire.",
                "Validation"
            )
            cboRoleMaxElevation.Focus()
            Return False
        End If

        ' Validation cohérence rôles
        Dim roleUtilisateur As AppRole = DirectCast(
            [Enum].Parse(GetType(AppRole), cboRoleUtilisateur.SelectedItem.ToString()),
            AppRole
        )
        Dim roleMaxElevation As AppRole = DirectCast(
            [Enum].Parse(GetType(AppRole), cboRoleMaxElevation.SelectedItem.ToString()),
            AppRole
        )

        If roleUtilisateur > roleMaxElevation Then
            DialogChoix.Avertissement(
                "Le rôle utilisateur ne peut pas être supérieur au rôle d'élévation maximale.",
                "Validation"
            )
            cboRoleMaxElevation.Focus()
            Return False
        End If

        Return True

    End Function

#End Region

End Class