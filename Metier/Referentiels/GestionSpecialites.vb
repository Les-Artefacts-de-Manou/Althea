' -------------------------------------------------------------------------------------------------
' Module      : GestionSpecialites
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Couche métier du référentiel des spécialités des thérapeutes (ref_specialites) : lecture,
' écriture, vérifications.
'
' Remarques   :
' - SQL dans QuerySpecialites ; accès par DatabaseManager ; requêtes paramétrées.
' - Pas de suppression physique : une spécialité qui ne sert plus est désactivée (elle reste
'   lisible chez les thérapeutes qui l'ont).
' - Chaque écriture reçoit le ContexteOperation et commence par ReglesReferentiels.ExigerDroitEcriture
'   (SuperUser), comme tous les référentiels (audit C.1).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionSpecialites

#Region "Lecture"

    Public Function GetSpecialites(afficherInactifs As Boolean) As List(Of Specialite)

        Dim liste As New List(Of Specialite)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(If(afficherInactifs, QuerySpecialites.SelectSpecialitesToutes,
                                             QuerySpecialites.SelectSpecialitesActives), conn)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        liste.Add(New Specialite With {
                            .IdSpecialite = reader.GetUInt64("id_specialite"),
                            .CodeSpecialite = reader.GetString("code_specialite"),
                            .LibelleSpecialite = reader.GetString("libelle_specialite"),
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

    Public Sub InsertSpecialite(contexte As ContexteOperation, s As Specialite)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Executer(QuerySpecialites.InsertSpecialite, s, False)
    End Sub

    Public Sub UpdateSpecialite(contexte As ContexteOperation, s As Specialite)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Executer(QuerySpecialites.UpdateSpecialite, s, True)
    End Sub

    Private Sub Executer(query As String, s As Specialite, avecId As Boolean)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                If avecId Then cmd.Parameters.AddWithValue("@id", s.IdSpecialite)
                cmd.Parameters.AddWithValue("@code", s.CodeSpecialite)
                cmd.Parameters.AddWithValue("@libelle", s.LibelleSpecialite)
                cmd.Parameters.AddWithValue("@actif", s.Actif)
                cmd.Parameters.AddWithValue("@ordre", s.OrdreAffichage)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    ' Active ou désactive (jamais de suppression physique).
    Public Sub DefinirActivation(contexte As ContexteOperation, id As ULong, actif As Boolean)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QuerySpecialites.DefinirActivationSpecialite, conn)
                cmd.Parameters.AddWithValue("@id", id)
                cmd.Parameters.AddWithValue("@actif", actif)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    Public Function CodeSpecialiteExiste(code As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QuerySpecialites.SelectCountCodeSpecialite, "@code", code, idExclu) > 0
    End Function

    Public Function LibelleSpecialiteExiste(libelle As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QuerySpecialites.SelectCountLibelleSpecialite, "@libelle", libelle, idExclu) > 0
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
