' -------------------------------------------------------------------------------------------------
' Classe      : UtilisateurApplication
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 07/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Représente un utilisateur applicatif Althéa.
'
' Responsabilités :
' - Contenir les données d'un utilisateur applicatif
' - Fournir des helpers de vérification de rôle (IsAdmin, IsSuperUserOrAdmin)
'
' Table source : sec_utilisateurs
'
' Remarques   :
' - Classe de données (DTO-like) sans logique métier complexe
' - Ne contient aucun accès base de données
' - Ne contient aucune logique UI
' - Ne contient aucun traitement de sécurité (hash/vérification dans PasswordSecurityHelper)
'
' Dépendances :
' - Utilisée par GestionUtilisateurs (mapping depuis DataReader)
' - Utilisée par QueryUtilisateurs (requêtes SQL)
' - Utilisée par Login (formulaire de connexion)
' - Utilisée par UC_Utilisateurs (gestion des utilisateurs)
' - Utilisée par UserSession (session utilisateur)
' -------------------------------------------------------------------------------------------------

Public Class UtilisateurApplication

#Region "Propriétés"

    ' Identifiant unique de l'utilisateur (clé primaire)
    Public Property IdUtilisateur As Long

    ' Code technique de l'utilisateur (généré automatiquement, non modifiable)
    Public Property CodeUtilisateur As String

    ' Login de l'utilisateur (unique)
    Public Property LoginUtilisateur As String

    ' Nom d'affichage de l'utilisateur
    Public Property NomAffichage As String

    ' Hash du mot de passe (PBKDF2 SHA256)
    Public Property PasswordHash As String

    ' Sel du mot de passe (généré aléatoirement)
    Public Property PasswordSalt As String

    ' Rôle de base de l'utilisateur (User, SuperUser, Admin)
    Public Property RoleUtilisateur As AppRole

    ' Rôle maximal atteignable par élévation temporaire
    Public Property RoleMaxElevation As AppRole

    ' Indique si l'utilisateur est actif (0 = désactivé, 1 = actif)
    Public Property Actif As Boolean

    ' Indique si l'utilisateur doit changer son mot de passe à la prochaine connexion
    Public Property MustChangePassword As Boolean

    ' Nombre d'échecs de connexion consécutifs
    Public Property NbEchecsLogin As Integer

    ' Indique si le compte est verrouillé (après 5 échecs)
    Public Property CompteVerrouille As Boolean

    ' Date de verrouillage du compte (Nothing si jamais verrouillé)
    Public Property DateVerrouillage As DateTime?

    ' Date du dernier login réussi (Nothing si jamais connecté)
    Public Property DernierLogin As DateTime?

    ' Date de création de l'utilisateur
    Public Property DateCreation As DateTime

    ' Date de dernière modification de l'utilisateur
    Public Property DateModification As DateTime

#End Region

#Region "Helpers"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : IsAdmin
    ' Version    : V1.0.0
    ' Date       : 07/05/2026
    '
    ' Rôle       :
    ' Indique si l'utilisateur possède le rôle Admin.
    '
    ' Retour     :
    ' - Boolean : True si RoleUtilisateur = AppRole.Admin, False sinon
    '
    ' Utilisé par :
    ' - UI (affichage/masquage de fonctionnalités)
    ' - GestionUtilisateurs (vérifications de rôle)
    ' -------------------------------------------------------------------------------------------------
    Public Function IsAdmin() As Boolean

        Return RoleUtilisateur = AppRole.Admin

    End Function


    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : IsSuperUserOrAdmin
    ' Version    : V1.0.0
    ' Date       : 07/05/2026
    '
    ' Rôle       :
    ' Indique si l'utilisateur possède le rôle SuperUser ou Admin.
    '
    ' Retour     :
    ' - Boolean : True si RoleUtilisateur >= AppRole.SuperUser, False sinon
    '
    ' Utilisé par :
    ' - UI (affichage/masquage de fonctionnalités)
    ' - GestionUtilisateurs (vérifications de rôle)
    ' -------------------------------------------------------------------------------------------------
    Public Function IsSuperUserOrAdmin() As Boolean

        Return RoleUtilisateur >= AppRole.SuperUser

    End Function

#End Region

End Class