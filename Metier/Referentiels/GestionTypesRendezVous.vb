' -------------------------------------------------------------------------------------------------
' Module   : GestionTypesRendezVous
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 09/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les types de rendez-vous (lecture depuis la base, écriture, vérifications),
' à partir de la table référentielle ref_types_rendez_vous.
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB
' - Utilise QueryTypesRendezVous pour les requêtes SQL
' - Retourne des objets TypeRendezVous
' - Suppression physique réservée aux types non utilisés ; sinon soft-delete (Actif = 0)
' -------------------------------------------------------------------------------------------------
Imports MySqlConnector

Public Module GestionTypesRendezVous

#Region "Lecture"

    Public Function GetTypesRendezVousActifs() As List(Of TypeRendezVous)

        Return GetTypesRendezVousDepuisRequete(QueryTypesRendezVous.SelectTypesRendezVousActifs)

    End Function

    Public Function GetTypesRendezVous(afficherInactifs As Boolean) As List(Of TypeRendezVous)

        If afficherInactifs Then
            Return GetTypesRendezVousDepuisRequete(QueryTypesRendezVous.SelectTypesRendezVousTous)
        End If

        Return GetTypesRendezVousActifs()

    End Function

    Private Function GetTypesRendezVousDepuisRequete(query As String) As List(Of TypeRendezVous)

        Dim result As New List(Of TypeRendezVous)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()

                    While reader.Read()

                        Dim typeRdv As New TypeRendezVous With {
                            .IdTypeRendezVous = reader.GetInt64("id_type_rendez_vous"),
                            .CodeTypeRendezVous = reader.GetString("code_type_rendez_vous"),
                            .LibelleTypeRendezVous = reader.GetString("libelle_type_rendez_vous"),
                            .Actif = reader.GetBoolean("actif"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage")
                        }

                        result.Add(typeRdv)

                    End While

                End Using
            End Using
        End Using

        Return result

    End Function

#End Region

#Region "Ecriture"

    Public Sub UpdateTypeRendezVous(typeRdv As TypeRendezVous)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryTypesRendezVous.UpdateTypeRendezVous, conn)

                cmd.Parameters.AddWithValue("@id", typeRdv.IdTypeRendezVous)
                cmd.Parameters.AddWithValue("@code", typeRdv.CodeTypeRendezVous)
                cmd.Parameters.AddWithValue("@libelle", typeRdv.LibelleTypeRendezVous)
                cmd.Parameters.AddWithValue("@actif", typeRdv.Actif)
                cmd.Parameters.AddWithValue("@ordre", typeRdv.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    Public Sub InsertTypeRendezVous(typeRdv As TypeRendezVous)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryTypesRendezVous.InsertTypeRendezVous, conn)

                cmd.Parameters.AddWithValue("@code", typeRdv.CodeTypeRendezVous)
                cmd.Parameters.AddWithValue("@libelle", typeRdv.LibelleTypeRendezVous)
                cmd.Parameters.AddWithValue("@actif", typeRdv.Actif)
                cmd.Parameters.AddWithValue("@ordre", typeRdv.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    Public Sub DesactiverTypeRendezVous(idTypeRendezVous As ULong)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryTypesRendezVous.DesactiverTypeRendezVous, conn)
                cmd.Parameters.AddWithValue("@id", idTypeRendezVous)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Sub SupprimerTypeRendezVous(idTypeRendezVous As ULong)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryTypesRendezVous.SupprimerTypeRendezVous, conn)
                cmd.Parameters.AddWithValue("@id", idTypeRendezVous)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    Public Function CodeTypeRendezVousExiste(code As String,
                                             Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryTypesRendezVous.SelectCountCodeTypeRendezVous, conn)

                cmd.Parameters.AddWithValue("@code", code)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    Public Function LibelleTypeRendezVousExiste(libelle As String,
                                                Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryTypesRendezVous.SelectCountLibelleTypeRendezVous, conn)

                cmd.Parameters.AddWithValue("@libelle", libelle)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    Public Function TypeRendezVousEstUtilise(idTypeRendezVous As ULong) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryTypesRendezVous.SelectCountUsageTypeRendezVous, conn)

                cmd.Parameters.AddWithValue("@id", idTypeRendezVous)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

#End Region

End Module
