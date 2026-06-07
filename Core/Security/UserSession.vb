' -------------------------------------------------------------------------------------------------
' Classe      : UserSession
' Projet      :Althéa
' Version     : V1.1.0
' Date        : 07/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Représente la session utilisateur courante en mémoire.
' Cette classe stocke l'état de la session après authentification réussie.
'
' Table correspondante dans la base de données : sec_utilisateurs
' (Les données de session ne sont PAS persistées en base, seules les infos utilisateur le sont)
'
' Responsabilités :
' - Stocker les informations de l'utilisateur connecté
' - Gérer le rôle de base et le rôle courant
' - Gérer l'élévation temporaire de privilèges
' - Suivre l'activité utilisateur (login, dernière activité)
' - Fournir des helpers pour les contrôles d'accès
'
' Remarques   :
' - Cette classe ne vérifie aucun mot de passe.
' - Elle stocke uniquement l'état de session après authentification.
' - L'élévation temporaire permet à un utilisateur d'accéder temporairement à des fonctions
'   réservées à un rôle supérieur (ex: utilisateur standard -> admin temporaire).
'
' Dépendances :
' - AppRole (enum des rôles applicatifs)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UserSession

#Region "Propriétés de session"

    ' -------------------------------------------------------------------------------------------------
    ' Description : Identifiant de l'utilisateur connecté
    ' Rôle        : Identifiant unique utilisé pour la connexion (correspond à sec_utilisateurs.nom_utilisateur)
    ' Type        : String
    ' -------------------------------------------------------------------------------------------------
    Public Property UserName As String

    ' -------------------------------------------------------------------------------------------------
    ' Description : Nom affiché dans l'interface utilisateur
    ' Rôle        : Nom complet ou pseudo de l'utilisateur pour l'affichage dans l'UI
    ' Type        : String
    ' -------------------------------------------------------------------------------------------------
    Public Property DisplayName As String

    ' -------------------------------------------------------------------------------------------------
    ' Description : Rôle de base de l'utilisateur
    ' Rôle        : Rôle attribué à la connexion, correspond au rôle permanent de l'utilisateur
    '               (Utilisateur, PowerUser, SuperUser, Admin)
    ' Type        : AppRole
    ' -------------------------------------------------------------------------------------------------
    Public Property BaseRole As AppRole

    ' -------------------------------------------------------------------------------------------------
    ' Description : Rôle actuellement actif
    ' Rôle        : Rôle effectif pour les contrôles d'accès. Peut être supérieur à BaseRole en cas d'élévation
    ' Type        : AppRole
    ' -------------------------------------------------------------------------------------------------
    Public Property CurrentRole As AppRole

    ' -------------------------------------------------------------------------------------------------
    ' Description : Date et heure de connexion
    ' Rôle        : Horodatage de l'authentification initiale de la session
    ' Type        : DateTime
    ' -------------------------------------------------------------------------------------------------
    Public Property LoginDateTime As DateTime

    ' -------------------------------------------------------------------------------------------------
    ' Description : Date et heure de dernière activité
    ' Rôle        : Horodatage de la dernière action utilisateur, utilisé pour la gestion de timeout
    ' Type        : DateTime
    ' -------------------------------------------------------------------------------------------------
    Public Property LastActivityDateTime As DateTime

    ' -------------------------------------------------------------------------------------------------
    ' Description : Indicateur d'élévation de privilèges
    ' Rôle        : Indique si l'utilisateur a élevé temporairement ses privilèges
    ' Type        : Boolean
    ' -------------------------------------------------------------------------------------------------
    Public Property IsElevated As Boolean

    ' -------------------------------------------------------------------------------------------------
    ' Description : Rôle élevé temporairement
    ' Rôle        : Rôle vers lequel l'utilisateur s'est élevé (Nothing si pas d'élévation)
    ' Type        : AppRole? (Nullable)
    ' -------------------------------------------------------------------------------------------------
    Public Property ElevatedRole As AppRole?

    ' -------------------------------------------------------------------------------------------------
    ' Description : Date et heure d'élévation
    ' Rôle        : Horodatage de l'élévation temporaire (Nothing si pas d'élévation)
    ' Type        : DateTime? (Nullable)
    ' -------------------------------------------------------------------------------------------------
    Public Property ElevationDateTime As DateTime?

#End Region

#Region "Constructeur"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version      : V1.1.0
    ' Date         : 07/05/2026
    '
    ' Rôle         :
    ' Initialise une nouvelle session utilisateur après authentification réussie.
    '
    ' Paramètres   :
    ' - userName    : Identifiant de l'utilisateur (depuis sec_utilisateurs)
    ' - displayName : Nom affiché dans l'interface
    ' - baseRole    : Rôle réel de l'utilisateur (depuis sec_utilisateurs)
    '
    ' Remarques    :
    ' - Initialise CurrentRole = BaseRole (pas d'élévation initiale)
    ' - Initialise LoginDateTime et LastActivityDateTime à l'instant présent
    ' - IsElevated = False par défaut
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(userName As String, displayName As String, baseRole As AppRole)

        Me.UserName = userName
        Me.DisplayName = displayName
        Me.BaseRole = baseRole
        Me.CurrentRole = baseRole

        Me.LoginDateTime = DateTime.Now
        Me.LastActivityDateTime = DateTime.Now

        Me.IsElevated = False
        Me.ElevatedRole = Nothing
        Me.ElevationDateTime = Nothing

    End Sub

