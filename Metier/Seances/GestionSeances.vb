' -------------------------------------------------------------------------------------------------
' Module      : GestionSeances
' Projet      : Althéa
' Version     : V1.17.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Gestion métier des séances rattachées à un dossier (lecture, création, mise à jour de
' l'identité, des notes/évolution et du statut).
'
' Responsabilités :
' - Charger la liste des séances d'un dossier (vue allégée SeanceListeItem)
' - Charger une séance complète par identifiant (objet Seance avec notes/évolution)
' - Enregistrer une séance (création ou modification : identité, notes, statut et dû en une
'   transaction ; statut initial forcé « Planifiée », tarif figé)
' - Vérifier les règles d'écriture : dossier figé, séance encaissée, montant libre (audit C.1)
' - Dire ce qu'une séance permet (report, suppression, encaissement, statuts proposés) et refuser
'   un chevauchement de créneau : une seule règle pour les boutons, la fenêtre et l'écriture (C.2)
' - Appliquer date et horaires à une séance (point d'entrée dédié agenda Google, Lot 3)
' - Mapper les DataReader vers les objets métier (Seance, SeanceListeItem)
'
' Important   :
' - Aucun accès UI direct (pas de MessageBox)
' - Toute la logique SQL est déléguée à QuerySeances
' - Les exceptions sont journalisées via GestionLog puis propagées (Throw)
'
' Architecture :
' UI → GestionSeances → QuerySeances → DatabaseManager → MariaDB
'
' Dépendances :
' - DatabaseManager (connexions DB)
' - QuerySeances (requêtes SQL)
' - DbHelper (lecture NULL-safe des colonnes)
' - GestionStatutsSeance / GestionNaturesSeance (référentiels)
' - GestionLog (traçabilité)
' - Seance / SeanceListeItem (objets métier)
'
' Historique  :
' - V1.2.0 (17/10/2026) : ReaffecterPaiementVersSeance laisse le trop-perçu sur la séance d'origine
'   (avoir visible et réutilisable) au lieu de la séance de remplacement, où il sortait des totaux.
' - V1.3.0 (17/10/2026) : CalculerSoldesPatient (montant à régler et avoir de chaque dossier du
'   patient, pour le bandeau du dossier) remplace CalculerAvoirDossier.
' - V1.3.1 (17/10/2026) : LireTimeNullable déplacé dans DbHelper (partagé avec GestionPatients).
' - V1.4.0 (18/10/2026, UI-9.2) : GetPlanningJour (séances du jour et créneaux libres) ; règle
'   « une séance occupe son créneau » (OccupeSonCreneau) et plage de la journée (PlageGrille) mises
'   en commun avec ExisteChevauchement et CreneauxProposes.
' - V1.5.0 (20/10/2026) : codes des statuts de séance lus dans GestionSeances.CODE_STATUT_* (une seule définition ; audit B9).
'   (PLANIFIEE, REALISEE, REPORTEE, ABSENTE, MALADE, annulations).
' - V1.6.0 (21/10/2026, audit B.1) : ReaffecterPaiementVersSeance reporte la part encore portée par la
'   séance d'origine et conserve les autres affectations du paiement.
' - V1.7.0 (21/10/2026, audit B.2) : EnregistrerSeance (identité, notes, statut et dû en une
'   transaction, pour la fenêtre Séance) ; CreateSeance, UpdateSeance et ChangerStatutSeance écrivent
'   la séance et son dû ensemble (plus d'échec du dû avalé) ; UpdateNotesSeance, remplacée,
'   supprimée.
' - V1.8.0 (21/10/2026, audit B.2) : ReporterSeance en une transaction (origine reportée sous
'   condition, nouvelle séance, dûs) ; compensation RetablirOrigineApresEchecReport et
'   SynchroniserPaiementAvecStatut supprimées.
' - V1.9.0 (21/10/2026, audit B.2, décision de Joëlle) : ReaffecterPaiementVersSeance supprimée,
'   remplacée par l'utilisation de l'avoir (GestionPaiements.UtiliserAvoirDeSeance) ;
'   NecessiteReaffectationPaiement renommée CreeraitUnAvoir.
' - V1.10.0 (21/10/2026, audit B.3, ADR-47) : GetSeancesParDossier n'écrit plus (bascule des séances
'   planifiées passées supprimée).
' - V1.11.0 (21/10/2026, audit B.4) : solde des séances (reste dû, avoir, facturable) lu dans la vue
'   v_seances_solde (LireSolde).
' - V1.12.0 (22/10/2026, audit B.6, v2.53.0) : la case « Absence non excusée » du statut est lue
'   (SeanceListeItem.StatutEstAbsence). CODE_STATUT_ABSENTE ne sert plus qu'à Seance.EstAnnulee ;
'   EstStatutNonTenu (demande de Joëlle : pas de contrôle de créneau entre deux statuts « non tenus »).
' - V1.13.0 (22/10/2026, audit C.1) : droits et règles d'écriture vérifiés par le métier :
'   EnregistrerSeance et AppliquerDateHeureSeance reçoivent le ContexteOperation (séance encaissée,
'   montant libre : SuperUser) ; dossier clôturé ou archivé refusé partout (GestionDossiers) ;
'   SeanceEstVerrouillee ; CreateSeance et ChangerStatutSeance, plus appelées depuis B.2,
'   supprimées ; UpdateSeance privée.
' - V1.14.0 (22/10/2026, audit C.2, étape 1) : règles des actions sorties des écrans
'   (StatutPermetReport, StatutPermetSuppression, StatutPermetEncaissement, StatutEstProposable,
'   StatutsProposes) ; EnregistrerSeance refuse un statut non proposable (« Reportée » hors du
'   bouton Reporter) et un chevauchement de créneau, ReporterSeance aussi pour la nouvelle séance
'   (décision de Joëlle) ; ExisteChevauchement remplacée par ChercherChevauchement (règle « reste non
'   tenue » comprise, lecture dans la transaction de l'écriture).
' - V1.15.0 (22/10/2026) : CalculerSoldesPatient rend aussi le prévu de chaque dossier (bandeau).
' - V1.16.0 (23/10/2026, recherche transversale R2, ADR-52) : GetSeancesParPeriode (séances d'une
'   période, tous patients) ; MapPlanningItem lit aussi le patient (identifiant, code) et le code du
'   dossier.
' - V1.16.1 (24/10/2026, audit C.3) : constantes CODE_STATUT_* déplacées dans
'   CodesSysteme.StatutsSeance.
' - V1.17.0 (24/10/2026, audit C.3 étape 2, v2.57.0) : « non tenue » (créneau libéré, pas de
'   contrôle d'horaire entre statuts non tenus) et « proposé à la création » lus dans le référentiel
'   des statuts (est_non_tenue, propose_creation) au lieu des listes de codes ; une séance passée à un
'   statut non tenu n'est plus contrôlée pour le chevauchement.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionSeances

    ' Codes des statuts de séance auxquels le code applique une règle : CodesSysteme.StatutsSeance
    ' (audit C.3 ; avant, constantes CODE_STATUT_* de ce module).

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : DureeSeanceParDefaut
    ' Version    : V2.0.0
    ' Date       : 12/10/2026
    '
    ' Rôle       :
    ' Durée standard d'une séance, en minutes (paramètre SEANCE_DUREE_DEFAUT_MIN, 45 par défaut).
    ' Le cabinet ne pratique plus de battement entre deux séances : la charge de travail a imposé de
    ' supprimer le quart d'heure historique, les séances s'enchaînent donc directement
    ' (ex. 9:00-9:45, 9:45-10:30).
    '
    ' Remarques  :
    ' - Source unique de vérité : toute proposition automatique d'heure de fin doit s'y référer.
    ' - Le circuit INAMI (Realism) facture sur une base de 45 minutes.
    ' - Remplace la constante DUREE_SEANCE_MINUTES (lot 30, v2.37.0).
    ' -------------------------------------------------------------------------------------------------
    Public Function DureeSeanceParDefaut() As Integer
        Return GestionParametres.GetEntier(ClesParametres.SeanceDureeDefautMinutes, ClesParametres.DefautSeanceDureeMinutes)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CreneauxProposes
    ' Version    : V1.0.1
    ' Date       : 18/10/2026
    '
    ' Rôle       :
    ' Heures proposées dans les listes de la fiche séance : de la première à la dernière heure de la
    ' grille (SEANCE_HEURE_DEBUT_GRILLE / SEANCE_HEURE_FIN_GRILLE), par pas de SEANCE_PAS_HORAIRE_MIN.
    '
    ' Remarques  :
    ' - Cohérence avec la durée par défaut : si elle n'est pas un multiple du pas, le pas est réduit
    '   à leur plus grand diviseur commun (ex. 50 min et pas de 15 → 5), pour que l'heure de fin
    '   proposée et la séance suivante tombent toujours sur la grille ; le fait est journalisé avec
    '   le réglage conseillé.
    ' - Plage incohérente (première heure ≥ dernière) : plage par défaut, journalisée.
    ' - Une heure hors grille reste saisissable dans la fiche (listes éditables).
    ' - L'agenda (Phase 5) pourra faire évoluer ces réglages.
    ' -------------------------------------------------------------------------------------------------
    Public Function CreneauxProposes() As List(Of TimeSpan)

        Dim pas As Integer = GestionParametres.GetEntier(ClesParametres.SeancePasHoraireMinutes,
                                                         ClesParametres.DefautSeancePasHoraireMinutes)
        Dim duree As Integer = DureeSeanceParDefaut()

        If pas <= 0 Then pas = ClesParametres.DefautSeancePasHoraireMinutes

        If duree > 0 AndAlso duree Mod pas <> 0 Then
            Dim pasCoherent As Integer = PlusGrandDiviseurCommun(duree, pas)
            GestionLog.EcrireLog(
                $"Grille des séances : la durée ({duree} min) n'est pas un multiple du pas ({pas} min) ; " &
                $"pas réduit à {pasCoherent} min. Réglage conseillé : un pas qui divise la durée.",
                GestionLog.LogLevel.Rapide, GestionLog.LogCategory.Process)
            pas = pasCoherent
        End If

        Dim plage = PlageGrille()

        Dim creneaux As New List(Of TimeSpan)
        Dim creneau As TimeSpan = plage.Premiere
        Dim borneFin As TimeSpan = plage.Derniere
        Dim increment As TimeSpan = TimeSpan.FromMinutes(pas)

        While creneau <= borneFin
            creneaux.Add(creneau)
            creneau = creneau.Add(increment)
        End While

        Return creneaux

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : PlageGrille
    ' Version    : V1.0.0
    ' Date       : 18/10/2026
    '
    ' Rôle       :
    ' Première et dernière heure de la journée de travail (SEANCE_HEURE_DEBUT_GRILLE /
    ' SEANCE_HEURE_FIN_GRILLE) : listes d'heures de la fiche séance et créneaux libres du planning.
    '
    ' Remarques  :
    ' - Plage incohérente (première heure ≥ dernière) : plage par défaut, journalisée.
    ' -------------------------------------------------------------------------------------------------
    Private Function PlageGrille() As (Premiere As TimeSpan, Derniere As TimeSpan)

        Dim premiereHeure As Integer = GestionParametres.GetEntier(ClesParametres.SeanceHeureDebutGrille,
                                                                   ClesParametres.DefautSeanceHeureDebutGrille)
        Dim derniereHeure As Integer = GestionParametres.GetEntier(ClesParametres.SeanceHeureFinGrille,
                                                                   ClesParametres.DefautSeanceHeureFinGrille)

        If premiereHeure >= derniereHeure Then
            GestionLog.EcrireLog(
                $"Grille des séances : première heure ({premiereHeure}) non antérieure à la dernière ({derniereHeure}) ; " &
                "plage par défaut utilisée.",
                GestionLog.LogLevel.Rapide, GestionLog.LogCategory.Process)
            premiereHeure = ClesParametres.DefautSeanceHeureDebutGrille
            derniereHeure = ClesParametres.DefautSeanceHeureFinGrille
        End If

        Return (New TimeSpan(premiereHeure, 0, 0), New TimeSpan(derniereHeure, 0, 0))

    End Function

    Private Function PlusGrandDiviseurCommun(a As Integer, b As Integer) As Integer
        While b <> 0
            Dim reste As Integer = a Mod b
            a = b
            b = reste
        End While
        Return Math.Max(a, 1)
    End Function

