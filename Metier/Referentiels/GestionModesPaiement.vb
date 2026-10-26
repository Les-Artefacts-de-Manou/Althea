' -------------------------------------------------------------------------------------------------
' Module   : GestionModesPaiement
' Projet   : Althéa
' Version  : V1.2.1
' Date     : 25/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les modes de paiement (lecture depuis la base, écriture, vérifications),
' à partir de la table référentielle ref_modes_paiement.
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB
' - Utilise QueryModesPaiement pour les requêtes SQL
' - Retourne des objets ModePaiement
' - Suppression physique réservée aux modes de paiement non utilisés ; sinon soft-delete (Actif = 0)
' - paiements.id_mode_paiement est une clé étrangère (v2.50.0) : la base refuse elle-même la
'   suppression d'un mode utilisé ; libellé et code se modifient sans toucher l'historique.
' - V1.1.0 (21/10/2026, audit B.5) : ModePaiementEstUtilise supprimée (comparait le code au libellé
'   stocké : ne trouvait jamais d'usage ; aucun appelant).
' - V1.2.0 (22/10/2026, audit C.1, étape 3) : chaque écriture reçoit le ContexteOperation et
'   commence par ReglesReferentiels.ExigerDroitEcriture (SuperUser) ; SupprimerModePaiement, plus
'   appelée, supprimée.
'   Lignes système : refus explicite si la requête n'a concerné aucune ligne (ExecuterSurLigneModifiable).
' - V1.2.1 (25/10/2026, audit D-1) : Option Strict On ; identifiant lu par GetUInt64 (colonne BIGINT
'   UNSIGNED, propriété ULong).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionModesPaiement

#Region "Lecture"

    Public Function GetModesPaiementActifs() As List(Of ModePaiement)

        Return GetModesPaiementDepuisRequete(QueryModesPaiement.SelectModesPaiementActifs)

    End Function

    Public Function GetModesPaiement(afficherInactifs As Boolean) As List(Of ModePaiement)

        If afficherInactifs Then
            Return GetModesPaiementDepuisRequete(QueryModesPaiement.SelectModesPaiementTous)
        End If

        Return GetModesPaiementActifs()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : GetModesPaiementSaisie
    ' Date     : 27/09/2026
    ' Rôle     : Modes actifs proposables à la saisie manuelle (exclut les modes est_avoir).
    ' -------------------------------------------------------------------------------------------------
    Public Function GetModesPaiementSaisie() As List(Of ModePaiement)

        Return GetModesPaiementActifs().Where(Function(m) Not m.EstAvoir).ToList()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : GetModePaiementAvoir
    ' Date     : 27/09/2026
    ' Rôle     : Mode actif paramétré comme « avoir » (est_avoir = 1), ou Nothing si le référentiel
    '            n'est pas alimenté (migration v2.20.0 non appliquée).
    ' -------------------------------------------------------------------------------------------------
    Public Function GetModePaiementAvoir() As ModePaiement

        Return GetModesPaiementActifs().
            Where(Function(m) m.EstAvoir).
            OrderBy(Function(m) m.OrdreAffichage).
            FirstOrDefault()

    End Function

    Private Function GetModesPaiementDepuisRequete(query As String) As List(Of ModePaiement)

        Dim result As New List(Of ModePaiement)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()

                    While reader.Read()

                        Dim mode As New ModePaiement With {
                            .IdModePaiement = reader.GetUInt64("id_mode_paiement"),
                            .CodeModePaiement = reader.GetString("code_mode_paiement"),
                            .LibelleModePaiement = reader.GetString("libelle_mode_paiement"),
                            .EstAvoir = reader.GetBoolean("est_avoir"),
                            .Actif = reader.GetBoolean("actif"),
                            .EstSysteme = UtilsDataReader.LireBooleenOptionnel(reader, "est_systeme"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage")
                        }

                        result.Add(mode)

                    End While

                End Using
            End Using
        End Using

        Return result

    End Function

#End Region

#Region "Ecriture"

    Public Sub UpdateModePaiement(contexte As ContexteOperation, mode As ModePaiement)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryModesPaiement.UpdateModePaiement, conn)

                cmd.Parameters.AddWithValue("@id", mode.IdModePaiement)
                cmd.Parameters.AddWithValue("@code", mode.CodeModePaiement)
                cmd.Parameters.AddWithValue("@libelle", mode.LibelleModePaiement)
                cmd.Parameters.AddWithValue("@est_avoir", mode.EstAvoir)
                cmd.Parameters.AddWithValue("@actif", mode.Actif)
                cmd.Parameters.AddWithValue("@ordre", mode.OrdreAffichage)

                ReglesReferentiels.ExecuterSurLigneModifiable(cmd)

            End Using

        End Using

    End Sub

    Public Sub InsertModePaiement(contexte As ContexteOperation, mode As ModePaiement)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryModesPaiement.InsertModePaiement, conn)

                cmd.Parameters.AddWithValue("@code", mode.CodeModePaiement)
                cmd.Parameters.AddWithValue("@libelle", mode.LibelleModePaiement)
                cmd.Parameters.AddWithValue("@est_avoir", mode.EstAvoir)
                cmd.Parameters.AddWithValue("@actif", mode.Actif)
                cmd.Parameters.AddWithValue("@ordre", mode.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    Public Sub DesactiverModePaiement(contexte As ContexteOperation, idModePaiement As ULong)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryModesPaiement.DesactiverModePaiement, conn)
                cmd.Parameters.AddWithValue("@id", idModePaiement)
                ReglesReferentiels.ExecuterSurLigneModifiable(cmd)
            End Using
        End Using
    End Sub

#End Region

#Region "Vérifications"

    Public Function CodeModePaiementExiste(code As String,
                                           Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryModesPaiement.SelectCountCodeModePaiement, conn)

                cmd.Parameters.AddWithValue("@code", code)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    Public Function LibelleModePaiementExiste(libelle As String,
                                              Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryModesPaiement.SelectCountLibelleModePaiement, conn)

                cmd.Parameters.AddWithValue("@libelle", libelle)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

#End Region

End Module
