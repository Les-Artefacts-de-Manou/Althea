' -------------------------------------------------------------------------------------------------
' Module   : GestionSituationsFamiliales
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 09/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les situations familiales (lecture depuis la base, écriture, vérifications),
' à partir de la table référentielle ref_situations_familiales.
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB
' - Utilise QuerySituationsFamiliales pour les requêtes SQL
' - Retourne des objets SituationFamiliale
' - Suppression physique réservée aux situations non utilisées ; sinon soft-delete (Actif = 0)
' -------------------------------------------------------------------------------------------------
Imports MySqlConnector

Public Module GestionSituationsFamiliales

#Region "Lecture"

    Public Function GetSituationsFamilialesActives() As List(Of SituationFamiliale)

        Return GetSituationsFamilialesDepuisRequete(QuerySituationsFamiliales.SelectSituationsFamilialesActives)

    End Function

    Public Function GetSituationsFamiliales(afficherInactifs As Boolean) As List(Of SituationFamiliale)

        If afficherInactifs Then
            Return GetSituationsFamilialesDepuisRequete(QuerySituationsFamiliales.SelectSituationsFamilialesToutes)
        End If

        Return GetSituationsFamilialesActives()

    End Function

    Private Function GetSituationsFamilialesDepuisRequete(query As String) As List(Of SituationFamiliale)

        Dim result As New List(Of SituationFamiliale)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()

                    While reader.Read()

                        Dim situation As New SituationFamiliale With {
                            .IdSituationFamiliale = reader.GetInt64("id_situation_familiale"),
                            .CodeSituationFamiliale = reader.GetString("code_situation_familiale"),
                            .LibelleSituationFamiliale = reader.GetString("libelle_situation_familiale"),
                            .Actif = reader.GetBoolean("actif"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage")
                        }

                        result.Add(situation)

                    End While

                End Using
            End Using
        End Using

        Return result

    End Function

#End Region

#Region "Ecriture"

    Public Sub UpdateSituationFamiliale(situation As SituationFamiliale)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QuerySituationsFamiliales.UpdateSituationFamiliale, conn)

                cmd.Parameters.AddWithValue("@id", situation.IdSituationFamiliale)
                cmd.Parameters.AddWithValue("@code", situation.CodeSituationFamiliale)
                cmd.Parameters.AddWithValue("@libelle", situation.LibelleSituationFamiliale)
                cmd.Parameters.AddWithValue("@actif", situation.Actif)
                cmd.Parameters.AddWithValue("@ordre", situation.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    Public Sub InsertSituationFamiliale(situation As SituationFamiliale)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QuerySituationsFamiliales.InsertSituationFamiliale, conn)

                cmd.Parameters.AddWithValue("@code", situation.CodeSituationFamiliale)
                cmd.Parameters.AddWithValue("@libelle", situation.LibelleSituationFamiliale)
                cmd.Parameters.AddWithValue("@actif", situation.Actif)
                cmd.Parameters.AddWithValue("@ordre", situation.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    Public Sub DesactiverSituationFamiliale(idSituationFamiliale As ULong)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QuerySituationsFamiliales.DesactiverSituationFamiliale, conn)
                cmd.Parameters.AddWithValue("@id", idSituationFamiliale)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Sub SupprimerSituationFamiliale(idSituationFamiliale As ULong)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QuerySituationsFamiliales.SupprimerSituationFamiliale, conn)
                cmd.Parameters.AddWithValue("@id", idSituationFamiliale)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    Public Function CodeSituationFamilialeExiste(code As String,
                                                 Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QuerySituationsFamiliales.SelectCountCodeSituationFamiliale, conn)

                cmd.Parameters.AddWithValue("@code", code)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    Public Function LibelleSituationFamilialeExiste(libelle As String,
                                                    Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QuerySituationsFamiliales.SelectCountLibelleSituationFamiliale, conn)

                cmd.Parameters.AddWithValue("@libelle", libelle)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    Public Function SituationFamilialeEstUtilisee(idSituationFamiliale As ULong) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QuerySituationsFamiliales.SelectCountUsageSituationFamiliale, conn)

                cmd.Parameters.AddWithValue("@id", idSituationFamiliale)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

#End Region

End Module
