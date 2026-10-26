' -------------------------------------------------------------------------------------------------
' Module   : GestionLiaisonsNaturesDomaines
' Projet   : Althéa
' Version  : V1.2.0
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les rattachements entre les natures de séance et les domaines (lia_natures_seance_domaines).
' Ces rattachements déterminent les natures proposées à la saisie d'une séance selon le domaine
' du dossier (D-POR-5).
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB et QueryLiaisonsNaturesDomaines pour les requêtes SQL
' - Cette liaison n'est PAS historisée : pas de période de validité, seulement un état actif
' - Dès qu'une séance a été saisie avec le couple nature / domaine, la suppression est refusée :
'   la désactivation prend le relais afin de préserver l'historique clinique
' - ValiderLiaison centralise les contrôles pour que tous les écrans affichent les mêmes messages
' - V1.2.0 (22/10/2026, audit C.1, étape 3) : chaque écriture reçoit le ContexteOperation et
'   commence par ReglesReferentiels.ExigerDroitEcriture (SuperUser).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionLiaisonsNaturesDomaines

#Region "Lecture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetLiaisonsParDomaine
    ' Rôle       : Retourne les natures de séance rattachées à un domaine, actives ou non.
    ' Paramètres : idDomaine - identifiant du domaine
    ' Retour     : List(Of LiaisonNatureDomaine) triée par ordre d'affichage des natures
    ' -------------------------------------------------------------------------------------------------
    Public Function GetLiaisonsParDomaine(idDomaine As ULong) As List(Of LiaisonNatureDomaine)

        Return GetLiaisons(QueryLiaisonsNaturesDomaines.SelectLiaisonsParDomaine,
                           "@idDomaine", idDomaine)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetLiaisonsParNature
    ' Rôle       : Retourne les domaines auxquels une nature de séance est rattachée.
    ' Paramètres : idNatureSeance - identifiant de la nature de séance
    ' Retour     : List(Of LiaisonNatureDomaine) triée par ordre d'affichage des domaines
    ' -------------------------------------------------------------------------------------------------
    Public Function GetLiaisonsParNature(idNatureSeance As ULong) As List(Of LiaisonNatureDomaine)

        Return GetLiaisons(QueryLiaisonsNaturesDomaines.SelectLiaisonsParNature,
                           "@idNatureSeance", idNatureSeance)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetLiaisons
    ' Rôle       : Exécute une requête de lecture paramétrée par un identifiant unique.
    ' Retour     : List(Of LiaisonNatureDomaine)
    ' Remarques  : Méthode privée, factorisation du mapping.
    ' -------------------------------------------------------------------------------------------------
    Private Function GetLiaisons(query As String,
                                 nomParametre As String,
                                 valeur As ULong) As List(Of LiaisonNatureDomaine)

        Dim result As New List(Of LiaisonNatureDomaine)

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
    ' Rôle       : Convertit la ligne courante du lecteur en objet LiaisonNatureDomaine.
    ' Remarques  : id_categorie_quota est NULL pour une nature hors quota dans ce domaine.
    ' -------------------------------------------------------------------------------------------------
    Private Function MapLiaison(reader As MySqlDataReader) As LiaisonNatureDomaine

        Dim ordinalCategorie As Integer = reader.GetOrdinal("id_categorie_quota")
        Dim ordinalLibelle As Integer = reader.GetOrdinal("libelle_categorie_quota")

        Return New LiaisonNatureDomaine With {
            .IdLiaison = CULng(reader.GetInt64("id_lia_nature_domaine")),
            .IdNatureSeance = CULng(reader.GetInt64("id_nature_seance")),
            .IdDomaine = CULng(reader.GetInt64("id_domaine")),
            .CodeNatureSeance = reader.GetString("code_nature_seance"),
            .LibelleNatureSeance = reader.GetString("libelle_nature_seance"),
            .IdCategorieQuota = If(reader.IsDBNull(ordinalCategorie),
                                   CType(Nothing, ULong?),
                                   Convert.ToUInt64(reader.GetValue(ordinalCategorie))),
            .LibelleCategorieQuota = If(reader.IsDBNull(ordinalLibelle),
                                        String.Empty,
                                        reader.GetString(ordinalLibelle)),
            .ActifNature = reader.GetBoolean("actif_nature"),
            .CodeDomaine = reader.GetString("code_domaine"),
            .LibelleDomaine = reader.GetString("libelle_domaine"),
            .Actif = reader.GetBoolean("actif")
        }

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetNaturesDisponiblesPourDomaine
    ' Rôle       : Retourne les natures actives pas encore rattachées à un domaine.
    ' Paramètres : idDomaine - identifiant du domaine
    ' Retour     : List(Of NatureSeance) destinée à la liste de sélection du panneau
    ' Remarques  : Évite que l'utilisateur tente un rattachement en doublon.
    ' -------------------------------------------------------------------------------------------------
    Public Function GetNaturesDisponiblesPourDomaine(idDomaine As ULong) As List(Of NatureSeance)

        Dim result As New List(Of NatureSeance)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiaisonsNaturesDomaines.SelectNaturesDisponiblesPourDomaine, conn)

                cmd.Parameters.AddWithValue("@idDomaine", idDomaine)

                Using reader = cmd.ExecuteReader()

                    While reader.Read()

                        result.Add(New NatureSeance With {
                            .IdNatureSeance = CULng(reader.GetInt64("id_nature_seance")),
                            .CodeNatureSeance = reader.GetString("code_nature_seance"),
                            .LibelleNatureSeance = reader.GetString("libelle_nature_seance"),
                            .Actif = reader.GetBoolean("actif"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage")
                        })

                    End While

                End Using

            End Using
        End Using

        Return result

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetDomainesDisponiblesPourNature
    ' Rôle       : Retourne les domaines actifs auxquels une nature n'est pas encore rattachée.
    ' Paramètres : idNatureSeance - identifiant de la nature de séance
    ' Retour     : List(Of Domaine) destinée à la liste de sélection du panneau
    ' -------------------------------------------------------------------------------------------------
    Public Function GetDomainesDisponiblesPourNature(idNatureSeance As ULong) As List(Of Domaine)

        Dim result As New List(Of Domaine)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiaisonsNaturesDomaines.SelectDomainesDisponiblesPourNature, conn)

                cmd.Parameters.AddWithValue("@idNatureSeance", idNatureSeance)

                Using reader = cmd.ExecuteReader()
                    While reader.Read()

                        result.Add(New Domaine With {
                            .IdDomaine = CULng(reader.GetInt64("id_domaine")),
                            .CodeDomaine = reader.GetString("code_domaine"),
                            .LibelleDomaine = reader.GetString("libelle_domaine"),
                            .TarifConvention = reader.GetBoolean("tarif_convention"),
                            .NoShowFacturable = reader.GetBoolean("no_show_facturable"),
                            .SoumisQuota = reader.GetBoolean("soumis_quota"),
                            .Actif = reader.GetBoolean("actif"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage")
                        })

                    End While
                End Using

            End Using
        End Using

        Return result

    End Function

#End Region

#Region "Vérifications"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : LiaisonExiste
    ' Rôle       : Indique si le couple nature / domaine est déjà enregistré.
    ' Paramètres : idNatureSeance, idDomaine, idExclu - rattachement à ignorer (0 en création)
    ' Retour     : Boolean
    ' -------------------------------------------------------------------------------------------------
    Public Function LiaisonExiste(idNatureSeance As ULong,
                                  idDomaine As ULong,
                                  Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiaisonsNaturesDomaines.SelectCountLiaisonExistante, conn)

                cmd.Parameters.AddWithValue("@idNatureSeance", idNatureSeance)
                cmd.Parameters.AddWithValue("@idDomaine", idDomaine)
                cmd.Parameters.AddWithValue("@idExclu", idExclu)

                Return Convert.ToInt32(cmd.ExecuteScalar()) > 0

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : LiaisonUtiliseeParSeances
    ' Rôle       : Indique si des séances ont déjà été saisies avec cette nature dans ce domaine.
    ' Retour     : Boolean
    ' Remarques  : Utilisé pour refuser la suppression et orienter vers la désactivation.
    ' -------------------------------------------------------------------------------------------------
    Public Function LiaisonUtiliseeParSeances(idNatureSeance As ULong, idDomaine As ULong) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiaisonsNaturesDomaines.SelectCountSeancesNatureDomaine, conn)

                cmd.Parameters.AddWithValue("@idNatureSeance", idNatureSeance)
                cmd.Parameters.AddWithValue("@idDomaine", idDomaine)

                Return Convert.ToInt32(cmd.ExecuteScalar()) > 0

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CompterLiaisonsActivesDomaine
    ' Rôle       : Compte les natures réellement proposées à la saisie pour un domaine.
    ' Paramètres : idDomaine - domaine concerné ; idExclu - rattachement en cours de retrait
    ' Retour     : Integer
    ' Remarques  : Un domaine sans nature active rend la saisie de séance impossible.
    ' -------------------------------------------------------------------------------------------------
    Public Function CompterLiaisonsActivesDomaine(idDomaine As ULong,
                                                  Optional idExclu As ULong = 0) As Integer

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiaisonsNaturesDomaines.SelectCountLiaisonsActivesDomaine, conn)

                cmd.Parameters.AddWithValue("@idDomaine", idDomaine)
                cmd.Parameters.AddWithValue("@idExclu", idExclu)

                Return Convert.ToInt32(cmd.ExecuteScalar())

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ValiderLiaison
    ' Rôle       : Contrôle la cohérence d'un rattachement avant enregistrement.
    ' Paramètres : liaison - rattachement à contrôler
    ' Retour     : String - message d'erreur destiné à l'utilisateur, ou chaîne vide si valide
    ' -------------------------------------------------------------------------------------------------
    Public Function ValiderLiaison(liaison As LiaisonNatureDomaine) As String

        If liaison Is Nothing Then
            Return "Aucun rattachement à enregistrer."
        End If

        If liaison.IdNatureSeance = 0 Then
            Return "Veuillez sélectionner une nature de séance."
        End If

        If liaison.IdDomaine = 0 Then
            Return "Veuillez sélectionner un domaine."
        End If

        If LiaisonExiste(liaison.IdNatureSeance, liaison.IdDomaine, liaison.IdLiaison) Then
            Return "Cette nature de séance est déjà rattachée à ce domaine." &
                   Environment.NewLine &
                   "Si elle n'apparaît pas à la saisie, réactivez le rattachement existant."
        End If

        Return String.Empty

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : AvertissementRetraitLiaison
    ' Rôle       : Message d'avertissement avant désactivation d'un rattachement, si le domaine
    '              risque de se retrouver sans aucune nature proposée.
    ' Paramètres : liaison - rattachement en cours de retrait
    ' Retour     : String - avertissement à afficher, ou chaîne vide si aucun risque
    ' Remarques  : L'utilisateur reste libre de confirmer : il s'agit d'un garde-fou, pas d'un blocage.
    ' -------------------------------------------------------------------------------------------------
    Public Function AvertissementRetraitLiaison(liaison As LiaisonNatureDomaine) As String

        If liaison Is Nothing OrElse Not liaison.Actif Then Return String.Empty

        If CompterLiaisonsActivesDomaine(liaison.IdDomaine, liaison.IdLiaison) > 0 Then
            Return String.Empty
        End If

        Return $"Après ce retrait, le domaine « {liaison.LibelleDomaine} » n'aurait plus aucune " &
               "nature de séance proposée." & Environment.NewLine &
               "Il deviendrait impossible d'y enregistrer une séance. Confirmez-vous le retrait ?"

    End Function

