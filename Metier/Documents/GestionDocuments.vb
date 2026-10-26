' -------------------------------------------------------------------------------------------------
' Module   : GestionDocuments
' Projet   : Althéa
' Version  : V1.0.1
' Date     : 20/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Trace les documents de l'application dans la table `documents` (traçabilité documentaire).
' À ce stade : exports simples (PDF / Word) déclenchés par l'utilisateur depuis l'éditeur
' enrichi, pour l'anamnèse (niveau patient) et les zones cliniques (niveau dossier).
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB et QueryDocuments pour les requêtes SQL
' - Le niveau (patient / dossier / séance) est porté par les colonnes renseignées du modèle
' - Les Flows Word / Google Docs / images / synchronisation Drive sont différés (voir POC)
' - V1.0.1 (20/10/2026) : InsertDocument rend le vrai identifiant (LASTVAL de la séquence ; audit B5).
' -------------------------------------------------------------------------------------------------
Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionDocuments

#Region "Résolution référentiel"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ResoudreIdTypeDocumentParCode
    ' Version    : V1.0.0
    ' Date       :  09/06/2026

    ' Rôle       :
    ' Résout l'id_type_document à partir de son code (ex. 'ANAMNESE', 'ZONE_CLINIQUE').

    ' Retour     :
    ' Retourne 0 si le code est introuvable.

    ' Remarques  :
    ' - Utilise DatabaseManager pour l'accès DB et QueryDocuments pour la requête SQL
    ' -------------------------------------------------------------------------------------------------
    Public Function ResoudreIdTypeDocumentParCode(codeTypeDocument As String) As ULong

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryDocuments.SelectIdTypeDocumentParCode, conn)

                cmd.Parameters.AddWithValue("@code", codeTypeDocument)

                Dim result = cmd.ExecuteScalar()

                If result Is Nothing OrElse result Is DBNull.Value Then
                    Return 0UL
                End If

                Return Convert.ToUInt64(result)

            End Using
        End Using

    End Function

#End Region

