'------------------------------------------------------------
' 📌 QueryUtilisateurs.vb
' Projet : Althéa
' Version : V1.1.0
' Date    : 21/05/2026
' Auteur  : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Centralise les requêtes SQL liées à la table sec_utilisateurs.
'
'Responsabilités :
' - Fournir des fonctions retournant les requêtes SQL pour les opérations de sélection et de mise à jour des utilisateurs.
' - Fournir  les requêtes de login Utilisateur, de mise à jour du mot de passe,   d'incrémentation du nombre d'échecs login, de verrouillage de compte et de réinitialisation du compteur d'échecs login.   
' - Séparer clairement les requêtes sensibles des requêtes de liste/édition

' Remarques :
' - Ce module ne contient aucune logique métier.
' - Il fournit uniquement les requêtes SQL.
'
' Dépendance :
' - None
'------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryUtilisateurs

#Region "SELECT"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetUtilisateurByLogin
    ' Version    : V1.0.0
    ' Date       : 07/05/2026
    '
    ' Rôle       :
    ' Retourne la requête SQL pour charger un utilisateur applicatif depuis son login.
    '
    ' Paramètres SQL :
    ' - @login_utilisateur : Login de l'utilisateur à charger
    '
    ' Retour     :
    ' - String : Requête SQL SELECT sur sec_utilisateurs
    '
    ' Utilisé par :
    ' - GestionUtilisateurs.AuthentifierUtilisateur
    '
    ' Remarques  :
    ' - Le mot de passe n'est jamais vérifié ici (vérification dans GestionUtilisateurs)
    ' - La requête retourne aussi les utilisateurs inactifs pour permettre un message adapté
    ' - LIMIT 1 pour garantir un seul résultat
    ' -------------------------------------------------------------------------------------------------
    Public Function GetUtilisateurByLogin() As String

        Return "
SELECT
    id_utilisateur,
    code_utilisateur,
    login_utilisateur,
    nom_affichage,
    password_hash,
    password_salt,
    role_utilisateur,
    role_max_elevation,
    actif,
    must_change_password,
    nb_echecs_login,
    compte_verrouille,
    date_verrouillage,
    dernier_login,
    date_creation,
    date_modification
FROM sec_utilisateurs
WHERE login_utilisateur = @login_utilisateur
LIMIT 1;
"

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Property  : SelectUtilisateursListe
    ' Type retour : String
    ' Version    : V1.0.0
    ' Date       : 19/05/2026
    '
    ' Rôle       :
    ' Retourne la requête SQL permettant de charger la liste des utilisateurs
    ' applicatifs pour affichage dans UC_Utilisateurs.
    '
    ' Remarques  :
    ' - Utilisée pour alimenter la grille principale des utilisateurs
    ' - Exclut volontairement les champs sensibles :
    '   password_hash et password_salt
    ' - Tri par nom d'affichage pour cohérence UX
    ' - Les rôles et états de sécurité sont inclus pour gestion UI
    '
    ' Utilisée par :
    ' - GestionUtilisateurs.GetUtilisateurs()
    '
    ' Sécurité   :
    ' - Aucun secret ni donnée sensible retourné
    ' -------------------------------------------------------------------------------------------------

    Public ReadOnly Property SelectUtilisateursListe As String
        Get
            Return "
SELECT
    id_utilisateur,
    code_utilisateur,
    login_utilisateur,
    nom_affichage,
    role_utilisateur,
    role_max_elevation,
    actif,
    must_change_password,
    nb_echecs_login,
    compte_verrouille,
    date_verrouillage,
    dernier_login,
    date_creation,
    date_modification
FROM sec_utilisateurs
ORDER BY nom_affichage;
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : UpdatePasswordUtilisateur
    ' Version    : V1.0.0
    ' Date       : 09/05/2026
    '
    ' Rôle       :
    ' Retourne la requête SQL pour mettre à jour le mot de passe d'un utilisateur.
    '
    ' Paramètres SQL :
    ' - @password_hash   : Nouveau hash du mot de passe (PBKDF2 SHA256)
    ' - @password_salt   : Nouveau sel du mot de passe
    ' - @id_utilisateur  : Identifiant de l'utilisateur
    '
    ' Retour     :
    ' - String : Requête SQL UPDATE sur sec_utilisateurs
    '
    ' Utilisé par :
    ' - GestionUtilisateurs.ChangerMotDePasse
    '
    ' Remarques  :
    ' - Aucun mot de passe en clair (uniquement hash PBKDF2)
    ' - Réinitialise must_change_password à 0
    ' - Met à jour date_modification avec NOW()
    ' -------------------------------------------------------------------------------------------------
    Public Function UpdatePasswordUtilisateur() As String

        Return "
