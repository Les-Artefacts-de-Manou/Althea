' -------------------------------------------------------------------------------------------------
' Formulaire : SeanceEditor
' Projet     : Althéa
' Version    : V1.11.0
' Date       : 25/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Form modale permettant la création et la modification d'une séance rattachée à un dossier
' (table seances), depuis l'onglet « Séances » du hub dossier (UC_DossierFiche).
'
' Responsabilités :
' - Saisir la nature de la séance (référentiel ref_natures_seance), la date, les horaires, la durée et le tarif
' - Choisir le statut de la séance (référentiel ref_statuts_seance)
' - Permettre l'ajout « en contexte » d'une nature ou d'un statut manquant via un bouton [+] (D-Q17)
' - Saisir des notes et une évolution enrichies (RTF + texte) via UC_RichTextEditor (profil Compact)
' - Résoudre le tarif selon le dossier, le domaine, le statut et la date de séance
' - Persister la séance via GestionSeances (création / mise à jour identité + notes + statut)
'
' Fonctionnement :
' - Mode Création    : le constructeur ne reçoit pas de séance existante (idSeance = 0)
' - Mode Modification : le constructeur reçoit l'identifiant de la séance à éditer
'
' Important   :
' - Retour DialogResult.OK après enregistrement réussi ; DialogResult.Cancel sinon
' - Le dossier doit être déjà enregistré (idDossier > 0)
' - Mode lecture seule possible (dossier archivé) : boutons d'enregistrement masqués
' - Aucun accès base direct : délègue à GestionSeances / GestionNaturesSeance / GestionStatutsSeance
' - La tarification et le contrôle du quota restent dans les couches métier ; la modale ne porte
'   aucune règle de montant ni de plafond.
' - V1.6.0 (19/10/2026) : modifications non enregistrées — fermer la fenêtre (Annuler, croix, Échap)
'   après une modification propose Enregistrer / Quitter sans enregistrer / Rester (GardeModifications).
' - V1.6.1 (19/10/2026) : éditeurs titrés pour « Ouvrir en grand » (« Notes de séance », « Évolution
'   de la séance »).
' - V1.6.2 (19/10/2026) : éditeur(s) de texte : textes types de leur usage (UsageTextesTypes, temps 2).
' - V1.5.4 (19/10/2026) : éditeur de texte refondu (UC_RichTextEditor V2.0.0, profil
'   Compact) ; notes et évolution reçoivent le contexte (horodatage signé).
' - V1.5.3 (18/10/2026) : listes Tarif, Nature et Statut — DisplayMember / ValueMember posés AVANT
'   DataSource. Dans l'ordre inverse, le changement de sélection levé par DataSource arrivait quand
'   SelectedValue était encore l'objet StatutSeance : la résolution du tarif échouait (« Unable to cast
'   … StatutSeance to IConvertible », journal du 18/10/2026, ouverture d'une séance) et se repliait
'   sur le montant du tarif sélectionné.
' - V1.5.2 (17/10/2026) : en modification, l'en-tête et le titre nomment la séance par son code.
' - V1.5.1 (17/10/2026) : compte rendu du report de paiement fourni par ResultatReaffectation.CompteRendu
'   (même message depuis l'onglet Séances du dossier) ; ConstruireCompteRenduReport retirée.
' - V1.5.0 (16/10/2026, UI-7) : heures en AltheaHeure (hh:mm, liste des créneaux paramétrés, durée
'   rappelée dans la liste des fins) à la place des listes Windows saisissables.
' - V1.4.0 (15/10/2026, UI-7) : fond AltheaFondFenetre, listes AltheaListe, date AltheaDate,
'   boutons du kit ([+] et cadenas en icône seule, planning du jour) ; l'état de saisie est posé
'   à un seul endroit (DefinirSaisieActive), en consultation lisible ; champs en deux cartes
'   (Prestation, Rendez-vous), comme les autres fenêtres.
'
' Dépendances :
' - GestionSeances / Seance (persistance et modèle)
' - GestionNaturesSeance / NatureSeance (combo Nature de séance)
' - GestionStatutsSeance / StatutSeance (combo Statut)
' - UC_RichTextEditor, profil Compact (éditeurs Notes / Évolution)
' - IFenetrePrincipale.OuvrirReferentielModal / UC_NaturesSeance / UC_StatutsSeance (ajout de référentiel en contexte)
' - UserControlContext / IContextAwareForm (contexte UI partagé)
' - GestionLog (journalisation)
' - V1.6.3 (20/10/2026) : codes des statuts de séance lus dans GestionSeances.CODE_STATUT_* (une seule définition ; audit B9).
' - V1.7.0 (21/10/2026, audit B.2) : l'enregistrement passe par GestionSeances.EnregistrerSeance (une
'   transaction : identité, notes, statut, dû) ; un échec ou un refus s'affiche dans une fenêtre.
' - V1.8.0 (21/10/2026, audit B.2, décision de Joëlle) : séance encaissée devenue non due → proposition
'   d'utiliser l'avoir (DialogUtilisationAvoir) au lieu de réaffecter l'encaissement.
' - V1.8.1 (22/10/2026, audit B.5) : l'utilisation de l'avoir reçoit l'identifiant de l'utilisateur.
' - V1.8.2 (22/10/2026, audit B.6) : l'alerte de dépassement du quota simule la séance à sa date
'   (période qui la contiendrait), avec le calcul de l'onglet Séances.
' - V1.8.3 (22/10/2026, audit B.6, v2.53.0) : tarif d'une absence résolu d'après la case « Absence non
'   excusée » du statut.
' - V1.8.4 (22/10/2026, demande de Joëlle) : passer d'un statut « non tenu » (annulée, maladie,
'   absence…) à un autre ne déclenche plus le contrôle de chevauchement du créneau.
' - V1.8.5 (22/10/2026, audit C.1) : l'utilisation de l'avoir reçoit le ContexteOperation.
' - V1.9.0 (22/10/2026, audit C.1, étape 2) : EnregistrerSeance reçoit le ContexteOperation (le métier
'   vérifie montant libre, séance encaissée et dossier figé ; refus de droits affiché) ; cadenas du
'   montant réservé au SuperUser (il était ouvert à tous) ; un montant hors tarif déjà enregistré est
'   conservé (plus de réalignement silencieux au changement de statut ; décision de Joëlle) ; verrou
'   « séance encaissée » lu dans le métier.
' - V1.10.0 (22/10/2026, audit C.2) : statuts proposés (GestionSeances.StatutsProposes) et
'   chevauchement du créneau (GestionSeances.ChercherChevauchement, règle « reste non tenue »
'   comprise) jugés par le métier, qui refuse aussi l'enregistrement ; ChargerStatutsProposes et
'   ResteNonTenue supprimées.
' - V1.10.1 (24/10/2026, audit C.3) : codes de statut lus dans CodesSysteme.StatutsSeance.
' - V1.11.0 (25/10/2026, audit C.4, ADR-55) : fenêtre principale reçue comme IFenetrePrincipale (et non
'   plus Home) ; boutons [+] inchangés.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class SeanceEditor
    Implements IContextAwareForm

#Region "Variables privées"

    ' Durée standard d'une séance, en minutes (paramètre lu une fois à l'ouverture, source unique
    ' de vérité côté métier) : les séances s'enchaînent sans battement.
    Private ReadOnly _dureeSeanceMinutes As Integer = GestionSeances.DureeSeanceParDefaut()

    ' Contexte UI partagé fourni par l'appelant (barre de statut, infobulles). Peut être Nothing.
    Private _context As UserControlContext

    ' Identifiant du dossier auquel la séance est rattachée (obligatoire, > 0).
    Private ReadOnly _idDossier As Long

    ' Identifiant de la séance éditée (0 en création).
    Private _idSeance As Long

    ' Séance en cours d'édition (Nothing en création tant qu'aucune n'est chargée).
    Private _seance As Seance

    ' Indique si l'on est en mode création (True) ou modification (False).
    Private ReadOnly _creation As Boolean

    ' Force la lecture seule quel que soit le mode (ex. dossier archivé, D-Q10).
    Private ReadOnly _lectureSeuleForcee As Boolean

    ' Empêche le pré-remplissage automatique du tarif pendant le chargement initial.
    Private _chargementEnCours As Boolean = False

    ' Memorise la derniere heure de debut connue, pour decaler l'heure de fin en conservant la duree.
    Private _heureDebutPrecedente As TimeSpan? = Nothing

    ' Domaine du dossier (sert à filtrer les tarifs proposés via ref_prix_seance / lia_prix_seance_domaines).
    Private _idDomaine As Long

    ' Indique si le tarif a été déverrouillé pour édition manuelle (prix d'ami / cas particulier).
    Private _tarifDeverrouille As Boolean = False

    ' Montant enregistré différent du tarif (prix d'ami fixé par un SuperUser, ou montant figé d'un
    ' tarif modifié depuis) : il est conservé tel quel, un changement de statut ne le réaligne pas
    ' (décision de Joëlle, 22/10/2026). Seul le cadenas (SuperUser) le change.
    Private _montantHorsTarifConserve As Boolean = False

    ' Raison du cadenas inactif pour un utilisateur simple.
    Private Const MONTANT_RESERVE_SUPERUSER As String =
        "Modifier le montant d'une séance est réservé au SuperUser ou à l'administrateur."

    ' Fenêtre principale, nécessaire pour ouvrir un référentiel en contexte ([+]). Peut être Nothing.
    Private ReadOnly _fenetrePrincipale As IFenetrePrincipale

    ' Indique que la séance est soldée (statut dû + paiement encaissé) et donc verrouillée en écriture.
    Private _seanceVerrouillee As Boolean = False

    ' Indique qu'un SuperUser a levé manuellement le verrou d'une séance soldée pour cette session d'édition.
    Private _verrouLeveManuellement As Boolean = False

    ' Icônes du bouton cadenas du tarif (fermé = tarif imposé, ouvert = saisie manuelle), chargées une fois.
    Private _iconeCadenasFerme As Image
    Private _iconeCadenasOuvert As Image

#End Region

#Region "Propriétés privées"

    ' Date saisie de la séance. AltheaDate obligatoire (non facultative) : Valeur n'est jamais vide.
    Private ReadOnly Property DateSeanceSaisie As Date
        Get
            Return dtpDateSeance.Valeur.Value.Date
        End Get
    End Property

#End Region

#Region "Constructeur"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version       : V1.11.0
    ' Date          : 25/10/2026
    '
    ' Rôle          :
    ' Initialise la modale pour la création ou la modification d'une séance.
    '
    ' Paramètres    :
    ' - idDossier          : Identifiant du dossier (obligatoire, > 0)
    ' - idSeance           : Identifiant de la séance à éditer, ou 0 pour une création
    ' - lectureSeuleForcee : Force la lecture seule (dossier archivé), optionnel
    ' - fenetrePrincipale : Fenêtre principale, requise pour activer les boutons [+], optionnel
    ' - V1.11.0 (25/10/2026, audit C.4, ADR-55) : reçoit la fenêtre principale (IFenetrePrincipale)
    '   au lieu de Home.
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(idDossier As Long,
                   Optional idSeance As Long = 0,
                   Optional lectureSeuleForcee As Boolean = False,
                   Optional fenetrePrincipale As IFenetrePrincipale = Nothing)

        InitializeComponent()

        ' Fond du kit : beige clair et petite feuille dessinée.
        AltheaFondFenetre.Appliquer(Me)

        _idDossier = idDossier
        _idSeance = idSeance
        _creation = idSeance <= 0
        _lectureSeuleForcee = lectureSeuleForcee
        _fenetrePrincipale = fenetrePrincipale

    End Sub

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SetContext
    ' Version   : V1.0.0
    ' Date      : 14/09/2026
    '
    ' Rôle      :
    ' Reçoit le contexte UI partagé fourni par l'appelant. Doit être appelée AVANT ShowDialog.
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(context As UserControlContext) Implements IContextAwareForm.SetContext

        _context = context
        rteNotes.SetContext(context)
        rteEvolution.SetContext(context)

    End Sub

#End Region

#Region "Cycle de vie"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SeanceEditor_Load
    ' Version   : V1.3.0
    ' Date      : 19/10/2026
    '
    ' Rôle      :
    ' Initialise les combos référentiels, applique le titre, le mode lecture seule éventuel et
    ' charge la séance (modification) ou les valeurs par défaut (création).
    ' - V1.2.0 (19/10/2026) : garde des modifications non enregistrées (GardeModifications.SurveillerFenetre).
    ' - V1.3.0 (19/10/2026) : textes types proposés dans l'éditeur (UsageTextesTypes).
    ' -------------------------------------------------------------------------------------------------
    Private Sub SeanceEditor_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        GardeModifications.SurveillerFenetre(Me, btnEnregistrer, "de la séance")

        ' Textes types proposés dans l'éditeur (temps 2 de l'éditeur, UsagesTexteType).
        rteNotes.UsageTextesTypes = UsagesTexteType.NotesSeance
        rteEvolution.UsageTextesTypes = UsagesTexteType.EvolutionSeance

        _chargementEnCours = True

        InitialiserBoutonsReferentiel()
        InitialiserCombos()

        If _creation Then
            Me.Text = "Nouvelle séance"
            enTete.SousTitre = "Nouvelle séance"
            AppliquerDefautsCreation()
        Else
            Me.Text = "Modifier la séance"
            enTete.SousTitre = "Modifier la séance"
            ChargerSeance()
            ' La fenêtre nomme la séance par son code : les dates seules ne suffisent pas à la distinguer.
            If _seance IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(_seance.CodeSeance) Then
                Me.Text = $"Modifier la séance {_seance.CodeSeance}"
                enTete.SousTitre = $"Modifier la séance {_seance.CodeSeance}"
            End If
        End If

        AppliquerLectureSeule()

        MemoriserMontantHorsTarif()

        EvaluerVerrouSeanceSoldee()

        AppliquerVerrouSeanceReportee()

        _chargementEnCours = False

    End Sub

#End Region

#Region "Initialisation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserBoutonsReferentiel
    ' Version   : V1.1.0
    ' Date      : 15/10/2026
    '
    ' Rôle      :
    ' Applique l'habillage standard aux boutons [+] d'ajout de référentiel en contexte et les masque
    ' lorsque la fenêtre principale n'est pas disponible (ouverture hors navigation principale).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserBoutonsReferentiel()

        ' Boutons [+] et cadenas : AltheaBouton Outil à icône seule. Pas d'UtilsButtons pour eux :
        ' ses icônes de survol, claires, étaient prévues pour l'ancien survol sauge et disparaîtraient
        ' sur le survol vert pâle du kit ; l'icône du Designer reste, atténuée quand le bouton est inactif.
        _iconeCadenasFerme = UtilsButtons.LoadImageFromAsset(UITheme.AssetsStandardButtonDiversPath, "cadenas_24_normal")
        _iconeCadenasOuvert = UtilsButtons.LoadImageFromAsset(UITheme.AssetsStandardButtonDiversPath, "cadenas_ouvert_24_normal")
        AfficherEtatCadenas()

        UtilsButtons.InitStandardButton(btnVoirPlanningJour)

        btnAjouterPrixSeance.Visible = _fenetrePrincipale IsNot Nothing
        btnAjouterNatureSeance.Visible = _fenetrePrincipale IsNot Nothing
        btnAjouterStatutSeance.Visible = _fenetrePrincipale IsNot Nothing

        If _context IsNot Nothing Then
            _context.SetToolTip(btnAjouterPrixSeance, "Ajouter un tarif de séance manquant au référentiel.")
            _context.SetToolTip(btnAjouterNatureSeance, "Ajouter une nature de séance manquante au référentiel.")
            _context.SetToolTip(btnAjouterStatutSeance, "Ajouter un statut de séance manquant au référentiel.")
            _context.SetToolTip(btnDeverrouillerTarif, "Déverrouiller le tarif pour le modifier manuellement.")
            _context.SetToolTip(btnVoirPlanningJour, "Afficher le planning du jour pour repérer les créneaux disponibles.")
            _context.SetToolTip(btnEnregistrer, "Enregistrer la séance.")
            _context.SetToolTip(btnFermer, "Fermer la fenêtre sans enregistrer.")
            _context.SetToolTip(btnDeverrouillerSeance, "Déverrouiller la séance pour la modifier (réservé SuperUser).")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserCombos
    ' Version   : V2.0.1
    ' Date      : 18/10/2026
    '
    ' Rôle      :
    ' Alimente les combos Tarif (filtré selon le domaine du dossier), Nature et Statut depuis les
    ' référentiels actifs. Le domaine du dossier est résolu via GestionDossiers.
    '
    ' Remarques :
    ' - DisplayMember et ValueMember TOUJOURS avant DataSource (ici et à chaque rechargement) : poser
    '   DataSource déclenche SelectedIndexChanged, et le Statut recalcule alors le tarif en lisant
    '   SelectedValue, qui doit déjà être l'identifiant (V1.5.3).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserCombos()

        _idDomaine = ResoudreIdDomaine()

        Dim prix As List(Of PrixSeance) = ChargerTarifsProposes()
        cboPrixSeance.DisplayMember = NameOf(PrixSeance.LibellePrixSeance)
        cboPrixSeance.ValueMember = NameOf(PrixSeance.IdPrixSeance)
        cboPrixSeance.DataSource = prix

        Dim natures As List(Of NatureSeance) = ChargerNaturesProposees()
        cboNatureSeance.DisplayMember = NameOf(NatureSeance.LibelleNatureSeance)
        cboNatureSeance.ValueMember = NameOf(NatureSeance.IdNatureSeance)
        cboNatureSeance.DataSource = natures

        Dim statuts As List(Of StatutSeance) = StatutsProposes()
        cboStatutSeance.DisplayMember = NameOf(StatutSeance.LibelleStatutSeance)
        cboStatutSeance.ValueMember = NameOf(StatutSeance.IdStatutSeance)
        cboStatutSeance.DataSource = statuts

        ' La durée est calculée automatiquement depuis les heures début/fin : saisie manuelle désactivée.
        numDureeMinutes.ReadOnly = True

        RemplirHeuresProposees()

        ' En création, force les combos à être vides (SelectedIndex = -1) pour que l'utilisateur fasse un choix conscient.
        If _creation Then
            cboPrixSeance.SelectedIndex = -1
            cboNatureSeance.SelectedIndex = -1
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplirHeuresProposees
    ' Version   : V2.0.0
    ' Date      : 16/10/2026
    '
    ' Rôle      :
    ' Donne aux champs d'heure (AltheaHeure) les créneaux de la grille paramétrée
    ' (GestionSeances.CreneauxProposes : plage et pas, cohérents avec la durée par défaut) et le pas
    ' des flèches Haut / Bas. Une heure libre reste saisissable au clavier.
    '
    ' Remarque :
    ' Le pas est plus fin que la durée d'une séance : les journées ne commencent pas toutes à la même
    ' heure (8:00, 8:15...) et l'enchaînement des séances produit des heures intermédiaires
    ' (9:00, 9:45, 10:30...) qui doivent toutes rester proposées.
    ' V2.0.0 (UI-7) : les listes Windows saisissables (format HH:mm:ss) sont remplacées par le champ
    ' d'heure du kit (hh:mm).
    ' -------------------------------------------------------------------------------------------------
    Private Sub RemplirHeuresProposees()

        Dim creneaux As List(Of TimeSpan) = GestionSeances.CreneauxProposes()

        ' Pas réellement appliqué par la grille (il peut avoir été réduit pour rester cohérent avec
        ' la durée par défaut) : écart entre deux créneaux.
        If creneaux.Count >= 2 Then
            Dim pas As Integer = CInt((creneaux(1) - creneaux(0)).TotalMinutes)
            hreDebut.Pas = pas
            hreFin.Pas = pas
        End If

        hreDebut.HeuresProposees = creneaux
        hreFin.HeuresProposees = creneaux

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ResoudreIdDomaine
    ' Version  : V1.0.0
    ' Date     : 15/09/2026
    '
    ' Rôle     :
    ' Retourne l'identifiant du domaine du dossier courant (utilisé pour filtrer les tarifs).
    ' Retourne 0 si le dossier est introuvable.
    ' -------------------------------------------------------------------------------------------------
    Private Function ResoudreIdDomaine() As Long

        If _idDossier <= 0 Then
            Return 0
        End If

        Try
            Dim dossier As Dossier = GestionDossiers.GetDossier(_idDossier)
            If dossier IsNot Nothing Then
                Return dossier.IdDomaine
            End If
        Catch ex As Exception
            GestionLog.EcrireLog(
                $"Erreur résolution domaine du dossier (SeanceEditor, idDossier={_idDossier}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )
        End Try

        Return 0

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ChargerTarifsProposes
    ' Version  : V3.0.0
    ' Date     : 19/09/2026
    '
    ' Rôle     :
    ' Retourne les tarifs proposés pour la séance : ceux rattachés au domaine du dossier et valides
    ' à la date saisie (lia_prix_seance_domaines).
    '
    ' Remarques :
    ' - Le rattachement au domaine s'applique quel que soit le mode de facturation : un tarif
    '   conventionné n'a aucun sens dans un domaine qui ne relève pas de cette convention. C'est
    '   tarif_convention qui décide du MONTANT appliqué, pas de la liste proposée.
    ' - La liaison étant historisée, la liste dépend de la date de la séance : elle est donc
    '   rechargée à chaque changement de date (voir dtpDateSeance_ValeurChangee).
    ' - Un domaine sans aucun tarif rattaché relève d'un référentiel incomplet : la liste complète
    '   est alors retournée pour ne pas bloquer la saisie, et l'anomalie est journalisée.
    ' -------------------------------------------------------------------------------------------------
    Private Function ChargerTarifsProposes() As List(Of PrixSeance)

        If _idDomaine > 0 Then

            Dim parDomaine As List(Of PrixSeance) =
                GestionPrixSeance.GetPrixSeanceParDomaine(_idDomaine, DateSeanceSaisie)

            If parDomaine IsNot Nothing AndAlso parDomaine.Count > 0 Then
                Return parDomaine
            End If

            GestionLog.EcrireLog(
                $"Aucun tarif rattache au domaine {_idDomaine} au {DateSeanceSaisie:yyyy-MM-dd} : repli sur la liste complete.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI
            )

        End If

        Return GestionPrixSeance.GetPrixSeanceActifs()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ChargerNaturesProposees
    ' Version  : V1.0.0
    ' Date     : 19/09/2026
    '
    ' Rôle     :
    ' Retourne les natures de séance proposées à l'utilisateur, selon le domaine du dossier.
    '
    ' Retour   :
    ' - List(Of NatureSeance) : natures rattachées au domaine (lia_natures_seance_domaines)
    '
    ' Remarques :
    ' - Le rattachement est porté par le référentiel (lot 14) et non plus déduit d'un code de
    '   prestation non vide : deux domaines conventionnés aux nomenclatures différentes sont
    '   ainsi distingués, et l'ajout d'un domaine ne demande aucune modification de code.
    ' - Repli sûr : si aucune nature n'est rattachée au domaine, la liste complète est retournée
    '   afin de ne jamais bloquer la saisie (référentiel incomplet), et l'anomalie est journalisée.
    ' -------------------------------------------------------------------------------------------------
    Private Function ChargerNaturesProposees() As List(Of NatureSeance)

        Dim toutes As List(Of NatureSeance) = GestionNaturesSeance.GetNaturesSeanceActives()

        If toutes Is Nothing OrElse toutes.Count = 0 Then
            Return New List(Of NatureSeance)
        End If

        Dim idDomaine As Long = LireIdDomaineDossier()

        If idDomaine <= 0 Then
            Return toutes
        End If

        Dim filtrees As List(Of NatureSeance) = GestionNaturesSeance.GetNaturesSeanceParDomaine(idDomaine)

        If filtrees Is Nothing OrElse filtrees.Count = 0 Then

            GestionLog.EcrireLog(
                $"Aucune nature de seance rattachee au domaine {idDomaine} : repli sur la liste complete.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI
            )

            Return toutes

        End If

        Return filtrees

    End Function

    ' Statuts proposés dans la liste (règle du métier, GestionSeances.StatutsProposes : création
    ' planifiée ou réalisée, « Reportée » réservé au bouton Reporter).
    Private Function StatutsProposes() As List(Of StatutSeance)
        Return GestionSeances.StatutsProposes(_creation, _seance?.CodeStatutSeance)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LireIdDomaineDossier
    ' Version  : V1.0.0
    ' Date     : 19/09/2026
    '
    ' Rôle     :
    ' Retourne l'identifiant du domaine du dossier de la séance.
    '
    ' Retour   :
    ' - Long : identifiant du domaine, ou 0 si inconnu
    '
    ' Remarques :
    ' - Remplace DossierEstInami : c'est le domaine, et non un drapeau, qui porte désormais les
    '   règles de sélection des natures.
    ' - En cas d'échec de lecture, retourne 0 : l'appelant propose alors toutes les natures plutôt
    '   que de bloquer la saisie.
    ' -------------------------------------------------------------------------------------------------
    Private Function LireIdDomaineDossier() As Long

        If _idDossier <= 0 Then Return 0L

        Try

            Dim ctx As GestionTarification.ContexteTarifaireDossier =
                GestionTarification.GetContexteTarifaireDossier(_idDossier)

            If ctx Is Nothing Then Return 0L

            Return ctx.IdDomaine

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur LireIdDomaineDossier (SeanceEditor).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            Return 0L

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerDefautsCreation
    ' Version   : V2.0.0
    ' Date      : 15/09/2026
    '
    ' Rôle      :
    ' Applique les valeurs par défaut d'une nouvelle séance : date du jour, tarif du tarif sélectionné,
    ' statut « Planifiée » présélectionné, nature non présélectionnée (choix obligatoire), notes vides.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerDefautsCreation()

        dtpDateSeance.Valeur = Date.Today.AddDays(1)
        hreDebut.Valeur = Nothing
        hreFin.Valeur = Nothing
        hreFin.HeureReference = Nothing
        RecalculerDureeAutomatique()

        cboNatureSeance.SelectedIndex = -1

        SelectionnerStatutParCode(CodesSysteme.StatutsSeance.Planifiee)
        ChargerTarifDefautPatient()
        AppliquerTarifDepuisSelection()

        rteNotes.ChargerContenu(String.Empty, String.Empty)
        rteEvolution.ChargerContenu(String.Empty, String.Empty)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerSeance
    ' Version   : V1.0.0
    ' Date      : 14/09/2026
    '
    ' Rôle      :
    ' Charge une séance existante et renseigne les champs de la modale.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerSeance()

        Try

            _seance = GestionSeances.GetSeance(_idSeance)

            If _seance Is Nothing Then
                If _context IsNot Nothing Then
                    _context.SetStatus("Séance introuvable.")
                End If
                Me.DialogResult = DialogResult.Cancel
                Me.Close()
                Return
            End If

            cboPrixSeance.SelectedValue = Convert.ToUInt64(_seance.IdPrixSeance)
            cboNatureSeance.SelectedValue = Convert.ToUInt64(_seance.IdNatureSeance)

            ' Recharger le combo Statut maintenant que la séance est connue : en modification le report
            ' est masqué, mais le statut courant (ex. REPORTEE) doit rester présent pour l'affichage.
            cboStatutSeance.DisplayMember = NameOf(StatutSeance.LibelleStatutSeance)
            cboStatutSeance.ValueMember = NameOf(StatutSeance.IdStatutSeance)
            cboStatutSeance.DataSource = StatutsProposes()
            cboStatutSeance.SelectedValue = Convert.ToUInt64(_seance.IdStatutSeance)
            dtpDateSeance.Valeur = _seance.DateSeance

            hreDebut.Valeur = _seance.HeureDebut
            hreFin.Valeur = _seance.HeureFin
            hreFin.HeureReference = hreDebut.Valeur
            _heureDebutPrecedente = hreDebut.Valeur
            RecalculerDureeAutomatique()
            numTarifSeance.Value = _seance.TarifSeance

            rteNotes.ChargerContenu(_seance.NotesSeanceRtf, _seance.NotesSeanceTxt)
            rteEvolution.ChargerContenu(_seance.EvolutionRtf, _seance.EvolutionTxt)

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur chargement séance (SeanceEditor, id={_idSeance}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors du chargement de la séance.")
            End If

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerLectureSeule
    ' Version   : V1.1.0
    ' Date      : 15/10/2026
    '
    ' Rôle      :
    ' Applique le mode lecture seule (dossier archivé) : désactive la saisie et masque Enregistrer.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerLectureSeule()

        If Not _lectureSeuleForcee Then
            Return
        End If

        DefinirSaisieActive(False)
        btnEnregistrer.Visible = False

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerVerrouSeanceReportee
    ' Version   : V1.1.0
    ' Date      : 15/10/2026
    '
    ' Rôle      :
    ' Verrouille en lecture seule une séance dont le statut est REPORTEE : reportée, elle n'a plus
    ' vocation à être modifiée (son créneau est libéré et son éventuel paiement a été traité par le
    ' report). Toute correction doit passer par la séance de remplacement, jamais par l'originale.
    '
    ' Remarques :
    ' - Ne s'applique qu'en modification d'une séance persistée.
    ' - Sans possibilité de déverrouillage : contrairement à une séance soldée, un report est définitif.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerVerrouSeanceReportee()

        If _creation OrElse _seance Is Nothing Then
            Return
        End If

        If Not String.Equals(_seance.CodeStatutSeance, CodesSysteme.StatutsSeance.Reportee, StringComparison.OrdinalIgnoreCase) Then
            Return
        End If

        DefinirSaisieActive(False)
        btnDeverrouillerSeance.Visible = False
        btnEnregistrer.Visible = False

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : EvaluerVerrouSeanceSoldee
    ' Version   : V2.0.0
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' Détermine si la séance éditée est « soldée » — c'est-à-dire réalisée (ou tout autre statut dû)
    ' ET intégralement encaissée — et, le cas échéant, verrouille la saisie. Un tel enregistrement est
    ' comptablement clos : le modifier fausserait l'historique des paiements du patient.
    '
    ' Remarques :
    ' - Ne s'applique jamais en création (aucun paiement encaissé possible).
    ' - Le verrou peut être levé manuellement par un utilisateur SuperUser ou supérieur.
    ' - Ne remplace pas la lecture seule liée au dossier archivé, qui reste prioritaire.
    ' - V2.0.0 (22/10/2026, audit C.1) : règle lue dans le métier (GestionSeances.SeanceEstVerrouillee),
    '   qui la vérifie aussi à l'enregistrement ; StatutCourantEstRealisee supprimée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub EvaluerVerrouSeanceSoldee()

        _seanceVerrouillee = False

        If _creation OrElse _idSeance <= 0 OrElse _lectureSeuleForcee Then
            AppliquerVerrouSeance()
            Return
        End If

        Try

            ' Réalisée et encaissée : une séance encore planifiée (même passée) reste modifiable.
            _seanceVerrouillee = GestionSeances.SeanceEstVerrouillee(_seance)

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur EvaluerVerrouSeanceSoldee (séance ID={_idSeance}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

        AppliquerVerrouSeance()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerVerrouSeance
    ' Version   : V1.1.0
    ' Date      : 15/10/2026
    '
    ' Rôle      :
    ' Reflète l'état du verrou « séance soldée » dans l'interface : désactive la saisie, masque le
    ' bouton Enregistrer et propose le déverrouillage manuel aux seuls utilisateurs SuperUser.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerVerrouSeance()

        Dim verrouActif As Boolean = _seanceVerrouillee AndAlso Not _verrouLeveManuellement

        ' Le bouton n'a de sens que pour une séance soldée non encore déverrouillée, et seulement
        ' si l'utilisateur dispose du niveau d'habilitation requis.
        btnDeverrouillerSeance.Visible = verrouActif AndAlso
                                         Not _lectureSeuleForcee AndAlso
                                         _context IsNot Nothing AndAlso
                                         _context.HasRole(AppRole.SuperUser)

        If _lectureSeuleForcee Then
            Return
        End If

        Dim saisieActive As Boolean = Not verrouActif

        DefinirSaisieActive(saisieActive)
        btnEnregistrer.Visible = saisieActive

        If verrouActif AndAlso _context IsNot Nothing Then
            _context.SetStatus("Séance réalisée et encaissée : modification verrouillée (déverrouillage SuperUser requis).")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DefinirSaisieActive
    ' Version   : V1.1.0
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' Seul endroit qui ouvre ou ferme la saisie de la séance (lecture seule du dossier archivé,
    ' séance reportée, séance soldée et son déverrouillage), selon la règle « Consultation lisible ».
    '
    ' Paramètres :
    ' - saisieActive : True = modification possible, False = consultation
    '
    ' Remarques :
    ' - Listes, date et heures du kit : Enabled ; désactivées, elles restent lisibles.
    ' - Durée : toujours en lecture seule (calculée depuis les heures) ; ses flèches sont donc
    '   désactivées elles aussi (elles restaient actives malgré ReadOnly).
    ' - Tarif : modifiable seulement en saisie ET cadenas ouvert.
    ' - V1.1.0 (22/10/2026, audit C.1) : cadenas réservé au SuperUser / Admin (il était ouvert à
    '   tous), raison au survol.
    ' -------------------------------------------------------------------------------------------------
    Private Sub DefinirSaisieActive(saisieActive As Boolean)

        cboPrixSeance.Enabled = saisieActive
        cboNatureSeance.Enabled = saisieActive
        cboStatutSeance.Enabled = saisieActive
        dtpDateSeance.Enabled = saisieActive
        hreDebut.Enabled = saisieActive
        hreFin.Enabled = saisieActive

        btnAjouterPrixSeance.Enabled = saisieActive
        btnAjouterNatureSeance.Enabled = saisieActive
        btnAjouterStatutSeance.Enabled = saisieActive
        btnDeverrouillerTarif.DefinirDisponibilite(saisieActive AndAlso PeutModifierMontant(),
                                                   If(saisieActive, MONTANT_RESERVE_SUPERUSER, String.Empty))

        UtilsControls.DefinirChampLectureSeule(numDureeMinutes, True)
        UtilsControls.DefinirChampLectureSeule(numTarifSeance, Not (saisieActive AndAlso _tarifDeverrouille))

        rteNotes.ReadOnlyMode = Not saisieActive
        rteEvolution.ReadOnlyMode = Not saisieActive

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherEtatCadenas
    ' Version   : V1.0.0
    ' Date      : 15/10/2026
    '
    ' Rôle      :
    ' Icône du bouton cadenas selon l'état du tarif : fermé (tarif imposé) ou ouvert (saisie manuelle).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherEtatCadenas()

        btnDeverrouillerTarif.Image = If(_tarifDeverrouille, _iconeCadenasOuvert, _iconeCadenasFerme)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SeanceEditor_FormClosed
    ' Rôle      : Libère les icônes du cadenas chargées par le code.
    ' -------------------------------------------------------------------------------------------------
    Private Sub SeanceEditor_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed

        btnDeverrouillerTarif.Image = Nothing
        _iconeCadenasFerme?.Dispose()
        _iconeCadenasOuvert?.Dispose()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnDeverrouillerSeance_Click
    ' Version   : V1.0.0
    ' Date      : 17/09/2026
    '
    ' Rôle      :
    ' Lève manuellement, après confirmation explicite, le verrou d'une séance soldée. L'action est
    ' réservée aux SuperUser et tracée dans le journal applicatif pour audit.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnDeverrouillerSeance_Click(sender As Object, e As EventArgs) Handles btnDeverrouillerSeance.Click

        If _context Is Nothing OrElse Not _context.HasRole(AppRole.SuperUser) Then
            DialogChoix.Avertissement(
                "Le déverrouillage d'une séance encaissée est réservé aux utilisateurs SuperUser.",
                "Autorisation insuffisante")
            Return
        End If

        Dim message As String =
            "Cette séance est réalisée et son paiement a été encaissé." & Environment.NewLine &
            Environment.NewLine &
            "Modifier un enregistrement soldé peut désynchroniser la comptabilité du patient." & Environment.NewLine &
            "Confirmez-vous le déverrouillage ?"

        If DialogChoix.Confirmer(message, "Déverrouillage SuperUser") <> DialogResult.Yes Then
            Return
        End If

        _verrouLeveManuellement = True

        GestionLog.EcrireLog(
            $"Déverrouillage SuperUser d'une séance soldée (ID={_idSeance}).",
            GestionLog.LogLevel.Succinct,
            GestionLog.LogCategory.UI
        )

        AppliquerVerrouSeance()

        If _context IsNot Nothing Then
            _context.SetStatus("Séance déverrouillée : les modifications sont autorisées pour cette session.")
        End If

    End Sub

#End Region

#Region "Aides de sélection"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerTarifDefautPatient
    ' Version   : V1.0.0
    ' Date      : 16/01/2025
    '
    ' Rôle      :
    ' Sélectionne le tarif par défaut du patient au moment de la création d'une nouvelle séance,
    ' si le patient possède un tarif défaut dans son profil (IdPrixSeanceDefaut).
    ' Si aucun tarif défaut n'existe ou en cas d'erreur, la sélection reste vide et l'utilisateur
    ' doit choisir manuellement.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerTarifDefautPatient()

        Try
            ' Récupérer le dossier et en extraire l'ID patient
            If _idDossier <= 0 Then
                Return
            End If

            Dim dossier As Dossier = GestionDossiers.GetDossier(_idDossier)
            If dossier Is Nothing Then
                Return
            End If

            ' Récupérer le patient et son tarif par défaut
            Dim patient As Patient = GestionPatients.GetPatientById(dossier.IdPatient)
            If patient Is Nothing OrElse patient.IdPrixSeanceDefaut = 0 Then
                Return
            End If

            ' Sélectionner le tarif dans le combobox
            cboPrixSeance.SelectedValue = Convert.ToUInt64(patient.IdPrixSeanceDefaut)

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur chargement tarif défaut patient en création de séance (SeanceEditor, idDossier={_idDossier}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerTarifDepuisSelection
    ' Version   : V2.0.0
    ' Date      : 15/09/2026
    '
    ' Rôle      :
    ' Renseigne le tarif proposé pour la séance à partir du tarif (ref_prix_seance) sélectionné, puis
    ' l'ajuste selon le cahier des charges Pearl (D-TAR-1/2/3) via GestionTarification : circuit INAMI
    ' (Realism) et statut « dû ». Le montant du tarif sélectionné sert de base et de repli sûr.
    '
    ' Remarques :
    ' - Ne s'applique pas lorsque le tarif a été déverrouillé manuellement (prix d'ami / cas particulier),
    '   ni à un montant hors tarif déjà enregistré (_montantHorsTarifConserve, audit C.1).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerTarifDepuisSelection()

        If _tarifDeverrouille OrElse _montantHorsTarifConserve Then
            Return
        End If

        Dim tarif As Decimal? = CalculerTarifAttendu()

        If tarif.HasValue Then
            numTarifSeance.Value = tarif.Value
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CalculerTarifAttendu
    ' Version  : V1.0.1
    ' Date     : 22/10/2026
    '
    ' Rôle     :
    ' Montant attendu pour la séance selon le tarif sélectionné, ajusté par la résolution métier
    ' (INAMI / statut), borné aux limites du champ. Nothing si aucun tarif n'est sélectionné.
    ' - V1.0.1 (22/10/2026, audit B.6) : la résolution reconnaît l'absence par le statut (plus par son code).
    ' -------------------------------------------------------------------------------------------------
    Private Function CalculerTarifAttendu() As Decimal?

        Dim prix As PrixSeance = TryCast(cboPrixSeance.SelectedItem, PrixSeance)

        If prix Is Nothing Then
            Return Nothing
        End If

        Dim tarifBase As Decimal = prix.Montant
        Dim tarif As Decimal = tarifBase

        ' Résolution métier (INAMI / statut) lorsque le dossier et le statut sont connus.
        If _idDossier > 0 AndAlso cboStatutSeance.SelectedValue IsNot Nothing Then

            Try

                Dim idStatut As Long = LireIdCombo(cboStatutSeance)

                tarif = GestionTarification.ResoudreTarifSeance(
                    _idDossier, idStatut, tarifBase, DateSeanceSaisie)

            Catch ex As Exception

                GestionLog.EcrireLog(
                    "Erreur résolution tarif séance (SeanceEditor) — repli sur le montant du tarif sélectionné.",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.UI,
                    ex
                )

                tarif = tarifBase

            End Try

        End If

        If tarif < numTarifSeance.Minimum Then
            tarif = numTarifSeance.Minimum
        ElseIf tarif > numTarifSeance.Maximum Then
            tarif = numTarifSeance.Maximum
        End If

        Return tarif

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SelectionnerStatutParCode
    ' Version   : V1.0.0
    ' Date      : 14/09/2026
    '
    ' Rôle      :
    ' Présélectionne dans le combo Statut l'entrée dont le code technique correspond.
    '
    ' Paramètres :
    ' - codeStatut : Code recherché (ex. 'PLANIFIEE')
    ' -------------------------------------------------------------------------------------------------
    Private Sub SelectionnerStatutParCode(codeStatut As String)

        Dim statuts As List(Of StatutSeance) = TryCast(cboStatutSeance.DataSource, List(Of StatutSeance))

        If statuts Is Nothing Then
            Return
        End If

        Dim cible As StatutSeance = statuts.
            FirstOrDefault(Function(s) String.Equals(s.CodeStatutSeance, codeStatut, StringComparison.OrdinalIgnoreCase))

        If cible IsNot Nothing Then
            cboStatutSeance.SelectedValue = cible.IdStatutSeance
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : cboPrixSeance_SelectedIndexChanged
    ' Version    : V1.0.0
    ' Date       : 15/09/2026
    '
    ' Rôle       :
    ' En création, réaligne le tarif proposé sur le montant du tarif sélectionné (sauf déverrouillage).
    ' -------------------------------------------------------------------------------------------------
    Private Sub cboPrixSeance_SelectedIndexChanged(sender As Object, e As EventArgs) _
        Handles cboPrixSeance.SelectedIndexChanged

        If _chargementEnCours OrElse Not _creation Then
            Return
        End If

        AppliquerTarifDepuisSelection()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : cboStatutSeance_SelectedIndexChanged
    ' Version    : V1.1.0
    ' Date       : 15/09/2026
    '
    ' Rôle       :
    ' En création, réaligne le tarif proposé selon le statut choisi (statut non dû → 0, No-show en
    ' Realism → 0, etc.), via la résolution métier GestionTarification.
    ' -------------------------------------------------------------------------------------------------
    Private Sub cboStatutSeance_SelectedIndexChanged(sender As Object, e As EventArgs) _
        Handles cboStatutSeance.SelectedIndexChanged

        AppliquerTarifDepuisSelection()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : hreDebut_ValeurChangee
    ' Version    : V2.0.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Quand l'heure de début change, propose automatiquement une heure de fin à début + la durée
    ' standard d'une séance (paramètre, 45 minutes par défaut), uniquement si la fin est vide ou antérieure au nouveau
    ' début, pour ne pas écraser une saisie manuelle volontaire, puis recalcule la durée.
    ' V2.0.0 : champ d'heure du kit ; le début devient l'heure de référence de la liste des fins
    ' (durée affichée sur chaque ligne).
    ' -------------------------------------------------------------------------------------------------
    Private Sub hreDebut_ValeurChangee(sender As Object, e As EventArgs) _
        Handles hreDebut.ValeurChangee

        If _chargementEnCours Then
            Return
        End If

        Dim debut As TimeSpan? = hreDebut.Valeur
        hreFin.HeureReference = debut

        If debut.HasValue Then
            Dim fin As TimeSpan? = hreFin.Valeur
            If Not fin.HasValue OrElse fin.Value <= debut.Value Then
                ' Aucune fin coherente : proposer debut + la duree standard d'une seance.
                hreFin.Valeur = debut.Value.Add(New TimeSpan(0, _dureeSeanceMinutes, 0))
            ElseIf _heureDebutPrecedente.HasValue Then
                ' Decaler la fin du meme delta que le debut, pour conserver la duree existante.
                Dim delta As TimeSpan = debut.Value - _heureDebutPrecedente.Value
                If delta <> TimeSpan.Zero Then
                    Dim nouvelleFin As TimeSpan = fin.Value.Add(delta)
                    If nouvelleFin.Ticks >= 0 AndAlso nouvelleFin < New TimeSpan(24, 0, 0) Then
                        hreFin.Valeur = nouvelleFin
                    End If
                End If
            End If
        End If

        _heureDebutPrecedente = debut

        RecalculerDureeAutomatique()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : hreFin_ValeurChangee
    ' Version    : V2.0.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Recalcule automatiquement la durée en minutes dès que l'heure de fin change.
    ' -------------------------------------------------------------------------------------------------
    Private Sub hreFin_ValeurChangee(sender As Object, e As EventArgs) _
        Handles hreFin.ValeurChangee

        If _chargementEnCours Then
            Return
        End If

        RecalculerDureeAutomatique()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RecalculerDureeAutomatique
    ' Version   : V1.0.0
    ' Date      : 17/09/2026
    '
    ' Rôle      :
    ' Calcule la durée de la séance (en minutes) à partir de l'écart entre l'heure de début et
    ' l'heure de fin. Une fin antérieure ou égale au début donne une durée nulle (0). Le champ
    ' durée reste piloté par les horaires : il n'est donc pas saisi manuellement.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RecalculerDureeAutomatique()

        Dim debut As TimeSpan? = hreDebut.Valeur
        Dim fin As TimeSpan? = hreFin.Valeur

        Dim duree As Integer = 0
        If debut.HasValue AndAlso fin.HasValue Then
            Dim minutes As Double = (fin.Value - debut.Value).TotalMinutes
            If minutes > 0 Then
                duree = Convert.ToInt32(Math.Round(minutes))
            End If
        End If

        Dim valeur As Decimal = Convert.ToDecimal(duree)
        If valeur < numDureeMinutes.Minimum Then
            valeur = numDureeMinutes.Minimum
        ElseIf valeur > numDureeMinutes.Maximum Then
            valeur = numDureeMinutes.Maximum
        End If

        numDureeMinutes.Value = valeur

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnDeverrouillerTarif_Click
    ' Version    : V1.2.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Bascule le champ Tarif entre l'état verrouillé (valeur imposée par le tarif sélectionné) et
    ' l'état déverrouillé (saisie manuelle : prix d'ami / cas particulier).
    ' - V1.2.0 (22/10/2026, audit C.1) : réservé au SuperUser / Admin ; refermer le cadenas abandonne
    '   aussi un montant hors tarif déjà enregistré.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnDeverrouillerTarif_Click(sender As Object, e As EventArgs) Handles btnDeverrouillerTarif.Click

        If _lectureSeuleForcee OrElse Not PeutModifierMontant() Then
            Return
        End If

        _tarifDeverrouille = Not _tarifDeverrouille

        UtilsControls.DefinirChampLectureSeule(numTarifSeance, Not _tarifDeverrouille)
        AfficherEtatCadenas()

        If _context IsNot Nothing Then
            _context.SetToolTip(btnDeverrouillerTarif,
                If(_tarifDeverrouille,
                   "Reverrouiller le tarif (revient à la valeur du tarif sélectionné).",
                   "Déverrouiller le tarif pour le modifier manuellement."))
        End If

        ' Reverrouillage : on réaligne sur le tarif sélectionné (montant hors tarif abandonné).
        If Not _tarifDeverrouille Then
            _montantHorsTarifConserve = False
            AppliquerTarifDepuisSelection()
        Else
            numTarifSeance.Focus()
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' -------------------------------------------------------------------------------------------------
    ' Événement : btnVoirPlanningJour_Click
    ' Version   : V1.0.0
    ' Date      : 16/01/2025
    '
    ' Rôle      :
    ' Ouvre une fenêtre de consultation affichant les séances déjà planifiées pour la date
    ' actuellement saisie, afin d'aider l'utilisateur à repérer un créneau libre en l'absence
    ' (pour l'instant) d'un agenda visuel complet.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnVoirPlanningJour_Click(sender As Object, e As EventArgs) Handles btnVoirPlanningJour.Click

        Using viewer As New PlanningJourViewer(DateSeanceSaisie)
            viewer.ShowDialog(Me)
        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : btnAjouterPrixSeance_Click
    ' Version    : V1.0.0
    ' Date       : 15/09/2026
    '
    ' Rôle       :
    ' Ouvre le référentiel des tarifs de séance dans une fenêtre modale (via la fenêtre principale, qui gère droits et
    ' élévation), puis recharge le combo en sélectionnant automatiquement la valeur nouvellement créée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterPrixSeance_Click(sender As Object, e As EventArgs) Handles btnAjouterPrixSeance.Click

        If _fenetrePrincipale Is Nothing OrElse _lectureSeuleForcee Then
            Return
        End If

        ' Identifiants présents avant l'ouverture du référentiel (détection des ajouts).
        Dim idsAvant As New List(Of ULong)
        Dim listeAvant As List(Of PrixSeance) = TryCast(cboPrixSeance.DataSource, List(Of PrixSeance))

        If listeAvant IsNot Nothing Then
            For Each prix As PrixSeance In listeAvant
                idsAvant.Add(prix.IdPrixSeance)
            Next
        End If

        ' Valeur actuellement sélectionnée, à préserver si aucun ajout n'est effectué.
        Dim idSelectionCourante As Object = cboPrixSeance.SelectedValue

        If Not _fenetrePrincipale.OuvrirReferentielModal(New UC_PrixSeance(), "Tarifs de séance") Then
            Return
        End If

        Try

            Dim prixApres As List(Of PrixSeance) = ChargerTarifsProposes()

            ' Recherche d'un identifiant nouvellement créé (présent après, absent avant).
            Dim idNouveau As ULong? = Nothing
            For Each prix As PrixSeance In prixApres
                If Not idsAvant.Contains(prix.IdPrixSeance) Then
                    idNouveau = prix.IdPrixSeance
                    Exit For
                End If
            Next

            _chargementEnCours = True

            cboPrixSeance.DisplayMember = NameOf(PrixSeance.LibellePrixSeance)
            cboPrixSeance.ValueMember = NameOf(PrixSeance.IdPrixSeance)
            cboPrixSeance.DataSource = prixApres

            If idNouveau.HasValue Then
                cboPrixSeance.SelectedValue = idNouveau.Value
            ElseIf idSelectionCourante IsNot Nothing Then
                cboPrixSeance.SelectedValue = idSelectionCourante
            End If

            _chargementEnCours = False

            ' Un tarif nouvellement créé porte son propre montant : on le répercute en création.
            If idNouveau.HasValue AndAlso _creation Then
                AppliquerTarifDepuisSelection()
            End If

        Catch ex As Exception

            _chargementEnCours = False

            GestionLog.EcrireLog(
                "Erreur rechargement des tarifs de séance après ajout (SeanceEditor).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dtpDateSeance_ValeurChangee
    ' Version   : V1.1.0
    ' Date      : 15/10/2026
    '
    ' Rôle      :
    ' Recharge les tarifs proposés lorsque la date de la séance change, la liaison tarif/domaine
    ' étant historisée : un tarif valable aujourd'hui peut ne plus l'être le mois prochain.
    '
    ' Remarques :
    ' - La sélection courante est préservée si elle reste valide à la nouvelle date. Sinon elle
    '   devient impossible à conserver, et le tarif est recalculé depuis le nouveau choix.
    ' - Sans ce rechargement, la liste restait figée sur l'état d'ouverture de la fenêtre.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dtpDateSeance_ValeurChangee(sender As Object, e As EventArgs) Handles dtpDateSeance.ValeurChangee

        If _chargementEnCours Then
            Return
        End If

        Try

            Dim idSelectionCourante As Object = cboPrixSeance.SelectedValue
            Dim prix As List(Of PrixSeance) = ChargerTarifsProposes()

            _chargementEnCours = True

            cboPrixSeance.DisplayMember = NameOf(PrixSeance.LibellePrixSeance)
            cboPrixSeance.ValueMember = NameOf(PrixSeance.IdPrixSeance)
            cboPrixSeance.DataSource = prix

            Dim selectionConservee As Boolean = False

            If idSelectionCourante IsNot Nothing Then

                Dim idCourant As ULong = Convert.ToUInt64(idSelectionCourante)

                If prix.Any(Function(p) p.IdPrixSeance = idCourant) Then
                    cboPrixSeance.SelectedValue = idCourant
                    selectionConservee = True
                End If

            End If

            _chargementEnCours = False

            ' Le tarif retenu n'est plus proposé à cette date : le montant doit suivre le nouveau choix.
            If Not selectionConservee Then
                AppliquerTarifDepuisSelection()
            End If

        Catch ex As Exception

            _chargementEnCours = False

            GestionLog.EcrireLog(
                "Erreur rechargement des tarifs après changement de date (SeanceEditor).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : btnAjouterNatureSeance_Click
    ' Version    : V1.0.0
    ' Date       : 15/09/2026
    '
    ' Rôle       :
    ' Ouvre le référentiel des natures de séance dans une fenêtre modale (via la fenêtre principale, qui gère droits et
    ' élévation), puis recharge le combo en sélectionnant automatiquement la valeur nouvellement créée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterNatureSeance_Click(sender As Object, e As EventArgs) Handles btnAjouterNatureSeance.Click

        If _fenetrePrincipale Is Nothing OrElse _lectureSeuleForcee Then
            Return
        End If

        ' Identifiants présents avant l'ouverture du référentiel (détection des ajouts).
        Dim idsAvant As New List(Of ULong)
        Dim listeAvant As List(Of NatureSeance) = TryCast(cboNatureSeance.DataSource, List(Of NatureSeance))

        If listeAvant IsNot Nothing Then
            For Each nature As NatureSeance In listeAvant
                idsAvant.Add(nature.IdNatureSeance)
            Next
        End If

        ' Valeur actuellement sélectionnée, à préserver si aucun ajout n'est effectué.
        Dim idSelectionCourante As Object = cboNatureSeance.SelectedValue

        If Not _fenetrePrincipale.OuvrirReferentielModal(New UC_NaturesSeance(), "Natures de séance") Then
            Return
        End If

        Try

            Dim naturesApres As List(Of NatureSeance) = ChargerNaturesProposees()

            ' Recherche d'un identifiant nouvellement créé (présent après, absent avant).
            Dim idNouveau As ULong? = Nothing
            For Each nature As NatureSeance In naturesApres
                If Not idsAvant.Contains(nature.IdNatureSeance) Then
                    idNouveau = nature.IdNatureSeance
                    Exit For
                End If
            Next

            _chargementEnCours = True

            cboNatureSeance.DisplayMember = NameOf(NatureSeance.LibelleNatureSeance)
            cboNatureSeance.ValueMember = NameOf(NatureSeance.IdNatureSeance)
            cboNatureSeance.DataSource = naturesApres

            If idNouveau.HasValue Then
                cboNatureSeance.SelectedValue = idNouveau.Value
            ElseIf idSelectionCourante IsNot Nothing Then
                cboNatureSeance.SelectedValue = idSelectionCourante
            End If

            _chargementEnCours = False

        Catch ex As Exception

            _chargementEnCours = False

            GestionLog.EcrireLog(
                "Erreur rechargement des natures de séance après ajout (SeanceEditor).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : btnAjouterStatutSeance_Click
    ' Version    : V1.0.0
    ' Date       : 15/09/2026
    '
    ' Rôle       :
    ' Ouvre le référentiel des statuts de séance dans une fenêtre modale (via la fenêtre principale, qui gère droits et
    ' élévation), puis recharge le combo en sélectionnant automatiquement la valeur nouvellement créée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterStatutSeance_Click(sender As Object, e As EventArgs) Handles btnAjouterStatutSeance.Click

        If _fenetrePrincipale Is Nothing OrElse _lectureSeuleForcee Then
            Return
        End If

        ' Identifiants présents avant l'ouverture du référentiel (détection des ajouts).
        Dim idsAvant As New List(Of ULong)
        Dim listeAvant As List(Of StatutSeance) = TryCast(cboStatutSeance.DataSource, List(Of StatutSeance))

        If listeAvant IsNot Nothing Then
            For Each statut As StatutSeance In listeAvant
                idsAvant.Add(statut.IdStatutSeance)
            Next
        End If

        ' Valeur actuellement sélectionnée, à préserver si aucun ajout n'est effectué.
        Dim idSelectionCourante As Object = cboStatutSeance.SelectedValue

        If Not _fenetrePrincipale.OuvrirReferentielModal(New UC_StatutsSeance(), "Statuts de séance") Then
            Return
        End If

        Try

            Dim statutsApres As List(Of StatutSeance) = GestionStatutsSeance.GetStatutsSeanceActifs()

            ' Recherche d'un identifiant nouvellement créé (présent après, absent avant).
            Dim idNouveau As ULong? = Nothing
            For Each statut As StatutSeance In statutsApres
                If Not idsAvant.Contains(statut.IdStatutSeance) Then
                    idNouveau = statut.IdStatutSeance
                    Exit For
                End If
            Next

            _chargementEnCours = True

            cboStatutSeance.DisplayMember = NameOf(StatutSeance.LibelleStatutSeance)
            cboStatutSeance.ValueMember = NameOf(StatutSeance.IdStatutSeance)
            cboStatutSeance.DataSource = statutsApres

            If idNouveau.HasValue Then
                cboStatutSeance.SelectedValue = idNouveau.Value
            ElseIf idSelectionCourante IsNot Nothing Then
                cboStatutSeance.SelectedValue = idSelectionCourante
            End If

            _chargementEnCours = False

        Catch ex As Exception

            _chargementEnCours = False

            GestionLog.EcrireLog(
                "Erreur rechargement des statuts de séance après ajout (SeanceEditor).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

#End Region

#Region "Actions"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnEnregistrer_Click
    ' Version   : V1.1.0
    ' Date      : 21/10/2026
    '
    ' Rôle      :
    ' Valide la saisie, persiste la séance via la couche métier, puis ferme la modale (DialogResult.OK).
    '
    ' Remarques :
    ' - V1.1.0 (21/10/2026) : un refus métier ou un échec s'affiche dans une fenêtre (il ne restait que
    '   dans la barre d'état) ; la fenêtre reste ouverte avec la saisie. L'enregistrement étant d'un
    '   seul tenant, un échec n'a rien modifié. Les contrôles de saisie sont dans le même Try.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEnregistrer_Click(sender As Object, e As EventArgs) Handles btnEnregistrer.Click

        If _lectureSeuleForcee Then
            Return
        End If

        Try

            ' Dans le Try : les contrôles lisent la base (chevauchement, quota) et doivent, en cas
            ' d'incident, aboutir au même message que l'enregistrement.
            If Not ValiderSaisie() Then
                Return
            End If

            If _creation Then
                EnregistrerCreation()
            Else
                EnregistrerModification()
            End If

            If _context IsNot Nothing Then
                _context.SetStatus("Séance enregistrée.")
            End If

            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch exAnnule As OperationCanceledException

            ' Renoncement volontaire de l'utilisateur : la fenêtre reste ouverte, rien n'est enregistré.
            Return

        Catch exRefus As InvalidOperationException

            ' Refus métier (message rédigé pour l'utilisatrice).
            DialogChoix.Information(exRefus.Message, "Enregistrement impossible")
            _context?.SetStatus("Enregistrement de la séance refusé.")

        Catch exDroits As UnauthorizedAccessException

            ' Droits vérifiés par le métier (montant libre, séance encaissée).
            DialogChoix.Avertissement(exDroits.Message, "Autorisation insuffisante")
            _context?.SetStatus("Enregistrement de la séance refusé.")

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur enregistrement séance (SeanceEditor, idDossier={_idDossier}, idSeance={_idSeance}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur(
                "La séance n'a pas pu être enregistrée ; rien n'a été modifié." & Environment.NewLine &
                "Vérifiez la connexion à la base de données, puis cliquez à nouveau sur Enregistrer.",
                "Enregistrement impossible")
            _context?.SetStatus("Erreur lors de l'enregistrement de la séance.")

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnFermer_Click
    ' Version   : V1.0.0
    ' Date      : 14/09/2026
    '
    ' Rôle      :
    ' Ferme la modale sans enregistrer (DialogResult.Cancel).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnFermer_Click(sender As Object, e As EventArgs) Handles btnFermer.Click

        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ValiderSaisie
    ' Version  : V1.0.0
    ' Date     : 14/09/2026
    '
    ' Rôle     :
    ' Vérifie la cohérence minimale de la saisie (type et statut sélectionnés).
    '
    ' Retour   :
    ' - Boolean : True si la saisie est valide, False sinon (message d'état positionné).
    ' -------------------------------------------------------------------------------------------------
    Private Function ValiderSaisie() As Boolean

        If _seanceVerrouillee AndAlso Not _verrouLeveManuellement Then
            If _context IsNot Nothing Then
                _context.SetStatus("Séance soldée : modification refusée sans déverrouillage SuperUser.")
            End If
            Return False
        End If

        If cboPrixSeance.SelectedValue Is Nothing Then
            MessageBox.Show(
                "Veuillez sélectionner un tarif.",
                "Validation - Tarif manquant",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)
            cboPrixSeance.Focus()
            Return False
        End If

        If cboNatureSeance.SelectedValue Is Nothing Then
            MessageBox.Show(
                "Veuillez sélectionner une nature de séance.",
                "Validation - Nature manquante",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)
            cboNatureSeance.Focus()
            Return False
        End If

        If cboStatutSeance.SelectedValue Is Nothing Then
            MessageBox.Show(
                "Veuillez sélectionner un statut.",
                "Validation - Statut manquant",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)
            cboStatutSeance.Focus()
            Return False
        End If

        If Not ControlerCoherenceTarif() Then
            Return False
        End If

        If Not ConfirmerDatePassee() Then
            Return False
        End If

        If Not ValiderChevauchement() Then
            Return False
        End If

        If Not ConfirmerDepassementQuota() Then
            Return False
        End If

        Return True

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Aides du montant (audit C.1)
    ' -------------------------------------------------------------------------------------------------

    ' Le cadenas du montant est réservé au SuperUser / Admin (rôle courant, élévation comprise).
    Private Function PeutModifierMontant() As Boolean
        Return Autorisation.Possede(_context?.ContexteOperation, AppRole.SuperUser)
    End Function

    ' En modification, montant et tarif sont ceux déjà enregistrés.
    Private Function MontantEnregistreInchange() As Boolean
        Return Not _creation AndAlso _seance IsNot Nothing AndAlso
               numTarifSeance.Value = _seance.TarifSeance AndAlso
               LireIdCombo(cboPrixSeance) = _seance.IdPrixSeance
    End Function

    ' Au chargement d'une séance existante : son montant diffère-t-il du tarif attendu ?
    Private Sub MemoriserMontantHorsTarif()

        _montantHorsTarifConserve = False

        If _creation OrElse _seance Is Nothing Then
            Return
        End If

        Dim attendu As Decimal? = CalculerTarifAttendu()
        _montantHorsTarifConserve = attendu.HasValue AndAlso attendu.Value <> _seance.TarifSeance

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ControlerCoherenceTarif
    ' Version  : V1.1.0
    ' Date     : 22/10/2026
    '
    ' Rôle     :
    ' À l'enregistrement, vérifie que le montant indiqué correspond au tarif sélectionné. En cas
    ' d'écart, bloque l'enregistrement (la fenêtre reste ouverte) et explique quoi faire selon le rôle :
    ' - utilisateur simple : faire appel à un SuperUser ;
    ' - SuperUser / Admin : utiliser le cadenas (reverrouiller = montant du tarif, ou saisie manuelle).
    '
    ' Remarques :
    ' - Un montant déverrouillé volontairement (prix d'ami, cas particulier) n'est pas contrôlé.
    ' - V1.1.0 (22/10/2026, audit C.1, décision de Joëlle) : un montant enregistré laissé tel quel, sur
    '   le même tarif, est accepté (même règle que GestionSeances.EnregistrerSeance).
    ' -------------------------------------------------------------------------------------------------
    Private Function ControlerCoherenceTarif() As Boolean

        If _tarifDeverrouille OrElse MontantEnregistreInchange() Then
            Return True
        End If

        Dim attendu As Decimal? = CalculerTarifAttendu()

        If Not attendu.HasValue OrElse attendu.Value = numTarifSeance.Value Then
            Return True
        End If

        Dim estSuperUser As Boolean = _context IsNot Nothing AndAlso _context.HasRole(AppRole.SuperUser)

        Dim consigne As String =
            If(estSuperUser,
               "Cliquez sur le cadenas 🔒 à côté du montant :" & Environment.NewLine &
               "• pour saisir le montant manuellement (cadenas ouvert) ;" & Environment.NewLine &
               "• ou, en le refermant, pour appliquer automatiquement le montant du tarif.",
               "Seul un SuperUser ou un Administrateur peut modifier le montant d'une séance." & Environment.NewLine &
               "Veuillez faire appel à un SuperUser.")

        Dim message As String = String.Format(
            "Le montant indiqué ({0:N2} €) ne correspond pas au tarif sélectionné « {1} » ({2:N2} €)." &
            Environment.NewLine & Environment.NewLine & "{3}" &
            Environment.NewLine & Environment.NewLine & "La séance n'a pas été enregistrée.",
            numTarifSeance.Value, cboPrixSeance.Text, attendu.Value, consigne)

        DialogChoix.Avertissement(message, "Montant différent du tarif")

        If estSuperUser AndAlso btnDeverrouillerTarif.Enabled Then
            btnDeverrouillerTarif.Focus()
        End If

        Return False

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ConfirmerDepassementQuota
    ' Version  : V1.1.0
    ' Date     : 22/10/2026
    '
    ' Rôle     :
    ' À la création, projette le quota du dossier après ajout de la séance. Si un plafond paramétré
    ' serait dépassé, affiche un avertissement bien visible ; l'utilisateur décide de poursuivre ou non.
    '
    ' Remarques :
    ' - Non bloquant : avertir suffit, la décision reste à l'utilisateur.
    ' - Un incident de lecture du quota ne bloque jamais l'encodage (journalisé).
    ' - V1.1.0 (22/10/2026, audit B.6) : simulation à la date de la séance (GestionQuotaInami.
    '   SimulerAjoutSeance) ; elle comptait la séance dans la période d'aujourd'hui, et seule la
    '   première période était connue.
    ' -------------------------------------------------------------------------------------------------
    Private Function ConfirmerDepassementQuota() As Boolean

        If Not _creation OrElse _idDossier <= 0 Then
            Return True
        End If

        Try

            If Not dtpDateSeance.Valeur.HasValue Then
                Return True
            End If

            Dim idCategorie As ULong =
                GestionQuotaInami.GetCategorieQuotaParNature(_idDossier, LireIdCombo(cboNatureSeance))

            If idCategorie = 0UL Then
                Return True
            End If

            Dim projection As QuotaInamiInfo =
                GestionQuotaInami.SimulerAjoutSeance(_idDossier, DateSeanceSaisie, hreDebut.Valeur, idCategorie)

            If projection Is Nothing OrElse Not projection.EstDomaineInami Then
                Return True
            End If

            Dim depassements As New List(Of String)

            Dim etat As QuotaCategorieEtat =
                projection.Categories.FirstOrDefault(Function(c) c.IdCategorieQuota = idCategorie)

            If etat IsNot Nothing AndAlso etat.Depasse Then
                depassements.Add($"• {etat.LibelleCategorie} : {etat.LibelleCompteur}")
            End If

            If projection.QuotaDepasse Then
                depassements.Add($"• Total : {projection.NbTotal} / {projection.PlafondTotal}")
            End If

            If depassements.Count = 0 Then
                Return True
            End If

            Dim message As String =
                "ATTENTION : cette séance dépasse le quota remboursé du dossier." &
                Environment.NewLine & Environment.NewLine &
                String.Join(Environment.NewLine, depassements) &
                Environment.NewLine & Environment.NewLine &
                "Au-delà du plafond, la séance n'est plus couverte par la convention." &
                Environment.NewLine & Environment.NewLine &
                "Enregistrer quand même cette séance ?"

            Using dlg As New DialogChoix()
                dlg.Titre = "Quota dépassé"
                dlg.Message = message
                dlg.TypeDialogue = TypeDialogue.Warning
                dlg.SetBoutons("Enregistrer quand même", "Annuler")
                Return dlg.ShowDialog(Me) = DialogResult.Yes
            End Using

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur contrôle du quota avant enregistrement (SeanceEditor, idDossier={_idDossier}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            Return True

        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ValiderChevauchement
    ' Version  : V2.0.0
    ' Date     : 22/10/2026
    '
    ' Rôle     :
    ' Contrôle qu'aucune autre séance (tous dossiers confondus) n'occupe déjà le créneau proposé
    ' à la même date, sachant qu'il n'y a qu'un seul thérapeute et donc un seul planning global.
    ' Ne s'applique que si les deux heures (début et fin) sont renseignées ; en leur absence, la
    ' validation existante côté saisie des horaires prend le relais.
    '
    ' Retour   :
    ' - Boolean : True si aucun chevauchement n'est détecté ou si les heures sont incomplètes,
    '   False si un conflit bloquant a été trouvé (message affiché à l'utilisateur).
    '
    ' Remarques :
    ' - V1.1.0 (22/10/2026, demande de Joëlle) : une séance qui passe d'un statut non tenu à un autre
    '   (annulée → absence, maladie → absence…) n'est pas contrôlée : elle n'a pas eu lieu et ne
    '   reprend pas son créneau, qu'une autre séance a pu prendre entre-temps.
    ' - V2.0.0 (22/10/2026, audit C.2) : règle jugée par GestionSeances.ChercherChevauchement (qui
    '   refuse aussi l'enregistrement) ; la fenêtre ne garde que le message, avec le nom du patient.
    ' -------------------------------------------------------------------------------------------------
    Private Function ValiderChevauchement() As Boolean

        Dim creneau As New Seance With {
            .IdSeance = _idSeance,
            .DateSeance = DateSeanceSaisie,
            .HeureDebut = hreDebut.Valeur,
            .HeureFin = hreFin.Valeur
        }

        Dim seanceEnConflit As SeancePlanningItem = GestionSeances.ChercherChevauchement(creneau, LireCodeStatutSelectionne())

        If seanceEnConflit Is Nothing Then
            Return True
        End If

        Dim heureDebutConflit As String = If(seanceEnConflit.HeureDebut.HasValue,
            New DateTime(seanceEnConflit.HeureDebut.Value.Ticks).ToString("HH:mm"), "?")
        Dim heureFinConflit As String = If(seanceEnConflit.HeureFin.HasValue,
            New DateTime(seanceEnConflit.HeureFin.Value.Ticks).ToString("HH:mm"), "?")

        Dim message As String =
            $"Ce créneau chevauche une séance déjà planifiée :" & Environment.NewLine & Environment.NewLine &
            $"{seanceEnConflit.NomCompletPatient} — {heureDebutConflit} à {heureFinConflit} ({seanceEnConflit.LibelleStatutSeance})" &
            Environment.NewLine & Environment.NewLine &
            "Veuillez choisir un autre horaire."

        MessageBox.Show(
            message,
            "Validation - Conflit d'horaire",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning)

        hreDebut.Focus()

        Return False

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ConfirmerDatePassee
    ' Version  : V1.0.0
    ' Date     : 17/09/2026
    '
    ' Rôle     :
    ' Avertit l'utilisateur lorsque la date de séance saisie est antérieure à aujourd'hui, afin
    ' d'éviter les erreurs de frappe sur l'année ou le mois. La saisie reste possible (encodage
    ' rétroactif légitime) : l'utilisateur doit simplement confirmer.
    '
    ' Retour   :
    ' - Boolean : True si la date est valide ou si l'utilisateur confirme, False s'il renonce.
    ' -------------------------------------------------------------------------------------------------
    Private Function ConfirmerDatePassee() As Boolean

        Dim dateSaisie As Date = DateSeanceSaisie

        If dateSaisie >= Date.Today Then
            Return True
        End If

        Dim message As String =
            $"La date choisie ({dateSaisie:dd/MM/yyyy}) est antérieure à aujourd'hui." & Environment.NewLine &
            Environment.NewLine &
            "S'agit-il bien d'un encodage rétroactif ?"

        Return DialogChoix.Confirmer(message, "Date dans le passé") = DialogResult.Yes

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : EnregistrerCreation
    ' Version   : V2.0.0
    ' Date      : 21/10/2026
    '
    ' Rôle      :
    ' Crée la séance avec ses notes/évolution et le statut choisi (la création part de « Planifiée »
    ' côté métier, puis applique le statut saisi).
    '
    ' Remarques :
    ' - V2.0.0 (21/10/2026, audit B.2) : un seul appel métier (EnregistrerSeance, une transaction) au
    '   lieu de trois écritures successives.
    ' -------------------------------------------------------------------------------------------------
    Private Sub EnregistrerCreation()

        Dim nouvelle As New Seance With {
            .IdDossier = _idDossier,
            .IdPrixSeance = LireIdCombo(cboPrixSeance),
            .IdNatureSeance = LireIdCombo(cboNatureSeance),
            .DateSeance = DateSeanceSaisie,
            .HeureDebut = hreDebut.Valeur,
            .HeureFin = hreFin.Valeur,
            .DureeMinutes = LireDureeSaisie(),
            .TarifSeance = numTarifSeance.Value
        }

        RenseignerNotes(nouvelle)

        _idSeance = GestionSeances.EnregistrerSeance(_context?.ContexteOperation, nouvelle, LireCodeStatutSelectionne())

    End Sub

    ' Recopie les notes et l'évolution saisies dans la séance à enregistrer.
    Private Sub RenseignerNotes(seance As Seance)
        seance.NotesSeanceRtf = rteNotes.RtfContent
        seance.NotesSeanceTxt = rteNotes.TextContent
        seance.EvolutionRtf = rteEvolution.RtfContent
        seance.EvolutionTxt = rteEvolution.TextContent
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : EnregistrerModification
    ' Version   : V2.0.0
    ' Date      : 21/10/2026
    '
    ' Rôle      :
    ' Met à jour l'identité, les notes/évolution et le statut d'une séance existante.
    '
    ' Remarques :
    ' - V2.0.0 (21/10/2026, audit B.2) : un seul appel métier (EnregistrerSeance, une transaction) ;
    '   l'utilisation de l'avoir éventuellement créé est proposée ensuite.
    ' -------------------------------------------------------------------------------------------------
    Private Sub EnregistrerModification()

        Dim maj As New Seance With {
            .IdSeance = _idSeance,
            .IdDossier = _idDossier,
            .IdPrixSeance = LireIdCombo(cboPrixSeance),
            .IdNatureSeance = LireIdCombo(cboNatureSeance),
            .DateSeance = DateSeanceSaisie,
            .HeureDebut = hreDebut.Valeur,
            .HeureFin = hreFin.Valeur,
            .DureeMinutes = LireDureeSaisie(),
            .TarifSeance = numTarifSeance.Value
        }

        ' Tarif modifié sur une séance déjà (partiellement) encaissée : l'encaissement n'est jamais
        ' retouché, la différence devient un avoir (tarif baissé) ou un reste dû (tarif augmenté).
        Dim ancienTarif As Decimal = If(_seance IsNot Nothing, _seance.TarifSeance, maj.TarifSeance)
        Dim montantRegle As Decimal = 0D

        If ancienTarif <> maj.TarifSeance Then

            montantRegle = LireMontantRegleSeance()

            If montantRegle > 0D Then

                Dim ecart As Decimal = montantRegle - maj.TarifSeance
                Dim consequence As String =
                    If(ecart > 0D,
                       String.Format("un avoir de {0:N2} € sera constitué.", ecart),
                       If(ecart < 0D,
                          String.Format("un reste dû de {0:N2} € sera à régler.", -ecart),
                          "la séance restera exactement soldée."))

                Dim message As String = String.Format(
                    "Le tarif passe de {0:N2} € à {1:N2} € alors que {2:N2} € ont déjà été encaissés." &
                    Environment.NewLine & Environment.NewLine &
                    "L'encaissement n'est pas modifié : {3}" &
                    Environment.NewLine & Environment.NewLine &
                    "Confirmer le nouveau tarif ?",
                    ancienTarif, maj.TarifSeance, montantRegle, consequence)

                If DialogChoix.Confirmer(message, "Changement de tarif d'une séance encaissée") <> DialogResult.Yes Then
                    Throw New OperationCanceledException()
                End If

            End If

        End If

        RenseignerNotes(maj)

        Dim codeStatut As String = LireCodeStatutSelectionne()

        ' Une séance déjà encaissée qui cesse d'être due fait de l'encaissement un avoir : à évaluer
        ' AVANT l'enregistrement (après, la séance n'est plus due), pour proposer ensuite de l'utiliser.
        Dim avoirCree As Boolean =
            Not String.IsNullOrEmpty(codeStatut) AndAlso
            GestionSeances.CreeraitUnAvoir(_idSeance, codeStatut)

        GestionSeances.EnregistrerSeance(_context?.ContexteOperation, maj, codeStatut)

        If avoirCree Then
            ProposerUtilisationAvoir(codeStatut)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LireMontantRegleSeance
    ' Version  : V1.0.0
    ' Date     : 26/09/2026
    '
    ' Rôle     :
    ' Montant réellement encaissé (paiements PAYE) sur la séance en cours d'édition.
    ' -------------------------------------------------------------------------------------------------
    Private Function LireMontantRegleSeance() As Decimal

        Try
            Dim item As SeanceListeItem = GestionSeances.GetSeancesParDossier(_idDossier).
                FirstOrDefault(Function(s) s.IdSeance = _idSeance)
            Return If(item Is Nothing, 0D, item.MontantRegle)
        Catch ex As Exception
            GestionLog.EcrireLog(
                $"Erreur LireMontantRegleSeance (séance ID={_idSeance}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )
            Return 0D
        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ProposerUtilisationAvoir
    ' Version   : V2.0.1
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' Une fois enregistré un statut non dû sur une séance encaissée, propose d'utiliser l'avoir ainsi
    ' créé pour régler une séance due du dossier (DialogUtilisationAvoir). Facultatif : l'avoir peut
    ' être gardé pour plus tard.
    '
    ' Paramètres :
    ' - codeStatut : Code du statut venant d'être appliqué (repli si la liste n'a pas de libellé)
    '
    ' Remarques  :
    ' - V2.0.0 (21/10/2026, audit B.2, décision de Joëlle) : ex-ProposerReaffectationPaiement ; l'avoir
    '   est utilisé au lieu de réaffecter l'encaissement ; le déroulé est commun avec l'onglet Séances
    '   (DialogUtilisationAvoir.ProposerApresChangementStatut).
    ' - V2.0.1 (22/10/2026, audit B.5) : utilisateur transmis par son identifiant.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ProposerUtilisationAvoir(codeStatut As String)

        Dim libelleStatut As String = If(String.IsNullOrWhiteSpace(cboStatutSeance.Text), codeStatut, cboStatutSeance.Text)
        DialogUtilisationAvoir.ProposerApresChangementStatut(Me, _idDossier, _idSeance, libelleStatut, 0,
                                                             _context?.ContexteOperation)

    End Sub

#End Region

#Region "Helpers de lecture des contrôles"

    ' Retourne l'identifiant (Long) sélectionné dans un combo lié par ValueMember.
    Private Function LireIdCombo(combo As ComboBox) As Long

        If combo.SelectedValue Is Nothing Then
            Return 0
        End If

        Return Convert.ToInt64(combo.SelectedValue)

    End Function

    ' Retourne la durée saisie en minutes, ou Nothing si 0 (durée non renseignée).
    Private Function LireDureeSaisie() As Integer?

        Dim duree As Integer = Convert.ToInt32(numDureeMinutes.Value)

        If duree <= 0 Then
            Return Nothing
        End If

        Return duree

    End Function

    ' Retourne le code technique du statut sélectionné, ou Nothing si aucun.
    Private Function LireCodeStatutSelectionne() As String

        Dim statut As StatutSeance = TryCast(cboStatutSeance.SelectedItem, StatutSeance)

        If statut Is Nothing Then
            Return Nothing
        End If

        Return statut.CodeStatutSeance

    End Function



#End Region

End Class
