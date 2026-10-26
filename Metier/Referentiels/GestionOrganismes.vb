' -------------------------------------------------------------------------------------------------
' Module   : GestionOrganismes
' Projet   : Althéa
' Version  : V1.2.1
' Date     : 25/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les organismes (remboursement et mutualités : lecture, écriture, vérifications) à partir
' de la table référentielle ref_organismes.
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB et QueryOrganismes pour le SQL
' - Pas de suppression physique : un organisme se désactive (soft-delete)
' - Les lignes système (est_systeme = 1) ne peuvent être ni désactivées ni supprimées ; leur type
'   n'est pas modifiable (garde-fou dans la requête UPDATE)
' - V1.2.0 (22/10/2026, audit C.1, étape 3) : chaque écriture reçoit le ContexteOperation et
'   commence par ReglesReferentiels.ExigerDroitEcriture (SuperUser) ; SupprimerOrganisme, plus
'   appelée, supprimée.
'   Lignes système : refus explicite si la requête n'a concerné aucune ligne (ExecuterSurLigneModifiable).
' - V1.2.1 (25/10/2026, audit D-1) : Option Strict On.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionOrganismes

#Region "Lecture"

    Public Function GetOrganismesActifs() As List(Of Organisme)
        Return GetOrganismesDepuisRequete(QueryOrganismes.SelectOrganismesActifs)
    End Function

    Public Function GetOrganismes(afficherInactifs As Boolean) As List(Of Organisme)

        If afficherInactifs Then
            Return GetOrganismesDepuisRequete(QueryOrganismes.SelectOrganismesTous)
        End If

        Return GetOrganismesActifs()

    End Function

    Private Function GetOrganismesDepuisRequete(query As String) As List(Of Organisme)

        Dim result As New List(Of Organisme)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()

                    While reader.Read()
                        result.Add(New Organisme With {
                            .IdOrganisme = reader.GetUInt64("id_organisme"),
                            .CodeOrganisme = reader.GetString("code_organisme"),
                            .LibelleOrganisme = reader.GetString("libelle_organisme"),
                            .TypeOrganisme = TypesOrganisme.DepuisCode(reader.GetString("type_organisme")),
                            .Adresse = LireTexte(reader, "adresse"),
                            .Telephone = LireTexte(reader, "telephone"),
                            .Telephone2 = LireTexte(reader, "telephone_2"),
                            .Email = LireTexte(reader, "email"),
                            .Email2 = LireTexte(reader, "email_2"),
                            .SiteWeb = LireTexte(reader, "site_web"),
                            .UrlPlateforme = LireTexte(reader, "url_plateforme"),
                            .ProcedureEncodage = LireTexte(reader, "procedure_encodage"),
                            .IdPeriodicite = If(reader.IsDBNull(reader.GetOrdinal("id_periodicite")), 0UL, reader.GetUInt64("id_periodicite")),
                            .LibellePeriodicite = LireTexte(reader, "libelle_periodicite"),
                            .EstSysteme = reader.GetBoolean("est_systeme"),
                            .Actif = reader.GetBoolean("actif"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage")
                        })
                    End While

                End Using
            End Using
        End Using

        Return result

    End Function

    Private Function LireTexte(reader As MySqlDataReader, colonne As String) As String
        Dim ord = reader.GetOrdinal(colonne)
        Return If(reader.IsDBNull(ord), String.Empty, reader.GetString(ord))
    End Function

#End Region

#Region "Ecriture"

    Public Sub InsertOrganisme(contexte As ContexteOperation, org As Organisme)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryOrganismes.InsertOrganisme, conn)
                AjouterParametresCommuns(cmd, org)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Sub UpdateOrganisme(contexte As ContexteOperation, org As Organisme)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryOrganismes.UpdateOrganisme, conn)
                cmd.Parameters.AddWithValue("@id", org.IdOrganisme)
                AjouterParametresCommuns(cmd, org)
                ReglesReferentiels.ExecuterSurLigneModifiable(cmd)
            End Using
        End Using
    End Sub

    Private Sub AjouterParametresCommuns(cmd As MySqlCommand, org As Organisme)
        cmd.Parameters.AddWithValue("@code", org.CodeOrganisme)
        cmd.Parameters.AddWithValue("@libelle", org.LibelleOrganisme)
        cmd.Parameters.AddWithValue("@type", TypesOrganisme.VersCode(org.TypeOrganisme))
        cmd.Parameters.AddWithValue("@adresse", TexteOuNull(org.Adresse))
        cmd.Parameters.AddWithValue("@telephone", TexteOuNull(org.Telephone))
        cmd.Parameters.AddWithValue("@telephone2", TexteOuNull(org.Telephone2))
        cmd.Parameters.AddWithValue("@email", TexteOuNull(org.Email))
        cmd.Parameters.AddWithValue("@email2", TexteOuNull(org.Email2))
        cmd.Parameters.AddWithValue("@siteWeb", TexteOuNull(org.SiteWeb))
        cmd.Parameters.AddWithValue("@urlPlateforme", TexteOuNull(org.UrlPlateforme))
        cmd.Parameters.AddWithValue("@procedure", TexteOuNull(org.ProcedureEncodage))
        cmd.Parameters.AddWithValue("@idPeriodicite", If(org.IdPeriodicite = 0UL, CObj(DBNull.Value), org.IdPeriodicite))
        cmd.Parameters.AddWithValue("@actif", org.Actif)
        cmd.Parameters.AddWithValue("@ordre", org.OrdreAffichage)
    End Sub

    Private Function TexteOuNull(valeur As String) As Object
        Dim v = If(valeur, String.Empty).Trim()
        Return If(v.Length = 0, CObj(DBNull.Value), v)
    End Function

    Public Sub DesactiverOrganisme(contexte As ContexteOperation, idOrganisme As ULong)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryOrganismes.DesactiverOrganisme, conn)
                cmd.Parameters.AddWithValue("@id", idOrganisme)
                ReglesReferentiels.ExecuterSurLigneModifiable(cmd)
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    Public Function CodeOrganismeExiste(code As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QueryOrganismes.SelectCountCodeOrganisme, "@code", code, idExclu) > 0
    End Function

    Public Function LibelleOrganismeExiste(libelle As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QueryOrganismes.SelectCountLibelleOrganisme, "@libelle", libelle, idExclu) > 0
    End Function

    Public Function OrganismeEstUtilise(idOrganisme As ULong) As Boolean
        Return Compter(QueryOrganismes.SelectCountUsageOrganisme, Nothing, Nothing, idOrganisme) > 0
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
