' -------------------------------------------------------------------------------------------------
' Module      : GestionPaiements
' Projet      : Althéa
' Version     : V1.16.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Gestion métier des paiements rattachés aux séances d'un dossier (lecture, création
' transactionnelle paiement + liaison, suppression).
'
' Responsabilités :
' - Charger la liste des paiements d'un dossier (vue allégée PaiementListeItem)
' - Charger la liste des paiements affectés à une séance
' - Créer un paiement et son affectation à une séance (dans une même transaction)
' - Supprimer un paiement (liaisons puis paiement, dans une même transaction)
' - Mapper les DataReader vers les objets métier (PaiementListeItem)
'
' Important   :
' - Aucun accès UI direct (pas de MessageBox)
' - Toute la logique SQL est déléguée à QueryPaiements
' - Les exceptions sont journalisées via GestionLog puis propagées (Throw)
' - En V1 (D-PAY-1), un paiement saisi est affecté à UNE séance ; l'utilisation d'un avoir peut
'   ensuite diminuer cette affectation (des réaffectations faites avant le 21/10/2026 ont pu la
'   répartir sur plusieurs séances).
' - V1.1.0 (17/10/2026) : EstReglementParAvoir (reconnaît un règlement par avoir même si le mode
'   « Avoir » a été désactivé depuis), utilisée par AnnulerPaiementEncaisse et par l'écran.
' - V1.2.0 (20/10/2026) : le motif d'une annulation n'est plus journalisé (texte libre, il peut parler
'   du patient) ; il reste dans le commentaire du paiement.
' - V1.3.0 (20/10/2026) : AnnulerPaiementEncaisse vérifie lui-même le droit (SuperUser, rôle courant ;
'   audit A.3).
' - V1.4.0 (21/10/2026, audit B.1) : un encaissement dont une partie a servi d'avoir ne s'annule plus
'   (GetMontantUtiliseEnAvoir) ; GetPaiementsEncaissesParSeance rend la part encore affectée à la
'   séance et GetAffectationsParPaiement la répartition d'un paiement (report sans double comptage) ;
'   une seule ligne de dû par séance (ResynchroniserDuSeance V2.0.0, qui remplace
'   SynchroniserMontantDuParSeance et ReactiverPaiementsAnnulesParSeance) ; GetResteDuSeance sans
'   paiement exclu.
' - V1.5.0 (21/10/2026, audit B.2) : la règle unique s'exécute dans une transaction, ou dans celle
'   de l'appelant (variante conn, tx) ; CreatePaiementEnAttente et AnnulerPaiementsNonEncaissesParSeance
'   deviennent privées et lèvent une exception au lieu de rendre -1.
' - V1.6.0 (21/10/2026, audit B.2, décision de Joëlle) : la réaffectation d'un encaissement est
'   remplacée par l'utilisation de l'avoir (UtiliserAvoirDeSeance ; ImputerAvoir, une transaction) ;
'   GetPaiementsEncaissesParSeance, GetResteDuSeance, GetAffectationsParPaiement et
'   RepartirPaiementSurSeances, qui ne servaient qu'à elle, sont supprimées.
' - V1.7.0 (21/10/2026, audit B.4) : la règle unique lit facturable et reste dû dans v_seances_solde.
' - V1.8.0 (21/10/2026, audit B.5, migration v2.50.0) : le mode d'un paiement est enregistré par son
'   identifiant ; le règlement par avoir est reconnu par le référentiel (est_avoir), plus par le
'   libellé ; EstReglementParAvoir(libellé) supprimée.
' - V1.9.0 (22/10/2026, audit B.5, migration v2.51.0) : annulation d'un encaissement écrite dans ses
'   colonnes (date, utilisateur par identifiant, motif) et d'un seul tenant (AnnulerPaiementEncaisse
'   V2.0.0) ; LONGUEUR_MAX_MOTIF_ANNULATION ; plus de nom d'utilisateur dans le journal de
'   l'annulation.
' - V1.10.0 (22/10/2026, audit B.5, migration v2.52.0) : chaque utilisation d'un avoir est reliée à
'   l'affectation encaissée qui l'a financée (lia_paiements_avoirs, ImputerAvoir V2.0.0) ; plus de
'   mentions dans les commentaires ; utilisateur par identifiant (UtiliserAvoirPourSeance,
'   UtiliserAvoirDeSeance) ; les listes reçoivent ces utilisations (AttacherUtilisationsAvoir).
' - V1.11.0 (22/10/2026, audit C.1) : les opérations sensibles reçoivent un ContexteOperation
'   (utilisateur + rôle courant) et le contrôlent par Autorisation (AnnulerPaiementEncaisse,
'   UtiliserAvoirPourSeance, UtiliserAvoirDeSeance).
' - V1.12.0 (22/10/2026, audit C.1) : dossier clôturé ou archivé refusé par le métier
'   (CreatePaiement, AnnulerPaiementEncaisse, utilisation d'avoir).
' - V1.13.0 (22/10/2026, audit C.2) : ValiderEncaissement (séance, montant, mode), règle sortie de la
'   fenêtre Paiement ; CreatePaiement l'applique sur le reste dû relu dans sa transaction (il
'   acceptait tout montant sur toute séance) et y réaligne le dû ; ResynchroniserDuSeanceSansErreur,
'   devenue inutilisée, supprimée.
' - V1.14.0 (22/10/2026, décision de Joëlle) : GetSeancesNonSoldees supprimée (liste des séances de
'   la fenêtre Paiement, retirée avec le bouton « Nouveau paiement »).
' - V1.15.0 (22/10/2026, relevé de Joëlle) : CompterDusDesalignes / RealignerDusDesalignes — dus en
'   attente restés faux depuis des opérations d'avant le lot B, réalignés par la règle unique depuis
'   État du système (une séance reportée, en lecture seule, ne pouvait pas être resynchronisée).
' - V1.16.0 (23/10/2026, recherche transversale R3, ADR-52) : GetPaiementsParPeriode (lignes d'une
'   période, tous patients, avec l'état de leur séance) ; MapListeItem lit le dossier et le patient ;
'   AttacherUtilisationsAvoir reçoit ses paramètres de l'appelant.
'
' Architecture :
' UI → GestionPaiements → QueryPaiements → DatabaseManager → MariaDB
'
' Dépendances :
' - DatabaseManager (connexions DB)
' - QueryPaiements (requêtes SQL)
' - DbHelper (lecture NULL-safe des colonnes)
' - GestionLog (traçabilité)
' - Paiement / PaiementListeItem (objets métier)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionPaiements

#Region "Constantes"

    ' Longueur maximale du motif d'une annulation (paiements.motif_annulation, migration v2.51.0) ;
    ' reprise par la fenêtre de saisie (DialogMotifAnnulation).
    Public Const LONGUEUR_MAX_MOTIF_ANNULATION As Integer = 500

#End Region

#Region "Lecture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetPaiementsParDossier
    ' Version    : V1.1.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Charge la liste des paiements d'un dossier pour l'onglet Paiements de la fiche dossier.
    '
    ' Paramètres :
    ' - idDossier : Identifiant du dossier dont on veut les paiements
    '
    ' Retour     :
    ' - List(Of PaiementListeItem) : Paiements triés par date décroissante
    '
    ' Remarques  :
    ' - Utilise QueryPaiements.SelectPaiementsParDossier
    ' - V1.1.0 (22/10/2026, audit B.5) : utilisations d'avoir rattachées (AttacherUtilisationsAvoir).
    ' -------------------------------------------------------------------------------------------------
    Public Function GetPaiementsParDossier(idDossier As Long) As List(Of PaiementListeItem)

        Dim result As New List(Of PaiementListeItem)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryPaiements.SelectPaiementsParDossier, conn)

                    cmd.Parameters.AddWithValue("@id_dossier", idDossier)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        While reader.Read()
                            result.Add(MapListeItem(reader))
                        End While

                    End Using

                End Using

                AttacherUtilisationsAvoir(result, QueryPaiements.SelectUtilisationsAvoirParDossier,
                                          Sub(parametres) parametres.AddWithValue("@id_dossier", idDossier), conn)

            End Using

            Return result

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur GetPaiementsParDossier.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetPaiementsParSeance
    ' Version    : V1.1.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Charge la liste des paiements affectés à une séance donnée.
    '
    ' Paramètres :
    ' - idSeance : Identifiant de la séance
    '
    ' Retour     :
    ' - List(Of PaiementListeItem) : Paiements triés par date décroissante
    '
    ' Remarques  :
    ' - Utilise QueryPaiements.SelectPaiementsParSeance
    ' - V1.1.0 (22/10/2026, audit B.5) : utilisations d'avoir rattachées (AttacherUtilisationsAvoir).
    ' -------------------------------------------------------------------------------------------------
    Public Function GetPaiementsParSeance(idSeance As Long) As List(Of PaiementListeItem)

        Dim result As New List(Of PaiementListeItem)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryPaiements.SelectPaiementsParSeance, conn)

                    cmd.Parameters.AddWithValue("@id_seance", idSeance)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        While reader.Read()
                            result.Add(MapListeItem(reader))
                        End While

                    End Using

                End Using

                AttacherUtilisationsAvoir(result, QueryPaiements.SelectUtilisationsAvoirParSeance,
                                          Sub(parametres) parametres.AddWithValue("@id_seance", idSeance), conn)

            End Using

            Return result

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur GetPaiementsParSeance.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetPaiementsParPeriode
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Lignes de paiement d'une période, tous patients (recherche transversale R3, ADR-52) : celles
    ' dont la date de mouvement (encaissement, ou séance pour un paiement attendu) tombe dans la
    ' période, la plus récente en premier.
    '
    ' Paramètres :
    ' - du, au  : bornes de la période (incluses ; du ≤ au, vérifié par l'appelant)
    ' - limite  : nombre maximal de lignes rendues (> 0)
    '
    ' Retour     :
    ' - List(Of PaiementListeItem) : au plus « limite » lignes, avec leur patient et leur dossier
    '
    ' Remarques  :
    ' - Chaque ligne apprend l'état de sa séance comme dans l'onglet Paiements du dossier : séance
    '   passée (SeanceListeItem.EstMomentPasse, heures comprises) et avoir de la séance (vue
    '   v_seances_solde) ; utilisations d'avoir rattachées (AttacherUtilisationsAvoir).
    ' - Lecture seule.
    ' -------------------------------------------------------------------------------------------------
    Public Function GetPaiementsParPeriode(du As Date, au As Date, limite As Integer) As List(Of PaiementListeItem)

        Dim result As New List(Of PaiementListeItem)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryPaiements.SelectPaiementsParPeriode, conn)

                    cmd.Parameters.AddWithValue("@du", du.Date)
                    cmd.Parameters.AddWithValue("@au", au.Date)
                    cmd.Parameters.AddWithValue("@limite", limite)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim item As PaiementListeItem = MapListeItem(reader)
                            item.SeancePassee = SeanceListeItem.EstMomentPasse(item.DateSeance,
                                                                               LireTimeNullable(reader, "heure_debut"),
                                                                               LireTimeNullable(reader, "heure_fin"))
                            item.AvoirSeance = LireDecimal(reader, "avoir_seance")
                            result.Add(item)
                        End While
                    End Using

                End Using

                AttacherUtilisationsAvoir(result, QueryPaiements.SelectUtilisationsAvoirParPeriode,
                                          Sub(parametres)
                                              parametres.AddWithValue("@du", du.Date)
                                              parametres.AddWithValue("@au", au.Date)
                                          End Sub,
                                          conn)

            End Using

            Return result

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur GetPaiementsParPeriode.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

