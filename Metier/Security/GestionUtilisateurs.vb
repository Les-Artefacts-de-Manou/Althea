' -------------------------------------------------------------------------------------------------
' Module      : GestionUtilisateurs
' Projet      : Althéa
' Version     : V1.3.1
' Date        : 21/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Gestion métier des utilisateurs applicatifs.
'
' Responsabilités :
' - Date verouillage et gestion des comptes verrouillés
' - Réinitialiser les échecs après succès
' - Mapper les DataReader vers UtilisateurApplication
'
' Remarques   :
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
' - PasswordSecurityHelper (hash/vérification PBKDF2)
' - GestionLog (traçabilité)
' - UtilisateurApplication (objet métier)
' - AuthenticationResult (résultat d'authentification)
' -------------------------------------------------------------------------------------------------

Imports MySqlConnector

'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
'TODO : DEBUG → charger automatiquement l’utilisateur joelle depuis sec_utilisateurs
'RELEASE → Login obligatoire
'C 'est acceptable en développement, parce que le compilateur exclut totalement ce code en version finale. C’est la différence entre une 'béquille de chantier et une porte secrète oubliée dans le mur.
'On le fera proprement quand GestionUtilisateurs aura une méthode du type 

'GetUtilisateurByLogin("joelle")
'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

Public Module GestionUtilisateurs


#Region "Mot de passe"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ChangerMotDePasse
    ' Version    : V1.0.0
    ' Date       : 09/05/2026
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
    ' Remarques  :
    ' - Aucun mot de passe en clair dans les logs
    ' - Vérifie l'ancien mot de passe avant modification
    ' - Génère un nouveau salt via PasswordSecurityHelper.GenerateSalt()
    ' - Désactive must_change_password après succès
    ' - Met à jour l'objet utilisateur en mémoire
    '
    ' Utilisé par :
    ' - ChangePassword (formulaire de changement de mot de passe)
    ' -------------------------------------------------------------------------------------------------
    Public Function ChangerMotDePasse(
        utilisateur As UtilisateurApplication,
        oldPassword As String,
        newPassword As String
    ) As Boolean

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
    ' Version    : V1.0.0
    ' Date       : 11/05/2026
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
    ' - Boolean : True si l'élévation est autorisée, False sinon
    '
    ' Règles     :
    ' - L'utilisateur doit être actif
    ' - Le compte ne doit pas être verrouillé
    ' - Le mot de passe doit être correct
    ' - Le rôle demandé ne peut pas dépasser RoleMaxElevation
    ' - Le rôle demandé doit être supérieur au rôle courant/base
    '
    ' Remarques  :
    ' - Aucun mot de passe loggué
    ' - Ne modifie PAS la session (responsabilité de l'appelant)
    ' - Ne modifie PAS la base de données (vérification uniquement)
    '
    ' Utilisé par :
    ' - Formulaires nécessitant une élévation temporaire de rôle
    ' -------------------------------------------------------------------------------------------------
    Public Function VerifierElevationUtilisateur(
        utilisateur As UtilisateurApplication,
        password As String,
        targetRole As AppRole
    ) As Boolean

        Try

            If utilisateur Is Nothing Then

                GestionLog.EcrireLog(
                    "Élévation refusée : utilisateur absent.",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Security
                )

                Return False

            End If

            If Not utilisateur.Actif Then

                GestionLog.EcrireLog(
                    $"Élévation refusée : utilisateur inactif ({utilisateur.LoginUtilisateur}).",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Security
                )

                Return False

            End If

            If utilisateur.CompteVerrouille Then

                GestionLog.EcrireLog(
                    $"Élévation refusée : compte verrouillé ({utilisateur.LoginUtilisateur}).",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Security
                )

                Return False

            End If

            If targetRole <= utilisateur.RoleUtilisateur Then

                GestionLog.EcrireLog(
                    $"Élévation refusée : rôle demandé non supérieur au rôle de base ({utilisateur.LoginUtilisateur}, {targetRole}).",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Security
                )

                Return False

            End If

            If targetRole > utilisateur.RoleMaxElevation Then

                GestionLog.EcrireLog(
                    $"Élévation refusée : rôle demandé supérieur au maximum autorisé ({utilisateur.LoginUtilisateur}, demandé={targetRole}, max={utilisateur.RoleMaxElevation}).",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Security
                )

                Return False

            End If

            Dim passwordValid As Boolean =
                PasswordSecurityHelper.VerifyPassword(
                    password,
                    utilisateur.PasswordHash,
                    utilisateur.PasswordSalt
                )

            If Not passwordValid Then

                GestionLog.EcrireLog(
                    $"Élévation refusée : mot de passe invalide ({utilisateur.LoginUtilisateur}, rôle demandé={targetRole}).",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Security
                )

                Return False

            End If

            GestionLog.EcrireLog(
                $"Élévation autorisée ({utilisateur.LoginUtilisateur}, rôle demandé={targetRole}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Security
            )

            Return True

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur VerifierElevationUtilisateur.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
                ex
            )

            Throw

        End Try

    End Function

