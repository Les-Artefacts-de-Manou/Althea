' -------------------------------------------------------------------------------------------------
' Module   : GestionStatutsDossier
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 09/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les statuts de dossier (lecture depuis la base, écriture, vérifications),
' à partir de la table référentielle ref_statuts_dossier.
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB
' - Utilise QueryStatutsDossier pour les requêtes SQL
' - Retourne des objets StatutDossier
' - Suppression physique réservée aux statuts non utilisés ; sinon soft-delete (Actif = 0)
' -------------------------------------------------------------------------------------------------
Imports MySqlConnector

Public Module GestionStatutsDossier

#Region "Lecture"

    Public Function GetStatutsDossierActifs() As List(Of StatutDossier)

        Return GetStatutsDossierDepuisRequete(QueryStatutsDossier.SelectStatutsDossierActifs)

    End Function

    Public Function GetStatutsDossier(afficherInactifs As Boolean) As List(Of StatutDossier)

        If afficherInactifs Then
            Return GetStatutsDossierDepuisRequete(QueryStatutsDossier.SelectStatutsDossierTous)
        End If

        Return GetStatutsDossierActifs()

    End Function

    Private Function GetStatutsDossierDepuisRequete(query As String) As List(Of StatutDossier)

        Dim result As New List(Of StatutDossier)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()

                    While reader.Read()

                        Dim statut As New StatutDossier With {
                            .IdStatutDossier = reader.GetInt64("id_statut_dossier"),
                            .CodeStatutDossier = reader.GetString("code_statut_dossier"),
                            .LibelleStatutDossier = reader.GetString("libelle_statut_dossier"),
                            .Actif = reader.GetBoolean("actif"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage")
                        }

                        result.Add(statut)

                    End While

                End Using
            End Using
        End Using

        Return result

    End Function

#End Region

#Region "Ecriture"

    Public Sub UpdateStatutDossier(statut As StatutDossier)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryStatutsDossier.UpdateStatutDossier, conn)

                cmd.Parameters.AddWithValue("@id", statut.IdStatutDossier)
                cmd.Parameters.AddWithValue("@code", statut.CodeStatutDossier)
                cmd.Parameters.AddWithValue("@libelle", statut.LibelleStatutDossier)
                cmd.Parameters.AddWithValue("@actif", statut.Actif)
                cmd.Parameters.AddWithValue("@ordre", statut.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    Public Sub InsertStatutDossier(statut As StatutDossier)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryStatutsDossier.InsertStatutDossier, conn)

                cmd.Parameters.AddWithValue("@code", statut.CodeStatutDossier)
                cmd.Parameters.AddWithValue("@libelle", statut.LibelleStatutDossier)
                cmd.Parameters.AddWithValue("@actif", statut.Actif)
                cmd.Parameters.AddWithValue("@ordre", statut.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    Public Sub DesactiverStatutDossier(idStatutDossier As ULong)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryStatutsDossier.DesactiverStatutDossier, conn)
                cmd.Parameters.AddWithValue("@id", idStatutDossier)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Sub SupprimerStatutDossier(idStatutDossier As ULong)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryStatutsDossier.SupprimerStatutDossier, conn)
                cmd.Parameters.AddWithValue("@id", idStatutDossier)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    Public Function CodeStatutDossierExiste(code As String,
                                            Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryStatutsDossier.SelectCountCodeStatutDossier, conn)

                cmd.Parameters.AddWithValue("@code", code)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    Public Function LibelleStatutDossierExiste(libelle As String,
                                               Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryStatutsDossier.SelectCountLibelleStatutDossier, conn)

                cmd.Parameters.AddWithValue("@libelle", libelle)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    Public Function StatutDossierEstUtilise(idStatutDossier As ULong) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryStatutsDossier.SelectCountUsageStatutDossier, conn)

                cmd.Parameters.AddWithValue("@id", idStatutDossier)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

#End Region

End Module
