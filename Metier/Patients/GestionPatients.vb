' -------------------------------------------------------------------------------------------------
' Module      : GestionPatients
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 11/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Gestion métier des patients (lecture, écriture, recherche, validations).
'
' Responsabilités :
' - Charger la liste des patients (avec compteurs de dossiers) et un patient par identifiant
' - Créer, mettre à jour et supprimer un patient
' - Mettre à jour la photo d'identité
' - Vérifier l'unicité du NISS et détecter les doublons (nom + prénom + date de naissance)
' - Mapper les DataReader vers les objets métier (Patient, PatientListeItem)
'
' Important   :
' - Aucun accès UI direct (pas de MessageBox)
' - Toute la logique SQL est déléguée à QueryPatients
' - Les exceptions sont journalisées via GestionLog puis propagées (Throw)
'
' Architecture :
' UI → GestionPatients → QueryPatients → DatabaseManager → MariaDB
'
' Dépendances :
' - DatabaseManager (connexions DB)
' - QueryPatients (requêtes SQL)
' - GestionLog (traçabilité)
' - Patient / PatientListeItem (objets métier)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionPatients

#Region "Lecture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetPatients
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Charge la liste des patients pour l'écran d'accueil/recherche, avec compteurs de dossiers.
    '
    ' Retour     :
    ' - List(Of PatientListeItem) : Patients triés par date de modification décroissante
    '
    ' Remarques  :
    ' - Utilise QueryPatients.SelectPatientsListe
    ' - Le filtrage multi-critères est réalisé en mémoire par l'UI (petite volumétrie en V1)
    ' -------------------------------------------------------------------------------------------------
    Public Function GetPatients() As List(Of PatientListeItem)

        Dim result As New List(Of PatientListeItem)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryPatients.SelectPatientsListe, conn)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        While reader.Read()
                            result.Add(MapListeItem(reader))
                        End While

                    End Using

                End Using

            End Using

            Return result

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur GetPatients.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetPatientById
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Charge un patient complet depuis son identifiant.
    '
    ' Paramètres :
    ' - idPatient : Identifiant du patient à charger
    '
    ' Retour     :
    ' - Patient : L'objet patient, ou Nothing si introuvable
    '
    ' Remarques  :
    ' - Utilise QueryPatients.SelectPatientById
    ' -------------------------------------------------------------------------------------------------
    Public Function GetPatientById(idPatient As Long) As Patient

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryPatients.SelectPatientById, conn)

                    cmd.Parameters.AddWithValue("@id_patient", idPatient)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        If reader.Read() Then
                            Return MapPatient(reader)
                        End If

                    End Using

                End Using

            End Using

            Return Nothing

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur GetPatientById (id={idPatient}).",
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
    ' Fonction   : CreatePatient
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Insère un nouveau patient et retourne l'identifiant généré par la base.
    '
    ' Paramètres :
    ' - patient : Patient à créer (les propriétés d'identité doivent être renseignées)
    '
    ' Retour     :
    ' - Long : Identifiant du patient créé, ou -1 en cas d'échec d'insertion
    '
    ' Remarques  :
    ' - Utilise QueryPatients.InsertPatient puis SELECT LASTVAL(seq_patients) sur la même connexion
    ' - id_patient et code_patient sont générés par la base
    ' - L'unicité du NISS et l'absence de doublon doivent être vérifiées en amont
    ' -------------------------------------------------------------------------------------------------
    Public Function CreatePatient(patient As Patient) As Long

        If patient Is Nothing Then
            Throw New ArgumentNullException(NameOf(patient))
        End If

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryPatients.InsertPatient, conn)

                    AjouterParametresPatient(cmd, patient)

                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected <> 1 Then

                        GestionLog.EcrireLog(
                            $"Création patient échouée : aucune ligne insérée (nom={patient.Nom}, prenom={patient.Prenom}).",
                            GestionLog.LogLevel.Succinct,
                            GestionLog.LogCategory.Database
                        )

                        Return -1

                    End If

                End Using

                Using cmdId As New MySqlCommand("SELECT LASTVAL(seq_patients);", conn)

                    Dim idPatient As Long = Convert.ToInt64(cmdId.ExecuteScalar())

                    If idPatient <= 0 Then

                        GestionLog.EcrireLog(
                            $"Création patient : ID invalide retourné (nom={patient.Nom}, prenom={patient.Prenom}).",
                            GestionLog.LogLevel.Succinct,
                            GestionLog.LogCategory.Database
                        )

                        Return -1

                    End If

                    patient.IdPatient = idPatient

                    GestionLog.EcrireLog(
                        $"Patient créé avec succès (ID={idPatient}, {patient.Prenom} {patient.Nom}).",
                        GestionLog.LogLevel.Rapide,
                        GestionLog.LogCategory.Database
                    )

                    Return idPatient

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur CreatePatient.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdatePatient
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Met à jour un patient existant dans la base de données.
    '
    ' Paramètres :
    ' - patient : Patient avec toutes les propriétés remplies (IdPatient > 0)
    '
    ' Remarques  :
    ' - Utilise QueryPatients.UpdatePatient
    ' - Ne modifie ni id_patient ni code_patient
    ' - L'unicité du NISS et l'absence de doublon doivent être vérifiées en amont
    ' -------------------------------------------------------------------------------------------------
    Public Sub UpdatePatient(patient As Patient)

        If patient Is Nothing Then
            Throw New ArgumentNullException(NameOf(patient))
        End If

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryPatients.UpdatePatient, conn)

                    AjouterParametresPatient(cmd, patient)
                    cmd.Parameters.AddWithValue("@id_patient", patient.IdPatient)

                    cmd.ExecuteNonQuery()

                End Using

            End Using

            GestionLog.EcrireLog(
                $"Patient mis à jour (ID={patient.IdPatient}, {patient.Prenom} {patient.Nom}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database
            )

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur UpdatePatient (id={patient.IdPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdatePhotoPatient
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Met à jour uniquement le nom du fichier photo d'un patient (ou le retire).
    '
    ' Paramètres :
    ' - idPatient    : Identifiant du patient
    ' - photoFichier : Nom du fichier photo (Nothing/vide pour retirer la photo)
    '
    ' Remarques  :
    ' - Utilise QueryPatients.UpdatePhotoPatient
    ' - Le fichier physique est géré par la couche UI/helper de chemins
    ' -------------------------------------------------------------------------------------------------
    Public Sub UpdatePhotoPatient(idPatient As Long, photoFichier As String)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryPatients.UpdatePhotoPatient, conn)

                    cmd.Parameters.AddWithValue("@photo_fichier", ValeurOuDBNull(photoFichier))
                    cmd.Parameters.AddWithValue("@id_patient", idPatient)

                    cmd.ExecuteNonQuery()

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur UpdatePhotoPatient (id={idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateAnamnesePatient
    ' Version    : V1.0.0
    ' Date       : 18/06/2026
    '
    ' Rôle       :
    ' Met à jour uniquement l'anamnèse d'un patient (RTF + texte brut), sans réécrire les autres
    ' colonnes, pour un enregistrement localisé depuis l'onglet Anamnèse de la fiche patient.
    '
    ' Paramètres :
    ' - idPatient    : Identifiant du patient (> 0)
    ' - anamneseRtf  : Contenu RTF de l'anamnèse (Nothing/vide pour vider)
    ' - anamneseTxt  : Texte brut de l'anamnèse (Nothing/vide pour vider)
    '
    ' Remarques  :
    ' - Utilise QueryPatients.UpdateAnamnesePatient
    ' - date_modification est gérée par la base (ON UPDATE current_timestamp())
    ' -------------------------------------------------------------------------------------------------
    Public Sub UpdateAnamnesePatient(idPatient As Long, anamneseRtf As String, anamneseTxt As String)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryPatients.UpdateAnamnesePatient, conn)

                    cmd.Parameters.AddWithValue("@anamnese_rtf", ValeurOuDBNull(anamneseRtf))
                    cmd.Parameters.AddWithValue("@anamnese_txt", ValeurOuDBNull(anamneseTxt))
                    cmd.Parameters.AddWithValue("@id_patient", idPatient)

                    cmd.ExecuteNonQuery()

                End Using

            End Using

            GestionLog.EcrireLog(
                $"Anamnèse mise à jour (patient ID={idPatient}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database
            )

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur UpdateAnamnesePatient (id={idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DeletePatient
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Supprime physiquement un patient de la base de données.
    '
    ' Paramètres :
    ' - idPatient : Identifiant du patient à supprimer
    '
    ' Remarques  :
    ' - Utilise QueryPatients.DeletePatient
    ' - La suppression ne devrait être appelée que si PeutSupprimerPatient retourne True
    ' -------------------------------------------------------------------------------------------------
    Public Sub DeletePatient(idPatient As Long)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryPatients.DeletePatient, conn)

                    cmd.Parameters.AddWithValue("@id_patient", idPatient)

                    cmd.ExecuteNonQuery()

                End Using

            End Using

            GestionLog.EcrireLog(
                $"Patient supprimé (ID={idPatient}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database
            )

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur DeletePatient (id={idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

#End Region

#Region "Validations"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : NissExiste
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Indique si un NISS est déjà utilisé par un autre patient.
    '
    ' Paramètres :
    ' - niss      : NISS à vérifier
    ' - idPatient : Identifiant du patient à exclure (0 en création)
    '
    ' Retour     :
    ' - Boolean : True si le NISS est déjà utilisé par un autre patient, False sinon
    '
    ' Remarques  :
    ' - Un NISS vide n'est jamais considéré comme un doublon (retourne False)
    ' -------------------------------------------------------------------------------------------------
    Public Function NissExiste(niss As String, idPatient As Long) As Boolean

        If String.IsNullOrWhiteSpace(niss) Then
            Return False
        End If

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryPatients.SelectCountNiss, conn)

                    cmd.Parameters.AddWithValue("@niss", niss.Trim())
                    cmd.Parameters.AddWithValue("@id_patient", idPatient)

                    Dim count As Long = Convert.ToInt64(cmd.ExecuteScalar())

                    Return count > 0

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur NissExiste.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : DoublonExiste
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Indique si un patient avec les mêmes nom, prénom et date de naissance existe déjà.
    '
    ' Paramètres :
    ' - nom           : Nom à vérifier
    ' - prenom        : Prénom à vérifier
    ' - dateNaissance : Date de naissance à vérifier (peut être Nothing)
    ' - idPatient     : Identifiant du patient à exclure (0 en création)
    '
    ' Retour     :
    ' - Boolean : True si un doublon potentiel existe, False sinon
    '
    ' Remarques  :
    ' - Aide à la saisie : la décision finale (créer malgré tout) revient à l'utilisateur via l'UI
    ' -------------------------------------------------------------------------------------------------
    Public Function DoublonExiste(
        nom As String,
        prenom As String,
        dateNaissance As Date?,
        idPatient As Long
    ) As Boolean

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryPatients.SelectCountDoublon, conn)

                    cmd.Parameters.AddWithValue("@nom", If(nom, String.Empty).Trim())
                    cmd.Parameters.AddWithValue("@prenom", If(prenom, String.Empty).Trim())
                    cmd.Parameters.AddWithValue("@date_naissance", ValeurDateOuDBNull(dateNaissance))
                    cmd.Parameters.AddWithValue("@id_patient", idPatient)

                    Dim count As Long = Convert.ToInt64(cmd.ExecuteScalar())

                    Return count > 0

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur DoublonExiste.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : PeutSupprimerPatient
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Indique si un patient peut être supprimé physiquement (aucun dossier rattaché).
    '
    ' Paramètres :
    ' - idPatient : Identifiant du patient à vérifier
    '
    ' Retour     :
    ' - Boolean : True si le patient n'a aucun dossier, False sinon
    ' -------------------------------------------------------------------------------------------------
    Public Function PeutSupprimerPatient(idPatient As Long) As Boolean

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryPatients.SelectCountDossiersDuPatient, conn)

                    cmd.Parameters.AddWithValue("@id_patient", idPatient)

                    Dim count As Long = Convert.ToInt64(cmd.ExecuteScalar())

                    Return count = 0

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur PeutSupprimerPatient (id={idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

#End Region

#Region "Helpers privés"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AjouterParametresPatient
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Ajoute les paramètres SQL communs aux requêtes INSERT et UPDATE d'un patient.
    '
    ' Paramètres :
    ' - cmd     : Commande SQL à paramétrer
    ' - patient : Patient source des valeurs
    '
    ' Remarques  :
    ' - Les chaînes vides sont converties en NULL (ValeurOuDBNull) pour les colonnes optionnelles
    ' - @id_patient n'est PAS ajouté ici (spécifique à l'UPDATE)
    ' -------------------------------------------------------------------------------------------------
    Private Sub AjouterParametresPatient(cmd As MySqlCommand, patient As Patient)

        cmd.Parameters.AddWithValue("@nom", If(patient.Nom, String.Empty).Trim())
        cmd.Parameters.AddWithValue("@prenom", If(patient.Prenom, String.Empty).Trim())
        cmd.Parameters.AddWithValue("@date_naissance", ValeurDateOuDBNull(patient.DateNaissance))
        cmd.Parameters.AddWithValue("@niss", ValeurOuDBNull(patient.Niss))
        cmd.Parameters.AddWithValue("@lateralite", ValeurOuDBNull(patient.Lateralite))
        cmd.Parameters.AddWithValue("@adresse_ligne1", ValeurOuDBNull(patient.AdresseLigne1))
        cmd.Parameters.AddWithValue("@adresse_ligne2", ValeurOuDBNull(patient.AdresseLigne2))
        cmd.Parameters.AddWithValue("@code_postal", ValeurOuDBNull(patient.CodePostal))
        cmd.Parameters.AddWithValue("@localite", ValeurOuDBNull(patient.Localite))
        cmd.Parameters.AddWithValue("@pays", ValeurOuDBNull(patient.Pays))
        cmd.Parameters.AddWithValue("@telephone", ValeurOuDBNull(patient.Telephone))
        cmd.Parameters.AddWithValue("@email", ValeurOuDBNull(patient.Email))
        cmd.Parameters.AddWithValue("@mutualite", ValeurOuDBNull(patient.Mutualite))
        cmd.Parameters.AddWithValue("@id_situation_familiale", ValeurLongOuDBNull(patient.IdSituationFamiliale))
        cmd.Parameters.AddWithValue("@photo_fichier", ValeurOuDBNull(patient.PhotoFichier))
        cmd.Parameters.AddWithValue("@alerte_rtf", ValeurOuDBNull(patient.AlerteRtf))
        cmd.Parameters.AddWithValue("@alerte_txt", ValeurOuDBNull(patient.AlerteTxt))
        cmd.Parameters.AddWithValue("@anamnese_rtf", ValeurOuDBNull(patient.AnamneseRtf))
        cmd.Parameters.AddWithValue("@anamnese_txt", ValeurOuDBNull(patient.AnamneseTxt))

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : MapPatient
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Mappe une ligne de DataReader (SelectPatientById) vers un objet Patient complet.
    '
    ' Paramètres :
    ' - reader : DataReader positionné sur la ligne à mapper
    '
    ' Retour     :
    ' - Patient : Objet métier renseigné
    ' -------------------------------------------------------------------------------------------------
    Private Function MapPatient(reader As MySqlDataReader) As Patient

        Dim p As New Patient With {
            .IdPatient = Convert.ToInt64(reader("id_patient")),
            .CodePatient = LireString(reader, "code_patient"),
            .Nom = LireString(reader, "nom"),
            .Prenom = LireString(reader, "prenom"),
            .DateNaissance = LireDateNullable(reader, "date_naissance"),
            .Niss = LireString(reader, "niss"),
            .Lateralite = LireString(reader, "lateralite"),
            .AdresseLigne1 = LireString(reader, "adresse_ligne1"),
            .AdresseLigne2 = LireString(reader, "adresse_ligne2"),
            .CodePostal = LireString(reader, "code_postal"),
            .Localite = LireString(reader, "localite"),
            .Pays = LireString(reader, "pays"),
            .Telephone = LireString(reader, "telephone"),
            .Email = LireString(reader, "email"),
            .Mutualite = LireString(reader, "mutualite"),
            .IdSituationFamiliale = LireLongNullable(reader, "id_situation_familiale"),
            .PhotoFichier = LireString(reader, "photo_fichier"),
            .AlerteRtf = LireString(reader, "alerte_rtf"),
            .AlerteTxt = LireString(reader, "alerte_txt"),
            .AnamneseRtf = LireString(reader, "anamnese_rtf"),
            .AnamneseTxt = LireString(reader, "anamnese_txt"),
            .DateCreation = CDate(reader("date_creation")),
            .DateModification = CDate(reader("date_modification"))
        }

        Return p

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : MapListeItem
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Mappe une ligne de DataReader (SelectPatientsListe) vers un PatientListeItem.
    '
    ' Paramètres :
    ' - reader : DataReader positionné sur la ligne à mapper
    '
    ' Retour     :
    ' - PatientListeItem : Élément de liste renseigné (avec compteurs de dossiers)
    '
    ' Remarques  :
    ' - nb_dossiers_actifs est issu d'un SUM(CASE...) pouvant être NULL si aucun dossier
    ' -------------------------------------------------------------------------------------------------
    Private Function MapListeItem(reader As MySqlDataReader) As PatientListeItem

        Dim item As New PatientListeItem With {
            .IdPatient = Convert.ToInt64(reader("id_patient")),
            .CodePatient = LireString(reader, "code_patient"),
            .Nom = LireString(reader, "nom"),
            .Prenom = LireString(reader, "prenom"),
            .DateNaissance = LireDateNullable(reader, "date_naissance"),
            .Niss = LireString(reader, "niss"),
            .Telephone = LireString(reader, "telephone"),
            .Email = LireString(reader, "email"),
            .AlerteTxt = LireString(reader, "alerte_txt"),
            .PhotoFichier = LireString(reader, "photo_fichier"),
            .NbDossiers = LireInt(reader, "nb_dossiers"),
            .NbDossiersActifs = LireInt(reader, "nb_dossiers_actifs"),
            .SuiviEnCours = LireInt(reader, "suivi_en_cours") <> 0,
            .DateModification = CDate(reader("date_modification"))
        }

        Return item

    End Function

#End Region

End Module
