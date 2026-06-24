' -------------------------------------------------------------------------------------------------
' Module      : GestionTherapeutes
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 16/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Gestion métier des thérapeutes (table therapeutes), référentiel riche partagé par
' dossiers.id_therapeute_traitant et autres_suivis_patient.id_therapeute.
'
' Responsabilités :
' - Charger la liste des thérapeutes (actifs ou tous) et un thérapeute par identifiant
' - Créer, mettre à jour, désactiver (soft-delete) et supprimer (physiquement) un thérapeute
' - Vérifier l'unicité du couple nom + prénom et l'utilisation par d'autres tables
' - Mapper les DataReader vers les objets métier (Therapeute)
'
' Important   :
' - Aucun accès UI direct (pas de MessageBox)
' - Toute la logique SQL est déléguée à QueryTherapeutes
' - Les exceptions sont journalisées via GestionLog puis propagées (Throw)
' - Suppression physique réservée aux thérapeutes non utilisés ; sinon soft-delete (actif = 0)
'
' Architecture :
' UI → GestionTherapeutes → QueryTherapeutes → DatabaseManager → MariaDB
'
' Dépendances :
' - DatabaseManager (connexions DB)
' - QueryTherapeutes (requêtes SQL)
' - GestionLog (traçabilité)
' - Therapeute (objet métier)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionTherapeutes

