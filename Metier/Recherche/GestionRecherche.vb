' -------------------------------------------------------------------------------------------------
' Module      : GestionRecherche
' Projet      : Althéa
' Version     : V1.5.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Recherche transversale (ADR-52) : retrouver un élément sans passer par son patient, et savoir où
' aller. R1 : recherche par code (PA, DO, SE, PAI, FC, AS, TH) ; R2 : séances d'une période ; R3 :
' paiements d'une période ; R4 : patients par lieu ; R5 : réseau par nom (thérapeutes, intervenants,
' contacts et leurs patients).
'
' Remarques   :
' - Lecture seule ; SQL dans QueryRecherche ; requêtes paramétrées.
' - Confidentialité : le journal ne reçoit jamais ce qui a été cherché, seulement le type de
'   recherche et le nombre de résultats.
' - Les codes sont générés par la base : préfixe + identifiant sur 6 chiffres (colonnes STORED).
'   La saisie est tolérante : casse, espaces et zéros de tête sont libres (« se83 », « SE 000083 »).
'
' Dépendances :
' - DatabaseManager, QueryRecherche, DbHelper (lecture), GestionLog, ResultatRecherche,
'   PaiementListeItem (libellé du statut d'un paiement, même règle que l'onglet Paiements),
'   GestionSeances (séances d'une période, R2), GestionPaiements (paiements d'une période, R3),
'   GestionPatients et GestionCabinet (patients par lieu, R4), LieuPatients, UtilsCodePostal,
'   PersonneReseau et UtilsTexte (réseau par nom, R5)
'
' Historique  :
' - V1.1.0 (23/10/2026, R2) : RechercherSeances (séances d'une période, plafond LIMITE_RESULTATS),
'   ResultatDeSeance (aller à une séance listée).
' - V1.2.0 (23/10/2026, R3) : RechercherPaiements (relevé d'une période, tous patients),
'   TotauxEncaissesParMode, ResultatDePaiement.
' - V1.3.0 (24/10/2026, R4) : ChargerPatientsPourLieux et GrouperParLieu (lieux d'adresse et leurs
'   patients, recherche en mémoire), ResultatDePatient.
' - V1.4.0 (24/10/2026, R5) : ChargerReseau (une personne par ligne : thérapeute, intervenant saisi,
'   contact ; décisions de Joëlle), FiltrerReseau (nom ou début, en mémoire), ResultatDeLien.
' - V1.5.0 (24/10/2026, décision de Joëlle) : code TH… (thérapeute) reconnu par la recherche par
'   code ; il ouvre le réseau par nom sur ce thérapeute (OuvertureResultat).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionRecherche

    ' Préfixes des codes générés par la base, du plus long au plus court (PAI avant PA).
    Private ReadOnly PrefixesCodes As (Prefixe As String, Nature As NatureResultat)() = {
        ("PAI", NatureResultat.Paiement),
        ("PA", NatureResultat.Patient),
        ("DO", NatureResultat.Dossier),
        ("SE", NatureResultat.Seance),
        ("FC", NatureResultat.Contact),
        ("AS", NatureResultat.Intervenant),
        ("TH", NatureResultat.Therapeute)
    }

    ' Préfixes reconnus, pour l'aide de l'écran (« PA, DO, SE, PAI, FC, AS »).
    Public ReadOnly Property PrefixesReconnus As String
        Get
            Return String.Join(", ", PrefixesCodes.Select(Function(p) p.Prefixe).OrderBy(Function(p) p.Length).ThenBy(Function(p) p))
        End Get
    End Property

    ' Aide de l'écran : « PA patient, DO dossier, SE séance, PAI paiement, FC contact, AS intervenant ».
    Public ReadOnly Property AideCodes As String
        Get
            Dim ordre As NatureResultat() = {NatureResultat.Patient, NatureResultat.Dossier, NatureResultat.Seance,
                                            NatureResultat.Paiement, NatureResultat.Contact, NatureResultat.Intervenant,
                                            NatureResultat.Therapeute}
            Return String.Join(", ", ordre.Select(Function(n) $"{PrefixesCodes.First(Function(p) p.Nature = n).Prefixe} {ResultatRecherche.LibelleDeNature(n).ToLowerInvariant()}"))
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CodeElement
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Code d'un élément d'après sa nature et son identifiant (PAI000123), comme la base le génère.
    '
    ' Remarques  :
    ' - Pour l'annoncer sans relire la base juste après une création (ex. « Paiement PAI000123
    '   enregistré. ») ; même règle que les colonnes STORED (préfixe + identifiant sur 6 chiffres).
    ' -------------------------------------------------------------------------------------------------
    Public Function CodeElement(nature As NatureResultat, id As Long) As String
        Return PrefixesCodes.First(Function(p) p.Nature = nature).Prefixe & id.ToString("000000", Globalization.CultureInfo.InvariantCulture)
    End Function

#Region "Par code (R1)"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : InterpreterCode
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Reconnaît un code saisi : sa nature et l'identifiant qu'il désigne.
    '
    ' Paramètres :
    ' - saisie : texte tapé (« SE83 », « se 000083 »…)
    ' - nature : nature reconnue (en sortie)
    ' - id     : identifiant désigné (en sortie, > 0)
    '
    ' Retour     :
    ' - Boolean : True si la saisie a la forme d'un code connu (préfixe puis chiffres seulement)
    ' -------------------------------------------------------------------------------------------------
    Public Function InterpreterCode(saisie As String, ByRef nature As NatureResultat, ByRef id As Long) As Boolean

        id = 0
        Dim texte As String = If(saisie, String.Empty).Replace(" ", String.Empty).Trim().ToUpperInvariant()
        If texte.Length = 0 Then Return False

        For Each p In PrefixesCodes
            If Not texte.StartsWith(p.Prefixe, StringComparison.Ordinal) Then Continue For

            Dim chiffres As String = texte.Substring(p.Prefixe.Length)
            If chiffres.Length = 0 OrElse chiffres.Length > 9 OrElse Not chiffres.All(AddressOf Char.IsDigit) Then Return False

            id = Long.Parse(chiffres, Globalization.CultureInfo.InvariantCulture)
            nature = p.Nature
            Return id > 0
        Next

        Return False

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : RechercherParCode
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' L'élément désigné par un code, avec son patient et de quoi y aller ; liste vide si la saisie
    ' n'est pas un code reconnu (InterpreterCode) ou si aucun élément ne le porte.
    ' -------------------------------------------------------------------------------------------------
    Public Function RechercherParCode(saisie As String) As List(Of ResultatRecherche)

        Dim resultats As New List(Of ResultatRecherche)
        Dim nature As NatureResultat
        Dim id As Long

        If Not InterpreterCode(saisie, nature, id) Then Return resultats

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()
                Using cmd As New MySqlCommand(RequeteParCode(nature), conn)
                    cmd.Parameters.AddWithValue("@id", id)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            resultats.Add(MapParCode(nature, reader))
                        End While
                    End Using
                End Using
            End Using

            GestionLog.EcrireLog($"Recherche par code ({nature}) : {resultats.Count} résultat(s).",
                                 GestionLog.LogLevel.Rapide, GestionLog.LogCategory.Database)

            Return resultats

        Catch ex As Exception
            GestionLog.EcrireLog($"Erreur recherche par code ({nature}).",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database, ex)
            Throw
        End Try

    End Function

    Private Function RequeteParCode(nature As NatureResultat) As String
        Select Case nature
            Case NatureResultat.Patient : Return QueryRecherche.SelectPatientParId
            Case NatureResultat.Dossier : Return QueryRecherche.SelectDossierParId
            Case NatureResultat.Seance : Return QueryRecherche.SelectSeanceParId
            Case NatureResultat.Paiement : Return QueryRecherche.SelectPaiementParId
            Case NatureResultat.Contact : Return QueryRecherche.SelectContactParId
            Case NatureResultat.Intervenant : Return QueryRecherche.SelectIntervenantParId
            Case NatureResultat.Therapeute : Return QueryRecherche.SelectTherapeuteParId
            Case Else : Throw New ArgumentOutOfRangeException(NameOf(nature))
        End Select
    End Function

    ' Une ligne de QueryRecherche vers un résultat : colonnes communes, puis description selon la nature.
    Private Function MapParCode(nature As NatureResultat, reader As MySqlDataReader) As ResultatRecherche

        Dim r As New ResultatRecherche With {
            .Nature = nature,
            .IdPatient = If(LireLongNullable(reader, "id_patient"), 0L),
            .CodePatient = LireString(reader, "code_patient"),
            .NomPatient = $"{LireString(reader, "nom_patient")} {LireString(reader, "prenom_patient")}".Trim()
        }

        Select Case nature

            Case NatureResultat.Patient
                r.Code = LireString(reader, "code_patient")
                r.DateElement = LireDateNullable(reader, "date_naissance")
                r.Description = Assembler(
                    If(r.DateElement.HasValue, $"né(e) le {r.DateElement.Value:dd/MM/yyyy}", Nothing),
                    LireString(reader, "localite"),
                    If(LireBool(reader, "is_actif"), Nothing, "inactif"))

            Case NatureResultat.Dossier
                r.Code = LireString(reader, "code_dossier")
                r.IdDossier = LireLongNullable(reader, "id_dossier")
                r.CodeDossier = r.Code
                r.DateElement = LireDateNullable(reader, "date_ouverture")
                r.Description = Assembler(
                    LireString(reader, "libelle_domaine"),
                    LireString(reader, "libelle_statut_dossier"),
                    If(r.DateElement.HasValue, $"ouvert le {r.DateElement.Value:dd/MM/yyyy}", Nothing))

            Case NatureResultat.Seance
                r.Code = LireString(reader, "code_seance")
                r.IdSeance = LireLongNullable(reader, "id_seance")
                r.IdDossier = LireLongNullable(reader, "id_dossier")
                r.CodeDossier = LireString(reader, "code_dossier")
                r.DateElement = LireDateNullable(reader, "date_seance")
                Dim heure As TimeSpan? = LireTimeNullable(reader, "heure_debut")
                r.Description = Assembler(
                    If(r.DateElement.HasValue, $"le {r.DateElement.Value:dd/MM/yyyy}" &
                                               If(heure.HasValue, $" à {heure.Value:hh\:mm}", String.Empty), Nothing),
                    LireString(reader, "libelle_statut_seance"),
                    LireString(reader, "libelle_nature_seance"),
                    LireString(reader, "libelle_domaine"),
                    $"{LireDecimalDb(reader, "tarif_seance"):N2} €",
                    r.CodeDossier)

            Case NatureResultat.Paiement
                r.Code = LireString(reader, "code_paiement")
                r.IdPaiement = LireLongNullable(reader, "id_paiement")
                r.IdSeance = LireLongNullable(reader, "id_seance")
                r.IdDossier = LireLongNullable(reader, "id_dossier")
                r.CodeDossier = LireString(reader, "code_dossier")
                r.DateElement = LireDateNullable(reader, "date_paiement")
                ' Libellé du statut : la règle de l'onglet Paiements (Encaissé, En attente, Annulé, Clos).
                Dim statut As New PaiementListeItem With {
                    .StatutPaiement = LireString(reader, "statut_paiement"),
                    .DatePaiement = r.DateElement,
                    .DateAnnulation = LireDateNullable(reader, "date_annulation"),
                    .Commentaire = LireString(reader, "commentaire")
                }
                r.Description = Assembler(
                    $"{LireDecimalDb(reader, "montant"):N2} €",
                    statut.LibelleStatutPaiement,
                    If(r.DateElement.HasValue, $"le {r.DateElement.Value:dd/MM/yyyy}", Nothing),
                    LireString(reader, "libelle_mode_paiement"),
                    If(String.IsNullOrEmpty(LireString(reader, "code_seance")), Nothing, "séance " & LireString(reader, "code_seance")),
                    r.CodeDossier)

            Case NatureResultat.Contact
                r.Code = LireString(reader, "code_famille_contact")
                r.Description = Assembler(
                    $"{LireString(reader, "nom_contact")} {LireString(reader, "prenom_contact")}".Trim(),
                    LireString(reader, "libelle_lien_patient"))

            Case NatureResultat.Intervenant
                r.Code = LireString(reader, "code_autre_suivi_patient")
                r.DateElement = LireDateNullable(reader, "date_debut")
                r.Description = Assembler(
                    LireString(reader, "nom_professionnel"),
                    LireString(reader, "specialite"),
                    LireString(reader, "libelle_role_intervenant"))

            Case NatureResultat.Therapeute
                r.Code = LireString(reader, "code_therapeute")
                r.IdTherapeute = LireLongNullable(reader, "id_therapeute")
                r.NomElement = $"{LireString(reader, "nom")} {LireString(reader, "prenom")}".Trim()
                r.Description = Assembler(
                    r.NomElement,
                    LireString(reader, "libelle_specialite"),
                    If(LireBool(reader, "Actif"), Nothing, "inactif"))

        End Select

        Return r

    End Function

    ' Parties renseignées, séparées par « · ».
    Private Function Assembler(ParamArray parties As String()) As String
        Return String.Join(" · ", parties.Where(Function(p) Not String.IsNullOrWhiteSpace(p)).Select(Function(p) p.Trim()))
    End Function

#End Region

#Region "Séances d'une période (R2)"

    ' Plafond des listes de recherche (ADR-52) : au-delà, la liste est tronquée et l'écran demande
    ' de préciser la recherche (une période plus courte).
    Public Const LIMITE_RESULTATS As Integer = 1000

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : RechercherSeances
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Les séances d'une période (bornes incluses), tous patients confondus, dans l'ordre du
    ' calendrier, avec leur patient, leur dossier, leur statut et leur paiement.
    '
    ' Paramètres :
    ' - du, au    : bornes de la période (du ≤ au)
    ' - tronquee  : True en sortie si la période compte plus de LIMITE_RESULTATS séances (seules les
    '               premières sont rendues)
    '
    ' Remarques  :
    ' - Lecture par GestionSeances.GetSeancesParPeriode (même requête et même mapping que le planning
    '   du jour), qui journalise ses erreurs ; ici, le nombre de résultats seulement, jamais la période.
    ' -------------------------------------------------------------------------------------------------
    Public Function RechercherSeances(du As Date, au As Date, ByRef tronquee As Boolean) As List(Of SeancePlanningItem)

        tronquee = False

        If au.Date < du.Date Then
            Throw New ArgumentException("La fin de la période précède son début.", NameOf(au))
        End If

        ' Une ligne de plus que le plafond : savoir si la liste est complète.
        Dim seances As List(Of SeancePlanningItem) = GestionSeances.GetSeancesParPeriode(du, au, LIMITE_RESULTATS + 1)

        If seances.Count > LIMITE_RESULTATS Then
            tronquee = True
            seances.RemoveRange(LIMITE_RESULTATS, seances.Count - LIMITE_RESULTATS)
        End If

        GestionLog.EcrireLog($"Recherche des séances d'une période : {seances.Count} résultat(s){If(tronquee, " (liste plafonnée)", String.Empty)}.",
                             GestionLog.LogLevel.Rapide, GestionLog.LogCategory.Database)

        Return seances

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ResultatDeSeance
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Le résultat commun (ResultatRecherche) d'une séance listée, pour y aller par OuvertureResultat :
    ' une recherche spécialisée affiche ses propres colonnes, l'ouverture reste commune.
    ' -------------------------------------------------------------------------------------------------
    Public Function ResultatDeSeance(seance As SeancePlanningItem) As ResultatRecherche

        If seance Is Nothing Then Return Nothing

        Return New ResultatRecherche With {
            .Nature = NatureResultat.Seance,
            .Code = seance.CodeSeance,
            .DateElement = seance.DateSeance,
            .Description = Assembler(seance.LibelleStatutAffichage, seance.LibelleNatureSeance, seance.LibelleDomaine),
            .IdPatient = seance.IdPatient,
            .CodePatient = If(seance.CodePatient, String.Empty),
            .NomPatient = seance.PatientAffichage,
            .IdDossier = seance.IdDossier,
            .CodeDossier = If(seance.CodeDossier, String.Empty),
            .IdSeance = seance.IdSeance
        }

    End Function

#End Region

#Region "Paiements d'une période (R3)"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : RechercherPaiements
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Les lignes de paiement d'une période, tous patients : le relevé de l'onglet Paiements du
    ' dossier (décision de Joëlle, 23/10/2026) — encaissements à leur date, paiements attendus à la
    ' date de leur séance, annulations — avec leur patient et leur dossier.
    '
    ' Paramètres :
    ' - du, au    : bornes de la période (du ≤ au)
    ' - tronquee  : True en sortie si la période compte plus de LIMITE_RESULTATS lignes (seules les
    '               plus récentes sont rendues)
    '
    ' Remarques  :
    ' - Lecture par GestionPaiements.GetPaiementsParPeriode, qui journalise ses erreurs ; ici, le
    '   nombre de résultats seulement, jamais la période.
    ' -------------------------------------------------------------------------------------------------
    Public Function RechercherPaiements(du As Date, au As Date, ByRef tronquee As Boolean) As List(Of PaiementListeItem)

        tronquee = False

        If au.Date < du.Date Then
            Throw New ArgumentException("La fin de la période précède son début.", NameOf(au))
        End If

        Dim lignes As List(Of PaiementListeItem) = GestionPaiements.GetPaiementsParPeriode(du, au, LIMITE_RESULTATS + 1)

        If lignes.Count > LIMITE_RESULTATS Then
            tronquee = True
            lignes.RemoveRange(LIMITE_RESULTATS, lignes.Count - LIMITE_RESULTATS)
        End If

        GestionLog.EcrireLog($"Recherche des paiements d'une période : {lignes.Count} résultat(s){If(tronquee, " (liste plafonnée)", String.Empty)}.",
                             GestionLog.LogLevel.Rapide, GestionLog.LogCategory.Database)

        Return lignes

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : TotauxEncaissesParMode
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Ce qui a été encaissé, par mode de paiement (demande de Joëlle, 23/10/2026 : « Espèces 120 € ·
    ' Virement 300 € »), sur les lignes données.
    '
    ' Paramètres :
    ' - lignes         : lignes du relevé à totaliser (période, mode choisi)
    ' - regleParAvoir  : en sortie, montant réglé par avoir — pas de l'argent reçu : hors des totaux
    '
    ' Retour     :
    ' - Liste (mode, montant), par libellé de mode ; vide si rien n'est encaissé
    '
    ' Remarques  :
    ' - Encaissements non annulés seulement ; montant affecté à la séance (un paiement réparti compte
    '   une fois en tout, comme le résumé « Encaissé »). Un encaissement dont l'avoir a servi ensuite
    '   reste de l'argent reçu : il compte.
    ' -------------------------------------------------------------------------------------------------
    Public Function TotauxEncaissesParMode(lignes As IEnumerable(Of PaiementListeItem), ByRef regleParAvoir As Decimal) As List(Of (Mode As String, Montant As Decimal))

        regleParAvoir = 0D
        If lignes Is Nothing Then Return New List(Of (Mode As String, Montant As Decimal))

        Dim encaisses = lignes.Where(Function(p) p.EstEncaisse).ToList()
        regleParAvoir = encaisses.Where(Function(p) p.EstReglementParAvoir).Sum(Function(p) p.MontantAffecte)

        Return encaisses.
            Where(Function(p) Not p.EstReglementParAvoir).
            GroupBy(Function(p) If(String.IsNullOrWhiteSpace(p.ModePaiement), "Sans mode", p.ModePaiement)).
            Select(Function(g) (Mode:=g.Key, Montant:=g.Sum(Function(p) p.MontantAffecte))).
            Where(Function(t) t.Montant <> 0D).
            OrderBy(Function(t) t.Mode, StringComparer.CurrentCultureIgnoreCase).
            ToList()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ResultatDePaiement
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Le résultat commun (ResultatRecherche) d'une ligne de paiement listée, pour y aller par
    ' OuvertureResultat : le dossier, onglet Paiements, le paiement sélectionné.
    ' -------------------------------------------------------------------------------------------------
    Public Function ResultatDePaiement(paiement As PaiementListeItem) As ResultatRecherche

        If paiement Is Nothing Then Return Nothing

        Return New ResultatRecherche With {
            .Nature = NatureResultat.Paiement,
            .Code = paiement.CodePaiement,
            .DateElement = paiement.DateMouvement,
            .Description = Assembler(paiement.MouvementCourt, paiement.EtatAffichage, paiement.ModePaiement),
            .IdPatient = paiement.IdPatient,
            .CodePatient = If(paiement.CodePatient, String.Empty),
            .NomPatient = paiement.PatientAffichage,
            .IdDossier = paiement.IdDossier,
            .CodeDossier = If(paiement.CodeDossier, String.Empty),
            .IdSeance = paiement.IdSeance,
            .IdPaiement = paiement.IdPaiement
        }

    End Function

#End Region

#Region "Patients par lieu (R4)"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ChargerPatientsPourLieux
    ' Version    : V1.0.0
    ' Date       : 24/10/2026
    '
    ' Rôle       :
    ' Les patients avec leur adresse (la liste de l'écran Patients) et le pays du cabinet, chargés
    ' une fois par l'écran des patients par lieu ; la recherche se fait ensuite en mémoire, à chaque
    ' frappe (GrouperParLieu).
    '
    ' Paramètres :
    ' - codePaysCabinet : en sortie, code ISO du pays du cabinet (vide s'il n'est pas renseigné)
    '
    ' Remarques  :
    ' - Lecture par GestionPatients.GetPatients, qui journalise ses erreurs ; ici, le nombre de
    '   patients seulement.
    ' -------------------------------------------------------------------------------------------------
    Public Function ChargerPatientsPourLieux(ByRef codePaysCabinet As String) As List(Of PatientListeItem)

        codePaysCabinet = CodePaysDuCabinet()
        Dim patients As List(Of PatientListeItem) = GestionPatients.GetPatients()

        GestionLog.EcrireLog($"Recherche des patients par lieu : {patients.Count} patient(s) chargé(s).",
                             GestionLog.LogLevel.Rapide, GestionLog.LogCategory.Database)

        Return patients

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GrouperParLieu
    ' Version    : V1.0.0
    ' Date       : 24/10/2026
    '
    ' Rôle       :
    ' Les lieux d'adresse (code postal, localité, pays) qui répondent à un texte, avec les patients de
    ' chacun (décisions de Joëlle, 24/10/2026 : une zone « code postal ou localité », la liste des
    ' lieux et leurs patients ; inactifs comme dans l'écran Patients).
    '
    ' Paramètres :
    ' - patients           : patients chargés (ChargerPatientsPourLieux)
    ' - texte              : mots cherchés ; un mot en chiffres = début du code postal (« 43 » →
    '                        4300, 4317…), un autre mot = partie de la localité, sans tenir compte
    '                        des accents ni des majuscules ; tous les mots doivent correspondre.
    '                        Vide : tous les lieux, et les patients sans adresse.
    ' - inclureInactifs    : True pour compter aussi les patients désactivés
    ' - codePaysCabinet    : pays du cabinet (une adresse sans pays en est ; non répété au libellé)
    ' - nbInactifsMasques  : en sortie, patients inactifs des lieux trouvés laissés de côté
    '
    ' Retour     :
    ' - Les lieux par code postal puis localité (« Sans adresse » en dernier), chacun avec ses
    '   patients par nom puis prénom.
    '
    ' Remarques  :
    ' - En mémoire, sans journal : rien de ce qui est cherché n'est écrit.
    ' -------------------------------------------------------------------------------------------------
    Public Function GrouperParLieu(patients As IEnumerable(Of PatientListeItem), texte As String, inclureInactifs As Boolean,
                                   codePaysCabinet As String, ByRef nbInactifsMasques As Integer) As List(Of LieuPatients)

        nbInactifsMasques = 0
        If patients Is Nothing Then Return New List(Of LieuPatients)

        Dim mots As String() = If(texte, String.Empty).Split({" "c, ","c, ";"c}, StringSplitOptions.RemoveEmptyEntries)

        Dim trouves As List(Of PatientListeItem) = patients.Where(Function(p) CorrespondLieu(p, mots)).ToList()

        If Not inclureInactifs Then
            nbInactifsMasques = trouves.Where(Function(p) Not p.IsActif).Count()
            trouves = trouves.Where(Function(p) p.IsActif).ToList()
        End If

        Return trouves.
            GroupBy(Function(p) LieuPatients.CleDe(p.CodePays, p.CodePostal, p.Localite, codePaysCabinet)).
            Select(Function(g) CreerLieu(g.Key, g.ToList(), codePaysCabinet)).
            OrderBy(Function(l) l.EstSansAdresse).
            ThenBy(Function(l) l.CodePostal, StringComparer.OrdinalIgnoreCase).
            ThenBy(Function(l) l.Localite, StringComparer.CurrentCultureIgnoreCase).
            ToList()

    End Function

    ' Le patient répond-il à tous les mots (chiffres : début du code postal ; sinon : partie de la
    ' localité) ? Sans mot, tout patient répond, adresse ou non.
    Private Function CorrespondLieu(patient As PatientListeItem, mots As String()) As Boolean

        If mots.Length = 0 Then Return True

        Dim cp As String = UtilsCodePostal.CleCodePostal(patient.CodePostal)
        Dim localite As String = UtilsCodePostal.CleLocalite(patient.Localite)

        For Each mot As String In mots
            If mot.All(AddressOf Char.IsDigit) Then
                If Not cp.StartsWith(mot, StringComparison.Ordinal) Then Return False
            Else
                Dim cle As String = UtilsCodePostal.CleLocalite(mot)
                If cle.Length > 0 AndAlso localite.IndexOf(cle, StringComparison.Ordinal) < 0 Then Return False
            End If
        Next

        Return True

    End Function

    ' Un lieu et ses patients ; la localité affichée est l'écriture la plus fréquente du groupe.
    Private Function CreerLieu(cle As String, patients As List(Of PatientListeItem), paysCabinet As String) As LieuPatients

        Dim lieu As New LieuPatients With {
            .CodePaysCabinet = paysCabinet,
            .EstSansAdresse = cle.Length = 0,
            .Patients = patients.
                OrderBy(Function(p) p.Nom, StringComparer.CurrentCultureIgnoreCase).
                ThenBy(Function(p) p.Prenom, StringComparer.CurrentCultureIgnoreCase).
                ToList()
        }

        If Not lieu.EstSansAdresse Then
            Dim modele As PatientListeItem = patients.
                GroupBy(Function(p) $"{If(p.CodePostal, String.Empty).Trim()}|{If(p.Localite, String.Empty).Trim()}").
                OrderByDescending(Function(g) g.Count()).
                First().First()
            lieu.CodePostal = If(modele.CodePostal, String.Empty).Trim()
            lieu.Localite = If(modele.Localite, String.Empty).Trim()
            lieu.CodePays = If(String.IsNullOrWhiteSpace(modele.CodePays), paysCabinet, modele.CodePays.Trim())
        End If

        Return lieu

    End Function

    ' Code ISO du pays du cabinet ; vide s'il n'est pas renseigné ou illisible (les lieux affichent
    ' alors leur pays).
    Private Function CodePaysDuCabinet() As String
        Try
            Return If(GestionCabinet.GetCabinetActif()?.CodePays, String.Empty).Trim().ToUpperInvariant()
        Catch ex As Exception
            GestionLog.EcrireException("GestionRecherche.CodePaysDuCabinet : lecture du pays du cabinet impossible", ex,
                                       category:=GestionLog.LogCategory.Database)
            Return String.Empty
        End Try
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ResultatDePatient
    ' Version    : V1.0.0
    ' Date       : 24/10/2026
    '
    ' Rôle       :
    ' Le résultat commun (ResultatRecherche) d'un patient listé, pour aller à sa fiche par
    ' OuvertureResultat.
    ' -------------------------------------------------------------------------------------------------
    Public Function ResultatDePatient(patient As PatientListeItem) As ResultatRecherche

        If patient Is Nothing Then Return Nothing

        Return New ResultatRecherche With {
            .Nature = NatureResultat.Patient,
            .Code = If(patient.CodePatient, String.Empty),
            .DateElement = patient.DateNaissance,
            .Description = Assembler($"{patient.CodePostal} {patient.Localite}".Trim(), If(patient.IsActif, Nothing, "inactif")),
            .IdPatient = patient.IdPatient,
            .CodePatient = If(patient.CodePatient, String.Empty),
            .NomPatient = $"{patient.Nom} {patient.Prenom}".Trim()
        }

    End Function

