' -------------------------------------------------------------------------------------------------
' Module      : GestionUtilisateurs
' Projet      : Althéa
' Version     : V1.8.1
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Gestion métier des utilisateurs applicatifs.
'
' Responsabilités :
' - Date verouillage et gestion des comptes verrouillés
' - Réinitialiser les échecs après succès
' - Mapper les DataReader vers UtilisateurApplication (MapUtilisateurPourAdministration)
'
' Important   :
' - Aucun accès UI direct (pas de MessageBox)
' - Aucun mot de passe en clair dans les logs
' - Retour de messages UI contrôlés via AuthenticationResult
' - Limite max 5 échecs login avant verrouillage
'
'
' Architecture :
' UI → GestionUtilisateurs → QueryUtilisateurs → DatabaseManager → MariaDB
'
' Dépendances :
' - DatabaseManager (connexions DB)
' - QueryUtilisateurs (requêtes SQL)
' - PasswordSecurityHelper (hash/vérification PBKDF2), PolitiqueMotDePasse (règle du mot de passe)
' - GestionLog (traçabilité)
' - UtilisateurApplication (objet métier)
' - AuthenticationResult (résultat d'authentification)
'
' Historique  :
' - V1.5.0 (20/10/2026) : GetUtilisateurs, HasUtilisateurs et MapUtilisateur supprimés : jamais
'   appelés, et cassés (lecture de password_hash, absent de la requête ; audit B6).
'   Traces [DEBUG] de CreateUtilisateur et ancien bloc « TODO DEBUG » (connexion automatique,
'   jamais écrite) retirés (audit § 2). GenererMotDePasseTemporaire V2.0.0 : sans mot de passe de
'   repli constant, tirage sans biais (audit § 3.2).
'   Droits côté métier sur le rôle COURANT (élévation comprise ; audit A.3, décision de Joëlle) :
'   CreateUtilisateur, UpdateUtilisateur, ResetPasswordUtilisateurWithCustomPassword,
'   DeverrouillerUtilisateur, UpdatePhotoUtilisateur, ActiverDesactiverUtilisateur reçoivent
'   roleCourant ; son propre rôle, son élévation et son état ne se modifient pas ; jamais sans
'   administrateur actif (VerifierResteUnAdmin). ResetPasswordUtilisateur supprimée (jamais appelée).
'   VerifierElevationUtilisateur V2.0.0 : essais comptés et verrouillage comme à la connexion,
'   compte relu en base, résultat détaillé (audit A.5).
' - V1.6.0 (21/10/2026, audit B.2) : CreateUtilisateur en une transaction, variante (conn, tx) pour
'   GestionCabinet.CreerCompteSuperUser (compte créé et lié ensemble).
' - V1.7.0 (22/10/2026, audit C.1) : les opérations d'administration des comptes reçoivent un
'   ContexteOperation (au lieu de roleCourant et de l'utilisateur authentifié) et vérifient le droit
'   par Autorisation (Exiger, ExigerUtilisateur).
' - V1.8.0 (23/10/2026, audit C.2 étape 4, ADR-51) : ValiderCompte (login d'au moins 3 caractères et
'   unique, nom d'affichage, rôles cohérents), appliqué par CreateUtilisateur ; la règle du mot de passe
'   (PolitiqueMotDePasse) est appliquée par ChangerMotDePasse, ResetPasswordUtilisateurWithCustomPassword
'   et CreateUtilisateur, qui acceptaient n'importe quel mot de passe non vide. Un refus de saisie lève
'   ErreurValidationException (message pour l'utilisatrice) au lieu de rendre -1.
' - V1.8.1 (25/10/2026, audit D-1) : Option Strict On.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionUtilisateurs


#Region "Validation"

    ' Longueur minimale d'un login (décision du 22/10/2026, inventaire C.2 : la même partout).
    Public Const LongueurMinLogin As Integer = 3

    Private Const MESSAGE_LOGIN_PRIS As String = "Ce login est déjà utilisé par un autre compte."

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ValiderCompte
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Erreurs de saisie d'un compte (liste vide s'il est valide) : login d'au moins 3 caractères et
    ' unique, nom d'affichage obligatoire, rôle de base au plus égal au rôle d'élévation maximale.
    '
    ' Paramètres :
    ' - login             : login saisi, ou Nothing en modification (le login ne change pas après la
    '                       création : il n'est pas jugé, un ancien login plus court reste valable)
    ' - nomAffichage      : nom d'affichage saisi
    ' - roleUtilisateur   : rôle de base
    ' - roleMaxElevation  : rôle d'élévation maximale
    '
    ' Remarques  :
    ' - Règles sorties de UtilisateurEdition et DialogComptePraticien (audit C.2, ADR-51), qui en avaient
    '   chacune une partie (login non vide ; login d'au moins 3 caractères). Champs : NameOf de
    '   UtilisateurApplication.
    ' - CreateUtilisateur applique les mêmes règles et relit l'unicité dans sa transaction.
    ' -------------------------------------------------------------------------------------------------
    Public Function ValiderCompte(login As String,
                                  nomAffichage As String,
                                  roleUtilisateur As AppRole,
                                  roleMaxElevation As AppRole) As List(Of ErreurSaisie)

        Dim erreurs As List(Of ErreurSaisie) = ErreursCompte(login, nomAffichage, roleUtilisateur, roleMaxElevation)

        If login IsNot Nothing AndAlso
           Not erreurs.Any(Function(e) e.Champ = NameOf(UtilisateurApplication.LoginUtilisateur)) Then

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()
                If LoginExiste(login.Trim(), conn, Nothing) Then
                    erreurs.Add(New ErreurSaisie(NameOf(UtilisateurApplication.LoginUtilisateur), MESSAGE_LOGIN_PRIS))
                End If
            End Using

        End If

        Return erreurs

    End Function

    ' Règles de saisie sans lecture de la base (l'unicité du login est contrôlée à part).
    Private Function ErreursCompte(login As String,
                                   nomAffichage As String,
                                   roleUtilisateur As AppRole,
                                   roleMaxElevation As AppRole) As List(Of ErreurSaisie)

        Dim erreurs As New List(Of ErreurSaisie)

        If login IsNot Nothing Then
            Dim loginSaisi As String = login.Trim()
            If loginSaisi.Length = 0 Then
                erreurs.Add(New ErreurSaisie(NameOf(UtilisateurApplication.LoginUtilisateur), "Le login est obligatoire."))
            ElseIf loginSaisi.Length < LongueurMinLogin Then
                erreurs.Add(New ErreurSaisie(NameOf(UtilisateurApplication.LoginUtilisateur),
                                             $"Le login doit comporter au moins {LongueurMinLogin} caractères."))
            End If
        End If

        If String.IsNullOrWhiteSpace(nomAffichage) Then
            erreurs.Add(New ErreurSaisie(NameOf(UtilisateurApplication.NomAffichage), "Le nom d'affichage est obligatoire."))
        End If

        If roleUtilisateur > roleMaxElevation Then
            erreurs.Add(New ErreurSaisie(NameOf(UtilisateurApplication.RoleMaxElevation),
                                         "Le rôle utilisateur ne peut pas être supérieur au rôle d'élévation maximale."))
        End If

        Return erreurs

    End Function

    ' Vrai si un compte porte déjà ce login (comparaison de la base : casse et accents ignorés).
    Private Function LoginExiste(login As String, conn As MySqlConnection, tx As MySqlTransaction) As Boolean

        Using cmd As New MySqlCommand(QueryUtilisateurs.GetUtilisateurByLogin(), conn, tx)
            cmd.Parameters.AddWithValue("@login_utilisateur", login)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                Return reader.HasRows
            End Using
        End Using

    End Function

