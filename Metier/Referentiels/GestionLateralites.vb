' -------------------------------------------------------------------------------------------------
' Module      : GestionLateralites
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Couche métier du référentiel des latéralités (ref_lateralites) : lecture, écriture,
' vérifications.
'
' Remarques   :
' - SQL dans QueryLateralites ; accès par DatabaseManager ; requêtes paramétrées.
' - Pas de suppression physique : une latéralité qui ne sert plus est désactivée (elle reste
'   lisible chez les patients qui l'ont).
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : chaque écriture reçoit le ContexteOperation et
'   commence par ReglesReferentiels.ExigerDroitEcriture (SuperUser).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionLateralites

#Region "Lecture"

    Public Function GetLateralites(afficherInactifs As Boolean) As List(Of Lateralite)

        Dim liste As New List(Of Lateralite)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(If(afficherInactifs, QueryLateralites.SelectLateralitesToutes,
                                             QueryLateralites.SelectLateralitesActives), conn)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        liste.Add(New Lateralite With {
                            .IdLateralite = reader.GetUInt64("id_lateralite"),
                            .CodeLateralite = reader.GetString("code_lateralite"),
                            .LibelleLateralite = reader.GetString("libelle_lateralite"),
                            .Actif = reader.GetBoolean("actif"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage")
                        })
                    End While
                End Using
            End Using
        End Using

        Return liste

    End Function

#End Region

#Region "Écriture"

    Public Sub InsertLateralite(contexte As ContexteOperation, l As Lateralite)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Executer(QueryLateralites.InsertLateralite, l, False)
    End Sub

    Public Sub UpdateLateralite(contexte As ContexteOperation, l As Lateralite)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Executer(QueryLateralites.UpdateLateralite, l, True)
    End Sub

    Private Sub Executer(query As String, l As Lateralite, avecId As Boolean)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                If avecId Then cmd.Parameters.AddWithValue("@id", l.IdLateralite)
                cmd.Parameters.AddWithValue("@code", l.CodeLateralite)
                cmd.Parameters.AddWithValue("@libelle", l.LibelleLateralite)
                cmd.Parameters.AddWithValue("@actif", l.Actif)
                cmd.Parameters.AddWithValue("@ordre", l.OrdreAffichage)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    ' Active ou désactive (jamais de suppression physique).
    Public Sub DefinirActivation(contexte As ContexteOperation, id As ULong, actif As Boolean)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLateralites.DefinirActivationLateralite, conn)
                cmd.Parameters.AddWithValue("@id", id)
                cmd.Parameters.AddWithValue("@actif", actif)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    Public Function CodeLateraliteExiste(code As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QueryLateralites.SelectCountCodeLateralite, "@code", code, idExclu) > 0
    End Function

    Public Function LibelleLateraliteExiste(libelle As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QueryLateralites.SelectCountLibelleLateralite, "@libelle", libelle, idExclu) > 0
    End Function

    Private Function Compter(query As String, nomParam As String, valeur As String, id As ULong) As Integer
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue(nomParam, valeur)
                cmd.Parameters.AddWithValue("@id", id)
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

#End Region

End Module