#Region "Lecture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetSeancesParDossier
    ' Version    : V1.1.0
    ' Date       : 21/10/2026
    '
    ' Rôle       :
    ' Charge la liste des séances d'un dossier pour l'onglet Séances de la fiche dossier.
    '
    ' Paramètres :
    ' - idDossier : Identifiant du dossier dont on veut les séances
    '
    ' Retour     :
    ' - List(Of SeanceListeItem) : Séances triées par date décroissante
    '
    ' Remarques  :
    ' - Utilise QuerySeances.SelectSeancesParDossier
    ' - Le montant réglé est agrégé depuis lia_paiements_seances ; le reste dû est calculé au mapping
    ' - V1.1.0 (21/10/2026, audit B.3, ADR-47) : plus aucune écriture (la bascule des planifiées passées
    '   en Réalisée est supprimée : lire un onglet modifiait la base et effaçait « confirmée ou non »).
    ' -------------------------------------------------------------------------------------------------
    Public Function GetSeancesParDossier(idDossier As Long) As List(Of SeanceListeItem)

        Dim result As New List(Of SeanceListeItem)

        Try

            ' Lecture seule (21/10/2026, ADR-47) : une séance planifiée passée n'est plus basculée en
            ' Réalisée au chargement ; elle s'affiche « À confirmer » (SeanceListeItem.EstAConfirmer).
            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QuerySeances.SelectSeancesParDossier, conn)

                    cmd.Parameters.AddWithValue("@id_dossier", idDossier)

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
                "Erreur GetSeancesParDossier.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CalculerSoldesPatient
    ' Version    : V1.1.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Situation financière de chacun des dossiers d'un patient : montant à régler (séances dues et
    ' passées), prévu (séances dues à venir) et avoir, avec les règles des grilles
    ' (SeanceListeItem.MontantExigible, MontantPrevisionnel, AvoirSeance).
    '
    ' Paramètres :
    ' - idPatient : Patient dont on veut les dossiers (> 0)
    '
    ' Retour     :
    ' - List(Of SoldeDossier) : une ligne par dossier (liste vide si le patient n'a pas de dossier).
    '
    ' Remarques  :
    ' - Calculé, jamais stocké. Les séances de chaque dossier sont relues : un patient n'a que
    '   quelques dossiers, et la règle reste celle des grilles, sans seconde définition en SQL.
    ' - Remplace CalculerAvoirDossier et le calcul du bandeau par codes de statut écrits en dur
    '   (REALISEE / ABSENTE) : un seul calcul pour les grilles et le bandeau.
    ' - V1.1.0 (22/10/2026) : MontantPrevu (ligne « Prévu » du bandeau).
    ' -------------------------------------------------------------------------------------------------
    Public Function CalculerSoldesPatient(idPatient As Long) As List(Of SoldeDossier)

        Dim soldes As New List(Of SoldeDossier)

        If idPatient <= 0 Then
            Return soldes
        End If

        Try
            For Each dossier In GestionDossiers.GetDossiersParPatient(idPatient)
                Dim seances As List(Of SeanceListeItem) = GetSeancesParDossier(dossier.IdDossier)
                soldes.Add(New SoldeDossier With {
                    .IdDossier = dossier.IdDossier,
                    .CodeDossier = dossier.CodeDossier,
                    .MontantExigible = Math.Round(seances.Sum(Function(s) s.MontantExigible), 2),
                    .MontantPrevu = Math.Round(seances.Sum(Function(s) s.MontantPrevisionnel), 2),
                    .Avoir = Math.Round(seances.Sum(Function(s) s.AvoirSeance), 2)
                })
            Next

            Return soldes

        Catch ex As Exception
            GestionLog.EcrireLog(
                $"Erreur CalculerSoldesPatient (patient ID={idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )
            Throw
        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetSeancesParDate
    ' Version    : V1.1.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Charge la liste des séances planifiées à une date donnée, tous dossiers/patients confondus.
    ' Alimente la grille de consultation du planning du jour.
    '
    ' Paramètres :
    ' - dateSeance : Date des séances à charger (l'heure est ignorée)
    '
    ' Retour     :
    ' - List(Of SeancePlanningItem) : Séances triées par heure de début croissante
    '
    ' Remarques  :
    ' - Utilise QuerySeances.SelectSeancesParDate
    ' - V1.1.0 (22/10/2026, audit C.2) : lecture dans LireSeancesParDate, partagée avec le contrôle de
    '   chevauchement, qui lit dans la transaction de l'écriture.
    ' -------------------------------------------------------------------------------------------------
    Public Function GetSeancesParDate(dateSeance As Date) As List(Of SeancePlanningItem)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()
                Return LireSeancesParDate(dateSeance, conn, Nothing)
            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur GetSeancesParDate.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' Séances d'une date sur la connexion (et la transaction, si fournie) de l'appelant.
    Private Function LireSeancesParDate(dateSeance As Date, conn As MySqlConnection, tx As MySqlTransaction) As List(Of SeancePlanningItem)

        Dim result As New List(Of SeancePlanningItem)

        Using cmd As New MySqlCommand(QuerySeances.SelectSeancesParDate, conn, tx)

            cmd.Parameters.AddWithValue("@date_seance", dateSeance.Date)

            Using reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    result.Add(MapPlanningItem(reader))
                End While
            End Using

        End Using

        Return result

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetSeancesParPeriode
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Séances d'une période (bornes incluses), tous dossiers et patients confondus, dans l'ordre du
    ' calendrier : recherche transversale R2 (ADR-52).
    '
    ' Paramètres :
    ' - du, au  : bornes de la période (l'heure est ignorée ; du ≤ au, vérifié par l'appelant)
    ' - limite  : nombre maximal de séances rendues (> 0)
    '
    ' Retour     :
    ' - List(Of SeancePlanningItem) : au plus « limite » séances, par date puis heure
    '
    ' Remarques  :
    ' - Lecture seule ; même mapping que le planning du jour (MapPlanningItem).
    ' -------------------------------------------------------------------------------------------------
    Public Function GetSeancesParPeriode(du As Date, au As Date, limite As Integer) As List(Of SeancePlanningItem)

        Dim result As New List(Of SeancePlanningItem)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()
                Using cmd As New MySqlCommand(QuerySeances.SelectSeancesParPeriode, conn)

                    cmd.Parameters.AddWithValue("@du", du.Date)
                    cmd.Parameters.AddWithValue("@au", au.Date)
                    cmd.Parameters.AddWithValue("@limite", limite)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            result.Add(MapPlanningItem(reader))
                        End While
                    End Using

                End Using
            End Using

            Return result

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur GetSeancesParPeriode.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetPlanningJour
    ' Version    : V1.0.0
    ' Date       : 18/10/2026
    '
    ' Rôle       :
    ' Lignes du planning d'une journée (fenêtre « Planning du jour ») : les séances, tous dossiers
    ' confondus, par heure, et entre elles les créneaux libres où une séance tient encore.
    '
    ' Paramètres :
    ' - dateSeance : jour affiché
    '
    ' Retour     :
    ' - List(Of LignePlanningJour) : séances et créneaux libres, dans l'ordre des heures ; séances
    '   sans horaire en tête
    '
    ' Remarques  :
    ' - Créneaux libres dans la plage de la journée (PlageGrille), calculés sur les séances qui
    '   occupent leur créneau (OccupeSonCreneau, même règle que le contrôle de chevauchement) : une
    '   séance annulée ou reportée reste affichée mais libère sa place.
    ' - Un trou plus court que la durée par défaut d'une séance n'est pas annoncé.
    ' - Aujourd'hui : rien n'est annoncé avant l'heure qu'il est (arrondie au pas des heures) ; jour
    '   passé : aucun créneau libre (le planning sert à planifier).
    ' -------------------------------------------------------------------------------------------------
    Public Function GetPlanningJour(dateSeance As Date) As List(Of LignePlanningJour)

        Dim seances As List(Of SeancePlanningItem) = GetSeancesParDate(dateSeance)
        Dim lignes As List(Of LignePlanningJour) = seances.Select(Function(s) New LignePlanningJour(s)).ToList()

        If dateSeance.Date >= Date.Today Then

            Dim plage = PlageGrille()
            Dim dureeMin As TimeSpan = TimeSpan.FromMinutes(Math.Max(1, DureeSeanceParDefaut()))
            Dim curseur As TimeSpan = plage.Premiere

            If dateSeance.Date = Date.Today Then
                Dim pas As Integer = Math.Max(1, GestionParametres.GetEntier(ClesParametres.SeancePasHoraireMinutes,
                                                                              ClesParametres.DefautSeancePasHoraireMinutes))
                Dim minutes As Integer = CInt(Math.Ceiling(Date.Now.TimeOfDay.TotalMinutes / pas)) * pas
                If TimeSpan.FromMinutes(minutes) > curseur Then curseur = TimeSpan.FromMinutes(minutes)
            End If

            For Each occupee In seances.Where(Function(s) OccupeSonCreneau(s)).OrderBy(Function(s) s.HeureDebut.Value)
                Dim finLibre As TimeSpan = If(occupee.HeureDebut.Value < plage.Derniere, occupee.HeureDebut.Value, plage.Derniere)
                If finLibre - curseur >= dureeMin Then lignes.Add(LignePlanningJour.CreneauLibre(curseur, finLibre))
                If occupee.HeureFin.Value > curseur Then curseur = occupee.HeureFin.Value
            Next

            If plage.Derniere - curseur >= dureeMin Then lignes.Add(LignePlanningJour.CreneauLibre(curseur, plage.Derniere))

        End If

        Return lignes.OrderBy(Function(l) l.CleTri).ThenBy(Function(l) l.EstLibre).ToList()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : EstStatutNonTenu
    ' Version    : V2.0.0
    ' Date       : 24/10/2026
    '
    ' Rôle       :
    ' Indique un statut de séance « non tenue » : case « Non tenue, libère le créneau » ou case
    ' « Absence non excusée » du référentiel.
    '
    ' Remarques  :
    ' - Demande de Joëlle (22/10/2026) : passer d'un statut non tenu à un autre (annulée → absence,
    '   maladie → absence…) ne doit pas déclencher le contrôle de chevauchement : la séance n'a pas eu
    '   lieu et ne reprend aucun créneau, que d'autres ont pu occuper entre-temps.
    ' - V2.0.0 (24/10/2026, audit C.3 étape 2, v2.57.0) : lu dans le référentiel (est_non_tenue) au lieu
    '   des codes Annulée / Maladie / Reportée ; un statut ajouté se règle par sa case.
    ' -------------------------------------------------------------------------------------------------
    Public Function EstStatutNonTenu(statut As StatutSeance) As Boolean
        Return statut IsNot Nothing AndAlso (statut.EstNonTenue OrElse statut.EstAbsence)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : OccupeSonCreneau
    ' Version    : V2.0.0
    ' Date       : 24/10/2026
    '
    ' Rôle       :
    ' Indique si une séance mobilise son créneau horaire : elle a un début et une fin, et son statut
    ' n'est pas coché « Non tenue, libère le créneau ».
    '
    ' Remarques  :
    ' - Une seule règle pour le contrôle de chevauchement et les créneaux libres du planning.
    ' - V2.0.0 (24/10/2026, audit C.3 étape 2, v2.57.0) : case du référentiel (SeanceListeItem.
    '   StatutNonTenu) au lieu de la liste Annulée / Reportée ; Maladie libère désormais son créneau
    '   (valeur de départ de la migration, réglable).
    ' -------------------------------------------------------------------------------------------------
    Public Function OccupeSonCreneau(seance As SeanceListeItem) As Boolean

        If seance Is Nothing OrElse Not seance.HeureDebut.HasValue OrElse Not seance.HeureFin.HasValue Then Return False

        Return Not seance.StatutNonTenu

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ChercherChevauchement
    ' Version    : V2.1.0
    ' Date       : 24/10/2026
    '
    ' Rôle       :
    ' Cherche une autre séance dont le créneau chevauche celui de la séance saisie, à la même date (un
    ' seul thérapeute : le planning est global, tous domaines et dossiers confondus). Les séances dont
    ' le statut est coché « Non tenue, libère le créneau » ne l'occupent pas (OccupeSonCreneau). La fenêtre Séance l'appelle pour
    ' son message ; EnregistrerSeance et ReporterSeance refusent l'écriture avec la même règle.
    '
    ' Paramètres :
    ' - seance          : Séance saisie (IdSeance = 0 en création ; date et horaires)
    ' - codeStatutCible : Statut choisi (vide : inchangé)
    '
    ' Retour     :
    ' - SeancePlanningItem : première séance en conflit, ou Nothing (pas de conflit, horaires
    '   incomplets, ou séance qui reste « non tenue »)
    '
    ' Remarques  :
    ' - Deux plages [DebutA, FinA) et [DebutB, FinB) se chevauchent si DebutA < FinB ET DebutB < FinA.
    ' - Passer d'un statut « non tenu » à un autre ne contrôle pas le créneau (demande de Joëlle,
    '   22/10/2026) : la séance n'a pas eu lieu et ne reprend aucune place.
    ' - V2.0.0 (22/10/2026, audit C.2) : remplace ExisteChevauchement (paramètres séparés, sortie
    '   ByRef) ; la règle « reste non tenue », qui était dans la fenêtre, est ici.
    ' - V2.1.0 (24/10/2026, audit C.3 étape 2, v2.57.0) : une séance dont le statut retenu est coché
    '   « Non tenue, libère le créneau » n'est pas contrôlée (elle n'occupe pas son horaire) ; les séances
    '   qui libèrent leur créneau sont lues dans le référentiel (OccupeSonCreneau).
    ' -------------------------------------------------------------------------------------------------
    Public Function ChercherChevauchement(seance As Seance, codeStatutCible As String) As SeancePlanningItem

        If seance Is Nothing Then Throw New ArgumentNullException(NameOf(seance))

        Try

            Dim codeOrigine As String = If(seance.IdSeance > 0, GetSeance(seance.IdSeance)?.CodeStatutSeance, Nothing)

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()
                Return ChercherChevauchement(seance, codeStatutCible, codeOrigine, conn, Nothing)
            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur ChercherChevauchement (séance ID={seance.IdSeance}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' Même règle, sur la connexion et la transaction de l'écriture ; codeOrigine : statut enregistré
    ' de la séance (Nothing en création).
    Private Function ChercherChevauchement(seance As Seance, codeStatutCible As String, codeOrigine As String,
                                           conn As MySqlConnection, tx As MySqlTransaction) As SeancePlanningItem

        If Not seance.HeureDebut.HasValue OrElse Not seance.HeureFin.HasValue Then Return Nothing

        If LibereSonCreneau(codeOrigine, codeStatutCible) Then Return Nothing
        If seance.IdSeance > 0 AndAlso ResteNonTenue(codeOrigine, codeStatutCible) Then Return Nothing

        Dim debut As TimeSpan = seance.HeureDebut.Value
        Dim fin As TimeSpan = seance.HeureFin.Value

        Return LireSeancesParDate(seance.DateSeance, conn, tx).
            FirstOrDefault(Function(c) c.IdSeance <> seance.IdSeance AndAlso OccupeSonCreneau(c) AndAlso
                                       debut < c.HeureFin.Value AndAlso c.HeureDebut.Value < fin)

    End Function

    ' Statut d'origine et statut choisi (vide : inchangé) tous deux « non tenus » ; le statut d'origine
    ' est cherché dans le référentiel, actifs et inactifs compris.
    Private Function ResteNonTenue(codeOrigine As String, codeCible As String) As Boolean

        If String.IsNullOrWhiteSpace(codeOrigine) Then Return False

        Dim statuts As List(Of StatutSeance) = GestionStatutsSeance.GetStatutsSeance(True)
        Dim codeRetenu As String = If(String.IsNullOrWhiteSpace(codeCible), codeOrigine, codeCible)

        Return EstStatutNonTenu(StatutParCode(statuts, codeOrigine)) AndAlso
               EstStatutNonTenu(StatutParCode(statuts, codeRetenu))

    End Function

    ' Statut retenu (choisi, sinon inchangé) coché « Non tenue, libère le créneau » : la séance
    ' n'occupe pas son horaire, rien à contrôler (24/10/2026, v2.57.0).
    Private Function LibereSonCreneau(codeOrigine As String, codeCible As String) As Boolean

        Dim codeRetenu As String = If(String.IsNullOrWhiteSpace(codeCible), codeOrigine, codeCible)
        If String.IsNullOrWhiteSpace(codeRetenu) Then Return False

        Dim statut As StatutSeance = StatutParCode(GestionStatutsSeance.GetStatutsSeance(True), codeRetenu)
        Return statut IsNot Nothing AndAlso statut.EstNonTenue

    End Function

    Private Function StatutParCode(statuts As List(Of StatutSeance), code As String) As StatutSeance
        Return statuts.FirstOrDefault(Function(s) String.Equals(s.CodeStatutSeance, code, StringComparison.OrdinalIgnoreCase))
    End Function

    ' Refus d'écriture d'un créneau occupé : code et horaires de l'autre séance, jamais le nom du
    ' patient (le message passe par le journal).
    Private Function MessageChevauchement(conflit As SeancePlanningItem) As String
        Return $"Ce créneau chevauche la séance {conflit.CodeSeance} ({conflit.HorairesAffichage}) : choisissez un autre horaire."
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetSeance
    ' Version    : V1.0.0
    ' Date       : 14/09/2026
    '
    ' Rôle       :
    ' Charge une séance complète depuis son identifiant.
    '
    ' Paramètres :
    ' - idSeance : Identifiant de la séance à charger
    '
    ' Retour     :
    ' - Seance : L'objet séance, ou Nothing si introuvable
    '
    ' Remarques  :
    ' - Utilise QuerySeances.SelectSeanceById
    ' -------------------------------------------------------------------------------------------------
    Public Function GetSeance(idSeance As Long) As Seance

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QuerySeances.SelectSeanceById, conn)

                    cmd.Parameters.AddWithValue("@id_seance", idSeance)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        If reader.Read() Then
                            Return MapSeance(reader)
                        End If

                    End Using

                End Using

            End Using

            Return Nothing

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur GetSeance.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

#End Region

#Region "Règles des actions"

    ' -------------------------------------------------------------------------------------------------
    ' Règles des actions sur une séance (audit C.2, étape 1)
    '
    ' Une seule définition, lue par les boutons de l'onglet Séances (SeanceListeItem.EstReportable…),
    ' la fenêtre Séance (StatutsProposes) et les écritures, qui refusent avec la même règle. Les
    ' statuts concernés sont des lignes système, reconnues par leur code (CodesSysteme.StatutsSeance) ; un
    ' drapeau du référentiel pourra les remplacer (lot C.3).
    ' -------------------------------------------------------------------------------------------------

    ' Seule une séance encore planifiée se reporte : une séance réalisée, une absence, une annulation
    ' ou une maladie ne sont pas des rendez-vous à déplacer.
    Public Function StatutPermetReport(codeStatut As String) As Boolean
        Return String.Equals(codeStatut, CodesSysteme.StatutsSeance.Planifiee, StringComparison.OrdinalIgnoreCase)
    End Function

    ' Une séance reportée reste dans le dossier : elle garde la trace du report. (Les autres blocages
    ' — encaissement, origine d'un report — sont lus en base par DeleteSeance.)
    Public Function StatutPermetSuppression(codeStatut As String) As Boolean
        Return Not String.Equals(codeStatut, CodesSysteme.StatutsSeance.Reportee, StringComparison.OrdinalIgnoreCase)
    End Function

    ' Encaisser une séance reportée créerait un faux avoir ; il faut en outre qu'elle soit due et pas
    ' soldée (SeanceListeItem.EstEncaissable, GestionPaiements.ValiderEncaissement).
    Public Function StatutPermetEncaissement(codeStatut As String) As Boolean
        Return Not String.Equals(codeStatut, CodesSysteme.StatutsSeance.Reportee, StringComparison.OrdinalIgnoreCase)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : StatutEstProposable
    ' Version    : V2.0.0
    ' Date       : 24/10/2026
    '
    ' Rôle       :
    ' Dit si un statut peut être choisi dans la fenêtre Séance :
    ' - création : statut coché « Proposé à la création » dans le référentiel (au départ Planifiée et
    '   Réalisée : jamais d'annulation ni de report d'une séance que l'on crée) ;
    ' - modification : tout sauf Reportée, réservé au bouton « Reporter » (qui crée la séance de
    '   remplacement) ; le statut courant reste proposé.
    '
    ' Paramètres :
    ' - creation    : True pour une nouvelle séance
    ' - statut      : Statut à juger (Nothing : refusé)
    ' - codeCourant : Statut enregistré de la séance (ignoré en création)
    '
    ' Remarques  :
    ' - Sortie de SeanceEditor.ChargerStatutsProposes (audit C.2) : EnregistrerSeance acceptait
    '   « Reportée » en modification, donc une séance reportée sans séance de remplacement.
    ' - V2.0.0 (24/10/2026, audit C.3 étape 2, v2.57.0) : reçoit le statut ; la création lit sa case
    '   « Proposé à la création » au lieu des codes Planifiée / Réalisée. Reportée reste réservée au
    '   bouton « Reporter » (circuit du report).
    ' -------------------------------------------------------------------------------------------------
    Public Function StatutEstProposable(creation As Boolean, statut As StatutSeance, codeCourant As String) As Boolean

        If statut Is Nothing Then Return False

        If creation Then Return statut.ProposeCreation

        Return Not String.Equals(statut.CodeStatutSeance, CodesSysteme.StatutsSeance.Reportee, StringComparison.OrdinalIgnoreCase) OrElse
               String.Equals(statut.CodeStatutSeance, codeCourant, StringComparison.OrdinalIgnoreCase)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : StatutsProposes
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Statuts actifs proposés dans la liste de la fenêtre Séance (StatutEstProposable).
    '
    ' Remarques  :
    ' - L'ancien repli de la fenêtre (liste complète si le filtre ne gardait rien) est retiré :
    '   Planifiée et Réalisée sont des lignes système, protégées depuis le lot C.1 ; une liste
    '   complète aurait proposé des statuts que l'écriture refuse.
    ' -------------------------------------------------------------------------------------------------
    Public Function StatutsProposes(creation As Boolean, codeCourant As String) As List(Of StatutSeance)

        Dim actifs As List(Of StatutSeance) = GestionStatutsSeance.GetStatutsSeanceActifs()

        If actifs Is Nothing Then Return New List(Of StatutSeance)

        Return actifs.Where(Function(s) StatutEstProposable(creation, s, codeCourant)).ToList()

    End Function

#End Region

#Region "Écriture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : EnregistrerSeance
    ' Version    : V2.2.0
    ' Date       : 24/10/2026
    '
    ' Rôle       :
    ' Enregistre en une fois ce que la fenêtre Séance a saisi : identité (type, date, horaires, durée,
    ' montant), notes et évolution, statut, puis le dû (règle unique). Tout est écrit dans une seule
    ' transaction : en cas d'échec, rien n'est enregistré.
    '
    ' Paramètres :
    ' - contexte   : Qui enregistre (rôle courant : montant libre, séance encaissée)
    ' - seance     : Séance saisie (IdSeance = 0 : création ; > 0 : modification), notes comprises
    ' - codeStatut : Statut choisi (vide : inchangé en modification, Planifiée en création)
    '
    ' Retour     :
    ' - Long : Identifiant de la séance (créée ou modifiée)
    '
    ' Remarques  :
    ' - Création : statut initial Planifiée (forcé, quel que soit le statut fourni), puis le statut choisi s'il
    '   diffère.
    ' - Une séance reportée est en lecture seule : son statut ne change plus.
    ' - La réaffectation d'un encaissement devenu orphelin n'en fait pas partie : c'est un choix de
    '   l'utilisatrice, proposé par l'écran une fois la séance enregistrée.
    ' - Remplace l'enchaînement CreateSeance / UpdateSeance → UpdateNotesSeance → ChangerStatutSeance
    '   orchestré par l'écran, chaque étape sur sa propre connexion (audit B.2).
    ' - V2.0.0 (22/10/2026, audit C.1) : les règles sont vérifiées ici et plus seulement par la
    '   fenêtre : dossier clôturé ou archivé refusé ; séance réalisée et encaissée réservée au
    '   SuperUser ; montant différent du tarif réservé au SuperUser (un montant libre déjà enregistré
    '   et laissé tel quel est accepté : décision de Joëlle).
    ' - V2.1.0 (22/10/2026, audit C.2) : statut non proposable refusé (StatutEstProposable : création
    '   planifiée ou réalisée, « Reportée » réservé au bouton Reporter) ; créneau qui chevauche une
    '   autre séance refusé, relu dans la transaction (ChercherChevauchement, décision de Joëlle).
    ' - V2.2.0 (24/10/2026, audit C.3 étape 2, v2.57.0) : statut proposé à la création lu dans le
    '   référentiel (StatutEstProposable).
    ' -------------------------------------------------------------------------------------------------
    Public Function EnregistrerSeance(contexte As ContexteOperation, seance As Seance, codeStatut As String) As Long

        If seance Is Nothing Then
            Throw New ArgumentNullException(NameOf(seance))
        End If

        Dim creation As Boolean = seance.IdSeance <= 0

        Try

            ' Référentiels et contrôles lus avant la transaction (lecture seule).
            Dim idStatutPlanifiee As Long = ResoudreIdStatutParCode(CodesSysteme.StatutsSeance.Planifiee)
            Dim idStatutCible As Long? = Nothing

            If Not String.IsNullOrWhiteSpace(codeStatut) Then
                idStatutCible = ResoudreIdStatutParCode(codeStatut)
            End If

            Dim courante As Seance = Nothing

            If Not creation Then

                courante = GetSeance(seance.IdSeance)

                If courante Is Nothing Then
                    Throw New InvalidOperationException("Cette séance n'existe plus. Fermez la fenêtre et rouvrez le dossier.")
                End If

                If idStatutCible.HasValue AndAlso
                   String.Equals(courante.CodeStatutSeance, CodesSysteme.StatutsSeance.Reportee, StringComparison.OrdinalIgnoreCase) AndAlso
                   Not String.Equals(codeStatut, CodesSysteme.StatutsSeance.Reportee, StringComparison.OrdinalIgnoreCase) Then
                    Throw New InvalidOperationException("Une séance reportée est en lecture seule : son statut ne peut plus changer.")
                End If

                ExigerDeverrouillage(contexte, courante)

            End If

            If idStatutCible.HasValue AndAlso
               Not StatutEstProposable(creation, StatutParCode(GestionStatutsSeance.GetStatutsSeance(True), codeStatut),
                                       courante?.CodeStatutSeance) Then
                Throw New InvalidOperationException(
                    If(creation,
                       "Ce statut n'est pas proposé à la création d'une séance (référentiel des statuts de séance).",
                       "Le report d'une séance se fait par le bouton « Reporter » : il crée la séance de remplacement."))
            End If

            Dim codeOrigine As String = courante?.CodeStatutSeance

            ' Statut retenu pour le tarif attendu : celui choisi, sinon l'actuel (Planifiée en création).
            Dim idStatutTarif As Long =
                If(idStatutCible, If(creation, idStatutPlanifiee, courante.IdStatutSeance))

            ExigerMontantAutorise(contexte, seance, courante, idStatutTarif)

            Dim idSeance As Long = DatabaseManager.ExecuterEnTransaction(Of Long)(
                Function(conn, tx)

                    Dim id As Long

                    If creation Then
                        GestionDossiers.ExigerDossierModifiable(seance.IdDossier, GestionDossiers.ElementDossier.SeancesEtPaiements, conn, tx)
                    Else
                        GestionDossiers.ExigerSeanceModifiable(seance.IdSeance, conn, tx)
                    End If

                    ' Créneau relu dans la transaction, juste avant l'écriture (même règle que la fenêtre).
                    Dim conflit As SeancePlanningItem = ChercherChevauchement(seance, codeStatut, codeOrigine, conn, tx)
                    If conflit IsNot Nothing Then
                        Throw New InvalidOperationException(MessageChevauchement(conflit))
                    End If

                    If creation Then
                        seance.IdStatutSeance = idStatutPlanifiee
                        id = InsererSeance(seance, conn, tx)
                    Else
                        id = seance.IdSeance
                        EcrireIdentiteSeance(seance, conn, tx)
                    End If

                    EcrireNotesSeance(id, seance, conn, tx)

                    If idStatutCible.HasValue AndAlso
                       (Not creation OrElse idStatutCible.Value <> idStatutPlanifiee) Then
                        EcrireStatutSeance(id, idStatutCible.Value, conn, tx)
                    End If

                    ' Dû aligné sur le montant et le statut enregistrés.
                    GestionPaiements.ResynchroniserDuSeance(id, conn, tx)

                    Return id

                End Function)

            seance.IdSeance = idSeance

            GestionLog.EcrireLog(
                $"Séance {If(creation, "créée", "enregistrée")} (ID={idSeance}, id_dossier={seance.IdDossier}" &
                If(String.IsNullOrWhiteSpace(codeStatut), ").", $", statut {codeStatut})."),
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database
            )

            Return idSeance

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur EnregistrerSeance (id={seance.IdSeance}, id_dossier={seance.IdDossier}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateSeance
    ' Version    : V3.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Met à jour l'identité d'une séance existante (type, date, horaires, durée, tarif) et son dû.
    '
    ' Paramètres :
    ' - seance : Séance avec IdSeance > 0 et les propriétés d'identité remplies
    '
    ' Remarques  :
    ' - Ne modifie ni id_seance, ni code_seance, ni id_dossier
    ' - Le statut n'est pas modifié ici (voir EnregistrerSeance)
    ' - V2.0.0 (21/10/2026, audit B.2) : identité et dû dans une seule transaction ; un échec du dû
    '   n'est plus avalé (il annule la mise à jour).
    ' - Utilisée par AppliquerDateHeureSeance ; la fenêtre Séance passe par EnregistrerSeance.
    ' - V3.0.0 (22/10/2026, audit C.1) : privée (seul appelant : AppliquerDateHeureSeance, qui vérifie
    '   les droits) ; dossier clôturé ou archivé refusé.
    ' -------------------------------------------------------------------------------------------------
    Private Sub UpdateSeance(seance As Seance)

        If seance Is Nothing Then
            Throw New ArgumentNullException(NameOf(seance))
        End If

        Try

            DatabaseManager.ExecuterEnTransaction(
                Sub(conn, tx)
                    GestionDossiers.ExigerSeanceModifiable(seance.IdSeance, conn, tx)
                    EcrireIdentiteSeance(seance, conn, tx)
                    ' Le tarif a pu changer : le dû non encaissé suit le montant courant.
                    GestionPaiements.ResynchroniserDuSeance(seance.IdSeance, conn, tx)
                End Sub)

            GestionLog.EcrireLog(
                $"Séance mise à jour (ID={seance.IdSeance}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database
            )

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur UpdateSeance (id={seance.IdSeance}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CreeraitUnAvoir
    ' Version    : V1.1.0
    ' Date       : 21/10/2026
    '
    ' Rôle       :
    ' Indique si le passage d'une séance vers un statut donné ferait d'un encaissement un avoir,
    ' c'est-à-dire un paiement déjà perçu sur une séance qui ne sera finalement pas due.
    ' L'appelant (UI) propose alors d'utiliser cet avoir pour régler une autre séance du dossier
    ' (pas de remboursement implicite).
    '
    ' Paramètres :
    ' - idSeance  : Identifiant de la séance concernée (> 0)
    ' - codeCible : Code du statut envisagé
    '
    ' Retour     :
    ' - Boolean : True si un encaissement existe et que le statut cible n'est pas dû
    '
    ' Remarques  :
    ' - Fonction de consultation : aucune modification n'est effectuée en base.
    ' - En cas d'erreur, retourne False pour ne pas bloquer l'utilisateur (l'incident est journalisé).
    ' - V1.1.0 (21/10/2026) : renommée (NecessiteReaffectationPaiement) : la réaffectation est remplacée
    '   par l'utilisation de l'avoir (décision de Joëlle).
    ' -------------------------------------------------------------------------------------------------
    Public Function CreeraitUnAvoir(idSeance As Long, codeCible As String) As Boolean

        If idSeance <= 0 OrElse String.IsNullOrWhiteSpace(codeCible) Then
            Return False
        End If

        Try

            Dim statut = GestionStatutsSeance.GetStatutsSeanceActifs().
                FirstOrDefault(Function(s) String.Equals(s.CodeStatutSeance, codeCible, StringComparison.OrdinalIgnoreCase))

            If statut Is Nothing OrElse statut.EstDu Then
                Return False
            End If

            Return GestionPaiements.GetEtatPaiementsParSeance(idSeance).ADejaEncaisse

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur CreeraitUnAvoir (id={idSeance}, cible={codeCible}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Return False

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ReporterSeance
    ' Version    : V2.2.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Reporte une séance à une nouvelle date : la séance d'origine est marquée « Reportée » (son dû
    ' en attente est annulé), et une nouvelle séance « Planifiée » est créée à la date cible avec son
    ' propre dû. Le tout dans une seule transaction.
    '
    ' Paramètres :
    ' - idSeance           : Identifiant de la séance à reporter (> 0)
    ' - nouvelleDate       : Date de la nouvelle séance
    ' - nouvelleHeureDebut : Heure de début de la nouvelle séance (optionnelle)
    ' - nouvelleHeureFin   : Heure de fin de la nouvelle séance (optionnelle)
    '
    ' Retour     :
    ' - Long : Identifiant de la nouvelle séance créée
    '
    ' Remarques  :
    ' - La séance d'origine est conservée (traçabilité) avec le statut REPORTEE.
    ' - Le type, la nature, la durée et le montant figé sont recopiés depuis la séance d'origine.
    ' - Un encaissement de l'origine n'est pas touché : il y devient un avoir, que l'écran propose
    '   ensuite d'utiliser pour la nouvelle séance (opération distincte, choix de l'utilisatrice).
    ' - V2.0.0 (21/10/2026, audit B.2) : origine, nouvelle séance et leurs dûs dans une seule
    '   transaction. Avant, la nouvelle séance était créée après coup et, en cas d'échec, une
    '   « compensation » remettait l'origine en Planifiée — compensation qui pouvait elle-même
    '   échouer et laisser une séance reportée sans remplaçante (audit § 4.2). L'origine n'est marquée
    '   reportée que si elle est encore planifiée en base (écran périmé : refus). Plus de retour -1.
    ' - V2.1.0 (22/10/2026, audit C.1) : dossier clôturé ou archivé refusé.
    ' - V2.2.0 (22/10/2026, audit C.2) : règle du report lue dans StatutPermetReport ; la nouvelle
    '   séance est refusée si son créneau chevauche une autre séance (même règle que la fenêtre
    '   Séance ; le report n'en contrôlait aucun).
    ' -------------------------------------------------------------------------------------------------
    Public Function ReporterSeance(idSeance As Long,
                                   nouvelleDate As Date,
                                   Optional nouvelleHeureDebut As TimeSpan? = Nothing,
                                   Optional nouvelleHeureFin As TimeSpan? = Nothing) As Long

        If idSeance <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(idSeance))
        End If

        Try

            Dim seanceOrigine As Seance = GetSeance(idSeance)

            If seanceOrigine Is Nothing Then
                Throw New InvalidOperationException("Cette séance n'existe plus. Rechargez le dossier.")
            End If

            If Not StatutPermetReport(seanceOrigine.CodeStatutSeance) Then
                Throw New InvalidOperationException("Seule une séance planifiée peut être reportée.")
            End If

            Dim idStatutPlanifiee As Long = ResoudreIdStatutParCode(CodesSysteme.StatutsSeance.Planifiee)
            Dim idStatutReportee As Long = ResoudreIdStatutParCode(CodesSysteme.StatutsSeance.Reportee)

            ' Nouvelle séance (Planifiée) : identité recopiée de l'origine.
            Dim nouvelleSeance As New Seance With {
                .IdDossier = seanceOrigine.IdDossier,
                .IdPrixSeance = seanceOrigine.IdPrixSeance,
                .IdNatureSeance = seanceOrigine.IdNatureSeance,
                .IdStatutSeance = idStatutPlanifiee,
                .IdSeanceOrigine = idSeance,
                .DateSeance = nouvelleDate,
                .HeureDebut = nouvelleHeureDebut,
                .HeureFin = nouvelleHeureFin,
                .DureeMinutes = seanceOrigine.DureeMinutes,
                .TarifSeance = seanceOrigine.TarifSeance
            }

            Dim nouvelIdSeance As Long = DatabaseManager.ExecuterEnTransaction(Of Long)(
                Function(conn, tx)

                    GestionDossiers.ExigerSeanceModifiable(idSeance, conn, tx)

                    ' 1) Origine → Reportée, seulement si elle est encore planifiée ; son dû est annulé
                    '    (REPORTEE n'est pas un statut dû ; un encaissement y reste, en avoir).
                    If Not EcrireStatutSeanceSiInchange(idSeance, idStatutPlanifiee, idStatutReportee, conn, tx) Then
                        Throw New InvalidOperationException(
                            "Cette séance a changé entre-temps (elle n'est plus planifiée). Rechargez le dossier, puis recommencez si besoin.")
                    End If

                    GestionPaiements.ResynchroniserDuSeance(idSeance, conn, tx)

                    ' 2) Nouvelle séance et son dû, si son créneau est libre (l'origine, désormais
                    '    reportée, a libéré le sien).
                    Dim conflit As SeancePlanningItem = ChercherChevauchement(nouvelleSeance, Nothing, Nothing, conn, tx)
                    If conflit IsNot Nothing Then
                        Throw New InvalidOperationException(MessageChevauchement(conflit))
                    End If

                    Dim id As Long = InsererSeance(nouvelleSeance, conn, tx)
                    GestionPaiements.ResynchroniserDuSeance(id, conn, tx)

                    Return id

                End Function)

            nouvelleSeance.IdSeance = nouvelIdSeance

            GestionLog.EcrireLog(
                $"Séance reportée (origine ID={idSeance} → nouvelle ID={nouvelIdSeance}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database
            )

            Return nouvelIdSeance

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur ReporterSeance (id={idSeance}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DeleteSeance
    ' Version    : V1.2.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Supprime définitivement une séance.
    '
    ' Paramètres :
    ' - idSeance : Identifiant de la séance à supprimer (> 0)
    '
    ' Remarques  :
    ' - Délègue à QuerySeances.DeleteSeance.
    ' - V1.1.0 (22/10/2026, audit C.1) : dossier clôturé ou archivé refusé.
    ' - V1.2.0 (22/10/2026, audit C.2) : règle lue dans StatutPermetSuppression (celle du bouton).
    ' -------------------------------------------------------------------------------------------------
    Public Sub DeleteSeance(idSeance As Long)

        If idSeance <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(idSeance))
        End If

        Try

            Dim seance As Seance = GetSeance(idSeance)

            If seance Is Nothing Then
                Throw New InvalidOperationException($"Séance introuvable (ID={idSeance}).")
            End If

            If Not StatutPermetSuppression(seance.CodeStatutSeance) Then
                Throw New InvalidOperationException("Une séance reportée est conservée pour la traçabilité : elle ne peut pas être supprimée.")
            End If

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using tx As MySqlTransaction = conn.BeginTransaction()

                    Try

                        GestionDossiers.ExigerSeanceModifiable(idSeance, conn, tx)

                        Using cmdBlocage As New MySqlCommand(QuerySeances.SelectBlocagesSuppressionSeance, conn, tx)
                            cmdBlocage.Parameters.AddWithValue("@id_seance", idSeance)
                            Using rd As MySqlDataReader = cmdBlocage.ExecuteReader()
                                If rd.Read() Then
                                    If Convert.ToInt64(rd("nb_encaisses")) > 0 Then
                                        Throw New InvalidOperationException("Cette séance porte un encaissement (actif ou annulé) : elle ne peut pas être supprimée. Changez plutôt son statut.")
                                    End If
                                    If Convert.ToInt64(rd("nb_reports")) > 0 Then
                                        Throw New InvalidOperationException("Cette séance est à l'origine d'un report : elle ne peut pas être supprimée.")
                                    End If
                                End If
                            End Using
                        End Using

                        Dim idsPaiements As New List(Of Long)

                        Using cmdSel As New MySqlCommand(QuerySeances.SelectPaiementsNonEncaissesParSeance, conn, tx)
                            cmdSel.Parameters.AddWithValue("@id_seance", idSeance)
                            Using rd As MySqlDataReader = cmdSel.ExecuteReader()
                                While rd.Read()
                                    idsPaiements.Add(Convert.ToInt64(rd("id_paiement")))
                                End While
                            End Using
                        End Using

                        For Each idPaiement As Long In idsPaiements
                            Using cmdLia As New MySqlCommand(QueryPaiements.DeleteLiaisonsParPaiement, conn, tx)
                                cmdLia.Parameters.AddWithValue("@id_paiement", idPaiement)
                                cmdLia.ExecuteNonQuery()
                            End Using
                            Using cmdPai As New MySqlCommand(QueryPaiements.DeletePaiement, conn, tx)
                                cmdPai.Parameters.AddWithValue("@id_paiement", idPaiement)
                                cmdPai.ExecuteNonQuery()
                            End Using
                        Next

                        Using cmd As New MySqlCommand(QuerySeances.DeleteSeance, conn, tx)
                            cmd.Parameters.AddWithValue("@id_seance", idSeance)
                            cmd.ExecuteNonQuery()
                        End Using

                        tx.Commit()

                    Catch

                        tx.Rollback()
                        Throw

                    End Try

                End Using

            End Using

            GestionLog.EcrireLog(
                $"Séance supprimée (ID={idSeance}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database
            )

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur DeleteSeance (id={idSeance}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AppliquerDateHeureSeance
    ' Version    : V2.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Applique une date et des horaires à une séance sans toucher aux autres colonnes d'identité.
    ' Point d'entrée dédié anticipant la synchronisation avec Google Agenda (Lot 3, rendez_vous).
    '
    ' Paramètres :
    ' - contexte   : Qui déplace la séance (séance réalisée et encaissée : SuperUser)
    ' - idSeance   : Identifiant de la séance concernée (> 0)
    ' - dateSeance : Nouvelle date de la séance
    ' - heureDebut : Heure de début (optionnelle)
    ' - heureFin   : Heure de fin (optionnelle)
    ' - dureeMinutes : Durée en minutes (optionnelle)
    '
    ' Remarques  :
    ' - Recharge la séance pour préserver le type et le tarif, puis délègue à UpdateSeance.
    ' - Isole la logique de repositionnement temporel pour éviter un refactor lors de l'intégration agenda.
    ' - V2.0.0 (22/10/2026, audit C.1) : reçoit le ContexteOperation ; mêmes règles que la fenêtre
    '   Séance (séance encaissée, dossier figé).
    ' -------------------------------------------------------------------------------------------------
    Public Sub AppliquerDateHeureSeance(contexte As ContexteOperation, idSeance As Long, dateSeance As Date,
                                        Optional heureDebut As TimeSpan? = Nothing,
                                        Optional heureFin As TimeSpan? = Nothing,
                                        Optional dureeMinutes As Integer? = Nothing)

        If idSeance <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(idSeance))
        End If

        Dim seance As Seance = GetSeance(idSeance)

        If seance Is Nothing Then
            Throw New InvalidOperationException($"Séance introuvable (ID={idSeance}).")
        End If

        ExigerDeverrouillage(contexte, seance)

        seance.DateSeance = dateSeance
        seance.HeureDebut = heureDebut
        seance.HeureFin = heureFin
        seance.DureeMinutes = dureeMinutes

        UpdateSeance(seance)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : SeanceEstVerrouillee
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Indique qu'une séance est réalisée ET encaissée : sa modification est alors réservée au
    ' SuperUser (verrou « séance encaissée »). Une seule règle pour la fenêtre Séance et le métier.
    '
    ' Remarques  :
    ' - Une séance encore planifiée, même passée, reste modifiable : elle peut légitimement être
    '   annulée à l'avance (patient dédit, oubli de pointage).
    ' -------------------------------------------------------------------------------------------------
    Public Function SeanceEstVerrouillee(seance As Seance) As Boolean

        If seance Is Nothing OrElse seance.IdSeance <= 0 OrElse
           Not String.Equals(seance.CodeStatutSeance, CodesSysteme.StatutsSeance.Realisee, StringComparison.OrdinalIgnoreCase) Then
            Return False
        End If

        Return GestionPaiements.GetEtatPaiementsParSeance(seance.IdSeance).ADejaEncaisse

    End Function

    ' Refuse, sauf au SuperUser, la modification d'une séance réalisée et encaissée.
    Private Sub ExigerDeverrouillage(contexte As ContexteOperation, courante As Seance)

        If SeanceEstVerrouillee(courante) Then
            Autorisation.Exiger(contexte, AppRole.SuperUser,
                "Cette séance est réalisée et encaissée : seul un SuperUser ou un Administrateur peut la modifier.")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ExigerMontantAutorise
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Réserve au SuperUser un montant différent du tarif attendu (cadenas : prix d'ami, cas
    ' particulier).
    '
    ' Paramètres :
    ' - contexte      : Qui enregistre
    ' - seance        : Séance saisie
    ' - courante      : Séance en base (Nothing en création)
    ' - idStatutTarif : Statut qui détermine le tarif attendu
    '
    ' Remarques  :
    ' - Un montant laissé tel quel, sur le même tarif, est accepté même s'il diffère du tarif
    '   attendu : montant libre déjà fixé par un SuperUser, ou montant figé d'une séance dont le
    '   référentiel a changé depuis (décision de Joëlle, 22/10/2026).
    ' - Tarif introuvable : pas de montant attendu, rien à contrôler ici.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ExigerMontantAutorise(contexte As ContexteOperation, seance As Seance, courante As Seance,
                                      idStatutTarif As Long)

        If courante IsNot Nothing AndAlso
           seance.TarifSeance = courante.TarifSeance AndAlso
           seance.IdPrixSeance = courante.IdPrixSeance Then
            Return
        End If

        Dim idDossier As Long = If(courante IsNot Nothing, courante.IdDossier, seance.IdDossier)
        Dim attendu As Decimal? = GestionTarification.CalculerMontantAttendu(
            idDossier, seance.IdPrixSeance, idStatutTarif, seance.DateSeance)

        If attendu.HasValue AndAlso attendu.Value <> seance.TarifSeance Then
            Autorisation.Exiger(contexte, AppRole.SuperUser,
                "Seul un SuperUser ou un Administrateur peut fixer un montant de séance différent du tarif.")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Écritures unitaires dans une transaction (audit B.2)
    '
    ' Chacune reçoit la connexion et la transaction de l'unité de travail appelante
    ' (DatabaseManager.ExecuterEnTransaction) ; aucune n'ouvre sa propre connexion.
    ' -------------------------------------------------------------------------------------------------

    ' Insère la séance (statut déjà renseigné) et rend l'identifiant généré par la base.
    Private Function InsererSeance(seance As Seance, conn As MySqlConnection, tx As MySqlTransaction) As Long

        Using cmd As New MySqlCommand(QuerySeances.InsertSeance, conn, tx)
            AjouterParametresIdentite(cmd, seance)
            If cmd.ExecuteNonQuery() <> 1 Then
                Throw New System.Data.DataException($"Création de séance : aucune ligne insérée (id_dossier={seance.IdDossier}).")
            End If
        End Using

        Dim idSeance As Long
        Using cmdId As New MySqlCommand("SELECT LASTVAL(seq_seances);", conn, tx)
            idSeance = Convert.ToInt64(cmdId.ExecuteScalar())
        End Using

        If idSeance <= 0 Then
            Throw New System.Data.DataException($"Création de séance : identifiant invalide (id_dossier={seance.IdDossier}).")
        End If

        Return idSeance

    End Function

    ' Identité d'une séance existante (type, date, horaires, durée, montant).
    Private Sub EcrireIdentiteSeance(seance As Seance, conn As MySqlConnection, tx As MySqlTransaction)

        Using cmd As New MySqlCommand(QuerySeances.UpdateSeance, conn, tx)
            cmd.Parameters.AddWithValue("@id_prix_seance", seance.IdPrixSeance)
            cmd.Parameters.AddWithValue("@id_nature_seance", seance.IdNatureSeance)
            cmd.Parameters.AddWithValue("@date_seance", seance.DateSeance)
            cmd.Parameters.AddWithValue("@heure_debut", ValeurTimeOuDBNull(seance.HeureDebut))
            cmd.Parameters.AddWithValue("@heure_fin", ValeurTimeOuDBNull(seance.HeureFin))
            cmd.Parameters.AddWithValue("@duree_minutes", ValeurIntOuDBNull(seance.DureeMinutes))
            cmd.Parameters.AddWithValue("@tarif_seance", seance.TarifSeance)
            cmd.Parameters.AddWithValue("@id_seance", seance.IdSeance)
            cmd.ExecuteNonQuery()
        End Using

    End Sub

    ' Notes et évolution (RTF + texte brut) ; Nothing ou vide efface la zone.
    Private Sub EcrireNotesSeance(idSeance As Long, seance As Seance, conn As MySqlConnection, tx As MySqlTransaction)

        Using cmd As New MySqlCommand(QuerySeances.UpdateNotesSeance, conn, tx)
            cmd.Parameters.AddWithValue("@notes_seance_rtf", ValeurOuDBNull(seance.NotesSeanceRtf))
            cmd.Parameters.AddWithValue("@notes_seance_txt", ValeurOuDBNull(seance.NotesSeanceTxt))
            cmd.Parameters.AddWithValue("@evolution_rtf", ValeurOuDBNull(seance.EvolutionRtf))
            cmd.Parameters.AddWithValue("@evolution_txt", ValeurOuDBNull(seance.EvolutionTxt))
            cmd.Parameters.AddWithValue("@id_seance", idSeance)
            cmd.ExecuteNonQuery()
        End Using

    End Sub

    ' Statut changé seulement si la séance a encore le statut attendu ; False si elle a changé entre-temps.
    Private Function EcrireStatutSeanceSiInchange(idSeance As Long, idStatutAttendu As Long, idStatutCible As Long,
                                                  conn As MySqlConnection, tx As MySqlTransaction) As Boolean

        Using cmd As New MySqlCommand(QuerySeances.UpdateStatutSeanceSiInchange, conn, tx)
            cmd.Parameters.AddWithValue("@id_statut_seance", idStatutCible)
            cmd.Parameters.AddWithValue("@id_seance", idSeance)
            cmd.Parameters.AddWithValue("@id_statut_attendu", idStatutAttendu)
            Return cmd.ExecuteNonQuery() = 1
        End Using

    End Function

    ' Statut de la séance (le dû est aligné ensuite par l'appelant, dans la même transaction).
    Private Sub EcrireStatutSeance(idSeance As Long, idStatut As Long, conn As MySqlConnection, tx As MySqlTransaction)

        Using cmd As New MySqlCommand(QuerySeances.UpdateStatutSeance, conn, tx)
            cmd.Parameters.AddWithValue("@id_statut_seance", idStatut)
            cmd.Parameters.AddWithValue("@id_seance", idSeance)
            cmd.ExecuteNonQuery()
        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AjouterParametresIdentite
    ' Version    : V1.0.0
    ' Date       : 14/09/2026
    '
    ' Rôle       :
    ' Ajoute les paramètres SQL de l'INSERT d'une séance (colonnes d'identité).
    '
    ' Paramètres :
    ' - cmd    : Commande SQL à paramétrer
    ' - seance : Séance source des valeurs
    '
    ' Remarques  :
    ' - Les horaires et la durée sont optionnels : convertis en NULL si absents.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AjouterParametresIdentite(cmd As MySqlCommand, seance As Seance)

        cmd.Parameters.AddWithValue("@id_dossier", seance.IdDossier)
        cmd.Parameters.AddWithValue("@id_prix_seance", seance.IdPrixSeance)
        cmd.Parameters.AddWithValue("@id_nature_seance", seance.IdNatureSeance)
        cmd.Parameters.AddWithValue("@id_statut_seance", seance.IdStatutSeance)
        cmd.Parameters.AddWithValue("@id_seance_origine", ValeurLongOuDBNull(seance.IdSeanceOrigine))
        cmd.Parameters.AddWithValue("@date_seance", seance.DateSeance)
        cmd.Parameters.AddWithValue("@heure_debut", ValeurTimeOuDBNull(seance.HeureDebut))
        cmd.Parameters.AddWithValue("@heure_fin", ValeurTimeOuDBNull(seance.HeureFin))
        cmd.Parameters.AddWithValue("@duree_minutes", ValeurIntOuDBNull(seance.DureeMinutes))
        cmd.Parameters.AddWithValue("@tarif_seance", seance.TarifSeance)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ResoudreIdStatutParCode
    ' Version    : V1.0.0
    ' Date       : 14/09/2026
    '
    ' Rôle       :
    ' Résout l'identifiant d'un statut de séance depuis son code technique.
    '
    ' Paramètres :
    ' - codeStatut : Code technique recherché (ex. 'PLANIFIEE', 'REALISEE')
    '
    ' Retour     :
    ' - Long : Identifiant du statut correspondant
    '
    ' Remarques  :
    ' - L'absence du statut demandé est une erreur bloquante (référentiel incohérent).
    ' -------------------------------------------------------------------------------------------------
    Private Function ResoudreIdStatutParCode(codeStatut As String) As Long

        Dim statut = GestionStatutsSeance.GetStatutsSeanceActifs().
            FirstOrDefault(Function(s) String.Equals(s.CodeStatutSeance, codeStatut, StringComparison.OrdinalIgnoreCase))

        If statut Is Nothing Then

            Throw New InvalidOperationException(
                $"Statut de séance « {codeStatut} » introuvable dans ref_statuts_seance.")

        End If

        Return Convert.ToInt64(statut.IdStatutSeance)

    End Function