#End Region

#Region "Mot de passe"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ChangerMotDePasse
    ' Version    : V1.1.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Change le mot de passe d'un utilisateur applicatif après vérification de l'ancien mot de passe.
    '
    ' Paramètres :
    ' - utilisateur : Utilisateur authentifié (UtilisateurApplication)
    ' - oldPassword : Ancien mot de passe saisi
    ' - newPassword : Nouveau mot de passe saisi
    '
    ' Retour     :
    ' - Boolean : True si changement effectué, False sinon
    '
    ' Important  :
    ' - Aucun mot de passe en clair dans les logs
    ' - Vérifie l'ancien mot de passe avant modification
    ' - Génère un nouveau salt via PasswordSecurityHelper.GenerateSalt()
    ' - Désactive must_change_password après succès
    ' - Met à jour l'objet utilisateur en mémoire
    ' - V1.1.0 (23/10/2026, audit C.2) : refus (ErreurValidationException) d'un nouveau mot de passe hors
    '   règle (PolitiqueMotDePasse) ou identique à l'ancien ; l'écran le vérifiait seul.
    '
    ' Utilisé par :
    ' - ChangePassword (formulaire de changement de mot de passe)
    ' -------------------------------------------------------------------------------------------------
    Public Function ChangerMotDePasse(
        utilisateur As UtilisateurApplication,
        oldPassword As String,
        newPassword As String
    ) As Boolean

        Dim erreurs As List(Of ErreurSaisie) = PolitiqueMotDePasse.Valider(newPassword)
        If erreurs.Count = 0 AndAlso String.Equals(oldPassword, newPassword, StringComparison.Ordinal) Then
            erreurs.Add(New ErreurSaisie(PolitiqueMotDePasse.ChampMotDePasse, "Le nouveau mot de passe doit être différent de l'ancien."))
        End If
        ErreurValidationException.LeverSiErreurs(erreurs)

        Try

            If utilisateur Is Nothing Then

                GestionLog.EcrireLog(
                    "Changement mot de passe refusé : utilisateur absent.",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Security
                )

                Return False

            End If

            Dim oldPasswordValid As Boolean =
                PasswordSecurityHelper.VerifyPassword(
                    oldPassword,
                    utilisateur.PasswordHash,
                    utilisateur.PasswordSalt
                )

            If Not oldPasswordValid Then

                GestionLog.EcrireLog(
                    $"Changement mot de passe refusé : ancien mot de passe invalide ({utilisateur.LoginUtilisateur}).",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Security
                )

                Return False

            End If

            Dim newSalt As String =
                PasswordSecurityHelper.GenerateSalt()

            Dim newHash As String =
                PasswordSecurityHelper.HashPassword(
                    newPassword,
                    newSalt
                )

            Using conn As MySqlConnection =
                DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(
                    QueryUtilisateurs.UpdatePasswordUtilisateur(),
                    conn)

                    cmd.Parameters.AddWithValue(
                        "@password_hash",
                        newHash
                    )

                    cmd.Parameters.AddWithValue(
                        "@password_salt",
                        newSalt
                    )

                    cmd.Parameters.AddWithValue(
                        "@id_utilisateur",
                        utilisateur.IdUtilisateur
                    )

                    Dim rowsAffected As Integer =
                        cmd.ExecuteNonQuery()

                    If rowsAffected <> 1 Then

                        GestionLog.EcrireLog(
                            $"Changement mot de passe échoué : aucune ligne mise à jour ({utilisateur.LoginUtilisateur}).",
                            GestionLog.LogLevel.Succinct,
                            GestionLog.LogCategory.Security
                        )

                        Return False

                    End If

                End Using

            End Using

            utilisateur.PasswordHash = newHash
            utilisateur.PasswordSalt = newSalt
            utilisateur.MustChangePassword = False
            utilisateur.DateModification = DateTime.Now

            GestionLog.EcrireLog(
                $"Changement mot de passe réussi ({utilisateur.LoginUtilisateur}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Security
            )

            Return True

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur ChangerMotDePasse.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
                ex
            )

            Throw

        End Try

    End Function

#End Region

