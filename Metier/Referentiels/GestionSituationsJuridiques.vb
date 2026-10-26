' -------------------------------------------------------------------------------------------------
' Module      : GestionSituationsJuridiques
' Projet      : Althéa
' Version     : V1.1.0
'
' Rôle        :
' Couche métier du référentiel des situations juridiques du cabinet (ref_situations_juridiques).
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : chaque écriture reçoit le ContexteOperation et
'   commence par ReglesReferentiels.ExigerDroitEcriture (SuperUser).
'   Lignes système : refus explicite si la requête n'a concerné aucune ligne (ExecuterSurLigneModifiable).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionSituationsJuridiques

#Region "Lecture"

    Public Function GetSituationsJuridiques(afficherInactifs As Boolean) As List(Of SituationJuridique)
        Return Lire(If(afficherInactifs,
                       QuerySituationsJuridiques.SelectSituationsJuridiquesToutes,
                       QuerySituationsJuridiques.SelectSituationsJuridiquesActives))
    End Function

    Private Function Lire(query As String) As List(Of SituationJuridique)

        Dim liste As New List(Of SituationJuridique)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        liste.Add(New SituationJuridique With {
                            .IdSituationJuridique = reader.GetUInt64("id_situation_juridique"),
                            .CodeSituationJuridique = reader.GetString("code_situation_juridique"),
                            .LibelleSituationJuridique = reader.GetString("libelle_situation_juridique"),
                            .PorteurBce = reader.GetString("porteur_bce"),
                            .Actif = reader.GetBoolean("actif"),
                            .EstSysteme = UtilsDataReader.LireBooleenOptionnel(reader, "est_systeme"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage")
                        })
                    End While
                End Using
            End Using
        End Using

        Return liste

    End Function

#End Region

#Region "Ecriture"

    Public Sub InsertSituationJuridique(contexte As ContexteOperation, s As SituationJuridique)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Executer(QuerySituationsJuridiques.InsertSituationJuridique, s, False)
    End Sub

    Public Sub UpdateSituationJuridique(contexte As ContexteOperation, s As SituationJuridique)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Executer(QuerySituationsJuridiques.UpdateSituationJuridique, s, True)
    End Sub

    Private Sub Executer(query As String, s As SituationJuridique, avecId As Boolean)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                If avecId Then cmd.Parameters.AddWithValue("@id", s.IdSituationJuridique)
                cmd.Parameters.AddWithValue("@code", s.CodeSituationJuridique)
                cmd.Parameters.AddWithValue("@libelle", s.LibelleSituationJuridique)
                cmd.Parameters.AddWithValue("@porteur", s.PorteurBce)
                cmd.Parameters.AddWithValue("@actif", s.Actif)
                cmd.Parameters.AddWithValue("@ordre", s.OrdreAffichage)
                If avecId Then
                    ' Modification : la requête exclut ce qu'une ligne système interdit (audit C.1).
                    ReglesReferentiels.ExecuterSurLigneModifiable(cmd)
                Else
                    cmd.ExecuteNonQuery()
                End If
            End Using
        End Using
    End Sub

    Public Sub DesactiverSituationJuridique(contexte As ContexteOperation, id As ULong)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QuerySituationsJuridiques.DesactiverSituationJuridique, conn)
                cmd.Parameters.AddWithValue("@id", id)
                ReglesReferentiels.ExecuterSurLigneModifiable(cmd)
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    Public Function CodeSituationJuridiqueExiste(code As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QuerySituationsJuridiques.SelectCountCodeSituationJuridique, "@code", code, idExclu) > 0
    End Function

    Public Function LibelleSituationJuridiqueExiste(libelle As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QuerySituationsJuridiques.SelectCountLibelleSituationJuridique, "@libelle", libelle, idExclu) > 0
    End Function

    Private Function Compter(query As String, nomParam As String, valeur As String, idExclu As ULong) As Integer
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue(nomParam, valeur)
                cmd.Parameters.AddWithValue("@id", idExclu)
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

#End Region

End Module