#Region "Lecture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetTherapeutesActifs
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Rôle       :
    ' Retourne la liste des thérapeutes actifs depuis la base de données.
    '
    ' Retour     :
    ' - List(Of Therapeute) : Liste des thérapeutes actifs (actif = 1)
    '
    ' Remarques  :
    ' - Utilise QueryTherapeutes.SelectTherapeutesActifs
    ' - Utilisé notamment pour alimenter les listes déroulantes métier (combo Intervenants)
    ' -------------------------------------------------------------------------------------------------
    Public Function GetTherapeutesActifs() As List(Of Therapeute)

        Return GetTherapeutesDepuisRequete(QueryTherapeutes.SelectTherapeutesActifs)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetTherapeutes
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Rôle       :
    ' Charge les thérapeutes selon l'option d'affichage des inactifs.
    '
    ' Paramètres :
    ' - afficherInactifs : True pour afficher actifs + désactivés, False pour actifs uniquement
    '
    ' Retour     :
    ' - List(Of Therapeute) : Liste des thérapeutes
    '
    ' Remarques  :
    ' - Si afficherInactifs = True, utilise SelectTherapeutesTous
    ' - Si afficherInactifs = False, utilise SelectTherapeutesActifs
    ' -------------------------------------------------------------------------------------------------
    Public Function GetTherapeutes(afficherInactifs As Boolean) As List(Of Therapeute)

        If afficherInactifs Then
            Return GetTherapeutesDepuisRequete(QueryTherapeutes.SelectTherapeutesTous)
        End If

        Return GetTherapeutesActifs()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetTherapeuteById
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Rôle       :
    ' Charge un thérapeute complet depuis son identifiant.
    '
    ' Paramètres :
    ' - idTherapeute : Identifiant du thérapeute à charger
    '
    ' Retour     :
    ' - Therapeute : L'objet thérapeute, ou Nothing si introuvable
    '
    ' Remarques  :
    ' - Utilise QueryTherapeutes.SelectTherapeuteById
    ' -------------------------------------------------------------------------------------------------
    Public Function GetTherapeuteById(idTherapeute As Long) As Therapeute

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryTherapeutes.SelectTherapeuteById, conn)

                    cmd.Parameters.AddWithValue("@id_therapeute", idTherapeute)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        If reader.Read() Then
                            Return MapTherapeute(reader)
                        End If

                    End Using

                End Using

            End Using

            Return Nothing

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur GetTherapeuteById (id={idTherapeute}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetTherapeutesDepuisRequete
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Rôle       :
    ' Exécute une requête SQL pour récupérer les thérapeutes et les mappe en objets métier.
    '
    ' Paramètres :
    ' - query : Requête SQL à exécuter (SelectTherapeutesActifs ou SelectTherapeutesTous)
    '
    ' Retour     :
    ' - List(Of Therapeute) : Liste des thérapeutes mappés
    '
    ' Remarques  :
    ' - Méthode privée, appelée par GetTherapeutes et GetTherapeutesActifs
    ' -------------------------------------------------------------------------------------------------
    Private Function GetTherapeutesDepuisRequete(query As String) As List(Of Therapeute)

        Dim result As New List(Of Therapeute)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(query, conn)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        While reader.Read()
                            result.Add(MapTherapeute(reader))
                        End While

                    End Using

                End Using

            End Using

            Return result

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur GetTherapeutesDepuisRequete.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

#End Region

#Region "Écriture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CreateTherapeute
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Rôle       :
    ' Insère un nouveau thérapeute et retourne l'identifiant généré par la base.
    '
    ' Paramètres :
    ' - therapeute : Thérapeute à créer (Nom doit être renseigné)
    '
    ' Retour     :
    ' - Long : Identifiant du thérapeute créé, ou -1 en cas d'échec d'insertion
    '
    ' Remarques  :
    ' - Utilise QueryTherapeutes.InsertTherapeute puis SELECT LASTVAL(seq_therapeutes)
    ' - id_therapeute et code_therapeute sont générés par la base
    ' - L'unicité du couple nom + prénom doit être vérifiée en amont
    ' -------------------------------------------------------------------------------------------------
    Public Function CreateTherapeute(therapeute As Therapeute) As Long

        If therapeute Is Nothing Then
            Throw New ArgumentNullException(NameOf(therapeute))
        End If

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryTherapeutes.InsertTherapeute, conn)

                    AjouterParametresTherapeute(cmd, therapeute)

                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected <> 1 Then

                        GestionLog.EcrireLog(
                            $"Création thérapeute échouée : aucune ligne insérée (nom={therapeute.Nom}, prenom={therapeute.Prenom}).",
                            GestionLog.LogLevel.Succinct,
                            GestionLog.LogCategory.Database
                        )

                        Return -1

                    End If

                End Using

                Using cmdId As New MySqlCommand("SELECT LASTVAL(seq_therapeutes);", conn)

                    Dim idTherapeute As Long = Convert.ToInt64(cmdId.ExecuteScalar())

                    If idTherapeute <= 0 Then

                        GestionLog.EcrireLog(
                            $"Création thérapeute : ID invalide retourné (nom={therapeute.Nom}, prenom={therapeute.Prenom}).",
                            GestionLog.LogLevel.Succinct,
                            GestionLog.LogCategory.Database
                        )

                        Return -1

                    End If

                    therapeute.IdTherapeute = idTherapeute

                    GestionLog.EcrireLog(
                        $"Thérapeute créé avec succès (ID={idTherapeute}, {therapeute.Prenom} {therapeute.Nom}).",
                        GestionLog.LogLevel.Rapide,
                        GestionLog.LogCategory.Database
                    )

                    Return idTherapeute

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur CreateTherapeute.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateTherapeute
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Rôle       :
    ' Met à jour un thérapeute existant dans la base de données.
    '
    ' Paramètres :
    ' - therapeute : Thérapeute avec toutes les propriétés remplies (IdTherapeute > 0)
    '
    ' Remarques  :
    ' - Utilise QueryTherapeutes.UpdateTherapeute
    ' - Ne modifie ni id_therapeute ni code_therapeute
    ' - L'unicité du couple nom + prénom doit être vérifiée en amont
    ' -------------------------------------------------------------------------------------------------
    Public Sub UpdateTherapeute(therapeute As Therapeute)

        If therapeute Is Nothing Then
            Throw New ArgumentNullException(NameOf(therapeute))
        End If

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryTherapeutes.UpdateTherapeute, conn)

                    AjouterParametresTherapeute(cmd, therapeute)
                    cmd.Parameters.AddWithValue("@id_therapeute", therapeute.IdTherapeute)

                    cmd.ExecuteNonQuery()

                End Using

            End Using

            GestionLog.EcrireLog(
                $"Thérapeute mis à jour (ID={therapeute.IdTherapeute}, {therapeute.Prenom} {therapeute.Nom}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database
            )

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur UpdateTherapeute (id={therapeute.IdTherapeute}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DesactiverTherapeute
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Rôle       :
    ' Désactive un thérapeute dans la base de données (soft-delete).
    '
    ' Paramètres :
    ' - idTherapeute : Identifiant du thérapeute à désactiver
    '
    ' Remarques  :
    ' - Utilise QueryTherapeutes.DesactiverTherapeute
    ' - Passe actif à 0 sans supprimer physiquement la ligne
    ' - À privilégier lorsqu'un thérapeute est déjà utilisé (FK existantes)
    ' -------------------------------------------------------------------------------------------------
    Public Sub DesactiverTherapeute(idTherapeute As Long)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryTherapeutes.DesactiverTherapeute, conn)

                    cmd.Parameters.AddWithValue("@id", idTherapeute)

                    cmd.ExecuteNonQuery()

                End Using

            End Using

            GestionLog.EcrireLog(
                $"Thérapeute désactivé (ID={idTherapeute}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database
            )

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur DesactiverTherapeute (id={idTherapeute}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SupprimerTherapeute
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Rôle       :
    ' Supprime physiquement un thérapeute dans la base de données.
    '
    ' Paramètres :
    ' - idTherapeute : Identifiant du thérapeute à supprimer
    '
    ' Remarques  :
    ' - Utilise QueryTherapeutes.SupprimerTherapeute
    ' - À n'utiliser que pour un thérapeute NON utilisé (voir TherapeuteEstUtilise)
    ' - Si le thérapeute est référencé, privilégier DesactiverTherapeute (soft-delete)
    ' - La confirmation utilisateur doit être réalisée en amont par l'UI
    ' -------------------------------------------------------------------------------------------------
    Public Sub SupprimerTherapeute(idTherapeute As Long)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryTherapeutes.SupprimerTherapeute, conn)

                    cmd.Parameters.AddWithValue("@id", idTherapeute)

                    cmd.ExecuteNonQuery()

                End Using

            End Using

            GestionLog.EcrireLog(
                $"Thérapeute supprimé (ID={idTherapeute}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database
            )

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur SupprimerTherapeute (id={idTherapeute}).",
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
    ' Fonction   : TherapeuteExiste
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Rôle       :
    ' Vérifie si un thérapeute portant le même couple nom + prénom existe déjà.
    '
    ' Paramètres :
    ' - nom     : Nom du thérapeute à vérifier
    ' - prenom  : Prénom du thérapeute à vérifier (peut être vide)
    ' - idExclu : Identifiant à exclure de la vérification (utile pour la mise à jour, optionnel)
    '
    ' Retour     :
    ' - Boolean : True si le couple existe, False sinon
    '
    ' Remarques  :
    ' - Utilise QueryTherapeutes.SelectCountNomPrenom
    ' - Utilisé pour garantir l'unicité du couple nom + prénom avant INSERT ou UPDATE
    ' -------------------------------------------------------------------------------------------------
    Public Function TherapeuteExiste(nom As String,
                                     prenom As String,
                                     Optional idExclu As Long = 0) As Boolean

        Using conn As MySqlConnection = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryTherapeutes.SelectCountNomPrenom, conn)

                cmd.Parameters.AddWithValue("@nom", If(nom, String.Empty).Trim())
                cmd.Parameters.AddWithValue("@prenom", If(prenom, String.Empty).Trim())
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using

        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : TherapeuteEstUtilise
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Rôle       :
    ' Vérifie si un thérapeute est référencé dans les tables qui en dépendent
    ' (dossiers et autres_suivis_patient).
    '
    ' Paramètres :
    ' - idTherapeute : Identifiant du thérapeute à vérifier
    '
    ' Retour     :
    ' - Boolean : True si le thérapeute est utilisé, False sinon
    '
    ' Remarques  :
    ' - Utilise QueryTherapeutes.SelectCountUsageTherapeute
    ' - Utilisé pour décider entre suppression physique (False) et soft-delete (True)
    ' -------------------------------------------------------------------------------------------------
    Public Function TherapeuteEstUtilise(idTherapeute As Long) As Boolean

        Using conn As MySqlConnection = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryTherapeutes.SelectCountUsageTherapeute, conn)

                cmd.Parameters.AddWithValue("@id", idTherapeute)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using

        End Using

    End Function