UPDATE sec_utilisateurs
SET
    password_hash = @password_hash,
    password_salt = @password_salt,
    must_change_password = 0,
    date_modification = NOW()
WHERE id_utilisateur = @id_utilisateur;"

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : IncrementerNbEchecsLogin
    ' Version    : V1.0.0
    ' Date       : 09/05/2026
    '
    ' Rôle       :
    ' Retourne la requête SQL pour incrémenter le nombre d'échecs de connexion d'un utilisateur.
    '
    ' Paramètres SQL :
    ' - @id_utilisateur : Identifiant de l'utilisateur
    '
    ' Retour     :
    ' - String : Requête SQL UPDATE sur sec_utilisateurs
    '
    ' Utilisé par :
    ' - GestionUtilisateurs.IncrementerNbEchecsLogin
    '
    ' Remarques  :
    ' - Incrémente nb_echecs_login de 1
    ' - Utilisé après chaque tentative de connexion échouée
    ' -------------------------------------------------------------------------------------------------
    Public Function IncrementerNbEchecsLogin() As String

        Return "
UPDATE sec_utilisateurs
SET
    nb_echecs_login = nb_echecs_login + 1
WHERE id_utilisateur = @id_utilisateur;"

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : VerrouillerCompteUtilisateur
    ' Version    : V1.0.0
    ' Date       : 09/05/2026
    '
    ' Rôle       :
    ' Retourne la requête SQL pour verrouiller le compte d'un utilisateur.
    '
    ' Paramètres SQL :
    ' - @id_utilisateur : Identifiant de l'utilisateur
    '
    ' Retour     :
    ' - String : Requête SQL UPDATE sur sec_utilisateurs
    '
    ' Utilisé par :
    ' - GestionUtilisateurs.VerrouillerCompteUtilisateur
    '
    ' Remarques  :
    ' - Passe compte_verrouille à 1
    ' - Enregistre date_verrouillage avec NOW()
    ' - Utilisé après dépassement du nombre maximal d'échecs de connexion
    ' -------------------------------------------------------------------------------------------------
    Public Function VerrouillerCompteUtilisateur() As String

        Return "
UPDATE sec_utilisateurs
SET
    compte_verrouille = 1,
    date_verrouillage = NOW()
WHERE id_utilisateur = @id_utilisateur;"

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ReinitialiserNbEchecsLogin
    ' Version    : V1.0.0
    ' Date       : 09/05/2026
    '
    ' Rôle       :
    ' Retourne la requête SQL pour réinitialiser le nombre d'échecs de connexion d'un utilisateur.
    '
    ' Paramètres SQL :
    ' - @id_utilisateur : Identifiant de l'utilisateur
    '
    ' Retour     :
    ' - String : Requête SQL UPDATE sur sec_utilisateurs
    '
    ' Utilisé par :
    ' - GestionUtilisateurs.ReinitialiserNbEchecsLogin
    '
    ' Remarques  :
    ' - Remet nb_echecs_login à 0
    ' - Utilisé après une connexion réussie
    ' -------------------------------------------------------------------------------------------------
    Public Function ReinitialiserNbEchecsLogin() As String

        Return "
UPDATE sec_utilisateurs
SET
    nb_echecs_login = 0
WHERE id_utilisateur = @id_utilisateur;"

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : UpdateDernierLoginUtilisateur
    ' Version    : V1.0.0
    ' Date       : 21/05/2026
    '
    ' Rôle       :
    ' Retourne la requête SQL pour mettre à jour la date du dernier login d'un utilisateur.
    '
    ' Paramètres SQL :
    ' - @id_utilisateur : Identifiant de l'utilisateur
    '
    ' Retour     :
    ' - String : Requête SQL UPDATE sur sec_utilisateurs
    '
    ' Utilisé par :
    ' - GestionUtilisateurs.UpdateDernierLoginUtilisateur
    '
    ' Remarques  :
    ' - Met à jour dernier_login avec NOW()
    ' - Met à jour date_modification avec NOW()
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateDernierLoginUtilisateur As String
        Get
            Return "