#End Region

#Region "Helpers de conversion"

    ' Convertit un TimeSpan nullable en valeur SQL : NULL si absent, sinon la valeur
    Private Function ValeurTimeOuDBNull(valeur As TimeSpan?) As Object

        If Not valeur.HasValue Then
            Return DBNull.Value
        End If

        Return valeur.Value

    End Function

    ' Convertit un entier nullable en valeur SQL : NULL si absent, sinon la valeur
    Private Function ValeurIntOuDBNull(valeur As Integer?) As Object

        If Not valeur.HasValue Then
            Return DBNull.Value
        End If

        Return valeur.Value

    End Function

    ' Retourne la valeur d'une colonne entière, ou Nothing si NULL
    Private Function LireIntNullable(reader As MySqlDataReader, colonne As String) As Integer?

        If reader(colonne) Is DBNull.Value Then
            Return Nothing
        End If

        Return Convert.ToInt32(reader(colonne))

    End Function

    ' Retourne la valeur d'une colonne décimale (NULL traité comme 0)
    Private Function LireDecimal(reader As MySqlDataReader, colonne As String) As Decimal

        If reader(colonne) Is DBNull.Value Then
            Return 0D
        End If

        Return Convert.ToDecimal(reader(colonne))

    End Function

    ' Retourne la valeur d'une colonne booléenne (tinyint(1)) — NULL traité comme False
    Private Function LireBool(reader As MySqlDataReader, colonne As String) As Boolean

        If reader(colonne) Is DBNull.Value Then
            Return False
        End If

        Return Convert.ToBoolean(reader(colonne))

    End Function