#End Region

#Region "Helpers privés"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AjouterParametresTherapeute
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Rôle       :
    ' Ajoute les paramètres SQL communs aux requêtes INSERT et UPDATE d'un thérapeute.
    '
    ' Paramètres :
    ' - cmd        : Commande SQL à paramétrer
    ' - therapeute : Thérapeute source des valeurs
    '
    ' Remarques  :
    ' - Les chaînes vides sont converties en NULL (ValeurOuDBNull) pour les colonnes optionnelles
    ' - @id_therapeute n'est PAS ajouté ici (spécifique à l'UPDATE)
    ' -------------------------------------------------------------------------------------------------
    Private Sub AjouterParametresTherapeute(cmd As MySqlCommand, therapeute As Therapeute)

        cmd.Parameters.AddWithValue("@nom", If(therapeute.Nom, String.Empty).Trim())
        cmd.Parameters.AddWithValue("@prenom", ValeurOuDBNull(therapeute.Prenom))
        cmd.Parameters.AddWithValue("@specialite", ValeurOuDBNull(therapeute.Specialite))
        cmd.Parameters.AddWithValue("@telephone", ValeurOuDBNull(therapeute.Telephone))
        cmd.Parameters.AddWithValue("@email", ValeurOuDBNull(therapeute.Email))
        cmd.Parameters.AddWithValue("@adresse_ligne1", ValeurOuDBNull(therapeute.AdresseLigne1))
        cmd.Parameters.AddWithValue("@adresse_ligne2", ValeurOuDBNull(therapeute.AdresseLigne2))
        cmd.Parameters.AddWithValue("@code_postal", ValeurOuDBNull(therapeute.CodePostal))
        cmd.Parameters.AddWithValue("@localite", ValeurOuDBNull(therapeute.Localite))
        cmd.Parameters.AddWithValue("@pays", ValeurOuDBNull(therapeute.Pays))
        cmd.Parameters.AddWithValue("@commentaire_txt", ValeurOuDBNull(therapeute.Commentaire_txt))
        cmd.Parameters.AddWithValue("@commentaire_rtf", ValeurOuDBNull(therapeute.Commentaire_rtf))
        cmd.Parameters.AddWithValue("@actif", therapeute.Actif)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : MapTherapeute
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Rôle       :
    ' Mappe une ligne de DataReader vers un objet Therapeute complet.
    '
    ' Paramètres :
    ' - reader : DataReader positionné sur la ligne à mapper
    '
    ' Retour     :
    ' - Therapeute : Objet métier renseigné
    ' -------------------------------------------------------------------------------------------------
    Private Function MapTherapeute(reader As MySqlDataReader) As Therapeute

        Dim t As New Therapeute With {
            .IdTherapeute = Convert.ToInt64(reader("id_therapeute")),
            .CodeTherapeute = LireString(reader, "code_therapeute"),
            .Nom = LireString(reader, "nom"),
            .Prenom = LireString(reader, "prenom"),
            .Specialite = LireString(reader, "specialite"),
            .Telephone = LireString(reader, "telephone"),
            .Email = LireString(reader, "email"),
            .AdresseLigne1 = LireString(reader, "adresse_ligne1"),
            .AdresseLigne2 = LireString(reader, "adresse_ligne2"),
            .CodePostal = LireString(reader, "code_postal"),
            .Localite = LireString(reader, "localite"),
            .Pays = LireString(reader, "pays"),
            .Commentaire_txt = LireString(reader, "commentaire_txt"),
            .Commentaire_rtf = LireString(reader, "commentaire_rtf"),
            .Actif = LireBool(reader, "actif"),
            .DateCreation = CDate(reader("date_creation")),
            .DateModification = CDate(reader("date_modification"))
        }

        Return t

    End Function

#End Region

End Module
