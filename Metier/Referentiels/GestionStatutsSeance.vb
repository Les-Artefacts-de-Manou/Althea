' -------------------------------------------------------------------------------------------------
' Module   : GestionStatutsSeance
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 09/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les statuts de séance (lecture depuis la base, écriture, vérifications),
' à partir de la table référentielle ref_statuts_seance.
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB
' - Utilise QueryStatutsSeance pour les requêtes SQL
' - Retourne des objets StatutSeance
' - Suppression physique réservée aux statuts non utilisés ; sinon soft-delete (Actif = 0)
' -------------------------------------------------------------------------------------------------
Imports MySqlConnector

Public Module GestionStatutsSeance

#Region "Lecture"

    Public Function GetStatutsSeanceActifs() As List(Of StatutSeance)

        Return GetStatutsSeanceDepuisRequete(QueryStatutsSeance.SelectStatutsSeanceActifs)

    End Function

    Public Function GetStatutsSeance(afficherInactifs As Boolean) As List(Of StatutSeance)

        If afficherInactifs Then
            Return GetStatutsSeanceDepuisRequete(QueryStatutsSeance.SelectStatutsSeanceTous)
        End If

        Return GetStatutsSeanceActifs()

    End Function

    Private Function GetStatutsSeanceDepuisRequete(query As String) As List(Of StatutSeance)

        Dim result As New List(Of StatutSeance)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()

                    While reader.Read()

                        Dim statut As New StatutSeance With {
                            .IdStatutSeance = reader.GetInt64("id_statut_seance"),
                            .CodeStatutSeance = reader.GetString("code_statut_seance"),
                            .LibelleStatutSeance = reader.GetString("libelle_statut_seance"),
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

    Public Sub UpdateStatutSeance(statut As StatutSeance)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryStatutsSeance.UpdateStatutSeance, conn)

                cmd.Parameters.AddWithValue("@id", statut.IdStatutSeance)
                cmd.Parameters.AddWithValue("@code", statut.CodeStatutSeance)
                cmd.Parameters.AddWithValue("@libelle", statut.LibelleStatutSeance)
                cmd.Parameters.AddWithValue("@actif", statut.Actif)
                cmd.Parameters.AddWithValue("@ordre", statut.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    Public Sub InsertStatutSeance(statut As StatutSeance)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryStatutsSeance.InsertStatutSeance, conn)

                cmd.Parameters.AddWithValue("@code", statut.CodeStatutSeance)
                cmd.Parameters.AddWithValue("@libelle", statut.LibelleStatutSeance)
                cmd.Parameters.AddWithValue("@actif", statut.Actif)
                cmd.Parameters.AddWithValue("@ordre", statut.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    Public Sub DesactiverStatutSeance(idStatutSeance As ULong)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryStatutsSeance.DesactiverStatutSeance, conn)
                cmd.Parameters.AddWithValue("@id", idStatutSeance)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Sub SupprimerStatutSeance(idStatutSeance As ULong)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryStatutsSeance.SupprimerStatutSeance, conn)
                cmd.Parameters.AddWithValue("@id", idStatutSeance)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    Public Function CodeStatutSeanceExiste(code As String,
                                           Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryStatutsSeance.SelectCountCodeStatutSeance, conn)

                cmd.Parameters.AddWithValue("@code", code)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    Public Function LibelleStatutSeanceExiste(libelle As String,
                                              Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryStatutsSeance.SelectCountLibelleStatutSeance, conn)

                cmd.Parameters.AddWithValue("@libelle", libelle)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    Public Function StatutSeanceEstUtilise(idStatutSeance As ULong) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryStatutsSeance.SelectCountUsageStatutSeance, conn)

                cmd.Parameters.AddWithValue("@id", idStatutSeance)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

#End Region

End Module