#Region "Élévation d'accès"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : VerifierElevationUtilisateur
    ' Version    : V2.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       :
    ' Vérifie si l'utilisateur connecté peut élever temporairement sa session vers le rôle demandé.
    '
    ' Paramètres :
    ' - utilisateur : Utilisateur connecté (UtilisateurApplication)
    ' - password    : Mot de passe de l'utilisateur connecté (en clair, vérifié via PBKDF2)
    ' - targetRole  : Rôle demandé (AppRole)
    '
    ' Retour     :
    ' - AuthenticationResult : Success ; sinon MessageUI (ce qui ne va pas et quoi faire),
    '   RemainingAttempts et IsLocked.
    '
    ' Règles     :
    ' - Compte relu en base (état à jour) : actif, non verrouillé
    ' - Rôle demandé supérieur au rôle de base et au plus égal à RoleMaxElevation
    ' - Mot de passe correct ; sinon l'échec compte comme à la connexion (SECURITE_MAX_ECHECS_LOGIN) et
    '   le compte est verrouillé au seuil (décision de Joëlle, 20/10/2026, audit A.5) — la session en
    '   cours continue au rôle de base ; un administrateur déverrouille le compte.
    ' - Réussite : compteur d'échecs remis à zéro.
    '
    ' Important  :
    ' - Aucun mot de passe loggué
    ' - Ne modifie PAS la session (responsabilité de l'appelant)
    ' - V2.0.0 (20/10/2026) : essais comptés, verrouillage, résultat détaillé (avant : Boolean, essais
    '   illimités, compte non relu).
    ' -------------------------------------------------------------------------------------------------
    Public Function VerifierElevationUtilisateur(
        utilisateur As UtilisateurApplication,
        password As String,
        targetRole As AppRole
    ) As AuthenticationResult

        Dim result As New AuthenticationResult With {.Success = False, .MessageUI = "Élévation refusée."}

        Try

            If utilisateur Is Nothing Then
                GestionLog.EcrireLog("Élévation refusée : utilisateur absent.",
                                     GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security)
                Return result
            End If

            ' État à jour : verrouillé, désactivé ou mot de passe changé depuis la connexion.
            Dim compte As UtilisateurApplication =
                GestionAuthentification.RelireUtilisateurPourAuthentification(utilisateur.LoginUtilisateur)

            If compte Is Nothing OrElse Not compte.Actif Then
                GestionLog.EcrireLog($"Élévation refusée : utilisateur absent ou inactif ({utilisateur.LoginUtilisateur}).",
                                     GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security)
                result.MessageUI = "Élévation refusée : ce compte n'est plus actif."
                Return result
            End If

            If compte.CompteVerrouille Then
                GestionLog.EcrireLog($"Élévation refusée : compte verrouillé ({compte.LoginUtilisateur}).",
                                     GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security)
                result.IsLocked = True
                result.MessageUI = "Élévation refusée : ce compte est verrouillé." & Environment.NewLine &
                                   "Un administrateur doit le déverrouiller."
                Return result
            End If

            If targetRole <= compte.RoleUtilisateur OrElse targetRole > compte.RoleMaxElevation Then
                GestionLog.EcrireLog($"Élévation refusée : rôle non autorisé ({compte.LoginUtilisateur}, demandé={targetRole}, base={compte.RoleUtilisateur}, max={compte.RoleMaxElevation}).",
                                     GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security)
                result.MessageUI = "Élévation refusée : ce rôle n'est pas autorisé pour ce compte."
                Return result
            End If

            If Not PasswordSecurityHelper.VerifyPassword(password, compte.PasswordHash, compte.PasswordSalt) Then

                Dim maxEchecs As Integer =
                    GestionParametres.GetEntier(ClesParametres.SecuriteMaxEchecsLogin, ClesParametres.DefautMaxEchecsLogin)

                GestionAuthentification.IncrementerNbEchecsLogin(compte)
                Dim restants As Integer = Math.Max(maxEchecs - compte.NbEchecsLogin, 0)
                result.RemainingAttempts = restants

                GestionLog.EcrireLog($"Élévation refusée : mot de passe invalide ({compte.LoginUtilisateur}, rôle demandé={targetRole}). Échec {compte.NbEchecsLogin}/{maxEchecs}.",
                                     GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security)

                If compte.NbEchecsLogin >= maxEchecs Then
                    GestionAuthentification.VerrouillerCompteUtilisateur(compte)
                    result.IsLocked = True
                    result.MessageUI = "Mot de passe incorrect, trop d'essais : votre compte est verrouillé." & Environment.NewLine &
                                       "Vous pouvez finir votre travail en cours ; à la prochaine connexion, " &
                                       "un administrateur devra déverrouiller le compte."
                ElseIf restants = 1 Then
                    result.MessageUI = "Mot de passe incorrect." & Environment.NewLine &
                                       "Dernier essai avant le verrouillage du compte."
                Else
                    result.MessageUI = "Mot de passe incorrect." & Environment.NewLine &
                                       $"Il reste {restants} essai(s) avant le verrouillage du compte."
                End If

                Return result

            End If

            If compte.NbEchecsLogin > 0 Then
                GestionAuthentification.ReinitialiserNbEchecsLogin(compte)
            End If

            GestionLog.EcrireLog($"Élévation autorisée ({compte.LoginUtilisateur}, rôle demandé={targetRole}).",
                                 GestionLog.LogLevel.Rapide, GestionLog.LogCategory.Security)

            result.Success = True
            result.MessageUI = Nothing
            Return result

        Catch ex As Exception

            GestionLog.EcrireLog("Erreur VerifierElevationUtilisateur.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security, ex)
            Throw

        End Try

    End Function

#End Region

#Region "Gestion utilisateurs"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetUtilisateursPourListe
    ' Type       : List(Of UtilisateurApplication)
    ' Version    : V1.0.0
    ' Date       : 21/05/2026
    '
    ' Rôle       :
    ' Charge la liste des utilisateurs destinée à l'affichage dans UC_Utilisateurs.
    '
    ' Paramètres :
    ' - afficherInactifs : Indique si les utilisateurs désactivés doivent être inclus.
    '
    ' Retour     :
    ' - List(Of UtilisateurApplication) : Liste des utilisateurs applicatifs.
    '
    ' Remarques  :
    ' - Ne charge jamais password_hash ni password_salt.
    ' - Utilisée uniquement pour l'administration des utilisateurs.
    ' - Aucun accès DB direct depuis l'UI.
    '
    ' Exceptions :
    ' - Les erreurs DB sont journalisées puis propagées.
    ' -------------------------------------------------------------------------------------------------
    Public Function GetUtilisateursPourListe(
        afficherInactifs As Boolean
    ) As List(Of UtilisateurApplication)

        Dim utilisateurs As New List(Of UtilisateurApplication)

        Try
            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(
                    QueryUtilisateurs.SelectUtilisateursPourListe,
                    conn
                )

                    cmd.Parameters.AddWithValue(
                        "@afficher_inactifs",
                        If(afficherInactifs, 1, 0)
                    )

                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        While reader.Read()

                            utilisateurs.Add(
                                MapUtilisateurPourAdministration(reader)
                            )

                        End While

                    End Using

                End Using

            End Using

            Return utilisateurs

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur GetUtilisateursPourListe.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : MapUtilisateurPourAdministration
    ' Type       : UtilisateurApplication
    ' Version    : V1.0.0
    ' Date       : 21/05/2026
    '
    ' Rôle       :
    ' Mappe une ligne SQL sec_utilisateurs vers un objet UtilisateurApplication
    ' pour les écrans d'administration.
    '
    ' Paramètres :
    ' - reader : DataReader positionné sur une ligne utilisateur.
    '
    ' Retour     :
    ' - UtilisateurApplication : Utilisateur mappé sans données sensibles.
    '
    ' Remarques  :
    ' - Ne mappe jamais password_hash ni password_salt.
    ' - À utiliser pour UC_Utilisateurs et UtilisateurEdition.
    '
    ' Exceptions :
    ' - InvalidCastException : Si un champ non nullable contient une valeur incohérente.
    ' -------------------------------------------------------------------------------------------------
    Private Function MapUtilisateurPourAdministration(
        reader As MySqlDataReader
    ) As UtilisateurApplication

        Dim utilisateur As New UtilisateurApplication

        utilisateur.IdUtilisateur = CLng(reader("id_utilisateur"))
        utilisateur.CodeUtilisateur = reader("code_utilisateur").ToString()
        utilisateur.LoginUtilisateur = reader("login_utilisateur").ToString()
        utilisateur.NomAffichage = reader("nom_affichage").ToString()

        utilisateur.RoleUtilisateur =
            CType(
                [Enum].Parse(GetType(AppRole), reader("role_utilisateur").ToString()),
                AppRole
            )

        utilisateur.RoleMaxElevation =
            CType(
                [Enum].Parse(GetType(AppRole), reader("role_max_elevation").ToString()),
                AppRole
            )

        utilisateur.Actif = CBool(reader("actif"))
        utilisateur.MustChangePassword = CBool(reader("must_change_password"))
        utilisateur.NbEchecsLogin = CInt(reader("nb_echecs_login"))
        utilisateur.CompteVerrouille = CBool(reader("compte_verrouille"))

        If reader("date_verrouillage") IsNot DBNull.Value Then
            utilisateur.DateVerrouillage = CDate(reader("date_verrouillage"))
        End If

        If reader("dernier_login") IsNot DBNull.Value Then
            utilisateur.DernierLogin = CDate(reader("dernier_login"))
        End If

        utilisateur.DateCreation = CDate(reader("date_creation"))
        utilisateur.DateModification = CDate(reader("date_modification"))

        Return utilisateur

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : GetUtilisateurPourEdition
    ' Type      : UtilisateurApplication
    ' Version   : V1.0.0
    ' Date      : 23/05/2026
    '
    ' Rôle      :
    ' Charge un utilisateur complet pour affichage dans UtilisateurEdition.
    '
    ' Paramètres :
    ' - idUtilisateur : Identifiant technique de l'utilisateur.
    '
    ' Retour    :
    ' - UtilisateurApplication : utilisateur trouvé, ou Nothing si inexistant.
    '
    ' Remarques :
    ' - Ne charge jamais password_hash ni password_salt.
    ' - Utilisée uniquement pour l'administration utilisateurs.
    ' -------------------------------------------------------------------------------------------------
    Public Function GetUtilisateurPourEdition(
        idUtilisateur As Long
    ) As UtilisateurApplication

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(
                    QueryUtilisateurs.SelectUtilisateurPourEdition,
                    conn
                )

                    cmd.Parameters.AddWithValue(
                        "@id_utilisateur",
                        idUtilisateur
                    )

                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        If reader.Read() Then
                            Dim utilisateur As UtilisateurApplication = MapUtilisateurPourAdministration(reader)
                            utilisateur.PhotoFichier = LireString(reader, "photo_fichier")
                            Return utilisateur
                        End If

                    End Using

                End Using

            End Using

            Return Nothing

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur GetUtilisateurPourEdition.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CreateUtilisateur
    ' Version    : V1.2.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Crée un nouvel utilisateur applicatif dans la base de données.
    '
    ' Paramètres :
    ' - loginUtilisateur    : Login unique de l'utilisateur
    ' - nomAffichage        : Nom d'affichage de l'utilisateur
    ' - passwordInitial     : Mot de passe initial en clair
    ' - roleUtilisateur     : Rôle de base de l'utilisateur
    ' - roleMaxElevation    : Rôle maximal d'élévation autorisé
    ' - actif               : Indique si le compte est actif
    ' - mustChangePassword  : Force le changement de mot de passe à la première connexion
    '
    ' Retour     :
    ' - Long : ID du nouvel utilisateur créé, ou -1 en cas d'échec
    '
    ' Validations (refus : ErreurValidationException) :
    ' - ValiderCompte : login d'au moins 3 caractères et unique (relu dans la transaction), nom
    '   d'affichage, RoleMaxElevation >= RoleUtilisateur
    ' - Mot de passe conforme à PolitiqueMotDePasse (le mot de passe temporaire généré l'est)
    '
    ' Important  :
    ' - Génère automatiquement un code_utilisateur unique
    ' - Hash le mot de passe avec PBKDF2
    ' - Aucun mot de passe en clair dans les logs
    ' - Action sensible journalisée
    '
    ' Utilisé par :
    ' - UtilisateurEdition (création), GestionCabinet.CreerCompteSuperUser
    '
    ' Sécurité   :
    ' - contexte : rôle courant (élévation comprise) Admin exigé (audit A.3, 20/10/2026 ; ContexteOperation,
    '   audit C.1, 22/10/2026).
    '
    ' Remarques  :
    ' - V1.1.0 (21/10/2026, audit B.2) : contrôle du login et insertion dans une transaction ; variante
    '   Friend (conn, tx) pour créer un compte avec d'autres écritures (compte du praticien).
    ' - V1.2.0 (23/10/2026, audit C.2) : règles de ValiderCompte et de PolitiqueMotDePasse ; un refus de
    '   saisie lève ErreurValidationException au lieu de rendre -1 (-1 = échec technique de l'insertion).
    ' -------------------------------------------------------------------------------------------------
    Public Function CreateUtilisateur(
        contexte As ContexteOperation,
        loginUtilisateur As String,
        nomAffichage As String,
        passwordInitial As String,
        roleUtilisateur As AppRole,
        roleMaxElevation As AppRole,
        actif As Boolean,
        mustChangePassword As Boolean
    ) As Long

        ' Contrôle d'unicité et insertion dans une même transaction (audit B.2) ; le droit est vérifié
        ' par la variante (conn, tx).
        Return DatabaseManager.ExecuterEnTransaction(Of Long)(
            Function(conn, tx) CreateUtilisateur(contexte, loginUtilisateur, nomAffichage, passwordInitial,
                                                 roleUtilisateur, roleMaxElevation, actif, mustChangePassword, conn, tx))

    End Function

    ' Variante dans la transaction de l'appelant (compte créé avec d'autres écritures, ex. compte du
    ' praticien : GestionCabinet.CreerCompteSuperUser). Mêmes contrôles et même retour (-1 = refus).
    Friend Function CreateUtilisateur(
        contexte As ContexteOperation,
        loginUtilisateur As String,
        nomAffichage As String,
        passwordInitial As String,
        roleUtilisateur As AppRole,
        roleMaxElevation As AppRole,
        actif As Boolean,
        mustChangePassword As Boolean,
        conn As MySqlConnection,
        tx As MySqlTransaction
    ) As Long

    ' Droits vérifiés sur le rôle COURANT (élévation comprise ; décision de Joëlle, 20/10/2026) :
    ' l'écran le transmet, le métier refuse de lui-même (audit A.3).
        Autorisation.Exiger(contexte, AppRole.Admin, "Seul un administrateur peut créer un compte.")

        Try

            ' Règles de saisie (ValiderCompte, sans l'unicité : relue ci-dessous dans la transaction) et
            ' règle du mot de passe : refus rédigé pour l'utilisatrice (audit C.2).
            Dim erreurs As List(Of ErreurSaisie) = ErreursCompte(loginUtilisateur, nomAffichage, roleUtilisateur, roleMaxElevation)
            erreurs.AddRange(PolitiqueMotDePasse.Valider(passwordInitial))
            If erreurs.Count > 0 Then
                GestionLog.EcrireLog(
                    $"Création utilisateur refusée : {erreurs.Count} règle(s) de saisie non respectée(s).",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Security
                )
                ErreurValidationException.LeverSiErreurs(erreurs)
            End If

            Dim normalizedLogin As String = loginUtilisateur.Trim()

            ' Unicité du login, relue dans la transaction (un autre poste a pu le prendre entre-temps).
            If LoginExiste(normalizedLogin, conn, tx) Then
                GestionLog.EcrireLog(
                    $"Création utilisateur refusée : login '{normalizedLogin}' déjà existant.",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Security
                )
                ErreurValidationException.LeverSiErreurs(New List(Of ErreurSaisie) From {
                    New ErreurSaisie(NameOf(UtilisateurApplication.LoginUtilisateur), MESSAGE_LOGIN_PRIS)})
            End If

            ' Générer code_utilisateur unique (format: USR_yyyyMMddHHmmss)
            Dim codeUtilisateur As String =
                "USR_" & DateTime.Now.ToString("yyyyMMddHHmmss")

            ' Générer salt et hash du mot de passe
            Dim salt As String = PasswordSecurityHelper.GenerateSalt()
            Dim hash As String = PasswordSecurityHelper.HashPassword(passwordInitial, salt)

            ' Insertion en base de données (connexion et transaction de l'appelant)
            Using cmd As New MySqlCommand(
                QueryUtilisateurs.InsertUtilisateur,
                conn, tx
            )

                cmd.Parameters.AddWithValue("@login_utilisateur", normalizedLogin)
                cmd.Parameters.AddWithValue("@nom_affichage", nomAffichage.Trim())
                cmd.Parameters.AddWithValue("@password_hash", hash)
                cmd.Parameters.AddWithValue("@password_salt", salt)
                cmd.Parameters.AddWithValue("@role_utilisateur", roleUtilisateur.ToString())
                cmd.Parameters.AddWithValue("@role_max_elevation", roleMaxElevation.ToString())
                cmd.Parameters.AddWithValue("@actif", If(actif, 1, 0))
                cmd.Parameters.AddWithValue("@must_change_password", If(mustChangePassword, 1, 0))

                Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                If rowsAffected <> 1 Then
                    GestionLog.EcrireLog(
                        $"Création utilisateur échouée : aucune ligne insérée (login={normalizedLogin}).",
                        GestionLog.LogLevel.Succinct,
                        GestionLog.LogCategory.Security
                    )
                    Return -1
                End If

            End Using

            ' Récupérer l'ID du nouvel utilisateur via LASTVAL de la séquence
            ' (doit être exécuté dans la même connexion, après l'INSERT)
            Using cmdId As New MySqlCommand(
                "SELECT LASTVAL(seq_sec_utilisateurs);",
                conn, tx
            )
                Dim idUtilisateur As Long = Convert.ToInt64(cmdId.ExecuteScalar())

                If idUtilisateur <= 0 Then
                    GestionLog.EcrireLog(
                        $"Création utilisateur : ID invalide retourné (login={normalizedLogin}).",
                        GestionLog.LogLevel.Succinct,
                        GestionLog.LogCategory.Security
                    )
                    Return -1
                End If

                GestionLog.EcrireLog(
                    $"Utilisateur créé avec succès (ID={idUtilisateur}, login={normalizedLogin}, rôle={roleUtilisateur}).",
                    GestionLog.LogLevel.Rapide,
                    GestionLog.LogCategory.Security
                )

                Return idUtilisateur

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur CreateUtilisateur.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : UpdateUtilisateur
    ' Version    : V1.0.0
    ' Date       : 17/05/2026
    '
    ' Rôle       :
    ' Met à jour un utilisateur existant dans la base de données.
    '
    ' Paramètres :
    ' - idUtilisateur : Identifiant de l'utilisateur à mettre à jour
    ' - nomAffichage : Nouveau nom d'affichage de l'utilisateur
    ' - roleUtilisateur : Nouveau rôle de l'utilisateur
    ' - roleMaxElevation : Nouvelle élévation maximale du rôle de l'utilisateur
    ' - actif : Nouveau statut actif de l'utilisateur
    ' - mustChangePassword : Indicateur si l'utilisateur doit changer son mot de passe
    ' - contexte : Qui fait la modification (ContexteOperation : utilisateur et rôle courant)
    '
    ' Retour     :
    ' - Boolean : True si la mise à jour a réussi, False sinon
    '
    ' Remarques  :
    ' - Vérifie que l'utilisateur authentifié a les droits nécessaires (Admin ou SuperAdmin)
    ' - Vérifie les règles de hiérarchie des rôles
    ' - Logue toutes les modifications
    ' -------------------------------------------------------------------------------------------------
    Public Function UpdateUtilisateur(
        idUtilisateur As Long,
        nomAffichage As String,
        roleUtilisateur As AppRole,
        roleMaxElevation As AppRole,
        actif As Boolean,
        mustChangePassword As Boolean,
        contexte As ContexteOperation
    ) As Boolean

        ' Validation des entrées
        If idUtilisateur <= 0 Then
            Throw New ArgumentException(
                "L'identifiant de l'utilisateur doit être positif.",
                NameOf(idUtilisateur)
            )
        End If

        If String.IsNullOrWhiteSpace(nomAffichage) Then
            Throw New ArgumentException(
                "Le nom d'affichage ne peut pas être vide.",
                NameOf(nomAffichage)
            )
        End If


        ' Droits vérifiés sur le rôle COURANT (élévation comprise ; décisions de Joëlle, 20 et 22/10/2026).
        Autorisation.Exiger(contexte, AppRole.Admin, "Seuls les administrateurs peuvent modifier des utilisateurs.")
        Autorisation.ExigerUtilisateur(contexte)

        ' Vérification de la hiérarchie des rôles
        If roleUtilisateur > contexte.RoleCourant OrElse roleMaxElevation > contexte.RoleCourant Then
            Throw New UnauthorizedAccessException(
                "Vous ne pouvez pas attribuer un rôle supérieur au vôtre."
            )
        End If

        ' Son propre compte : ni rôle, ni élévation, ni état ne changent (on ne se retire pas ses droits,
        ' on ne se les donne pas non plus ; décision de Joëlle, 20/10/2026).
        If idUtilisateur = contexte.IdUtilisateur Then
            Dim actuel As UtilisateurApplication = GetUtilisateurPourEdition(idUtilisateur)
            If actuel IsNot Nothing AndAlso
               (actuel.RoleUtilisateur <> roleUtilisateur OrElse actuel.RoleMaxElevation <> roleMaxElevation OrElse actuel.Actif <> actif) Then
                Throw New InvalidOperationException(
                    "Vous ne pouvez pas modifier votre propre rôle, votre élévation ni l'état de votre compte." & Environment.NewLine &
                    "Demandez-le à un autre administrateur.")
            End If
        End If

        VerifierResteUnAdmin(idUtilisateur, roleUtilisateur = AppRole.Admin AndAlso actif)

        ' Vérification cohérence role_utilisateur <= role_max_elevation
        If roleUtilisateur > roleMaxElevation Then
            Throw New ArgumentException(
                "Le rôle utilisateur ne peut pas être supérieur au rôle d'élévation maximale.",
                NameOf(roleUtilisateur)
            )
        End If

        Try

            Using conn As MySqlConnection = DatabaseManager.CreateConnection()

                conn.Open()

                Using cmd As New MySqlCommand(
                    QueryUtilisateurs.UpdateUtilisateur,
                    conn
                )

                    cmd.Parameters.AddWithValue("@id_utilisateur", idUtilisateur)
                    cmd.Parameters.AddWithValue("@nom_affichage", nomAffichage.Trim())
                    cmd.Parameters.AddWithValue("@role_utilisateur", roleUtilisateur.ToString())
                    cmd.Parameters.AddWithValue("@role_max_elevation", roleMaxElevation.ToString())
                    cmd.Parameters.AddWithValue("@actif", actif)
                    cmd.Parameters.AddWithValue("@must_change_password", mustChangePassword)

                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected > 0 Then

                        GestionLog.EcrireLog(
                            $"Utilisateur modifié avec succès (ID={idUtilisateur}, nom={nomAffichage}, rôle={roleUtilisateur}, actif={actif}).",
                            GestionLog.LogLevel.Rapide,
                            GestionLog.LogCategory.Security
                        )

                        Return True

                    Else

                        GestionLog.EcrireLog(
                            $"Aucun utilisateur trouvé avec l'ID {idUtilisateur} lors de la mise à jour.",
                            GestionLog.LogLevel.Rapide,
                            GestionLog.LogCategory.Security
                        )

                        Return False

                    End If

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur UpdateUtilisateur.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GenererMotDePasseTemporaire
    ' Version    : V2.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       :
    ' Génère un mot de passe temporaire pour un nouveau compte ou une réinitialisation.
    '
    ' Retour     :
    ' - String : 12 caractères (au moins une majuscule, une minuscule, un chiffre, un caractère spécial)
    '
    ' Remarques  :
    ' - À communiquer à l'utilisateur, qui le change à sa première connexion.
    ' - Tirage cryptographique sans biais (RandomNumberGenerator.GetInt32), mélange de Fisher-Yates.
    ' - Caractères ambigus exclus (0/O, 1/l/I) pour éviter les erreurs de saisie.
    ' - V2.0.0 (20/10/2026, audit § 3.2) : plus de repli sur un mot de passe constant en cas d'erreur
    '   (l'exception remonte à l'écran) ; RNGCryptoServiceProvider (obsolète) et tirage « modulo »
    '   (biaisé) remplacés.
    ' -------------------------------------------------------------------------------------------------
    Public Function GenererMotDePasseTemporaire() As String

        ' Caractères utilisables (sans ambiguïté : pas de 0/O, 1/l/I)
        Const MAJUSCULES As String = "ABCDEFGHJKLMNPQRSTUVWXYZ"
        Const MINUSCULES As String = "abcdefghjkmnpqrstuvwxyz"
        Const CHIFFRES As String = "23456789"
        Const SPECIAUX As String = "!@#$%&*"
        Const LONGUEUR As Integer = 12

        Dim tous As String = MAJUSCULES & MINUSCULES & CHIFFRES & SPECIAUX
        Dim caracteres As New List(Of Char)(LONGUEUR)

        ' Au moins un caractère de chaque famille, puis le reste parmi tous.
        For Each famille In {MAJUSCULES, MINUSCULES, CHIFFRES, SPECIAUX}
            caracteres.Add(TirerCaractere(famille))
        Next
        While caracteres.Count < LONGUEUR
            caracteres.Add(TirerCaractere(tous))
        End While

        ' Mélange (Fisher-Yates) : les familles imposées ne restent pas en tête.
        For i As Integer = caracteres.Count - 1 To 1 Step -1
            Dim j As Integer = System.Security.Cryptography.RandomNumberGenerator.GetInt32(i + 1)
            Dim temp As Char = caracteres(i)
            caracteres(i) = caracteres(j)
            caracteres(j) = temp
        Next

        Return New String(caracteres.ToArray())

    End Function

    Private Function TirerCaractere(alphabet As String) As Char
        Return alphabet(System.Security.Cryptography.RandomNumberGenerator.GetInt32(alphabet.Length))
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : DeverrouillerUtilisateur
    ' Version    : V1.0.0
    ' Date       : 17/05/2026
    '
    ' Rôle       :
    ' Déverrouille un compte utilisateur bloqué après trop de tentatives de connexion échouées.
    '
    ' Paramètres :
    ' - idUtilisateur : Identifiant de l'utilisateur à déverrouiller
    ' - contexte : Qui fait l'action (ContexteOperation)
    '
    ' Retour     :
    ' - Boolean : True si le déverrouillage a réussi, False sinon
    '
    ' Remarques  :
    ' - Réinitialise nb_echecs_login à 0
    ' - Passe compte_verrouille à 0
    ' - Efface date_verrouillage (NULL)
    ' - Seuls les Admin peuvent déverrouiller des comptes
    ' - Logue l'action pour traçabilité
    ' -------------------------------------------------------------------------------------------------
    Public Function DeverrouillerUtilisateur(
        idUtilisateur As Long,
        contexte As ContexteOperation
    ) As Boolean

        ' Validation des entrées
        If idUtilisateur <= 0 Then
            Throw New ArgumentException(
                "L'identifiant de l'utilisateur doit être positif.",
                NameOf(idUtilisateur)
            )
        End If


        ' Droits vérifiés sur le rôle COURANT (élévation comprise ; décisions de Joëlle, 20 et 22/10/2026).
        Autorisation.Exiger(contexte, AppRole.Admin, "Seuls les administrateurs peuvent déverrouiller les comptes.")
        Autorisation.ExigerUtilisateur(contexte)

        Try

            Using conn As MySqlConnection = DatabaseManager.CreateConnection()

                conn.Open()

                Using cmd As New MySqlCommand(
                    QueryUtilisateurs.DeverrouillerUtilisateur,
                    conn
                )

                    cmd.Parameters.AddWithValue("@id_utilisateur", idUtilisateur)

                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected > 0 Then

                        GestionLog.EcrireLog(
                            $"Compte déverrouillé pour l'utilisateur ID={idUtilisateur} par {contexte.LoginUtilisateur}.",
                            GestionLog.LogLevel.Rapide,
                            GestionLog.LogCategory.Security
                        )

                        Return True

                    Else

                        GestionLog.EcrireLog(
                            $"Aucun utilisateur trouvé avec l'ID {idUtilisateur} lors du déverrouillage.",
                            GestionLog.LogLevel.Rapide,
                            GestionLog.LogCategory.Security
                        )

                        Return False

                    End If

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur DeverrouillerUtilisateur.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetPhotoUtilisateur
    ' Version    : V1.0.0
    ' Date       : 12/10/2026
    '
    ' Rôle       :
    ' Retourne le nom du fichier photo propre d'un compte (Nothing si aucune), pour l'affichage.
    '
    ' Remarques  :
    ' - Lecture seule, sans contrôle de droits : sert au bandeau de la Home pour le compte connecté.
    ' - La règle d'affichage (photo du praticien lié en priorité) est portée par PhotosUtilisateur.
    ' -------------------------------------------------------------------------------------------------
    Public Function GetPhotoUtilisateur(idUtilisateur As Long) As String

        Using conn As MySqlConnection = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryUtilisateurs.SelectPhotoUtilisateur, conn)
                cmd.Parameters.AddWithValue("@id_utilisateur", idUtilisateur)
                Dim valeur As Object = cmd.ExecuteScalar()
                Return If(valeur Is Nothing OrElse valeur Is DBNull.Value, Nothing, valeur.ToString())
            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : UpdatePhotoUtilisateur
    ' Version    : V1.0.0
    ' Date       : 12/10/2026
    '
    ' Rôle       :
    ' Enregistre le nom du fichier photo propre d'un compte (après import du fichier).
    '
    ' Paramètres :
    ' - idUtilisateur     : Compte concerné
    ' - photoFichier      : Nom seul du fichier (Nothing = aucune photo)
    ' - contexte : Qui fait l'action (ContexteOperation)
    '
    ' Remarques  :
    ' - Mêmes droits que les autres actions d'administration des comptes : Admin uniquement.
    ' - Logue l'action (sans chemin complet) pour traçabilité.
    ' -------------------------------------------------------------------------------------------------
    Public Function UpdatePhotoUtilisateur(
        idUtilisateur As Long,
        photoFichier As String,
        contexte As ContexteOperation
    ) As Boolean

        If idUtilisateur <= 0 Then
            Throw New ArgumentException("L'identifiant de l'utilisateur doit être positif.", NameOf(idUtilisateur))
        End If

        ' Droits vérifiés sur le rôle COURANT (élévation comprise ; décisions de Joëlle, 20 et 22/10/2026).
        Autorisation.Exiger(contexte, AppRole.Admin, "Seuls les administrateurs peuvent modifier la photo d'un compte.")
        Autorisation.ExigerUtilisateur(contexte)

        Try
            Using conn As MySqlConnection = DatabaseManager.OpenConnection()
                Using cmd As New MySqlCommand(QueryUtilisateurs.UpdatePhotoUtilisateur, conn)
                    cmd.Parameters.AddWithValue("@id_utilisateur", idUtilisateur)
                    cmd.Parameters.AddWithValue("@photo_fichier", ValeurOuDBNull(photoFichier))
                    Dim succes As Boolean = cmd.ExecuteNonQuery() > 0
                    GestionLog.EcrireLog(
                        $"Photo du compte ID={idUtilisateur} {If(succes, "mise à jour", "non mise à jour (compte introuvable)")} par {contexte.LoginUtilisateur}.",
                        GestionLog.LogLevel.Rapide,
                        GestionLog.LogCategory.Security)
                    Return succes
                End Using
            End Using
        Catch ex As Exception
            GestionLog.EcrireLog("Erreur UpdatePhotoUtilisateur.", GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security, ex)
            Throw
        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ActiverDesactiverUtilisateur
    ' Version    : V1.0.0
    ' Date       : 17/05/2026
    '
    ' Rôle       :
    ' Active ou désactive un utilisateur.
    '
    ' Paramètres :
    ' - idUtilisateur : Identifiant de l'utilisateur à activer/désactiver
    ' - actif : True pour activer, False pour désactiver
    ' - contexte : Qui fait l'action (ContexteOperation)
    '
    ' Retour     :
    ' - Boolean : True si l'action a réussi, False sinon
    '
    ' Remarques  :
    ' - Un utilisateur désactivé ne peut plus se connecter
    ' - Seuls les Admin peuvent activer/désactiver des utilisateurs
    ' - Un admin ne peut pas se désactiver lui-même
    ' - Logue l'action pour traçabilité
    ' -------------------------------------------------------------------------------------------------
    Public Function ActiverDesactiverUtilisateur(
        idUtilisateur As Long,
        actif As Boolean,
        contexte As ContexteOperation
    ) As Boolean

        ' Validation des entrées
        If idUtilisateur <= 0 Then
            Throw New ArgumentException(
                "L'identifiant de l'utilisateur doit être positif.",
                NameOf(idUtilisateur)
            )
        End If


        ' Droits vérifiés sur le rôle COURANT (élévation comprise ; décisions de Joëlle, 20 et 22/10/2026).
        Autorisation.Exiger(contexte, AppRole.Admin, "Seuls les administrateurs peuvent activer/désactiver les utilisateurs.")
        Autorisation.ExigerUtilisateur(contexte)

        ' Interdire la désactivation de soi-même
        If idUtilisateur = contexte.IdUtilisateur AndAlso Not actif Then
            Throw New InvalidOperationException(
                "Vous ne pouvez pas vous désactiver vous-même."
            )
        End If

        VerifierResteUnAdmin(idUtilisateur, actif)

        Try

            Using conn As MySqlConnection = DatabaseManager.CreateConnection()

                conn.Open()

                Using cmd As New MySqlCommand(
                    QueryUtilisateurs.UpdateActifUtilisateur,
                    conn
                )

                    cmd.Parameters.AddWithValue("@id_utilisateur", idUtilisateur)
                    cmd.Parameters.AddWithValue("@actif", actif)

                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected > 0 Then

                        Dim action As String = If(actif, "activé", "désactivé")

                        GestionLog.EcrireLog(
                            $"Utilisateur ID={idUtilisateur} {action} par {contexte.LoginUtilisateur}.",
                            GestionLog.LogLevel.Rapide,
                            GestionLog.LogCategory.Security
                        )

                        Return True

                    Else

                        GestionLog.EcrireLog(
                            $"Aucun utilisateur trouvé avec l'ID {idUtilisateur} lors de l'activation/désactivation.",
                            GestionLog.LogLevel.Rapide,
                            GestionLog.LogCategory.Security
                        )

                        Return False

                    End If

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur ActiverDesactiverUtilisateur.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ResetPasswordUtilisateurWithCustomPassword
    ' Version    : V1.1.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Réinitialise le mot de passe d'un utilisateur avec un mot de passe spécifié (depuis ChangePassword en mode AdminReset).
    '
    ' Paramètres :
    ' - idUtilisateur : Identifiant de l'utilisateur à réinitialiser
    ' - nouveauPassword : Nouveau mot de passe spécifié par l'admin
    '
    ' Retour     :
    ' - Boolean : True si la réinitialisation a réussi, False sinon
    '
    ' Remarques  :
    ' - Appelée uniquement depuis ChangePassword.vb en mode AdminReset
    ' - Force le changement de mot de passe à la prochaine connexion (must_change_password = 1)
    ' - Déverrouille le compte si nécessaire (nb_echecs_login = 0, compte_verrouille = 0)
    ' - Droits vérifiés ici sur le rôle courant (Admin, élévation comprise ; audit A.3, 20/10/2026) :
    '   avant, la méthode se fiait à l'écran.
    ' - Logue l'action pour traçabilité
    ' - V1.1.0 (23/10/2026, audit C.2) : mot de passe hors règle refusé (PolitiqueMotDePasse,
    '   ErreurValidationException) ; seul le mot de passe vide l'était.
    ' -------------------------------------------------------------------------------------------------
    Public Function ResetPasswordUtilisateurWithCustomPassword(
        contexte As ContexteOperation,
        idUtilisateur As Long,
        nouveauPassword As String
    ) As Boolean

        Autorisation.Exiger(contexte, AppRole.Admin, "Seuls les administrateurs peuvent réinitialiser un mot de passe.")

        ' Validation des entrées
        If idUtilisateur <= 0 Then
            Throw New ArgumentException(
                "L'identifiant de l'utilisateur doit être positif.",
                NameOf(idUtilisateur)
            )
        End If

        ErreurValidationException.LeverSiErreurs(PolitiqueMotDePasse.Valider(nouveauPassword))

        Try

            ' Génération du hash et du salt
            Dim salt As String = PasswordSecurityHelper.GenerateSalt()
            Dim hash As String = PasswordSecurityHelper.HashPassword(nouveauPassword, salt)

            Using conn As MySqlConnection = DatabaseManager.CreateConnection()

                conn.Open()

                Using cmd As New MySqlCommand(
                    QueryUtilisateurs.ResetPasswordUtilisateur,
                    conn
                )

                    cmd.Parameters.AddWithValue("@id_utilisateur", idUtilisateur)
                    cmd.Parameters.AddWithValue("@password_hash", hash)
                    cmd.Parameters.AddWithValue("@password_salt", salt)

                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected > 0 Then

                        GestionLog.EcrireLog(
                            $"Mot de passe réinitialisé pour l'utilisateur ID={idUtilisateur} (mot de passe personnalisé).",
                            GestionLog.LogLevel.Rapide,
                            GestionLog.LogCategory.Security
                        )

                        Return True

                    Else

                        GestionLog.EcrireLog(
                            $"Aucun utilisateur trouvé avec l'ID {idUtilisateur} lors de la réinitialisation du mot de passe.",
                            GestionLog.LogLevel.Rapide,
                            GestionLog.LogCategory.Security
                        )

                        Return False

                    End If

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur ResetPasswordUtilisateurWithCustomPassword.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
                ex
            )

            Throw

        End Try

    End Function



