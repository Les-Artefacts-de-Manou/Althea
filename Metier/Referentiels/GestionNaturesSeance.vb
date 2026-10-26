' -------------------------------------------------------------------------------------------------
' Module   : GestionNaturesSeance
' Projet   : Althéa
' Version  : V1.1.0
' Date     : 22/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les natures de séance (lecture depuis la base, écriture, vérifications), à partir de la
' table référentielle ref_natures_seance.
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB
' - Utilise QueryNaturesSeance pour les requêtes SQL
' - Retourne des objets NatureSeance
' - Suppression physique réservée aux natures non utilisées ; sinon soft-delete (Actif = 0)
' - PARTICULARITÉ : la nature de séance n'a AUCUNE incidence sur le prix
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : chaque écriture reçoit le ContexteOperation et
'   commence par ReglesReferentiels.ExigerDroitEcriture (SuperUser) ; SupprimerNatureSeance, plus
'   appelée, supprimée.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionNaturesSeance

#Region "Lecture"

    Public Function GetNaturesSeanceActives() As List(Of NatureSeance)

        Return GetNaturesSeanceDepuisRequete(QueryNaturesSeance.SelectNaturesSeanceActives)

    End Function

    Public Function GetNaturesSeance(afficherInactifs As Boolean) As List(Of NatureSeance)

        If afficherInactifs Then
            Return GetNaturesSeanceDepuisRequete(QueryNaturesSeance.SelectNaturesSeanceToutes)
        End If

        Return GetNaturesSeanceActives()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : GetNaturesSeanceParDomaine
    ' Version   : V1.0.0
    ' Date      : 19/09/2026
    '
    ' Rôle      :
    ' Natures de séance actives rattachées à un domaine (lia_natures_seance_domaines).
    '
    ' Paramètres :
    ' - idDomaine : Domaine du dossier
    '
    ' Retour    :
    ' - List(Of NatureSeance) : natures rattachées, liste vide si aucun rattachement
    '
    ' Remarques :
    ' - Remplace le filtrage par nature « INAMI ou non », qui ne distinguait pas deux domaines
    '   conventionnés. L'appelant décide du repli si la liste est vide.
    ' -------------------------------------------------------------------------------------------------
    Public Function GetNaturesSeanceParDomaine(idDomaine As Long) As List(Of NatureSeance)

        Dim parametres As New Dictionary(Of String, Object) From {
            {"@id_domaine", idDomaine}
        }

        Return GetNaturesSeanceDepuisRequete(QueryNaturesSeance.SelectNaturesSeanceParDomaine, parametres)

    End Function

    Private Function GetNaturesSeanceDepuisRequete(query As String) As List(Of NatureSeance)

        Return GetNaturesSeanceDepuisRequete(query, Nothing)

    End Function

    Private Function GetNaturesSeanceDepuisRequete(query As String,
                                                   parametres As Dictionary(Of String, Object)) As List(Of NatureSeance)

        Dim result As New List(Of NatureSeance)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)

                If parametres IsNot Nothing Then
                    For Each parametre In parametres
                        cmd.Parameters.AddWithValue(parametre.Key, parametre.Value)
                    Next
                End If

                Using reader = cmd.ExecuteReader()

                    While reader.Read()

                        Dim nature As New NatureSeance With {
                            .IdNatureSeance = reader.GetUInt64("id_nature_seance"),
                            .CodeNatureSeance = reader.GetString("code_nature_seance"),
                            .LibelleNatureSeance = reader.GetString("libelle_nature_seance"),
                            .Actif = reader.GetBoolean("actif"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage")
                        }

                        result.Add(nature)

                    End While

                End Using
            End Using
        End Using

        Return result

    End Function

#End Region

#Region "Ecriture"

    Public Sub UpdateNatureSeance(contexte As ContexteOperation, nature As NatureSeance)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryNaturesSeance.UpdateNatureSeance, conn)

                cmd.Parameters.AddWithValue("@id", nature.IdNatureSeance)
                cmd.Parameters.AddWithValue("@code", nature.CodeNatureSeance)
                cmd.Parameters.AddWithValue("@libelle", nature.LibelleNatureSeance)
                cmd.Parameters.AddWithValue("@actif", nature.Actif)
                cmd.Parameters.AddWithValue("@ordre", nature.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using
        End Using

    End Sub

    Public Sub InsertNatureSeance(contexte As ContexteOperation, nature As NatureSeance)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryNaturesSeance.InsertNatureSeance, conn)

                cmd.Parameters.AddWithValue("@code", nature.CodeNatureSeance)
                cmd.Parameters.AddWithValue("@libelle", nature.LibelleNatureSeance)
                cmd.Parameters.AddWithValue("@actif", nature.Actif)
                cmd.Parameters.AddWithValue("@ordre", nature.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using
        End Using

    End Sub

    Public Sub DesactiverNatureSeance(contexte As ContexteOperation, idNatureSeance As ULong)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryNaturesSeance.DesactiverNatureSeance, conn)
                cmd.Parameters.AddWithValue("@id", idNatureSeance)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    Public Function CodeNatureSeanceExiste(code As String,
                                           Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryNaturesSeance.SelectCountCodeNatureSeance, conn)

                cmd.Parameters.AddWithValue("@code", code)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Return Convert.ToInt32(cmd.ExecuteScalar()) > 0

            End Using
        End Using

    End Function

    Public Function LibelleNatureSeanceExiste(libelle As String,
                                              Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryNaturesSeance.SelectCountLibelleNatureSeance, conn)

                cmd.Parameters.AddWithValue("@libelle", libelle)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Return Convert.ToInt32(cmd.ExecuteScalar()) > 0

            End Using
        End Using

    End Function

    Public Function NatureSeanceEstUtilisee(idNatureSeance As ULong) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryNaturesSeance.SelectCountUsageNatureSeance, conn)

                cmd.Parameters.AddWithValue("@id", idNatureSeance)

                Return Convert.ToInt32(cmd.ExecuteScalar()) > 0

            End Using
        End Using

    End Function

#End Region

End Module
