' -------------------------------------------------------------------------------------------------
' Module   : GestionTypesDocuments
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 09/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les types de document (lecture depuis la base, écriture, vérifications),
' à partir de la table référentielle ref_types_documents.
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB
' - Utilise QueryTypesDocuments pour les requêtes SQL
' - Retourne des objets TypeDocument
' - Suppression physique réservée aux types non utilisés ; sinon soft-delete (Actif = 0)
' -------------------------------------------------------------------------------------------------
Imports MySqlConnector

Public Module GestionTypesDocuments

#Region "Lecture"

    Public Function GetTypesDocumentsActifs() As List(Of TypeDocument)

        Return GetTypesDocumentsDepuisRequete(QueryTypesDocuments.SelectTypesDocumentsActifs)

    End Function

    Public Function GetTypesDocuments(afficherInactifs As Boolean) As List(Of TypeDocument)

        If afficherInactifs Then
            Return GetTypesDocumentsDepuisRequete(QueryTypesDocuments.SelectTypesDocumentsTous)
        End If

        Return GetTypesDocumentsActifs()

    End Function

    Private Function GetTypesDocumentsDepuisRequete(query As String) As List(Of TypeDocument)

        Dim result As New List(Of TypeDocument)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()

                    While reader.Read()

                        Dim typeDoc As New TypeDocument With {
                            .IdTypeDocument = reader.GetInt64("id_type_document"),
                            .CodeTypeDocument = reader.GetString("code_type_document"),
                            .LibelleTypeDocument = reader.GetString("libelle_type_document"),
                            .Actif = reader.GetBoolean("actif"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage")
                        }

                        result.Add(typeDoc)

                    End While

                End Using
            End Using
        End Using

        Return result

    End Function

#End Region

#Region "Ecriture"

    Public Sub UpdateTypeDocument(typeDoc As TypeDocument)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryTypesDocuments.UpdateTypeDocument, conn)

                cmd.Parameters.AddWithValue("@id", typeDoc.IdTypeDocument)
                cmd.Parameters.AddWithValue("@code", typeDoc.CodeTypeDocument)
                cmd.Parameters.AddWithValue("@libelle", typeDoc.LibelleTypeDocument)
                cmd.Parameters.AddWithValue("@actif", typeDoc.Actif)
                cmd.Parameters.AddWithValue("@ordre", typeDoc.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    Public Sub InsertTypeDocument(typeDoc As TypeDocument)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryTypesDocuments.InsertTypeDocument, conn)

                cmd.Parameters.AddWithValue("@code", typeDoc.CodeTypeDocument)
                cmd.Parameters.AddWithValue("@libelle", typeDoc.LibelleTypeDocument)
                cmd.Parameters.AddWithValue("@actif", typeDoc.Actif)
                cmd.Parameters.AddWithValue("@ordre", typeDoc.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    Public Sub DesactiverTypeDocument(idTypeDocument As ULong)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryTypesDocuments.DesactiverTypeDocument, conn)
                cmd.Parameters.AddWithValue("@id", idTypeDocument)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Sub SupprimerTypeDocument(idTypeDocument As ULong)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryTypesDocuments.SupprimerTypeDocument, conn)
                cmd.Parameters.AddWithValue("@id", idTypeDocument)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    Public Function CodeTypeDocumentExiste(code As String,
                                           Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryTypesDocuments.SelectCountCodeTypeDocument, conn)

                cmd.Parameters.AddWithValue("@code", code)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    Public Function LibelleTypeDocumentExiste(libelle As String,
                                              Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryTypesDocuments.SelectCountLibelleTypeDocument, conn)

                cmd.Parameters.AddWithValue("@libelle", libelle)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    Public Function TypeDocumentEstUtilise(idTypeDocument As ULong) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryTypesDocuments.SelectCountUsageTypeDocument, conn)

                cmd.Parameters.AddWithValue("@id", idTypeDocument)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

#End Region

End Module