#End Region

#Region "Ecriture"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InsertLiaison
    ' Rôle       : Rattache une nature de séance à un domaine.
    ' Paramètres : liaison - rattachement à créer (validé en amont par ValiderLiaison)
    ' -------------------------------------------------------------------------------------------------
    Public Sub InsertLiaison(contexte As ContexteOperation, liaison As LiaisonNatureDomaine)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiaisonsNaturesDomaines.InsertLiaison, conn)

                cmd.Parameters.AddWithValue("@idNatureSeance", liaison.IdNatureSeance)
                cmd.Parameters.AddWithValue("@idDomaine", liaison.IdDomaine)
                cmd.Parameters.AddWithValue("@idCategorieQuota",
                                            If(liaison.IdCategorieQuota.HasValue, CObj(liaison.IdCategorieQuota.Value), DBNull.Value))
                cmd.Parameters.AddWithValue("@actif", liaison.Actif)

                cmd.ExecuteNonQuery()

            End Using
        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DefinirActivationLiaison
    ' Rôle       : Active ou désactive un rattachement nature / domaine.
    ' Remarques  : Préserve les séances déjà enregistrées avec cette nature.
    ' -------------------------------------------------------------------------------------------------
    Public Sub DefinirActivationLiaison(contexte As ContexteOperation, idLiaison As ULong, actif As Boolean)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiaisonsNaturesDomaines.DefinirActivationLiaison, conn)

                cmd.Parameters.AddWithValue("@id", idLiaison)
                cmd.Parameters.AddWithValue("@actif", actif)

                cmd.ExecuteNonQuery()

            End Using
        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SupprimerLiaison
    ' Rôle       : Supprime un rattachement créé par erreur.
    ' Remarques  : Vérifier au préalable LiaisonUtiliseeParSeances : si des séances existent,
    '              utiliser DefinirActivationLiaison plutôt que la suppression.
    ' -------------------------------------------------------------------------------------------------
    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DefinirCategorieQuota
    ' Rôle       : Fixe la catégorie de quota consommée par la nature dans le domaine du rattachement.
    ' Paramètres : idLiaison ; idCategorieQuota - Nothing pour « hors quota »
    ' Remarques  : Invalide le cache des quotas : les compteurs des dossiers en dépendent.
    ' -------------------------------------------------------------------------------------------------
    Public Sub DefinirCategorieQuota(contexte As ContexteOperation, idLiaison As ULong, idCategorieQuota As ULong?)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiaisonsNaturesDomaines.DefinirCategorieQuotaLiaison, conn)

                cmd.Parameters.AddWithValue("@id", idLiaison)
                cmd.Parameters.AddWithValue("@id_categorie_quota",
                                            If(idCategorieQuota.HasValue, CObj(idCategorieQuota.Value), DBNull.Value))
                cmd.ExecuteNonQuery()

            End Using
        End Using

        GestionQuotasInami.ViderCache()

    End Sub

    Public Sub SupprimerLiaison(contexte As ContexteOperation, idLiaison As ULong)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryLiaisonsNaturesDomaines.SupprimerLiaison, conn)

                cmd.Parameters.AddWithValue("@id", idLiaison)

                cmd.ExecuteNonQuery()

            End Using
        End Using

    End Sub

#End Region

End Module
