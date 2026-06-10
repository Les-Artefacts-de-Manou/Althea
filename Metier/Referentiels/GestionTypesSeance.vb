' -------------------------------------------------------------------------------------------------
' Module   : GestionTypesSeance
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 09/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les types de séance (lecture depuis la base, écriture, vérifications), à partir de la
' table référentielle ref_types_seance.
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB
' - Utilise QueryTypesSeance pour les requêtes SQL
' - Retourne des objets TypeSeance
' - Suppression physique réservée aux types de séance non utilisés ; sinon soft-delete (Actif = 0)
' - PARTICULARITÉ : gère le champ tarif_defaut (decimal(10,2) NOT NULL)
' -------------------------------------------------------------------------------------------------
Imports MySqlConnector

Public Module GestionTypesSeance

#Region "Lecture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetTypesSeanceActifs
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Retourne la liste des types de séance actifs depuis la base de données.
    '
    ' Retour     :
    ' - List(Of TypeSeance) : Liste des types de séance actifs (actif = 1)
    '
    ' Remarques  :
    ' - Utilise QueryTypesSeance.SelectTypesSeanceActifs
    ' - Utilisé notamment pour alimenter les listes déroulantes métier
    ' -------------------------------------------------------------------------------------------------
    Public Function GetTypesSeanceActifs() As List(Of TypeSeance)

        Return GetTypesSeanceDepuisRequete(QueryTypesSeance.SelectTypesSeanceActifs)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetTypesSeance
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Charge les types de séance selon l'option d'affichage des inactifs.
    '
    ' Paramètres :
    ' - afficherInactifs : True pour afficher actifs + désactivés, False pour actifs uniquement
    '
    ' Retour     :
    ' - List(Of TypeSeance) : Liste des types de séance
    '
    ' Remarques  :
    ' - Si afficherInactifs = True, utilise SelectTypesSeanceTous
    ' - Si afficherInactifs = False, utilise SelectTypesSeanceActifs
    ' -------------------------------------------------------------------------------------------------
    Public Function GetTypesSeance(afficherInactifs As Boolean) As List(Of TypeSeance)

        If afficherInactifs Then
            Return GetTypesSeanceDepuisRequete(QueryTypesSeance.SelectTypesSeanceTous)
        End If

        Return GetTypesSeanceActifs()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetTypesSeanceDepuisRequete
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Exécute une requête SQL pour récupérer les types de séance et les mappe en objets métier.
    '
    ' Paramètres :
    ' - query : Requête SQL à exécuter (SelectTypesSeanceActifs ou SelectTypesSeanceTous)
    '
    ' Retour     :
    ' - List(Of TypeSeance) : Liste des types de séance mappés
    '
    ' Remarques  :
    ' - Méthode privée, appelée par GetTypesSeance et GetTypesSeanceActifs
    ' -------------------------------------------------------------------------------------------------
    Private Function GetTypesSeanceDepuisRequete(query As String) As List(Of TypeSeance)

        Dim result As New List(Of TypeSeance)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()

                    While reader.Read()

                        Dim typeSeance As New TypeSeance With {
                            .IdTypeSeance = reader.GetInt64("id_type_seance"),
                            .CodeTypeSeance = reader.GetString("code_type_seance"),
                            .LibelleTypeSeance = reader.GetString("libelle_type_seance"),
                            .TarifDefaut = reader.GetDecimal("tarif_defaut"),
                            .Actif = reader.GetBoolean("actif"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage")
                        }

                        result.Add(typeSeance)

                    End While

                End Using
            End Using
        End Using

        Return result

    End Function

#End Region

#Region "Ecriture"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateTypeSeance
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Met à jour un type de séance dans la base de données.
    '
    ' Paramètres :
    ' - typeSeance : TypeSeance avec toutes les propriétés remplies
    '
    ' Remarques  :
    ' - Utilise QueryTypesSeance.UpdateTypeSeance
    ' - Ne modifie PAS id_type_seance
    ' - L'unicité du code et du libellé doit être vérifiée en amont
    ' -------------------------------------------------------------------------------------------------
    Public Sub UpdateTypeSeance(typeSeance As TypeSeance)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryTypesSeance.UpdateTypeSeance, conn)

                cmd.Parameters.AddWithValue("@id", typeSeance.IdTypeSeance)
                cmd.Parameters.AddWithValue("@code", typeSeance.CodeTypeSeance)
                cmd.Parameters.AddWithValue("@libelle", typeSeance.LibelleTypeSeance)
                cmd.Parameters.AddWithValue("@tarif", typeSeance.TarifDefaut)
                cmd.Parameters.AddWithValue("@actif", typeSeance.Actif)
                cmd.Parameters.AddWithValue("@ordre", typeSeance.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InsertTypeSeance
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Insère un nouveau type de séance dans la base de données.
    '
    ' Paramètres :
    ' - typeSeance : TypeSeance avec toutes les propriétés remplies
    '
    ' Remarques  :
    ' - Utilise QueryTypesSeance.InsertTypeSeance
    ' - id_type_seance est généré automatiquement par la base (AUTO_INCREMENT)
    ' - code_type_seance et libelle_type_seance doivent être uniques (vérifié avant l'appel)
    ' -------------------------------------------------------------------------------------------------
    Public Sub InsertTypeSeance(typeSeance As TypeSeance)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryTypesSeance.InsertTypeSeance, conn)

                cmd.Parameters.AddWithValue("@code", typeSeance.CodeTypeSeance)
                cmd.Parameters.AddWithValue("@libelle", typeSeance.LibelleTypeSeance)
                cmd.Parameters.AddWithValue("@tarif", typeSeance.TarifDefaut)
                cmd.Parameters.AddWithValue("@actif", typeSeance.Actif)
                cmd.Parameters.AddWithValue("@ordre", typeSeance.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DesactiverTypeSeance
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Désactive un type de séance dans la base de données (soft-delete).
    '
    ' Paramètres :
    ' - idTypeSeance : Identifiant du type de séance à désactiver (ULong)
    '
    ' Remarques  :
    ' - Utilise QueryTypesSeance.DesactiverTypeSeance
    ' - Passe actif à 0 sans supprimer physiquement la ligne
    ' - À privilégier lorsqu'un type de séance est déjà utilisé (FK existantes)
    ' -------------------------------------------------------------------------------------------------
    Public Sub DesactiverTypeSeance(idTypeSeance As ULong)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryTypesSeance.DesactiverTypeSeance, conn)
                cmd.Parameters.AddWithValue("@id", idTypeSeance)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SupprimerTypeSeance
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Supprime physiquement un type de séance dans la base de données.
    '
    ' Paramètres :
    ' - idTypeSeance : Identifiant du type de séance à supprimer (ULong)
    '
    ' Remarques  :
    ' - Utilise QueryTypesSeance.SupprimerTypeSeance
    ' - À n'utiliser que pour un type de séance NON utilisé (voir TypeSeanceEstUtilise)
    ' - Si le type de séance est référencé, privilégier DesactiverTypeSeance (soft-delete)
    ' -------------------------------------------------------------------------------------------------
    Public Sub SupprimerTypeSeance(idTypeSeance As ULong)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryTypesSeance.SupprimerTypeSeance, conn)
                cmd.Parameters.AddWithValue("@id", idTypeSeance)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CodeTypeSeanceExiste
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Vérifie si un code de type de séance existe déjà dans la base de données.
    '
    ' Paramètres :
    ' - code    : Code de type de séance à vérifier (String)
    ' - idExclu : Identifiant à exclure de la vérification (utile pour la mise à jour, optionnel)
    '
    ' Retour     :
    ' - Boolean : True si le code existe, False sinon
    '
    ' Remarques  :
    ' - Utilise QueryTypesSeance.SelectCountCodeTypeSeance
    ' - Utilisé pour garantir l'unicité du code avant INSERT ou UPDATE
    ' -------------------------------------------------------------------------------------------------
    Public Function CodeTypeSeanceExiste(code As String,
                                         Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryTypesSeance.SelectCountCodeTypeSeance, conn)

                cmd.Parameters.AddWithValue("@code", code)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : LibelleTypeSeanceExiste
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Vérifie si un libellé de type de séance existe déjà dans la base de données.
    '
    ' Paramètres :
    ' - libelle : Libellé de type de séance à vérifier (String)
    ' - idExclu : Identifiant à exclure de la vérification (utile pour la mise à jour, optionnel)
    '
    ' Retour     :
    ' - Boolean : True si le libellé existe, False sinon
    '
    ' Remarques  :
    ' - Utilise QueryTypesSeance.SelectCountLibelleTypeSeance
    ' - Utilisé pour garantir l'unicité du libellé avant INSERT ou UPDATE
    ' -------------------------------------------------------------------------------------------------
    Public Function LibelleTypeSeanceExiste(libelle As String,
                                            Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryTypesSeance.SelectCountLibelleTypeSeance, conn)

                cmd.Parameters.AddWithValue("@libelle", libelle)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : TypeSeanceEstUtilise
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Vérifie si un type de séance est référencé dans les tables qui en dépendent (seances).
    '
    ' Paramètres :
    ' - idTypeSeance : Identifiant du type de séance à vérifier (ULong)
    '
    ' Retour     :
    ' - Boolean : True si le type de séance est utilisé, False sinon
    '
    ' Remarques  :
    ' - Utilise QueryTypesSeance.SelectCountUsageTypeSeance
    ' - Utilisé pour décider entre suppression physique (False) et soft-delete (True)
    ' -------------------------------------------------------------------------------------------------
    Public Function TypeSeanceEstUtilise(idTypeSeance As ULong) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryTypesSeance.SelectCountUsageTypeSeance, conn)

                cmd.Parameters.AddWithValue("@id", idTypeSeance)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

#End Region

End Module
