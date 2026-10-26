' -------------------------------------------------------------------------------------------------
' Module      : GestionTarification
' Projet      : Althéa
' Version     : V1.4.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Résout le tarif proposé à la création d'une séance selon les règles PORTÉES PAR LE DOMAINE
' (lot 14) : tarif conventionné rattaché au domaine et historisé, tarif patient via le dossier,
' facturation ou non du no-show, et prise en compte du statut « dû ».
'
' Responsabilités :
' - Charger le contexte tarifaire d'un dossier (comportements du domaine, tarif patient rattaché)
' - Résoudre le tarif conventionné d'un domaine à la date de la séance
' - Indiquer si un statut de séance est « dû » (est_du) ou une absence non excusée (est_absence)
' - Calculer le tarif proposé pour une séance donnée (dossier + statut + date), repli sûr inclus
'
' Règles appliquées :
' 1. Statut non dû (est_du=0)      → 0
' 2. No-show non facturable        → 0 (règle portée par ref_domaines.no_show_facturable)
' 3. Domaine à tarif conventionné  → tarif rattaché au domaine, en vigueur à la date de la séance
' 4. Domaine standard              → tarif patient rattaché au dossier
' 5. Repli documenté               → montant nul et journalisation ; aucun tarif métier codé en dur
'
' Important   :
' - Aucun accès UI direct (pas de MessageBox)
' - Toute la logique SQL est déléguée à QueryTarification
' - Les exceptions sont journalisées via GestionLog puis propagées (Throw)
' - Le montant retourné est un instantané destiné à être FIGÉ dans seances.tarif_seance (§2.4)
' - Aucun code métier en dur ne pilote la tarification : cf. Rules.md §6 (pilotage par référentiels)
'
' Architecture :
' UI (SeanceEditor) → GestionTarification → QueryTarification → DatabaseManager → MariaDB
'
' Dépendances :
' - DatabaseManager (connexions DB)
' - QueryTarification (requêtes SQL)
' - GestionLog (traçabilité)
'
' Historique :
' - V1.2.0 (20/10/2026) : le no-show est le statut ABSENTE (GestionSeances.CODE_STATUT_ABSENTE) ;
'   la constante « NO_SHOW », code inexistant, rendait la règle « no-show non facturable » inopérante (audit B9).
' - V1.3.0 (22/10/2026, audit B.6, migration v2.53.0) : le no-show est reconnu par la case
'   « Absence non excusée » du statut (StatutEstAbsence) au lieu du code ABSENTE ;
'   ResoudreTarifSeance ne reçoit plus le code du statut ; lecture des drapeaux factorisée
'   (LireDrapeauStatut).
' - V1.4.0 (22/10/2026, audit C.1) : CalculerMontantAttendu (contrôle du montant libre par le métier).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionTarification

#Region "Type imbriqué"

    ' -------------------------------------------------------------------------------------------------
    ' Classe      : ContexteTarifaireDossier
    ' Version     : V1.1.0
    ' Date        : 15/09/2026
    '
    ' Rôle        :
    ' Transporte les comportements tarifaires du domaine et le tarif patient du dossier nécessaires
    ' à la résolution du montant d'une séance.
    '
    ' Important   :
    ' - DTO interne sans accès à la base ; les valeurs sont issues des référentiels et du dossier.
    ' -------------------------------------------------------------------------------------------------
    Public Class ContexteTarifaireDossier

        ' Domaine du dossier : c'est lui qui porte désormais les règles de facturation.
        Public Property IdDomaine As Long

        ' Vrai si le tarif provient du domaine (lia_prix_seance_domaines) et non du tarif patient.
        Public Property TarifConvention As Boolean

        ' Vrai si un no-show est facturé au patient. Faux pour un circuit conventionné qui ne
        ' rembourse pas les absences.
        Public Property NoShowFacturable As Boolean

        ' Vrai si les séances du domaine sont décomptées d'un quota conventionné.
        Public Property SoumisQuota As Boolean

        ' Montant du tarif patient rattaché au dossier (Nothing si aucun tarif patient renseigné)
        Public Property MontantPrixPatient As Decimal?

    End Class

#End Region