#End Region

#Region "Mapping"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : MapListeItem
    ' Version    : V1.0.0
    ' Date       : 14/09/2026
    '
    ' Rôle       :
    ' Mappe une ligne de DataReader (SelectSeancesParDossier) vers un SeanceListeItem.
    '
    ' Paramètres :
    ' - reader : DataReader positionné sur la ligne à mapper
    '
    ' Retour     :
    ' - SeanceListeItem : Élément de liste renseigné (montant réglé + reste dû calculé)
    ' -------------------------------------------------------------------------------------------------
    Private Function MapListeItem(reader As MySqlDataReader) As SeanceListeItem

        Dim tarif As Decimal = LireDecimal(reader, "tarif_seance")
        Dim montantRegle As Decimal = LireDecimal(reader, "montant_regle")

        Dim item As New SeanceListeItem With {
            .IdSeance = Convert.ToInt64(reader("id_seance")),
            .CodeSeance = LireString(reader, "code_seance"),
            .IdDossier = Convert.ToInt64(reader("id_dossier")),
            .IdPrixSeance = Convert.ToInt64(reader("id_prix_seance")),
            .LibellePrixSeance = LireString(reader, "libelle_prix_seance"),
            .IdNatureSeance = Convert.ToInt64(reader("id_nature_seance")),
            .LibelleNatureSeance = LireString(reader, "libelle_nature_seance"),
            .IdStatutSeance = Convert.ToInt64(reader("id_statut_seance")),
            .CodeStatutSeance = LireString(reader, "code_statut_seance"),
            .LibelleStatutSeance = LireString(reader, "libelle_statut_seance"),
            .DateSeance = CDate(reader("date_seance")),
            .HeureDebut = LireTimeNullable(reader, "heure_debut"),
            .DureeMinutes = LireIntNullable(reader, "duree_minutes"),
            .TarifSeance = tarif,
            .MontantRegle = montantRegle,
            .DateModification = CDate(reader("date_modification"))
        }

        item.HeureFin = LireTimeNullable(reader, "heure_fin")
        item.EstDuStatut = LireBool(reader, "est_du")
        item.IdSeanceOrigine = LireLongNullable(reader, "id_seance_origine")
        item.CodeSeanceOrigine = LireString(reader, "code_seance_origine")
        item.SoumisQuota = LireBool(reader, "soumis_quota")
        item.NoShowFacturable = LireBool(reader, "no_show_facturable")
        item.StatutEstAbsence = LireBool(reader, "est_absence")
        item.StatutNonTenu = LireBool(reader, "est_non_tenue")
        LireSolde(item, reader)
        Dim idCategorie As Long? = LireLongNullable(reader, "id_categorie_quota")
        item.IdCategorieQuota = If(idCategorie.HasValue, CULng(idCategorie.Value), 0UL)
        item.LibelleCategorieQuota = LireString(reader, "libelle_categorie_quota")
        item.StatutConsommeQuota = LireBool(reader, "consomme_quota")

        Return item

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : MapPlanningItem
    ' Version    : V1.1.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Mappe une ligne de DataReader (SelectSeancesParDate) vers un SeancePlanningItem, en ajoutant
    ' l'identité du patient et le libellé du domaine par rapport à MapListeItem.
    '
    ' Paramètres :
    ' - reader : DataReader positionné sur la ligne à mapper
    '
    ' Retour     :
    ' - SeancePlanningItem : Élément de planning renseigné
    '
    ' Remarques  :
    ' - V1.1.0 (23/10/2026) : aussi pour SelectSeancesParPeriode ; identifiant et code du patient, code
    '   du dossier.
    ' -------------------------------------------------------------------------------------------------
    Private Function MapPlanningItem(reader As MySqlDataReader) As SeancePlanningItem

        Dim tarif As Decimal = LireDecimal(reader, "tarif_seance")
        Dim montantRegle As Decimal = LireDecimal(reader, "montant_regle")

        Dim item As New SeancePlanningItem With {
            .IdSeance = Convert.ToInt64(reader("id_seance")),
            .CodeSeance = LireString(reader, "code_seance"),
            .IdDossier = Convert.ToInt64(reader("id_dossier")),
            .IdPrixSeance = Convert.ToInt64(reader("id_prix_seance")),
            .LibellePrixSeance = LireString(reader, "libelle_prix_seance"),
            .IdNatureSeance = Convert.ToInt64(reader("id_nature_seance")),
            .LibelleNatureSeance = LireString(reader, "libelle_nature_seance"),
            .IdStatutSeance = Convert.ToInt64(reader("id_statut_seance")),
            .CodeStatutSeance = LireString(reader, "code_statut_seance"),
            .LibelleStatutSeance = LireString(reader, "libelle_statut_seance"),
            .DateSeance = CDate(reader("date_seance")),
            .HeureDebut = LireTimeNullable(reader, "heure_debut"),
            .DureeMinutes = LireIntNullable(reader, "duree_minutes"),
            .TarifSeance = tarif,
            .MontantRegle = montantRegle,
            .DateModification = CDate(reader("date_modification")),
            .NomPatient = LireString(reader, "nom_patient"),
            .PrenomPatient = LireString(reader, "prenom_patient"),
            .LibelleDomaine = LireString(reader, "libelle_domaine"),
            .IdPatient = If(LireLongNullable(reader, "id_patient"), 0L),
            .CodePatient = LireString(reader, "code_patient"),
            .CodeDossier = LireString(reader, "code_dossier")
        }

        item.HeureFin = LireTimeNullable(reader, "heure_fin")
        item.EstDuStatut = LireBool(reader, "est_du")
        item.SoumisQuota = LireBool(reader, "soumis_quota")
        item.NoShowFacturable = LireBool(reader, "no_show_facturable")
        item.StatutEstAbsence = LireBool(reader, "est_absence")
        item.StatutNonTenu = LireBool(reader, "est_non_tenue")
        LireSolde(item, reader)

        Return item

    End Function

    ' Solde de la séance lu dans la vue v_seances_solde (une seule définition, audit B.4).
    Private Sub LireSolde(item As SeanceListeItem, reader As MySqlDataReader)
        item.EstFacturable = LireBool(reader, "est_facturable")
        item.ResteDu = LireDecimal(reader, "reste_du")
        item.AvoirSeance = LireDecimal(reader, "montant_avoir")
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : MapSeance
    ' Version    : V1.0.0
    ' Date       : 14/09/2026
    '
    ' Rôle       :
    ' Mappe une ligne de DataReader (SelectSeanceById) vers un objet Seance complet.
    '
    ' Paramètres :
    ' - reader : DataReader positionné sur la ligne à mapper
    '
    ' Retour     :
    ' - Seance : Objet métier renseigné (identité + notes/évolution)
    ' -------------------------------------------------------------------------------------------------
    Private Function MapSeance(reader As MySqlDataReader) As Seance

        Dim s As New Seance With {
            .IdSeance = Convert.ToInt64(reader("id_seance")),
            .CodeSeance = LireString(reader, "code_seance"),
            .IdDossier = Convert.ToInt64(reader("id_dossier")),
            .IdPrixSeance = Convert.ToInt64(reader("id_prix_seance")),
            .LibellePrixSeance = LireString(reader, "libelle_prix_seance"),
            .IdNatureSeance = Convert.ToInt64(reader("id_nature_seance")),
            .LibelleNatureSeance = LireString(reader, "libelle_nature_seance"),
            .IdStatutSeance = Convert.ToInt64(reader("id_statut_seance")),
            .CodeStatutSeance = LireString(reader, "code_statut_seance"),
            .LibelleStatutSeance = LireString(reader, "libelle_statut_seance"),
            .DateSeance = CDate(reader("date_seance")),
            .HeureDebut = LireTimeNullable(reader, "heure_debut"),
            .HeureFin = LireTimeNullable(reader, "heure_fin"),
            .DureeMinutes = LireIntNullable(reader, "duree_minutes"),
            .TarifSeance = LireDecimal(reader, "tarif_seance"),
            .NotesSeanceRtf = LireString(reader, "notes_seance_rtf"),
            .NotesSeanceTxt = LireString(reader, "notes_seance_txt"),
            .EvolutionRtf = LireString(reader, "evolution_rtf"),
            .EvolutionTxt = LireString(reader, "evolution_txt"),
            .DateCreation = CDate(reader("date_creation")),
            .DateModification = CDate(reader("date_modification"))
        }

        Return s

    End Function

#End Region

End Module