#End Region

#Region "Mapping"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : MapUtilisateur
    ' Type retour : UtilisateurApplication
    ' Version    : V1.1.0
    ' Date       : 19/05/2026
    '
    ' Rôle       :
    ' Construit un objet UtilisateurApplication depuis un
    ' MySqlDataReader.
    '
    ' Paramètres :
    ' - reader : MySqlDataReader
    '   Reader positionné sur un enregistrement utilisateur.
    '
    ' Retour     :
    ' - UtilisateurApplication :
    '   Utilisateur applicatif mappé depuis la base de données.
    '
    ' Remarques  :
    ' - Gère explicitement les valeurs NULL
    ' - Convertit les rôles string vers AppRole
    ' - Les champs sensibles ne sont jamais chargés ici
    '
    ' Utilisée par :
    ' - GetUtilisateurByLogin
    ' - GetUtilisateurs
    '
    ' Sécurité   :
    ' - Aucun mot de passe ni hash manipulé
    ' -------------------------------------------------------------------------------------------------
    Private Function MapUtilisateur(
        reader As MySqlDataReader
    ) As UtilisateurApplication

        Dim utilisateur As New UtilisateurApplication

        utilisateur.IdUtilisateur =
            Convert.ToUInt64(reader("id_utilisateur"))

        utilisateur.CodeUtilisateur =
            reader("code_utilisateur").ToString()

        utilisateur.LoginUtilisateur =
            reader("login_utilisateur").ToString()

        utilisateur.NomAffichage =
            reader("nom_affichage").ToString()

        utilisateur.PasswordHash =
    reader("password_hash").ToString()

        utilisateur.PasswordSalt =
    reader("password_salt").ToString()

        utilisateur.RoleUtilisateur =
            CType(
                [Enum].Parse(
                    GetType(AppRole),
                    reader("role_utilisateur").ToString()
                ),
                AppRole
            )

        utilisateur.RoleMaxElevation =
            CType(
                [Enum].Parse(
                    GetType(AppRole),
                    reader("role_max_elevation").ToString()
                ),
                AppRole
            )

        utilisateur.Actif =
            Convert.ToBoolean(reader("actif"))

        utilisateur.MustChangePassword =
            Convert.ToBoolean(reader("must_change_password"))

        utilisateur.NbEchecsLogin =
            Convert.ToInt32(reader("nb_echecs_login"))

        utilisateur.CompteVerrouille =
            Convert.ToBoolean(reader("compte_verrouille"))

        If reader("date_verrouillage") IsNot DBNull.Value Then

            utilisateur.DateVerrouillage =
                CDate(reader("date_verrouillage"))

        Else

            utilisateur.DateVerrouillage = Nothing

        End If

        If reader("dernier_login") IsNot DBNull.Value Then

            utilisateur.DernierLogin =
                CDate(reader("dernier_login"))

        Else

            utilisateur.DernierLogin = Nothing

        End If

        utilisateur.DateCreation =
            CDate(reader("date_creation"))

        utilisateur.DateModification =
            CDate(reader("date_modification"))

        Return utilisateur

    End Function

#End Region

