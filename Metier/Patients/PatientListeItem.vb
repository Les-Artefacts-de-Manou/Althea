' -------------------------------------------------------------------------------------------------
' Classe      : PatientListeItem
' Projet      : Althéa
' Version     : V1.4.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Modèle de présentation léger pour la liste/recherche de patients (UC_PatientHome).
' Contient les colonnes affichées dans la grille ainsi que les compteurs de dossiers.
'
' Responsabilités :
' - Porter les données minimales d'un patient pour l'affichage en liste
' - Indiquer le nombre total de dossiers et de dossiers actifs
' - Fournir des helpers de présentation (NomComplet, AAlerte, textes de la grille)
'
' Remarques   :
' - Cette classe ne contient aucun accès base de données.
' - Elle est produite par GestionPatients à partir de la requête SelectPatientsListe.
' - Volontairement distincte de Patient (vue allégée, enrichie des compteurs de dossiers).
' - V1.1.0 (17/10/2026) : rendez-vous du patient — prochain, dernier vu, séances du jour — et
'   dossiers au statut Actif, pour la grille Patients « hub de la journée » (étape 1).
' - V1.2.0 (17/10/2026) : textes et familles d'affichage de la grille (étape 2) — groupe de la liste,
'   rendez-vous, dernière séance, suivi ; ordre « agenda » par défaut. APhoto, StatutPaiementAffichage
'   et DossierAffichage retirés (plus affichés).
' - V1.3.0 (17/10/2026) : repères du prochain rendez-vous et de la dernière séance (identifiant, code,
'   dossier) pour ouvrir la séance d'un double-clic depuis la grille ; SeanceDuRendezVous.
' - V1.4.0 (24/10/2026, recherche R4) : code postal, localité et pays de l'adresse (patients par lieu).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class PatientListeItem

#Region "Propriétés"

    ' Identifiant unique du patient (clé primaire)
    Public Property IdPatient As Long

    ' Code technique du patient (ex. 'PA000001')
    Public Property CodePatient As String

    ' Nom de famille du patient
    Public Property Nom As String

    ' Prénom du patient
    Public Property Prenom As String

    ' Date de naissance (optionnelle)
    Public Property DateNaissance As Date?

    ' Numéro d'identification (NISS)
    Public Property Niss As String

    ' Téléphone de contact
    Public Property Telephone As String

    ' Adresse e-mail de contact
    Public Property Email As String

    ' Lieu de l'adresse : code postal, localité, code ISO du pays (Nothing si non renseignés)
    Public Property CodePostal As String
    Public Property Localite As String
    Public Property CodePays As String

    ' Alerte patient en texte brut (pour affichage rapide de l'indicateur)
    Public Property AlerteTxt As String

    ' Nom seul du fichier photo d'identité (ou Nothing si aucune photo)
    Public Property PhotoFichier As String

    ' Nombre total de dossiers du patient
    Public Property NbDossiers As Integer

    ' Nombre de dossiers actifs (ni clôturés ni archivés)
    Public Property NbDossiersActifs As Integer

    ' Activation manuelle du patient (indépendante des dossiers) : True = actif, False = inactif
    Public Property IsActif As Boolean

    ' Code du statut du dossier « dominant » (dernier dossier vivant, sinon dernier clôturé/archivé) ; Nothing si aucun dossier
    Public Property CodeStatutDossierDominant As String

    ' Libellé lisible du statut dominant (pour la pastille/infobulle de la liste)
    Public Property LibelleStatutDossierDominant As String

    ' Code du domaine du dossier « courant » (PSY, GRA, TCA, REA…) ; Nothing si aucun dossier
    Public Property CodeDomaineDominant As String

    ' Date de dernière modification de la fiche patient
    Public Property DateModification As DateTime

    ' Montant réellement exigible : séances dues dont la date est atteinte, déduction faite des encaissements
    Public Property MontantDu As Decimal

    ' Montant conservé en avoir : encaissements subsistant sur des séances non dues (crédit réutilisable)
    Public Property MontantAvoir As Decimal

    ' Nombre d'absences non prévenues (statut ABSENTE), servant d'alerte comportementale
    Public Property NbNoShow As Integer

    ' Nombre de dossiers au statut Actif seulement (un dossier en pause n'appelle pas de rendez-vous)
    Public Property NbDossiersStatutActif As Integer

    ' Prochain rendez-vous : première séance due pas encore passée, tous dossiers (Nothing si aucune)
    Public Property ProchaineSeanceDate As Date?

    ' Heure de début du prochain rendez-vous (Nothing si la séance n'a pas d'heure)
    Public Property ProchaineSeanceHeure As TimeSpan?

    ' Dernière séance où le patient a été vu : due, passée, hors absence non excusée (Nothing si aucune)
    Public Property DerniereSeanceDate As Date?

    ' Heure de début de cette dernière séance (Nothing si elle n'a pas d'heure)
    Public Property DerniereSeanceHeure As TimeSpan?

    ' Séances dues du jour, passées ou non (le patient était attendu aujourd'hui)
    Public Property NbSeancesJour As Integer

    ' Séance du prochain rendez-vous (identifiant, code, dossier) ; Nothing si aucune
    Public Property ProchaineSeance As RepereSeance

    ' Dernière séance où le patient est venu (identifiant, code, dossier) ; Nothing si aucune
    Public Property DerniereSeance As RepereSeance

