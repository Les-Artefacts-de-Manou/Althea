' -------------------------------------------------------------------------------------------------
' Module      :  GestionAuthentification
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 20/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Gestion de l'authentification des utilisateurs applicatifs.
'
' Responsabilités :
' - Authentifier les utilisateurs en vérifiant leurs identifiants
' - Gérer les tentatives d'authentification
' - Gérer les comptes verrouillés
'
' Important   :
' - Aucun accès UI direct (pas de MessageBox)
' - Aucun mot de passe en clair dans les logs
' - Retour de messages UI contrôlés via AuthenticationResult
' - Limite max 5 échecs login avant verrouillage
'
' Architecture :
' UI → GestionAuthentification → QueryUtilisateurs → DatabaseManager → MariaDB
'
' Dépendances :
' - DatabaseManager (connexions DB)
' - QueryUtilisateurs (requêtes SQL)
' - PasswordSecurityHelper (hash/vérification PBKDF2)
' - GestionLog (traçabilité)
' - UtilisateurApplication (objet métier)

' -------------------------------------------------------------------------------------------------

Imports MySqlConnector

Public Module GestionAuthentification

#Region "Authentification"

    ' -------------------------------------------------------------------------------------------------
    '    ' Fonction   : AuthentifierUtilisateur
    '    ' Version    : V1.3.0
    '    ' Date       : 20/05/2026
    '    '
    '    ' Rôle       :
    '    ' Vérifie les identifiants utilisateur et retourne un résultat d'authentification complet.
    '    '
    '    ' Paramètres :
    '    ' - login    : Login de l'utilisateur
    '    ' - password : Mot de passe en clair (vérifié via PBKDF2)
    '    '
    '    ' Retour     :
    '    ' - AuthenticationResult : Objet contenant le résultat (Success, Utilisateur, MessageUI, RemainingAttempts, IsLocked)
    '    '
    '    ' Sécurité   :
    '    ' - Refuse les comptes inactifs
    '    ' - Refuse les comptes verrouillés
    '    ' - Incrémente les échecs login après mot de passe invalide
    '    ' - Verrouille le compte après 5 échecs
    '    ' - Réinitialise les échecs après succès
    '    '
    '    ' Important  :
    '    ' - Aucun détail sensible exposé à l'UI (message générique)
    '    ' - Aucun mot de passe loggué
    '    '
    '    ' Utilisé par :
    '    ' - Login (formulaire de connexion)
    '    ' -------------------------------------------------------------------------------------------------

    Public Function AuthentifierUtilisateur(
        login As String,
        password As String
    ) As AuthenticationResult

        Const MaxEchecsLogin As Integer = 5

        Dim result As New AuthenticationResult() With {
            .Success = False,
            .Utilisateur = Nothing,
            .MessageUI = "Identifiants invalides.",
            .RemainingAttempts = 0,
            .IsLocked = False
        }

        Try
            If String.IsNullOrWhiteSpace(login) OrElse
               String.IsNullOrWhiteSpace(password) Then

                Return result
            End If

            Dim normalizedLogin As String = login.Trim()

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(
                    QueryUtilisateurs.GetUtilisateurByLogin(),
                    conn
                )

                    cmd.Parameters.AddWithValue("@login_utilisateur", normalizedLogin)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        If Not reader.Read() Then
                            GestionLog.EcrireLog(
                                $"Authentification refusée : utilisateur introuvable ({normalizedLogin}).",
                                GestionLog.LogLevel.Succinct,
                                GestionLog.LogCategory.Security
                            )

                            Return result
                        End If

                        Dim utilisateur As UtilisateurApplication =
                            MapUtilisateurPourAuthentification(reader)

                        If Not utilisateur.Actif Then
                            GestionLog.EcrireLog(
                                $"Authentification refusée : utilisateur inactif ({normalizedLogin}).",
                                GestionLog.LogLevel.Succinct,
                                GestionLog.LogCategory.Security
                            )

                            Return result
                        End If

                        If utilisateur.CompteVerrouille Then
                            GestionLog.EcrireLog(
                                $"Authentification refusée : compte verrouillé ({normalizedLogin}).",
                                GestionLog.LogLevel.Succinct,
                                GestionLog.LogCategory.Security
                            )

                            result.IsLocked = True
                            result.MessageUI =
                                "Compte verrouillé." &
                                Environment.NewLine &
                                "Veuillez contacter l'administrateur."

                            Return result
                        End If

                        Dim passwordValid As Boolean =
                            PasswordSecurityHelper.VerifyPassword(
                                password,
                                utilisateur.PasswordHash,
                                utilisateur.PasswordSalt
                            )

                        If Not passwordValid Then

                            IncrementerNbEchecsLogin(utilisateur)

                            GestionLog.EcrireLog(
                                $"Authentification refusée : mot de passe invalide ({normalizedLogin}). Échec {utilisateur.NbEchecsLogin}/{MaxEchecsLogin}.",
                                GestionLog.LogLevel.Succinct,
                                GestionLog.LogCategory.Security
                            )

                            Dim remainingAttempts As Integer =
                                MaxEchecsLogin - utilisateur.NbEchecsLogin

                            result.RemainingAttempts = Math.Max(remainingAttempts, 0)

                            If utilisateur.NbEchecsLogin >= MaxEchecsLogin Then

                                VerrouillerCompteUtilisateur(utilisateur)

                                result.IsLocked = True
                                result.MessageUI =
                                    "Compte verrouillé." &
                                    Environment.NewLine &
                                    "Veuillez contacter l'administrateur."

                            ElseIf remainingAttempts = 1 Then

                                result.MessageUI =
                                    "Mot de passe incorrect." &
                                    Environment.NewLine &
                                    "Dernière tentative avant verrouillage."

                            Else

                                result.MessageUI =
                                    "Mot de passe incorrect." &
                                    Environment.NewLine &
                                    $"Il reste {remainingAttempts} tentative(s)."

                            End If

                            Return result
                        End If

                        If utilisateur.NbEchecsLogin > 0 Then
                            ReinitialiserNbEchecsLogin(utilisateur)
                        End If

                        MettreAJourDernierLogin(utilisateur)

                        GestionLog.EcrireLog(
                            $"Authentification réussie : {normalizedLogin}.",
                            GestionLog.LogLevel.Rapide,
                            GestionLog.LogCategory.Security
                        )

                        result.Success = True
                        result.Utilisateur = utilisateur
                        result.MessageUI = String.Empty
                        result.RemainingAttempts = MaxEchecsLogin
                        result.IsLocked = False

                        Return result

                    End Using
                End Using
            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur AuthentifierUtilisateur.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Security,
                ex
            )

            Throw
        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    '  Procédure : MettreAJourDernierLogin
    ' Version    : 1.0.0
    ' Date       : 20/05/2026
    '
    ' Rôle       :
    ' Met à jour la date du dernier login réussi pour un utilisateur donné.
    '
    ' Paramètres :
    ' - utilisateur : L'utilisateur pour lequel mettre à jour le dernier login
    '
    ' Remarques  :
    ' - Met à jour la base de données et l'objet utilisateur en mémoire
    ' -------------------------------------------------------------------------------------------------
    Private Sub MettreAJourDernierLogin(
    utilisateur As UtilisateurApplication
)

        Using conn As MySqlConnection = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(
            QueryUtilisateurs.UpdateDernierLoginUtilisateur,
            conn
        )

                cmd.Parameters.AddWithValue(
                "@id_utilisateur",
                utilisateur.IdUtilisateur
            )

                cmd.ExecuteNonQuery()

            End Using

        End Using

        utilisateur.DernierLogin = DateTime.Now

    End Sub

