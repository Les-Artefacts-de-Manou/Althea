' -------------------------------------------------------------------------------------------------
' Module   : GestionRolesIntervenant
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 09/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les rôles d'intervenant (lecture depuis la base, écriture, vérifications),
' à partir de la table référentielle ref_roles_intervenant.
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB
' - Utilise QueryRolesIntervenant pour les requêtes SQL
' - Retourne des objets RoleIntervenant
' - Suppression physique réservée aux rôles non utilisés ; sinon soft-delete (Actif = 0)
' -------------------------------------------------------------------------------------------------
Imports MySqlConnector

Public Module GestionRolesIntervenant

#Region "Lecture"

    Public Function GetRolesIntervenantActifs() As List(Of RoleIntervenant)

        Return GetRolesIntervenantDepuisRequete(QueryRolesIntervenant.SelectRolesIntervenantActifs)

    End Function

    Public Function GetRolesIntervenant(afficherInactifs As Boolean) As List(Of RoleIntervenant)

        If afficherInactifs Then
            Return GetRolesIntervenantDepuisRequete(QueryRolesIntervenant.SelectRolesIntervenantTous)
        End If

        Return GetRolesIntervenantActifs()

    End Function

    Private Function GetRolesIntervenantDepuisRequete(query As String) As List(Of RoleIntervenant)

        Dim result As New List(Of RoleIntervenant)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()

                    While reader.Read()

                        Dim role As New RoleIntervenant With {
                            .IdRoleIntervenant = reader.GetInt64("id_role_intervenant"),
                            .CodeRoleIntervenant = reader.GetString("code_role_intervenant"),
                            .LibelleRoleIntervenant = reader.GetString("libelle_role_intervenant"),
                            .Actif = reader.GetBoolean("actif"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage")
                        }

                        result.Add(role)

                    End While

                End Using
            End Using
        End Using

        Return result

    End Function

#End Region

#Region "Ecriture"

    Public Sub UpdateRoleIntervenant(role As RoleIntervenant)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryRolesIntervenant.UpdateRoleIntervenant, conn)

                cmd.Parameters.AddWithValue("@id", role.IdRoleIntervenant)
                cmd.Parameters.AddWithValue("@code", role.CodeRoleIntervenant)
                cmd.Parameters.AddWithValue("@libelle", role.LibelleRoleIntervenant)
                cmd.Parameters.AddWithValue("@actif", role.Actif)
                cmd.Parameters.AddWithValue("@ordre", role.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    Public Sub InsertRoleIntervenant(role As RoleIntervenant)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryRolesIntervenant.InsertRoleIntervenant, conn)

                cmd.Parameters.AddWithValue("@code", role.CodeRoleIntervenant)
                cmd.Parameters.AddWithValue("@libelle", role.LibelleRoleIntervenant)
                cmd.Parameters.AddWithValue("@actif", role.Actif)
                cmd.Parameters.AddWithValue("@ordre", role.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    Public Sub DesactiverRoleIntervenant(idRoleIntervenant As ULong)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryRolesIntervenant.DesactiverRoleIntervenant, conn)
                cmd.Parameters.AddWithValue("@id", idRoleIntervenant)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Sub SupprimerRoleIntervenant(idRoleIntervenant As ULong)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryRolesIntervenant.SupprimerRoleIntervenant, conn)
                cmd.Parameters.AddWithValue("@id", idRoleIntervenant)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    Public Function CodeRoleIntervenantExiste(code As String,
                                              Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryRolesIntervenant.SelectCountCodeRoleIntervenant, conn)

                cmd.Parameters.AddWithValue("@code", code)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    Public Function LibelleRoleIntervenantExiste(libelle As String,
                                                 Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryRolesIntervenant.SelectCountLibelleRoleIntervenant, conn)

                cmd.Parameters.AddWithValue("@libelle", libelle)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    Public Function RoleIntervenantEstUtilise(idRoleIntervenant As ULong) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryRolesIntervenant.SelectCountUsageRoleIntervenant, conn)

                cmd.Parameters.AddWithValue("@id", idRoleIntervenant)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

#End Region

End Module
