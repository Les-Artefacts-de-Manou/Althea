' -------------------------------------------------------------------------------------------------
' Module      : GestionPatients
' Projet      : Althéa
' Version     : V1.7.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Gestion métier des patients (lecture, écriture, recherche, validations).
'
' Responsabilités :
' - Charger la liste des patients (avec compteurs de dossiers) et un patient par identifiant
' - Créer, mettre à jour et supprimer un patient
' - Mettre à jour la photo d'identité
' - Valider la fiche (ValiderPatient : règles de saisie, unicité du NISS) ; détecter les doublons
'   possibles (nom + prénom + date de naissance : confirmation laissée à l'écran)
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
'
' Historique  :
' - V1.3.0 (17/10/2026) : la liste porte prochain rendez-vous, dernière séance, séances du jour et
'   dossiers au statut Actif (grille Patients, étape 1).
' - V1.3.1 (17/10/2026) : repères des séances (prochain rendez-vous, dernière séance).
' - V1.4.0 (20/10/2026, v2.46.0) : latéralité lue et écrite par son identifiant (ref_lateralites).
' - V1.5.0 (23/10/2026, audit C.2 étape 2, décisions de Joëlle) : ValiderPatient (règles sorties de
'   UC_PatientFiche, liste d'erreurs par champ, ADR-51), appliqué aussi par CreatePatient et
'   UpdatePatient, qui n'en vérifiaient aucune ; NISS enregistré sous forme normalisée
'   (UtilsNiss.Normaliser, migration v2.54.0) et comparé sous cette forme (NissExiste) ; le doublon
'   possible tient compte d'une date de naissance inconnue (DoublonExiste, relevé de Joëlle).
' - V1.6.0 (24/10/2026, recherche R4) : la liste des patients porte code postal, localité et pays.
' - V1.7.0 (24/10/2026, v2.56.0) : ValiderPatient refuse un code postal belge inexistant
'   (GestionCodesPostauxBe).
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
    ' Version    : V1.1.0
    ' Date       : 23/10/2026
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
    ' - V1.1.0 (23/10/2026, audit C.2) : NISS normalisé, puis refus si ValiderPatient trouve une
    '   erreur (ErreurValidationException). Le doublon possible (nom + prénom + naissance) reste une
    '   confirmation de l'écran : ce n'est pas une règle, deux homonymes nés le même jour existent.
    ' -------------------------------------------------------------------------------------------------
    Public Function CreatePatient(patient As Patient) As Long

        If patient Is Nothing Then
            Throw New ArgumentNullException(NameOf(patient))
        End If

        Try

            patient.Niss = UtilsNiss.Normaliser(patient.Niss)
            ErreurValidationException.LeverSiErreurs(ValiderPatient(patient))

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryPatients.InsertPatient, conn)

                    AjouterParametresPatient(cmd, patient)

                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected <> 1 Then

                        GestionLog.EcrireLog(
                            $"Création patient échouée : aucune ligne insérée (identité non journalisée).",
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
                            $"Création patient : ID invalide retourné (identité non journalisée).",
                            GestionLog.LogLevel.Succinct,
                            GestionLog.LogCategory.Database
                        )

                        Return -1

                    End If

                    patient.IdPatient = idPatient

                    GestionLog.EcrireLog(
                        $"Patient créé avec succès (ID={idPatient}).",
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
    ' Version    : V1.1.0
    ' Date       : 23/10/2026
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
    ' - V1.1.0 (23/10/2026, audit C.2) : NISS normalisé, puis refus si ValiderPatient trouve une
    '   erreur (ErreurValidationException).
    ' -------------------------------------------------------------------------------------------------
    Public Sub UpdatePatient(patient As Patient)

        If patient Is Nothing Then
            Throw New ArgumentNullException(NameOf(patient))
        End If

        Try

            patient.Niss = UtilsNiss.Normaliser(patient.Niss)
            ErreurValidationException.LeverSiErreurs(ValiderPatient(patient))

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryPatients.UpdatePatient, conn)

                    AjouterParametresPatient(cmd, patient)
                    cmd.Parameters.AddWithValue("@id_patient", patient.IdPatient)

                    cmd.ExecuteNonQuery()

                End Using

            End Using

            GestionLog.EcrireLog(
                $"Patient mis à jour (ID={patient.IdPatient}).",
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
    ' Procédure  : ChangerStatutActif
    ' Version    : V1.0.0
    ' Date       : 14/07/2026
    '
    ' Rôle       :
    ' Active ou désactive manuellement un patient (is_actif), indépendamment de ses dossiers.
    ' Les dates d'activation/désactivation sont mises à jour par la base :
    ' - actif = True  : date_activation = aujourd'hui, date_desactivation = NULL
    ' - actif = False : date_desactivation = aujourd'hui, date_activation conservée
    '
    ' Paramètres :
    ' - idPatient : Identifiant du patient (> 0)
    ' - actif     : Nouvel état d'activation (True = actif, False = inactif)
    '
    ' Remarques  :
    ' - Bascule manuelle déclenchée par l'utilisateur depuis la fiche patient.
    ' - Sans lien avec le cycle de vie des dossiers (clôture/archivage ne modifient pas is_actif).
    ' -------------------------------------------------------------------------------------------------
    Public Sub ChangerStatutActif(idPatient As Long, actif As Boolean)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryPatients.UpdateStatutActifPatient, conn)

                    cmd.Parameters.AddWithValue("@is_actif", If(actif, 1, 0))
                    cmd.Parameters.AddWithValue("@id_patient", idPatient)

                    cmd.ExecuteNonQuery()

                End Using

            End Using

            GestionLog.EcrireLog(
                $"Statut d'activation patient modifié (ID={idPatient}, actif={actif}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database
            )

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur ChangerStatutActif (id={idPatient}).",
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
    ' Fonction   : ValiderPatient
    ' Version    : V1.1.0
    ' Date       : 24/10/2026
    '
    ' Rôle       :
    ' Erreurs de saisie de la fiche d'un patient (liste vide si elle est valide) :
    ' - nom et prénom obligatoires ;
    ' - téléphone et code postal plausibles pour le pays de l'adresse ;
    ' - e-mail au bon format ;
    ' - NISS au format et à la clé de son type d'identifiant (TypeIdentifiantNiss, sinon déduit du
    '   numéro), puis unique ;
    ' - codes titulaire de la mutualité (CT1, CT2) de 3 chiffres.
    ' Tout champ facultatif vide est accepté.
    '
    ' Paramètres :
    ' - patient : Fiche saisie (IdPatient = 0 en création)
    '
    ' Remarques  :
    ' - Sortie de UC_PatientFiche.ValiderSaisie (audit C.2, ADR-51) ; la fiche l'appelle pour placer
    '   les erreurs sur ses champs, CreatePatient et UpdatePatient l'appliquent.
    ' - Champs : NameOf des propriétés de Patient (ErreurSaisie).
    ' - Les messages ne recopient jamais la valeur saisie (le refus peut passer par le journal).
    ' - Le doublon possible (nom + prénom + naissance) n'est pas une erreur : DoublonExiste.
    ' - V1.1.0 (24/10/2026, v2.56.0) : en Belgique, le code postal doit exister (GestionCodesPostauxBe).
    ' -------------------------------------------------------------------------------------------------
    Public Function ValiderPatient(patient As Patient) As List(Of ErreurSaisie)

        If patient Is Nothing Then Throw New ArgumentNullException(NameOf(patient))

        Dim erreurs As New List(Of ErreurSaisie)
        Dim message As String = String.Empty
        Dim codePays As String = GestionPays.CodePaysParId(patient.IdPays)

        If String.IsNullOrWhiteSpace(patient.Nom) Then
            erreurs.Add(New ErreurSaisie(NameOf(Patient.Nom), "Le nom est obligatoire."))
        End If

        If String.IsNullOrWhiteSpace(patient.Prenom) Then
            erreurs.Add(New ErreurSaisie(NameOf(Patient.Prenom), "Le prénom est obligatoire."))
        End If

        If Not UtilsTelephone.Valider(patient.Telephone, codePays, message) Then
            erreurs.Add(New ErreurSaisie(NameOf(Patient.Telephone), message))
        End If

        If Not GestionCodesPostauxBe.Valider(patient.CodePostal, codePays, message) Then
            erreurs.Add(New ErreurSaisie(NameOf(Patient.CodePostal), message))
        End If

        If Not UtilsValidation.IsValidEmail(patient.Email, message) Then
            erreurs.Add(New ErreurSaisie(NameOf(Patient.Email), message))
        End If

        Dim typeNiss As String = If(String.IsNullOrWhiteSpace(patient.TypeIdentifiantNiss),
                                    UtilsNiss.DeduireType(patient.Niss), patient.TypeIdentifiantNiss)

        If Not UtilsNiss.Valider(patient.Niss, typeNiss, message) Then
            erreurs.Add(New ErreurSaisie(NameOf(Patient.Niss), message))
        ElseIf NissExiste(patient.Niss, patient.IdPatient) Then
            erreurs.Add(New ErreurSaisie(NameOf(Patient.Niss), "Ce NISS est déjà utilisé par un autre patient."))
        End If

        If Not UtilsIdentifiantBelge.ValiderCodeTitulaire(patient.CodeTitulaire1, message) Then
            erreurs.Add(New ErreurSaisie(NameOf(Patient.CodeTitulaire1), "CT1 : " & message))
        End If

        If Not UtilsIdentifiantBelge.ValiderCodeTitulaire(patient.CodeTitulaire2, message) Then
            erreurs.Add(New ErreurSaisie(NameOf(Patient.CodeTitulaire2), "CT2 : " & message))
        End If

        Return erreurs

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : NissExiste
    ' Version    : V1.1.0
    ' Date       : 23/10/2026
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
    ' - V1.1.0 (23/10/2026, audit C.2) : comparé sous sa forme normalisée (UtilsNiss.Normaliser), celle
    '   de la base depuis la migration v2.54.0.
    ' -------------------------------------------------------------------------------------------------
    Public Function NissExiste(niss As String, idPatient As Long) As Boolean

        niss = UtilsNiss.Normaliser(niss)

        If String.IsNullOrWhiteSpace(niss) Then
            Return False
        End If

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryPatients.SelectCountNiss, conn)

                    cmd.Parameters.AddWithValue("@niss", niss)
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
    ' Version    : V1.1.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Indique si un patient avec les mêmes nom et prénom existe déjà, né le même jour ou dont la date
    ' de naissance est inconnue (d'un côté ou de l'autre).
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
    ' - V1.1.0 (23/10/2026, relevé de Joëlle) : sans date de naissance, l'homonyme était ignoré
    '   (QueryPatients.SelectCountDoublon V1.1.0).
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
        cmd.Parameters.AddWithValue("@id_lateralite", ValeurLongOuDBNull(patient.IdLateralite))
        cmd.Parameters.AddWithValue("@adresse_ligne1", ValeurOuDBNull(patient.AdresseLigne1))
        cmd.Parameters.AddWithValue("@adresse_ligne2", ValeurOuDBNull(patient.AdresseLigne2))
        cmd.Parameters.AddWithValue("@code_postal", ValeurOuDBNull(patient.CodePostal))
        cmd.Parameters.AddWithValue("@localite", ValeurOuDBNull(patient.Localite))
        cmd.Parameters.AddWithValue("@id_pays", ValeurULongOuDBNull(patient.IdPays))
        cmd.Parameters.AddWithValue("@id_civilite", ValeurULongOuDBNull(patient.IdCivilite))
        cmd.Parameters.AddWithValue("@telephone", ValeurOuDBNull(patient.Telephone))
        cmd.Parameters.AddWithValue("@email", ValeurOuDBNull(patient.Email))
        cmd.Parameters.AddWithValue("@id_organisme_mutualite", ValeurULongOuDBNull(patient.IdOrganismeMutualite))
        cmd.Parameters.AddWithValue("@numero_affiliation", ValeurOuDBNull(patient.NumeroAffiliation))
        cmd.Parameters.AddWithValue("@code_titulaire_1", ValeurOuDBNull(patient.CodeTitulaire1))
        cmd.Parameters.AddWithValue("@code_titulaire_2", ValeurOuDBNull(patient.CodeTitulaire2))
        cmd.Parameters.AddWithValue("@id_situation_familiale", ValeurLongOuDBNull(patient.IdSituationFamiliale))
        cmd.Parameters.AddWithValue("@id_prix_seance_defaut", ValeurULongOuDBNull(patient.IdPrixSeanceDefaut))

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
            .IdLateralite = LireLongNullable(reader, "id_lateralite"),
            .AdresseLigne1 = LireString(reader, "adresse_ligne1"),
            .AdresseLigne2 = LireString(reader, "adresse_ligne2"),
            .CodePostal = LireString(reader, "code_postal"),
            .Localite = LireString(reader, "localite"),
            .IdPays = LireULongNullable(reader, "id_pays"),
            .CodePays = LireString(reader, "code_pays"),
            .IdCivilite = LireULongNullable(reader, "id_civilite"),
            .Telephone = LireString(reader, "telephone"),
            .Email = LireString(reader, "email"),
            .IdOrganismeMutualite = LireULongNullable(reader, "id_organisme_mutualite"),
            .NumeroAffiliation = LireString(reader, "numero_affiliation"),
            .CodeTitulaire1 = LireString(reader, "code_titulaire_1"),
            .CodeTitulaire2 = LireString(reader, "code_titulaire_2"),
            .IdSituationFamiliale = LireLongNullable(reader, "id_situation_familiale"),
            .IdPrixSeanceDefaut = LireULongNullable(reader, "id_prix_seance_defaut"),
            .PhotoFichier = LireString(reader, "photo_fichier"),
            .AlerteRtf = LireString(reader, "alerte_rtf"),
            .AlerteTxt = LireString(reader, "alerte_txt"),
            .AnamneseRtf = LireString(reader, "anamnese_rtf"),
            .AnamneseTxt = LireString(reader, "anamnese_txt"),
            .IsActif = LireBool(reader, "is_actif"),
            .DateActivation = LireDateNullable(reader, "date_activation"),
            .DateDesactivation = LireDateNullable(reader, "date_desactivation"),
            .DateCreation = CDate(reader("date_creation")),
            .DateModification = CDate(reader("date_modification"))
        }

        Return p

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : MapListeItem
    ' Version    : V1.2.0
    ' Date       : 24/10/2026
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
    ' - V1.2.0 (24/10/2026, recherche R4) : code postal, localité et pays.
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
            .CodePostal = LireString(reader, "code_postal"),
            .Localite = LireString(reader, "localite"),
            .CodePays = LireString(reader, "code_pays"),
            .AlerteTxt = LireString(reader, "alerte_txt"),
            .PhotoFichier = LireString(reader, "photo_fichier"),
            .NbDossiers = LireInt(reader, "nb_dossiers"),
            .NbDossiersActifs = LireInt(reader, "nb_dossiers_actifs"),
            .IsActif = LireBool(reader, "is_actif"),
            .CodeStatutDossierDominant = LireString(reader, "code_statut_dernier_dossier"),
            .LibelleStatutDossierDominant = LireString(reader, "libelle_statut_dernier_dossier"),
            .CodeDomaineDominant = LireString(reader, "code_domaine_dominant"),
            .MontantDu = LireDecimalDb(reader, "montant_du"),
            .MontantAvoir = LireDecimalDb(reader, "montant_avoir"),
            .NbNoShow = LireInt(reader, "nb_no_show"),
            .NbDossiersStatutActif = LireInt(reader, "nb_dossiers_statut_actif"),
            .ProchaineSeanceDate = LireDateNullable(reader, "prochaine_seance_date"),
            .ProchaineSeanceHeure = LireTimeNullable(reader, "prochaine_seance_heure"),
            .DerniereSeanceDate = LireDateNullable(reader, "derniere_seance_date"),
            .DerniereSeanceHeure = LireTimeNullable(reader, "derniere_seance_heure"),
            .NbSeancesJour = LireInt(reader, "nb_seances_jour"),
            .ProchaineSeance = LireRepereSeance(reader, "prochaine"),
            .DerniereSeance = LireRepereSeance(reader, "derniere"),
            .DateModification = CDate(reader("date_modification"))
        }

        If String.IsNullOrWhiteSpace(item.LibelleStatutDossierDominant) Then
            item.LibelleStatutDossierDominant = item.CodeStatutDossierDominant
        End If

        Return item

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : LireRepereSeance
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Lit le repère d'une séance de SelectPatientsListe (colonnes <prefixe>_seance_id / _code /
    ' _dossier) ; Nothing si la séance n'existe pas.
    ' -------------------------------------------------------------------------------------------------
    Private Function LireRepereSeance(reader As MySqlDataReader, prefixe As String) As RepereSeance

        Dim idSeance As Long? = LireLongNullable(reader, $"{prefixe}_seance_id")
        Dim idDossier As Long? = LireLongNullable(reader, $"{prefixe}_seance_dossier")

        If Not idSeance.HasValue OrElse Not idDossier.HasValue Then
            Return Nothing
        End If

        Return New RepereSeance With {
            .IdSeance = idSeance.Value,
            .CodeSeance = LireString(reader, $"{prefixe}_seance_code"),
            .IdDossier = idDossier.Value
        }

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : LireBool
    ' Version    : V1.0.0
    ' Date       : 14/07/2026
    '
    ' Rôle       :
    ' Lit une colonne tinyint NOT NULL : False si NULL (sécurité), sinon True si <> 0.
    ' -------------------------------------------------------------------------------------------------
    Private Function LireBool(reader As MySqlDataReader, colonne As String) As Boolean

        If reader(colonne) Is DBNull.Value Then
            Return False
        End If

        Return Convert.ToInt32(reader(colonne)) <> 0

    End Function

#End Region

End Module
