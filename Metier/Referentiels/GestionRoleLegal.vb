' -------------------------------------------------------------------------------------------------
' Module   : GestionRoleLegal
' Projet   : Althéa
' Version  : V1.2.1
' Date     : 25/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les rôles légaux des contacts de l'entourage du patient (lecture depuis la base, écriture,
' vérifications), à partir de la table référentielle ref_role_legal.
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB
' - Utilise QueryRoleLegal pour les requêtes SQL
' - Retourne des objets RoleLegal
' - Suppression physique réservée aux rôles légaux non utilisés ; sinon soft-delete (Actif = 0)
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : chaque écriture reçoit le ContexteOperation et
'   commence par ReglesReferentiels.ExigerDroitEcriture (SuperUser) ; SupprimerRoleLegal, plus
'   appelée, supprimée.
'   Lignes système : refus explicite si la requête n'a concerné aucune ligne (ExecuterSurLigneModifiable).
' - V1.2.0 (24/10/2026, audit C.3 étape 2, v2.57.0) : ParDefaut lu et enregistré ; une seule valeur par défaut, les autres décochées dans la
'   transaction de l'enregistrement.
' - V1.2.1 (25/10/2026, audit D-1) : Option Strict On ; identifiant lu par GetUInt64 (colonne BIGINT
'   UNSIGNED, propriété ULong).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionRoleLegal

#Region "Lecture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetRolesLegauxActifs
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Rôle       :
    ' Retourne la liste des rôles légaux actifs depuis la base de données.
    '
    ' Retour     :
    ' - List(Of RoleLegal) : Liste des rôles légaux actifs (actif = 1)
    '
    ' Remarques  :
    ' - Utilise QueryRoleLegal.SelectRolesLegauxActifs
    ' - Utilisé notamment pour alimenter les listes déroulantes métier
    ' -------------------------------------------------------------------------------------------------
    Public Function GetRolesLegauxActifs() As List(Of RoleLegal)

        Return GetRolesLegauxDepuisRequete(QueryRoleLegal.SelectRolesLegauxActifs)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetRolesLegaux
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Rôle       :
    ' Charge les rôles légaux selon l'option d'affichage des inactifs.
    '
    ' Paramètres :
    ' - afficherInactifs : True pour afficher actifs + désactivés, False pour actifs uniquement
    '
    ' Retour     :
    ' - List(Of RoleLegal) : Liste des rôles légaux
    '
    ' Remarques  :
    ' - Si afficherInactifs = True, utilise SelectRolesLegauxTous
    ' - Si afficherInactifs = False, utilise SelectRolesLegauxActifs
    ' -------------------------------------------------------------------------------------------------
    Public Function GetRolesLegaux(afficherInactifs As Boolean) As List(Of RoleLegal)

        If afficherInactifs Then
            Return GetRolesLegauxDepuisRequete(QueryRoleLegal.SelectRolesLegauxTous)
        End If

        Return GetRolesLegauxActifs()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetRolesLegauxDepuisRequete
    ' Version    : V1.0.1
    ' Date       : 25/10/2026
    '
    ' Rôle       :
    ' Exécute une requête SQL pour récupérer les rôles légaux et les mappe en objets métier.
    '
    ' Paramètres :
    ' - query : Requête SQL à exécuter (SelectRolesLegauxActifs ou SelectRolesLegauxTous)
    '
    ' Retour     :
    ' - List(Of RoleLegal) : Liste des rôles légaux mappés
    '
    ' Remarques  :
    ' - Méthode privée, appelée par GetRolesLegaux et GetRolesLegauxActifs
    ' - V1.0.1 (25/10/2026, audit D-1) : identifiant lu par GetUInt64 (colonne BIGINT UNSIGNED,
    '   propriété ULong).
    ' -------------------------------------------------------------------------------------------------
    Private Function GetRolesLegauxDepuisRequete(query As String) As List(Of RoleLegal)

        Dim result As New List(Of RoleLegal)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()

                    While reader.Read()

                        Dim role As New RoleLegal With {
                            .IdRoleLegal = reader.GetUInt64("id_role_legal"),
                            .CodeRoleLegal = reader.GetString("code_role_legal"),
                            .LibelleRoleLegal = reader.GetString("libelle_role_legal"),
                            .Actif = reader.GetBoolean("actif"),
                            .EstSysteme = UtilsDataReader.LireBooleenOptionnel(reader, "est_systeme"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage"),
                            .ParDefaut = UtilsDataReader.LireBooleenOptionnel(reader, "par_defaut")
                        }

                        result.Add(role)

                    End While

                End Using
            End Using
        End Using

        Return result

    End Function

#End Region

#Region "Ecriture"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateRoleLegal
    ' Version    : V1.1.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Met à jour un rôle légal dans la base de données.
    '
    ' Paramètres :
    ' - role : RoleLegal avec toutes les propriétés remplies
    '
    ' Remarques  :
    ' - Utilise QueryRoleLegal.UpdateRoleLegal
    ' - Ne modifie PAS id_role_legal
    ' - L'unicité du code et du libellé doit être vérifiée en amont
    ' - V1.1.0 (22/10/2026, audit C.1) : reçoit le ContexteOperation ; droit d'écriture vérifié par le métier.
    ' -------------------------------------------------------------------------------------------------
    Public Sub UpdateRoleLegal(contexte As ContexteOperation, role As RoleLegal)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        ' Une seule valeur par défaut : les autres sont décochées dans la même transaction (v2.57.0).
        DatabaseManager.ExecuterEnTransaction(
            Sub(conn, tx)

                If role.ParDefaut Then
                    Using cmdDefaut As New MySqlCommand(QueryRoleLegal.RetirerParDefautSaufCode, conn, tx)
                        cmdDefaut.Parameters.AddWithValue("@code", role.CodeRoleLegal)
                        cmdDefaut.ExecuteNonQuery()
                    End Using
                End If

                Using cmd As New MySqlCommand(QueryRoleLegal.UpdateRoleLegal, conn, tx)
                    cmd.Parameters.AddWithValue("@id", role.IdRoleLegal)
                    cmd.Parameters.AddWithValue("@code", role.CodeRoleLegal)
                    cmd.Parameters.AddWithValue("@libelle", role.LibelleRoleLegal)
                    cmd.Parameters.AddWithValue("@actif", role.Actif)
                    cmd.Parameters.AddWithValue("@ordre", role.OrdreAffichage)
                    cmd.Parameters.AddWithValue("@par_defaut", role.ParDefaut)

                    ReglesReferentiels.ExecuterSurLigneModifiable(cmd)

                End Using

            End Sub)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InsertRoleLegal
    ' Version    : V1.1.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Insère un nouveau rôle légal dans la base de données.
    '
    ' Paramètres :
    ' - role : RoleLegal avec toutes les propriétés remplies
    '
    ' Remarques  :
    ' - Utilise QueryRoleLegal.InsertRoleLegal
    ' - id_role_legal est généré automatiquement par la base (AUTO_INCREMENT)
    ' - code_role_legal et libelle_role_legal doivent être uniques (vérifié avant l'appel)
    ' - V1.1.0 (22/10/2026, audit C.1) : reçoit le ContexteOperation ; droit d'écriture vérifié par le métier.
    ' -------------------------------------------------------------------------------------------------
    Public Sub InsertRoleLegal(contexte As ContexteOperation, role As RoleLegal)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        ' Une seule valeur par défaut : les autres sont décochées dans la même transaction (v2.57.0).
        DatabaseManager.ExecuterEnTransaction(
            Sub(conn, tx)

                If role.ParDefaut Then
                    Using cmdDefaut As New MySqlCommand(QueryRoleLegal.RetirerParDefautSaufCode, conn, tx)
                        cmdDefaut.Parameters.AddWithValue("@code", role.CodeRoleLegal)
                        cmdDefaut.ExecuteNonQuery()
                    End Using
                End If

                Using cmd As New MySqlCommand(QueryRoleLegal.InsertRoleLegal, conn, tx)
                    cmd.Parameters.AddWithValue("@code", role.CodeRoleLegal)
                    cmd.Parameters.AddWithValue("@libelle", role.LibelleRoleLegal)
                    cmd.Parameters.AddWithValue("@actif", role.Actif)
                    cmd.Parameters.AddWithValue("@ordre", role.OrdreAffichage)
                    cmd.Parameters.AddWithValue("@par_defaut", role.ParDefaut)

                    cmd.ExecuteNonQuery()

                End Using

            End Sub)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DesactiverRoleLegal
    ' Version    : V1.1.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Désactive un rôle légal dans la base de données (soft-delete).
    '
    ' Paramètres :
    ' - idRoleLegal : Identifiant du rôle légal à désactiver (ULong)
    '
    ' Remarques  :
    ' - Utilise QueryRoleLegal.DesactiverRoleLegal
    ' - Passe actif à 0 sans supprimer physiquement la ligne
    ' - À privilégier lorsqu'un rôle légal est déjà utilisé (FK existantes)
    ' - V1.1.0 (22/10/2026, audit C.1) : reçoit le ContexteOperation ; droit d'écriture vérifié par le métier.
    ' -------------------------------------------------------------------------------------------------
    Public Sub DesactiverRoleLegal(contexte As ContexteOperation, idRoleLegal As ULong)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryRoleLegal.DesactiverRoleLegal, conn)
                cmd.Parameters.AddWithValue("@id", idRoleLegal)
                ReglesReferentiels.ExecuterSurLigneModifiable(cmd)
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CodeRoleLegalExiste
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Rôle       :
    ' Vérifie si un code de rôle légal existe déjà dans la base de données.
    '
    ' Paramètres :
    ' - code    : Code de rôle légal à vérifier (String)
    ' - idExclu : Identifiant à exclure de la vérification (utile pour la mise à jour, optionnel)
    '
    ' Retour     :
    ' - Boolean : True si le code existe, False sinon
    '
    ' Remarques  :
    ' - Utilise QueryRoleLegal.SelectCountCodeRoleLegal
    ' - Utilisé pour garantir l'unicité du code avant INSERT ou UPDATE
    ' -------------------------------------------------------------------------------------------------
    Public Function CodeRoleLegalExiste(code As String,
                                        Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryRoleLegal.SelectCountCodeRoleLegal, conn)

                cmd.Parameters.AddWithValue("@code", code)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : LibelleRoleLegalExiste
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Rôle       :
    ' Vérifie si un libellé de rôle légal existe déjà dans la base de données.
    '
    ' Paramètres :
    ' - libelle : Libellé de rôle légal à vérifier (String)
    ' - idExclu : Identifiant à exclure de la vérification (utile pour la mise à jour, optionnel)
    '
    ' Retour     :
    ' - Boolean : True si le libellé existe, False sinon
    '
    ' Remarques  :
    ' - Utilise QueryRoleLegal.SelectCountLibelleRoleLegal
    ' - Utilisé pour garantir l'unicité du libellé avant INSERT ou UPDATE
    ' -------------------------------------------------------------------------------------------------
    Public Function LibelleRoleLegalExiste(libelle As String,
                                           Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryRoleLegal.SelectCountLibelleRoleLegal, conn)

                cmd.Parameters.AddWithValue("@libelle", libelle)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : RoleLegalEstUtilise
    ' Version    : V1.0.0
    ' Date       : 14/06/2026
    '
    ' Rôle       :
    ' Vérifie si un rôle légal est référencé dans les tables qui en dépendent
    ' (famille_contacts).
    '
    ' Paramètres :
    ' - idRoleLegal : Identifiant du rôle légal à vérifier (ULong)
    '
    ' Retour     :
    ' - Boolean : True si le rôle légal est utilisé, False sinon
    '
    ' Remarques  :
    ' - Utilise QueryRoleLegal.SelectCountUsageRoleLegal
    ' - Utilisé pour décider entre suppression physique (False) et soft-delete (True)
    ' -------------------------------------------------------------------------------------------------
    Public Function RoleLegalEstUtilise(idRoleLegal As ULong) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryRoleLegal.SelectCountUsageRoleLegal, conn)

                cmd.Parameters.AddWithValue("@id", idRoleLegal)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

#End Region

End Module