#End Region

#Region "Gestion échecs login"

    ' -------------------------------------------------------------------------------------------------
    '  Procédure : IncrementerNb
    ' Version    : 1.0.0
    ' Date       : 20/05/2026
    '
    ' Rôle       :
    ' Incrémente le nombre d'échecs de login pour un utilisateur donné.
    '
    ' Paramètres :
    ' - utilisateur : L'utilisateur pour lequel incrémenter les échecs de login
    '
    ' Remarques  :
    ' - Doit être appelé après une tentative d'authentification infructueuse
    ' - Met à jour la base de données et l'objet utilisateur en mémoire
    ' -------------------------------------------------------------------------------------------------
    '    
    Private Sub IncrementerNbEchecsLogin(
        utilisateur As UtilisateurApplication
    )

        Using conn As MySqlConnection = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(
                QueryUtilisateurs.IncrementerNbEchecsLogin(),
                conn
            )

                cmd.Parameters.AddWithValue(
                    "@id_utilisateur",
                    utilisateur.IdUtilisateur
                )

                cmd.ExecuteNonQuery()

            End Using
        End Using

        utilisateur.NbEchecsLogin += 1

    End Sub

    ' -------------------------------------------------------------------------------------------------
    '  Procédure : VerrouillerCompteUtilisateur
    ' Version    : 1.0.0
    ' Date       : 20/05/2026
    '
    ' Rôle       :
    ' Verrouille le compte d'un utilisateur donné.
    '
    ' Paramètres :
    ' - utilisateur : L'utilisateur dont le compte doit être verrouillé
    '
    ' Remarques  :
    ' - Met à jour la base de données et l'objet utilisateur en mémoire
    ' -------------------------------------------------------------------------------------------------
    Private Sub VerrouillerCompteUtilisateur(
        utilisateur As UtilisateurApplication
    )

        Using conn As MySqlConnection = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(
                QueryUtilisateurs.VerrouillerCompteUtilisateur(),
                conn
            )

                cmd.Parameters.AddWithValue(
                    "@id_utilisateur",
                    utilisateur.IdUtilisateur
                )

                cmd.ExecuteNonQuery()

            End Using
        End Using

        utilisateur.CompteVerrouille = True
        utilisateur.DateVerrouillage = DateTime.Now

        GestionLog.EcrireLog(
            $"Compte verrouillé ({utilisateur.LoginUtilisateur}).",
            GestionLog.LogLevel.Succinct,
            GestionLog.LogCategory.Security
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    '  Procédure : ReinitialiserNbEchecsLogin
    ' Version    : 1.0.0
    ' Date       : 20/05/2026
    '
    ' Rôle       :
    ' Résinitialise le nombre d'échecs de login d'un utilisateur donné.
    '
    ' Paramètres :
    ' - utilisateur : L'utilisateur dont le nombre d'échecs de login doit быть résinitialisé
    '
    ' Remarques  :
    ' - Met à jour la base de données et l'objet utilisateur en mémoire
    ' -------------------------------------------------------------------------------------------------
    Private Sub ReinitialiserNbEchecsLogin(
        utilisateur As UtilisateurApplication
    )

        Using conn As MySqlConnection = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(
                QueryUtilisateurs.ReinitialiserNbEchecsLogin(),
                conn
            )

                cmd.Parameters.AddWithValue(
                    "@id_utilisateur",
                    utilisateur.IdUtilisateur
                )

                cmd.ExecuteNonQuery()

            End Using
        End Using

        utilisateur.NbEchecsLogin = 0

    End Sub

#End Region

#Region "Mapping authentification"

    ' -------------------------------------------------------------------------------------------------
    '  Procédure : MapUtilisateurPourAuthentification
    ' Version    : 1.0.0
    ' Date       : 20/05/2026
    '
    ' Rôle       :
    ' Map un utilisateur de la base de données vers un objet utilisateur pour authentification.
    '
    ' Paramètres :
    ' - reader : L'objet MySqlDataReader contenant les données de l'utilisateur
    '
    ' Retour     :
    ' - Un objet UtilisateurApplication contenant les informations de l'utilisateur
    '
    ' Remarques  :
    ' - Doit être utilisé uniquement pour le mapping lors de l'authentification (inclut les champs de sécurité)
    ' -------------------------------------------------------------------------------------------------
    Private Function MapUtilisateurPourAuthentification(
        reader As MySqlDataReader
    ) As UtilisateurApplication

        Dim utilisateur As New UtilisateurApplication

        utilisateur.IdUtilisateur = Convert.ToInt64(reader("id_utilisateur"))
        utilisateur.CodeUtilisateur = reader("code_utilisateur").ToString()
        utilisateur.LoginUtilisateur = reader("login_utilisateur").ToString()
        utilisateur.NomAffichage = reader("nom_affichage").ToString()

        utilisateur.PasswordHash = reader("password_hash").ToString()
        utilisateur.PasswordSalt = reader("password_salt").ToString()

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

        utilisateur.Actif = Convert.ToBoolean(reader("actif"))
        utilisateur.MustChangePassword = Convert.ToBoolean(reader("must_change_password"))
        utilisateur.NbEchecsLogin = Convert.ToInt32(reader("nb_echecs_login"))
        utilisateur.CompteVerrouille = Convert.ToBoolean(reader("compte_verrouille"))

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

#End Region

End Module