#End Region

#Region "Contrôle de cohérence"

    ' Séances dont le dû en attente diffère du reste dû (QueryPaiements.SelectSeancesDuDesaligne).
    Private Function LireSeancesDuDesaligne() As List(Of Long)

        Dim ids As New List(Of Long)

        Using conn As MySqlConnection = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryPaiements.SelectSeancesDuDesaligne, conn)
                Using rd As MySqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        ids.Add(Convert.ToInt64(rd("id_seance")))
                    End While
                End Using
            End Using
        End Using

        Return ids

    End Function

    ' Nombre de séances dont le dû en attente est à réaligner (ligne de l'État du système).
    Public Function CompterDusDesalignes() As Integer
        Return LireSeancesDuDesaligne().Count
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : RealignerDusDesalignes
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Réaligne, par la règle unique (ResynchroniserDuSeance), chaque séance dont le dû en attente
    ' diffère du reste dû. Une transaction par séance : une séance qui échoue n'empêche pas les autres.
    '
    ' Paramètres :
    ' - contexte : Qui lance le réalignement (administrateur)
    '
    ' Retour     :
    ' - Integer : nombre de séances réalignées
    '
    ' Remarques  :
    ' - Le dû en attente est une donnée dérivée (tarif − encaissé) : le réaligner ne modifie ni la
    '   séance ni un encaissement ; c'est pourquoi un dossier clôturé ou archivé, ou une séance
    '   reportée, n'y font pas obstacle (aucun autre chemin ne pouvait les corriger).
    ' - Journal : nombre et identifiants des séances, jamais de donnée du patient.
    ' - Lève InvalidOperationException si une séance n'a pas pu être réalignée (les autres le sont).
    ' -------------------------------------------------------------------------------------------------
    Public Function RealignerDusDesalignes(contexte As ContexteOperation) As Integer

        Autorisation.Exiger(contexte, AppRole.Admin, "Le réalignement des montants en attente est réservé à l'administrateur.")

        Dim realignees As New List(Of Long)
        Dim echecs As New List(Of Long)

        For Each idSeance As Long In LireSeancesDuDesaligne()
            Try
                DatabaseManager.ExecuterEnTransaction(Sub(conn, tx) ResynchroniserDuSeance(idSeance, conn, tx))
                realignees.Add(idSeance)
            Catch ex As Exception
                echecs.Add(idSeance)
                GestionLog.EcrireLog($"Réalignement du dû impossible (séance ID={idSeance}).",
                                     GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database, ex)
            End Try
        Next

        GestionLog.EcrireLog(
            $"Dus en attente réalignés : {realignees.Count} séance(s) (ID={String.Join(", ", realignees)}) ; échecs : {echecs.Count}.",
            GestionLog.LogLevel.Succinct,
            GestionLog.LogCategory.Database
        )

        If echecs.Count > 0 Then
            Throw New InvalidOperationException(
                $"{realignees.Count} séance(s) réalignée(s), mais {echecs.Count} n'ont pas pu l'être (détail dans le journal).")
        End If

        Return realignees.Count

    End Function

#End Region

