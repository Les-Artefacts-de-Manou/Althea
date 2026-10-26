' -------------------------------------------------------------------------------------------------
' Module      : GestionPeriodicites
' Projet      : Althéa
' Version     : V1.1.0
'
' Rôle        :
' Couche métier du référentiel des périodicités d'encodage (ref_periodicites).
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : chaque écriture reçoit le ContexteOperation et
'   commence par ReglesReferentiels.ExigerDroitEcriture (SuperUser).
'   Lignes système : refus explicite si la requête n'a concerné aucune ligne (ExecuterSurLigneModifiable).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionPeriodicites

#Region "Lecture"

    Public Function GetPeriodicitesActives() As List(Of Periodicite)
        Return Lire(QueryPeriodicites.SelectPeriodicitesActives)
    End Function

    Public Function GetPeriodicites(afficherInactifs As Boolean) As List(Of Periodicite)
        Return Lire(If(afficherInactifs,
                       QueryPeriodicites.SelectPeriodicitesToutes,
                       QueryPeriodicites.SelectPeriodicitesActives))
    End Function

    Private Function Lire(query As String) As List(Of Periodicite)

        Dim liste As New List(Of Periodicite)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()
                    Dim ordNbMois = reader.GetOrdinal("nb_mois")
                    While reader.Read()
                        liste.Add(New Periodicite With {
                            .IdPeriodicite = reader.GetUInt64("id_periodicite"),
                            .CodePeriodicite = reader.GetString("code_periodicite"),
                            .LibellePeriodicite = reader.GetString("libelle_periodicite"),
                            .NbMois = If(reader.IsDBNull(ordNbMois), CType(Nothing, Integer?), reader.GetInt32(ordNbMois)),
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

    Public Sub InsertPeriodicite(contexte As ContexteOperation, p As Periodicite)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Executer(QueryPeriodicites.InsertPeriodicite, p, False)
    End Sub

    Public Sub UpdatePeriodicite(contexte As ContexteOperation, p As Periodicite)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Executer(QueryPeriodicites.UpdatePeriodicite, p, True)
    End Sub

    Private Sub Executer(query As String, p As Periodicite, avecId As Boolean)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                If avecId Then cmd.Parameters.AddWithValue("@id", p.IdPeriodicite)
                cmd.Parameters.AddWithValue("@code", p.CodePeriodicite)
                cmd.Parameters.AddWithValue("@libelle", p.LibellePeriodicite)
                cmd.Parameters.AddWithValue("@nbMois", If(p.NbMois.HasValue, CObj(p.NbMois.Value), DBNull.Value))
                cmd.Parameters.AddWithValue("@actif", p.Actif)
                cmd.Parameters.AddWithValue("@ordre", p.OrdreAffichage)
                If avecId Then
                    ' Modification : la requête exclut ce qu'une ligne système interdit (audit C.1).
                    ReglesReferentiels.ExecuterSurLigneModifiable(cmd)
                Else
                    cmd.ExecuteNonQuery()
                End If
            End Using
        End Using
    End Sub

    Public Sub DesactiverPeriodicite(contexte As ContexteOperation, id As ULong)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryPeriodicites.DesactiverPeriodicite, conn)
                cmd.Parameters.AddWithValue("@id", id)
                ReglesReferentiels.ExecuterSurLigneModifiable(cmd)
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    Public Function CodePeriodiciteExiste(code As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QueryPeriodicites.SelectCountCodePeriodicite, "@code", code, idExclu) > 0
    End Function

    Public Function LibellePeriodiciteExiste(libelle As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QueryPeriodicites.SelectCountLibellePeriodicite, "@libelle", libelle, idExclu) > 0
    End Function

    Public Function PeriodiciteEstUtilisee(id As ULong) As Boolean
        Return Compter(QueryPeriodicites.SelectCountUsagePeriodicite, Nothing, Nothing, id) > 0
    End Function

    Private Function Compter(query As String, nomParam As String, valeur As String, id As ULong) As Integer
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                If nomParam IsNot Nothing Then cmd.Parameters.AddWithValue(nomParam, valeur)
                cmd.Parameters.AddWithValue("@id", id)
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

#End Region

End Module