#Region "Lecture du contexte"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetContexteTarifaireDossier
    ' Version    : V1.0.0
    ' Date       : 15/09/2026
    '
    ' Rôle       :
    ' Charge le contexte tarifaire d'un dossier (comportements du domaine et montant du tarif patient).
    '
    ' Paramètres :
    ' - idDossier : Identifiant du dossier
    '
    ' Retour     :
    ' - ContexteTarifaireDossier : contexte renseigné, ou Nothing si le dossier est introuvable
    ' -------------------------------------------------------------------------------------------------
    Public Function GetContexteTarifaireDossier(idDossier As Long) As ContexteTarifaireDossier

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryTarification.SelectContexteTarifaireDossier, conn)

                    cmd.Parameters.AddWithValue("@id_dossier", idDossier)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        If reader.Read() Then

                            Dim ctx As New ContexteTarifaireDossier With {
                                .IdDomaine = Convert.ToInt64(reader("id_domaine")),
                                .TarifConvention = Convert.ToBoolean(reader("tarif_convention")),
                                .NoShowFacturable = Convert.ToBoolean(reader("no_show_facturable")),
                                .SoumisQuota = Convert.ToBoolean(reader("soumis_quota"))
                            }

                            If reader("montant_prix_patient") Is DBNull.Value Then
                                ctx.MontantPrixPatient = Nothing
                            Else
                                ctx.MontantPrixPatient = Convert.ToDecimal(reader("montant_prix_patient"))
                            End If

                            Return ctx

                        End If

                    End Using

                End Using

            End Using

            Return Nothing

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur GetContexteTarifaireDossier (id={idDossier}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetMontantParCodePrix
    ' Version    : V1.0.0
    ' Date       : 15/09/2026
    '
    ' Rôle       :
    ' Résout le montant courant d'un tarif de ref_prix_seance à partir de son code (ex. INAMI).
    '
    ' Paramètres :
    ' - codePrixSeance : Code technique du tarif (ex. 'INAMI')
    '
    ' Retour     :
    ' - Decimal? : montant courant, ou Nothing si le code est absent/inactif
    ' -------------------------------------------------------------------------------------------------
    Public Function GetMontantParCodePrix(codePrixSeance As String) As Decimal?

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryTarification.SelectMontantParCodePrix, conn)

                    cmd.Parameters.AddWithValue("@code_prix_seance", codePrixSeance)

                    Dim resultat As Object = cmd.ExecuteScalar()

                    If resultat Is Nothing OrElse resultat Is DBNull.Value Then
                        Return Nothing
                    End If

                    Return Convert.ToDecimal(resultat)

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur GetMontantParCodePrix (code={codePrixSeance}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : StatutEstDu
    ' Version    : V1.0.0
    ' Date       : 15/09/2026
    '
    ' Rôle       :
    ' Indique si un statut de séance est « dû » (ref_statuts_seance.est_du).
    '
    ' Paramètres :
    ' - idStatutSeance : Identifiant du statut de séance
    '
    ' Retour     :
    ' - Boolean : True si dû (ou statut introuvable → défaut prudent True), False si non dû
    ' -------------------------------------------------------------------------------------------------
    Public Function StatutEstDu(idStatutSeance As Long) As Boolean
        Return LireDrapeauStatut(QueryTarification.SelectEstDuParStatut, idStatutSeance, True, NameOf(StatutEstDu))
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : StatutEstAbsence
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Indique si un statut de séance traduit une absence non excusée (ref_statuts_seance.est_absence,
    ' case du référentiel, migration v2.53.0).
    '
    ' Retour     :
    ' - Boolean : True si coché ; False sinon, ou statut introuvable
    ' -------------------------------------------------------------------------------------------------
    Public Function StatutEstAbsence(idStatutSeance As Long) As Boolean
        Return LireDrapeauStatut(QueryTarification.SelectEstAbsenceParStatut, idStatutSeance, False, NameOf(StatutEstAbsence))
    End Function

    ' Lit un drapeau (0 / 1) d'un statut de séance ; valeurParDefaut si le statut est introuvable.
    Private Function LireDrapeauStatut(requete As String, idStatutSeance As Long, valeurParDefaut As Boolean,
                                       nomAppelant As String) As Boolean

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(requete, conn)

                    cmd.Parameters.AddWithValue("@id_statut_seance", idStatutSeance)

                    Dim resultat As Object = cmd.ExecuteScalar()

                    If resultat Is Nothing OrElse resultat Is DBNull.Value Then
                        Return valeurParDefaut
                    End If

                    Return Convert.ToBoolean(resultat)

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur {nomAppelant} (id={idStatutSeance}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Throw

        End Try

    End Function

#End Region

#Region "Résolution"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ResoudreTarifSeance
    ' Version    : V2.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Calcule le tarif proposé pour une séance à sa création, selon le contexte du dossier et le
    ' statut choisi (§7.2). Le montant retourné est destiné à être figé dans seances.tarif_seance.
    '
    ' Paramètres :
    ' - idDossier         : Identifiant du dossier de la séance
    ' - idStatutSeance    : Identifiant du statut choisi pour la séance
    ' - tarifDefautType   : Tarif par défaut du type de séance (repli si aucun tarif patient)
    ' - dateSeance        : Date de la séance, servant à résoudre le tarif conventionné en vigueur.
    '                       Optionnel : la date du jour est retenue à défaut, ce qui convient à la
    '                       création d'une séance mais fausserait la reprise d'une séance ancienne.
    '
    ' Retour     :
    ' - Decimal : montant proposé (>= 0)
    '
    ' Règles :
    ' 1. Statut non dû (est_du=0)   → 0
    ' 2. Domaine à tarif conventionné → no-show non facturable : 0 ; sinon tarif du domaine
    ' 3. Domaine standard            → tarif patient rattaché, à défaut repli sur tarifDefautType
    '
    ' Remarques  :
    ' - Les règles sont désormais portées par le domaine (lot 14) et non plus par le drapeau
    '   un drapeau global : un domaine conventionné dont le no-show serait facturé est donc exprimable.
    ' - V2.0.0 (22/10/2026, audit B.6, v2.53.0) : le no-show est le statut coché « Absence non
    '   excusée » (StatutEstAbsence) ; le paramètre codeStatutSeance est retiré.
    ' -------------------------------------------------------------------------------------------------
    Public Function ResoudreTarifSeance(idDossier As Long,
                                        idStatutSeance As Long,
                                        tarifDefautType As Decimal,
                                        Optional dateSeance As Date? = Nothing) As Decimal

        ' 1. Un statut non dû n'entraîne aucun montant.
        If Not StatutEstDu(idStatutSeance) Then
            Return 0D
        End If

        ' No-show = statut coché « Absence non excusée » dans le référentiel (v2.53.0, audit B.6).
        Dim estNoShow As Boolean = StatutEstAbsence(idStatutSeance)

        Dim ctx As ContexteTarifaireDossier = GetContexteTarifaireDossier(idDossier)

        ' Dossier introuvable : repli prudent sur le tarif par défaut du type.
        If ctx Is Nothing Then
            Return tarifDefautType
        End If

        ' 2. Un no-show n'est pas facturé lorsque le domaine ne le permet pas (circuit conventionné
        '    qui ne rembourse pas les absences). La règle vient du référentiel, plus du code.
        If estNoShow AndAlso Not ctx.NoShowFacturable Then
            Return 0D
        End If

        ' 3. Domaine à tarif conventionné : le montant provient du rattachement au domaine.
        If ctx.TarifConvention Then

            Dim dateReference As Date = If(dateSeance.HasValue, dateSeance.Value.Date, Date.Today)
            Dim montantConvention As Decimal? = GetTarifConventionDomaine(ctx.IdDomaine, dateReference)

            If montantConvention.HasValue Then
                Return montantConvention.Value
            End If

            ' Repli historique : aucun tarif rattaché au domaine à cette date. On conserve le
            ' comportement antérieur au lot 14 plutôt que de facturer un montant inattendu, mais
            ' l'anomalie de paramétrage doit être corrigée dans le référentiel.
            GestionLog.EcrireLog(
                $"Aucun tarif conventionne parametre pour le domaine {ctx.IdDomaine} au {dateReference:yyyy-MM-dd} : repli sur le tarif de code {QueryTarification.CODE_PRIX_INAMI}.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.General
            )

            Dim montantRepli As Decimal? = GetMontantParCodePrix(QueryTarification.CODE_PRIX_INAMI)
            Return If(montantRepli.HasValue, montantRepli.Value, tarifDefautType)

        End If

        ' 4. Domaine standard : tarif patient rattaché, sinon repli sur le tarif du type.
        If ctx.MontantPrixPatient.HasValue Then
            Return ctx.MontantPrixPatient.Value
        End If

        ' Repli silencieux à proscrire : sans tarif patient, le montant retenu est celui du tarif
        ' sélectionné, qui peut relever d'une autre logique de facturation. Le dossier doit se voir
        ' attribuer un tarif patient (dossiers.id_prix_seance).
        GestionLog.EcrireLog(
            $"Aucun tarif patient rattache au dossier {idDossier} (domaine {ctx.IdDomaine}) : repli sur le tarif selectionne ({tarifDefautType:0.00}).",
            GestionLog.LogLevel.Succinct,
            GestionLog.LogCategory.General
        )

        Return tarifDefautType

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CalculerMontantAttendu
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Montant qu'une séance doit porter sans intervention manuelle : celui du tarif choisi, ajusté
    ' par la résolution métier (statut, domaine, convention en vigueur à la date de la séance).
    '
    ' Paramètres :
    ' - idDossier      : Dossier de la séance
    ' - idPrixSeance   : Tarif choisi (ref_prix_seance)
    ' - idStatutSeance : Statut choisi
    ' - dateSeance     : Date de la séance
    '
    ' Retour     :
    ' - Decimal? : montant attendu, ou Nothing si le tarif est introuvable
    '
    ' Remarques  :
    ' - Sert au contrôle du montant libre (GestionSeances, audit C.1) : un montant différent est
    '   réservé au SuperUser. La fenêtre Séance fait le même calcul avec le tarif de sa liste.
    ' -------------------------------------------------------------------------------------------------
    Public Function CalculerMontantAttendu(idDossier As Long, idPrixSeance As Long,
                                           idStatutSeance As Long, dateSeance As Date) As Decimal?

        Dim prix As PrixSeance = GestionPrixSeance.GetPrixSeance(True).
            FirstOrDefault(Function(p) CLng(p.IdPrixSeance) = idPrixSeance)

        If prix Is Nothing Then
            Return Nothing
        End If

        Return ResoudreTarifSeance(idDossier, idStatutSeance, prix.Montant, dateSeance)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetTarifConventionDomaine
    ' Version    : V1.0.0
    ' Date       : 19/09/2026
    '
    ' Rôle       :
    ' Montant du tarif conventionné rattaché à un domaine et en vigueur à une date donnée.
    '
    ' Paramètres :
    ' - idDomaine     : Domaine concerné
    ' - dateReference : Date évaluée (date de la séance, et non date du jour)
    '
    ' Retour     :
    ' - Decimal? : montant applicable, ou Nothing si aucun tarif n'est rattaché à cette date
    '
    ' Remarques  :
    ' - Un chevauchement de périodes rendrait le montant facturé arbitraire : il est détecté et
    '   journalisé, sans interrompre la saisie (le tarif le plus récemment ouvert l'emporte).
    ' - Les erreurs sont journalisées et converties en Nothing afin que l'appelant applique son
    '   repli plutôt que d'échouer : un incident de lecture ne doit pas bloquer un encodage.
    ' -------------------------------------------------------------------------------------------------
    Public Function GetTarifConventionDomaine(idDomaine As Long, dateReference As Date) As Decimal?

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmdControle As New MySqlCommand(QueryTarification.SelectCountTarifsConventionApplicables, conn)

                    cmdControle.Parameters.AddWithValue("@id_domaine", idDomaine)
                    cmdControle.Parameters.AddWithValue("@date_reference", dateReference)

                    Dim nbApplicables As Integer = Convert.ToInt32(cmdControle.ExecuteScalar())

                    If nbApplicables > 1 Then
                        GestionLog.EcrireLog(
                            $"Chevauchement de tarifs conventionnes pour le domaine {idDomaine} au {dateReference:yyyy-MM-dd} : {nbApplicables} tarifs applicables, le plus recent est retenu.",
                            GestionLog.LogLevel.Succinct,
                            GestionLog.LogCategory.General
                        )
                    End If

                End Using

                Using cmd As New MySqlCommand(QueryTarification.SelectTarifConventionParDomaine, conn)

                    cmd.Parameters.AddWithValue("@id_domaine", idDomaine)
                    cmd.Parameters.AddWithValue("@date_reference", dateReference)

                    Dim resultat As Object = cmd.ExecuteScalar()

                    If resultat Is Nothing OrElse resultat Is DBNull.Value Then
                        Return Nothing
                    End If

                    Return Convert.ToDecimal(resultat)

                End Using

            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur GetTarifConventionDomaine (domaine={idDomaine}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )

            Return Nothing

        End Try

    End Function

#End Region

End Module