UPDATE sec_utilisateurs
SET
    dernier_login = NOW(),
    date_modification = NOW()
WHERE id_utilisateur = @id_utilisateur;
"
        End Get
    End Property

#End Region

#Region "Gestion Utilisateurs"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectUtilisateurPourAuthentification
    ' Type retour : String
    ' Version    : V1.0.0
    ' Date       : 21/05/2026
    '
    ' Rôle       :
    ' Retourne la requête SQL permettant de charger un utilisateur pour authentification.
    '
    ' Paramètres SQL :
    ' - @login_utilisateur : Login de l'utilisateur
    '
    ' Retour     :
    ' - String : Requête SQL SELECT sur sec_utilisateurs
    '
    ' Utilisé par :
    ' - GestionUtilisateurs.Authentifier
    '
    ' Remarques  :
    ' - Utilisée pour authentifier l'utilisateur
    ' -------------------------------------------------------------------------------------------------

    Public ReadOnly Property SelectUtilisateurPourAuthentification As String
        Get
            Return "
SELECT
    id_utilisateur,
    code_utilisateur,
    login_utilisateur,
    nom_affichage,
    password_hash,
    password_salt,
    role_utilisateur,
    role_max_elevation,
    actif,
    must_change_password,
    nb_echecs_login,
    compte_verrouille,
    date_verrouillage,
    dernier_login,
    date_creation,
    date_modification
FROM sec_utilisateurs
WHERE login_utilisateur = @login_utilisateur;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectUtilisateursPourListe
    ' Type retour : String
    ' Version    : V1.0.0
    ' Date       : 21/05/2026
    '
    ' Rôle       :
    ' Retourne la requête SQL permettant de charger la liste des utilisateurs pour affichage dans UC_Utilisateurs.
    '
    ' Paramètres SQL :
    ' - @afficher_inactifs : Indique si les utilisateurs inactifs doivent être inclus (1 = oui, 0 = non)
    '
    ' Retour     : 
    ' - String : Requête SQL SELECT sur sec_utilisateurs
    '
    ' Utilisé par :
    ' - GestionUtilisateurs.GetUtilisateurs
    '
    ' Remarques  :
    ' - Utilisée pour alimenter la grille principale des utilisateurs
    ' - Exclut volontairement les champs sensibles :
    '   password_hash et password_salt
    ' - Tri par nom d'affichage pour cohérence UX
    ' - Les rôles et états de sécurité sont inclus pour gestion UI
    ' -------------------------------------------------------------------------------------------------
    '
    Public ReadOnly Property SelectUtilisateursPourListe As String
        Get
            Return "
SELECT
    id_utilisateur,
    code_utilisateur,
    login_utilisateur,
    nom_affichage,
    role_utilisateur,
    role_max_elevation,
    actif,
    must_change_password,
    nb_echecs_login,
    compte_verrouille,
    date_verrouillage,
    dernier_login,
    date_creation,
    date_modification
FROM sec_utilisateurs
WHERE (@afficher_inactifs = 1 OR actif = 1)
ORDER BY
    actif DESC,
    nom_affichage ASC,
    login_utilisateur ASC;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectUtilisateurPourEdition
    ' Type retour : String
    ' Version    : V1.0.0
    ' Date       : 21/05/2026
    '
    ' Rôle       :  
    ' Retourne la requête SQL permettant de charger les détails d'un utilisateur pour édition dans UC_Utilisateurs.
    '
    ' Paramètres SQL :  
    ' - @id_utilisateur : Identifiant de l'utilisateur à charger
    '
    ' Retour     : 
    ' - String : Requête SQL SELECT sur sec_utilisateurs
    '
    ' Utilisé par :
    ' - GestionUtilisateurs.GetUtilisateurPourEdition
    '
    ' Remarques  :
    ' - Utilisée pour alimenter le formulaire d'édition des utilisateurs
    ' -------------------------------------------------------------------------------------------------

    Public ReadOnly Property SelectUtilisateurPourEdition As String
        Get
            Return "