#End Region

#Region "Helpers"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SansDossier
    ' Rôle       : Indique que le patient n'a aucun dossier (état neutre distinct du suivi arrêté)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SansDossier As Boolean
        Get
            Return NbDossiers = 0
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : NomComplet
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    ' Rôle       : Fournit le nom complet du patient au format "Prénom NOM" pour l'affichage en liste
    ' Retour     :
    ' - String : Concaténation lisible du prénom et du nom (sans espaces superflus)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property NomComplet As String
        Get
            Return $"{Prenom} {Nom}".Trim()
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : AAlerte
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    ' Rôle       : Indique si le patient porte une alerte (basé sur le texte brut)
    ' Retour     :
    ' - Boolean : True si AlerteTxt n'est pas vide ou null, sinon False
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property AAlerte As Boolean
        Get
            Return Not String.IsNullOrWhiteSpace(AlerteTxt)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : ADesImpayes
    ' Version    : V1.0.0
    ' Date       : 17/09/2026
    ' Rôle       : Indique qu'une somme est réellement due par le patient
    ' Retour     :
    ' - Boolean : True si MontantDu est strictement positif
    ' Remarques  :
    ' - Les séances simplement planifiées ne sont pas comptées : elles ne constituent pas une créance.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property ADesImpayes As Boolean
        Get
            Return MontantDu > 0D
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : AUnAvoir
    ' Version    : V1.0.0
    ' Date       : 25/09/2026
    ' Rôle       : Indique que le patient dispose d'un avoir (crédit conservé) et n'a pas d'impayé.
    ' Retour     :
    ' - Boolean : True si MontantAvoir est strictement positif et qu'aucune somme n'est due.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property AUnAvoir As Boolean
        Get
            Return MontantAvoir > 0D AndAlso Not ADesImpayes
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : ASeanceAujourdhui
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    ' Rôle       : Indique que le patient avait ou a une séance due aujourd'hui (passée ou non)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property ASeanceAujourdhui As Boolean
        Get
            Return NbSeancesJour > 0
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : ProchaineSeanceEstAujourdhui
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    ' Rôle       : Indique que le prochain rendez-vous (pas encore passé) a lieu aujourd'hui
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property ProchaineSeanceEstAujourdhui As Boolean
        Get
            Return ProchaineSeanceDate.HasValue AndAlso ProchaineSeanceDate.Value.Date = Date.Today
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstSuiviSansRendezVous
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    ' Rôle       : Suivi actif sans aucun rendez-vous à venir : le patient risque d'être perdu de vue
    ' Remarques  :
    ' - Seuls les dossiers au statut Actif comptent : un dossier en pause n'appelle pas de rendez-vous
    '   (décision du 17/10/2026).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstSuiviSansRendezVous As Boolean
        Get
            Return NbDossiersStatutActif > 0 AndAlso Not ProchaineSeanceDate.HasValue
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : Age
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    ' Rôle       : Âge du patient aujourd'hui (Nothing sans date de naissance)
    ' Remarques  :
    ' - Même calcul que la fiche patient (Patient.CalculerAge).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property Age As Integer?
        Get
            If Not DateNaissance.HasValue Then
                Return Nothing
            End If
            Return Patient.CalculerAge(DateNaissance.Value, Date.Today)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : Groupe
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    ' Rôle       : Groupe de la liste dans l'ordre par défaut : vient aujourd'hui, a un rendez-vous à
    '              venir, ou n'en a pas (décision du 17/10/2026).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property Groupe As GroupeListePatient
        Get
            If ASeanceAujourdhui Then
                Return GroupeListePatient.Aujourdhui
            End If
            If ProchaineSeanceDate.HasValue Then
                Return GroupeListePatient.AVenir
            End If
            Return GroupeListePatient.SansRendezVous
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : OrdreAgenda
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    ' Rôle       : Clé de tri à l'intérieur d'un groupe : heure du jour pour « Aujourd'hui », date du
    '              rendez-vous pour « À venir » ; les patients sans rendez-vous se trient ensuite par nom.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property OrdreAgenda As DateTime
        Get
            Select Case Groupe
                Case GroupeListePatient.Aujourdhui
                    If ProchaineSeanceEstAujourdhui Then
                        Return Date.Today.Add(If(ProchaineSeanceHeure, TimeSpan.Zero))
                    End If
                    If DerniereSeanceDate.HasValue AndAlso DerniereSeanceDate.Value.Date = Date.Today Then
                        Return Date.Today.Add(If(DerniereSeanceHeure, TimeSpan.Zero))
                    End If
                    Return Date.Today
                Case GroupeListePatient.AVenir
                    Return RendezVousTri.Value
                Case Else
                    Return DateTime.MaxValue
            End Select
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : RendezVousTri
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    ' Rôle       : Date et heure du prochain rendez-vous (tri de la colonne « Prochain RDV »)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property RendezVousTri As DateTime?
        Get
            If Not ProchaineSeanceDate.HasValue Then
                Return Nothing
            End If
            Return ProchaineSeanceDate.Value.Date.Add(If(ProchaineSeanceHeure, TimeSpan.Zero))
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DerniereSeanceTri
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    ' Rôle       : Date et heure de la dernière séance où le patient est venu (tri de la colonne)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property DerniereSeanceTri As DateTime?
        Get
            If Not DerniereSeanceDate.HasValue Then
                Return Nothing
            End If
            Return DerniereSeanceDate.Value.Date.Add(If(DerniereSeanceHeure, TimeSpan.Zero))
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : FamilleRendezVous
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    ' Rôle       : Famille d'affichage de la pastille « Prochain RDV » (couleurs dans UtilsPastilles)
    ' Remarques  :
    ' - Une séance du jour déjà passée reste montrée (« Aujourd'hui 09:00 · passée ») : la grille sert
    '   aussi à revoir la journée ; le rendez-vous suivant est alors dans l'infobulle.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property FamilleRendezVous As FamilleRendezVousPatient
        Get
            If ProchaineSeanceEstAujourdhui Then
                Return FamilleRendezVousPatient.AujourdhuiAVenir
            End If
            If ASeanceAujourdhui Then
                Return FamilleRendezVousPatient.AujourdhuiPassee
            End If
            If ProchaineSeanceDate.HasValue Then
                Return FamilleRendezVousPatient.AVenir
            End If
            If EstSuiviSansRendezVous Then
                Return FamilleRendezVousPatient.SansRendezVous
            End If
            Return FamilleRendezVousPatient.Aucune
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SeanceDuRendezVous
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    ' Rôle       : Séance montrée dans la colonne « Prochain RDV », à ouvrir d'un double-clic : celle
    '              d'aujourd'hui ou à venir, ou la séance du jour déjà passée (prise de notes après
    '              coup) ; Nothing pour « Aucun RDV » ou une cellule vide.
    ' Remarques  :
    ' - Séance du jour passée : c'est la dernière séance où le patient est venu ; si c'était une
    '   absence non excusée, il n'y a pas de séance à ouvrir (Nothing).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SeanceDuRendezVous As RepereSeance
        Get
            Select Case FamilleRendezVous
                Case FamilleRendezVousPatient.AujourdhuiAVenir, FamilleRendezVousPatient.AVenir
                    Return ProchaineSeance
                Case FamilleRendezVousPatient.AujourdhuiPassee
                    If DerniereSeanceDate.HasValue AndAlso DerniereSeanceDate.Value.Date = Date.Today Then
                        Return DerniereSeance
                    End If
                    Return Nothing
                Case Else
                    Return Nothing
            End Select
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : RendezVousAffichage
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    ' Rôle       : Texte de la pastille « Prochain RDV » : « Aujourd'hui 14:00 », « Demain 09:00 »,
    '              « lun. 19/10 · 17:00 », « Aucun RDV » (suivi actif sans rendez-vous), sinon vide
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property RendezVousAffichage As String
        Get
            Select Case FamilleRendezVous
                Case FamilleRendezVousPatient.AujourdhuiAVenir
                    Return "Aujourd'hui" & HeureCourte(ProchaineSeanceHeure, " ")
                Case FamilleRendezVousPatient.AujourdhuiPassee
                    Dim heure As TimeSpan? =
                        If(DerniereSeanceDate.HasValue AndAlso DerniereSeanceDate.Value.Date = Date.Today, DerniereSeanceHeure, Nothing)
                    Return "Aujourd'hui" & HeureCourte(heure, " ") & " · passée"
                Case FamilleRendezVousPatient.AVenir
                    Return JourRendezVous(ProchaineSeanceDate.Value, ProchaineSeanceHeure)
                Case FamilleRendezVousPatient.SansRendezVous
                    Return "Aucun RDV"
                Case Else
                    Return String.Empty
            End Select
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DerniereSeanceAffichage
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    ' Rôle       : « aujourd'hui », « 26/09 · il y a 3 sem. », « 12/06/2025 · il y a 1 an », sinon vide
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property DerniereSeanceAffichage As String
        Get
            If Not DerniereSeanceDate.HasValue Then
                Return String.Empty
            End If
            Dim jour As Date = DerniereSeanceDate.Value.Date
            If jour = Date.Today Then
                Return "aujourd'hui"
            End If
            Dim format As String = If(jour.Year = Date.Today.Year, "dd/MM", "dd/MM/yyyy")
            Return $"{jour.ToString(format)} · {Anciennete(jour)}"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SuiviAffichage
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    ' Rôle       : Texte de la pastille « Suivi » : « Actif · PSY », « Sans dossier »
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SuiviAffichage As String
        Get
            If SansDossier Then
                Return "Sans dossier"
            End If
            Dim statut As String = If(String.IsNullOrWhiteSpace(LibelleStatutDossierDominant), "?", LibelleStatutDossierDominant)
            Return If(String.IsNullOrWhiteSpace(CodeDomaineDominant), statut, $"{statut} · {CodeDomaineDominant}")
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SuiviMention
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    ' Rôle       : Mention à droite de la pastille : « +1 » s'il existe d'autres dossiers, sinon vide
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SuiviMention As String
        Get
            Return If(NbDossiers > 1, $"+{NbDossiers - 1}", String.Empty)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : Anciennete
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    ' Rôle       : Durée écoulée depuis une date, en mots courts (« hier », « il y a 5 j »,
    '              « il y a 3 sem. », « il y a 2 mois », « il y a 1 an »)
    ' -------------------------------------------------------------------------------------------------
    Public Shared Function Anciennete(jour As Date) As String
        Dim jours As Integer = (Date.Today - jour.Date).Days
        Select Case jours
            Case Is <= 0
                Return "aujourd'hui"
            Case 1
                Return "hier"
            Case Is < 14
                Return $"il y a {jours} j"
            Case Is < 60
                Return $"il y a {jours \ 7} sem."
            Case Is < 365
                Return $"il y a {jours \ 30} mois"
            Case Else
                Dim ans As Integer = jours \ 365
                Return $"il y a {ans} an{If(ans > 1, "s", "")}"
        End Select
    End Function

    ' « Demain 09:00 », « lun. 19/10 · 17:00 », « 03/01/2027 » : jour du rendez-vous, l'heure si connue.
    Private Shared Function JourRendezVous(jour As Date, heure As TimeSpan?) As String
        If jour.Date = Date.Today.AddDays(1) Then
            Return "Demain" & HeureCourte(heure, " ")
        End If
        Dim texte As String = If(jour.Year = Date.Today.Year, jour.ToString("ddd dd/MM"), jour.ToString("dd/MM/yyyy"))
        Return texte & HeureCourte(heure, " · ")
    End Function

    ' Heure au format hh:mm précédée du séparateur indiqué, ou vide si l'heure est inconnue.
    Private Shared Function HeureCourte(heure As TimeSpan?, separateur As String) As String
        Return If(heure.HasValue, separateur & heure.Value.ToString("hh\:mm"), String.Empty)
    End Function

#End Region

End Class

' -------------------------------------------------------------------------------------------------
' Classe  : RepereSeance
' Version : V1.0.0
' Date    : 17/10/2026
'
' Rôle :
' Désigne une séance depuis la liste des patients (identifiant, code, dossier), le temps de l'ouvrir
' dans la fenêtre Séance. Aucune donnée clinique ici.
' -------------------------------------------------------------------------------------------------
Public Class RepereSeance

    Public Property IdSeance As Long

    Public Property CodeSeance As String

    Public Property IdDossier As Long

End Class

' -------------------------------------------------------------------------------------------------
' Énumérations : GroupeListePatient, FamilleRendezVousPatient
' Version : V1.0.0
' Date    : 17/10/2026
'
' Rôle :
' Groupes de l'ordre par défaut de la grille Patients et familles d'affichage de la pastille
' « Prochain RDV » ; les couleurs sont dans UtilsPastilles (couche UI).
' -------------------------------------------------------------------------------------------------
Public Enum GroupeListePatient
    Aujourdhui = 0
    AVenir = 1
    SansRendezVous = 2
End Enum

Public Enum FamilleRendezVousPatient
    Aucune
    AujourdhuiAVenir
    AujourdhuiPassee
    AVenir
    SansRendezVous
End Enum
