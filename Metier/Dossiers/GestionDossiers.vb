' -------------------------------------------------------------------------------------------------
' Module      : GestionDossiers
' Projet      : Althéa
' Version     : V1.6.1
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Gestion métier des dossiers de suivi clinique (lecture, création, mise à jour, zones cliniques
' et cycle de vie des statuts).
'
' Responsabilités :
' - Charger la liste des dossiers d'un patient (vue allégée DossierListeItem)
' - Charger un dossier complet par identifiant (objet Dossier avec zones cliniques)
' - Créer et mettre à jour un dossier (identité, modalités)
' - Lire et mettre à jour le contenu des 7 zones cliniques
' - Appliquer les transitions de statut (cycle de vie) selon la whitelist autorisée
' - Contrôler la clôture : séances encore à venir (refus) et montants à régler (avertissement)
' - Refuser une écriture dans un dossier clôturé ou archivé (règle unique, statut relu en base)
' - Mapper les DataReader vers les objets métier (Dossier, DossierListeItem)
'
' Important   :
' - Aucun accès UI direct (pas de MessageBox)
' - Toute la logique SQL est déléguée à QueryDossiers
' - Les exceptions sont journalisées via GestionLog puis propagées (Throw)
'
' Architecture :
' UI → GestionDossiers → QueryDossiers → DatabaseManager → MariaDB
'
' Dépendances :
' - DatabaseManager (connexions DB)
' - QueryDossiers (requêtes SQL)
' - DbHelper (lecture NULL-safe des colonnes)
' - GestionLog (traçabilité)
' - Dossier / DossierListeItem / ControleClotureDossier (objets métier)
' - GestionSeances (séances du dossier, pour le contrôle de clôture)
'
' Historique  :
' - V1.1.0 (17/10/2026) : ControlerCloture ; ChangerStatutDossier refuse la clôture d'un dossier
'   qui a encore des séances à venir ; codes de statut publics (une seule définition).
' - V1.2.0 (17/10/2026) : ChoisirDossierDominant (aperçu du patient), sans codes écrits en dur.
' - V1.3.0 (20/10/2026) : logo du domaine lu avec le dossier (MapDossier).
' - V1.4.0 (21/10/2026, audit B.1) : ChangerStatutDossier relit le statut en base, écrit sous
'   condition et conserve la date de clôture à l'archivage.
' - V1.5.0 (21/10/2026, audit B.4) : dossier dominant désigné par la vue v_dossier_dominant
'   (ChoisirDossierDominant V2.0.0).
' - V1.6.0 (22/10/2026, audit C.1) : dossier figé refusé par le métier (RaisonDossierFige,
'   ExigerDossierModifiable, ExigerSeanceModifiable) ; archivé : fiche et zones refusées aussi.
' - V1.6.1 (24/10/2026, audit C.3) : constantes CODE_STATUT_* déplacées dans
'   CodesSysteme.StatutsDossier.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionDossiers

#Region "Lecture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetDossiersParPatient
    ' Version    : V1.0.0
    ' Date       : 21/07/2026
    '
    ' Rôle       :
    ' Charge la liste des dossiers d'un patient pour l'onglet Dossiers de la fiche patient.
    '
    ' Paramètres :
    ' - idPatient : Identifiant du patient dont on veut les dossiers
    '
    ' Retour     :
    ' - List(Of DossierListeItem) : Dossiers triés par date d'ouverture décroissante
    '
    ' Remarques  :
    ' - Utilise QueryDossiers.SelectDossiersParPatient
    ' - Le filtrage éventuel (statut/domaine) est réalisé en mémoire par l'UI (petite volumétrie)
    ' -------------------------------------------------------------------------------------------------
    Public Function GetDossiersParPatient(idPatient As Long) As List(Of DossierListeItem)

        Dim result As New List(Of DossierListeItem)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryDossiers.SelectDossiersParPatient, conn)

                    cmd.Parameters.AddWithValue("@id_patient", idPatient)

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
                "Erreur GetDossiersParPatient.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetCodeStatutDernierDossier
    ' Version    : V1.0.0
    ' Date       : 14/07/2026
    '
    ' Rôle       :
    ' Retourne le code du statut du dossier « dominant » d'un patient (le suivi affiché) :
    ' priorité aux dossiers vivants (ACTIF/PAUSE), sinon le dernier dossier clôturé/archivé.
    ' Reproduit la logique de priorité de QueryPatients.SelectPatientsListe.
    '
    ' Paramètres :
    ' - idPatient : Identifiant du patient
    '
    ' Retour     :
    ' - String : Code du statut dominant, ou Nothing si le patient n'a aucun dossier
    ' -------------------------------------------------------------------------------------------------
    Public Function GetCodeStatutDernierDossier(idPatient As Long) As String

        Return GetDossierDominant(idPatient)?.CodeStatutDossier

    End Function

    ' Retourne le dossier dominant avec le libellé de statut issu de ref_statuts_dossier.
    Public Function GetDossierDominant(idPatient As Long) As DossierListeItem

        Return ChoisirDossierDominant(idPatient, GetDossiersParPatient(idPatient))

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ChoisirDossierDominant
    ' Version    : V2.0.0
    ' Date       : 21/10/2026
    '
    ' Rôle       :
    ' Retrouve, parmi les dossiers déjà chargés d'un patient, le dossier « dominant » (celui qu'on
    ' montre comme suivi du patient), désigné par la vue v_dossier_dominant : un dossier vivant (actif
    ' ou en pause) d'abord, puis celui dont une séance est la plus proche d'aujourd'hui, puis le plus
    ' récemment ouvert.
    '
    ' Retour     :
    ' - DossierListeItem : le dossier dominant, ou Nothing s'il n'y a aucun dossier
    '
    ' Remarques  :
    ' - Sortie de GetDossierDominant (17/10/2026) pour servir aussi à l'aperçu du patient, qui a déjà
    '   les dossiers en main.
    ' - V2.0.0 (21/10/2026, audit B.4, décision de Joëlle) : la règle n'est plus écrite ici mais dans la
    '   vue (une seule définition avec la grille Patients, où statut et domaine pouvaient venir de deux
    '   dossiers différents) ; la proximité d'une séance départage deux dossiers vivants.
    ' -------------------------------------------------------------------------------------------------
    Public Function ChoisirDossierDominant(idPatient As Long, dossiers As IEnumerable(Of DossierListeItem)) As DossierListeItem

        If dossiers Is Nothing OrElse Not dossiers.Any() Then
            Return Nothing
        End If

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()
                Using cmd As New MySqlCommand(QueryDossiers.SelectIdDossierDominant, conn)
                    cmd.Parameters.AddWithValue("@id_patient", idPatient)
                    Dim valeur As Object = cmd.ExecuteScalar()
                    If valeur Is Nothing OrElse valeur Is DBNull.Value Then
                        Return Nothing
                    End If
                    Dim idDominant As Long = Convert.ToInt64(valeur)
                    Return dossiers.FirstOrDefault(Function(d) d.IdDossier = idDominant)
                End Using
            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur ChoisirDossierDominant (patient ID={idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetDossier
    ' Version    : V1.0.0
    ' Date       : 21/07/2026
    '
    ' Rôle       :
    ' Charge un dossier complet depuis son identifiant.
    '
    ' Paramètres :
    ' - idDossier : Identifiant du dossier à charger
    '
    ' Retour     :
    ' - Dossier : L'objet dossier, ou Nothing si introuvable
    '
    ' Remarques  :
    ' - Utilise QueryDossiers.SelectDossierById
    ' -------------------------------------------------------------------------------------------------
    Public Function GetDossier(idDossier As Long) As Dossier

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryDossiers.SelectDossierById, conn)

                    cmd.Parameters.AddWithValue("@id_dossier", idDossier)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        If reader.Read() Then
                            Return MapDossier(reader)
                        End If

                    End Using

                End Using

            End Using

            Return Nothing

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur GetDossier.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

#End Region

#Region "Écriture"

    ' Codes des statuts de dossier (cycle de vie, brique B6) : CodesSysteme.StatutsDossier (audit C.3 ;
    ' avant, constantes CODE_STATUT_* de ce module). Le cycle de vie reste une règle codée (whitelist
    ' ci-dessous).

    ' Table des transitions de statut autorisées (whitelist) : clé = statut courant, valeur = cibles légales.
    ' Règles D-Q5/D-Q10 : Archivé est terminal (aucune sortie) ; la réouverture n'existe que depuis Clôturé.
    Private ReadOnly _transitionsAutorisees As New Dictionary(Of String, String()) From {
        {CodesSysteme.StatutsDossier.Actif, {CodesSysteme.StatutsDossier.Pause, CodesSysteme.StatutsDossier.Cloture}},
        {CodesSysteme.StatutsDossier.Pause, {CodesSysteme.StatutsDossier.Actif, CodesSysteme.StatutsDossier.Cloture}},
        {CodesSysteme.StatutsDossier.Cloture, {CodesSysteme.StatutsDossier.Actif, CodesSysteme.StatutsDossier.Archive}},
        {CodesSysteme.StatutsDossier.Archive, Array.Empty(Of String)()}
    }

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CreateDossier
    ' Version    : V1.0.0
    ' Date       : 21/07/2026
    '
    ' Rôle       :
    ' Insère un nouveau dossier et retourne l'identifiant généré par la base.
    '
    ' Paramètres :
    ' - dossier : Dossier à créer (IdPatient, IdDomaine et DateOuverture doivent être renseignés)
    '
    ' Retour     :
    ' - Long : Identifiant du dossier créé, ou -1 en cas d'échec d'insertion
    '
    ' Remarques  :
    ' - Le statut initial est TOUJOURS forcé à « Actif » (résolu depuis ref_statuts_dossier)
    ' - id_dossier et code_dossier sont générés par la base (seq_dossiers + colonne GENERATED)
    ' - Utilise QueryDossiers.InsertDossier puis SELECT LASTVAL(seq_dossiers) sur la même connexion
    ' -------------------------------------------------------------------------------------------------
    Public Function CreateDossier(dossier As Dossier) As Long

        If dossier Is Nothing Then
            Throw New ArgumentNullException(NameOf(dossier))
        End If

        Try

            ' Statut initial forcé à Actif (indépendamment de la valeur fournie par l'appelant)
            dossier.IdStatutDossier = ResoudreIdStatutActif()

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryDossiers.InsertDossier, conn)

                    AjouterParametresIdentite(cmd, dossier)

                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected <> 1 Then

                        GestionLog.EcrireLog(
                            $"Création dossier échouée : aucune ligne insérée (id_patient={dossier.IdPatient}).",
                            GestionLog.LogLevel.Succinct,
                            GestionLog.LogCategory.Database
                        )

                        Return -1

                    End If

                End Using

                Using cmdId As New MySqlCommand("SELECT LASTVAL(seq_dossiers);", conn)

                    Dim idDossier As Long = Convert.ToInt64(cmdId.ExecuteScalar())

                    If idDossier <= 0 Then

                        GestionLog.EcrireLog(
                            $"Création dossier : ID invalide retourné (id_patient={dossier.IdPatient}).",
                            GestionLog.LogLevel.Succinct,
                            GestionLog.LogCategory.Database
                        )

                        Return -1

                    End If

                    dossier.IdDossier = idDossier

                    GestionLog.EcrireLog(
                        $"Dossier créé avec succès (ID={idDossier}, id_patient={dossier.IdPatient}).",
                        GestionLog.LogLevel.Rapide,
                        GestionLog.LogCategory.Database
                    )

                    Return idDossier

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur CreateDossier.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateDossier
    ' Version    : V1.1.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Met à jour l'identité d'un dossier existant (domaine, date d'ouverture, modalités).
    '
    ' Paramètres :
    ' - dossier : Dossier avec IdDossier > 0 et les propriétés d'identité remplies
    '
    ' Remarques  :
    ' - Utilise QueryDossiers.UpdateDossier
    ' - Ne modifie ni id_dossier, ni code_dossier, ni id_patient
    ' - Le statut n'est pas modifié ici (transitions de cycle de vie réservées à la brique B6)
    ' - V1.1.0 (22/10/2026, audit C.1) : un dossier archivé est refusé (ExigerDossierModifiable).
    ' -------------------------------------------------------------------------------------------------
    Public Sub UpdateDossier(dossier As Dossier)

        If dossier Is Nothing Then
            Throw New ArgumentNullException(NameOf(dossier))
        End If

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                ExigerDossierModifiable(dossier.IdDossier, ElementDossier.FicheEtZones, conn, Nothing)

                Using cmd As New MySqlCommand(QueryDossiers.UpdateDossier, conn)

                    cmd.Parameters.AddWithValue("@id_domaine", dossier.IdDomaine)
                    cmd.Parameters.AddWithValue("@date_ouverture", dossier.DateOuverture)
                    cmd.Parameters.AddWithValue("@modalites_paiement", ValeurOuDBNull(dossier.ModalitesPaiement))
                    cmd.Parameters.AddWithValue("@id_dossier", dossier.IdDossier)

                    cmd.ExecuteNonQuery()

                End Using

            End Using

            GestionLog.EcrireLog(
                $"Dossier mis à jour (ID={dossier.IdDossier}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database
            )

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur UpdateDossier (id={dossier.IdDossier}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AjouterParametresIdentite
    ' Version    : V1.0.0
    ' Date       : 21/07/2026
    '
    ' Rôle       :
    ' Ajoute les paramètres SQL de l'INSERT d'un dossier (colonnes d'identité).
    '
    ' Paramètres :
    ' - cmd     : Commande SQL à paramétrer
    ' - dossier : Dossier source des valeurs
    '
    ' Remarques  :
    ' - modalites_paiement est optionnel : chaîne vide convertie en NULL (ValeurOuDBNull)
    ' - id_therapeute_traitant volontairement non renseigné (reste NULL) — retiré de l'UI dossier
    ' -------------------------------------------------------------------------------------------------
    Private Sub AjouterParametresIdentite(cmd As MySqlCommand, dossier As Dossier)

        cmd.Parameters.AddWithValue("@id_patient", dossier.IdPatient)
        cmd.Parameters.AddWithValue("@id_domaine", dossier.IdDomaine)
        cmd.Parameters.AddWithValue("@id_statut_dossier", dossier.IdStatutDossier)
        cmd.Parameters.AddWithValue("@date_ouverture", dossier.DateOuverture)
        cmd.Parameters.AddWithValue("@modalites_paiement", ValeurOuDBNull(dossier.ModalitesPaiement))

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ResoudreIdStatutActif
    ' Version    : V1.0.0
    ' Date       : 21/07/2026
    '
    ' Rôle       :
    ' Résout l'identifiant du statut « Actif » depuis le référentiel des statuts de dossier.
    '
    ' Retour     :
    ' - Long : Identifiant du statut dont le code vaut CodesSysteme.StatutsDossier.Actif
    '
    ' Remarques  :
    ' - Le statut « Actif » est indispensable au fonctionnement : son absence est une erreur bloquante
    ' -------------------------------------------------------------------------------------------------
    Private Function ResoudreIdStatutActif() As Long

        Dim statutActif = GestionStatutsDossier.GetStatutsDossierActifs().
            FirstOrDefault(Function(s) String.Equals(s.CodeStatutDossier, CodesSysteme.StatutsDossier.Actif, StringComparison.OrdinalIgnoreCase))

        If statutActif Is Nothing Then

            Throw New InvalidOperationException(
                $"Statut de dossier « {CodesSysteme.StatutsDossier.Actif} » introuvable dans ref_statuts_dossier.")

        End If

        Return Convert.ToInt64(statutActif.IdStatutDossier)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ResoudreIdStatutParCode
    ' Version    : V1.0.0
    ' Date       : 14/07/2026
    '
    ' Rôle       :
    ' Résout l'identifiant d'un statut de dossier depuis son code technique.
    '
    ' Paramètres :
    ' - codeStatut : Code technique recherché (ex. 'PAUSE', 'CLOTURE', 'ARCHIVE')
    '
    ' Retour     :
    ' - Long : Identifiant du statut correspondant
    '
    ' Remarques  :
    ' - L'absence du statut demandé est une erreur bloquante (référentiel incohérent).
    ' -------------------------------------------------------------------------------------------------
    Private Function ResoudreIdStatutParCode(codeStatut As String) As Long

        Dim statut = GestionStatutsDossier.GetStatutsDossierActifs().
            FirstOrDefault(Function(s) String.Equals(s.CodeStatutDossier, codeStatut, StringComparison.OrdinalIgnoreCase))

        If statut Is Nothing Then

            Throw New InvalidOperationException(
                $"Statut de dossier « {codeStatut} » introuvable dans ref_statuts_dossier.")

        End If

        Return Convert.ToInt64(statut.IdStatutDossier)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : TransitionEstAutorisee
    ' Version    : V1.0.0
    ' Date       : 14/07/2026
    '
    ' Rôle       :
    ' Indique si le passage du statut courant vers un statut cible est autorisé (whitelist B6).
    '
    ' Paramètres :
    ' - codeCourant : Code du statut courant du dossier
    ' - codeCible   : Code du statut cible visé
    '
    ' Retour     :
    ' - Boolean : True si la transition est légale, False sinon
    ' -------------------------------------------------------------------------------------------------
    Public Function TransitionEstAutorisee(codeCourant As String, codeCible As String) As Boolean

        If String.IsNullOrWhiteSpace(codeCourant) OrElse String.IsNullOrWhiteSpace(codeCible) Then
            Return False
        End If

        Dim cibles As String() = Nothing

        If Not _transitionsAutorisees.TryGetValue(codeCourant.ToUpperInvariant(), cibles) Then
            Return False
        End If

        Return cibles.Any(Function(c) String.Equals(c, codeCible, StringComparison.OrdinalIgnoreCase))

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetTransitionsPossibles
    ' Version    : V1.0.0
    ' Date       : 14/07/2026
    '
    ' Rôle       :
    ' Retourne les codes de statut cibles légaux depuis un statut courant (pour alimenter l'UI).
    '
    ' Paramètres :
    ' - codeCourant : Code du statut courant du dossier
    '
    ' Retour     :
    ' - List(Of String) : Codes cibles autorisés (vide si aucun, ex. Archivé terminal)
    ' -------------------------------------------------------------------------------------------------
    Public Function GetTransitionsPossibles(codeCourant As String) As List(Of String)

        Dim cibles As String() = Nothing

        If String.IsNullOrWhiteSpace(codeCourant) OrElse
           Not _transitionsAutorisees.TryGetValue(codeCourant.ToUpperInvariant(), cibles) Then
            Return New List(Of String)()
        End If

        Return cibles.ToList()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ControlerCloture
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Examine les séances d'un dossier avant sa clôture : séances dues encore à venir (elles
    ' empêchent la clôture) et séances dues et passées qui restent à régler (signalées).
    '
    ' Paramètres :
    ' - idDossier : Identifiant du dossier (> 0)
    '
    ' Retour     :
    ' - ControleClotureDossier : résultat et messages associés.
    '
    ' Remarques  :
    ' - Mêmes règles que les grilles : « à venir » = statut dû et séance pas encore passée
    '   (SeanceListeItem.EstPassee, heure de fin comprise), payée d'avance ou non ; « à régler » =
    '   MontantExigible > 0. Une séance planifiée dont l'heure est passée compte comme réalisée.
    ' - Une séance non due (annulée, malade, reportée) ne bloque rien : son paiement éventuel est
    '   un avoir du patient, utilisable ailleurs.
    ' -------------------------------------------------------------------------------------------------
    Public Function ControlerCloture(idDossier As Long) As ControleClotureDossier

        If idDossier <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(idDossier))
        End If

        Dim seances As List(Of SeanceListeItem) = GestionSeances.GetSeancesParDossier(idDossier)

        Return New ControleClotureDossier With {
            .SeancesAVenir = seances.Where(Function(s) s.EstDuStatut AndAlso Not s.EstPassee).ToList(),
            .SeancesARegler = seances.Where(Function(s) s.MontantExigible > 0D).ToList()
        }

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ChangerStatutDossier
    ' Version    : V2.0.0
    ' Date       : 21/10/2026
    '
    ' Rôle       :
    ' Applique une transition de statut sur un dossier (cycle de vie, brique B6) : relit le statut en
    ' base, valide la transition, calcule les dates de clôture / archivage, persiste et journalise.
    '
    ' Paramètres :
    ' - idDossier   : Identifiant du dossier concerné (> 0)
    ' - codeAttendu : Code du statut affiché par l'écran (celui que l'utilisatrice a vu et confirmé)
    ' - codeCible   : Code du statut cible visé
    ' - dateCloture : Date de clôture obligatoire pour la cible CLOTURE, ignorée sinon
    '
    ' Remarques  :
    ' - Transition validée par la whitelist _transitionsAutorisees (aucune transition illégale).
    ' - CLOTURE : date_cloture obligatoire ; date_archivage remis à NULL. Refusée tant que des
    '   séances sont à venir (ControlerCloture) : l'écran le vérifie avant de demander confirmation,
    '   ce contrôle-ci garantit la règle pour tout autre appelant (InvalidOperationException dont le
    '   message s'adresse à l'utilisatrice).
    ' - ARCHIVE : date_archivage = aujourd'hui ; date_cloture conservée.
    ' - Réouverture (→ ACTIF depuis CLOTURE) : date_cloture et date_archivage remises à NULL.
    ' - PAUSE / réactivation : dates de clôture et d'archivage remises à NULL.
    ' - V2.0.0 (21/10/2026, audit B.1) :
    '   - le statut est relu en base et la transition validée sur lui, plus sur celui de l'écran ;
    '     s'il diffère de codeAttendu (dossier modifié ailleurs entre-temps), le changement est refusé
    '     plutôt que d'appliquer une transition que l'utilisatrice n'a pas vue ;
    '   - l'écriture est conditionnelle (statut inchangé depuis la relecture) : un dossier archivé ne
    '     peut plus être réactivé par un écran périmé ;
    '   - ARCHIVE conserve réellement la date de clôture (elle était remise à NULL).
    ' -------------------------------------------------------------------------------------------------
    Public Sub ChangerStatutDossier(idDossier As Long, codeAttendu As String, codeCible As String,
                                     Optional dateCloture As Date? = Nothing)

        If idDossier <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(idDossier))
        End If

        Dim actuel = LireStatutPourTransition(idDossier)

        If actuel.CodeStatut Is Nothing Then
            Throw New InvalidOperationException("Ce dossier est introuvable. Fermez-le puis rouvrez la liste des dossiers du patient.")
        End If

        If Not String.Equals(actuel.CodeStatut, codeAttendu, StringComparison.OrdinalIgnoreCase) Then
            RefuserStatutModifie(idDossier)
        End If

        Dim codeCourant As String = actuel.CodeStatut

        If Not TransitionEstAutorisee(codeCourant, codeCible) Then
            Throw New InvalidOperationException(
                $"Transition de statut non autorisée : « {codeCourant} » → « {codeCible} ».")
        End If

        Dim cible As String = codeCible.ToUpperInvariant()

        ' Calcul des dates selon la cible.
        Dim dateClotureFinale As Date? = Nothing
        Dim dateArchivageFinale As Date? = Nothing

        Select Case cible

            Case CodesSysteme.StatutsDossier.Cloture

                If Not dateCloture.HasValue Then
                    Throw New ArgumentException(
                        "La date de clôture est obligatoire pour clôturer un dossier.", NameOf(dateCloture))
                End If

                dateClotureFinale = dateCloture.Value

                Dim controle As ControleClotureDossier = ControlerCloture(idDossier)
                If controle.EstBloquant Then
                    Throw New InvalidOperationException(controle.MessageBlocage)
                End If

            Case CodesSysteme.StatutsDossier.Archive

                ' Archivage : on conserve la date de clôture existante et on horodate l'archivage.
                dateClotureFinale = actuel.DateCloture
                dateArchivageFinale = Date.Today

        End Select

        Dim nbLignes As Integer

        Try

            Dim idStatutCible As Long = ResoudreIdStatutParCode(cible)

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryDossiers.UpdateStatutDossier, conn)

                    cmd.Parameters.AddWithValue("@id_statut_dossier", idStatutCible)
                    cmd.Parameters.AddWithValue("@date_cloture", ValeurDateOuDBNull(dateClotureFinale))
                    cmd.Parameters.AddWithValue("@date_archivage", ValeurDateOuDBNull(dateArchivageFinale))
                    cmd.Parameters.AddWithValue("@id_dossier", idDossier)
                    cmd.Parameters.AddWithValue("@id_statut_courant", actuel.IdStatut)

                    nbLignes = cmd.ExecuteNonQuery()

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur ChangerStatutDossier (id={idDossier}, {codeCourant}→{codeCible}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

        ' Aucune ligne : le statut a changé entre la relecture et l'écriture.
        If nbLignes = 0 Then
            RefuserStatutModifie(idDossier)
        End If

        GestionLog.EcrireLog(
            $"Transition de statut dossier (ID={idDossier}) : {codeCourant} → {cible}.",
            GestionLog.LogLevel.Rapide,
            GestionLog.LogCategory.Database
        )

    End Sub

    ' Statut actuel d'un dossier, relu en base avant une transition (CodeStatut = Nothing : introuvable).
    Private Function LireStatutPourTransition(idDossier As Long) As (IdStatut As Long, CodeStatut As String, DateCloture As Date?)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()
                Using cmd As New MySqlCommand(QueryDossiers.SelectStatutDossierPourTransition, conn)
                    cmd.Parameters.AddWithValue("@id_dossier", idDossier)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        If Not reader.Read() Then
                            Return (0L, Nothing, Nothing)
                        End If
                        Return (Convert.ToInt64(reader("id_statut_dossier")),
                                LireString(reader, "code_statut_dossier"),
                                LireDateNullable(reader, "date_cloture"))
                    End Using
                End Using
            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur LireStatutPourTransition (id={idDossier}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' Refus commun aux deux contrôles de concurrence (statut différent de l'écran, écriture sans effet).
    Private Sub RefuserStatutModifie(idDossier As Long)

        GestionLog.EcrireLog(
            $"Changement de statut refusé : statut du dossier modifié entre-temps (ID={idDossier}).",
            GestionLog.LogLevel.Rapide,
            GestionLog.LogCategory.Database
        )

        Throw New InvalidOperationException(
            "Le statut de ce dossier a été modifié entre-temps (depuis une autre fenêtre ou un autre poste)." & Environment.NewLine &
            "Fermez puis rouvrez le dossier pour voir son état actuel, puis recommencez si besoin.")

    End Sub
    ' Version    : V1.0.0
    ' Date       : 08/07/2026
    '
    ' Rôle       :
    ' Charge le contenu (RTF + texte brut) d'UNE zone clinique d'un dossier.
    '
    ' Paramètres :
    ' - idDossier : Identifiant du dossier (> 0)
    ' - zone      : Zone clinique à charger
    '
    ' Retour     :
    ' - ContenuZoneDossier : contenu de la zone (Rtf/Txt éventuellement Nothing), ou Nothing si
    '   le dossier est introuvable
    '
    ' Remarques  :
    ' - La colonne interrogée provient de la whitelist ZonesDossier (anti-injection)
    ' -------------------------------------------------------------------------------------------------
    Public Function GetZoneDossier(idDossier As Long, zone As ZoneDossier) As ContenuZoneDossier

        Dim info As ZoneDossierInfo = ZonesDossier.GetInfo(zone)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryDossiers.SelectZoneDossier(info), conn)

                    cmd.Parameters.AddWithValue("@id_dossier", idDossier)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        If reader.Read() Then

                            Return New ContenuZoneDossier With {
                                .Rtf = LireString(reader, "zone_rtf"),
                                .Txt = LireString(reader, "zone_txt")
                            }

                        End If

                    End Using

                End Using

            End Using

            Return Nothing

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur GetZoneDossier (id={idDossier}, zone={zone}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateZoneDossier
    ' Version    : V1.1.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Met à jour de façon ciblée le contenu (RTF + texte brut) d'UNE zone clinique d'un dossier,
    ' sans réécrire les autres colonnes (enregistrement localisé, pattern UpdateAnamnesePatient).
    '
    ' Paramètres :
    ' - idDossier : Identifiant du dossier (> 0)
    ' - zone      : Zone clinique à mettre à jour
    ' - rtf       : Contenu RTF (Nothing/vide pour vider)
    ' - txt       : Texte brut (Nothing/vide pour vider)
    '
    ' Remarques  :
    ' - La zone HistoriqueArchive est en lecture seule (D-DOS-4) : toute écriture est refusée ici
    '   (garde-fou métier), sa gestion relevant de l'archivage (brique B6).
    ' - Les colonnes écrites proviennent de la whitelist ZonesDossier (anti-injection).
    ' - V1.1.0 (22/10/2026, audit C.1) : un dossier archivé est refusé (ExigerDossierModifiable).
    ' -------------------------------------------------------------------------------------------------
    Public Sub UpdateZoneDossier(idDossier As Long, zone As ZoneDossier, rtf As String, txt As String)

        Dim info As ZoneDossierInfo = ZonesDossier.GetInfo(zone)

        If info.LectureSeule Then
            Throw New InvalidOperationException(
                $"La zone « {info.Libelle} » est en lecture seule et ne peut pas être modifiée ici.")
        End If

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                ExigerDossierModifiable(idDossier, ElementDossier.FicheEtZones, conn, Nothing)

                Using cmd As New MySqlCommand(QueryDossiers.UpdateZoneDossier(info), conn)

                    cmd.Parameters.AddWithValue("@zone_rtf", ValeurOuDBNull(rtf))
                    cmd.Parameters.AddWithValue("@zone_txt", ValeurOuDBNull(txt))
                    cmd.Parameters.AddWithValue("@id_dossier", idDossier)

                    cmd.ExecuteNonQuery()

                End Using

            End Using

            GestionLog.EcrireLog(
                $"Zone « {info.Libelle} » mise à jour (dossier ID={idDossier}).",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.Database
            )

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur UpdateZoneDossier (id={idDossier}, zone={zone}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Sub

#End Region

#Region "Dossier figé (audit C.1)"

    ' Ce qu'une écriture touche dans un dossier : la règle diffère selon le statut (clôturé : séances
    ' et paiements figés ; archivé : tout est figé).
    Public Enum ElementDossier
        SeancesEtPaiements
        FicheEtZones
    End Enum

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : RaisonDossierFige
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Pourquoi un dossier refuse une écriture, selon son statut et ce qu'elle touche (vide s'il
    ' l'accepte). Une seule rédaction pour le refus du métier et pour la raison des boutons inactifs.
    '
    ' Paramètres :
    ' - codeStatut : Code du statut du dossier (ACTIF, PAUSE, CLOTURE, ARCHIVE)
    ' - element    : Ce que l'écriture touche
    '
    ' Remarques  :
    ' - En pause, le dossier reste modifiable : le suivi est suspendu, pas terminé.
    ' - Clôturé : séances et paiements figés ; la fiche et les zones cliniques restent modifiables.
    ' - Archivé : tout est figé, fiche comprise (archivage définitif ; décision de Joëlle, 22/10/2026).
    ' -------------------------------------------------------------------------------------------------
    Public Function RaisonDossierFige(codeStatut As String, element As ElementDossier) As String

        If String.Equals(codeStatut, CodesSysteme.StatutsDossier.Archive, StringComparison.OrdinalIgnoreCase) Then
            Return If(element = ElementDossier.SeancesEtPaiements,
                      "Dossier archivé : séances et paiements en consultation seulement.",
                      "Dossier archivé : consultation seulement (archivage définitif).")
        End If

        If element = ElementDossier.SeancesEtPaiements AndAlso
           String.Equals(codeStatut, CodesSysteme.StatutsDossier.Cloture, StringComparison.OrdinalIgnoreCase) Then
            Return "Dossier clôturé : séances et paiements en consultation seulement. " &
                   "Pour reprendre le suivi : « Changer le statut… »."
        End If

        Return String.Empty

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ExigerDossierModifiable
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Refuse une écriture dans un dossier clôturé ou archivé, d'après le statut relu en base (et non
    ' celui de l'écran, qui peut être périmé).
    '
    ' Paramètres :
    ' - idDossier : Dossier concerné (> 0)
    ' - element   : Ce que l'écriture touche
    ' - conn, tx  : Connexion et transaction de l'écriture (tx peut être Nothing)
    '
    ' Remarques  :
    ' - Refus : InvalidOperationException, message rédigé pour l'utilisatrice.
    ' -------------------------------------------------------------------------------------------------
    Friend Sub ExigerDossierModifiable(idDossier As Long, element As ElementDossier,
                                       conn As MySqlConnection, tx As MySqlTransaction)

        Dim codeStatut As String = Nothing

        Using cmd As New MySqlCommand(QueryDossiers.SelectStatutDossierPourTransition, conn, tx)
            cmd.Parameters.AddWithValue("@id_dossier", idDossier)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    codeStatut = LireString(reader, "code_statut_dossier")
                End If
            End Using
        End Using

        If codeStatut Is Nothing Then
            Throw New InvalidOperationException("Ce dossier n'existe plus. Fermez la fenêtre et rouvrez le patient.")
        End If

        RefuserSiFige(codeStatut, element)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ExigerSeanceModifiable
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Refuse l'écriture d'une séance, ou d'un paiement qui la règle, si son dossier est clôturé ou
    ' archivé (statut relu en base).
    '
    ' Paramètres :
    ' - idSeance : Séance concernée (> 0)
    ' - conn, tx : Connexion et transaction de l'écriture
    ' -------------------------------------------------------------------------------------------------
    Friend Sub ExigerSeanceModifiable(idSeance As Long, conn As MySqlConnection, tx As MySqlTransaction)

        Dim codeStatut As String

        Using cmd As New MySqlCommand(QueryDossiers.SelectStatutDossierParSeance, conn, tx)
            cmd.Parameters.AddWithValue("@id_seance", idSeance)
            codeStatut = TryCast(cmd.ExecuteScalar(), String)
        End Using

        If codeStatut Is Nothing Then
            Throw New InvalidOperationException("Cette séance n'existe plus. Fermez la fenêtre et rouvrez le dossier.")
        End If

        RefuserSiFige(codeStatut, ElementDossier.SeancesEtPaiements)

    End Sub

    Private Sub RefuserSiFige(codeStatut As String, element As ElementDossier)

        Dim raison As String = RaisonDossierFige(codeStatut, element)

        If raison.Length > 0 Then
            Throw New InvalidOperationException(raison)
        End If

    End Sub

#End Region

#Region "Mapping"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : MapListeItem
    ' Version    : V1.0.0
    ' Date       : 21/07/2026
    '
    ' Rôle       :
    ' Mappe une ligne de DataReader (SelectDossiersParPatient) vers un DossierListeItem.
    '
    ' Paramètres :
    ' - reader : DataReader positionné sur la ligne à mapper
    '
    ' Retour     :
    ' - DossierListeItem : Élément de liste renseigné
    ' -------------------------------------------------------------------------------------------------
    Private Function MapListeItem(reader As MySqlDataReader) As DossierListeItem

        Dim item As New DossierListeItem With {
            .IdDossier = Convert.ToInt64(reader("id_dossier")),
            .CodeDossier = LireString(reader, "code_dossier"),
            .IdPatient = Convert.ToInt64(reader("id_patient")),
            .IdDomaine = Convert.ToInt64(reader("id_domaine")),
            .LibelleDomaine = LireString(reader, "libelle_domaine"),
            .IdStatutDossier = Convert.ToInt64(reader("id_statut_dossier")),
            .CodeStatutDossier = LireString(reader, "code_statut_dossier"),
            .LibelleStatutDossier = LireString(reader, "libelle_statut_dossier"),
            .DateOuverture = CDate(reader("date_ouverture")),
            .DateCloture = LireDateNullable(reader, "date_cloture"),
            .TherapeuteTraitant = LireString(reader, "therapeute_traitant"),
            .MotifSuiviTxt = LireString(reader, "motif_suivi_txt"),
            .DateModification = CDate(reader("date_modification"))
        }

        Return item

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : MapDossier
    ' Version    : V1.1.0
    ' Date       : 20/10/2026
    '
    ' Rôle       :
    ' Mappe une ligne de DataReader (SelectDossierById) vers un objet Dossier complet.
    '
    ' Paramètres :
    ' - reader : DataReader positionné sur la ligne à mapper
    '
    ' Retour     :
    ' - Dossier : Objet métier renseigné (identité + 7 zones cliniques)
    ' - V1.1.0 (20/10/2026) : logo du domaine (LogoDomaine).
    ' -------------------------------------------------------------------------------------------------
    Private Function MapDossier(reader As MySqlDataReader) As Dossier

        Dim d As New Dossier With {
            .IdDossier = Convert.ToInt64(reader("id_dossier")),
            .CodeDossier = LireString(reader, "code_dossier"),
            .IdPatient = Convert.ToInt64(reader("id_patient")),
            .IdDomaine = Convert.ToInt64(reader("id_domaine")),
            .LibelleDomaine = LireString(reader, "libelle_domaine"),
            .LogoDomaine = LireString(reader, "logo_domaine"),
            .IdStatutDossier = Convert.ToInt64(reader("id_statut_dossier")),
            .CodeStatutDossier = LireString(reader, "code_statut_dossier"),
            .LibelleStatutDossier = LireString(reader, "libelle_statut_dossier"),
            .DateOuverture = CDate(reader("date_ouverture")),
            .DateCloture = LireDateNullable(reader, "date_cloture"),
            .DateArchivage = LireDateNullable(reader, "date_archivage"),
            .ModalitesPaiement = LireString(reader, "modalites_paiement"),
            .IdTherapeuteTraitant = LireLongNullable(reader, "id_therapeute_traitant"),
            .TherapeuteTraitant = LireString(reader, "therapeute_traitant"),
            .MotifSuiviRtf = LireString(reader, "motif_suivi_rtf"),
            .MotifSuiviTxt = LireString(reader, "motif_suivi_txt"),
            .HistoriqueRtf = LireString(reader, "historique_rtf"),
            .HistoriqueTxt = LireString(reader, "historique_txt"),
            .AntecedentsRtf = LireString(reader, "antecedents_rtf"),
            .AntecedentsTxt = LireString(reader, "antecedents_txt"),
            .DiagnosticRtf = LireString(reader, "diagnostic_rtf"),
            .DiagnosticTxt = LireString(reader, "diagnostic_txt"),
            .TraitementsEnCoursRtf = LireString(reader, "traitements_en_cours_rtf"),
            .TraitementsEnCoursTxt = LireString(reader, "traitements_en_cours_txt"),
            .ObjectifsSuiviRtf = LireString(reader, "objectifs_suivi_rtf"),
            .ObjectifsSuiviTxt = LireString(reader, "objectifs_suivi_txt"),
            .HistoriqueArchiveRtf = LireString(reader, "historique_archive_rtf"),
            .HistoriqueArchiveTxt = LireString(reader, "historique_archive_txt"),
            .DateCreation = CDate(reader("date_creation")),
            .DateModification = CDate(reader("date_modification"))
        }

        Return d

    End Function

#End Region

End Module