SELECT
    id_utilisateur,
    code_utilisateur,
    login_utilisateur,
    nom_affichage,
    role_utilisateur,
    role_max_elevation,
    actif,
    must_change_password,
    nb_echecs_login,
    compte_verrouille,
    date_verrouillage,
    dernier_login,
    date_creation,
    date_modification
FROM sec_utilisateurs
WHERE id_utilisateur = @id_utilisateur;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectUtilisateurPourSuppression
    ' Type retour : String
    ' Version    : V1.0.0
    ' Date       : 21/05/2026
    '
    ' Rôle       :  
    ' Retourne la requête SQL permettant de charger les détails d'un utilisateur pour suppression dans UC_Utilisateurs.
    '
    ' Paramètres SQL :  
    ' - @id_utilisateur : Identifiant de l'utilisateur à charger
    '
    ' Retour     : 
    ' - String : Requête SQL SELECT sur sec_utilisateurs
    '
    ' Utilisé par :
    ' - GestionUtilisateurs.GetUtilisateurPourSuppression
    '
    ' Remarques  :
    ' - Utilisée pour alimenter le formulaire de suppression des utilisateurs
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property InsertUtilisateur As String
        Get
            Return "
INSERT INTO sec_utilisateurs
(
    login_utilisateur,
    nom_affichage,
    password_hash,
    password_salt,
    role_utilisateur,
    role_max_elevation,
    actif,
    must_change_password,
    nb_echecs_login,
    compte_verrouille,
    date_creation,
    date_modification
)
VALUES
(
    @login_utilisateur,
    @nom_affichage,
    @password_hash,
    @password_salt,
    @role_utilisateur,
    @role_max_elevation,
    @actif,
    @must_change_password,
    0,
    0,
    NOW(),
    NOW()
);
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateUtilisateur
    ' Type retour : String
    ' Version    : V1.0.0
    ' Date       : 21/05/2026
    '
    ' Rôle       :
    ' Retourne la requête SQL permettant de mettre à jour les informations d'un utilisateur depuis le formulaire d'édition dans UC_Utilisateurs.
    '
    ' Paramètres SQL :
    ' - @id_utilisateur : Identifiant de l'utilisateur à mettre à jour
    ' - @nom_affichage : Nouveau nom d'affichage de l'utilisateur
    ' - @role_utilisateur : Nouveau rôle de l'utilisateur
    ' - @role_max_elevation : Nouvelle élévation maximale du rôle de l'utilisateur
    ' - @actif : Nouveau statut actif de l'utilisateur
    ' - @must_change_password : Indicateur si l'utilisateur doit changer son mot de passe
    '
    ' Retour     : 
    ' - String : Requête SQL UPDATE sur sec_utilisateurs
    '
    ' Utilisé par :
    ' - GestionUtilisateurs.UpdateUtilisateur
    '
    ' Remarques  :
    ' - Utilisée pour mettre à jour les informations d'un utilisateur depuis le formulaire d'édition
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateUtilisateur As String
        Get
            Return "
UPDATE sec_utilisateurs
SET
    nom_affichage = @nom_affichage,
    role_utilisateur = @role_utilisateur,
    role_max_elevation = @role_max_elevation,
    actif = @actif,
    must_change_password = @must_change_password,
    date_modification = NOW()
WHERE id_utilisateur = @id_utilisateur;
"
        End Get
    End Property

#End Region

#Region "Actions sécurité utilisateur"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateActifUtilisateur
    ' Type retour : String
    ' Version    : V1.0.0
    ' Date       : 21/05/2026

    ' Rôle       :  
    ' Retourne la requête SQL permettant de mettre à jour le statut actif d'un utilisateur.
    '   
    ' Paramètres SQL :
    ' - @id_utilisateur : Identifiant de l'utilisateur à mettre à jour
    ' - @actif : Nouveau statut actif de l'utilisateur (0 = désactivé, 1 = actif)
    ' Retour     :
    ' - String : Requête SQL UPDATE sur sec_utilisateurs
    ' Utilisé par :
    ' - GestionUtilisateurs.ActiverDesactiverUtilisateur
    ' Remarques  :
    ' - Utilisée pour activer ou désactiver un utilisateur
    ' - Met à jour date_modification avec NOW()
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateActifUtilisateur As String
        Get
            Return "
