' -------------------------------------------------------------------------------------------------
' Module   : GestionParametres
' Projet   : Althéa
' Version  : V1.6.1
' Date     : 25/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les paramètres applicatifs (lecture depuis la base, transformation en objets métier).
'
' Remarques :
' - Utilise DatabaseManager pour l’accès DB
' - Utilise QueryParametres pour les requêtes SQL
' - Retourne des objets ParametreApplication
' - Seules écritures : la valeur des paramètres (UpdateValeursParametres, en transaction, pour l'écran
'   Paramètres ; UpdateValeurParametreParCle pour un écran métier) ; les paramètres naissent par
'   migration (ADR-36)
' - V1.5.0 (16/10/2026) : titre et ordre du groupe lus avec chaque paramètre (v2.40.0)
' - Lecture typée pour le code (GetEntier, GetBooleen, GetTexte) : bornes appliquées, valeur de
'   secours (ClesParametres) et journalisation si le paramètre est absent, inactif, invalide ou
'   illisible
' - V1.6.0 (19/10/2026) : GetTexte (mention de confidentialité des documents, v2.41.0)
' - V1.6.1 (25/10/2026, audit D-1) : Option Strict On ; identifiant lu par GetUInt64 (colonne BIGINT
'   UNSIGNED, propriété ULong).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

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
    ' Version    : V1.0.1
    ' Date       : 25/10/2026
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
    ' - V1.0.1 (25/10/2026, audit D-1) : identifiant lu par GetUInt64 (colonne BIGINT UNSIGNED,
    '   propriété ULong).
    ' -------------------------------------------------------------------------------------------------
    Private Function GetParametresDepuisRequete(query As String,
                                            modeAcces As ModeAccesParametres) As List(Of ParametreApplication)

        Dim result As New List(Of ParametreApplication)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()

                    While reader.Read()

                        Dim param As New ParametreApplication With {
                        .IdParametre = reader.GetUInt64("id_parametre"),
                        .CodeParametre = reader.GetString("code_parametre"),
                        .CleParametre = reader.GetString("cle_parametre"),
                        .LibelleParametre = reader.GetString("libelle_parametre"),
                        .GroupeParametre = reader.GetString("groupe_parametre"),
                        .LibelleGroupe = reader.GetString("libelle_groupe"),
                        .OrdreGroupe = reader.GetInt32("ordre_groupe"),
                        .TypeValeur = reader.GetString("type_valeur"),
                        .ValeurParametre = If(reader.IsDBNull("valeur_parametre"), Nothing, reader("valeur_parametre").ToString()),
                        .ValeurMin = If(reader.IsDBNull("valeur_min"), CType(Nothing, Decimal?), reader.GetDecimal("valeur_min")),
                        .ValeurMax = If(reader.IsDBNull("valeur_max"), CType(Nothing, Decimal?), reader.GetDecimal("valeur_max")),
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

#Region "Lecture typée"

    ' Valeur brute et bornes d'un paramètre actif.
    Private Class ValeurBornee
        Public Valeur As String
        Public Min As Decimal?
        Public Max As Decimal?
    End Class

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetEntier
    ' Version    : V1.0.0
    ' Date       : 12/10/2026
    '
    ' Rôle       :
    ' Lit un paramètre entier actif, ramené dans ses bornes (valeur_min / valeur_max).
    '
    ' Paramètres :
    ' - cle             : clé du paramètre (ClesParametres)
    ' - valeurParDefaut : valeur de secours (ClesParametres)
    '
    ' Remarques  :
    ' - Base injoignable, paramètre absent, inactif ou non entier : valeur de secours, journalisée,
    '   pour qu'un paramètre cassé se voie dans les logs sans bloquer l'écran qui l'utilise.
    ' - Une valeur hors bornes (modifiée directement en base) est ramenée dans les bornes.
    ' -------------------------------------------------------------------------------------------------
    Public Function GetEntier(cle As String, valeurParDefaut As Integer) As Integer

        Try
            Dim lecture As ValeurBornee = LireValeurEtBornes(cle)
            If lecture Is Nothing Then
                SignalerRepli(cle, "absent ou inactif", valeurParDefaut.ToString())
                Return valeurParDefaut
            End If

            Dim valeur As Integer
            If Not Integer.TryParse(lecture.Valeur, Globalization.NumberStyles.Integer,
                                    Globalization.CultureInfo.InvariantCulture, valeur) Then
                SignalerRepli(cle, $"valeur non entière « {lecture.Valeur} »", valeurParDefaut.ToString())
                Return valeurParDefaut
            End If

            If lecture.Min.HasValue AndAlso valeur < lecture.Min.Value Then
                Dim borneBasse As Integer = CInt(Math.Ceiling(lecture.Min.Value))
                SignalerRepli(cle, $"{valeur} sous la borne basse", borneBasse.ToString())
                Return borneBasse
            End If

            If lecture.Max.HasValue AndAlso valeur > lecture.Max.Value Then
                Dim borneHaute As Integer = CInt(Math.Floor(lecture.Max.Value))
                SignalerRepli(cle, $"{valeur} au-dessus de la borne haute", borneHaute.ToString())
                Return borneHaute
            End If

            Return valeur

        Catch ex As Exception
            GestionLog.EcrireLog($"Paramètre {cle} illisible : valeur de secours {valeurParDefaut} utilisée.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database, ex)
            Return valeurParDefaut
        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetBooleen
    ' Version    : V1.0.0
    ' Date       : 12/10/2026
    '
    ' Rôle       :
    ' Lit un paramètre booléen actif (1 / 0, true / false) ; même règle de repli que GetEntier.
    ' -------------------------------------------------------------------------------------------------
    Public Function GetBooleen(cle As String, valeurParDefaut As Boolean) As Boolean

        Try
            Dim lecture As ValeurBornee = LireValeurEtBornes(cle)
            If lecture Is Nothing Then
                SignalerRepli(cle, "absent ou inactif", valeurParDefaut.ToString())
                Return valeurParDefaut
            End If

            Dim valeur As String = If(lecture.Valeur, String.Empty).Trim()
            If valeur = "1" OrElse valeur.Equals("true", StringComparison.OrdinalIgnoreCase) Then Return True
            If valeur = "0" OrElse valeur.Equals("false", StringComparison.OrdinalIgnoreCase) Then Return False

            SignalerRepli(cle, $"valeur non booléenne « {valeur} »", valeurParDefaut.ToString())
            Return valeurParDefaut

        Catch ex As Exception
            GestionLog.EcrireLog($"Paramètre {cle} illisible : valeur de secours {valeurParDefaut} utilisée.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database, ex)
            Return valeurParDefaut
        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetTexte
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       :
    ' Lit un paramètre texte actif ; même règle de repli que GetEntier (absent, inactif ou illisible :
    ' valeur de secours, journalisée).
    '
    ' Remarques  : Une valeur vide est une valeur choisie (ex. pas de mention), rendue telle quelle.
    ' -------------------------------------------------------------------------------------------------
    Public Function GetTexte(cle As String, valeurParDefaut As String) As String

        Try
            Dim lecture As ValeurBornee = LireValeurEtBornes(cle)
            If lecture Is Nothing Then
                SignalerRepli(cle, "absent ou inactif", $"« {valeurParDefaut} »")
                Return valeurParDefaut
            End If

            Return If(lecture.Valeur, String.Empty)

        Catch ex As Exception
            GestionLog.EcrireLog($"Paramètre {cle} illisible : valeur de secours « {valeurParDefaut} » utilisée.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database, ex)
            Return valeurParDefaut
        End Try

    End Function

    ' Nothing si la clé est introuvable ou le paramètre inactif.
    Private Function LireValeurEtBornes(cle As String) As ValeurBornee

        If String.IsNullOrWhiteSpace(cle) Then Return Nothing

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryParametres.SelectValeurEtBornesByCle, conn)
                cmd.Parameters.AddWithValue("@cle", cle)
                Using reader = cmd.ExecuteReader()
                    If Not reader.Read() Then Return Nothing
                    Return New ValeurBornee With {
                        .Valeur = If(reader.IsDBNull("valeur_parametre"), Nothing, reader("valeur_parametre").ToString()),
                        .Min = If(reader.IsDBNull("valeur_min"), CType(Nothing, Decimal?), reader.GetDecimal("valeur_min")),
                        .Max = If(reader.IsDBNull("valeur_max"), CType(Nothing, Decimal?), reader.GetDecimal("valeur_max"))
                    }
                End Using
            End Using
        End Using

    End Function

    Private Sub SignalerRepli(cle As String, raison As String, valeurRetenue As String)
        GestionLog.EcrireLog($"Paramètre {cle} : {raison} ; valeur retenue : {valeurRetenue}.",
                             GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database)
    End Sub

#End Region

#Region "Ecriture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : UpdateValeursParametres
    ' Version    : V1.0.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Enregistre en une seule fois les nouvelles valeurs de plusieurs paramètres (écran Paramètres :
    ' un seul Enregistrer pour toute la page) ; seule écriture depuis l'écran Paramètres.
    '
    ' Paramètres :
    ' - parametres : paramètres dont ValeurParametre porte la nouvelle valeur (déjà validée par l'écran)
    ' - modeAcces  : Admin (tous les paramètres) ou SuperUser (paramètres ouverts seulement)
    '
    ' Retour     :
    ' - Boolean : True si toutes les valeurs sont enregistrées ; False si un paramètre est fermé au
    '   SuperUser ou introuvable — dans ce cas, AUCUNE valeur n'est enregistrée
    '
    ' Remarques  :
    ' - Une transaction : tout ou rien, la base ne garde jamais une page à moitié enregistrée.
    ' - La définition d'un paramètre (clé, libellé, groupe, type, bornes, description, activation,
    '   ouverture au SuperUser) vient de sa migration et n'est jamais modifiée ici (ADR-36).
    ' - Le droit du SuperUser est revérifié dans la requête, pas seulement par l'écran.
    ' - Une erreur de base remonte à l'appelant (transaction annulée), qui la journalise.
    ' - Remplace UpdateValeurParametre (un paramètre à la fois), retiré avec la refonte de l'écran.
    ' -------------------------------------------------------------------------------------------------
    Public Function UpdateValeursParametres(parametres As IEnumerable(Of ParametreApplication),
                                            modeAcces As ModeAccesParametres) As Boolean

        If parametres Is Nothing OrElse Not parametres.Any() Then Return True

        Using conn = DatabaseManager.OpenConnection()
            Using tx = conn.BeginTransaction()

                For Each parametre In parametres

                    Using cmd As New MySqlCommand(QueryParametres.UpdateValeurParametreModifiable, conn, tx)

                        cmd.Parameters.AddWithValue("@id", parametre.IdParametre)
                        cmd.Parameters.AddWithValue("@valeur", If(CObj(parametre.ValeurParametre), DBNull.Value))
                        cmd.Parameters.AddWithValue("@admin", If(modeAcces = ModeAccesParametres.Admin, 1, 0))

                        If cmd.ExecuteNonQuery() = 0 Then
                            tx.Rollback()
                            GestionLog.EcrireLog(
                                $"Paramètres : enregistrement annulé, paramètre {parametre.CleParametre} fermé à ce mode d'accès ou introuvable.",
                                GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database)
                            Return False
                        End If

                    End Using

                Next

                tx.Commit()

            End Using
        End Using

        Return True

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : UpdateValeurParametreParCle
    ' Version    : V1.0.0
    ' Date       : 12/10/2026
    '
    ' Rôle       :
    ' Comme UpdateValeursParametres pour un seul paramètre (actif) désigné par sa clé (ClesParametres) :
    ' réglage depuis un écran métier qui n'a pas l'identifiant (ex. dossier des sauvegardes).
    '
    ' Retour     :
    ' - Boolean : False si le paramètre est introuvable, inactif ou fermé au SuperUser
    '
    ' Remarques  :
    ' - La validation de la valeur reste à l'appelant (règle propre au paramètre).
    ' -------------------------------------------------------------------------------------------------
    Public Function UpdateValeurParametreParCle(cle As String,
                                                valeur As String,
                                                modeAcces As ModeAccesParametres) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryParametres.UpdateValeurParametreParCle, conn)

                cmd.Parameters.AddWithValue("@cle", cle)
                cmd.Parameters.AddWithValue("@valeur", If(CObj(valeur), DBNull.Value))
                cmd.Parameters.AddWithValue("@admin", If(modeAcces = ModeAccesParametres.Admin, 1, 0))

                Return cmd.ExecuteNonQuery() > 0

            End Using
        End Using

    End Function

#End Region

End Module