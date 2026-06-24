' -------------------------------------------------------------------------------------------------
' Module   : GestionParametres
' Projet   : Althéa
' Version  : V1.2
' Date     : 02/05/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les paramètres applicatifs (lecture depuis la base, transformation en objets métier).
'
' Remarques :
' - Utilise DatabaseManager pour l’accès DB
' - Utilise QueryParametres pour les requêtes SQL
' - Retourne des objets ParametreApplication
' -------------------------------------------------------------------------------------------------
Imports MySqlConnector

Public Module GestionParametres

#Region "Lecture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetParametresActifs
    ' Version    : V1.3.0
    ' Date       : 02/05/2026
    '
    ' Rôle       :
    ' Retourne la liste des paramètres actifs depuis la base de données,
    ' en appliquant le filtrage selon le mode d'accès.
    '
    ' Paramètres :
    ' - modeAcces : Mode d'accès courant (Admin ou SuperUser)
    '
    ' Retour     :
    ' - List(Of ParametreApplication) : Liste des paramètres actifs (actif = 1)
    '
    ' Remarques  :
    ' - Filtre les paramètres selon ModifiableUtilisateur si modeAcces = SuperUser
    ' - Utilise QueryParametres.SelectParametresActifs
    ' -------------------------------------------------------------------------------------------------
    Public Function GetParametresActifs(modeAcces As ModeAccesParametres) As List(Of ParametreApplication)

        ' Dim result As New List(Of ParametreApplication)

        Return GetParametresDepuisRequete(QueryParametres.SelectParametresActifs, modeAcces)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetParametres
    ' Version    : V1.5.1
    ' Date       : 02/05/2026
    '
    ' Rôle       :
    ' Charge les paramètres applicatifs selon le mode d'accès et l'option d'affichage des inactifs.
    '
    ' Paramètres :
    ' - modeAcces        : Mode d'accès courant (Admin ou SuperUser)
    ' - afficherInactifs : True pour afficher actifs + désactivés, False pour actifs uniquement
    '
    ' Retour     :
    ' - List(Of ParametreApplication) : Liste des paramètres applicatifs
    '
    ' Remarques  :
    ' - Si afficherInactifs = True, utilise SelectParametresTous
    ' - Si afficherInactifs = False, utilise SelectParametresActifs
    ' -------------------------------------------------------------------------------------------------
    Public Function GetParametres(modeAcces As ModeAccesParametres,
                              afficherInactifs As Boolean) As List(Of ParametreApplication)

        If afficherInactifs Then
            Return GetParametresDepuisRequete(QueryParametres.SelectParametresTous, modeAcces)
        End If

        Return GetParametresActifs(modeAcces)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetParametresDepuisRequete
    ' Version    : V1.0.0
    ' Date       : 02/05/2026
    '
    ' Rôle       :
    ' Exécute une requête SQL pour récupérer les paramètres applicatifs
    ' et applique le filtrage selon le mode d'accès.
    '
    ' Paramètres :
    ' - query     : Requête SQL à exécuter (SelectParametresActifs ou SelectParametresTous)
    ' - modeAcces : Mode d'accès courant (Admin ou SuperUser)
    '
    ' Retour     :
    ' - List(Of ParametreApplication) : Liste des paramètres mappés et filtrés
    '
    ' Remarques  :
    ' - Méthode privée, appelée par GetParametres et GetParametresActifs
    ' - Filtre les paramètres selon ModifiableUtilisateur si modeAcces = SuperUser
    ' -------------------------------------------------------------------------------------------------
    Private Function GetParametresDepuisRequete(query As String,
                                            modeAcces As ModeAccesParametres) As List(Of ParametreApplication)

        Dim result As New List(Of ParametreApplication)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()

                    While reader.Read()

                        Dim param As New ParametreApplication With {
                        .IdParametre = reader.GetInt32("id_parametre"),
                        .CodeParametre = reader.GetString("code_parametre"),
                        .CleParametre = reader.GetString("cle_parametre"),
                        .LibelleParametre = reader.GetString("libelle_parametre"),
                        .GroupeParametre = reader.GetString("groupe_parametre"),
                        .TypeValeur = reader.GetString("type_valeur"),
                        .ValeurParametre = If(reader.IsDBNull("valeur_parametre"), Nothing, reader("valeur_parametre").ToString()),
                        .DescriptionParametre = If(reader.IsDBNull("description_parametre"), Nothing, reader("description_parametre").ToString()),
                        .ModifiableUtilisateur = reader.GetBoolean("modifiable_utilisateur"),
                        .Actif = reader.GetBoolean("actif"),
                        .OrdreAffichage = reader.GetInt32("ordre_affichage")
                    }

                        ' 👉 Filtrage selon mode d'accès
                        If modeAcces = ModeAccesParametres.SuperUser AndAlso Not param.ModifiableUtilisateur Then
                            Continue While
                        End If

                        result.Add(param)

                    End While

                End Using
            End Using
        End Using

        Return result

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetValeurParametre
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Retourne la valeur d'un paramètre actif identifié par sa clé (accès programmatique direct).
    '
    ' Paramètres :
    ' - cle : Clé unique du paramètre à lire (cle_parametre, ex : "PATH_GENERAL")
    '
    ' Retour     :
    ' - String : Valeur du paramètre, ou Nothing si la clé est introuvable ou le paramètre inactif
    '
    ' Remarques  :
    ' - Utilise QueryParametres.SelectValeurParametreByCle (filtre actif = 1)
    ' - Lecture scalaire (ExecuteScalar), sans filtrage par mode d'accès
    ' - Destiné aux accès techniques (ex : racine de stockage des documents patients)
    ' -------------------------------------------------------------------------------------------------
    Public Function GetValeurParametre(cle As String) As String

        If String.IsNullOrWhiteSpace(cle) Then
            Return Nothing
        End If

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryParametres.SelectValeurParametreByCle, conn)

                cmd.Parameters.AddWithValue("@cle", cle)

                Dim resultat As Object = cmd.ExecuteScalar()

                If resultat Is Nothing OrElse Convert.IsDBNull(resultat) Then
                    Return Nothing
                End If

                Return resultat.ToString()

            End Using
        End Using

    End Function