#Region "Écriture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : TracerDocument
    ' Version    : V1.0.0
    ' Date       :   09/06/2026

    ' Rôle       :
    ' Trace un document exporté déterministe (anamnèse, zone clinique…) selon la règle d'unicité :
    ' un seul fichier par contexte + type. Le chemin_relatif étant déterministe (sans horodatage),
    ' il sert de clé naturelle : première fois -> INSERT, ensuite -> UPDATE (écrasement logique).

    ' Retour     :
    ' Retourne l'id_document (créé ou existant).

    ' Remarques  :
    ' - Utilise DatabaseManager pour l'accès DB et QueryDocuments pour les requêtes SQL
    ' - Le niveau (patient / dossier / séance) est porté par les colonnes renseignées du modèle
    ' -------------------------------------------------------------------------------------------------
    Public Function TracerDocument(doc As Document) As ULong

        Dim idExistant As ULong = TrouverIdParCheminRelatif(doc.CheminRelatif)

        If idExistant = 0UL Then
            Return InsertDocument(doc)
        End If

        doc.IdDocument = idExistant
        UpdateDocument(doc)
        Return idExistant

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : TracerDocumentParPatientEtType
    ' Version    : V1.0.0 
    ' Date       : 09/06/2026 

    ' Rôle       : 
    ' Trace un document unique par (patient + type) dont le nom de fichier peut changer d'extension
    ' (ex. photo d'identité : identite.jpg -> identite.png). La clé d'unicité est le couple
    ' patient + type de document, pas le chemin : première fois -> INSERT, ensuite -> UPDATE.  

    ' Retour     :
    '  Retourne l'id_document (créé ou existant).

    ' Remarques  :
    ' 
    ' -------------------------------------------------------------------------------------------------
    Public Function TracerDocumentParPatientEtType(doc As Document) As ULong

        If Not doc.IdPatient.HasValue Then
            Return TracerDocument(doc)
        End If

        Dim idExistant As ULong = TrouverIdParPatientEtType(doc.IdPatient.Value, doc.IdTypeDocument)

        If idExistant = 0UL Then
            Return InsertDocument(doc)
        End If

        doc.IdDocument = idExistant
        UpdateDocument(doc)
        Return idExistant

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : TrouverIdParPatientEtType 
    ' Version    : V1.0.0
    ' Date       : 09/06/2026 
    ' Rôle       :
    '  Recherche l'id_document d'un document par (patient + type). Retourne 0 si absent.

    ' Retour     :
    '  Retourne l'id_document (créé ou existant).

    ' Remarques  :
    ' - Utilise DatabaseManager pour l'accès DB et QueryDocuments pour les requêtes SQL
    ' -------------------------------------------------------------------------------------------------
    Public Function TrouverIdParPatientEtType(idPatient As ULong, idTypeDocument As ULong) As ULong

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryDocuments.SelectIdDocumentParPatientEtType, conn)

                cmd.Parameters.AddWithValue("@id_patient", idPatient)
                cmd.Parameters.AddWithValue("@id_type_document", idTypeDocument)

                Dim result = cmd.ExecuteScalar()

                If result Is Nothing OrElse result Is DBNull.Value Then
                    Return 0UL
                End If

                Return Convert.ToUInt64(result)

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : TrouverIdParCheminRelatif
    ' Version    : V1.0.0
    ' Date       : 09/06/2026 

    ' Rôle       :
    '  Recherche l'id_document d'un document par son chemin relatif. Retourne 0 si absent.

    ' Retour     :
    '  Retourne l'id_document (créé ou existant).

    ' Remarques  :
    ' - Utilise DatabaseManager pour l'accès DB et QueryDocuments pour les requêtes SQL
    ' -------------------------------------------------------------------------------------------------
    Public Function TrouverIdParCheminRelatif(cheminRelatif As String) As ULong

        If String.IsNullOrWhiteSpace(cheminRelatif) Then
            Return 0UL
        End If

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryDocuments.SelectIdDocumentParCheminRelatif, conn)

                cmd.Parameters.AddWithValue("@chemin_relatif", cheminRelatif)

                Dim result = cmd.ExecuteScalar()

                If result Is Nothing OrElse result Is DBNull.Value Then
                    Return 0UL
                End If

                Return Convert.ToUInt64(result)

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : InsertDocument 
    ' Version    : V1.0.1
    ' Date       : 20/10/2026

    ' Rôle       :
    '  Insère une trace de document et retourne l'id_document généré par la base.

    ' Retour     :
    '  Retourne l'id_document généré par la base.

    ' Remarques  :
    ' - Utilise DatabaseManager pour l'accès DB et QueryDocuments pour les requêtes SQL
    ' - id_document vient de la séquence seq_documents : lu par LASTVAL sur la même connexion.
    ' - V1.0.1 (20/10/2026) : LAST_INSERT_ID() rendait toujours 0 avec une séquence (audit B5).
    ' -------------------------------------------------------------------------------------------------
    Public Function InsertDocument(doc As Document) As ULong

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryDocuments.InsertDocument, conn)

                cmd.Parameters.AddWithValue("@id_dossier", ValeurOuNull(doc.IdDossier))
                cmd.Parameters.AddWithValue("@id_patient", ValeurOuNull(doc.IdPatient))
                cmd.Parameters.AddWithValue("@id_seance", ValeurOuNull(doc.IdSeance))
                cmd.Parameters.AddWithValue("@id_rendez_vous", ValeurOuNull(doc.IdRendezVous))
                cmd.Parameters.AddWithValue("@id_type_document", doc.IdTypeDocument)
                cmd.Parameters.AddWithValue("@type_source_document", doc.TypeSourceDocument)
                cmd.Parameters.AddWithValue("@est_document_image_metier", doc.EstDocumentImageMetier)
                cmd.Parameters.AddWithValue("@id_modele_document", ValeurOuNull(doc.IdModeleDocument))
                cmd.Parameters.AddWithValue("@nom_document", doc.NomDocument)
                cmd.Parameters.AddWithValue("@nom_fichier", doc.NomFichier)
                cmd.Parameters.AddWithValue("@extension_fichier", TexteOuNull(doc.ExtensionFichier))
                cmd.Parameters.AddWithValue("@taille_fichier_octets", ValeurOuNull(doc.TailleFichierOctets))
                cmd.Parameters.AddWithValue("@chemin_relatif", doc.CheminRelatif)
                cmd.Parameters.AddWithValue("@statut_sync_google", doc.StatutSyncGoogle)
                cmd.Parameters.AddWithValue("@date_document", ValeurOuNull(doc.DateDocument))
                cmd.Parameters.AddWithValue("@commentaire", TexteOuNull(doc.Commentaire))

                cmd.ExecuteNonQuery()

            End Using

            Using cmdId As New MySqlCommand(QueryDocuments.SelectDernierIdDocument, conn)
                Dim result = cmdId.ExecuteScalar()

                If result Is Nothing OrElse result Is DBNull.Value Then
                    Return 0UL
                End If

                Return Convert.ToUInt64(result)
            End Using
        End Using

    End Function



    ' Met à jour une trace de document existante (écrasement logique de la version précédente).
    Public Sub UpdateDocument(doc As Document)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryDocuments.UpdateDocument, conn)

                cmd.Parameters.AddWithValue("@type_source_document", doc.TypeSourceDocument)
                cmd.Parameters.AddWithValue("@nom_document", doc.NomDocument)
                cmd.Parameters.AddWithValue("@nom_fichier", doc.NomFichier)
                cmd.Parameters.AddWithValue("@extension_fichier", TexteOuNull(doc.ExtensionFichier))
                cmd.Parameters.AddWithValue("@taille_fichier_octets", ValeurOuNull(doc.TailleFichierOctets))
                cmd.Parameters.AddWithValue("@chemin_relatif", doc.CheminRelatif)
                cmd.Parameters.AddWithValue("@date_document", ValeurOuNull(doc.DateDocument))
                cmd.Parameters.AddWithValue("@id_document", doc.IdDocument)

                cmd.ExecuteNonQuery()

            End Using
        End Using

    End Sub

#End Region

#Region "Helpers privés"

    Private Function ValeurOuNull(Of T As Structure)(valeur As T?) As Object
        If valeur.HasValue Then
            Return valeur.Value
        End If
        Return DBNull.Value
    End Function

    Private Function TexteOuNull(valeur As String) As Object
        If String.IsNullOrWhiteSpace(valeur) Then
            Return DBNull.Value
        End If
        Return valeur
    End Function

#End Region

End Module
