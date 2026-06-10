' -------------------------------------------------------------------------------------------------
' Module   : GestionDomaines
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 09/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les domaines de prise en charge (lecture depuis la base, écriture, vérifications),
' à partir de la table référentielle ref_domaines.
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB
' - Utilise QueryDomaines pour les requêtes SQL
' - Retourne des objets Domaine
' - Suppression physique réservée aux domaines non utilisés ; sinon soft-delete (Actif = 0)
' -------------------------------------------------------------------------------------------------
Imports MySqlConnector

Public Module GestionDomaines

#Region "Lecture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetDomainesActifs
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Retourne la liste des domaines actifs depuis la base de données.
    '
    ' Retour     :
    ' - List(Of Domaine) : Liste des domaines actifs (actif = 1)
    '
    ' Remarques  :
    ' - Utilise QueryDomaines.SelectDomainesActifs
    ' - Utilisé notamment pour alimenter les listes déroulantes métier
    ' -------------------------------------------------------------------------------------------------
    Public Function GetDomainesActifs() As List(Of Domaine)

        Return GetDomainesDepuisRequete(QueryDomaines.SelectDomainesActifs)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetDomaines
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Charge les domaines selon l'option d'affichage des inactifs.
    '
    ' Paramètres :
    ' - afficherInactifs : True pour afficher actifs + désactivés, False pour actifs uniquement
    '
    ' Retour     :
    ' - List(Of Domaine) : Liste des domaines
    '
    ' Remarques  :
    ' - Si afficherInactifs = True, utilise SelectDomainesTous
    ' - Si afficherInactifs = False, utilise SelectDomainesActifs
    ' -------------------------------------------------------------------------------------------------
    Public Function GetDomaines(afficherInactifs As Boolean) As List(Of Domaine)

        If afficherInactifs Then
            Return GetDomainesDepuisRequete(QueryDomaines.SelectDomainesTous)
        End If

        Return GetDomainesActifs()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetDomainesDepuisRequete
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Exécute une requête SQL pour récupérer les domaines et les mappe en objets métier.
    '
    ' Paramètres :
    ' - query : Requête SQL à exécuter (SelectDomainesActifs ou SelectDomainesTous)
    '
    ' Retour     :
    ' - List(Of Domaine) : Liste des domaines mappés
    '
    ' Remarques  :
    ' - Méthode privée, appelée par GetDomaines et GetDomainesActifs
    ' -------------------------------------------------------------------------------------------------
    Private Function GetDomainesDepuisRequete(query As String) As List(Of Domaine)

        Dim result As New List(Of Domaine)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()

                    While reader.Read()

                        Dim dom As New Domaine With {
                            .IdDomaine = reader.GetInt64("id_domaine"),
                            .CodeDomaine = reader.GetString("code_domaine"),
                            .LibelleDomaine = reader.GetString("libelle_domaine"),
                            .Actif = reader.GetBoolean("actif"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage")
                        }

                        result.Add(dom)

                    End While

                End Using
            End Using
        End Using

        Return result

    End Function

#End Region

#Region "Ecriture"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateDomaine
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Met à jour un domaine dans la base de données.
    '
    ' Paramètres :
    ' - dom : Domaine avec toutes les propriétés remplies
    '
    ' Remarques  :
    ' - Utilise QueryDomaines.UpdateDomaine
    ' - Ne modifie PAS id_domaine
    ' - L'unicité du code et du libellé doit être vérifiée en amont
    ' -------------------------------------------------------------------------------------------------
    Public Sub UpdateDomaine(dom As Domaine)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryDomaines.UpdateDomaine, conn)

                cmd.Parameters.AddWithValue("@id", dom.IdDomaine)
                cmd.Parameters.AddWithValue("@code", dom.CodeDomaine)
                cmd.Parameters.AddWithValue("@libelle", dom.LibelleDomaine)
                cmd.Parameters.AddWithValue("@actif", dom.Actif)
                cmd.Parameters.AddWithValue("@ordre", dom.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InsertDomaine
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Insère un nouveau domaine dans la base de données.
    '
    ' Paramètres :
    ' - dom : Domaine avec toutes les propriétés remplies
    '
    ' Remarques  :
    ' - Utilise QueryDomaines.InsertDomaine
    ' - id_domaine est généré automatiquement par la base (AUTO_INCREMENT)
    ' - code_domaine et libelle_domaine doivent être uniques (vérifié avant l'appel)
    ' -------------------------------------------------------------------------------------------------
    Public Sub InsertDomaine(dom As Domaine)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryDomaines.InsertDomaine, conn)

                cmd.Parameters.AddWithValue("@code", dom.CodeDomaine)
                cmd.Parameters.AddWithValue("@libelle", dom.LibelleDomaine)
                cmd.Parameters.AddWithValue("@actif", dom.Actif)
                cmd.Parameters.AddWithValue("@ordre", dom.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DesactiverDomaine
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Désactive un domaine dans la base de données (soft-delete).
    '
    ' Paramètres :
    ' - idDomaine : Identifiant du domaine à désactiver (ULong)
    '
    ' Remarques  :
    ' - Utilise QueryDomaines.DesactiverDomaine
    ' - Passe actif à 0 sans supprimer physiquement la ligne
    ' - À privilégier lorsqu'un domaine est déjà utilisé (FK existantes)
    ' -------------------------------------------------------------------------------------------------
    Public Sub DesactiverDomaine(idDomaine As ULong)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryDomaines.DesactiverDomaine, conn)
                cmd.Parameters.AddWithValue("@id", idDomaine)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SupprimerDomaine
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Supprime physiquement un domaine dans la base de données.
    '
    ' Paramètres :
    ' - idDomaine : Identifiant du domaine à supprimer (ULong)
    '
    ' Remarques  :
    ' - Utilise QueryDomaines.SupprimerDomaine
    ' - À n'utiliser que pour un domaine NON utilisé (voir DomaineEstUtilise)
    ' - Si le domaine est référencé, privilégier DesactiverDomaine (soft-delete)
    ' -------------------------------------------------------------------------------------------------
    Public Sub SupprimerDomaine(idDomaine As ULong)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryDomaines.SupprimerDomaine, conn)
                cmd.Parameters.AddWithValue("@id", idDomaine)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CodeDomaineExiste
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Vérifie si un code de domaine existe déjà dans la base de données.
    '
    ' Paramètres :
    ' - code    : Code de domaine à vérifier (String)
    ' - idExclu : Identifiant à exclure de la vérification (utile pour la mise à jour, optionnel)
    '
    ' Retour     :
    ' - Boolean : True si le code existe, False sinon
    '
    ' Remarques  :
    ' - Utilise QueryDomaines.SelectCountCodeDomaine
    ' - Utilisé pour garantir l'unicité du code avant INSERT ou UPDATE
    ' -------------------------------------------------------------------------------------------------
    Public Function CodeDomaineExiste(code As String,
                                      Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryDomaines.SelectCountCodeDomaine, conn)

                cmd.Parameters.AddWithValue("@code", code)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : LibelleDomaineExiste
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Vérifie si un libellé de domaine existe déjà dans la base de données.
    '
    ' Paramètres :
    ' - libelle : Libellé de domaine à vérifier (String)
    ' - idExclu : Identifiant à exclure de la vérification (utile pour la mise à jour, optionnel)
    '
    ' Retour     :
    ' - Boolean : True si le libellé existe, False sinon
    '
    ' Remarques  :
    ' - Utilise QueryDomaines.SelectCountLibelleDomaine
    ' - Utilisé pour garantir l'unicité du libellé avant INSERT ou UPDATE
    ' -------------------------------------------------------------------------------------------------
    Public Function LibelleDomaineExiste(libelle As String,
                                         Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryDomaines.SelectCountLibelleDomaine, conn)

                cmd.Parameters.AddWithValue("@libelle", libelle)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : DomaineEstUtilise
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Vérifie si un domaine est référencé dans les tables qui en dépendent
    ' (dossiers et modeles_documents).
    '
    ' Paramètres :
    ' - idDomaine : Identifiant du domaine à vérifier (ULong)
    '
    ' Retour     :
    ' - Boolean : True si le domaine est utilisé, False sinon
    '
    ' Remarques  :
    ' - Utilise QueryDomaines.SelectCountUsageDomaine
    ' - Utilisé pour décider entre suppression physique (False) et soft-delete (True)
    ' -------------------------------------------------------------------------------------------------
    Public Function DomaineEstUtilise(idDomaine As ULong) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryDomaines.SelectCountUsageDomaine, conn)

                cmd.Parameters.AddWithValue("@id", idDomaine)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

#End Region

End Module
