' -------------------------------------------------------------------------------------------------
' Module      : GestionPays
' Projet      : Althéa
' Version     : V1.2.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Couche métier du référentiel des pays (ref_pays) : lecture, écriture, vérifications.
'
' Remarques   :
' - SQL centralisé dans QueryPays ; accès via DatabaseManager.
' - Les lignes système (pays dotés de règles de validation) ne peuvent ni changer de code ni être
'   désactivées (garde-fou dans l'interface et dans les requêtes).
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : chaque écriture reçoit le ContexteOperation et
'   commence par ReglesReferentiels.ExigerDroitEcriture (SuperUser).
'   Lignes système : refus explicite si la requête n'a concerné aucune ligne (ExecuterSurLigneModifiable).
' - V1.2.0 (23/10/2026, audit C.2) : CodePaysParId (valideurs métier des fiches avec adresse).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionPays

#Region "Lecture"

    Public Function GetPaysActifs() As List(Of Pays)
        Return Lire(QueryPays.SelectPaysActifs)
    End Function

    Public Function GetPays(afficherInactifs As Boolean) As List(Of Pays)
        Return Lire(If(afficherInactifs, QueryPays.SelectPaysTous, QueryPays.SelectPaysActifs))
    End Function

    ' Code ISO d'un pays du référentiel (actif ou non), ou Nothing (pays absent ou non choisi) : sert
    ' aux valideurs métier, qui jugent téléphone et code postal selon le pays de l'adresse (audit C.2).
    Public Function CodePaysParId(idPays As ULong?) As String
        If Not idPays.HasValue OrElse idPays.Value = 0UL Then Return Nothing
        Return GetPays(True).FirstOrDefault(Function(p) p.IdPays = idPays.Value)?.CodePays
    End Function

    Private Function Lire(query As String) As List(Of Pays)

        Dim liste As New List(Of Pays)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        liste.Add(New Pays With {
                            .IdPays = reader.GetUInt64("id_pays"),
                            .CodePays = reader.GetString("code_pays"),
                            .LibellePays = reader.GetString("libelle_pays"),
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

    Public Sub InsertPays(contexte As ContexteOperation, p As Pays)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Executer(QueryPays.InsertPays, p, False)
    End Sub

    Public Sub UpdatePays(contexte As ContexteOperation, p As Pays)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Executer(QueryPays.UpdatePays, p, True)
    End Sub

    Private Sub Executer(query As String, p As Pays, avecId As Boolean)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                If avecId Then cmd.Parameters.AddWithValue("@id", p.IdPays)
                cmd.Parameters.AddWithValue("@code", p.CodePays)
                cmd.Parameters.AddWithValue("@libelle", p.LibellePays)
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

    Public Sub DesactiverPays(contexte As ContexteOperation, id As ULong)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryPays.DesactiverPays, conn)
                cmd.Parameters.AddWithValue("@id", id)
                ReglesReferentiels.ExecuterSurLigneModifiable(cmd)
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    Public Function CodePaysExiste(code As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QueryPays.SelectCountCodePays, "@code", code, idExclu) > 0
    End Function

    Public Function LibellePaysExiste(libelle As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QueryPays.SelectCountLibellePays, "@libelle", libelle, idExclu) > 0
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
