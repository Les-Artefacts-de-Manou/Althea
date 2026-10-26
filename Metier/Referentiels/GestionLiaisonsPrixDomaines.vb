' -------------------------------------------------------------------------------------------------
' Module   : GestionLiaisonsPrixDomaines
' Projet   : Althéa
' Version  : V1.1.0
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les rattachements historisés entre un tarif de séance et un domaine (lia_prix_seance_domaines).
' Ces rattachements pilotent la résolution du tarif conventionné à la date de la séance (D-POR-4).
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB et QueryLiaisonsPrixDomaines pour les requêtes SQL
' - La liaison est HISTORISÉE : faire évoluer un tarif consiste à clôturer la période en cours
'   puis à créer une nouvelle liaison, jamais à réécrire une période déjà appliquée
' - ValiderLiaison centralise tous les contrôles de cohérence afin que les écrans affichent
'   exactement les mêmes messages
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : chaque écriture reçoit le ContexteOperation et
'   commence par ReglesReferentiels.ExigerDroitEcriture (SuperUser) ; UpdateLiaison, plus appelée,
'   supprimée.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionLiaisonsPrixDomaines

#Region "Lecture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetLiaisonsParPrix
    ' Rôle       : Retourne tous les rattachements d'un tarif de séance à ses domaines.
    ' Paramètres : idPrixSeance - identifiant du tarif
    ' Retour     : List(Of LiaisonPrixDomaine) triée par domaine puis par période décroissante
    ' -------------------------------------------------------------------------------------------------
    Public Function GetLiaisonsParPrix(idPrixSeance As ULong) As List(Of LiaisonPrixDomaine)

        Return GetLiaisons(QueryLiaisonsPrixDomaines.SelectLiaisonsParPrix,
                           "@idPrixSeance", idPrixSeance)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetLiaisonsParDomaine
    ' Rôle       : Retourne tous les rattachements d'un domaine à ses tarifs conventionnés.
    ' Paramètres : idDomaine - identifiant du domaine
    ' Retour     : List(Of LiaisonPrixDomaine) triée par période décroissante
    ' -------------------------------------------------------------------------------------------------
    Public Function GetLiaisonsParDomaine(idDomaine As ULong) As List(Of LiaisonPrixDomaine)

        Return GetLiaisons(QueryLiaisonsPrixDomaines.SelectLiaisonsParDomaine,
                           "@idDomaine", idDomaine)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetLiaisons
    ' Rôle       : Exécute une requête de lecture paramétrée par un identifiant unique.
    ' Paramètres : query - requête SQL ; nomParametre - nom du paramètre ; valeur - identifiant
    ' Retour     : List(Of LiaisonPrixDomaine)
    ' Remarques  : Méthode privée, factorisation du mapping.
    ' -------------------------------------------------------------------------------------------------
    Private Function GetLiaisons(query As String,
                                 nomParametre As String,
                                 valeur As ULong) As List(Of LiaisonPrixDomaine)

        Dim result As New List(Of LiaisonPrixDomaine)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)

                cmd.Parameters.AddWithValue(nomParametre, valeur)

                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        result.Add(MapLiaison(reader))
                    End While
                End Using

            End Using
        End Using

        Return result

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : MapLiaison
    ' Rôle       : Convertit la ligne courante du lecteur en objet LiaisonPrixDomaine.
    ' Remarques  : date_fin_validite peut être NULL (« toujours en vigueur »).
    ' -------------------------------------------------------------------------------------------------
    Private Function MapLiaison(reader As MySqlDataReader) As LiaisonPrixDomaine

        Dim ordinalDateFin As Integer = reader.GetOrdinal("date_fin_validite")

        Return New LiaisonPrixDomaine With {
            .IdLiaison = CULng(reader.GetInt64("id_lia_prix_seance_domaine")),
            .IdPrixSeance = CULng(reader.GetInt64("id_prix_seance")),
            .IdDomaine = CULng(reader.GetInt64("id_domaine")),
            .CodeDomaine = reader.GetString("code_domaine"),
            .LibelleDomaine = reader.GetString("libelle_domaine"),
            .CodePrixSeance = reader.GetString("code_prix_seance"),
            .LibellePrixSeance = reader.GetString("libelle_prix_seance"),
            .Montant = reader.GetDecimal("montant"),
            .DateDebutValidite = reader.GetDateTime("date_debut_validite"),
            .DateFinValidite = If(reader.IsDBNull(ordinalDateFin),
                                  CType(Nothing, Date?),
                                  reader.GetDateTime(ordinalDateFin)),
            .Actif = reader.GetBoolean("actif")
        }

    End Function

