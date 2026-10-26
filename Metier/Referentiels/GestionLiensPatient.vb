' -------------------------------------------------------------------------------------------------
' Module   : GestionLiensPatient
' Projet   : Althéa
' Version  : V1.2.1
' Date     : 25/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les liens de parenté/relation avec le patient (lecture depuis la base, écriture,
' vérifications), à partir de la table référentielle ref_liens_patient.
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB
' - Utilise QueryLiensPatient pour les requêtes SQL
' - Retourne des objets LienPatient
' - Suppression physique réservée aux liens patient non utilisés ; sinon soft-delete (Actif = 0)
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : chaque écriture reçoit le ContexteOperation et
'   commence par ReglesReferentiels.ExigerDroitEcriture (SuperUser) ; SupprimerLienPatient, plus
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

Public Module GestionLiensPatient

#Region "Lecture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetLiensPatientActifs
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Retourne la liste des liens patient actifs depuis la base de données.
    '
    ' Retour     :
    ' - List(Of LienPatient) : Liste des liens patient actifs (actif = 1)
    '
    ' Remarques  :
    ' - Utilise QueryLiensPatient.SelectLiensPatientActifs
    ' - Utilisé notamment pour alimenter les listes déroulantes métier
    ' -------------------------------------------------------------------------------------------------
    Public Function GetLiensPatientActifs() As List(Of LienPatient)

        Return GetLiensPatientDepuisRequete(QueryLiensPatient.SelectLiensPatientActifs)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetLiensPatient
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Charge les liens patient selon l'option d'affichage des inactifs.
    '
    ' Paramètres :
    ' - afficherInactifs : True pour afficher actifs + désactivés, False pour actifs uniquement
    '
    ' Retour     :
    ' - List(Of LienPatient) : Liste des liens patient
    '
    ' Remarques  :
    ' - Si afficherInactifs = True, utilise SelectLiensPatientTous
    ' - Si afficherInactifs = False, utilise SelectLiensPatientActifs
    ' -------------------------------------------------------------------------------------------------
    Public Function GetLiensPatient(afficherInactifs As Boolean) As List(Of LienPatient)

        If afficherInactifs Then
            Return GetLiensPatientDepuisRequete(QueryLiensPatient.SelectLiensPatientTous)
        End If

        Return GetLiensPatientActifs()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetLiensPatientDepuisRequete
    ' Version    : V1.0.1
    ' Date       : 25/10/2026
    '
    ' Rôle       :
    ' Exécute une requête SQL pour récupérer les liens patient et les mappe en objets métier.
    '
    ' Paramètres :
    ' - query : Requête SQL à exécuter (SelectLiensPatientActifs ou SelectLiensPatientTous)
    '
    ' Retour     :
    ' - List(Of LienPatient) : Liste des liens patient mappés
    '
    ' Remarques  :
    ' - Méthode privée, appelée par GetLiensPatient et GetLiensPatientActifs
    ' - V1.0.1 (25/10/2026, audit D-1) : identifiant lu par GetUInt64 (colonne BIGINT UNSIGNED,
    '   propriété ULong).
    ' -------------------------------------------------------------------------------------------------
    Private Function GetLiensPatientDepuisRequete(query As String) As List(Of LienPatient)

        Dim result As New List(Of LienPatient)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()

                    While reader.Read()

                        Dim lien As New LienPatient With {
                            .IdLienPatient = reader.GetUInt64("id_lien_patient"),
                            .CodeLienPatient = reader.GetString("code_lien_patient"),
                            .LibelleLienPatient = reader.GetString("libelle_lien_patient"),
                            .Actif = reader.GetBoolean("actif"),
                            .EstSysteme = UtilsDataReader.LireBooleenOptionnel(reader, "est_systeme"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage"),
                            .ParDefaut = UtilsDataReader.LireBooleenOptionnel(reader, "par_defaut")
                        }

                        result.Add(lien)

                    End While

                End Using
            End Using
        End Using

        Return result

    End Function

#End Region

#Region "Ecriture"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateLienPatient
    ' Version    : V1.1.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Met à jour un lien patient dans la base de données.
    '
    ' Paramètres :
    ' - lien : LienPatient avec toutes les propriétés remplies
    '
    ' Remarques  :
    ' - Utilise QueryLiensPatient.UpdateLienPatient
    ' - Ne modifie PAS id_lien_patient
    ' - L'unicité du code et du libellé doit être vérifiée en amont
    ' - V1.1.0 (22/10/2026, audit C.1) : reçoit le ContexteOperation ; droit d'écriture vérifié par le métier.
    ' -------------------------------------------------------------------------------------------------
    Public Sub UpdateLienPatient(contexte As ContexteOperation, lien As LienPatient)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        ' Une seule valeur par défaut : les autres sont décochées dans la même transaction (v2.57.0).
        DatabaseManager.ExecuterEnTransaction(
            Sub(conn, tx)

                If lien.ParDefaut Then
                    Using cmdDefaut As New MySqlCommand(QueryLiensPatient.RetirerParDefautSaufCode, conn, tx)
                        cmdDefaut.Parameters.AddWithValue("@code", lien.CodeLienPatient)
                        cmdDefaut.ExecuteNonQuery()
                    End Using
                End If

                Using cmd As New MySqlCommand(QueryLiensPatient.UpdateLienPatient, conn, tx)
                    cmd.Parameters.AddWithValue("@id", lien.IdLienPatient)
                    cmd.Parameters.AddWithValue("@code", lien.CodeLienPatient)
                    cmd.Parameters.AddWithValue("@libelle", lien.LibelleLienPatient)
                    cmd.Parameters.AddWithValue("@actif", lien.Actif)
                    cmd.Parameters.AddWithValue("@ordre", lien.OrdreAffichage)
                    cmd.Parameters.AddWithValue("@par_defaut", lien.ParDefaut)

                    ReglesReferentiels.ExecuterSurLigneModifiable(cmd)

                End Using

            End Sub)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InsertLienPatient
    ' Version    : V1.1.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Insère un nouveau lien patient dans la base de données.
    '
    ' Paramètres :
    ' - lien : LienPatient avec toutes les propriétés remplies
    '
    ' Remarques  :
    ' - Utilise QueryLiensPatient.InsertLienPatient
    ' - id_lien_patient est généré automatiquement par la base (AUTO_INCREMENT)
    ' - code_lien_patient et libelle_lien_patient doivent être uniques (vérifié avant l'appel)
    ' - V1.1.0 (22/10/2026, audit C.1) : reçoit le ContexteOperation ; droit d'écriture vérifié par le métier.
    ' -------------------------------------------------------------------------------------------------
    Public Sub InsertLienPatient(contexte As ContexteOperation, lien As LienPatient)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        ' Une seule valeur par défaut : les autres sont décochées dans la même transaction (v2.57.0).
        DatabaseManager.ExecuterEnTransaction(
            Sub(conn, tx)

                If lien.ParDefaut Then
                    Using cmdDefaut As New MySqlCommand(QueryLiensPatient.RetirerParDefautSaufCode, conn, tx)
                        cmdDefaut.Parameters.AddWithValue("@code", lien.CodeLienPatient)
                        cmdDefaut.ExecuteNonQuery()
                    End Using
                End If

                Using cmd As New MySqlCommand(QueryLiensPatient.InsertLienPatient, conn, tx)
                    cmd.Parameters.AddWithValue("@code", lien.CodeLienPatient)
                    cmd.Parameters.AddWithValue("@libelle", lien.LibelleLienPatient)
                    cmd.Parameters.AddWithValue("@actif", lien.Actif)
                    cmd.Parameters.AddWithValue("@ordre", lien.OrdreAffichage)
                    cmd.Parameters.AddWithValue("@par_defaut", lien.ParDefaut)

                    cmd.ExecuteNonQuery()

                End Using

            End Sub)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DesactiverLienPatient
    ' Version    : V1.1.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Désactive un lien patient dans la base de données (soft-delete).
    '
    ' Paramètres :
    ' - idLienPatient : Identifiant du lien patient à désactiver (ULong)
    '
    ' Remarques  :
    ' - Utilise QueryLiensPatient.DesactiverLienPatient
    ' - Passe actif à 0 sans supprimer physiquement la ligne
    ' - À privilégier lorsqu'un lien patient est déjà utilisé (FK existantes)
    ' - V1.1.0 (22/10/2026, audit C.1) : reçoit le ContexteOperation ; droit d'écriture vérifié par le métier.
    ' -------------------------------------------------------------------------------------------------
    Public Sub DesactiverLienPatient(contexte As ContexteOperation, idLienPatient As ULong)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiensPatient.DesactiverLienPatient, conn)
                cmd.Parameters.AddWithValue("@id", idLienPatient)
                ReglesReferentiels.ExecuterSurLigneModifiable(cmd)
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CodeLienPatientExiste
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Vérifie si un code de lien patient existe déjà dans la base de données.
    '
    ' Paramètres :
    ' - code    : Code de lien patient à vérifier (String)
    ' - idExclu : Identifiant à exclure de la vérification (utile pour la mise à jour, optionnel)
    '
    ' Retour     :
    ' - Boolean : True si le code existe, False sinon
    '
    ' Remarques  :
    ' - Utilise QueryLiensPatient.SelectCountCodeLienPatient
    ' - Utilisé pour garantir l'unicité du code avant INSERT ou UPDATE
    ' -------------------------------------------------------------------------------------------------
    Public Function CodeLienPatientExiste(code As String,
                                          Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiensPatient.SelectCountCodeLienPatient, conn)

                cmd.Parameters.AddWithValue("@code", code)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : LibelleLienPatientExiste
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Vérifie si un libellé de lien patient existe déjà dans la base de données.
    '
    ' Paramètres :
    ' - libelle : Libellé de lien patient à vérifier (String)
    ' - idExclu : Identifiant à exclure de la vérification (utile pour la mise à jour, optionnel)
    '
    ' Retour     :
    ' - Boolean : True si le libellé existe, False sinon
    '
    ' Remarques  :
    ' - Utilise QueryLiensPatient.SelectCountLibelleLienPatient
    ' - Utilisé pour garantir l'unicité du libellé avant INSERT ou UPDATE
    ' -------------------------------------------------------------------------------------------------
    Public Function LibelleLienPatientExiste(libelle As String,
                                             Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiensPatient.SelectCountLibelleLienPatient, conn)

                cmd.Parameters.AddWithValue("@libelle", libelle)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : LienPatientEstUtilise
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Vérifie si un lien patient est référencé dans les tables qui en dépendent
    ' (famille_contacts).
    '
    ' Paramètres :
    ' - idLienPatient : Identifiant du lien patient à vérifier (ULong)
    '
    ' Retour     :
    ' - Boolean : True si le lien patient est utilisé, False sinon
    '
    ' Remarques  :
    ' - Utilise QueryLiensPatient.SelectCountUsageLienPatient
    ' - Utilisé pour décider entre suppression physique (False) et soft-delete (True)
    ' -------------------------------------------------------------------------------------------------
    Public Function LienPatientEstUtilise(idLienPatient As ULong) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiensPatient.SelectCountUsageLienPatient, conn)

                cmd.Parameters.AddWithValue("@id", idLienPatient)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

#End Region

End Module