UPDATE sec_utilisateurs
SET
    actif = @actif,
    date_modification = NOW()
WHERE id_utilisateur = @id_utilisateur;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : ResetPasswordUtilisateur
    ' Type retour : String
    ' Version    : V1.0.0
    ' Date       : 21/05/2026
    '
    ' Rôle       :
    ' Retourne la requête SQL permettant de réinitialiser le mot de passe d'un utilisateur.
    '
    ' Paramètres SQL :
    ' - @id_utilisateur : Identifiant de l'utilisateur à réinitialiser
    ' - @password_hash : Nouveau hash du mot de passe
    ' - @password_salt : Nouveau sel du mot de passe
    '
    ' Retour     :
    ' - String : Requête SQL UPDATE sur sec_utilisateurs
    '
    ' Utilisé par :
    ' - GestionUtilisateurs.ReinitialiserMotDePasseUtilisateur
    '
    ' Remarques  :  
    ' - Utilisée pour réinitialiser le mot de passe d'un utilisateur
    ' - Réinitialise must_change_password à 1 pour forcer le changement à la prochaine connexion
    ' - Réinitialise nb_echecs_login à 0 et compte_verrouille à 0 pour éviter les blocages après réinitialisation
    ' - Met à jour date_modification avec NOW()
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property ResetPasswordUtilisateur As String
        Get
            Return "
UPDATE sec_utilisateurs
SET
    password_hash = @password_hash,
    password_salt = @password_salt,
    must_change_password = 1,
    nb_echecs_login = 0,
    compte_verrouille = 0,
    date_verrouillage = NULL,
    date_modification = NOW()
WHERE id_utilisateur = @id_utilisateur;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : ForcerChangementPasswordUtilisateur
    ' Type retour : String
    ' Version    : V1.0.0
    ' Date       : 21/05/2026
    '
    ' Rôle       :
    ' Retourne la requête SQL permettant de forcer le changement de mot de passe d'un utilisateur.
    '
    ' Paramètres SQL :
    ' - @id_utilisateur : Identifiant de l'utilisateur à mettre à jour
    '
    ' Retour     :
    ' - String : Requête SQL UPDATE sur sec_utilisateurs
    '
    ' Utilisé par :
    ' - GestionUtilisateurs.ForcierChangementMotDePasseUtilisateur
    '
    ' Remarques  :
    ' - Utilisée pour forcer le changement de mot de passe d'un utilisateur
    ' - Met à jour date_modification avec NOW()
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property ForcerChangementPasswordUtilisateur As String
        Get
            Return "
UPDATE sec_utilisateurs
SET
    must_change_password = 1,
    date_modification = NOW()
WHERE id_utilisateur = @id_utilisateur;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DeverrouillerUtilisateur
    ' Type retour : String
    ' Version    : V1.0.0
    ' Date       : 21/05/2026
    '
    ' Rôle       :
    ' Retourne la requête SQL permettant de deverrouiller un utilisateur.
    '
    ' Paramètres SQL :
    ' - @id_utilisateur : Identifiant de l'utilisateur à mettre à jour
    '
    ' Retour     :
    ' - String : Requête SQL UPDATE sur sec_utilisateurs
    '
    ' Utilisé par :
    ' - GestionUtilisateurs.DeverrouillerUtilisateur
    '
    ' Remarques  :
    ' - Utilisée pour deverrouiller un utilisateur
    ' - Passe compte_verrouille à 0, nb_echecs_login à 0 et date_verrouillage à NULL
    ' - Met à jour date_modification avec NOW()
    ' -------------------------------------------------------------------------------------------------

    Public ReadOnly Property DeverrouillerUtilisateur As String
        Get
            Return "
UPDATE sec_utilisateurs
SET
    compte_verrouille = 0,
    nb_echecs_login = 0,
    date_verrouillage = NULL,
    date_modification = NOW()
WHERE id_utilisateur = @id_utilisateur;
"
        End Get
    End Property

#End Region
End Module