#Region "Validation"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ValiderEncaissement
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Erreurs de saisie d'un encaissement (liste vide s'il est valide) : séance choisie et encore à
    ' encaisser, montant strictement positif et au plus égal au reste dû, mode de paiement choisi.
    '
    ' Paramètres :
    ' - paiement         : Encaissement saisi (IdSeance, Montant, IdModePaiement)
    ' - resteEncaissable : Reste dû de la séance s'il peut être encaissé, sinon 0 (séance non due,
    '                      reportée ou soldée)
    '
    ' Remarques  :
    ' - Sortie de PaiementEditor.ValiderSaisie (audit C.2) : la fenêtre l'appelle avec le reste dû
    '   qu'elle affiche, CreatePaiement avec celui relu en base dans sa transaction.
    ' - Champs : NameOf des propriétés de Paiement (ErreurSaisie).
    ' -------------------------------------------------------------------------------------------------
    Public Function ValiderEncaissement(paiement As Paiement, resteEncaissable As Decimal) As List(Of ErreurSaisie)

        If paiement Is Nothing Then Throw New ArgumentNullException(NameOf(paiement))

        Dim erreurs As New List(Of ErreurSaisie)

        If paiement.IdSeance <= 0 Then
            erreurs.Add(New ErreurSaisie(NameOf(Paiement.IdSeance), "Choisissez la séance à laquelle ce paiement se rapporte."))
        ElseIf resteEncaissable <= 0D Then
            erreurs.Add(New ErreurSaisie(NameOf(Paiement.IdSeance),
                                         "Cette séance n'est pas due ou elle est déjà soldée : elle ne peut pas être encaissée."))
        End If

        If paiement.Montant <= 0D Then
            erreurs.Add(New ErreurSaisie(NameOf(Paiement.Montant), "Le montant doit être supérieur à zéro."))
        ElseIf resteEncaissable > 0D AndAlso paiement.Montant > resteEncaissable Then
            erreurs.Add(New ErreurSaisie(NameOf(Paiement.Montant),
                                         $"Le montant ne peut pas dépasser le reste dû de la séance ({resteEncaissable:N2} €)."))
        End If

        If Not paiement.IdModePaiement.HasValue Then
            erreurs.Add(New ErreurSaisie(NameOf(Paiement.IdModePaiement), "Choisissez un mode de paiement."))
        End If

        Return erreurs

    End Function

    ' Reste dû d'une séance s'il peut être encaissé (statut dû, non reportée), sinon 0 ; lu dans la
    ' transaction de l'écriture (vue v_seances_solde, une seule définition du reste dû).
    Private Function LireResteEncaissable(idSeance As Long, conn As MySqlConnection, tx As MySqlTransaction) As Decimal

        Using cmd As New MySqlCommand(QueryPaiements.SelectEncaissementSeance, conn, tx)
            cmd.Parameters.AddWithValue("@id_seance", idSeance)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If Not rd.Read() Then Return 0D
                If Convert.ToInt32(rd("est_du")) <> 1 OrElse
                   Not GestionSeances.StatutPermetEncaissement(Convert.ToString(rd("code_statut_seance"))) Then
                    Return 0D
                End If
                Return Convert.ToDecimal(rd("reste_du"))
            End Using
        End Using

    End Function

#End Region

#Region "Écriture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CreatePaiement
    ' Version    : V1.3.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Insère un nouveau paiement et l'affecte à une séance, dans une même transaction.
    '
    ' Paramètres :
    ' - paiement : Paiement à créer (DatePaiement, Montant, IdSeance doivent être renseignés)
    '
    ' Retour     :
    ' - Long : Identifiant du paiement créé, ou -1 en cas d'échec
    '
    ' Remarques  :
    ' - En V1, MontantAffecte est forcé à Montant si non renseigné (1 paiement ↔ 1 séance)
    ' - id_paiement et code_paiement sont générés par la base (seq_paiements + colonne GENERATED)
    ' - L'insertion du paiement et de sa liaison sont atomiques (transaction)
    ' - V1.1.0 (21/10/2026, audit B.5) : mode enregistré par son identifiant (id_mode_paiement).
    ' - V1.2.0 (22/10/2026, audit C.1) : dossier clôturé ou archivé refusé.
    ' - V1.3.0 (22/10/2026, audit C.2) : saisie refusée si ValiderEncaissement trouve une erreur, sur le
    '   reste dû relu dans la transaction (ErreurValidationException) : rien n'empêchait d'encaisser
    '   une séance non due ou soldée, ni un montant supérieur au reste dû. Le dû de la séance est
    '   réaligné dans la même transaction (il l'était après coup, sur une autre connexion, et un
    '   échec n'était que journalisé).
    ' -------------------------------------------------------------------------------------------------
    Public Function CreatePaiement(paiement As Paiement) As Long

        If paiement Is Nothing Then
            Throw New ArgumentNullException(NameOf(paiement))
        End If

        If paiement.IdSeance <= 0 Then
            Throw New ArgumentException("Le paiement doit être affecté à une séance (IdSeance).", NameOf(paiement))
        End If

        ' En V1, le montant affecté couvre le montant total du paiement
        Dim montantAffecte As Decimal = If(paiement.MontantAffecte > 0D, paiement.MontantAffecte, paiement.Montant)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using tx As MySqlTransaction = conn.BeginTransaction()

                    Try

                        GestionDossiers.ExigerSeanceModifiable(paiement.IdSeance, conn, tx)

                        ErreurValidationException.LeverSiErreurs(
                            ValiderEncaissement(paiement, LireResteEncaissable(paiement.IdSeance, conn, tx)))

                        Dim idPaiement As Long

                        Using cmd As New MySqlCommand(QueryPaiements.InsertPaiement, conn, tx)

                            cmd.Parameters.AddWithValue("@date_paiement", paiement.DatePaiement)
                            cmd.Parameters.AddWithValue("@montant", paiement.Montant)
                            cmd.Parameters.AddWithValue("@id_mode_paiement", If(paiement.IdModePaiement.HasValue, CObj(paiement.IdModePaiement.Value), DBNull.Value))
                            cmd.Parameters.AddWithValue("@reference_paiement", ValeurStringOuDBNull(paiement.ReferencePaiement))
                            cmd.Parameters.AddWithValue("@commentaire", ValeurStringOuDBNull(paiement.Commentaire))

                            Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                            If rowsAffected <> 1 Then

                                tx.Rollback()

                                GestionLog.EcrireLog(
                                    $"Création paiement échouée : aucune ligne insérée (id_seance={paiement.IdSeance}).",
                                    GestionLog.LogLevel.Succinct,
                                    GestionLog.LogCategory.Database
                                )

                                Return -1

                            End If

                        End Using

                        Using cmdId As New MySqlCommand("SELECT LASTVAL(seq_paiements);", conn, tx)

                            idPaiement = Convert.ToInt64(cmdId.ExecuteScalar())

                            If idPaiement <= 0 Then

                                tx.Rollback()

                                GestionLog.EcrireLog(
                                    $"Création paiement : ID invalide retourné (id_seance={paiement.IdSeance}).",
                                    GestionLog.LogLevel.Succinct,
                                    GestionLog.LogCategory.Database
                                )

                                Return -1

                            End If

                        End Using

                        Using cmdLia As New MySqlCommand(QueryPaiements.InsertLiaisonPaiementSeance, conn, tx)

                            cmdLia.Parameters.AddWithValue("@id_paiement", idPaiement)
                            cmdLia.Parameters.AddWithValue("@id_seance", paiement.IdSeance)
                            cmdLia.Parameters.AddWithValue("@montant_affecte", montantAffecte)

                            cmdLia.ExecuteNonQuery()

                        End Using

                        ' Dû de la séance aligné sur le nouvel encaissement, dans la même transaction.
                        ResynchroniserDuSeance(paiement.IdSeance, conn, tx)

                        tx.Commit()

                        paiement.IdPaiement = idPaiement
                        paiement.MontantAffecte = montantAffecte

                        GestionLog.EcrireLog(
                            $"Paiement créé avec succès (ID={idPaiement}, id_seance={paiement.IdSeance}).",
                            GestionLog.LogLevel.Rapide,
                            GestionLog.LogCategory.Database
                        )

                        Return idPaiement

                    Catch

                        tx.Rollback()
                        Throw

                    End Try

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur CreatePaiement.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CreatePaiementEnAttente
    ' Version    : V2.0.0
    ' Date       : 21/10/2026
    '
    ' Rôle       :
    ' Crée un paiement « en attente » (dû, non encaissé) et sa liaison à la séance, dans la
    ' transaction de l'appelant.
    '
    ' Paramètres :
    ' - idSeance  : Séance concernée (> 0)
    ' - montantDu : Montant dû (> 0)
    ' - conn, tx  : Connexion et transaction de l'unité de travail
    '
    ' Retour     :
    ' - Long : Identifiant du paiement créé
    '
    ' Remarques  :
    ' - Le paiement est créé avec date_paiement NULL et statut_paiement = 'EN_ATTENTE'.
    ' - V2.0.0 (21/10/2026, audit B.2) : privée, appelée par la seule règle unique
    '   (ResynchroniserDuSeance), y compris à la création d'une séance ; un échec lève une exception qui
    '   annule toute l'opération, au lieu de rendre -1 (la séance pouvait rester sans dû).
    ' -------------------------------------------------------------------------------------------------
    Private Function CreatePaiementEnAttente(idSeance As Long, montantDu As Decimal,
                                             conn As MySqlConnection, tx As MySqlTransaction) As Long

        Using cmd As New MySqlCommand(QueryPaiements.InsertPaiementEnAttente, conn, tx)
            cmd.Parameters.AddWithValue("@montant", montantDu)
            If cmd.ExecuteNonQuery() <> 1 Then
                Throw New System.Data.DataException($"Création du dû en attente : aucune ligne insérée (séance ID={idSeance}).")
            End If
        End Using

        Dim idPaiement As Long
        Using cmdId As New MySqlCommand("SELECT LASTVAL(seq_paiements);", conn, tx)
            idPaiement = Convert.ToInt64(cmdId.ExecuteScalar())
        End Using

        If idPaiement <= 0 Then
            Throw New System.Data.DataException($"Création du dû en attente : identifiant invalide (séance ID={idSeance}).")
        End If

        Using cmdLia As New MySqlCommand(QueryPaiements.InsertLiaisonPaiementSeance, conn, tx)
            cmdLia.Parameters.AddWithValue("@id_paiement", idPaiement)
            cmdLia.Parameters.AddWithValue("@id_seance", idSeance)
            cmdLia.Parameters.AddWithValue("@montant_affecte", montantDu)
            cmdLia.ExecuteNonQuery()
        End Using

        GestionLog.EcrireLog(
            $"Paiement en attente créé (ID={idPaiement}, id_seance={idSeance}, montant={montantDu}).",
            GestionLog.LogLevel.Rapide,
            GestionLog.LogCategory.Database
        )

        Return idPaiement

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AnnulerPaiementEncaisse
    ' Version    : V2.2.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Annule de façon tracée un paiement encaissé (statut PAYE → ANNULE). La ligne est conservée
    ' avec sa date, son montant et son mode ; la date, l'utilisateur et le motif de l'annulation sont
    ' enregistrés dans leurs colonnes. La séance redevient due à hauteur du montant annulé (le reste
    ' dû ne compte que les paiements PAYE).
    '
    ' Paramètres :
    ' - contexte   : Qui annule (SuperUser au moins, élévation comprise ; son identifiant est enregistré)
    ' - idPaiement : Identifiant du paiement à annuler
    ' - motif      : Motif saisi par l'utilisatrice (obligatoire, LONGUEUR_MAX_MOTIF_ANNULATION au plus)
    '
    ' Remarques  :
    ' - V1.1.0 : le refus d'un règlement par avoir passe par EstReglementParAvoir (modes inactifs
    '   compris) ; l'écran le signale avant même de demander le motif.
    ' - V1.2.0 (20/10/2026) : motif retiré du journal (identifiants seulement).
    ' - V1.3.0 (20/10/2026) : roleCourant (SuperUser au moins, élévation comprise) vérifié ici, plus
    '   seulement par l'écran (audit A.3).
    ' - V1.4.0 (21/10/2026) : refus si une partie de l'encaissement a déjà servi d'avoir (audit B.1) :
    '   la séance réglée par cet avoir resterait payée avec un montant qui n'existe plus.
    ' - V1.5.0 (21/10/2026, audit B.5) : règlement par avoir reconnu par le référentiel
    '   (QueryPaiements.SelectEstReglementParAvoir), plus par le libellé du mode.
    ' - V2.0.0 (22/10/2026, audit B.5, migration v2.51.0, ADR-49) : date, utilisateur et motif écrits
    '   dans leurs colonnes (plus de mention ajoutée au commentaire) ; l'auteur est un identifiant, son
    '   nom n'est plus recopié ni journalisé. Contrôles, annulation et dû des séances dans une seule
    '   transaction (ADR-46) : la resynchronisation du dû, faite après coup sur une autre connexion,
    '   pouvait échouer sans que l'écran le sache.
    ' - V2.1.0 (22/10/2026, audit C.1) : ContexteOperation au lieu de (roleCourant, idUtilisateur) ;
    '   droits par Autorisation.
    ' - V2.2.0 (22/10/2026, audit C.1) : refus si une séance réglée appartient à un dossier clôturé ou
    '   archivé.
    ' -------------------------------------------------------------------------------------------------
    Public Sub AnnulerPaiementEncaisse(contexte As ContexteOperation, idPaiement As Long, motif As String)

        Autorisation.Exiger(contexte, AppRole.SuperUser, "Seuls le SuperUser et l'administrateur peuvent annuler un encaissement.")
        Autorisation.ExigerUtilisateur(contexte)

        If idPaiement <= 0 Then
            Throw New ArgumentException("Identifiant de paiement invalide.", NameOf(idPaiement))
        End If

        Dim idUtilisateur As Long = contexte.IdUtilisateur

        If String.IsNullOrWhiteSpace(motif) Then
            Throw New ArgumentException("Le motif d'annulation est obligatoire.", NameOf(motif))
        End If

        Dim motifSaisi As String = motif.Trim()

        If motifSaisi.Length > LONGUEUR_MAX_MOTIF_ANNULATION Then
            Throw New InvalidOperationException(
                $"Le motif ne peut pas dépasser {LONGUEUR_MAX_MOTIF_ANNULATION} caractères : raccourcissez-le, puis recommencez.")
        End If

        Try

            DatabaseManager.ExecuterEnTransaction(
                Sub(conn, tx)

                    Using cmd As New MySqlCommand(QueryPaiements.SelectEstReglementParAvoir, conn, tx)
                        cmd.Parameters.AddWithValue("@id_paiement", idPaiement)
                        If Convert.ToInt32(If(cmd.ExecuteScalar(), 0)) = 1 Then
                            Throw New InvalidOperationException("Un règlement par avoir ne peut pas être annulé directement : l'avoir d'origine serait perdu.")
                        End If
                    End Using

                    Dim montantUtilise As Decimal = LireMontantUtiliseEnAvoir(idPaiement, conn, tx)
                    If montantUtilise > 0D Then
                        Throw New InvalidOperationException(MessageAnnulationAvoirUtilise(montantUtilise))
                    End If

                    Dim idsSeances As New List(Of Long)
                    Using cmd As New MySqlCommand(QueryPaiements.SelectIdsSeancesParPaiement, conn, tx)
                        cmd.Parameters.AddWithValue("@id_paiement", idPaiement)
                        Using reader As MySqlDataReader = cmd.ExecuteReader()
                            While reader.Read()
                                idsSeances.Add(reader.GetInt64(0))
                            End While
                        End Using
                    End Using

                    For Each idSeance In idsSeances
                        GestionDossiers.ExigerSeanceModifiable(idSeance, conn, tx)
                    Next

                    ' Écriture conditionnelle (statut PAYE) : un écran périmé est refusé, tout est annulé.
                    Using cmd As New MySqlCommand(QueryPaiements.AnnulerPaiementEncaisse, conn, tx)
                        cmd.Parameters.AddWithValue("@id_paiement", idPaiement)
                        cmd.Parameters.AddWithValue("@id_utilisateur", idUtilisateur)
                        cmd.Parameters.AddWithValue("@motif", motifSaisi)
                        If cmd.ExecuteNonQuery() = 0 Then
                            Throw New InvalidOperationException("Seul un paiement encaissé peut être annulé.")
                        End If
                    End Using

                    For Each idSeance In idsSeances
                        ResynchroniserDuSeance(idSeance, conn, tx)
                    Next

                End Sub)

            GestionLog.EcrireLog(
                $"Paiement encaissé annulé (ID={idPaiement}, utilisateur ID={idUtilisateur}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database
            )

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur AnnulerPaiementEncaisse (id={idPaiement}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : AnnulerPaiementsNonEncaissesParSeance
    ' Version    : V2.0.0
    ' Date       : 21/10/2026
    '
    ' Rôle       :
    ' Bascule en statut « Annulé » (montant ramené à 0) les paiements non encaissés d'une séance
    ' devenue non due ou soldée. La ligne de paiement est conservée pour garder la trace de
    ' l'annulation, contrairement à une suppression pure.
    '
    ' Paramètres :
    ' - idSeance : Identifiant de la séance concernée (> 0)
    ' - conn, tx : Connexion et transaction de l'unité de travail
    '
    ' Retour     :
    ' - Integer : Nombre de lignes modifiées
    '
    ' Remarques  :
    ' - Les paiements déjà encaissés (statut 'PAYE') ne sont jamais touchés.
    ' - V2.0.0 (21/10/2026, audit B.2) : privée, dans la transaction de la règle unique (seule appelante).
    ' -------------------------------------------------------------------------------------------------
    Private Function AnnulerPaiementsNonEncaissesParSeance(idSeance As Long,
                                                           conn As MySqlConnection, tx As MySqlTransaction) As Integer

        Using cmd As New MySqlCommand(QueryPaiements.AnnulerPaiementsNonEncaissesParSeance, conn, tx)
            cmd.Parameters.AddWithValue("@id_seance", idSeance)
            Return cmd.ExecuteNonQuery()
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ResynchroniserDuSeance
    ' Version    : V2.2.0
    ' Date       : 21/10/2026
    '
    ' Rôle       :
    ' Réaligne la ligne « en attente » d'une séance sur son reste dû réel (tarif - encaissé PAYE).
    ' Règle unique appelée après tout encaissement, annulation, réaffectation, changement de tarif
    ' ou de statut.
    '
    ' Règles appliquées :
    ' - Séance non due ou soldée  → la ligne en attente est annulée (trace conservée)
    ' - Reste dû > 0              → UNE ligne de dû mise au reste dû : la ligne en attente la plus
    '                               récente, à défaut le dû annulé le plus récent (réactivé), à défaut
    '                               une ligne créée ; les autres lignes en attente sont annulées
    ' - Les paiements PAYE ne sont jamais modifiés ; un encaissé supérieur au tarif reste visible
    '   comme avoir (calculé, non stocké).
    '
    ' Remarques  :
    ' - V2.0.0 (21/10/2026, audit B.1) : une seule ligne de dû par séance. Auparavant, tous les dûs
    '   annulés étaient réactivés : après plusieurs allers-retours de statut, le dû était multiplié.
    '   Les doublons déjà présents sont annulés à la première resynchronisation de leur séance.
    ' - V2.2.0 (21/10/2026, audit B.4) : séance « due » = due ET facturable, et reste dû, lus dans la
    '   vue v_seances_solde : une absence non facturable n'a plus de dû en attente.
    ' - V2.1.0 (21/10/2026, audit B.2) : lecture et écritures dans une seule transaction. Une variante
    '   (idSeance, conn, tx) s'exécute dans l'unité de travail d'un appelant (création ou modification
    '   d'une séance) : la séance et son dû sont alors enregistrés ensemble, ou pas du tout.
    ' -------------------------------------------------------------------------------------------------
    Public Sub ResynchroniserDuSeance(idSeance As Long)

        If idSeance <= 0 Then
            Return
        End If

        DatabaseManager.ExecuterEnTransaction(Sub(conn, tx) ResynchroniserDuSeance(idSeance, conn, tx))

    End Sub

    ' Règle unique dans la transaction de l'appelant (voir ci-dessus).
    Friend Sub ResynchroniserDuSeance(idSeance As Long, conn As MySqlConnection, tx As MySqlTransaction)

        If idSeance <= 0 Then
            Return
        End If

        Dim estDu As Boolean
        Dim encaisse As Decimal
        Dim reste As Decimal
        Dim nbEnAttente As Integer
        Dim idEnAttente As Long
        Dim idDuAnnule As Long

        Using cmd As New MySqlCommand(QueryPaiements.SelectSyntheseDuSeance, conn, tx)
            cmd.Parameters.AddWithValue("@id_seance", idSeance)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If Not reader.Read() Then
                    Return
                End If
                ' Facturable et reste dû : une seule définition, la vue v_seances_solde (audit B.4).
                estDu = Convert.ToBoolean(reader("est_facturable"))
                encaisse = reader.GetDecimal("montant_encaisse")
                reste = reader.GetDecimal("reste_du")
                nbEnAttente = Convert.ToInt32(reader("nb_en_attente"))
                idEnAttente = DbHelper.LireLongNullable(reader, "id_en_attente").GetValueOrDefault()
                idDuAnnule = DbHelper.LireLongNullable(reader, "id_du_annule").GetValueOrDefault()
            End Using
        End Using

        If Not estDu OrElse reste <= 0D Then
            If nbEnAttente > 0 Then
                AnnulerPaiementsNonEncaissesParSeance(idSeance, conn, tx)
                GestionLog.EcrireLog(
                    $"Dû en attente annulé (séance ID={idSeance}, due={estDu}, encaissé={encaisse:N2}).",
                    GestionLog.LogLevel.Rapide,
                    GestionLog.LogCategory.Database
                )
            End If
            Return
        End If

        If idEnAttente = 0L AndAlso idDuAnnule = 0L Then
            CreatePaiementEnAttente(idSeance, reste, conn, tx)
        Else
            Dim idGarde As Long = If(idEnAttente > 0L, idEnAttente, idDuAnnule)
            AlignerLigneDuSeance(idSeance, idGarde, reste, conn, tx)
        End If

        Dim nbDoublons As Integer = Math.Max(0, nbEnAttente - 1)

        GestionLog.EcrireLog(
            $"Dû en attente réaligné à {reste:N2} € (séance ID={idSeance})" &
            If(nbDoublons > 0, $", {nbDoublons} ligne(s) de dû en double annulée(s).", "."),
            GestionLog.LogLevel.Rapide,
            GestionLog.LogCategory.Database
        )

    End Sub

    ' Met la ligne de dû gardée au reste dû et annule les autres lignes en attente de la séance
    ' (même transaction) : la séance ne doit jamais rester avec zéro ou deux lignes de dû.
    Private Sub AlignerLigneDuSeance(idSeance As Long, idPaiementGarde As Long, reste As Decimal,
                                     conn As MySqlConnection, tx As MySqlTransaction)

        Using cmd As New MySqlCommand(QueryPaiements.UpdateDuSeance, conn, tx)
            cmd.Parameters.AddWithValue("@id_paiement", idPaiementGarde)
            cmd.Parameters.AddWithValue("@id_seance", idSeance)
            cmd.Parameters.AddWithValue("@montant", reste)
            cmd.ExecuteNonQuery()
        End Using

        Using cmd As New MySqlCommand(QueryPaiements.AnnulerAutresPaiementsEnAttenteParSeance, conn, tx)
            cmd.Parameters.AddWithValue("@id_seance", idSeance)
            cmd.Parameters.AddWithValue("@id_paiement_garde", idPaiementGarde)
            cmd.ExecuteNonQuery()
        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetAvoirsDisponiblesPourSeance
    ' Version    : V1.0.0
    ' Date       : 27/09/2026
    '
    ' Rôle       :
    ' Retourne les sources d'avoir du patient propriétaire de la séance (tous dossiers), du plus
    ' ancien au plus récent. La séance cible elle-même est exclue.
    ' -------------------------------------------------------------------------------------------------
    Public Function GetAvoirsDisponiblesPourSeance(idSeance As Long) As List(Of AvoirDisponible)

        Dim resultat As New List(Of AvoirDisponible)

        If idSeance <= 0 Then
            Return resultat
        End If

        Using conn As MySqlConnection = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryPaiements.SelectAvoirsDisponiblesPatient, conn)
                cmd.Parameters.AddWithValue("@id_seance", idSeance)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        resultat.Add(New AvoirDisponible With {
                            .IdLiaison = Convert.ToInt64(reader("id_lia_paiements_seances")),
                            .IdPaiement = reader.GetInt64("id_paiement"),
                            .CodePaiement = Convert.ToString(reader("code_paiement")),
                            .IdSeance = reader.GetInt64("id_seance"),
                            .CodeSeance = Convert.ToString(reader("code_seance")),
                            .Montant = reader.GetDecimal("montant_affecte")
                        })
                    End While
                End Using
            End Using
        End Using

        Return resultat

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : UtiliserAvoirPourSeance
    ' Version    : V2.2.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Impute l'avoir disponible du patient (tous dossiers, du plus ancien au plus récent) sur le reste
    ' dû d'une séance. Seul le montant nécessaire est consommé ; le surplus reste en avoir.
    '
    ' Paramètres :
    ' - idSeance      : Séance à régler (> 0)
    ' - montantMax    : Montant maximum à imputer (reste dû de la séance)
    ' - contexte      : Qui fait l'opération (son identifiant est enregistré avec l'utilisation)
    '
    ' Retour     :
    ' - Decimal : Montant réellement imputé (0 si aucun avoir disponible)
    '
    ' Remarques  :
    ' - V2.0.0 (21/10/2026, audit B.2) : le travail est fait par ImputerAvoir, partagé avec
    '   UtiliserAvoirDeSeance ; le dû de la séance réglée est réaligné dans la même transaction.
    ' - V2.1.0 (22/10/2026, audit B.5) : l'utilisateur est transmis par son identifiant (il l'était
    '   par son nom affiché, recopié dans les commentaires et le journal).
    ' - V2.2.0 (22/10/2026, audit C.1) : ContexteOperation.
    ' -------------------------------------------------------------------------------------------------
    Public Function UtiliserAvoirPourSeance(idSeance As Long, montantMax As Decimal, contexte As ContexteOperation) As Decimal

        If idSeance <= 0 OrElse montantMax <= 0D Then
            Return 0D
        End If

        Return ImputerAvoir(idSeance, montantMax, GetAvoirsDisponiblesPourSeance(idSeance), contexte)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : UtiliserAvoirDeSeance
    ' Version    : V1.2.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Règle une séance avec l'avoir né d'UNE séance précise — celle qui vient de devenir non due
    ' (report, annulation, maladie) — sans toucher aux autres avoirs du patient. Remplace la
    ' réaffectation d'un encaissement (décision de Joëlle, 21/10/2026) : le paiement d'origine reste
    ' sur sa séance, avec la trace de l'avoir utilisé.
    '
    ' Paramètres :
    ' - idSeanceSource : Séance dont l'avoir est utilisé (> 0)
    ' - idSeanceCible  : Séance à régler (> 0, différente de la source)
    ' - montantMax     : Montant maximum à imputer (reste dû de la séance à régler)
    ' - contexte       : Qui fait l'opération (son identifiant est enregistré avec l'utilisation)
    '
    ' Retour     :
    ' - Decimal : Montant réellement imputé (0 si la séance source n'a pas d'avoir)
    '
    ' Remarques  :
    ' - V1.1.0 (22/10/2026, audit B.5) : utilisateur transmis par son identifiant.
    ' - V1.2.0 (22/10/2026, audit C.1) : ContexteOperation.
    ' -------------------------------------------------------------------------------------------------
    Public Function UtiliserAvoirDeSeance(idSeanceSource As Long, idSeanceCible As Long, montantMax As Decimal,
                                          contexte As ContexteOperation) As Decimal

        If idSeanceSource <= 0 OrElse idSeanceCible <= 0 OrElse idSeanceSource = idSeanceCible OrElse montantMax <= 0D Then
            Return 0D
        End If

        Dim sources As List(Of AvoirDisponible) =
            GetAvoirsDisponiblesPourSeance(idSeanceCible).
            Where(Function(a) a.IdSeance = idSeanceSource).
            ToList()

        Return ImputerAvoir(idSeanceCible, montantMax, sources, contexte)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ImputerAvoir
    ' Version    : V2.1.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Impute des sources d'avoir sur une séance, dans une seule transaction :
    ' - les affectations sources sont diminuées dans l'ordre fourni ;
    ' - un paiement PAYE est créé sur la séance réglée avec le mode de paiement marqué est_avoir ;
    ' - chaque source utilisée est reliée à ce règlement (lia_paiements_avoirs : montant, utilisateur) ;
    ' - le dû en attente de la séance réglée est réaligné (règle unique).
    '
    ' Remarques  :
    ' - Reprend le corps de UtiliserAvoirPourSeance V1.0.0 (27/09/2026).
    ' - V1.1.0 (21/10/2026, audit B.5) : paiement « Avoir » enregistré avec l'identifiant du mode.
    ' - V2.0.0 (22/10/2026, audit B.5, migration v2.52.0, ADR-49) : le lien source ↔ règlement est
    '   enregistré dans lia_paiements_avoirs au lieu de mentions dans les commentaires (« [Avoir … utilisé
    '   le … par … pour S…] », « Avoir utilisé : PAI… (S…) … ») ; l'utilisateur est un identifiant, son
    '   nom n'est plus recopié ni journalisé.
    ' - V2.1.0 (22/10/2026, audit C.1) : séance à régler dans un dossier clôturé ou archivé refusée
    '   (l'avoir lui-même peut venir d'un dossier clôturé : c'est un avoir du patient).
    ' -------------------------------------------------------------------------------------------------
    Private Function ImputerAvoir(idSeance As Long, montantMax As Decimal, sources As List(Of AvoirDisponible),
                                  contexte As ContexteOperation) As Decimal

        Autorisation.ExigerUtilisateur(contexte)
        Dim idUtilisateur As Long = contexte.IdUtilisateur

        Dim modeAvoir As ModePaiement = GestionModesPaiement.GetModePaiementAvoir()

        If modeAvoir Is Nothing Then
            GestionLog.EcrireLog(
                "Utilisation d'avoir impossible : aucun mode de paiement actif marqué est_avoir.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database
            )
            Throw New InvalidOperationException("Aucun mode de paiement « avoir » actif n'est paramétré (référentiel Modes de paiement).")
        End If

        Dim idPaiementAvoir As Long = 0

        Try

            Dim total As Decimal = DatabaseManager.ExecuterEnTransaction(Of Decimal)(
                Function(conn, tx)

                    GestionDossiers.ExigerSeanceModifiable(idSeance, conn, tx)

                    Dim restant As Decimal = montantMax
                    Dim parts As New List(Of KeyValuePair(Of Long, Decimal))   ' (liaison source, part prise)

                    For Each source In sources

                        If restant <= 0D Then
                            Exit For
                        End If

                        Dim part As Decimal = Math.Min(source.Montant, restant)

                        Using cmd As New MySqlCommand(QueryPaiements.DiminuerAffectationAvoir, conn, tx)
                            cmd.Parameters.AddWithValue("@id_lia", source.IdLiaison)
                            cmd.Parameters.AddWithValue("@montant", part)

                            If cmd.ExecuteNonQuery() = 0 Then
                                Throw New InvalidOperationException("L'avoir a été modifié entre-temps ; veuillez réessayer.")
                            End If
                        End Using

                        restant -= part
                        parts.Add(New KeyValuePair(Of Long, Decimal)(source.IdLiaison, part))

                    Next

                    Dim impute As Decimal = parts.Sum(Function(x) x.Value)

                    If impute <= 0D Then
                        Return 0D
                    End If

                    Using cmd As New MySqlCommand(QueryPaiements.InsertPaiement, conn, tx)
                        cmd.Parameters.AddWithValue("@date_paiement", Date.Today)
                        cmd.Parameters.AddWithValue("@montant", impute)
                        cmd.Parameters.AddWithValue("@id_mode_paiement", modeAvoir.IdModePaiement)
                        cmd.Parameters.AddWithValue("@reference_paiement", DBNull.Value)
                        cmd.Parameters.AddWithValue("@commentaire", DBNull.Value)
                        cmd.ExecuteNonQuery()
                    End Using

                    Using cmdId As New MySqlCommand("SELECT LASTVAL(seq_paiements);", conn, tx)
                        idPaiementAvoir = Convert.ToInt64(cmdId.ExecuteScalar())
                    End Using

                    Using cmdLia As New MySqlCommand(QueryPaiements.InsertLiaisonPaiementSeance, conn, tx)
                        cmdLia.Parameters.AddWithValue("@id_paiement", idPaiementAvoir)
                        cmdLia.Parameters.AddWithValue("@id_seance", idSeance)
                        cmdLia.Parameters.AddWithValue("@montant_affecte", impute)
                        cmdLia.ExecuteNonQuery()
                    End Using

                    For Each partSource In parts
                        Using cmd As New MySqlCommand(QueryPaiements.InsertUtilisationAvoir, conn, tx)
                            cmd.Parameters.AddWithValue("@id_paiement_avoir", idPaiementAvoir)
                            cmd.Parameters.AddWithValue("@id_lia_source", partSource.Key)
                            cmd.Parameters.AddWithValue("@montant", partSource.Value)
                            cmd.Parameters.AddWithValue("@id_utilisateur", idUtilisateur)
                            cmd.ExecuteNonQuery()
                        End Using
                    Next

                    ResynchroniserDuSeance(idSeance, conn, tx)

                    Return impute

                End Function)

            If total > 0D Then
                GestionLog.EcrireLog(
                    $"Avoir de {total:N2} € imputé sur la séance ID={idSeance} (paiement ID={idPaiementAvoir}, utilisateur ID={idUtilisateur}).",
                    GestionLog.LogLevel.Rapide,
                    GestionLog.LogCategory.Database
                )
            End If

            Return total

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur ImputerAvoir (séance ID={idSeance}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetMontantUtiliseEnAvoir
    ' Version    : V1.1.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Indique quelle part d'un encaissement a déjà servi d'avoir pour régler d'autres séances
    ' (écart entre le montant encaissé et ses affectations). Un tel encaissement ne s'annule plus.
    '
    ' Paramètres :
    ' - idPaiement : Paiement concerné (> 0)
    '
    ' Retour     :
    ' - Decimal : Montant utilisé en avoir (0 si aucun, ou si le paiement n'est pas encaissé)
    '
    ' Remarques  :
    ' - V1.1.0 (22/10/2026) : lecture partagée avec l'annulation (LireMontantUtiliseEnAvoir), qui la
    '   refait dans sa transaction.
    ' -------------------------------------------------------------------------------------------------
    Public Function GetMontantUtiliseEnAvoir(idPaiement As Long) As Decimal

        If idPaiement <= 0 Then
            Return 0D
        End If

        Using conn As MySqlConnection = DatabaseManager.OpenConnection()
            Return LireMontantUtiliseEnAvoir(idPaiement, conn, Nothing)
        End Using

    End Function

    ' Même lecture sur la connexion (et la transaction) de l'appelant : le contrôle de l'annulation se
    ' fait dans sa transaction (22/10/2026, audit B.5).
    Private Function LireMontantUtiliseEnAvoir(idPaiement As Long, conn As MySqlConnection, tx As MySqlTransaction) As Decimal

        Using cmd As New MySqlCommand(QueryPaiements.SelectMontantUtiliseEnAvoir, conn, tx)
            cmd.Parameters.AddWithValue("@id_paiement", idPaiement)
            Dim valeur As Object = cmd.ExecuteScalar()
            If valeur Is Nothing OrElse valeur Is DBNull.Value Then
                Return 0D
            End If
            Return Math.Max(0D, Convert.ToDecimal(valeur))
        End Using

    End Function

    ' Message de refus commun à l'écran (contrôle anticipé) et à AnnulerPaiementEncaisse.
    Public Function MessageAnnulationAvoirUtilise(montantUtilise As Decimal) As String
        Return $"{montantUtilise:N2} € de cet encaissement ont déjà servi d'avoir pour régler une autre séance : " &
               "il ne peut plus être annulé, cette séance resterait réglée avec un montant qui n'existe plus."
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetEtatPaiementsParSeance
    ' Version    : V1.0.0
    ' Date       : 17/09/2026
    '
    ' Rôle       :
    ' Retourne le décompte des paiements d'une séance par état (encaissés, en attente, annulés).
    ' Permet à la couche métier de décider de l'action à mener sans charger les lignes complètes.
    '
    ' Paramètres :
    ' - idSeance : Identifiant de la séance concernée (> 0)
    '
    ' Retour     :
    ' - EtatPaiementsSeance : Décompte par état (jamais Nothing)
    ' -------------------------------------------------------------------------------------------------
    Public Function GetEtatPaiementsParSeance(idSeance As Long) As EtatPaiementsSeance

        If idSeance <= 0 Then
            Throw New ArgumentException("Identifiant de séance invalide.", NameOf(idSeance))
        End If

        Dim etat As New EtatPaiementsSeance()

        Using conn As MySqlConnection = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryPaiements.SelectEtatPaiementsParSeance, conn)

                cmd.Parameters.AddWithValue("@id_seance", idSeance)

                Using reader As MySqlDataReader = cmd.ExecuteReader()

                    If reader.Read() Then
                        etat.NbPayes = LireCompteur(reader, "nb_payes")
                        etat.NbEnAttente = LireCompteur(reader, "nb_en_attente")
                        etat.NbAnnules = LireCompteur(reader, "nb_annules")
                    End If

                End Using

            End Using

        End Using

        Return etat

    End Function

    ' Lit un compteur agrégé pouvant être NULL (aucune ligne correspondante)
    Private Function LireCompteur(reader As MySqlDataReader, colonne As String) As Integer

        If reader(colonne) Is DBNull.Value Then
            Return 0
        End If

        Return Convert.ToInt32(reader(colonne))

    End Function

#End Region

#Region "Helpers de conversion"

    ' Convertit une chaîne en valeur SQL : NULL si vide/absente, sinon la valeur
    Private Function ValeurStringOuDBNull(valeur As String) As Object

        If String.IsNullOrWhiteSpace(valeur) Then
            Return DBNull.Value
        End If

        Return valeur

    End Function

    ' Retourne la valeur d'une colonne décimale (NULL traité comme 0)
    Private Function LireDecimal(reader As MySqlDataReader, colonne As String) As Decimal

        If reader(colonne) Is DBNull.Value Then
            Return 0D
        End If

        Return Convert.ToDecimal(reader(colonne))

    End Function

    ' Retourne la valeur d'une colonne date, ou Nothing si NULL
    Private Function LireDateNullable(reader As MySqlDataReader, colonne As String) As Date?

        If reader(colonne) Is DBNull.Value Then
            Return Nothing
        End If

        Return CDate(reader(colonne))

    End Function

#End Region

#Region "Mapping"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AttacherUtilisationsAvoir
    ' Version    : V1.1.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Lit les utilisations d'avoir (lia_paiements_avoirs) qui touchent les lignes chargées et les
    ' rattache des deux côtés : à la ligne de l'encaissement source (même paiement, même séance) et à
    ' la ligne du règlement par avoir.
    '
    ' Paramètres :
    ' - items              : Lignes de paiement chargées
    ' - requete            : Requête de lecture (dossier, séance ou période)
    ' - ajouterParametres  : Pose les paramètres de la requête (identifiant, ou bornes de la période)
    ' - conn               : Connexion ouverte par l'appelant
    '
    ' Remarques  :
    ' - V1.1.0 (23/10/2026, recherche R3) : paramètres posés par l'appelant (la période en a deux).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AttacherUtilisationsAvoir(items As List(Of PaiementListeItem), requete As String,
                                          ajouterParametres As Action(Of MySqlParameterCollection), conn As MySqlConnection)

        If items.Count = 0 Then
            Return
        End If

        Dim utilisations As New List(Of UtilisationAvoir)

        Using cmd As New MySqlCommand(requete, conn)
            ajouterParametres(cmd.Parameters)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    utilisations.Add(New UtilisationAvoir With {
                        .IdPaiementAvoir = Convert.ToInt64(reader("id_paiement_avoir")),
                        .IdPaiementSource = Convert.ToInt64(reader("id_paiement_source")),
                        .CodePaiementSource = LireString(reader, "code_paiement_source"),
                        .IdSeanceSource = Convert.ToInt64(reader("id_seance_source")),
                        .CodeSeanceSource = LireString(reader, "code_seance_source"),
                        .CodeSeanceReglee = LireString(reader, "code_seance_reglee"),
                        .Montant = LireDecimal(reader, "montant"),
                        .DateUtilisation = CDate(reader("date_utilisation")),
                        .Auteur = LireString(reader, "auteur")
                    })
                End While
            End Using
        End Using

        If utilisations.Count = 0 Then
            Return
        End If

        For Each item In items
            item.UtilisationsAvoirSource = utilisations.
                Where(Function(u) u.IdPaiementSource = item.IdPaiement AndAlso u.IdSeanceSource = item.IdSeance).
                ToList()
            item.OriginesAvoir = utilisations.
                Where(Function(u) u.IdPaiementAvoir = item.IdPaiement).
                ToList()
        Next

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : MapListeItem
    ' Version    : V1.3.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Mappe une ligne de DataReader (QueryPaiements.SelectPaiementsBase : par dossier, par séance, par
    ' période) vers un PaiementListeItem.
    '
    ' Paramètres :
    ' - reader : DataReader positionné sur la ligne à mapper
    '
    ' Retour     :
    ' - PaiementListeItem : Élément de liste renseigné
    '
    ' Remarques  :
    ' - V1.1.0 (21/10/2026, audit B.5) : identifiant du mode, libellé et caractère « avoir » joints.
    ' - V1.2.0 (22/10/2026, audit B.5) : date, motif et auteur (nom, par jointure) de l'annulation.
    ' - V1.3.0 (23/10/2026, recherche R3) : dossier et patient de la ligne.
    ' -------------------------------------------------------------------------------------------------
    Private Function MapListeItem(reader As MySqlDataReader) As PaiementListeItem

        Dim item As New PaiementListeItem With {
            .IdPaiement = Convert.ToInt64(reader("id_paiement")),
            .CodePaiement = LireString(reader, "code_paiement"),
            .DatePaiement = LireDateNullable(reader, "date_paiement"),
            .Montant = LireDecimal(reader, "montant"),
            .IdModePaiement = LireULongNullable(reader, "id_mode_paiement"),
            .ModePaiement = LireString(reader, "mode_paiement"),
            .EstReglementParAvoir = Convert.ToInt32(reader("mode_est_avoir")) = 1,
            .ReferencePaiement = LireString(reader, "reference_paiement"),
            .IdSeance = Convert.ToInt64(reader("id_seance")),
            .CodeSeance = LireString(reader, "code_seance"),
            .DateSeance = CDate(reader("date_seance")),
            .TarifSeance = LireDecimal(reader, "tarif_seance"),
            .NatureSeance = LireString(reader, "libelle_nature_seance"),
            .ResteDuSeance = LireDecimal(reader, "reste_du_seance"),
            .SeanceEstDue = Convert.ToBoolean(reader("seance_est_due")),
            .MontantAffecte = LireDecimal(reader, "montant_affecte"),
            .StatutPaiement = LireString(reader, "statut_paiement"),
            .Commentaire = LireString(reader, "commentaire"),
            .DateAnnulation = LireDateNullable(reader, "date_annulation"),
            .MotifAnnulation = LireString(reader, "motif_annulation"),
            .AuteurAnnulation = LireString(reader, "auteur_annulation"),
            .CodeStatutSeance = LireString(reader, "code_statut_seance"),
            .CodeSeanceOrigine = LireString(reader, "code_seance_origine"),
            .CodeSeanceReport = LireString(reader, "code_seance_report"),
            .DateModification = CDate(reader("date_modification")),
            .IdDossier = Convert.ToInt64(reader("id_dossier")),
            .CodeDossier = LireString(reader, "code_dossier"),
            .IdPatient = Convert.ToInt64(reader("id_patient")),
            .CodePatient = LireString(reader, "code_patient"),
            .NomPatient = LireString(reader, "nom_patient"),
            .PrenomPatient = LireString(reader, "prenom_patient")
        }

        Return item

    End Function

#End Region

End Module
