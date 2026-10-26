' -------------------------------------------------------------------------------------------------
' Module   : GestionQuotasInami
' Projet   : Althéa
' Version  : V1.6.0
' Date     : 23/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les règles de quota annuel de prestations remboursées (lecture, écriture, vérifications),
' à partir de la table référentielle ref_quotas.
'
' Contexte métier :
' Les plafonds INAMI (10 séances de soutien, 20 de traitement) étaient auparavant codés en dur.
' Ils sont désormais paramétrés par domaine et historisés par période de validité, afin de pouvoir
' suivre une révision de convention sans livraison de code et sans fausser les quotas passés.
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB et QueryQuotasInami pour les requêtes SQL
' - Les règles sont mises en cache : elles sont lues à chaque calcul de quota et ne changent
'   qu'exceptionnellement. Toute écriture invalide le cache.
' - Si aucune règle n'est paramétrée pour un domaine, la lecture retourne Nothing : l'appelant
'   applique alors ses valeurs de repli plutôt que de bloquer l'utilisateur.
'
' Historique :
' - V1.3.0 (21/10/2026, audit B.2) : AjouterRegleEnChaine (arrêt de la règle précédente et ajout de
'   la nouvelle en une transaction, chevauchement contrôlé dans la transaction) ; ValiderCoherence
'   (contrôles sans base) séparée de ValiderRegle ; InsertQuotaInami passe par InsererRegle.
' - V1.4.0 (22/10/2026, audit B.6) : les règles ne sont plus gardées en mémoire que par domaine
'   (GetRegleApplicableParDossier lit d'abord le domaine actuel du dossier) ; le cache n'a plus à
'   connaître les changements de domaine d'un dossier.
' - V1.5.0 (22/10/2026, audit C.1, étape 3) : chaque écriture reçoit le ContexteOperation et
'   commence par ReglesReferentiels.ExigerDroitEcriture (SuperUser) ; InsertQuotaInami, plus
'   appelée, supprimée.
' - V1.6.0 (23/10/2026, audit C.2 étape 3) : GetQuotasParDomaine (lecture que l'écran Domaines et sa
'   synthèse faisaient chacun de leur côté).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionQuotasInami

#Region "Cache"

    ' Cache des règles applicables, indexé par domaine et date de référence.
    ' La règle de quota ne change qu'à l'occasion d'une révision de convention : la relire à chaque
    ' affichage de dossier serait un coût inutile.
    Private ReadOnly _cacheReglesApplicables As New Dictionary(Of String, QuotaInamiRegle)

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ViderCache
    ' Version   : V1.0.0
    ' Date      : 19/09/2026
    '
    ' Rôle      :
    ' Vide le cache des règles de quota.
    '
    ' Remarques :
    ' - Appelée après toute écriture : sans cela, un plafond corrigé ne serait pris en compte qu'au
    '   prochain démarrage de l'application, ce qui donnerait à l'utilisateur l'impression que sa
    '   modification n'a pas été enregistrée.
    ' -------------------------------------------------------------------------------------------------
    Public Sub ViderCache()

        SyncLock _cacheReglesApplicables
            _cacheReglesApplicables.Clear()
        End SyncLock

    End Sub

#End Region