#End Region

#Region "Ecriture"
    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateParametre
    ' Version    : V1.1.0
    ' Date       : 29/04/2026
    '
    ' Rôle       :
    ' Met à jour un paramètre dans la base de données.
    '
    ' Paramètres :
    ' - param : ParametreApplication avec toutes les propriétés remplies
    '
    ' Remarques  :
    ' - Utilise QueryParametres.UpdateParametre
    ' - Met à jour date_modification automatiquement (via requête SQL)
    ' - Ne modifie PAS id_parametre, code_parametre, cle_parametre
    ' -------------------------------------------------------------------------------------------------

    Public Sub UpdateParametre(param As ParametreApplication)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryParametres.UpdateParametre, conn)

                cmd.Parameters.AddWithValue("@id", param.IdParametre)
                cmd.Parameters.AddWithValue("@valeur", param.ValeurParametre)
                cmd.Parameters.AddWithValue("@libelle", param.LibelleParametre)
                cmd.Parameters.AddWithValue("@groupe", param.GroupeParametre)
                cmd.Parameters.AddWithValue("@type", param.TypeValeur)
                cmd.Parameters.AddWithValue("@description", param.DescriptionParametre)
                cmd.Parameters.AddWithValue("@modifiable", param.ModifiableUtilisateur)
                cmd.Parameters.AddWithValue("@actif", param.Actif)
                cmd.Parameters.AddWithValue("@ordre", param.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InsertParametre
    ' Version    : V1.2.0
    ' Date       : 29/04/2026
    '
    ' Rôle       :
    ' Insère un nouveau paramètre dans la base de données.
    '
    ' Paramètres :
    ' - param : ParametreApplication avec toutes les propriétés remplies
    '
    ' Remarques  :
    ' - Utilise QueryParametres.InsertParametre
    ' - id_parametre et code_parametre sont générés automatiquement par la base
    ' - cle_parametre doit être unique (vérifié avant l'appel via CleParametreExiste)
    ' -------------------------------------------------------------------------------------------------
    Public Sub InsertParametre(param As ParametreApplication)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryParametres.InsertParametre, conn)

                cmd.Parameters.AddWithValue("@cle", param.CleParametre)
                cmd.Parameters.AddWithValue("@libelle", param.LibelleParametre)
                cmd.Parameters.AddWithValue("@groupe", param.GroupeParametre)
                cmd.Parameters.AddWithValue("@type", param.TypeValeur)
                cmd.Parameters.AddWithValue("@valeur", param.ValeurParametre)
                cmd.Parameters.AddWithValue("@description", param.DescriptionParametre)
                cmd.Parameters.AddWithValue("@modifiable", param.ModifiableUtilisateur)
                cmd.Parameters.AddWithValue("@actif", param.Actif)
                cmd.Parameters.AddWithValue("@ordre", param.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DesactiverParametre
    ' Version    : V1.0.0
    ' Date       : 30/04/2026
    '
    ' Rôle       :
    ' Désactive un paramètre dans la base de données (soft-delete).
    '
    ' Paramètres :
    ' - idParametre : Identifiant du paramètre à désactiver (ULong)
    '
    ' Remarques  :
    ' - Utilise QueryParametres.DesactiverParametre
    ' - Passe actif à 0 sans supprimer physiquement la ligne
    ' - Met à jour date_modification automatiquement (via requête SQL)
    ' -------------------------------------------------------------------------------------------------
    Public Sub DesactiverParametre(idParametre As ULong)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryParametres.DesactiverParametre, conn)
                cmd.Parameters.AddWithValue("@id", idParametre)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CleParametreExiste
    ' Version    : V1.0.0
    ' Date       : 02/05/2026
    '
    ' Rôle       :
    ' Vérifie si une clé de paramètre existe déjà dans la base de données.
    '
    ' Paramètres :
    ' - cle     : Clé de paramètre à vérifier (String)
    ' - idExclu : Identifiant à exclure de la vérification (utile pour la mise à jour, optionnel)
    '
    ' Retour     :
    ' - Boolean : True si la clé existe, False sinon
    '
    ' Remarques  :
    ' - Utilise QueryParametres.SelectCountCleParametre
    ' - Utilisé pour garantir l'unicité des clés avant INSERT ou UPDATE
    ' -------------------------------------------------------------------------------------------------

    Public Function CleParametreExiste(cle As String,
                                   Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryParametres.SelectCountCleParametre, conn)

                cmd.Parameters.AddWithValue("@cle", cle)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

#End Region

End Module