' -------------------------------------------------------------------------------------------------
' Module     : GestionTextesTypes
' Projet     : Althéa
' Version    : V1.2.0
' Date       : 22/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Gère les textes types (ref_textes_types), leurs usages (ref_usages_textes_types) et la liaison
' entre les deux (lia_textes_types_usages) : référentiels « Textes types » et « Usages des textes
' types », textes proposés par les éditeurs de texte.
'
' Remarques  :
' - DatabaseManager pour l'accès, QueryTextesTypes pour le SQL ; requêtes paramétrées.
' - Pas de suppression physique : un texte type qui ne sert plus est désactivé.
' - Les usages naissent par migration (lignes système) : seuls libellé, ordre et activation
'   changent (UpdateUsage).
' - Une erreur de base remonte à l'appelant, qui la journalise sans contenu de texte.
' - V1.1.0 (19/10/2026, v2.43.0) : plusieurs usages par texte, enregistrés avec le texte dans une
'   transaction ; lecture et modification des usages.
' - V1.2.0 (22/10/2026, audit C.1, étape 3) : chaque écriture reçoit le ContexteOperation et
'   commence par ReglesReferentiels.ExigerDroitEcriture (SuperUser).
'   Lignes système : refus explicite si la requête n'a concerné aucune ligne (ExecuterSurLigneModifiable).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionTextesTypes

#Region "Textes types : lecture"

    ' Textes du référentiel (actifs seulement, ou tous).
    Public Function GetTextesTypes(afficherInactifs As Boolean) As List(Of TexteType)
        Return LireTextes(If(afficherInactifs, QueryTextesTypes.SelectTextesTypesTous, QueryTextesTypes.SelectTextesTypesActifs), Nothing)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetTextesTypesPourUsage
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Textes proposés dans un éditeur : actifs, liés à son usage (UsagesTexteType) ou
    '              sans usage (proposés partout).
    ' -------------------------------------------------------------------------------------------------
    Public Function GetTextesTypesPourUsage(codeUsage As String) As List(Of TexteType)
        Return LireTextes(QueryTextesTypes.SelectTextesTypesPourUsage, If(codeUsage, String.Empty))
    End Function

    Private Function LireTextes(requete As String, codeUsage As String) As List(Of TexteType)

        Dim textes As New List(Of TexteType)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(requete, conn)

                If codeUsage IsNot Nothing Then cmd.Parameters.AddWithValue("@codeUsage", codeUsage)

                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        textes.Add(New TexteType With {
                            .IdTexteType = CULng(reader.GetInt64("id_texte_type")),
                            .CodeTexteType = reader.GetString("code_texte_type"),
                            .LibelleTexteType = reader.GetString("libelle_texte_type"),
                            .IdsUsages = LireIdentifiants(If(reader.IsDBNull("ids_usages"), Nothing, reader.GetString("ids_usages"))),
                            .LibellesUsages = If(reader.IsDBNull("libelles_usages"), String.Empty, reader.GetString("libelles_usages")),
                            .ContenuRtf = If(reader.IsDBNull("contenu_rtf"), Nothing, reader.GetString("contenu_rtf")),
                            .ContenuTxt = If(reader.IsDBNull("contenu_txt"), Nothing, reader.GetString("contenu_txt")),
                            .Actif = reader.GetBoolean("actif"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage")
                        })
                    End While
                End Using

            End Using
        End Using

        Return textes

    End Function

    ' « 3,7,12 » (GROUP_CONCAT) → identifiants ; vide → aucun usage.
    Private Function LireIdentifiants(liste As String) As List(Of ULong)

        Dim ids As New List(Of ULong)
        If String.IsNullOrWhiteSpace(liste) Then Return ids

        For Each morceau As String In liste.Split(","c)
            Dim id As ULong
            If ULong.TryParse(morceau.Trim(), id) Then ids.Add(id)
        Next

        Return ids

    End Function

#End Region

#Region "Textes types : écriture"

    ' -------------------------------------------------------------------------------------------------
    ' Procédures : InsertTexteType / UpdateTexteType
    ' Version    : V1.2.0
    ' Date       : 22/10/2026
    '
    ' Rôle       : Enregistre le texte et ses usages en une transaction (tout ou rien) : un texte ne
    '              peut pas rester enregistré sans les usages choisis, ce qui le rendrait « partout ».
    ' - V1.2.0 (22/10/2026, audit C.1) : reçoit le ContexteOperation ; droit d'écriture vérifié par le métier.
    ' -------------------------------------------------------------------------------------------------
    Public Sub InsertTexteType(contexte As ContexteOperation, texte As TexteType)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Enregistrer(texte, False)
    End Sub

    Public Sub UpdateTexteType(contexte As ContexteOperation, texte As TexteType)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Enregistrer(texte, True)
    End Sub

    ' Active ou désactive un texte type (jamais de suppression physique).
    Public Sub DefinirActivation(contexte As ContexteOperation, idTexteType As ULong, actif As Boolean)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryTextesTypes.DefinirActivationTexteType, conn)
                cmd.Parameters.AddWithValue("@id", idTexteType)
                cmd.Parameters.AddWithValue("@actif", actif)
                cmd.ExecuteNonQuery()
            End Using
        End Using

    End Sub

    Private Sub Enregistrer(texte As TexteType, existant As Boolean)

        Using conn = DatabaseManager.OpenConnection()
            Using tx = conn.BeginTransaction()

                Try
                    Using cmd As New MySqlCommand(If(existant, QueryTextesTypes.UpdateTexteType, QueryTextesTypes.InsertTexteType), conn, tx)

                        If existant Then cmd.Parameters.AddWithValue("@id", texte.IdTexteType)
                        cmd.Parameters.AddWithValue("@code", texte.CodeTexteType)
                        cmd.Parameters.AddWithValue("@libelle", texte.LibelleTexteType)
                        cmd.Parameters.AddWithValue("@rtf", If(String.IsNullOrEmpty(texte.ContenuRtf), CObj(DBNull.Value), texte.ContenuRtf))
                        cmd.Parameters.AddWithValue("@txt", If(String.IsNullOrEmpty(texte.ContenuTxt), CObj(DBNull.Value), texte.ContenuTxt))
                        cmd.Parameters.AddWithValue("@actif", texte.Actif)
                        cmd.Parameters.AddWithValue("@ordre", texte.OrdreAffichage)
                        cmd.ExecuteNonQuery()

                        If Not existant Then texte.IdTexteType = CULng(cmd.LastInsertedId)

                    End Using

                    Using cmd As New MySqlCommand(QueryTextesTypes.SupprimerUsagesTexteType, conn, tx)
                        cmd.Parameters.AddWithValue("@id", texte.IdTexteType)
                        cmd.ExecuteNonQuery()
                    End Using

                    For Each idUsage As ULong In texte.IdsUsages.Distinct()
                        Using cmd As New MySqlCommand(QueryTextesTypes.InsertUsageTexteType, conn, tx)
                            cmd.Parameters.AddWithValue("@id", texte.IdTexteType)
                            cmd.Parameters.AddWithValue("@idUsage", idUsage)
                            cmd.ExecuteNonQuery()
                        End Using
                    Next

                    tx.Commit()

                Catch
                    tx.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Sub

