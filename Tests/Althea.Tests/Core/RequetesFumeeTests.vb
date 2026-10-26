' -------------------------------------------------------------------------------------------------
' Classe   : RequetesFumeeTests
' Projet   : Althéa (tests automatisés)
' Version  : V1.0.0
' Date     : 26/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Test de fumée des requêtes : chaque requête des modules Query* est préparée par MariaDB sur la
' base vide althea_tests (PREPARE), jamais exécutée. Une colonne mal écrite, une table disparue ou
' une requête tronquée (audit B4) la font échouer, avec le message de MariaDB.
'
' Remarques (ADR-58) :
' - Ignoré si ALTHEA_TESTS_CONNEXION n'est pas définie sur le poste (une seule ligne « ignorée »).
' - Aucune donnée : la base de test ne contient que la structure (BaseDeTests).
' - Le découpage des requêtes (CatalogueRequetes.Instructions) est lui-même vérifié, sans base.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Class RequetesFumeeTests
    Implements IClassFixture(Of BaseDeTests)

    Private Const SANS_BASE As String = "(base de test non configurée)"

    Private ReadOnly _base As BaseDeTests

    Public Sub New(base As BaseDeTests)
        _base = base
    End Sub

    Public Shared Function Requetes() As TheoryData(Of String)
        Dim data As New TheoryData(Of String)
        If Not BaseDeTests.EstConfiguree Then
            data.Add(SANS_BASE)
            Return data
        End If
        For Each nom In CatalogueRequetes.Toutes.Keys
            data.Add(nom)
        Next
        Return data
    End Function

    <Theory>
    <MemberData(NameOf(Requetes))>
    Public Sub Requete_AccepteeParMariaDB(nom As String)

        If Not _base.Disponible Then
            Assert.Skip($"Base de test non configurée sur ce poste ({BaseDeTests.VARIABLE_CONNEXION}).")
        End If

        Dim instructions = CatalogueRequetes.Instructions(CatalogueRequetes.Toutes(nom))
        Assert.NotEmpty(instructions)

        Using conn As New MySqlConnection(_base.ChaineConnexion)
            conn.Open()
            For Each instruction In instructions
                Try
                    Using cmd As New MySqlCommand("PREPARE fumee FROM @texte", conn)
                        cmd.Parameters.AddWithValue("@texte", instruction)
                        cmd.ExecuteNonQuery()
                    End Using
                    Using cmd As New MySqlCommand("DEALLOCATE PREPARE fumee", conn)
                        cmd.ExecuteNonQuery()
                    End Using
                Catch ex As MySqlException
                    Assert.Fail($"MariaDB refuse la requête : {ex.Message}{Environment.NewLine}{instruction}")
                End Try
            Next
        End Using

    End Sub

    <Fact>
    Public Sub Instructions_ParametresEnPointDInterrogationEtDecoupage()
        Dim sql = "UPDATE t SET a = @a, b = 'x;@y' WHERE id = @id_t; -- fin ; commentaire" & vbLf &
                  "SELECT @@version, LAST_INSERT_ID();"
        Assert.Equal(New String() {"UPDATE t SET a = ?, b = 'x;@y' WHERE id = ?",
                                   "-- fin ; commentaire" & vbLf & "SELECT @@version, LAST_INSERT_ID()"},
                     CatalogueRequetes.Instructions(sql))
    End Sub

    <Fact>
    Public Sub Instructions_CommentaireSeulEcarte()
        Assert.Equal(New String() {"SELECT 1"}, CatalogueRequetes.Instructions("SELECT 1;" & vbLf & "-- rien" & vbLf))
    End Sub

    <Fact>
    Public Sub Catalogue_RecenseLesModulesQuery()
        ' Sans base : le recensement lui-même (plus de 300 requêtes au 26/10/2026, zones cliniques comprises).
        Assert.True(CatalogueRequetes.Toutes.Count > 300, $"{CatalogueRequetes.Toutes.Count} requêtes recensées")
        Assert.Contains("QueryDossiers.SelectZoneDossier(MotifSuivi)", CatalogueRequetes.Toutes.Keys)
        Assert.All(CatalogueRequetes.Toutes, Sub(kv) Assert.False(String.IsNullOrWhiteSpace(kv.Value), kv.Key))
    End Sub

End Class