#End Region

#Region "Gestion de l'activité"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Touch
    ' Version    : V1.1.0
    ' Date       : 07/05/2026
    '
    ' Rôle       :
    ' Met à jour la date de dernière activité de la session.
    '
    ' Remarques  :
    ' - Appelée à chaque action utilisateur significative
    ' - Utilisée pour détecter l'inactivité et gérer les timeouts de session
    ' - Appelée automatiquement par ElevateTo() et ResetElevation()
    ' -------------------------------------------------------------------------------------------------
    Public Sub Touch()

        LastActivityDateTime = DateTime.Now

    End Sub

#End Region

#Region "Gestion de l'élévation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ElevateTo
    ' Version    : V1.1.0
    ' Date       : 07/05/2026
    '
    ' Rôle       :
    ' Active une élévation temporaire de privilèges vers un rôle supérieur.
    '
    ' Paramètres :
    ' - targetRole : Le rôle cible vers lequel élever les privilèges
    '
    ' Remarques  :
    ' - La vérification du mot de passe doit être effectuée AVANT l'appel de cette méthode
    ' - Met à jour CurrentRole, ElevatedRole, ElevationDateTime et IsElevated
    ' - Appelle Touch() pour mettre à jour LastActivityDateTime
    ' - L'élévation doit être validée via ElevationAcces.vb avant appel
    ' -------------------------------------------------------------------------------------------------
    Public Sub ElevateTo(targetRole As AppRole)

        CurrentRole = targetRole
        ElevatedRole = targetRole
        ElevationDateTime = DateTime.Now
        IsElevated = (targetRole > BaseRole)

        Touch()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ResetElevation
    ' Version    : V1.1.0
    ' Date       : 07/05/2026
    '
    ' Rôle       :
    ' Supprime l'élévation temporaire et revient au rôle de base.
    '
    ' Remarques  :
    ' - Restaure CurrentRole à BaseRole
    ' - Réinitialise ElevatedRole, ElevationDateTime et IsElevated
    ' - Appelle Touch() pour mettre à jour LastActivityDateTime
    ' - Appelée automatiquement lors de la navigation ou sur demande utilisateur
    ' -------------------------------------------------------------------------------------------------
    Public Sub ResetElevation()

        CurrentRole = BaseRole
        ElevatedRole = Nothing
        ElevationDateTime = Nothing
        IsElevated = False

        Touch()

    End Sub

#End Region

#Region "Helpers de contrôle d'accès"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : HasAtLeastRole
    ' Version    : V1.1.0
    ' Date       : 07/05/2026
    '
    ' Rôle       :
    ' Vérifie si le rôle courant de la session atteint au moins le niveau requis.
    '
    ' Paramètres :
    ' - requiredRole : Le rôle minimum requis
    '
    ' Retour     :
    ' - Boolean : True si CurrentRole >= requiredRole, False sinon
    '
    ' Remarques  :
    ' - Utilise CurrentRole (pas BaseRole) pour tenir compte de l'élévation temporaire
    ' - Utilisé pour les contrôles d'accès dans toute l'application
    ' -------------------------------------------------------------------------------------------------
    Public Function HasAtLeastRole(requiredRole As AppRole) As Boolean

        Return CurrentRole >= requiredRole

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : IsAdmin
    ' Version    : V1.1.0
    ' Date       : 07/05/2026
    '
    ' Rôle       :
    ' Vérifie si la session courante possède le rôle Admin.
    '
    ' Retour     :
    ' - Boolean : True si CurrentRole = AppRole.Admin, False sinon
    '
    ' Remarques  :
    ' - Helper spécifique pour les fonctionnalités réservées aux administrateurs
    ' - Utilise CurrentRole pour tenir compte de l'élévation temporaire
    ' -------------------------------------------------------------------------------------------------
    Public Function IsAdmin() As Boolean

        Return CurrentRole = AppRole.Admin

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : IsSuperUserOrAdmin
    ' Version    : V1.1.0
    ' Date       : 07/05/2026
    '
    ' Rôle       :
    ' Vérifie si la session courante possède le rôle SuperUser ou Admin.
    '
    ' Retour     :
    ' - Boolean : True si CurrentRole >= AppRole.SuperUser, False sinon
    '
    ' Remarques  :
    ' - Helper spécifique pour les fonctionnalités avancées (paramètres, gestion utilisateurs, etc.)
    ' - Utilise CurrentRole pour tenir compte de l'élévation temporaire
    ' -------------------------------------------------------------------------------------------------
    Public Function IsSuperUserOrAdmin() As Boolean

        Return CurrentRole >= AppRole.SuperUser

    End Function

#End Region

End Class