#End Region

#Region "Usages"

    Public Function GetUsagesActifs() As List(Of UsageTexteType)
        Return LireUsages(QueryTextesTypes.SelectUsagesActifs)
    End Function

    Public Function GetUsages(afficherInactifs As Boolean) As List(Of UsageTexteType)
        Return LireUsages(If(afficherInactifs, QueryTextesTypes.SelectUsagesTous, QueryTextesTypes.SelectUsagesActifs))
    End Function

    Private Function LireUsages(requete As String) As List(Of UsageTexteType)

        Dim usages As New List(Of UsageTexteType)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(requete, conn)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        usages.Add(New UsageTexteType With {
                            .IdUsageTexteType = CULng(reader.GetInt64("id_usage_texte_type")),
                            .CodeUsage = reader.GetString("code_usage"),
                            .LibelleUsage = reader.GetString("libelle_usage"),
                            .Actif = reader.GetBoolean("actif"),
                            .EstSysteme = reader.GetBoolean("est_systeme"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage")
                        })
                    End While
                End Using
            End Using
        End Using

        Return usages

    End Function

    ' Libellé, ordre et activation d'un usage (son code est repris par l'application).
    Public Sub UpdateUsage(contexte As ContexteOperation, usage As UsageTexteType)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryTextesTypes.UpdateUsage, conn)
                cmd.Parameters.AddWithValue("@id", usage.IdUsageTexteType)
                cmd.Parameters.AddWithValue("@libelle", usage.LibelleUsage)
                cmd.Parameters.AddWithValue("@ordre", usage.OrdreAffichage)
                cmd.Parameters.AddWithValue("@actif", usage.Actif)
                ReglesReferentiels.ExecuterSurLigneModifiable(cmd)
            End Using
        End Using

    End Sub

#End Region

#Region "Vérifications"

    Public Function CodeTexteTypeExiste(code As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QueryTextesTypes.SelectCountCodeTexteType, "@code", code, idExclu) > 0
    End Function

    Public Function LibelleTexteTypeExiste(libelle As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QueryTextesTypes.SelectCountLibelleTexteType, "@libelle", libelle, idExclu) > 0
    End Function

    Public Function CodeUsageExiste(code As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QueryTextesTypes.SelectCountCodeUsage, "@code", code, idExclu) > 0
    End Function

    Public Function LibelleUsageExiste(libelle As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QueryTextesTypes.SelectCountLibelleUsage, "@libelle", libelle, idExclu) > 0
    End Function

    Private Function Compter(requete As String, nomParametre As String, valeur As String, idExclu As ULong) As Integer

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(requete, conn)
                cmd.Parameters.AddWithValue(nomParametre, valeur)
                cmd.Parameters.AddWithValue("@id", idExclu)
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using

    End Function

#End Region

End Module
