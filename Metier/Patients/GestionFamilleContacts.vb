' -------------------------------------------------------------------------------------------------
' Module      : GestionFamilleContacts
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 13/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Gestion métier des contacts de l'entourage d'un patient (table famille_contacts).
'
' Responsabilités :
' - Charger la liste des contacts d'un patient et un contact par identifiant
' - Créer, mettre à jour et supprimer (physiquement) un contact
' - Mapper les DataReader vers les objets métier (FamilleContact)
'
' Important   :
' - Aucun accès UI direct (pas de MessageBox)
' - Toute la logique SQL est déléguée à QueryFamilleContacts
' - Les exceptions sont journalisées via GestionLog puis propagées (Throw)
'
' Architecture :
' UI → GestionFamilleContacts → QueryFamilleContacts → DatabaseManager → MariaDB
'
' Dépendances :
' - DatabaseManager (connexions DB)
' - QueryFamilleContacts (requêtes SQL)
' - GestionLog (traçabilité)
' - FamilleContact (objet métier)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionFamilleContacts

#Region "Lecture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetContactsParPatient
    ' Version    : V1.0.0
    ' Date       : 13/06/2026
    '
    ' Rôle       :
    ' Charge la liste des contacts rattachés à un patient.
    '
    ' Paramètres :
    ' - idPatient : Identifiant du patient
    '
    ' Retour     :
    ' - List(Of FamilleContact) : Contacts triés par ordre d'affichage puis nom/prénom
    '
    ' Remarques  :
    ' - Utilise QueryFamilleContacts.SelectContactsParPatient
    ' -------------------------------------------------------------------------------------------------
    Public Function GetContactsParPatient(idPatient As Long) As List(Of FamilleContact)

        Dim result As New List(Of FamilleContact)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryFamilleContacts.SelectContactsParPatient, conn)

                    cmd.Parameters.AddWithValue("@id_patient", idPatient)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        While reader.Read()
                            result.Add(MapContact(reader))
                        End While

                    End Using

                End Using

            End Using

            Return result

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur GetContactsParPatient (id_patient={idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetContactById
    ' Version    : V1.0.0
    ' Date       : 13/06/2026
    '
    ' Rôle       :
    ' Charge un contact complet depuis son identifiant.
    '
    ' Paramètres :
    ' - idFamilleContact : Identifiant du contact à charger
    '
    ' Retour     :
    ' - FamilleContact : L'objet contact, ou Nothing si introuvable
    '
    ' Remarques  :
    ' - Utilise QueryFamilleContacts.SelectContactById
    ' -------------------------------------------------------------------------------------------------
    Public Function GetContactById(idFamilleContact As Long) As FamilleContact

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryFamilleContacts.SelectContactById, conn)

                    cmd.Parameters.AddWithValue("@id_famille_contact", idFamilleContact)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        If reader.Read() Then
                            Return MapContact(reader)
                        End If

                    End Using

                End Using

            End Using

            Return Nothing

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur GetContactById (id={idFamilleContact}).",
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
    ' Fonction   : CreateContact
    ' Version    : V1.0.0
    ' Date       : 13/06/2026
    '
    ' Rôle       :
    ' Insère un nouveau contact et retourne l'identifiant généré par la base.
    '
    ' Paramètres :
    ' - contact : Contact à créer (IdPatient et IdLienPatient doivent être renseignés)
    '
    ' Retour     :
    ' - Long : Identifiant du contact créé, ou -1 en cas d'échec d'insertion
    '
    ' Remarques  :
    ' - Utilise QueryFamilleContacts.InsertContact puis SELECT LASTVAL(seq_famille_contacts)
    ' - id_famille_contact et code_famille_contact sont générés par la base
    ' -------------------------------------------------------------------------------------------------
    Public Function CreateContact(contact As FamilleContact) As Long

        If contact Is Nothing Then
            Throw New ArgumentNullException(NameOf(contact))
        End If

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryFamilleContacts.InsertContact, conn)

                    AjouterParametresContact(cmd, contact)

                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected <> 1 Then

                        GestionLog.EcrireLog(
                            $"Création contact échouée : aucune ligne insérée (nom={contact.Nom}, prenom={contact.Prenom}).",
                            GestionLog.LogLevel.Succinct,
                            GestionLog.LogCategory.Database
                        )

                        Return -1

                    End If

                End Using

                Using cmdId As New MySqlCommand("SELECT LASTVAL(seq_famille_contacts);", conn)

                    Dim idContact As Long = Convert.ToInt64(cmdId.ExecuteScalar())

                    If idContact <= 0 Then

                        GestionLog.EcrireLog(
                            $"Création contact : ID invalide retourné (nom={contact.Nom}, prenom={contact.Prenom}).",
                            GestionLog.LogLevel.Succinct,
                            GestionLog.LogCategory.Database
                        )

                        Return -1

                    End If

                    contact.IdFamilleContact = idContact

                    GestionLog.EcrireLog(
                        $"Contact créé avec succès (ID={idContact}, {contact.Prenom} {contact.Nom}).",
                        GestionLog.LogLevel.Rapide,
                        GestionLog.LogCategory.Database
                    )

                    Return idContact

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur CreateContact.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateContact
    ' Version    : V1.0.0
    ' Date       : 13/06/2026
    '
    ' Rôle       :
    ' Met à jour un contact existant dans la base de données.
    '
    ' Paramètres :
    ' - contact : Contact avec toutes les propriétés remplies (IdFamilleContact > 0)
    '
    ' Remarques  :
    ' - Utilise QueryFamilleContacts.UpdateContact
    ' - Ne modifie ni id_famille_contact ni code_famille_contact ni id_patient
    ' -------------------------------------------------------------------------------------------------
    Public Sub UpdateContact(contact As FamilleContact)

        If contact Is Nothing Then
            Throw New ArgumentNullException(NameOf(contact))
        End If

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryFamilleContacts.UpdateContact, conn)

                    AjouterParametresContact(cmd, contact)
                    cmd.Parameters.AddWithValue("@id_famille_contact", contact.IdFamilleContact)

                    cmd.ExecuteNonQuery()

                End Using

            End Using

            GestionLog.EcrireLog(
                $"Contact mis à jour (ID={contact.IdFamilleContact}, {contact.Prenom} {contact.Nom}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database
            )

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur UpdateContact (id={contact.IdFamilleContact}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DeleteContact
    ' Version    : V1.0.0
    ' Date       : 13/06/2026
    '
    ' Rôle       :
    ' Supprime physiquement un contact.
    '
    ' Paramètres :
    ' - idFamilleContact : Identifiant du contact à supprimer
    '
    ' Remarques  :
    ' - Utilise QueryFamilleContacts.DeleteContact
    ' - La confirmation utilisateur doit être réalisée en amont par l'UI
    ' -------------------------------------------------------------------------------------------------
    Public Sub DeleteContact(idFamilleContact As Long)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryFamilleContacts.DeleteContact, conn)

                    cmd.Parameters.AddWithValue("@id_famille_contact", idFamilleContact)

                    cmd.ExecuteNonQuery()

                End Using

            End Using

            GestionLog.EcrireLog(
                $"Contact supprimé (ID={idFamilleContact}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database
            )

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur DeleteContact (id={idFamilleContact}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

#End Region

#Region "Helpers privés"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AjouterParametresContact
    ' Version    : V1.0.0
    ' Date       : 13/06/2026
    '
    ' Rôle       :
    ' Ajoute les paramètres SQL communs aux requêtes INSERT et UPDATE d'un contact.
    '
    ' Paramètres :
    ' - cmd     : Commande SQL à paramétrer
    ' - contact : Contact source des valeurs
    '
    ' Remarques  :
    ' - Les chaînes vides sont converties en NULL (ValeurOuDBNull) pour les colonnes optionnelles
    ' - @id_famille_contact n'est PAS ajouté ici (spécifique à l'UPDATE)
    ' -------------------------------------------------------------------------------------------------
    Private Sub AjouterParametresContact(cmd As MySqlCommand, contact As FamilleContact)

        cmd.Parameters.AddWithValue("@id_patient", contact.IdPatient)
        cmd.Parameters.AddWithValue("@id_lien_patient", contact.IdLienPatient)
        cmd.Parameters.AddWithValue("@nom", If(contact.Nom, String.Empty).Trim())
        cmd.Parameters.AddWithValue("@prenom", If(contact.Prenom, String.Empty).Trim())
        cmd.Parameters.AddWithValue("@date_naissance", ValeurDateOuDBNull(contact.DateNaissance))
        cmd.Parameters.AddWithValue("@telephone", ValeurOuDBNull(contact.Telephone))
        cmd.Parameters.AddWithValue("@email", ValeurOuDBNull(contact.Email))
        cmd.Parameters.AddWithValue("@adresse_ligne1", ValeurOuDBNull(contact.AdresseLigne1))
        cmd.Parameters.AddWithValue("@adresse_ligne2", ValeurOuDBNull(contact.AdresseLigne2))
        cmd.Parameters.AddWithValue("@code_postal", ValeurOuDBNull(contact.CodePostal))
        cmd.Parameters.AddWithValue("@localite", ValeurOuDBNull(contact.Localite))
        cmd.Parameters.AddWithValue("@pays", ValeurOuDBNull(contact.Pays))
        cmd.Parameters.AddWithValue("@id_role_legal", contact.IdRoleLegal)
        cmd.Parameters.AddWithValue("@ordre_affichage", contact.OrdreAffichage)
        cmd.Parameters.AddWithValue("@commentaire_rtf", ValeurOuDBNull(contact.CommentaireRtf))
        cmd.Parameters.AddWithValue("@commentaire_txt", ValeurOuDBNull(contact.CommentaireTxt))

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : MapContact
    ' Version    : V1.0.0
    ' Date       : 13/06/2026
    '
    ' Rôle       :
    ' Mappe une ligne de DataReader vers un objet FamilleContact complet.
    '
    ' Paramètres :
    ' - reader : DataReader positionné sur la ligne à mapper
    '
    ' Retour     :
    ' - FamilleContact : Objet métier renseigné
    ' -------------------------------------------------------------------------------------------------
    Private Function MapContact(reader As MySqlDataReader) As FamilleContact

        Dim c As New FamilleContact With {
            .IdFamilleContact = Convert.ToInt64(reader("id_famille_contact")),
            .CodeFamilleContact = LireString(reader, "code_famille_contact"),
            .IdPatient = Convert.ToInt64(reader("id_patient")),
            .IdLienPatient = Convert.ToUInt64(reader("id_lien_patient")),
            .LibelleLienPatient = LireString(reader, "libelle_lien_patient"),
            .Nom = LireString(reader, "nom"),
            .Prenom = LireString(reader, "prenom"),
            .DateNaissance = LireDateNullable(reader, "date_naissance"),
            .Telephone = LireString(reader, "telephone"),
            .Email = LireString(reader, "email"),
            .AdresseLigne1 = LireString(reader, "adresse_ligne1"),
            .AdresseLigne2 = LireString(reader, "adresse_ligne2"),
            .CodePostal = LireString(reader, "code_postal"),
            .Localite = LireString(reader, "localite"),
            .Pays = LireString(reader, "pays"),
            .IdRoleLegal = Convert.ToUInt64(reader("id_role_legal")),
            .LibelleRoleLegal = LireString(reader, "libelle_role_legal"),
            .OrdreAffichage = LireInt(reader, "ordre_affichage"),
            .CommentaireRtf = LireString(reader, "commentaire_rtf"),
            .CommentaireTxt = LireString(reader, "commentaire_txt"),
            .DateCreation = CDate(reader("date_creation")),
            .DateModification = CDate(reader("date_modification"))
        }

        Return c

    End Function

#End Region

End Module
