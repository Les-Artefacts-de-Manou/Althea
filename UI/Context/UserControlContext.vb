' -------------------------------------------------------------------------------------------------
' Classe      : UserControlContext
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 01/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Fournit un contexte UI partagé pour les UserControls de l'application.
'
' Table correspondante dans la base de données : 
' Aucune (classe de contexte UI)
'
' Responsabilités :
' - Centraliser l'accès aux composants globaux de la Form Home (StatusStrip, ToolTip, ErrorProvider, Header)
' - Fournir des méthodes pour mettre à jour le statut, l'en-tête, les infobulles et les erreurs
' - Donner accès à la session utilisateur (UserSession) pour la gestion des rôles
' - Simplifier le passage de contexte aux UserControls
'
' Remarques   :
' - Cette classe est instanciée par la Form Home et passée aux UserControls lors de leur création
' - Aucune validation des paramètres n'est effectuée dans le constructeur (responsabilité de l'appelant)
'
' Dépendances :
' - UserSession (pour la gestion des rôles et l'authentification)
' - System.Windows.Forms (ToolStripStatusLabel, ToolTip, ErrorProvider, Label, Control)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UserControlContext

#Region "Champs privés"

    ' -------------------------------------------------------------------------------------------------
    ' Champs privés de contexte UI
    ' Version     : V1.0.0
    ' Date        : 01/05/2026
    ' Rôle        :
    ' Contiennent les références aux composants globaux fournis par la Form Home.
    ' -------------------------------------------------------------------------------------------------

    ' Label de la barre de statut partagé (Home)
    Private ReadOnly _statusLabel As ToolStripStatusLabel

    ' Outil ToolTip partagé (Home)
    Private ReadOnly _toolTip As ToolTip

    ' Outil ErrorProvider partagé (Home)
    Private ReadOnly _errorProvider As ErrorProvider

    ' Label de contexte partagé pour les titres de section (Home)
    Private ReadOnly _headerLabel As Label

    ' Session utilisateur partagée (Home)
    Private ReadOnly _userSession As UserSession

    ' Utilisateur authentifié (Home)
    Private ReadOnly _authenticatedUser As UtilisateurApplication

#End Region

