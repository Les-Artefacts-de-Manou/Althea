' -------------------------------------------------------------------------------------------------
' Module      : GestionApercuPatient
' Projet      : Althéa
' Version     : V1.0.1
' Date        : 21/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Rassemble l'aperçu d'un patient (panneau de droite de l'écran Patients) à partir des modules
' métier existants : fiche, dossiers, séances, quota, contacts.
'
' Important   :
' - Aucune requête propre : tout passe par GestionPatients, GestionDossiers, GestionSeances,
'   GestionQuotaInami et GestionFamilleContacts, pour garder une seule définition de chaque règle.
' - Les sommes (à régler, avoir) suivent les règles des grilles et du bandeau du dossier :
'   SeanceListeItem.MontantExigible et AvoirSeance, comme GestionSeances.CalculerSoldesPatient.
' - Aucun accès UI ; les erreurs sont journalisées puis propagées.
' - V1.0.1 (21/10/2026, audit B.4) : dossier courant désigné par la vue v_dossier_dominant (même
'   règle que la grille Patients).
'
' Dépendances :
' - GestionPatients, GestionDossiers, GestionSeances, GestionQuotaInami, GestionFamilleContacts
' - ApercuPatient, SeanceListeItem, DossierListeItem, FamilleContact
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module GestionApercuPatient

    ' Nombre de séances listées de part et d'autre d'aujourd'hui (maquette du 17/10/2026).
    Private Const NB_PROCHAINES_SEANCES As Integer = 2
    Private Const NB_DERNIERES_SEANCES As Integer = 3

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetApercuPatient
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Charge l'aperçu d'un patient.
    '
    ' Paramètres :
    ' - idPatient : Identifiant du patient (> 0)
    '
    ' Retour     :
    ' - ApercuPatient : l'aperçu, ou Nothing si le patient n'existe pas
    '
    ' Remarques  :
    ' - Séances : seules les séances dues comptent (une séance annulée ou reportée n'est ni un
    '   rendez-vous ni une séance où le patient est venu) ; une absence non excusée figure parmi les
    '   dernières séances, avec son statut.
    ' - Quelques requêtes par appel (fiche, dossiers, séances de chaque dossier, quota, contacts) :
    '   l'écran ne l'appelle qu'une fois la sélection posée.
    ' -------------------------------------------------------------------------------------------------
    Public Function GetApercuPatient(idPatient As Long) As ApercuPatient

        If idPatient <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(idPatient))
        End If

        Try

            Dim patient As Patient = GestionPatients.GetPatientById(idPatient)
            If patient Is Nothing Then
                Return Nothing
            End If

            Dim apercu As New ApercuPatient With {
                .Patient = patient,
                .Dossiers = GestionDossiers.GetDossiersParPatient(idPatient),
                .Contacts = GestionFamilleContacts.GetContactsParPatient(idPatient)
            }

            apercu.DossierCourant = GestionDossiers.ChoisirDossierDominant(idPatient, apercu.Dossiers)

            Dim seances As New List(Of SeanceListeItem)
            For Each dossier As DossierListeItem In apercu.Dossiers
                seances.AddRange(GestionSeances.GetSeancesParDossier(dossier.IdDossier))
            Next

            Dim dues As List(Of SeanceListeItem) = seances.Where(Function(s) s.EstDuStatut).ToList()

            apercu.SeancesDuJour = dues.
                Where(Function(s) s.DateSeance.Date = Date.Today).
                OrderBy(Function(s) If(s.HeureDebut, TimeSpan.Zero)).
                ToList()

            apercu.ProchainesSeances = dues.
                Where(Function(s) s.DateSeance.Date > Date.Today).
                OrderBy(Function(s) s.DateSeance).ThenBy(Function(s) If(s.HeureDebut, TimeSpan.Zero)).
                Take(NB_PROCHAINES_SEANCES).
                ToList()

            apercu.DernieresSeances = dues.
                Where(Function(s) s.DateSeance.Date < Date.Today).
                OrderByDescending(Function(s) s.DateSeance).ThenByDescending(Function(s) If(s.HeureDebut, TimeSpan.Zero)).
                Take(NB_DERNIERES_SEANCES).
                ToList()

            apercu.MontantARegler = Math.Round(seances.Sum(Function(s) s.MontantExigible), 2)
            apercu.Avoir = Math.Round(seances.Sum(Function(s) s.AvoirSeance), 2)
            apercu.NbAbsences = seances.Where(Function(s) s.EstNoShow).Count()

            If apercu.DossierCourant IsNot Nothing Then
                Dim quota As QuotaInamiInfo = GestionQuotaInami.GetQuotaDossier(apercu.DossierCourant.IdDossier)
                If quota IsNot Nothing AndAlso quota.EstDomaineInami AndAlso quota.ARegle Then
                    apercu.QuotaDossierCourant = quota.LibelleCompteurs
                End If
            End If

            Return apercu

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur GetApercuPatient (patient ID={idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Database,
                ex
            )
            Throw

        End Try

    End Function

End Module
