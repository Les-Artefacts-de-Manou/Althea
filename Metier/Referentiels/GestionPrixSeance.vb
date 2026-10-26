' -------------------------------------------------------------------------------------------------
' Module   : GestionPrixSeance
' Projet   : Althéa
' Version  : V1.1.1
' Date     : 25/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les tarifs de séance (lecture depuis la base, écriture, vérifications), à partir de la
' table référentielle ref_prix_seance.
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB
' - Utilise QueryPrixSeance pour les requêtes SQL
' - Retourne des objets PrixSeance
' - Suppression physique réservée aux tarifs non utilisés ; sinon soft-delete (Actif = 0)
' - PARTICULARITÉ : gère le champ montant (decimal(10,2) NOT NULL), figé sur la séance à sa création
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : chaque écriture reçoit le ContexteOperation et
'   commence par ReglesReferentiels.ExigerDroitEcriture (SuperUser) ; SupprimerPrixSeance, plus
'   appelée, supprimée.
'   Lignes système : refus explicite si la requête n'a concerné aucune ligne (ExecuterSurLigneModifiable).
' - V1.1.1 (25/10/2026, audit D-1) : Option Strict On ; identifiant lu par GetUInt64 (colonne BIGINT
'   UNSIGNED, propriété ULong).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionPrixSeance

#Region "Lecture"

    Public Function GetPrixSeanceActifs() As List(Of PrixSeance)

        Return GetPrixSeanceDepuisRequete(QueryPrixSeance.SelectPrixSeanceActifs)

    End Function

    Public Function GetPrixSeance(afficherInactifs As Boolean) As List(Of PrixSeance)

        If afficherInactifs Then
            Return GetPrixSeanceDepuisRequete(QueryPrixSeance.SelectPrixSeanceTous)
        End If

        Return GetPrixSeanceActifs()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetPrixSeanceParDomaine
    ' Version    : V2.0.0
    ' Date       : 19/09/2026
    '
    ' Rôle       :
    ' Retourne les tarifs actifs rattachés à un domaine à une date donnée (lia_prix_seance_domaines).
    ' Utilisé pour filtrer les tarifs proposés dans SeanceEditor pour un domaine conventionné.
    '
    ' Paramètres :
    ' - idDomaine     : Identifiant du domaine du dossier
    ' - dateReference : Date de la séance, la liaison étant historisée
    '
    ' Retour     :
    ' - List(Of PrixSeance) : tarifs rattachés à ce domaine et valides à cette date
    ' -------------------------------------------------------------------------------------------------
    Public Function GetPrixSeanceParDomaine(idDomaine As Long, dateReference As Date) As List(Of PrixSeance)

        Dim result As New List(Of PrixSeance)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryPrixSeance.SelectPrixSeanceParDomaine, conn)

                cmd.Parameters.AddWithValue("@id_domaine", idDomaine)
                cmd.Parameters.AddWithValue("@date_reference", dateReference.Date)

                Using reader = cmd.ExecuteReader()

                    While reader.Read()

                        result.Add(MapPrixSeance(reader))

                    End While

                End Using
            End Using
        End Using

        Return result

    End Function

    Private Function GetPrixSeanceDepuisRequete(query As String) As List(Of PrixSeance)

        Dim result As New List(Of PrixSeance)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()

                    While reader.Read()

                        result.Add(MapPrixSeance(reader))

                    End While

                End Using
            End Using
        End Using

        Return result

    End Function

    Private Function MapPrixSeance(reader As MySqlDataReader) As PrixSeance

        Return New PrixSeance With {
            .IdPrixSeance = reader.GetUInt64("id_prix_seance"),
            .CodePrixSeance = reader.GetString("code_prix_seance"),
            .LibellePrixSeance = reader.GetString("libelle_prix_seance"),
            .Montant = reader.GetDecimal("montant"),
            .Actif = reader.GetBoolean("actif"),
            .EstSysteme = UtilsDataReader.LireBooleenOptionnel(reader, "est_systeme"),
            .OrdreAffichage = reader.GetInt32("ordre_affichage")
        }

    End Function

#End Region

#Region "Ecriture"

    Public Sub UpdatePrixSeance(contexte As ContexteOperation, prix As PrixSeance)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryPrixSeance.UpdatePrixSeance, conn)

                cmd.Parameters.AddWithValue("@id", prix.IdPrixSeance)
                cmd.Parameters.AddWithValue("@code", prix.CodePrixSeance)
                cmd.Parameters.AddWithValue("@libelle", prix.LibellePrixSeance)
                cmd.Parameters.AddWithValue("@montant", prix.Montant)
                cmd.Parameters.AddWithValue("@actif", prix.Actif)
                cmd.Parameters.AddWithValue("@ordre", prix.OrdreAffichage)

                ReglesReferentiels.ExecuterSurLigneModifiable(cmd)

            End Using

        End Using

    End Sub

    Public Sub InsertPrixSeance(contexte As ContexteOperation, prix As PrixSeance)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryPrixSeance.InsertPrixSeance, conn)

                cmd.Parameters.AddWithValue("@code", prix.CodePrixSeance)
                cmd.Parameters.AddWithValue("@libelle", prix.LibellePrixSeance)
                cmd.Parameters.AddWithValue("@montant", prix.Montant)
                cmd.Parameters.AddWithValue("@actif", prix.Actif)
                cmd.Parameters.AddWithValue("@ordre", prix.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    Public Sub DesactiverPrixSeance(contexte As ContexteOperation, idPrixSeance As ULong)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryPrixSeance.DesactiverPrixSeance, conn)
                cmd.Parameters.AddWithValue("@id", idPrixSeance)
                ReglesReferentiels.ExecuterSurLigneModifiable(cmd)
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    Public Function CodePrixSeanceExiste(code As String,
                                         Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryPrixSeance.SelectCountCodePrixSeance, conn)

                cmd.Parameters.AddWithValue("@code", code)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    Public Function LibellePrixSeanceExiste(libelle As String,
                                            Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryPrixSeance.SelectCountLibellePrixSeance, conn)

                cmd.Parameters.AddWithValue("@libelle", libelle)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    Public Function PrixSeanceEstUtilise(idPrixSeance As ULong) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryPrixSeance.SelectCountUsagePrixSeance, conn)

                cmd.Parameters.AddWithValue("@id", idPrixSeance)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

#End Region

End Module