#End Region

#Region "Réseau par nom (R5)"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ChargerReseau
    ' Version    : V1.0.0
    ' Date       : 24/10/2026
    '
    ' Rôle       :
    ' Les personnes du réseau des patients — thérapeutes du référentiel, intervenants saisis, contacts —
    ' avec leurs liens (décisions de Joëlle, 24/10/2026) : une ligne par personne, un intervenant lié
    ' à un thérapeute compte sous ce thérapeute ; chargées une fois par l'écran, la recherche se fait
    ' ensuite en mémoire (FiltrerReseau).
    '
    ' Remarques  :
    ' - Trois lectures sur une connexion (pas d'UNION : collations différentes entre tables).
    ' - Journal : le nombre de personnes seulement.
    ' -------------------------------------------------------------------------------------------------
    Public Function ChargerReseau() As List(Of PersonneReseau)

        Dim personnes As New Dictionary(Of String, PersonneReseau)(StringComparer.Ordinal)
        ' Écritures rencontrées par personne (nom, prénom, précision), pour afficher la plus fréquente.
        Dim ecritures As New Dictionary(Of String, List(Of (Nom As String, Prenom As String, Precision As String)))(StringComparer.Ordinal)

        Try

            Using conn As MySqlConnection = DatabaseManager.OpenConnection()

                Using cmd As New MySqlCommand(QueryRecherche.SelectReseauTherapeutes, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim t As New PersonneReseau With {
                                .Nature = NaturePersonneReseau.Therapeute,
                                .IdTherapeute = Convert.ToInt64(reader("id_therapeute")),
                                .CodeTherapeute = If(LireString(reader, "code_therapeute"), String.Empty),
                                .Nom = If(LireString(reader, "nom"), String.Empty).Trim(),
                                .Prenom = If(LireString(reader, "prenom"), String.Empty).Trim(),
                                .Precision = If(LireString(reader, "libelle_specialite"), String.Empty).Trim(),
                                .TherapeuteActif = LireBool(reader, "Actif")
                            }
                            personnes(t.Cle) = t
                        End While
                    End Using
                End Using

                Using cmd As New MySqlCommand(QueryRecherche.SelectReseauIntervenants, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim lien As LienReseau = LireLienReseau(reader, NatureResultat.Intervenant,
                                                                    "id_autre_suivi_patient", "code_autre_suivi_patient",
                                                                    LireString(reader, "libelle_role_intervenant"))
                            lien.DateDebut = LireDateNullable(reader, "date_debut")
                            lien.DateFin = LireDateNullable(reader, "date_fin")

                            Dim idTherapeute As Long = If(LireLongNullable(reader, "id_therapeute"), 0L)
                            Dim cleTherapeute As String = PersonneReseau.CleDe(NaturePersonneReseau.Therapeute, idTherapeute, Nothing, Nothing)

                            If idTherapeute > 0 AndAlso personnes.ContainsKey(cleTherapeute) Then
                                personnes(cleTherapeute).Liens.Add(lien)
                            Else
                                AjouterLien(personnes, ecritures, NaturePersonneReseau.Intervenant,
                                            LireString(reader, "nom_professionnel"), Nothing,
                                            LireString(reader, "specialite"), lien)
                            End If
                        End While
                    End Using
                End Using

                Using cmd As New MySqlCommand(QueryRecherche.SelectReseauContacts, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim lien As LienReseau = LireLienReseau(reader, NatureResultat.Contact,
                                                                    "id_famille_contact", "code_famille_contact",
                                                                    LireString(reader, "libelle_lien_patient"))
                            AjouterLien(personnes, ecritures, NaturePersonneReseau.Contact,
                                        LireString(reader, "nom_contact"), LireString(reader, "prenom_contact"),
                                        Nothing, lien)
                        End While
                    End Using
                End Using

            End Using

        Catch ex As Exception
            GestionLog.EcrireLog("Erreur recherche du réseau des patients.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database, ex)
            Throw
        End Try

        ' Écriture affichée : la plus fréquente ; précision d'un contact : ses liens (« Mère, Tutrice »).
        For Each kv In ecritures
            Dim p As PersonneReseau = personnes(kv.Key)
            Dim modele = kv.Value.GroupBy(Function(e) $"{e.Nom}|{e.Prenom}").OrderByDescending(Function(g) g.Count()).First().First()
            p.Nom = modele.Nom
            p.Prenom = modele.Prenom
            If p.Nature = NaturePersonneReseau.Contact Then
                p.Precision = String.Join(", ", p.Liens.Select(Function(l) l.Role).Where(Function(r) Not String.IsNullOrWhiteSpace(r)).Distinct(StringComparer.CurrentCultureIgnoreCase))
            Else
                p.Precision = kv.Value.Select(Function(e) e.Precision).Where(Function(s) Not String.IsNullOrWhiteSpace(s)).
                    GroupBy(Function(s) s, StringComparer.CurrentCultureIgnoreCase).OrderByDescending(Function(g) g.Count()).
                    Select(Function(g) g.Key).FirstOrDefault()
                If p.Precision Is Nothing Then p.Precision = String.Empty
            End If
        Next

        For Each p As PersonneReseau In personnes.Values
            p.Liens = p.Liens.
                OrderBy(Function(l) l.NomPatient, StringComparer.CurrentCultureIgnoreCase).
                ThenBy(Function(l) l.PrenomPatient, StringComparer.CurrentCultureIgnoreCase).
                ToList()
        Next

        GestionLog.EcrireLog($"Recherche du réseau par nom : {personnes.Count} personne(s) chargée(s).",
                             GestionLog.LogLevel.Rapide, GestionLog.LogCategory.Database)

        Return personnes.Values.ToList()

    End Function

    ' Un lien (contact ou suivi) et son patient, d'après une ligne des requêtes du réseau.
    Private Function LireLienReseau(reader As MySqlDataReader, nature As NatureResultat,
                                    colonneId As String, colonneCode As String, role As String) As LienReseau
        Return New LienReseau With {
            .Nature = nature,
            .IdElement = Convert.ToInt64(reader(colonneId)),
            .CodeElement = If(LireString(reader, colonneCode), String.Empty),
            .IdPatient = Convert.ToInt64(reader("id_patient")),
            .CodePatient = If(LireString(reader, "code_patient"), String.Empty),
            .NomPatient = If(LireString(reader, "nom_patient"), String.Empty),
            .PrenomPatient = If(LireString(reader, "prenom_patient"), String.Empty),
            .PatientActif = LireBool(reader, "is_actif"),
            .Role = If(role, String.Empty).Trim()
        }
    End Function

    ' Range un lien sous sa personne (même nature, même nom et prénom), créée au besoin ; garde
    ' l'écriture rencontrée.
    Private Sub AjouterLien(personnes As Dictionary(Of String, PersonneReseau),
                            ecritures As Dictionary(Of String, List(Of (Nom As String, Prenom As String, Precision As String))),
                            nature As NaturePersonneReseau, nom As String, prenom As String, precision As String,
                            lien As LienReseau)

        Dim n As String = If(nom, String.Empty).Trim()
        Dim pr As String = If(prenom, String.Empty).Trim()
        Dim cle As String = PersonneReseau.CleDe(nature, 0L, n, pr)

        If Not personnes.ContainsKey(cle) Then
            personnes(cle) = New PersonneReseau With {.Nature = nature, .Nom = n, .Prenom = pr}
            ecritures(cle) = New List(Of (Nom As String, Prenom As String, Precision As String))
        End If

        personnes(cle).Liens.Add(lien)
        ecritures(cle).Add((n, pr, If(precision, String.Empty).Trim()))

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : FiltrerReseau
    ' Version    : V1.0.0
    ' Date       : 24/10/2026
    '
    ' Rôle       :
    ' Les personnes dont le nom répond au texte : chaque mot tapé doit être le début d'un mot du nom
    ' ou du prénom (« dup » → Dupont ; « dupont ma » → Dupont Marie), sans tenir compte des accents ni
    ' des majuscules ; texte vide : tout le réseau.
    '
    ' Paramètres :
    ' - personnes         : réseau chargé (ChargerReseau)
    ' - texte             : nom, début de nom, prénom
    ' - inclureInactifs   : True pour garder aussi les liens avec des patients désactivés
    ' - nbLiensMasques    : en sortie, liens avec des patients inactifs laissés de côté
    '
    ' Retour     :
    ' - Les personnes par nom puis prénom puis nature ; un contact ou un intervenant dont tous les
    '   patients sont inactifs n'apparaît qu'avec les inactifs ; un thérapeute apparaît toujours.
    '
    ' Remarques  :
    ' - En mémoire, sans journal : rien de ce qui est cherché n'est écrit. Suivis terminés inclus
    '   (LienReseau.EtatAffichage les signale).
    ' -------------------------------------------------------------------------------------------------
    Public Function FiltrerReseau(personnes As IEnumerable(Of PersonneReseau), texte As String, inclureInactifs As Boolean,
                                  ByRef nbLiensMasques As Integer) As List(Of PersonneReseau)

        nbLiensMasques = 0
        Dim resultat As New List(Of PersonneReseau)
        If personnes Is Nothing Then Return resultat

        Dim mots As String() = UtilsTexte.Mots(texte)

        For Each p As PersonneReseau In personnes
            If Not CorrespondNom(p, mots) Then Continue For

            Dim liens As List(Of LienReseau) = p.Liens
            If Not inclureInactifs Then
                Dim gardes As List(Of LienReseau) = liens.Where(Function(l) l.PatientActif).ToList()
                nbLiensMasques += liens.Count - gardes.Count
                If gardes.Count = 0 AndAlso liens.Count > 0 AndAlso p.Nature <> NaturePersonneReseau.Therapeute Then Continue For
                liens = gardes
            End If

            resultat.Add(p.AvecLiens(liens))
        Next

        Return resultat.
            OrderBy(Function(p) p.Nom, StringComparer.CurrentCultureIgnoreCase).
            ThenBy(Function(p) p.Prenom, StringComparer.CurrentCultureIgnoreCase).
            ThenBy(Function(p) p.Nature).
            ToList()

    End Function

    ' Chaque mot cherché est-il le début d'un mot du nom ou du prénom ?
    Private Function CorrespondNom(personne As PersonneReseau, mots As String()) As Boolean

        If mots.Length = 0 Then Return True

        Dim motsNom As String() = UtilsTexte.Mots($"{personne.Nom} {personne.Prenom}")
        Return mots.All(Function(m) motsNom.Any(Function(n) n.StartsWith(m, StringComparison.Ordinal)))

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ResultatDeLien
    ' Version    : V1.0.0
    ' Date       : 24/10/2026
    '
    ' Rôle       :
    ' Le résultat commun (ResultatRecherche) d'un lien listé, pour aller au patient par
    ' OuvertureResultat : sa fiche, onglet Famille / Contacts (contact) ou Intervenants (suivi).
    ' -------------------------------------------------------------------------------------------------
    Public Function ResultatDeLien(personne As PersonneReseau, lien As LienReseau) As ResultatRecherche

        If personne Is Nothing OrElse lien Is Nothing Then Return Nothing

        Return New ResultatRecherche With {
            .Nature = lien.Nature,
            .Code = lien.CodeElement,
            .DateElement = lien.DateDebut,
            .Description = Assembler(personne.NomComplet, lien.Role, lien.EtatAffichage),
            .IdPatient = lien.IdPatient,
            .CodePatient = lien.CodePatient,
            .NomPatient = lien.PatientAffichage
        }

    End Function

#End Region

End Module
