' -------------------------------------------------------------------------------------------------
' Module      : GestionSuivisIntervenants
' Projet      : Althéa
' Version     : V1.3.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Gestion métier du réseau d'intervenants externes d'un patient (table autres_suivis_patient,
' liaison N-N — D-Q1bis, Option A actée).
'
' Responsabilités :
' - Charger la liste des suivis d'un patient et un suivi par identifiant
' - Créer, mettre à jour et supprimer (physiquement) un suivi
' - Valider la saisie d'un suivi (ValiderSuivi, audit C.2)
' - Mapper les DataReader vers les objets métier (SuiviIntervenant)
'
' Important   :
' - Aucun accès UI direct (pas de MessageBox)
' - Toute la logique SQL est déléguée à QuerySuivisIntervenants
' - Les exceptions sont journalisées via GestionLog puis propagées (Throw)
' - Le suivi référence obligatoirement un thérapeute du référentiel (id_therapeute) ; nom, spécialité
'   et lieu en sont une copie figée (trace)
'
' Architecture :
' UI → GestionSuivisIntervenants → QuerySuivisIntervenants → DatabaseManager → MariaDB
'
' Dépendances :
' - DatabaseManager (connexions DB)
' - QuerySuivisIntervenants (requêtes SQL)
' - GestionLog (traçabilité)
' - SuiviIntervenant (objet métier)
'
' Historique  :
' - V1.2.0 (20/10/2026) : plus aucun nom d'intervenant dans le journal, seulement des identifiants
'   (relevé avec l'audit B3 : un intervenant d'un patient est une donnée personnelle liée à lui).
' - V1.3.0 (23/10/2026, audit C.2 étape 2b) : ValiderSuivi (règles sorties d'IntervenantEdition,
'   ADR-51), appliqué par CreateSuivi et UpdateSuivi.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionSuivisIntervenants

#Region "Lecture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetSuivisParPatient
    ' Version    : V1.0.0
    ' Date       : 15/06/2026
    '
    ' Rôle       :
    ' Charge la liste des intervenants rattachés à un patient.
    '
    ' Paramètres :
    ' - idPatient : Identifiant du patient
    '
    ' Retour     :
    ' - List(Of SuiviIntervenant) : Suivis triés par date de début décroissante puis nom
    '
    ' Remarques  :
    ' - Utilise QuerySuivisIntervenants.SelectSuivisParPatient
    ' -------------------------------------------------------------------------------------------------
    Public Function GetSuivisParPatient(idPatient As Long) As List(Of SuiviIntervenant)

        Dim result As New List(Of SuiviIntervenant)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QuerySuivisIntervenants.SelectSuivisParPatient, conn)

                    cmd.Parameters.AddWithValue("@id_patient", idPatient)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        While reader.Read()
                            result.Add(MapSuivi(reader))
                        End While

                    End Using

                End Using

            End Using

            Return result

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur GetSuivisParPatient (id_patient={idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetSuiviById
    ' Version    : V1.0.0
    ' Date       : 15/06/2026
    '
    ' Rôle       :
    ' Charge un suivi complet depuis son identifiant.
    '
    ' Paramètres :
    ' - idAutreSuiviPatient : Identifiant du suivi à charger
    '
    ' Retour     :
    ' - SuiviIntervenant : L'objet suivi, ou Nothing si introuvable
    '
    ' Remarques  :
    ' - Utilise QuerySuivisIntervenants.SelectSuiviById
    ' -------------------------------------------------------------------------------------------------
    Public Function GetSuiviById(idAutreSuiviPatient As Long) As SuiviIntervenant

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QuerySuivisIntervenants.SelectSuiviById, conn)

                    cmd.Parameters.AddWithValue("@id_autre_suivi_patient", idAutreSuiviPatient)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        If reader.Read() Then
                            Return MapSuivi(reader)
                        End If

                    End Using

                End Using

            End Using

            Return Nothing

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur GetSuiviById (id={idAutreSuiviPatient}).",
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
    ' Fonction   : CreateSuivi
    ' Version    : V1.1.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Insère un nouveau suivi et retourne l'identifiant généré par la base.
    '
    ' Paramètres :
    ' - suivi : Suivi à créer (IdPatient doit être renseigné)
    '
    ' Retour     :
    ' - Long : Identifiant du suivi créé, ou -1 en cas d'échec d'insertion
    '
    ' Remarques  :
    ' - Utilise QuerySuivisIntervenants.InsertSuivi puis SELECT LASTVAL(seq_autres_suivis_patient)
    ' - id_autre_suivi_patient et code_autre_suivi_patient sont générés par la base
    ' - V1.1.0 (23/10/2026, audit C.2) : refus si ValiderSuivi trouve une erreur (ErreurValidationException).
    ' -------------------------------------------------------------------------------------------------
    Public Function CreateSuivi(suivi As SuiviIntervenant) As Long

        If suivi Is Nothing Then
            Throw New ArgumentNullException(NameOf(suivi))
        End If

        Try

            ErreurValidationException.LeverSiErreurs(ValiderSuivi(suivi))

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QuerySuivisIntervenants.InsertSuivi, conn)

                    AjouterParametresSuivi(cmd, suivi)

                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected <> 1 Then

                        GestionLog.EcrireLog(
                            $"Création suivi échouée : aucune ligne insérée (patient ID={suivi.IdPatient}).",
                            GestionLog.LogLevel.Succinct,
                            GestionLog.LogCategory.Database
                        )

                        Return -1

                    End If

                End Using

                Using cmdId As New MySqlCommand("SELECT LASTVAL(seq_autres_suivis_patient);", conn)

                    Dim idSuivi As Long = Convert.ToInt64(cmdId.ExecuteScalar())

                    If idSuivi <= 0 Then

                        GestionLog.EcrireLog(
                            $"Création suivi : ID invalide retourné (patient ID={suivi.IdPatient}).",
                            GestionLog.LogLevel.Succinct,
                            GestionLog.LogCategory.Database
                        )

                        Return -1

                    End If

                    suivi.IdAutreSuiviPatient = idSuivi

                    GestionLog.EcrireLog(
                        $"Suivi intervenant créé avec succès (ID={idSuivi}, patient ID={suivi.IdPatient}).",
                        GestionLog.LogLevel.Rapide,
                        GestionLog.LogCategory.Database
                    )

                    Return idSuivi

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur CreateSuivi.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateSuivi
    ' Version    : V1.1.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Met à jour un suivi existant dans la base de données.
    '
    ' Paramètres :
    ' - suivi : Suivi avec toutes les propriétés remplies (IdAutreSuiviPatient > 0)
    '
    ' Remarques  :
    ' - Utilise QuerySuivisIntervenants.UpdateSuivi
    ' - Ne modifie ni id_autre_suivi_patient ni code_autre_suivi_patient ni id_patient
    ' - V1.1.0 (23/10/2026, audit C.2) : refus si ValiderSuivi trouve une erreur (ErreurValidationException).
    ' -------------------------------------------------------------------------------------------------
    Public Sub UpdateSuivi(suivi As SuiviIntervenant)

        If suivi Is Nothing Then
            Throw New ArgumentNullException(NameOf(suivi))
        End If

        Try

            ErreurValidationException.LeverSiErreurs(ValiderSuivi(suivi))

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QuerySuivisIntervenants.UpdateSuivi, conn)

                    AjouterParametresSuivi(cmd, suivi)
                    cmd.Parameters.AddWithValue("@id_autre_suivi_patient", suivi.IdAutreSuiviPatient)

                    cmd.ExecuteNonQuery()

                End Using

            End Using

            GestionLog.EcrireLog(
                $"Suivi intervenant mis à jour (ID={suivi.IdAutreSuiviPatient}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database
            )

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur UpdateSuivi (id={suivi.IdAutreSuiviPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DeleteSuivi
    ' Version    : V1.0.0
    ' Date       : 15/06/2026
    '
    ' Rôle       :
    ' Supprime physiquement un suivi.
    '
    ' Paramètres :
    ' - idAutreSuiviPatient : Identifiant du suivi à supprimer
    '
    ' Remarques  :
    ' - Utilise QuerySuivisIntervenants.DeleteSuivi
    ' - La confirmation utilisateur doit être réalisée en amont par l'UI
    ' -------------------------------------------------------------------------------------------------
    Public Sub DeleteSuivi(idAutreSuiviPatient As Long)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QuerySuivisIntervenants.DeleteSuivi, conn)

                    cmd.Parameters.AddWithValue("@id_autre_suivi_patient", idAutreSuiviPatient)

                    cmd.ExecuteNonQuery()

                End Using

            End Using

            GestionLog.EcrireLog(
                $"Suivi intervenant supprimé (ID={idAutreSuiviPatient}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database
            )

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur DeleteSuivi (id={idAutreSuiviPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

#End Region

#Region "Validation"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ValiderSuivi
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Erreurs de saisie d'un suivi (liste vide s'il est valide) : thérapeute du référentiel obligatoire,
    ' date de fin jamais antérieure à la date de début (quand les deux sont renseignées).
    '
    ' Remarques  :
    ' - Sortie d'IntervenantEdition.ValiderSaisie (audit C.2, ADR-51) ; la fenêtre l'appelle pour ses
    '   champs, CreateSuivi et UpdateSuivi l'appliquent. Champs : NameOf de SuiviIntervenant.
    ' - Pas d'intervenant sans thérapeute : s'il n'existe pas réellement, on n'en crée pas (décision actée).
    ' -------------------------------------------------------------------------------------------------
    Public Function ValiderSuivi(suivi As SuiviIntervenant) As List(Of ErreurSaisie)

        If suivi Is Nothing Then Throw New ArgumentNullException(NameOf(suivi))

        Dim erreurs As New List(Of ErreurSaisie)

        If Not suivi.IdTherapeute.HasValue OrElse suivi.IdTherapeute.Value = 0UL Then
            erreurs.Add(New ErreurSaisie(NameOf(SuiviIntervenant.IdTherapeute), "Le thérapeute est obligatoire."))
        End If

        If suivi.DateDebut.HasValue AndAlso suivi.DateFin.HasValue AndAlso
           suivi.DateFin.Value.Date < suivi.DateDebut.Value.Date Then
            erreurs.Add(New ErreurSaisie(NameOf(SuiviIntervenant.DateFin), "La date de fin ne peut pas précéder la date de début."))
        End If

        Return erreurs

    End Function

#End Region

#Region "Helpers privés"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AjouterParametresSuivi
    ' Version    : V1.0.0
    ' Date       : 15/06/2026
    '
    ' Rôle       :
    ' Ajoute les paramètres SQL communs aux requêtes INSERT et UPDATE d'un suivi.
    '
    ' Paramètres :
    ' - cmd   : Commande SQL à paramétrer
    ' - suivi : Suivi source des valeurs
    '
    ' Remarques  :
    ' - Les chaînes vides sont converties en NULL (ValeurOuDBNull) pour les colonnes optionnelles
    ' - Les identifiants nullables (rôle, thérapeute) sont convertis en NULL si absents
    ' - @id_autre_suivi_patient n'est PAS ajouté ici (spécifique à l'UPDATE)
    ' -------------------------------------------------------------------------------------------------
    Private Sub AjouterParametresSuivi(cmd As MySqlCommand, suivi As SuiviIntervenant)

        cmd.Parameters.AddWithValue("@id_patient", suivi.IdPatient)
        cmd.Parameters.AddWithValue("@id_role_intervenant", ValeurULongOuDBNull(suivi.IdRoleIntervenant))
        cmd.Parameters.AddWithValue("@id_therapeute", ValeurULongOuDBNull(suivi.IdTherapeute))
        cmd.Parameters.AddWithValue("@id_civilite", ValeurULongOuDBNull(suivi.IdCivilite))
        cmd.Parameters.AddWithValue("@nom_professionnel", If(suivi.NomProfessionnel, String.Empty).Trim())
        cmd.Parameters.AddWithValue("@specialite", ValeurOuDBNull(suivi.Specialite))
        cmd.Parameters.AddWithValue("@lieu", ValeurOuDBNull(suivi.Lieu))
        cmd.Parameters.AddWithValue("@date_debut", ValeurDateOuDBNull(suivi.DateDebut))
        cmd.Parameters.AddWithValue("@date_fin", ValeurDateOuDBNull(suivi.DateFin))
        cmd.Parameters.AddWithValue("@commentaire_rtf", ValeurOuDBNull(suivi.CommentaireRtf))
        cmd.Parameters.AddWithValue("@commentaire_txt", ValeurOuDBNull(suivi.CommentaireTxt))

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : MapSuivi
    ' Version    : V1.0.0
    ' Date       : 15/06/2026
    '
    ' Rôle       :
    ' Mappe une ligne de DataReader vers un objet SuiviIntervenant complet.
    '
    ' Paramètres :
    ' - reader : DataReader positionné sur la ligne à mapper
    '
    ' Retour     :
    ' - SuiviIntervenant : Objet métier renseigné
    ' -------------------------------------------------------------------------------------------------
    Private Function MapSuivi(reader As MySqlDataReader) As SuiviIntervenant

        Dim s As New SuiviIntervenant With {
            .IdAutreSuiviPatient = Convert.ToInt64(reader("id_autre_suivi_patient")),
            .CodeAutreSuiviPatient = LireString(reader, "code_autre_suivi_patient"),
            .IdPatient = Convert.ToInt64(reader("id_patient")),
            .IdRoleIntervenant = LireULongNullable(reader, "id_role_intervenant"),
            .LibelleRoleIntervenant = LireString(reader, "libelle_role_intervenant"),
            .IdTherapeute = LireULongNullable(reader, "id_therapeute"),
            .IdCivilite = LireULongNullable(reader, "id_civilite"),
            .LibelleCivilite = LireString(reader, "libelle_civilite"),
            .NomProfessionnel = LireString(reader, "nom_professionnel"),
            .Specialite = LireString(reader, "specialite"),
            .Lieu = LireString(reader, "lieu"),
            .DateDebut = LireDateNullable(reader, "date_debut"),
            .DateFin = LireDateNullable(reader, "date_fin"),
            .CommentaireRtf = LireString(reader, "commentaire_rtf"),
            .CommentaireTxt = LireString(reader, "commentaire_txt"),
            .DateCreation = CDate(reader("date_creation")),
            .DateModification = CDate(reader("date_modification"))
        }

        Return s

    End Function

#End Region

End Module