#Region "Lecture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : GetQuotasInami
    ' Version   : V1.0.0
    ' Date      : 19/09/2026
    '
    ' Rôle      :
    ' Retourne toutes les règles paramétrées, pour l'écran d'administration.
    '
    ' Paramètres :
    ' - afficherInactifs : True pour inclure les règles désactivées (historique)
    ' -------------------------------------------------------------------------------------------------
    Public Function GetQuotasInami(afficherInactifs As Boolean) As List(Of QuotaInamiRegle)

        Dim result As New List(Of QuotaInamiRegle)

        Try

            Using conn = DatabaseManager.OpenConnection()
                Using cmd As New MySqlCommand(QueryQuotasInami.SelectQuotasInamiTous, conn)
                    Using reader = cmd.ExecuteReader()

                        While reader.Read()
                            result.Add(MapRegle(reader))
                        End While

                    End Using
                End Using

                ChargerPlafonds(conn, result)

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur GetQuotasInami.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

        If afficherInactifs Then Return result

        Return result.Where(Function(r) r.Actif).ToList()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : GetQuotasParDomaine
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Retourne toutes les règles d'un domaine, inactives comprises (historique, périodes sans règle).
    '
    ' Paramètres :
    ' - idDomaine : domaine concerné
    ' -------------------------------------------------------------------------------------------------
    Public Function GetQuotasParDomaine(idDomaine As ULong) As List(Of QuotaInamiRegle)

        Return GetQuotasInami(True).Where(Function(r) r.IdDomaine = idDomaine).ToList()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : GetRegleApplicableParDossier
    ' Version   : V2.0.0
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' Retourne la règle de quota applicable au domaine d'un dossier à une date donnée.
    '
    ' Paramètres :
    ' - idDossier     : dossier évalué
    ' - dateReference : date à laquelle la règle doit s'appliquer
    '
    ' Retour    :
    ' - QuotaInamiRegle, ou Nothing si aucune règle n'est paramétrée pour ce domaine
    '
    ' Remarques :
    ' - La date de référence est celle du DÉBUT de la période de quota évaluée, et non la date du
    '   jour : une période ouverte sous l'ancienne convention doit se terminer sous cette convention,
    '   sinon les compteurs déjà communiqués au patient changeraient rétroactivement.
    ' - L'absence de règle n'est pas une erreur bloquante, mais elle est journalisée : elle traduit
    '   un référentiel incomplet que l'utilisateur doit compléter.
    ' - V2.0.0 (22/10/2026, audit B.6) : le domaine ACTUEL du dossier est lu, puis la règle de ce
    '   domaine (GetRegleApplicableParDomaine, en mémoire par domaine). La règle était gardée en
    '   mémoire par dossier : un dossier changé de domaine gardait celle de l'ancien jusqu'au
    '   redémarrage.
    ' -------------------------------------------------------------------------------------------------
    Public Function GetRegleApplicableParDossier(idDossier As Long,
                                                 dateReference As Date) As QuotaInamiRegle

        Dim idDomaine As ULong

        Try

            Using conn = DatabaseManager.OpenConnection()
                Using cmd As New MySqlCommand(QueryQuotasInami.SelectIdDomaineDossier, conn)
                    cmd.Parameters.AddWithValue("@id_dossier", idDossier)
                    Dim valeur As Object = cmd.ExecuteScalar()
                    If valeur Is Nothing OrElse valeur Is DBNull.Value Then
                        Return Nothing
                    End If
                    idDomaine = Convert.ToUInt64(valeur)
                End Using
            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur GetRegleApplicableParDossier (id={idDossier}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

        Return GetRegleApplicableParDomaine(idDomaine, dateReference)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : GetRegleApplicableParDomaine
    ' Version   : V1.0.0
    ' Date      : 19/09/2026
    '
    ' Rôle      :
    ' Retourne la règle de quota applicable à un domaine à une date donnée.
    '
    ' Retour    :
    ' - QuotaInamiRegle, ou Nothing si aucune règle n'est paramétrée pour ce domaine
    ' -------------------------------------------------------------------------------------------------
    Public Function GetRegleApplicableParDomaine(idDomaine As ULong,
                                                 dateReference As Date) As QuotaInamiRegle

        Dim cle As String = $"M{idDomaine}|{dateReference:yyyyMMdd}"

        SyncLock _cacheReglesApplicables
            Dim enCache As QuotaInamiRegle = Nothing
            If _cacheReglesApplicables.TryGetValue(cle, enCache) Then
                Return enCache
            End If
        End SyncLock

        Dim regle As QuotaInamiRegle = Nothing

        Try

            Using conn = DatabaseManager.OpenConnection()
                Using cmd As New MySqlCommand(QueryQuotasInami.SelectQuotaInamiApplicable, conn)

                    cmd.Parameters.AddWithValue("@id_domaine", idDomaine)
                    cmd.Parameters.AddWithValue("@date_reference", dateReference.Date)

                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then
                            regle = MapRegle(reader)
                        End If
                    End Using

                End Using

                If regle IsNot Nothing Then ChargerPlafonds(conn, New List(Of QuotaInamiRegle) From {regle})

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur GetRegleApplicableParDomaine (id={idDomaine}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

        SyncLock _cacheReglesApplicables
            _cacheReglesApplicables(cle) = regle
        End SyncLock

        Return regle

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : MapRegle
    ' Version   : V1.0.0
    ' Date      : 19/09/2026
    '
    ' Rôle      :
    ' Construit un objet QuotaInamiRegle à partir de la ligne courante du lecteur.
    ' -------------------------------------------------------------------------------------------------
    Private Function MapRegle(reader As MySqlDataReader) As QuotaInamiRegle

        Return New QuotaInamiRegle With {
            .IdQuotaInami = reader.GetUInt64("id_quota"),
            .IdDomaine = reader.GetUInt64("id_domaine"),
            .CodeDomaine = reader.GetString("code_domaine"),
            .LibelleDomaine = reader.GetString("libelle_domaine"),
            .SeuilAlerteTotal = reader.GetInt32("seuil_alerte_total"),
            .DureePeriodeMois = reader.GetInt32("duree_periode_mois"),
            .DateDebutValidite = reader.GetDateTime("date_debut_validite").Date,
            .DateFinValidite = If(reader.IsDBNull(reader.GetOrdinal("date_fin_validite")),
                                  CType(Nothing, Date?),
                                  reader.GetDateTime("date_fin_validite").Date),
            .Actif = reader.GetBoolean("actif")
        }

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerPlafonds
    '
    ' Rôle      :
    ' Alimente les plafonds par catégorie des règles fournies, en une seule requête.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerPlafonds(conn As MySqlConnection, regles As List(Of QuotaInamiRegle))

        If regles Is Nothing OrElse regles.Count = 0 Then Return

        Dim parId As Dictionary(Of ULong, QuotaInamiRegle) = regles.ToDictionary(Function(r) r.IdQuotaInami)
        Dim ids As String = String.Join(",", parId.Keys)

        Using cmd As New MySqlCommand(QueryQuotasInami.SelectPlafondsParQuotas.Replace("{IDS}", ids), conn)
            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    Dim regle As QuotaInamiRegle = Nothing
                    If parId.TryGetValue(reader.GetUInt64("id_quota"), regle) Then
                        regle.Plafonds.Add(New QuotaPlafond With {
                            .IdCategorieQuota = reader.GetUInt64("id_categorie_quota"),
                            .CodeCategorie = reader.GetString("code_categorie"),
                            .LibelleCategorie = reader.GetString("libelle_categorie"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage"),
                            .Plafond = reader.GetInt32("plafond")
                        })
                    End If
                End While
            End Using
        End Using

    End Sub

#End Region

#Region "Ecriture"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : UpdateQuotaInami
    ' Version   : V1.1.0
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' Met à jour une règle de quota existante.
    ' - V1.1.0 (22/10/2026, audit C.1) : reçoit le ContexteOperation ; droit d'écriture vérifié par le métier.
    ' -------------------------------------------------------------------------------------------------
    Public Sub UpdateQuotaInami(contexte As ContexteOperation, regle As QuotaInamiRegle)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Try

            Using conn = DatabaseManager.OpenConnection()
                Using tx = conn.BeginTransaction()

                    Using cmd As New MySqlCommand(QueryQuotasInami.UpdateQuotaInami, conn, tx)
                        AjouterParametresRegle(cmd, regle)
                        cmd.Parameters.AddWithValue("@id", regle.IdQuotaInami)
                        cmd.ExecuteNonQuery()
                    End Using

                    EnregistrerPlafonds(conn, tx, regle)

                    tx.Commit()

                End Using
            End Using

            ViderCache()

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur UpdateQuotaInami (id={regle.IdQuotaInami}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AjouterRegleEnChaine
    ' Version   : V1.1.0
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' Enregistre une nouvelle règle et, s'il y a lieu, arrête la veille de son début la règle ouverte
    ' qui la précède : les deux dans une seule transaction, tout ou rien.
    '
    ' Paramètres :
    ' - regle      : nouvelle règle (plafonds compris)
    ' - aRemplacer : règle ouverte et active du même domaine, commencée avant la nouvelle ; Nothing
    '                s'il n'y en a pas
    '
    ' Remarques :
    ' - Remplace l'enchaînement fait par l'écran Domaines (arrêt, contrôle, insertion, puis
    '   rétablissement à la main si l'ajout échouait — rétablissement qui pouvait lui-même échouer et
    '   laisser une période sans règle) (audit § 4.2).
    ' - Le chevauchement est contrôlé DANS la transaction, après l'arrêt de la règle précédente : vue
    '   d'une autre connexion, elle serait encore ouverte.
    ' - La règle précédente n'est arrêtée que si elle est encore ouverte en base (écran périmé : refus).
    '
    ' Exceptions :
    ' - InvalidOperationException : refus, message destiné à l'utilisatrice (rien n'est enregistré)
    ' - Erreurs base : journalisées puis relancées (rien n'est enregistré)
    ' - V1.1.0 (22/10/2026, audit C.1) : reçoit le ContexteOperation ; droit d'écriture vérifié par le métier.
    ' -------------------------------------------------------------------------------------------------
    Public Sub AjouterRegleEnChaine(contexte As ContexteOperation, regle As QuotaInamiRegle, aRemplacer As QuotaInamiRegle)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Dim erreur As String = ValiderCoherence(regle)
        If erreur <> String.Empty Then Throw New InvalidOperationException(erreur)

        If aRemplacer IsNot Nothing AndAlso
           (aRemplacer.IdDomaine <> regle.IdDomaine OrElse aRemplacer.DateFinValidite.HasValue OrElse
            aRemplacer.DateDebutValidite.Date >= regle.DateDebutValidite.Date) Then
            Throw New ArgumentException("La règle à arrêter doit être ouverte, du même domaine et commencer avant la nouvelle.",
                                        NameOf(aRemplacer))
        End If

        Dim veille As Date = regle.DateDebutValidite.Date.AddDays(-1)

        Try

            DatabaseManager.ExecuterEnTransaction(
                Sub(conn, tx)

                    If aRemplacer IsNot Nothing Then
                        Using cmd As New MySqlCommand(QueryQuotasInami.ArreterRegleOuverte, conn, tx)
                            cmd.Parameters.AddWithValue("@id", aRemplacer.IdQuotaInami)
                            cmd.Parameters.AddWithValue("@date_fin", veille)
                            If cmd.ExecuteNonQuery() = 0 Then
                                Throw New InvalidOperationException(
                                    "La règle actuelle a été modifiée entre-temps (elle n'est plus ouverte). Rechargez l'écran, puis recommencez.")
                            End If
                        End Using
                    End If

                    If PeriodeChevauche(regle.IdDomaine, regle.DateDebutValidite, regle.DateFinValidite, regle.IdQuotaInami, conn, tx) Then
                        Throw New InvalidOperationException(MESSAGE_CHEVAUCHEMENT)
                    End If

                    InsererRegle(regle, conn, tx)

                End Sub)

        Catch exRefus As InvalidOperationException
            ' Refus métier : la transaction est annulée, le message remonte tel quel.
            Throw
        Catch ex As Exception
            GestionLog.EcrireLog(
                $"Erreur AjouterRegleEnChaine (domaine={regle.IdDomaine}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )
            Throw
        End Try

        ViderCache()

        GestionLog.EcrireLog(
            $"Règle de quota {regle.IdQuotaInami} ajoutée (domaine={regle.IdDomaine})" &
            If(aRemplacer Is Nothing, ".", $" ; règle {aRemplacer.IdQuotaInami} arrêtée au {veille:dd/MM/yyyy}."),
            GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DefinirActivationQuotaInami
    ' Version   : V1.1.0
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' Active ou désactive une règle (soft-delete).
    '
    ' Remarques :
    ' - La désactivation est préférée à la suppression : une règle ayant servi à calculer des quotas
    '   doit rester consultable pour justifier les compteurs historiques.
    ' - V1.1.0 (22/10/2026, audit C.1) : reçoit le ContexteOperation ; droit d'écriture vérifié par le métier.
    ' -------------------------------------------------------------------------------------------------
    Public Sub DefinirActivationQuotaInami(contexte As ContexteOperation, idQuotaInami As ULong, actif As Boolean)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Try

            Using conn = DatabaseManager.OpenConnection()
                Using cmd As New MySqlCommand(QueryQuotasInami.DefinirActivationQuotaInami, conn)

                    cmd.Parameters.AddWithValue("@id", idQuotaInami)
                    cmd.Parameters.AddWithValue("@actif", actif)
                    cmd.ExecuteNonQuery()

                End Using
            End Using

            ViderCache()

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur DefinirActivationQuotaInami (id={idQuotaInami}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : PeriodesSansRegle
    ' Version   : V1.0.0
    ' Date      : 14/10/2026
    '
    ' Rôle      :
    ' Repère les périodes qu'aucune règle active ne couvre, entre deux règles ou après la dernière
    ' (si elle s'arrête). Les séances de ces périodes ne sont soumises à aucun quota.
    '
    ' Paramètres :
    ' - regles : règles d'un même domaine
    '
    ' Retour    :
    ' - Liste de périodes (Fin = Nothing : période ouverte, sans règle à ce jour ni après)
    '
    ' Remarques :
    ' - Avant la première règle, il n'y a pas de « trou » : le domaine n'avait pas encore de quota.
    ' -------------------------------------------------------------------------------------------------
    Public Function PeriodesSansRegle(regles As IEnumerable(Of QuotaInamiRegle)) As List(Of (Debut As Date, Fin As Date?))

        Dim trous As New List(Of (Debut As Date, Fin As Date?))
        If regles Is Nothing Then Return trous

        Dim actives As List(Of QuotaInamiRegle) =
            regles.Where(Function(r) r.Actif).OrderBy(Function(r) r.DateDebutValidite).ToList()

        For i As Integer = 0 To actives.Count - 1

            Dim fin As Date? = actives(i).DateFinValidite
            If Not fin.HasValue Then Continue For

            Dim lendemain As Date = fin.Value.Date.AddDays(1)

            If i < actives.Count - 1 Then
                Dim debutSuivante As Date = actives(i + 1).DateDebutValidite.Date
                If lendemain < debutSuivante Then trous.Add((lendemain, CType(debutSuivante.AddDays(-1), Date?)))
            Else
                trous.Add((lendemain, CType(Nothing, Date?)))
            End If

        Next

        Return trous

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : ControlerSuppression
    ' Version   : V1.0.0
    ' Date      : 14/10/2026
    '
    ' Rôle      :
    ' Dit si une règle peut être supprimée. Une règle qui a servi au décompte (au moins une séance
    ' du domaine dans sa période de validité) doit rester, pour que les quotas passés restent
    ' justifiables : elle se clôture ou se désactive.
    '
    ' Retour    :
    ' - String : message destiné à l'utilisateur, ou chaîne vide si la suppression est permise
    ' -------------------------------------------------------------------------------------------------
    Public Function ControlerSuppression(regle As QuotaInamiRegle) As String

        If regle Is Nothing Then Return "Aucune règle sélectionnée."

        If RegleAServi(regle) Then
            Return "Cette règle a déjà servi au décompte de séances : elle ne peut pas être supprimée, " &
                   "pour que l'historique des quotas reste exact." & Environment.NewLine & Environment.NewLine &
                   "Pour l'arrêter à une date : « Clôturer la règle ». Pour l'écarter : « Désactiver »."
        End If

        Return String.Empty

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : ControlerModification
    ' Version   : V1.0.0
    ' Date      : 14/10/2026
    '
    ' Rôle      :
    ' Dit si une règle peut être modifiée. Décision du 14/10/2026 (complète ADR-27) : une règle qui
    ' n'a encore servi à aucune séance n'a pas d'historique à protéger ; la corriger équivaut à la
    ' supprimer puis la recréer, sans laisser de période sans règle.
    '
    ' Retour    :
    ' - String : message destiné à l'utilisateur, ou chaîne vide si la modification est permise
    ' -------------------------------------------------------------------------------------------------
    Public Function ControlerModification(regle As QuotaInamiRegle) As String

        If regle Is Nothing Then Return "Aucune règle sélectionnée."

        If RegleAServi(regle) Then
            Return "Cette règle a déjà servi au décompte de séances : elle ne peut plus être modifiée, " &
                   "pour que l'historique des quotas reste exact." & Environment.NewLine & Environment.NewLine &
                   "Pour changer un plafond à partir d'une date : « Nouvelle règle » (la règle en cours s'arrête la veille). " &
                   "Pour l'arrêter : « Clôturer la règle »."
        End If

        Return String.Empty

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ModifierRegleInutilisee
    ' Version   : V1.1.0
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' Enregistre la correction d'une règle qui n'a encore servi à aucune séance.
    '
    ' Paramètres :
    ' - originale : règle telle qu'elle est en base (sa période sert au contrôle d'utilisation)
    ' - modifiee  : nouvelles valeurs (même identifiant)
    '
    ' Exceptions :
    ' - InvalidOperationException si la règle a servi ou si les nouvelles valeurs sont invalides
    '   (chevauchement…), avec le message destiné à l'utilisateur
    ' - V1.1.0 (22/10/2026, audit C.1) : reçoit le ContexteOperation ; droit d'écriture vérifié par le métier.
    ' -------------------------------------------------------------------------------------------------
    Public Sub ModifierRegleInutilisee(contexte As ContexteOperation, originale As QuotaInamiRegle, modifiee As QuotaInamiRegle)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Dim refus As String = ControlerModification(originale)
        If refus <> String.Empty Then Throw New InvalidOperationException(refus)

        Dim erreur As String = ValiderRegle(modifiee)
        If erreur <> String.Empty Then Throw New InvalidOperationException(erreur)

        UpdateQuotaInami(contexte, modifiee)

        GestionLog.EcrireLog($"Règle de quota {modifiee.IdQuotaInami} corrigée (pas encore utilisée).",
                             GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database)

    End Sub

    ' Au moins une séance du domaine dans la période de la règle : elle a servi au décompte.
    Private Function RegleAServi(regle As QuotaInamiRegle) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryQuotasInami.SelectCountSeancesSurPeriodeRegle, conn)

                cmd.Parameters.AddWithValue("@idDomaine", regle.IdDomaine)
                cmd.Parameters.AddWithValue("@dateDebut", regle.DateDebutValidite.Date)
                cmd.Parameters.AddWithValue("@dateFin",
                    If(regle.DateFinValidite.HasValue, CType(regle.DateFinValidite.Value.Date, Object), DBNull.Value))

                Return Convert.ToInt32(cmd.ExecuteScalar()) > 0

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : ReglePrecedenteArretee
    ' Version   : V1.0.0
    ' Date      : 14/10/2026
    '
    ' Rôle      :
    ' Retrouve la règle active du même domaine qui s'arrête la veille du début de la règle donnée :
    ' c'est celle que l'ajout de cette règle a clôturée automatiquement.
    '
    ' Retour    :
    ' - QuotaInamiRegle : la règle précédente, ou Nothing s'il n'y en a pas
    ' -------------------------------------------------------------------------------------------------
    Public Function ReglePrecedenteArretee(regle As QuotaInamiRegle,
                                           regles As IEnumerable(Of QuotaInamiRegle)) As QuotaInamiRegle

        If regle Is Nothing OrElse regles Is Nothing Then Return Nothing

        Dim veille As Date = regle.DateDebutValidite.Date.AddDays(-1)

        Return regles.FirstOrDefault(
            Function(r) r.IdQuotaInami <> regle.IdQuotaInami AndAlso r.IdDomaine = regle.IdDomaine AndAlso
                        r.Actif AndAlso r.DateFinValidite.HasValue AndAlso r.DateFinValidite.Value.Date = veille)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SupprimerRegleErronee
    ' Version   : V1.1.0
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' Supprime une règle saisie par erreur et, s'il y a lieu, rouvre la règle précédente qu'elle avait
    ' arrêtée : la précédente reprend la fin de la règle supprimée (ouverte si celle-ci l'était).
    '
    ' Paramètres :
    ' - regle      : règle à supprimer (ControlerSuppression doit l'avoir permise)
    ' - precedente : règle à rouvrir (ReglePrecedenteArretee), ou Nothing
    '
    ' Remarques :
    ' - Une seule transaction : la période n'est jamais laissée sans règle par un échec à mi-chemin.
    ' - Ce n'est pas une modification de règle au sens d'ADR-27 : l'arrêt automatique est annulé.
    ' - Contrôle de nouveau ControlerSuppression (défense en profondeur).
    '
    ' Exceptions :
    ' - InvalidOperationException si la règle a déjà servi ; erreurs base journalisées puis relancées
    ' - V1.1.0 (22/10/2026, audit C.1) : reçoit le ContexteOperation ; droit d'écriture vérifié par le métier.
    ' -------------------------------------------------------------------------------------------------
    Public Sub SupprimerRegleErronee(contexte As ContexteOperation, regle As QuotaInamiRegle, precedente As QuotaInamiRegle)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Dim refus As String = ControlerSuppression(regle)
        If refus <> String.Empty Then Throw New InvalidOperationException(refus)

        Try

            Using conn = DatabaseManager.OpenConnection()
                Using tx = conn.BeginTransaction()

                    Using cmd As New MySqlCommand(QueryQuotasInami.SupprimerQuotaInami, conn, tx)
                        cmd.Parameters.AddWithValue("@id", regle.IdQuotaInami)
                        cmd.ExecuteNonQuery()
                    End Using

                    If precedente IsNot Nothing Then
                        Using cmd As New MySqlCommand(QueryQuotasInami.UpdateFinValiditeQuota, conn, tx)
                            cmd.Parameters.AddWithValue("@id", precedente.IdQuotaInami)
                            cmd.Parameters.AddWithValue("@date_fin",
                                If(regle.DateFinValidite.HasValue, CType(regle.DateFinValidite.Value.Date, Object), DBNull.Value))
                            cmd.ExecuteNonQuery()
                        End Using
                    End If

                    tx.Commit()

                End Using
            End Using

            ViderCache()

            GestionLog.EcrireLog(
                $"Règle de quota {regle.IdQuotaInami} supprimée (saisie erronée)" &
                If(precedente Is Nothing, ".", $" ; règle {precedente.IdQuotaInami} rouverte."),
                GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database)

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur SupprimerRegleErronee (id={regle.IdQuotaInami}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

    ' Insère la règle et ses plafonds dans la transaction de l'appelant ; renseigne IdQuotaInami.
    Private Sub InsererRegle(regle As QuotaInamiRegle, conn As MySqlConnection, tx As MySqlTransaction)

        Using cmd As New MySqlCommand(QueryQuotasInami.InsertQuotaInami, conn, tx)
            AjouterParametresRegle(cmd, regle)
            cmd.ExecuteNonQuery()
            regle.IdQuotaInami = CULng(cmd.LastInsertedId)
        End Using

        EnregistrerPlafonds(conn, tx, regle)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AjouterParametresRegle
    ' Version   : V1.0.0
    ' Date      : 19/09/2026
    '
    ' Rôle      :
    ' Alimente les paramètres SQL communs à l'insertion et à la mise à jour.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AjouterParametresRegle(cmd As MySqlCommand, regle As QuotaInamiRegle)

        cmd.Parameters.AddWithValue("@id_domaine", regle.IdDomaine)
        cmd.Parameters.AddWithValue("@seuil_alerte_total", regle.SeuilAlerteTotal)
        cmd.Parameters.AddWithValue("@duree_periode_mois", regle.DureePeriodeMois)
        cmd.Parameters.AddWithValue("@date_debut", regle.DateDebutValidite.Date)
        cmd.Parameters.AddWithValue("@date_fin",
            If(regle.DateFinValidite.HasValue, CType(regle.DateFinValidite.Value.Date, Object), DBNull.Value))
        cmd.Parameters.AddWithValue("@actif", regle.Actif)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : EnregistrerPlafonds
    '
    ' Rôle      :
    ' Réécrit les plafonds par catégorie d'une règle, dans la transaction de la règle.
    ' -------------------------------------------------------------------------------------------------
    Private Sub EnregistrerPlafonds(conn As MySqlConnection, tx As MySqlTransaction, regle As QuotaInamiRegle)

        Using cmd As New MySqlCommand(QueryQuotasInami.SupprimerPlafondsQuota, conn, tx)
            cmd.Parameters.AddWithValue("@id_quota", regle.IdQuotaInami)
            cmd.ExecuteNonQuery()
        End Using

        For Each p In regle.Plafonds
            Using cmd As New MySqlCommand(QueryQuotasInami.InsertPlafondQuota, conn, tx)
                cmd.Parameters.AddWithValue("@id_quota", regle.IdQuotaInami)
                cmd.Parameters.AddWithValue("@id_categorie_quota", p.IdCategorieQuota)
                cmd.Parameters.AddWithValue("@plafond", p.Plafond)
                cmd.ExecuteNonQuery()
            End Using
        Next

    End Sub

#End Region

#Region "Vérifications"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : PeriodeChevauche
    ' Version   : V1.0.0
    ' Date      : 19/09/2026
    '
    ' Rôle      :
    ' Indique si la période de validité saisie recouvre celle d'une autre règle du même domaine.
    '
    ' Paramètres :
    ' - idDomaine : domaine concerné
    ' - dateDebut : début de la période saisie
    ' - dateFin   : fin de la période saisie (Nothing si toujours en vigueur)
    ' - idExclu   : règle en cours d'édition, exclue du contrôle (0 en création)
    '
    ' Remarques :
    ' - Contrôle indispensable : deux règles concurrentes rendraient le plafond applicable
    '   indéterminé, et donc la facturation non justifiable.
    ' -------------------------------------------------------------------------------------------------
    Public Function PeriodeChevauche(idDomaine As ULong,
                                     dateDebut As Date,
                                     dateFin As Date?,
                                     Optional idExclu As ULong = 0) As Boolean

        Try

            Using conn = DatabaseManager.OpenConnection()
                Return PeriodeChevauche(idDomaine, dateDebut, dateFin, idExclu, conn, Nothing)
            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur PeriodeChevauche (domaine={idDomaine}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' Même contrôle sur une connexion (et une transaction) déjà ouvertes : dans une unité de travail,
    ' il voit les écritures non encore validées (ex. la règle précédente que l'on vient d'arrêter).
    Private Function PeriodeChevauche(idDomaine As ULong, dateDebut As Date, dateFin As Date?, idExclu As ULong,
                                      conn As MySqlConnection, tx As MySqlTransaction) As Boolean

        Using cmd As New MySqlCommand(QueryQuotasInami.SelectCountChevauchementQuotaInami, conn, tx)
            cmd.Parameters.AddWithValue("@id_domaine", idDomaine)
            cmd.Parameters.AddWithValue("@date_debut", dateDebut.Date)
            cmd.Parameters.AddWithValue("@date_fin",
                If(dateFin.HasValue, CType(dateFin.Value.Date, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@id", idExclu)
            Return Convert.ToInt32(cmd.ExecuteScalar()) > 0
        End Using

    End Function

    ' Message du refus pour chevauchement (ValiderRegle et AjouterRegleEnChaine).
    Private Const MESSAGE_CHEVAUCHEMENT As String =
        "Une autre règle de ce domaine couvre déjà tout ou partie de cette période. Clôturez-la avant d'en créer une nouvelle."

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : ValiderRegle
    ' Version   : V1.1.0
    ' Date      : 21/10/2026
    '
    ' Rôle      :
    ' Contrôle la cohérence d'une règle avant enregistrement.
    '
    ' Retour    :
    ' - Chaîne vide si la règle est valide, sinon le message d'erreur à présenter à l'utilisateur
    '
    ' Remarques :
    ' - Les contrôles portent aussi bien sur la cohérence interne (plafonds, dates) que sur la
    '   cohérence d'ensemble du référentiel (chevauchement).
    ' - V1.1.0 (21/10/2026) : la cohérence interne est dans ValiderCoherence (sans base).
    ' -------------------------------------------------------------------------------------------------
    Public Function ValiderRegle(regle As QuotaInamiRegle) As String

        Dim erreur As String = ValiderCoherence(regle)
        If erreur <> String.Empty Then Return erreur

        If PeriodeChevauche(regle.IdDomaine, regle.DateDebutValidite, regle.DateFinValidite, regle.IdQuotaInami) Then
            Return MESSAGE_CHEVAUCHEMENT
        End If

        Return String.Empty

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : ValiderCoherence
    ' Version   : V1.0.0
    ' Date      : 21/10/2026
    '
    ' Rôle      :
    ' Contrôles d'une règle qui ne lisent pas la base (domaine, plafonds, durée, seuil, dates). Le
    ' chevauchement est contrôlé à part (ValiderRegle, ou dans la transaction d'AjouterRegleEnChaine).
    '
    ' Retour    :
    ' - Chaîne vide si la règle est cohérente, sinon le message à présenter à l'utilisateur
    ' -------------------------------------------------------------------------------------------------
    Public Function ValiderCoherence(regle As QuotaInamiRegle) As String

        If regle Is Nothing Then
            Return "Aucune règle à enregistrer."
        End If

        If regle.IdDomaine = 0 Then
            Return "Le domaine est obligatoire."
        End If

        If regle.Plafonds.Count = 0 Then
            Return "Le domaine n'a aucune catégorie de quota : créez au moins une catégorie avant de définir un plafond."
        End If

        If regle.Plafonds.Any(Function(p) p.Plafond < 0) Then
            Return "Les plafonds ne peuvent pas être négatifs."
        End If

        If regle.PlafondTotal = 0 Then
            Return "Au moins un plafond doit être supérieur à zéro."
        End If

        If regle.DureePeriodeMois <= 0 Then
            Return "La durée de la période doit être d'au moins un mois."
        End If

        ' Un seuil supérieur au plafond ne se déclencherait jamais : l'utilisateur perdrait
        ' l'avertissement qui précède le dépassement.
        If regle.SeuilAlerteTotal > regle.PlafondTotal Then
            Return $"Le seuil d'alerte ({regle.SeuilAlerteTotal}) ne peut pas dépasser le plafond total ({regle.PlafondTotal})."
        End If

        If regle.SeuilAlerteTotal < 0 Then
            Return "Le seuil d'alerte ne peut pas être négatif."
        End If

        If regle.DateFinValidite.HasValue AndAlso
           regle.DateFinValidite.Value.Date < regle.DateDebutValidite.Date Then
            Return "La date de fin de validité doit être postérieure à la date de début."
        End If

        Return String.Empty

    End Function

#End Region

End Module