#End Region

#Region "Vérifications"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : PeriodeChevauche
    ' Rôle       : Indique si une période proposée chevauche une période existante du même couple
    '              tarif / domaine.
    ' Paramètres : idPrixSeance, idDomaine, dateDebut, dateFin (Nothing = toujours en vigueur),
    '              idExclu - rattachement à exclure lors d'une modification
    ' Retour     : Boolean
    ' -------------------------------------------------------------------------------------------------
    Public Function PeriodeChevauche(idPrixSeance As ULong,
                                     idDomaine As ULong,
                                     dateDebut As Date,
                                     dateFin As Date?,
                                     Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiaisonsPrixDomaines.SelectCountChevauchementLiaison, conn)

                cmd.Parameters.AddWithValue("@idPrixSeance", idPrixSeance)
                cmd.Parameters.AddWithValue("@idDomaine", idDomaine)
                cmd.Parameters.AddWithValue("@dateDebut", dateDebut)
                cmd.Parameters.AddWithValue("@dateFin", ValeurDateFin(dateFin))
                cmd.Parameters.AddWithValue("@idExclu", idExclu)

                Return Convert.ToInt32(cmd.ExecuteScalar()) > 0

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : AutreTarifActifSurPeriode
    ' Rôle       : Indique si un AUTRE tarif est déjà rattaché activement au même domaine sur une
    '              période qui chevauche celle proposée.
    ' Retour     : Boolean
    ' Remarques  : Deux tarifs conventionnés simultanés rendraient la résolution ambiguë.
    ' -------------------------------------------------------------------------------------------------
    Public Function AutreTarifActifSurPeriode(idPrixSeance As ULong,
                                              idDomaine As ULong,
                                              dateDebut As Date,
                                              dateFin As Date?,
                                              Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiaisonsPrixDomaines.SelectCountAutreTarifActifDomaine, conn)

                cmd.Parameters.AddWithValue("@idPrixSeance", idPrixSeance)
                cmd.Parameters.AddWithValue("@idDomaine", idDomaine)
                cmd.Parameters.AddWithValue("@dateDebut", dateDebut)
                cmd.Parameters.AddWithValue("@dateFin", ValeurDateFin(dateFin))
                cmd.Parameters.AddWithValue("@idExclu", idExclu)

                Return Convert.ToInt32(cmd.ExecuteScalar()) > 0

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : PeriodeComporteSeances
    ' Rôle       : Indique si des séances ont déjà été enregistrées sur la période d'un rattachement.
    ' Retour     : Boolean
    ' Remarques  : Utilisé pour avertir avant une suppression d'historique tarifaire.
    ' -------------------------------------------------------------------------------------------------
    Public Function PeriodeComporteSeances(idDomaine As ULong,
                                           dateDebut As Date,
                                           dateFin As Date?) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiaisonsPrixDomaines.SelectCountSeancesSurPeriodeLiaison, conn)

                cmd.Parameters.AddWithValue("@idDomaine", idDomaine)
                cmd.Parameters.AddWithValue("@dateDebut", dateDebut)
                cmd.Parameters.AddWithValue("@dateFin", ValeurDateFin(dateFin))

                Return Convert.ToInt32(cmd.ExecuteScalar()) > 0

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ValiderLiaison
    ' Rôle       : Contrôle la cohérence d'un rattachement avant enregistrement.
    ' Paramètres : liaison - rattachement à contrôler
    ' Retour     : String - message d'erreur destiné à l'utilisateur, ou chaîne vide si valide
    ' Remarques  : Centralise les règles pour que tous les écrans affichent les mêmes messages.
    ' -------------------------------------------------------------------------------------------------
    Public Function ValiderLiaison(liaison As LiaisonPrixDomaine) As String

        If liaison Is Nothing Then
            Return "Aucun rattachement à enregistrer."
        End If

        If liaison.IdPrixSeance = 0 Then
            Return "Veuillez sélectionner un tarif de séance."
        End If

        If liaison.IdDomaine = 0 Then
            Return "Veuillez sélectionner un domaine."
        End If

        If liaison.DateFinValidite.HasValue AndAlso
           liaison.DateFinValidite.Value < liaison.DateDebutValidite Then

            Return "La date de fin de validité ne peut pas être antérieure à la date de début." &
                   Environment.NewLine &
                   "Laissez la date de fin vide si le tarif reste en vigueur sans échéance."
        End If

        If PeriodeChevauche(liaison.IdPrixSeance,
                            liaison.IdDomaine,
                            liaison.DateDebutValidite,
                            liaison.DateFinValidite,
                            liaison.IdLiaison) Then

            Return "Ce tarif est déjà rattaché à ce domaine sur une période qui se chevauche." &
                   Environment.NewLine &
                   "Clôturez d'abord la période existante avant d'en créer une nouvelle."
        End If

        If liaison.Actif AndAlso
           AutreTarifActifSurPeriode(liaison.IdPrixSeance,
                                     liaison.IdDomaine,
                                     liaison.DateDebutValidite,
                                     liaison.DateFinValidite,
                                     liaison.IdLiaison) Then

            Return "Un autre tarif est déjà actif pour ce domaine sur cette période." &
                   Environment.NewLine &
                   "Le tarif conventionné deviendrait ambigu : clôturez le tarif précédent à la veille " &
                   "de la nouvelle date de début."
        End If

        Return String.Empty

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ValeurDateFin
    ' Rôle       : Convertit une date de fin optionnelle en valeur SQL (NULL si absente).
    ' -------------------------------------------------------------------------------------------------
    Private Function ValeurDateFin(dateFin As Date?) As Object

        If dateFin.HasValue Then Return dateFin.Value

        Return DBNull.Value

    End Function