#Region "Constructeur"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : New (Constructeur)
    ' Version    : V1.0.0
    ' Date       : 01/05/2026
    '
    ' Rôle       :
    ' Initialise une nouvelle instance de UserControlContext avec les composants globaux de la Form Home.
    '
    ' Paramètres :
    ' - statusLabel       : Le label de la barre de statut (ToolStripStatusLabel)
    ' - toolTip           : L'objet ToolTip pour les infobulles
    ' - errorProvider     : L'objet ErrorProvider pour les messages d'erreur visuels
    ' - headerLabel       : Le label de contexte pour les titres de section
    ' - userSession       : L'objet UserSession pour l'utilisateur actuellement connecté
    ' - authenticatedUser : L'objet UtilisateurApplication complet de l'utilisateur authentifié
    '
    ' Remarques  :
    ' - Aucune validation des paramètres n'est effectuée (responsabilité de l'appelant)
    ' - Tous les paramètres doivent être non Nothing pour un fonctionnement correct
    '
    ' Exceptions :
    ' - Aucune exception levée
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(statusLabel As ToolStripStatusLabel,
                   toolTip As ToolTip,
                   errorProvider As ErrorProvider,
                   headerLabel As Label,
                   userSession As UserSession,
                   authenticatedUser As UtilisateurApplication)

        _statusLabel = statusLabel
        _toolTip = toolTip
        _errorProvider = errorProvider
        _headerLabel = headerLabel
        _userSession = userSession
        _authenticatedUser = authenticatedUser

    End Sub

#End Region

#Region "Méthodes publiques - UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetStatus
    ' Version    : V1.0.0
    ' Date       : 01/05/2026
    '
    ' Rôle       :
    ' Met à jour le message affiché dans la barre de statut de la Form Home.
    '
    ' Paramètres :
    ' - message : Le texte à afficher dans la barre de statut
    '
    ' Remarques  :
    ' - Si _statusLabel est Nothing, l'appel est ignoré sans erreur
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetStatus(message As String)
        If _statusLabel IsNot Nothing Then
            _statusLabel.Text = message
        End If
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetHeader
    ' Version    : V1.0.0
    ' Date       : 01/05/2026
    '
    ' Rôle       :
    ' Met à jour le texte affiché dans le label de contexte (header) de la Form Home.
    '
    ' Paramètres :
    ' - message : Le texte à afficher dans le label de contexte
    '
    ' Remarques  :
    ' - Si _headerLabel est Nothing, l'appel est ignoré sans erreur
    ' - Utilisé pour afficher le titre de la section active (ex: "Gestion des paramètres")
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetHeader(message As String)
        If _headerLabel IsNot Nothing Then
            _headerLabel.Text = message
        End If
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetToolTip
    ' Version    : V1.0.0
    ' Date       : 01/05/2026
    '
    ' Rôle       :
    ' Définit ou met à jour l'infobulle d'un contrôle.
    '
    ' Paramètres :
    ' - control : Le contrôle pour lequel définir l'infobulle
    ' - message : Le texte de l'infobulle
    '
    ' Remarques  :
    ' - Si _toolTip ou control est Nothing, l'appel est ignoré sans erreur
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetToolTip(control As Control, message As String)
        If _toolTip IsNot Nothing AndAlso control IsNot Nothing Then
            _toolTip.SetToolTip(control, message)
        End If
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetError
    ' Version    : V1.0.0
    ' Date       : 01/05/2026
    '
    ' Rôle       :
    ' Définit ou met à jour le message d'erreur visuel d'un contrôle via ErrorProvider.
    '
    ' Paramètres :
    ' - control : Le contrôle pour lequel définir le message d'erreur
    ' - message : Le message d'erreur à afficher
    '
    ' Remarques  :
    ' - Si _errorProvider ou control est Nothing, l'appel est ignoré sans erreur
    ' - Un icône d'erreur apparaît à côté du contrôle
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetError(control As Control, message As String)
        If _errorProvider IsNot Nothing AndAlso control IsNot Nothing Then
            _errorProvider.SetError(control, message)
        End If
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ClearError
    ' Version    : V1.0.0
    ' Date       : 01/05/2026
    '
    ' Rôle       :
    ' Efface le message d'erreur visuel d'un contrôle spécifique.
    '
    ' Paramètres :
    ' - control : Le contrôle pour lequel effacer le message d'erreur
    '
    ' Remarques  :
    ' - Si _errorProvider ou control est Nothing, l'appel est ignoré sans erreur
    ' - Supprime l'icône d'erreur affichée à côté du contrôle
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub ClearError(control As Control)
        If _errorProvider IsNot Nothing AndAlso control IsNot Nothing Then
            _errorProvider.SetError(control, String.Empty)
        End If
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ClearAllErrors
    ' Version    : V1.0.0
    ' Date       : 02/05/2026
    '
    ' Rôle       :
    ' Efface tous les messages d'erreur de l'ErrorProvider pour tous les contrôles.
    '
    ' Paramètres :
    ' - Aucun
    '
    ' Remarques  :
    ' - Si _errorProvider est Nothing, l'appel est ignoré sans erreur
    ' - Supprime toutes les icônes d'erreur affichées
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub ClearAllErrors()

        If _errorProvider IsNot Nothing Then
            _errorProvider.Clear()
        End If

    End Sub

#End Region

#Region "Méthodes publiques - Sécurité"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : HasRole
    ' Version    : V1.0.0
    ' Date       : 01/05/2026
    '
    ' Rôle       :
    ' Vérifie si l'utilisateur actuellement connecté possède le rôle requis ou un rôle supérieur.
    '
    ' Paramètres :
    ' - role : Le rôle requis (AppRole)
    '
    ' Retour     :
    ' - Boolean : True si l'utilisateur a le rôle requis ou supérieur, False sinon
    '
    ' Remarques  :
    ' - Utilise la comparaison >= sur CurrentRole (hiérarchie des rôles)
    ' - Si _userSession est Nothing, retourne False
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Function HasRole(role As AppRole) As Boolean

        Return _userSession IsNot Nothing AndAlso _userSession.CurrentRole >= role

    End Function

#End Region

#Region "Propriétés publiques"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : AuthenticatedUser
    ' Version    : V1.0.0
    ' Date       : 17/05/2026
    '
    ' Rôle       :
    ' Expose l'utilisateur authentifié complet pour les formulaires/contrôles nécessitant
    ' les informations détaillées de l'utilisateur.
    '
    ' Retour     :
    ' - UtilisateurApplication : L'utilisateur actuellement authentifié
    '
    ' Remarques  :
    ' - Utilisé notamment par UtilisateurEdition pour passer l'utilisateur aux méthodes métier
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property AuthenticatedUser As UtilisateurApplication
        Get
            Return _authenticatedUser
        End Get
    End Property

#End Region

End Class