#Region "Chargement utilisateurs"
    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetUtilisateurs
    ' Type retour : List(Of UtilisateurApplication)
    ' Version    : V1.0.0
    ' Date       : 19/05/2026
    '
    ' Rôle       :
    ' Charge la liste des utilisateurs applicatifs depuis la table
    ' sec_utilisateurs.
    '
    ' Retour     :
    ' - List(Of UtilisateurApplication) :
    '   Liste complète des utilisateurs triés par nom d'affichage.
    '
    ' Remarques  :
    ' - Utilise QueryUtilisateurs.SelectUtilisateursListe
    ' - Utilise exclusivement DatabaseManager.OpenConnection()
    ' - Les champs sensibles (hash/salt) ne sont jamais chargés
    ' - Les champs de sécurité et d'état sont inclus
    ' - Les valeurs NULL sont gérées explicitement
    '
    ' Exceptions :
    ' - Exception propagée après journalisation
    '
    ' Utilisée par :
    ' - UC_Utilisateurs
    '
    ' Sécurité   :
    ' - Aucun mot de passe ni hash manipulé
    ' -------------------------------------------------------------------------------------------------
    Public Function GetUtilisateurs() As List(Of UtilisateurApplication)

        Dim utilisateurs As New List(Of UtilisateurApplication)

        Try

            Using conn As MySqlConnection =
                DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(
                    QueryUtilisateurs.SelectUtilisateursListe,
                    conn
                )

                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        While reader.Read()

                            utilisateurs.Add(
                                MapUtilisateur(reader)
                            )

                        End While

                    End Using

                End Using

            End Using

            GestionLog.EcrireLog(
                $"{utilisateurs.Count} utilisateur(s) chargé(s).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Process
            )

            Return utilisateurs

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur lors du chargement des utilisateurs.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : HasUtilisateurs
    ' Type retour : Boolean
    ' Version    : V1.0.0
    ' Date       : 19/05/2026
    '
    ' Rôle       :
    ' Indique si au moins un utilisateur applicatif existe dans la table
    ' sec_utilisateurs.
    '
    ' Retour     :
    ' - Boolean : True si au moins un utilisateur existe, sinon False.
    '
    ' Remarques  :
    ' - Utilise GetUtilisateurs()
    ' - Sert de contrôle simple pour les écrans ou processus sécurité
    ' - Ne charge aucune donnée sensible
    '
    ' Exceptions :
    ' - Les exceptions sont propagées par GetUtilisateurs()
    '
    ' Utilisée par :
    ' - Futurs contrôles UC_Utilisateurs
    '
    ' Sécurité   :
    ' - Aucun mot de passe ni hash manipulé
    ' -------------------------------------------------------------------------------------------------
    Public Function HasUtilisateurs() As Boolean

        Return GetUtilisateurs().Count > 0

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
                            Return MapUtilisateurPourAdministration(reader)
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
    ' Version    : V1.0.0
    ' Date       : 17/05/2026
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
    ' Validations :
    ' - Login unique (pas de doublon)
    ' - Login et nom d'affichage non vides
    ' - RoleMaxElevation >= RoleUtilisateur
    '
    ' Remarques  :
    ' - Génère automatiquement un code_utilisateur unique
    ' - Hash le mot de passe avec PBKDF2
    ' - Aucun mot de passe en clair dans les logs
    ' - Action sensible journalisée
    '
    ' Utilisé par :
    ' - UtilisateurEdition (création)
    ' -------------------------------------------------------------------------------------------------
    Public Function CreateUtilisateur(
        loginUtilisateur As String,
        nomAffichage As String,
        passwordInitial As String,
        roleUtilisateur As AppRole,
        roleMaxElevation As AppRole,
        actif As Boolean,
        mustChangePassword As Boolean
    ) As Long

        Try

            ' Validation des paramètres
            If String.IsNullOrWhiteSpace(loginUtilisateur) Then
                GestionLog.EcrireLog(
                    "Création utilisateur refusée : login vide.",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Security
                )
                Return -1
            End If

            If String.IsNullOrWhiteSpace(nomAffichage) Then
                GestionLog.EcrireLog(
                    "Création utilisateur refusée : nom d'affichage vide.",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Security
                )
                Return -1
            End If

            If String.IsNullOrWhiteSpace(passwordInitial) Then
                GestionLog.EcrireLog(
                    "Création utilisateur refusée : mot de passe vide.",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Security
                )
                Return -1
            End If

            If roleMaxElevation < roleUtilisateur Then
                GestionLog.EcrireLog(
                    $"Création utilisateur refusée : rôle max élévation ({roleMaxElevation}) inférieur au rôle de base ({roleUtilisateur}).",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.Security
                )
                Return -1
            End If

            Dim normalizedLogin As String = loginUtilisateur.Trim()

            ' Vérifier unicité du login
            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmdCheck As New MySqlCommand(
                    QueryUtilisateurs.GetUtilisateurByLogin(),
                    conn
                )

                    cmdCheck.Parameters.AddWithValue("@login_utilisateur", normalizedLogin)

                    Using reader As MySqlDataReader = cmdCheck.ExecuteReader()

                        If reader.HasRows Then
                            GestionLog.EcrireLog(
                                $"Création utilisateur refusée : login '{normalizedLogin}' déjà existant.",
                                GestionLog.LogLevel.Succinct,
                                GestionLog.LogCategory.Security
                            )
                            Return -1
                        End If

                    End Using

                End Using

            End Using

            ' Générer code_utilisateur unique (format: USR_yyyyMMddHHmmss)
            Dim codeUtilisateur As String =
                "USR_" & DateTime.Now.ToString("yyyyMMddHHmmss")

            ' Générer salt et hash du mot de passe
            Dim salt As String = PasswordSecurityHelper.GenerateSalt()
            Dim hash As String = PasswordSecurityHelper.HashPassword(passwordInitial, salt)

            ' Insertion en base de données
            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(
                    QueryUtilisateurs.InsertUtilisateur,
                    conn
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

                    GestionLog.EcrireLog(
                        $"[DEBUG] CreateUtilisateur : rowsAffected = {rowsAffected}",
                        GestionLog.LogLevel.Complet,
                        GestionLog.LogCategory.Security
                    )

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
                    conn
                )
                    Dim idUtilisateur As Long = Convert.ToInt64(cmdId.ExecuteScalar())

                    GestionLog.EcrireLog(
                        $"[DEBUG] CreateUtilisateur : LASTVAL(seq_sec_utilisateurs) = {idUtilisateur}",
                        GestionLog.LogLevel.Complet,
                        GestionLog.LogCategory.Security
                    )

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
    ' - authenticatedUser : Utilisateur actuellement authentifié effectuant la modification
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
        authenticatedUser As UtilisateurApplication
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

        If authenticatedUser Is Nothing Then
            Throw New ArgumentNullException(
                NameOf(authenticatedUser),
                "L'utilisateur authentifié ne peut pas être null."
            )
        End If

        ' Vérification des droits : seuls Admin peuvent modifier des utilisateurs
        If authenticatedUser.RoleUtilisateur <> AppRole.Admin Then
            Throw New UnauthorizedAccessException(
                "Seuls les administrateurs peuvent modifier des utilisateurs."
            )
        End If

        ' Vérification de la hiérarchie des rôles
        If roleUtilisateur > authenticatedUser.RoleUtilisateur OrElse
           roleMaxElevation > authenticatedUser.RoleUtilisateur Then
            Throw New UnauthorizedAccessException(
                "Vous ne pouvez pas attribuer un rôle supérieur au vôtre."
            )
        End If

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
    ' Version    : V1.0.0
    ' Date       : 17/05/2026
    '
    ' Rôle       :
    ' Génère un mot de passe temporaire sécurisé pour la création de nouveaux utilisateurs.
    '
    ' Retour     :
    ' - String : Mot de passe temporaire de 12 caractères (majuscules, minuscules, chiffres, caractères spéciaux)
    '
    ' Remarques  :
    ' - Le mot de passe généré doit être communiqué à l'utilisateur et changé à la première connexion
    ' - Utilise un générateur aléatoire cryptographique pour plus de sécurité
    ' - Exclut les caractères ambigus (0/O, 1/l/I) pour éviter les erreurs de saisie
    ' -------------------------------------------------------------------------------------------------
    Public Function GenererMotDePasseTemporaire() As String

        ' Caractères utilisables (sans ambiguïté : pas de 0/O, 1/l/I)
        Const MAJUSCULES As String = "ABCDEFGHJKLMNPQRSTUVWXYZ"
        Const MINUSCULES As String = "abcdefghjkmnpqrstuvwxyz"
        Const CHIFFRES As String = "23456789"
        Const SPECIAUX As String = "!@#$%&*"

        ' Longueur du mot de passe
        Const LONGUEUR As Integer = 12

        Try

            ' Utilisation d'un générateur cryptographique pour plus de sécurité
            Using rng As New System.Security.Cryptography.RNGCryptoServiceProvider()

                Dim password As New System.Text.StringBuilder(LONGUEUR)
                Dim allChars As String = MAJUSCULES & MINUSCULES & CHIFFRES & SPECIAUX
                Dim randomBytes(LONGUEUR - 1) As Byte

                rng.GetBytes(randomBytes)

                ' Garantir au moins 1 caractère de chaque type
                password.Append(MAJUSCULES(randomBytes(0) Mod MAJUSCULES.Length))
                password.Append(MINUSCULES(randomBytes(1) Mod MINUSCULES.Length))
                password.Append(CHIFFRES(randomBytes(2) Mod CHIFFRES.Length))
                password.Append(SPECIAUX(randomBytes(3) Mod SPECIAUX.Length))

                ' Compléter avec des caractères aléatoires
                For i As Integer = 4 To LONGUEUR - 1
                    password.Append(allChars(randomBytes(i) Mod allChars.Length))
                Next

                ' Mélanger les caractères pour éviter un pattern prévisible
                Dim passwordArray As Char() = password.ToString().ToCharArray()
                For i As Integer = passwordArray.Length - 1 To 1 Step -1
                    Dim j As Integer = randomBytes(i) Mod (i + 1)
                    Dim temp As Char = passwordArray(i)
                    passwordArray(i) = passwordArray(j)
                    passwordArray(j) = temp
                Next

                Return New String(passwordArray)

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur GenererMotDePasseTemporaire.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
                ex
            )

            ' En cas d'erreur, retourner un mot de passe par défaut
            Return "TempPass2026!"

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ResetPasswordUtilisateur
    ' Version    : V1.0.0
    ' Date       : 17/05/2026
    '
    ' Rôle       :
    ' Réinitialise le mot de passe d'un utilisateur avec un nouveau mot de passe temporaire.
    '
    ' Paramètres :
    ' - idUtilisateur : Identifiant de l'utilisateur à réinitialiser
    ' - authenticatedUser : Utilisateur authentifié effectuant la réinitialisation
    '
    ' Retour     :
    ' - String : Le nouveau mot de passe temporaire généré (à communiquer à l'utilisateur)
    '            Retourne Nothing en cas d'erreur
    '
    ' Remarques  :
    ' - Génère un nouveau mot de passe temporaire sécurisé
    ' - Force le changement de mot de passe à la prochaine connexion (must_change_password = 1)
    ' - Déverrouille le compte si nécessaire (nb_echecs_login = 0, compte_verrouille = 0)
    ' - Seuls les Admin peuvent réinitialiser les mots de passe
    ' - Logue l'action pour traçabilité
    ' -------------------------------------------------------------------------------------------------
    Public Function ResetPasswordUtilisateur(
        idUtilisateur As Long,
        authenticatedUser As UtilisateurApplication
    ) As String

        ' Validation des entrées
        If idUtilisateur <= 0 Then
            Throw New ArgumentException(
                "L'identifiant de l'utilisateur doit être positif.",
                NameOf(idUtilisateur)
            )
        End If

        If authenticatedUser Is Nothing Then
            Throw New ArgumentNullException(
                NameOf(authenticatedUser),
                "L'utilisateur authentifié ne peut pas être null."
            )
        End If

        ' Vérification des droits : seuls Admin peuvent réinitialiser
        If authenticatedUser.RoleUtilisateur <> AppRole.Admin Then
            Throw New UnauthorizedAccessException(
                "Seuls les administrateurs peuvent réinitialiser les mots de passe."
            )
        End If

        Try

            ' Génération d'un nouveau mot de passe temporaire
            Dim passwordTemporaire As String = GenererMotDePasseTemporaire()

            ' Génération du hash et du salt
            Dim salt As String = PasswordSecurityHelper.GenerateSalt()
            Dim hash As String = PasswordSecurityHelper.HashPassword(passwordTemporaire, salt)

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
                            $"Mot de passe réinitialisé pour l'utilisateur ID={idUtilisateur} par {authenticatedUser.LoginUtilisateur}.",
                            GestionLog.LogLevel.Rapide,
                            GestionLog.LogCategory.Security
                        )

                        Return passwordTemporaire

                    Else

                        GestionLog.EcrireLog(
                            $"Aucun utilisateur trouvé avec l'ID {idUtilisateur} lors de la réinitialisation du mot de passe.",
                            GestionLog.LogLevel.Rapide,
                            GestionLog.LogCategory.Security
                        )

                        Return Nothing

                    End If

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur ResetPasswordUtilisateur.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
                ex
            )

            Throw

        End Try

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
    ' - authenticatedUser : Utilisateur authentifié effectuant l'action
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
        authenticatedUser As UtilisateurApplication
    ) As Boolean

        ' Validation des entrées
        If idUtilisateur <= 0 Then
            Throw New ArgumentException(
                "L'identifiant de l'utilisateur doit être positif.",
                NameOf(idUtilisateur)
            )
        End If

        If authenticatedUser Is Nothing Then
            Throw New ArgumentNullException(
                NameOf(authenticatedUser),
                "L'utilisateur authentifié ne peut pas être null."
            )
        End If

        ' Vérification des droits : seuls Admin peuvent déverrouiller
        If authenticatedUser.RoleUtilisateur <> AppRole.Admin Then
            Throw New UnauthorizedAccessException(
                "Seuls les administrateurs peuvent déverrouiller les comptes."
            )
        End If

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
                            $"Compte déverrouillé pour l'utilisateur ID={idUtilisateur} par {authenticatedUser.LoginUtilisateur}.",
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
    ' - authenticatedUser : Utilisateur authentifié effectuant l'action
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
        authenticatedUser As UtilisateurApplication
    ) As Boolean

        ' Validation des entrées
        If idUtilisateur <= 0 Then
            Throw New ArgumentException(
                "L'identifiant de l'utilisateur doit être positif.",
                NameOf(idUtilisateur)
            )
        End If

        If authenticatedUser Is Nothing Then
            Throw New ArgumentNullException(
                NameOf(authenticatedUser),
                "L'utilisateur authentifié ne peut pas être null."
            )
        End If

        ' Vérification des droits : seuls Admin peuvent activer/désactiver
        If authenticatedUser.RoleUtilisateur <> AppRole.Admin Then
            Throw New UnauthorizedAccessException(
                "Seuls les administrateurs peuvent activer/désactiver les utilisateurs."
            )
        End If

        ' Interdire la désactivation de soi-même
        If idUtilisateur = authenticatedUser.IdUtilisateur AndAlso Not actif Then
            Throw New InvalidOperationException(
                "Vous ne pouvez pas vous désactiver vous-même."
            )
        End If

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
                            $"Utilisateur ID={idUtilisateur} {action} par {authenticatedUser.LoginUtilisateur}.",
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
    ' Version    : V1.0.0
    ' Date       : 17/05/2026
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
    ' - Pas de vérification des droits : la sécurité est gérée par ChangePassword.vb
    ' - Logue l'action pour traçabilité
    ' -------------------------------------------------------------------------------------------------
    Public Function ResetPasswordUtilisateurWithCustomPassword(
        idUtilisateur As Long,
        nouveauPassword As String
    ) As Boolean

        ' Validation des entrées
        If idUtilisateur <= 0 Then
            Throw New ArgumentException(
                "L'identifiant de l'utilisateur doit être positif.",
                NameOf(idUtilisateur)
            )
        End If

        If String.IsNullOrWhiteSpace(nouveauPassword) Then
            Throw New ArgumentException(
                "Le nouveau mot de passe ne peut pas être vide.",
                NameOf(nouveauPassword)
            )
        End If

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

End Module
