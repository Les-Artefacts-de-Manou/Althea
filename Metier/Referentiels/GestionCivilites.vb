' -------------------------------------------------------------------------------------------------
' Module      : GestionCivilites
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Couche métier du référentiel des civilités (ref_civilites) : lecture, écriture, vérifications.
'
' Remarques   :
' - SQL centralisé dans QueryCivilites ; accès via DatabaseManager.
' - Abréviation facultative : chaîne vide enregistrée comme NULL.
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : chaque écriture reçoit le ContexteOperation et
'   commence par ReglesReferentiels.ExigerDroitEcriture (SuperUser).
'   Lignes système : refus explicite si la requête n'a concerné aucune ligne (ExecuterSurLigneModifiable).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionCivilites

#Region "Lecture"

    Public Function GetCivilitesActives() As List(Of Civilite)
        Return Lire(QueryCivilites.SelectCivilitesActives)
    End Function

    Public Function GetCivilites(afficherInactifs As Boolean) As List(Of Civilite)
        Return Lire(If(afficherInactifs, QueryCivilites.SelectCivilitesToutes, QueryCivilites.SelectCivilitesActives))
    End Function

    Private Function Lire(query As String) As List(Of Civilite)

        Dim liste As New List(Of Civilite)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()
                    Dim ordAbreviation = reader.GetOrdinal("abreviation_civilite")
                    While reader.Read()
                        liste.Add(New Civilite With {
                            .IdCivilite = reader.GetUInt64("id_civilite"),
                            .CodeCivilite = reader.GetString("code_civilite"),
                            .LibelleCivilite = reader.GetString("libelle_civilite"),
                            .AbreviationCivilite = If(reader.IsDBNull(ordAbreviation), Nothing, reader.GetString(ordAbreviation)),
                            .Actif = reader.GetBoolean("actif"),
                            .EstSysteme = reader.GetBoolean("est_systeme"),
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

    Public Sub InsertCivilite(contexte As ContexteOperation, c As Civilite)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Executer(QueryCivilites.InsertCivilite, c, False)
    End Sub

    Public Sub UpdateCivilite(contexte As ContexteOperation, c As Civilite)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Executer(QueryCivilites.UpdateCivilite, c, True)
    End Sub

    Private Sub Executer(query As String, c As Civilite, avecId As Boolean)
        Dim abreviation As String = If(c.AbreviationCivilite, String.Empty).Trim()
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                If avecId Then cmd.Parameters.AddWithValue("@id", c.IdCivilite)
                cmd.Parameters.AddWithValue("@code", c.CodeCivilite)
                cmd.Parameters.AddWithValue("@libelle", c.LibelleCivilite)
                cmd.Parameters.AddWithValue("@abreviation", If(abreviation.Length = 0, CObj(DBNull.Value), abreviation))
                cmd.Parameters.AddWithValue("@actif", c.Actif)
                cmd.Parameters.AddWithValue("@ordre", c.OrdreAffichage)
                If avecId Then
                    ' Modification : la requête exclut ce qu'une ligne système interdit (audit C.1).
                    ReglesReferentiels.ExecuterSurLigneModifiable(cmd)
                Else
                    cmd.ExecuteNonQuery()
                End If
            End Using
        End Using
    End Sub

    Public Sub DesactiverCivilite(contexte As ContexteOperation, id As ULong)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryCivilites.DesactiverCivilite, conn)
                cmd.Parameters.AddWithValue("@id", id)
                ReglesReferentiels.ExecuterSurLigneModifiable(cmd)
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    Public Function CodeCiviliteExiste(code As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QueryCivilites.SelectCountCodeCivilite, "@code", code, idExclu) > 0
    End Function

    Public Function LibelleCiviliteExiste(libelle As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QueryCivilites.SelectCountLibelleCivilite, "@libelle", libelle, idExclu) > 0
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