#End Region

#Region "Garde-fous des administrateurs"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : VerifierResteUnAdmin
    ' Version    : V1.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       :
    ' Refuse une modification qui laisserait la base sans administrateur actif (décision de Joëlle,
    ' 20/10/2026 : on ne s'enferme pas dehors).
    '
    ' Paramètres :
    ' - idUtilisateur   : compte modifié
    ' - resteAdminActif : True si, après la modification, ce compte est encore Admin et actif
    ' -------------------------------------------------------------------------------------------------
    Private Sub VerifierResteUnAdmin(idUtilisateur As Long, resteAdminActif As Boolean)

        If resteAdminActif Then Return

        Dim actuel As UtilisateurApplication = GetUtilisateurPourEdition(idUtilisateur)
        If actuel Is Nothing OrElse actuel.RoleUtilisateur <> AppRole.Admin OrElse Not actuel.Actif Then Return

        Dim autresAdmins As Integer
        Using conn As MySqlConnection = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryUtilisateurs.SelectCountAutresAdminsActifs, conn)
                cmd.Parameters.AddWithValue("@id_utilisateur", idUtilisateur)
                cmd.Parameters.AddWithValue("@role_admin", AppRole.Admin.ToString())
                autresAdmins = Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using

        If autresAdmins = 0 Then
            Throw New InvalidOperationException(
                "Ce compte est le dernier administrateur actif : il doit le rester." & Environment.NewLine &
                "Donnez d'abord le rôle Administrateur à un autre compte actif.")
        End If

    End Sub

#End Region

End Module