#End Region

#Region "Ecriture"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InsertLiaison
    ' Rôle       : Crée un nouveau rattachement tarif / domaine.
    ' Paramètres : liaison - rattachement à créer (validé en amont par ValiderLiaison)
    ' -------------------------------------------------------------------------------------------------
    Public Sub InsertLiaison(contexte As ContexteOperation, liaison As LiaisonPrixDomaine)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiaisonsPrixDomaines.InsertLiaison, conn)

                cmd.Parameters.AddWithValue("@idPrixSeance", liaison.IdPrixSeance)
                cmd.Parameters.AddWithValue("@idDomaine", liaison.IdDomaine)
                cmd.Parameters.AddWithValue("@dateDebut", liaison.DateDebutValidite)
                cmd.Parameters.AddWithValue("@dateFin", ValeurDateFin(liaison.DateFinValidite))
                cmd.Parameters.AddWithValue("@actif", liaison.Actif)

                cmd.ExecuteNonQuery()

            End Using
        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : CloturerLiaison
    ' Rôle       : Clôture un rattachement à une date donnée, en préservant l'historique.
    ' Paramètres : idLiaison - rattachement à clôturer ; dateFin - dernier jour d'application
    ' -------------------------------------------------------------------------------------------------
    Public Sub CloturerLiaison(contexte As ContexteOperation, idLiaison As ULong, dateFin As Date)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiaisonsPrixDomaines.CloturerLiaison, conn)

                cmd.Parameters.AddWithValue("@id", idLiaison)
                cmd.Parameters.AddWithValue("@dateFin", dateFin)

                cmd.ExecuteNonQuery()

            End Using
        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DefinirActivationLiaison
    ' Rôle       : Active ou désactive un rattachement (soft-delete).
    ' -------------------------------------------------------------------------------------------------
    Public Sub DefinirActivationLiaison(contexte As ContexteOperation, idLiaison As ULong, actif As Boolean)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiaisonsPrixDomaines.DefinirActivationLiaison, conn)

                cmd.Parameters.AddWithValue("@id", idLiaison)
                cmd.Parameters.AddWithValue("@actif", actif)

                cmd.ExecuteNonQuery()

            End Using
        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SupprimerLiaison
    ' Rôle       : Supprime physiquement un rattachement créé par erreur.
    ' Remarques  : Vérifier au préalable PeriodeComporteSeances afin de ne pas perdre un historique
    '              tarifaire déjà appliqué à des séances.
    ' -------------------------------------------------------------------------------------------------
    Public Sub SupprimerLiaison(contexte As ContexteOperation, idLiaison As ULong)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiaisonsPrixDomaines.SupprimerLiaison, conn)

                cmd.Parameters.AddWithValue("@id", idLiaison)

                cmd.ExecuteNonQuery()

            End Using
        End Using

    End Sub

#End Region

End Module
