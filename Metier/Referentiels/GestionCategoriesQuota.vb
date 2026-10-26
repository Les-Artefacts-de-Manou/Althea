' -------------------------------------------------------------------------------------------------
' Module   : GestionCategoriesQuota
' Projet   : Althéa
' Version  : V1.1.0
' Date     : 22/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les catégories de quota d'un domaine (lecture, écriture, vérifications) à partir de la
' table référentielle ref_categories_quota.
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB et QueryCategoriesQuota pour le SQL
' - Suppression physique réservée aux catégories jamais utilisées (ni nature, ni plafond) ;
'   sinon désactivation, afin de préserver l'historique des règles de quota.
' - Toute écriture vide le cache des règles de quota.
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : chaque écriture reçoit le ContexteOperation et
'   commence par ReglesReferentiels.ExigerDroitEcriture (SuperUser).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionCategoriesQuota

#Region "Lecture"

    Public Function GetCategoriesParDomaine(idDomaine As ULong,
                                            afficherInactives As Boolean) As List(Of CategorieQuota)

        Dim result As New List(Of CategorieQuota)

        Try

            Using conn = DatabaseManager.OpenConnection()
                Using cmd As New MySqlCommand(QueryCategoriesQuota.SelectCategoriesParDomaine, conn)
                    cmd.Parameters.AddWithValue("@id_domaine", idDomaine)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            result.Add(MapCategorie(reader))
                        End While
                    End Using
                End Using
            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur GetCategoriesParDomaine (domaine={idDomaine}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

        If afficherInactives Then Return result

        Return result.Where(Function(c) c.Actif).ToList()

    End Function

    Public Function GetCategoriesActives() As List(Of CategorieQuota)

        Dim result As New List(Of CategorieQuota)

        Try

            Using conn = DatabaseManager.OpenConnection()
                Using cmd As New MySqlCommand(QueryCategoriesQuota.SelectCategoriesActives, conn)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            result.Add(MapCategorie(reader))
                        End While
                    End Using
                End Using
            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur GetCategoriesActives.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

        Return result

    End Function

    Private Function MapCategorie(reader As MySqlDataReader) As CategorieQuota

        Return New CategorieQuota With {
            .IdCategorieQuota = reader.GetUInt64("id_categorie_quota"),
            .IdDomaine = reader.GetUInt64("id_domaine"),
            .CodeCategorie = reader.GetString("code_categorie"),
            .LibelleCategorie = reader.GetString("libelle_categorie"),
            .Actif = reader.GetBoolean("actif"),
            .OrdreAffichage = reader.GetInt32("ordre_affichage")
        }

    End Function

#End Region

#Region "Ecriture"

    Public Sub InsertCategorie(contexte As ContexteOperation, categorie As CategorieQuota)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Executer(QueryCategoriesQuota.InsertCategorie, categorie, "InsertCategorie")

    End Sub

    Public Sub UpdateCategorie(contexte As ContexteOperation, categorie As CategorieQuota)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Executer(QueryCategoriesQuota.UpdateCategorie, categorie, "UpdateCategorie")

    End Sub

    Public Sub SupprimerCategorie(contexte As ContexteOperation, idCategorieQuota As ULong)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Try

            Using conn = DatabaseManager.OpenConnection()
                Using cmd As New MySqlCommand(QueryCategoriesQuota.SupprimerCategorie, conn)
                    cmd.Parameters.AddWithValue("@id", idCategorieQuota)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            GestionQuotasInami.ViderCache()

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur SupprimerCategorie (id={idCategorieQuota}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

    Private Sub Executer(query As String, categorie As CategorieQuota, contexte As String)

        Try

            Using conn = DatabaseManager.OpenConnection()
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@id", categorie.IdCategorieQuota)
                    cmd.Parameters.AddWithValue("@id_domaine", categorie.IdDomaine)
                    cmd.Parameters.AddWithValue("@code", categorie.CodeCategorie.Trim().ToUpperInvariant())
                    cmd.Parameters.AddWithValue("@libelle", categorie.LibelleCategorie.Trim())
                    cmd.Parameters.AddWithValue("@actif", categorie.Actif)
                    cmd.Parameters.AddWithValue("@ordre", categorie.OrdreAffichage)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            GestionQuotasInami.ViderCache()

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur {contexte} (id={categorie.IdCategorieQuota}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

#End Region

#Region "Vérifications"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : ValiderCategorie
    '
    ' Rôle      :
    ' Contrôle la cohérence d'une catégorie avant enregistrement.
    '
    ' Retour    :
    ' - Chaîne vide si valide, sinon le message à présenter à l'utilisateur
    ' -------------------------------------------------------------------------------------------------
    Public Function ValiderCategorie(categorie As CategorieQuota) As String

        If categorie Is Nothing Then Return "Aucune catégorie à enregistrer."
        If categorie.IdDomaine = 0 Then Return "Le domaine est obligatoire."
        If String.IsNullOrWhiteSpace(categorie.CodeCategorie) Then Return "Le code de la catégorie est obligatoire."
        If String.IsNullOrWhiteSpace(categorie.LibelleCategorie) Then Return "Le libellé de la catégorie est obligatoire."
        If categorie.CodeCategorie.Trim().Length > 30 Then Return "Le code ne peut pas dépasser 30 caractères."
        If categorie.LibelleCategorie.Trim().Length > 100 Then Return "Le libellé ne peut pas dépasser 100 caractères."

        If Compter(QueryCategoriesQuota.SelectCountCodeCategorie, categorie,
                   categorie.CodeCategorie.Trim().ToUpperInvariant()) > 0 Then
            Return "Ce code de catégorie existe déjà pour ce domaine."
        End If

        If Compter(QueryCategoriesQuota.SelectCountLibelleCategorie, categorie,
                   categorie.LibelleCategorie.Trim()) > 0 Then
            Return "Ce libellé de catégorie existe déjà pour ce domaine."
        End If

        Return String.Empty

    End Function

    ' Une catégorie utilisée (nature rattachée ou plafond historisé) ne peut qu'être désactivée
    Public Function CategorieEstUtilisee(idCategorieQuota As ULong) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryCategoriesQuota.SelectCountUsageCategorie, conn)
                cmd.Parameters.AddWithValue("@id", idCategorieQuota)
                Return Convert.ToInt32(cmd.ExecuteScalar()) > 0
            End Using
        End Using

    End Function

    Private Function Compter(query As String, categorie As CategorieQuota, valeur As String) As Integer

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@id_domaine", categorie.IdDomaine)
                cmd.Parameters.AddWithValue("@valeur", valeur)
                cmd.Parameters.AddWithValue("@id", categorie.IdCategorieQuota)
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using

    End Function

#End Region

End Module
