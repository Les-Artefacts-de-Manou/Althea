' -------------------------------------------------------------------------------------------------
' Formulaire : IntervenantEdition
' Projet     : Althéa
' Version    : V1.8.0
' Date       : 25/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Form modale permettant la création et la modification d'un intervenant du réseau de suivi
' d'un patient (table autres_suivis_patient, liaison N-N — D-Q1bis), depuis l'onglet
' « Intervenants » de la fiche patient.
'
' Responsabilités :
' - Saisir le rôle de l'intervenant (référentiel ref_roles_intervenant, optionnel)
' - Choisir le thérapeute ; recopier son identité (nom / praticien, spécialité, lieu) en trace et
'   afficher ses coordonnées actuelles (téléphone, e-mail, adresse complète), en lecture seule
' - Saisir la période de suivi (date de début, date de fin), toutes deux optionnelles
' - Saisir un commentaire enrichi (RTF + texte) via UC_RichTextEditor, profil Compact (D-Q7bis)
' - Faire valider la saisie par le métier (GestionSuivisIntervenants.ValiderSuivi) et montrer les erreurs
' - Persister le suivi via GestionSuivisIntervenants (création / mise à jour)
' - Proposer l'ajout d'un rôle manquant via le bouton [+] (référentiel en modal, auto-sélection)
'
' Fonctionnement :
' - Mode Création     : le constructeur ne reçoit pas de suivi existant
' - Mode Modification : le constructeur reçoit le suivi à éditer
'
' Remarques  :
' - Retour DialogResult.OK après enregistrement réussi
' - Le patient doit être déjà enregistré (idPatient > 0) ; garanti par l'activation progressive
' - Le thérapeute est obligatoire (référentiel therapeutes, id_therapeute)
' - Réutilise IFenetrePrincipale.OuvrirReferentielModal (droits + élévation) pour le bouton [+]
' - V1.2.0 (14/10/2026, UI-7) : cartes du kit, listes AltheaListe, dates AltheaDate facultatives,
'   consultation lisible ; champs de trace en fond teinté (toujours en lecture seule) ; fond
'   AltheaFondFenetre ; boutons [+] et loupe en AltheaBouton Outil (icône seule)
' - V1.3.0 (15/10/2026) : petits boutons du kit sans UtilsButtons (icône du Designer, survol du kit)
' - V1.3.1 (19/10/2026) : éditeur de texte refondu (UC_RichTextEditor V2.0.0, profil
'   Compact) ; le commentaire reçoit le contexte (horodatage signé).
' - V1.4.0 (19/10/2026) : modifications non enregistrées — fermer la fenêtre (Annuler, croix, Échap)
'   après une modification propose Enregistrer / Quitter sans enregistrer / Rester (GardeModifications).
' - V1.4.1 (19/10/2026) : éditeur titré pour « Ouvrir en grand » (TitreDocument = « Commentaire de
'   l'intervenant »).
' - V1.4.2 (19/10/2026) : éditeur(s) de texte : textes types de leur usage (UsageTextesTypes, temps 2).
' - V1.4.3 (19/10/2026) : textes types de l'usage « Commentaire d'un intervenant » (v2.43.0).
' - V1.5.0 (20/10/2026) : le téléphone affiché est celui, actuel, du thérapeute lié — aussi à la
'   réouverture d'un suivi, où la case restait vide (audit B12 ; décision de Joëlle : numéro actuel,
'   pas de copie figée avec le suivi).
' - V1.6.0 (23/10/2026, audit C.2 étape 2b) : saisie jugée par GestionSuivisIntervenants.ValiderSuivi
'   (règles sorties de ValiderSaisie, appliquées aussi à l'enregistrement) ; erreurs sur leurs champs et
'   dans une fenêtre récapitulative (AffichageErreursSaisie) ; refus du métier affiché. Relevé de
'   Joëlle : la fiche montre aussi l'e-mail et l'adresse complète actuels du thérapeute (case « Lieu »
'   devenue « Adresse » ; le lieu enregistré reste la localité figée).
' - V1.7.0 (24/10/2026, relevé de Joëlle) : le code du suivi (AS…) au titre de la fenêtre et dans
'   l'annonce de l'enregistrement.
' - V1.7.1 (25/10/2026, relevé aux captures) : consultation ouverte avec le focus sur « Fermer »,
'   plus de texte sélectionné dans le premier champ ; en-tête d'AppliquerMode replacé au-dessus d'elle.
' - V1.8.0 (25/10/2026, audit C.4, ADR-55) : fenêtre principale reçue comme IFenetrePrincipale (et non
'   plus Home) ; boutons [+] inchangés.
'
' Dépendances :
' - GestionSuivisIntervenants / SuiviIntervenant (persistance et modèle)
' - GestionRolesIntervenant / RoleIntervenant (combo Rôle)
' - UC_RolesIntervenant (référentiel ouvert via le bouton [+])
' - IFenetrePrincipale (ouverture du référentiel en modal avec gestion des droits)
' - GestionLog (journalisation)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class IntervenantEdition
    Implements IContextAwareForm

#Region "Variables privées"

    ' Contexte UI partagé fourni par Home (barre de statut, infobulles). Peut être Nothing si la
    ' fenêtre est ouverte sans contexte : un repli local (ttMain) garantit alors les infobulles.
    ' Remarque : errProvider reste local (les icônes d'erreur doivent se positionner sur la modale).
    Private _context As UserControlContext

    ' Identifiant du patient auquel l'intervenant est rattaché.
    Private ReadOnly _idPatient As Long

    ' Fenêtre principale (pour ouvrir le référentiel des rôles via le bouton [+]).
    Private ReadOnly _fenetrePrincipale As IFenetrePrincipale

    ' Suivi en cours d'édition (Nothing en création).
    Private _suivi As SuiviIntervenant

    ' Indique si l'on est en mode création (True) ou modification (False).
    Private ReadOnly _creation As Boolean

    ' Vrai tant que la fenêtre est ouverte en consultation (lecture seule) : passe à False
    ' dès que l'utilisateur clique sur « Modifier » pour entrer en édition.
    Private _consultation As Boolean

    ' Vrai pendant le remplissage programmatique des contrôles : neutralise l'auto-remplissage
    ' du snapshot déclenché par cboTherapeute.SelectedIndexChanged (évite d'écraser la trace).
    Private _chargementEnCours As Boolean

    ' Civilité de la trace (snapshot) : reprise du thérapeute choisi, ou du suivi enregistré.
    ' Le champ txtCivilite n'en affiche que le libellé (lecture seule, comme les autres champs de trace).
    Private _idCiviliteTrace As ULong?

    ' Lieu de la trace (snapshot) : localité du thérapeute choisi, ou lieu du suivi enregistré. Il
    ' n'est plus affiché tel quel : la case Adresse montre l'adresse actuelle complète (V1.6.0).
    Private _lieuTrace As String

    ' Valeur sentinelle du combo des rôles signifiant « aucun rôle précisé » (rôle optionnel).
    Private Const IdRoleNonPrecise As ULong = 0UL

    ' Valeur sentinelle du combo des thérapeutes signifiant « aucun thérapeute sélectionné »
    ' (le thérapeute est obligatoire : cette entrée invalide la saisie tant qu'elle est choisie).
    Private Const IdTherapeuteNonSelectionne As Long = 0L

#End Region

#Region "Constructeur"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version       : V1.8.0
    ' Date          : 25/10/2026
    '
    ' Rôle          :
    ' Initialise le formulaire pour la création, la consultation ou la modification d'un intervenant.
    '
    ' Paramètres    :
    ' - idPatient            : Identifiant du patient (obligatoire, > 0)
    ' - fenetrePrincipale : Fenêtre principale pour le bouton [+] (peut être Nothing : le [+] sera masqué)
    ' - suiviAEditer         : Suivi à consulter/modifier, ou Nothing pour une création
    ' - ouvrirEnConsultation : True pour ouvrir un suivi existant en lecture seule (consultation
    '                          d'abord) ; ignoré en création
    ' - V1.8.0 (25/10/2026, audit C.4, ADR-55) : reçoit la fenêtre principale (IFenetrePrincipale)
    '   au lieu de Home.
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(idPatient As Long, fenetrePrincipale As IFenetrePrincipale, suiviAEditer As SuiviIntervenant, Optional ouvrirEnConsultation As Boolean = False)

        InitializeComponent()

        ' Fond du kit : beige clair et petite feuille dessinée (remplace l'image étirée).
        AltheaFondFenetre.Appliquer(Me)

        _idPatient = idPatient
        _fenetrePrincipale = fenetrePrincipale
        _suivi = suiviAEditer
        _creation = suiviAEditer Is Nothing

        ' La consultation ne s'applique qu'à un suivi existant (jamais en création).
        _consultation = ouvrirEnConsultation AndAlso Not _creation

        UtilsButtons.InitStandardButton(btnModifier)
        UtilsButtons.InitStandardButton(btnFermer)
        UtilsButtons.InitStandardButton(btnEnregistrer)
        UtilsButtons.InitStandardButton(btnAnnuler)
        ' Boutons [+] / loupe / copie : AltheaBouton Outil à icône seule, sans UtilsButtons (ses icônes de
        ' survol, claires, disparaîtraient sur le survol vert pâle du kit) : l'icône du Designer reste.

        InitialiserCombos()
        InitialiserFenetre()

    End Sub

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SetContext
    ' Version   : V1.0.0
    ' Date      : 01/07/2026
    '
    ' Rôle      :
    ' Reçoit le contexte UI partagé fourni par Home (barre de statut et infobulles communes).
    '
    ' Paramètres :
    ' - context : Contexte UI partagé.
    '
    ' Remarques :
    ' - Doit être appelée par l'appelant AVANT ShowDialog (cf. UC_PatientFiche).
    ' - errProvider reste local (positionnement des icônes sur la modale).
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(context As UserControlContext) Implements IContextAwareForm.SetContext

        _context = context
        rteCommentaire.SetContext(context)

    End Sub

#End Region

#Region "Chargement Form"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : IntervenantEdition_Load
    ' Version   : V1.3.0
    ' Date      : 19/10/2026
    '
    ' Rôle      :
    ' Initialise les infobulles une fois le contexte injecté (les tooltips passent par _context
    ' lorsqu'il est disponible, sinon par le ToolTip local).
    ' - V1.1.0 (19/10/2026) : garde des modifications non enregistrées (GardeModifications.SurveillerFenetre).
    ' - V1.2.0 (19/10/2026) : textes types proposés dans l'éditeur (UsageTextesTypes).
    ' - V1.3.0 (19/10/2026) : usage propre (alerte, commentaire séparé), v2.43.0.
    ' -------------------------------------------------------------------------------------------------
    Private Sub IntervenantEdition_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        GardeModifications.SurveillerFenetre(Me, btnEnregistrer, "de l'intervention")

        ' Textes types proposés dans l'éditeur (temps 2 de l'éditeur, UsagesTexteType).
        rteCommentaire.UsageTextesTypes = UsagesTexteType.CommentaireIntervenant

        InitialiserToolTips()

    End Sub

#End Region

#Region "Initialisation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserCombos
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Alimente le combo des thérapeutes (référentiel actif) et le combo des rôles (référentiel actif),
    ' puis règle la visibilité des boutons [+].
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserCombos()

        ChargerTherapeutes(Nothing)
        ChargerRoles(Nothing)

        ' Les boutons [+] n'ont de sens que si la fenêtre principale est disponible (gestion des droits/élévation).
        btnAjouterTherapeute.Visible = _fenetrePrincipale IsNot Nothing
        btnAjouterRole.Visible = _fenetrePrincipale IsNot Nothing

        ' La consultation de la liste des thérapeutes passe aussi par la fenêtre principale (droits/élévation).
        btnVoirTherapeutes.Visible = _fenetrePrincipale IsNot Nothing

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerTherapeutes
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' (Re)charge le combo des thérapeutes depuis le référentiel actif et sélectionne le thérapeute
    ' demandé. Une entrée sentinelle « (sélectionner…) » est ajoutée en tête : le thérapeute est
    ' obligatoire, cette entrée invalide donc la saisie tant qu'elle reste choisie.
    '
    ' Paramètres :
    ' - idASelectionner : identifiant du thérapeute à sélectionner, ou Nothing pour « (sélectionner…) »
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerTherapeutes(idASelectionner As Long?)

        Dim therapeutes As List(Of Therapeute) = GestionTherapeutes.GetTherapeutesActifs()

        ' Entrée sentinelle imposant un choix explicite (thérapeute obligatoire).
        therapeutes.Insert(0, New Therapeute With {
            .IdTherapeute = IdTherapeuteNonSelectionne,
            .Nom = "(sélectionner…)"
        })

        ' Le rechargement du DataSource déclenche SelectedIndexChanged : on neutralise
        ' l'auto-remplissage du snapshot le temps de rétablir la sélection voulue.
        Dim ancienEtat As Boolean = _chargementEnCours
        _chargementEnCours = True

        cboTherapeute.DataSource = therapeutes
        cboTherapeute.DisplayMember = NameOf(Therapeute.LibelleAffichage)
        cboTherapeute.ValueMember = NameOf(Therapeute.IdTherapeute)

        If idASelectionner.HasValue Then

            Dim index As Integer = therapeutes.FindIndex(
                Function(t) t.IdTherapeute = idASelectionner.Value)

            cboTherapeute.SelectedIndex = If(index >= 0, index, 0)

        Else
            cboTherapeute.SelectedIndex = 0
        End If

        _chargementEnCours = ancienEtat

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerRoles
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' (Re)charge le combo des rôles depuis le référentiel actif et sélectionne le rôle demandé.
    ' Une entrée sentinelle « (non précisé) » est ajoutée en tête car le rôle est optionnel.
    '
    ' Paramètres :
    ' - idASelectionner : identifiant du rôle à sélectionner, ou Nothing pour « (non précisé) »
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerRoles(idASelectionner As ULong?)

        Dim roles As List(Of RoleIntervenant) = GestionRolesIntervenant.GetRolesIntervenantActifs()

        ' Entrée sentinelle permettant de ne pas préciser de rôle (FK optionnelle).
        roles.Insert(0, New RoleIntervenant With {
            .IdRoleIntervenant = IdRoleNonPrecise,
            .LibelleRoleIntervenant = "(non précisé)"
        })

        cboRole.DataSource = roles
        cboRole.DisplayMember = NameOf(RoleIntervenant.LibelleRoleIntervenant)
        cboRole.ValueMember = NameOf(RoleIntervenant.IdRoleIntervenant)

        If idASelectionner.HasValue Then

            Dim index As Integer = roles.FindIndex(
                Function(r) r.IdRoleIntervenant = idASelectionner.Value)

            cboRole.SelectedIndex = If(index >= 0, index, 0)

        Else
            cboRole.SelectedIndex = 0
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserToolTips
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Initialise les aides contextuelles (infobulles) des boutons d'action du formulaire.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserToolTips()

        DefinirToolTip(btnAjouterTherapeute, "Ajouter un thérapeute manquant au référentiel.")
        DefinirToolTip(btnVoirTherapeutes, "Consulter la liste des thérapeutes (consulter ou compléter une fiche).")
        DefinirToolTip(btnAjouterRole, "Ajouter un rôle d'intervenant manquant au référentiel.")
        DefinirToolTip(btnModifier, "Passer en modification pour éditer cet intervenant.")
        DefinirToolTip(btnFermer, "Fermer la consultation sans modifier.")
        DefinirToolTip(btnEnregistrer, "Enregistrer cet intervenant.")
        DefinirToolTip(txtTelephone, "Téléphone actuel du thérapeute (fiche Thérapeutes) : il n'est pas enregistré avec le suivi.")
        DefinirToolTip(btnAnnuler, "Annuler la saisie en cours.")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DefinirToolTip
    ' Version   : V1.0.0
    ' Date      : 01/07/2026
    '
    ' Rôle      :
    ' Définit une infobulle en privilégiant le contexte partagé de Home (_context) pour harmoniser
    ' le comportement ; repli sur le ToolTip local (ttMain) si aucun contexte n'est injecté.
    ' -------------------------------------------------------------------------------------------------
    Private Sub DefinirToolTip(control As Control, message As String)

        If _context IsNot Nothing Then
            _context.SetToolTip(control, message)
        Else
            ttMain.SetToolTip(control, message)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserFenetre
    ' Version   : V1.1.0
    ' Date      : 14/10/2026
    '
    ' Rôle      :
    ' Applique le titre selon le mode et remplit les champs en modification.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserFenetre()

        If _creation Then

            Text = "Nouvel intervenant"
            enTete.SousTitre = "Nouvel intervenant"
            dtpDateDebut.Valeur = Nothing
            dtpDateFin.Valeur = Nothing

        Else

            RemplirChamps(_suivi)

        End If

        AppliquerMode()

    End Sub

    ' Code du suivi affiché au titre (« — AS000007 ») ; vide pour un nouvel intervenant.
    Private Function SuffixeCode() As String
        Dim code As String = _suivi?.CodeAutreSuiviPatient
        Return If(String.IsNullOrWhiteSpace(code), String.Empty, $" — {code}")
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerMode
    ' Version   : V1.1.0
    ' Date      : 25/10/2026
    '
    ' Rôle      :
    ' Applique le titre, l'état éditable des contrôles et la visibilité des boutons d'action
    ' selon le mode courant (création, consultation ou modification).
    '
    ' Remarques :
    ' - Consultation : champs verrouillés ; boutons « Modifier » et « Fermer » visibles.
    ' - Création / Modification : champs éditables ; boutons « Enregistrer » et « Annuler » visibles.
    ' - V1.1.0 (25/10/2026, relevé aux captures) : en consultation, focus sur « Fermer » (le texte du
    '   premier champ n'apparaît plus sélectionné).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerMode()

        If _consultation Then

            Text = "Consulter l'intervenant" & SuffixeCode()
            enTete.SousTitre = Text

        ElseIf _creation Then

            Text = "Nouvel intervenant"
            enTete.SousTitre = "Nouvel intervenant"

        Else

            Text = "Modifier l'intervenant" & SuffixeCode()
            enTete.SousTitre = Text

        End If

        DefinirEtatChamps(Not _consultation)

        btnEnregistrer.Visible = Not _consultation
        btnAnnuler.Visible = Not _consultation
        btnModifier.Visible = _consultation
        btnFermer.Visible = _consultation

        ' Consultation : le focus sur « Fermer » plutôt que sur le premier champ, dont le texte
        ' apparaîtrait sélectionné alors qu'il n'est pas modifiable.
        If _consultation Then ActiveControl = btnFermer

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DefinirEtatChamps
    ' Version   : V2.1.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Active ou désactive les contrôles de saisie et les boutons annexes selon que la fenêtre
    ' est en édition (editable = True) ou en consultation (editable = False).
    '
    ' Paramètres :
    ' - editable : True pour autoriser la saisie, False pour verrouiller en lecture seule
    '
    ' Remarques :
    ' - Les champs de trace (civilité, nom professionnel, spécialité, adresse, téléphone, e-mail) restent ReadOnly en
    '   permanence (trace pilotée par la sélection du thérapeute) : jamais rendus modifiables, ils
    '   gardent le fond teinté de la lecture seule. C'est pourquoi UtilsControls.DefinirLectureSeule
    '   n'est pas appliqué à la carte Intervenant (il rendrait ces zones modifiables en édition).
    ' - V2.0.0 (UI-7) : listes AltheaListe (désactivées, elles restent lisibles) ; carte Période via
    '   DefinirLectureSeule (règle « Consultation lisible »).
    ' - V2.1.0 (23/10/2026) : champs Adresse (ex-Lieu) et E-mail.
    ' -------------------------------------------------------------------------------------------------
    Private Sub DefinirEtatChamps(editable As Boolean)

        cboTherapeute.Enabled = editable
        cboRole.Enabled = editable
        UtilsControls.DefinirLectureSeule(grpPeriode, Not editable)

        For Each champTrace As TextBox In {txtCivilite, txtNomProfessionnel, txtSpecialite, txtAdresse, txtTelephone, txtEmail}
            champTrace.BackColor = UITheme.ColorChampLectureSeule
        Next

        rteCommentaire.ReadOnlyMode = Not editable

        ' Boutons annexes : verrouillés en consultation, sinon selon leur disponibilité de base.
        btnAjouterTherapeute.Enabled = editable AndAlso _fenetrePrincipale IsNot Nothing
        btnAjouterRole.Enabled = editable AndAlso _fenetrePrincipale IsNot Nothing

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplirChamps
    ' Version   : V1.3.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Renseigne les contrôles depuis un suivi existant.
    '
    ' Paramètres :
    ' - suivi : Suivi source
    '
    ' Remarques :
    ' - V1.2.0 (20/10/2026) : téléphone actuel du thérapeute lié (audit B12).
    ' - V1.3.0 (23/10/2026) : coordonnées actuelles complètes (AfficherCoordonnees) ; lieu figé gardé en
    '   _lieuTrace.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RemplirChamps(suivi As SuiviIntervenant)

        ' Neutralise l'auto-remplissage du snapshot pendant le remplissage programmatique :
        ' le snapshot historique stocké doit primer sur l'identité courante du thérapeute.
        _chargementEnCours = True

        ChargerRoles(suivi.IdRoleIntervenant)

        Dim idTherapeute As Long? = If(suivi.IdTherapeute.HasValue,
                                       CLng(suivi.IdTherapeute.Value),
                                       CType(Nothing, Long?))
        ChargerTherapeutes(idTherapeute)

        ' Snapshot historique figé (trace) : recopié tel qu'enregistré, jamais recalculé ici.
        _idCiviliteTrace = suivi.IdCivilite
        txtCivilite.Text = If(suivi.LibelleCivilite, String.Empty)
        txtNomProfessionnel.Text = suivi.NomProfessionnel
        txtSpecialite.Text = suivi.Specialite
        _lieuTrace = suivi.Lieu

        ' Les coordonnées, elles, ne sont pas figées : celles du thérapeute lié, telles qu'elles sont
        ' aujourd'hui (désactivé compris, il n'est alors pas dans la liste).
        Dim therapeuteLie As Therapeute = Nothing
        If idTherapeute.HasValue Then
            therapeuteLie = TryCast(cboTherapeute.SelectedItem, Therapeute)
            If therapeuteLie Is Nothing OrElse therapeuteLie.IdTherapeute <> idTherapeute.Value Then
                therapeuteLie = GestionTherapeutes.GetTherapeuteById(idTherapeute.Value)
            End If
        End If
        AfficherCoordonnees(therapeuteLie)

        dtpDateDebut.Valeur = suivi.DateDebut
        dtpDateFin.Valeur = suivi.DateFin

        rteCommentaire.ChargerContenu(suivi.CommentaireRtf, suivi.CommentaireTxt)

        _chargementEnCours = False

    End Sub

#End Region

#Region "Validation et enregistrement"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ValiderSaisie
    ' Version  : V2.0.0
    ' Date     : 23/10/2026
    '
    ' Rôle     :
    ' Fait juger la saisie par le métier (GestionSuivisIntervenants.ValiderSuivi : thérapeute choisi,
    ' date de fin pas avant la date de début) et montre les erreurs.
    '
    ' Retour   :
    ' - Boolean : True si la saisie est valide, False sinon
    '
    ' Remarques :
    ' - V2.0.0 (23/10/2026, audit C.2, ADR-51) : plus de copie des règles ici ; icône sur chaque champ
    '   en cause, fenêtre récapitulative et focus sur le premier (AffichageErreursSaisie).
    ' -------------------------------------------------------------------------------------------------
    Private Function ValiderSaisie() As Boolean

        errProvider.Clear()

        Dim saisie As New SuiviIntervenant()
        AlimenterSuiviDepuisChamps(saisie)

        Dim erreurs As List(Of ErreurSaisie) = GestionSuivisIntervenants.ValiderSuivi(saisie)

        Return Not AffichageErreursSaisie.Signaler(erreurs, ChampsDuSuivi(),
                                                   Sub(c, m) errProvider.SetError(c, m),
                                                   "Intervenant")

    End Function

    ' Contrôle de chaque propriété du suivi jugée par ValiderSuivi (ErreurSaisie.Champ).
    Private Function ChampsDuSuivi() As Dictionary(Of String, Control)
        Return New Dictionary(Of String, Control) From {
            {NameOf(SuiviIntervenant.IdTherapeute), cboTherapeute},
            {NameOf(SuiviIntervenant.DateFin), dtpDateFin}
        }
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AlimenterSuiviDepuisChamps
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Recopie la saisie dans un suivi (sans l'enregistrer) : sert à la validation comme à
    ' l'enregistrement, pour que le métier juge exactement ce qui sera écrit.
    '
    ' Paramètres :
    ' - suivi : Suivi à alimenter
    '
    ' Remarques :
    ' - Le lieu enregistré reste la localité figée (_lieuTrace), pas l'adresse complète affichée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AlimenterSuiviDepuisChamps(suivi As SuiviIntervenant)

        suivi.IdPatient = _idPatient
        suivi.IdRoleIntervenant = RoleSelectionne()
        suivi.IdTherapeute = TherapeuteSelectionne()
        suivi.IdCivilite = _idCiviliteTrace
        suivi.NomProfessionnel = txtNomProfessionnel.Text.Trim()
        suivi.Specialite = NormaliserTexte(txtSpecialite.Text)
        suivi.Lieu = NormaliserTexte(_lieuTrace)
        suivi.DateDebut = dtpDateDebut.Valeur
        suivi.DateFin = dtpDateFin.Valeur
        suivi.CommentaireRtf = NormaliserTexte(rteCommentaire.RtfContent)
        suivi.CommentaireTxt = NormaliserTexte(rteCommentaire.TextContent)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Enregistrer
    ' Version   : V1.2.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Valide la saisie, construit le suivi et le persiste (création ou mise à jour),
    ' puis ferme la fenêtre avec DialogResult.OK.
    '
    ' Remarques :
    ' - V1.2.0 (23/10/2026, audit C.2) : suivi alimenté par AlimenterSuiviDepuisChamps ; un refus du
    '   métier (ErreurValidationException…) s'affiche dans une fenêtre, la saisie reste ouverte.
    ' -------------------------------------------------------------------------------------------------
    Private Sub Enregistrer()

        If Not ValiderSaisie() Then
            Return
        End If

        Try

            Dim suivi As SuiviIntervenant = If(_suivi, New SuiviIntervenant())
            AlimenterSuiviDepuisChamps(suivi)

            Dim code As String
            If _creation Then
                code = GestionRecherche.CodeElement(NatureResultat.Intervenant, GestionSuivisIntervenants.CreateSuivi(suivi))
            Else
                GestionSuivisIntervenants.UpdateSuivi(suivi)
                code = suivi.CodeAutreSuiviPatient
            End If

            _suivi = suivi

            _context?.SetStatus(If(_creation, $"Intervenant {code} créé.", $"Intervenant {code} mis à jour."))

            DialogResult = DialogResult.OK
            Close()

        Catch exRefus As InvalidOperationException

            ' Refus du métier : message rédigé pour l'utilisatrice ; la fenêtre reste en saisie.
            DialogChoix.Information(exRefus.Message, "Enregistrement impossible")

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur enregistrement intervenant (IntervenantEdition).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur(
                "Impossible d'enregistrer l'intervenant.",
                "Erreur"
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : TherapeuteSelectionne
    ' Version  : V1.0.0
    ' Date     : 15/06/2026
    '
    ' Rôle     :
    ' Retourne l'identifiant du thérapeute sélectionné, ou Nothing si l'entrée sentinelle
    ' « (sélectionner…) » est choisie (aucun thérapeute valide).
    '
    ' Retour   :
    ' - ULong? : Identifiant du thérapeute, ou Nothing
    ' -------------------------------------------------------------------------------------------------
    Private Function TherapeuteSelectionne() As ULong?

        If cboTherapeute.SelectedValue Is Nothing Then
            Return Nothing
        End If

        Dim idTherapeute As Long = CLng(cboTherapeute.SelectedValue)

        If idTherapeute = IdTherapeuteNonSelectionne Then
            Return Nothing
        End If

        Return CULng(idTherapeute)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : RoleSelectionne
    ' Version  : V1.0.0
    ' Date     : 15/06/2026
    '
    ' Rôle     :
    ' Retourne l'identifiant du rôle sélectionné, ou Nothing si l'entrée « (non précisé) » est choisie.
    '
    ' Retour   :
    ' - ULong? : Identifiant du rôle, ou Nothing
    ' -------------------------------------------------------------------------------------------------
    Private Function RoleSelectionne() As ULong?

        If cboRole.SelectedValue Is Nothing Then
            Return Nothing
        End If

        Dim idRole As ULong = CULng(cboRole.SelectedValue)

        If idRole = IdRoleNonPrecise Then
            Return Nothing
        End If

        Return idRole

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : NormaliserTexte
    ' Version  : V1.0.0
    ' Date     : 15/06/2026
    '
    ' Rôle     :
    ' Convertit une chaîne en valeur stockable : Nothing si vide/blanc, sinon la chaîne nettoyée.
    '
    ' Paramètres :
    ' - valeur (String) : La chaîne à nettoyer.
    '
    ' Retour   :
    ' - String? : La chaîne nettoyée, ou Nothing si vide/blanc.
    ' -------------------------------------------------------------------------------------------------
    ' Convertit une chaîne en valeur stockable : Nothing si vide/blanc, sinon la chaîne nettoyée.
    Private Function NormaliserTexte(valeur As String) As String

        If String.IsNullOrWhiteSpace(valeur) Then
            Return Nothing
        End If

        Return valeur.Trim()

    End Function

#End Region

#Region "Gestionnaires d'événements"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnEnregistrer_Click
    ' Version   : V1.0.0
    ' Date      : 15/06/2026

    ' Rôle      : Déclenche la validation et l'enregistrement du suivi.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEnregistrer_Click(sender As Object, e As EventArgs) Handles btnEnregistrer.Click
        Enregistrer()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAnnuler_Click
    ' Version   : V1.0.0
    ' Date      : 15/06/2026

    ' Rôle      : Ferme la fenêtre sans enregistrer.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAnnuler_Click(sender As Object, e As EventArgs) Handles btnAnnuler.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnModifier_Click
    ' Version   : V1.0.0
    ' Date      : 15/06/2026

    ' Rôle      : Bascule la fenêtre de la consultation (lecture seule) vers l'édition.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnModifier_Click(sender As Object, e As EventArgs) Handles btnModifier.Click
        _consultation = False
        AppliquerMode()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnFermer_Click
    ' Version   : V1.0.0
    ' Date      : 15/06/2026

    ' Rôle      : Ferme la fenêtre de consultation sans modification.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnFermer_Click(sender As Object, e As EventArgs) Handles btnFermer.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : cboTherapeute_SelectedIndexChanged
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' À chaque changement de thérapeute sélectionné, recopie son identité dans les champs snapshot
    ' (Civilité, Nom / praticien, Spécialité, Lieu) afin de figer la trace historique, et affiche ses
    ' coordonnées actuelles (téléphone, e-mail, adresse). Les champs de trace sont
    ' en lecture seule : l'utilisateur n'a pas à les modifier.
    '
    ' Remarques :
    ' - Neutralisé pendant le chargement programmatique (_chargementEnCours) pour ne pas écraser
    '   le snapshot d'un suivi existant lors de l'initialisation.
    ' - Le Lieu enregistré est la localité du thérapeute (vide si la localité est vide) ; il n'est pas
    '   affiché tel quel (case Adresse : adresse complète actuelle).
    ' -------------------------------------------------------------------------------------------------
    Private Sub cboTherapeute_SelectedIndexChanged(sender As Object, e As EventArgs) _
        Handles cboTherapeute.SelectedIndexChanged

        If _chargementEnCours Then
            Return
        End If

        AppliquerSnapshotTherapeute(TryCast(cboTherapeute.SelectedItem, Therapeute))

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerSnapshotTherapeute
    ' Version   : V1.1.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Renseigne les champs snapshot (lecture seule) depuis le thérapeute fourni, ou les vide si le
    ' thérapeute est absent / correspond à l'entrée sentinelle « (sélectionner…) ».
    ' Les coordonnées ne sont pas figées (AfficherCoordonnees, partagé avec RemplirChamps).
    '
    ' Paramètres :
    ' - therapeute : Thérapeute source du snapshot, ou Nothing
    ' - V1.1.0 (23/10/2026) : lieu figé en _lieuTrace ; coordonnées par AfficherCoordonnees.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerSnapshotTherapeute(therapeute As Therapeute)

        If therapeute Is Nothing OrElse therapeute.IdTherapeute = IdTherapeuteNonSelectionne Then
            _idCiviliteTrace = Nothing
            txtCivilite.Clear()
            txtNomProfessionnel.Clear()
            txtSpecialite.Clear()
            _lieuTrace = Nothing
            AfficherCoordonnees(Nothing)
            Return
        End If

        _idCiviliteTrace = therapeute.IdCivilite
        txtCivilite.Text = If(therapeute.LibelleCivilite, String.Empty)
        txtNomProfessionnel.Text = therapeute.NomComplet
        txtSpecialite.Text = If(therapeute.Specialite, String.Empty)
        _lieuTrace = therapeute.Localite
        AfficherCoordonnees(therapeute)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherCoordonnees
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Affiche les coordonnées actuelles du thérapeute (téléphone présenté selon son pays, e-mail,
    ' adresse complète) ; sans thérapeute, l'adresse retombe sur le lieu figé du suivi.
    '
    ' Paramètres :
    ' - therapeute : Thérapeute lié, ou Nothing
    '
    ' Remarques :
    ' - Remplace AfficherTelephone (V1.5.0) : même décision de Joëlle — coordonnées actuelles, pas de
    '   copie figée — étendue à l'e-mail et à l'adresse (relevé du 23/10/2026 : la fiche ne montrait pas
    '   toutes les informations du thérapeute choisi).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherCoordonnees(therapeute As Therapeute)

        If therapeute Is Nothing OrElse therapeute.IdTherapeute = IdTherapeuteNonSelectionne Then
            txtTelephone.Clear()
            txtEmail.Clear()
            txtAdresse.Text = If(_lieuTrace, String.Empty)
            Return
        End If

        Dim numero As String = If(therapeute.Telephone, String.Empty).Trim()
        txtTelephone.Text = If(numero.Length > 0, UtilsTelephone.FormaterAffichage(numero, therapeute.CodePays), String.Empty)
        txtEmail.Text = If(therapeute.Email, String.Empty).Trim()
        txtAdresse.Text = therapeute.AdresseUneLigne

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAjouterRole_Click
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Ouvre le référentiel des rôles d'intervenant dans une fenêtre modale (via la fenêtre principale, qui gère droits
    ' et élévation), puis recharge le combo et sélectionne automatiquement la valeur nouvellement créée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterRole_Click(sender As Object, e As EventArgs) Handles btnAjouterRole.Click

        If _fenetrePrincipale Is Nothing Then
            Return
        End If

        ' Identifiants présents avant l'ouverture du référentiel (détection des ajouts).
        Dim idsAvant As New List(Of ULong)
        Dim listeAvant As List(Of RoleIntervenant) = TryCast(cboRole.DataSource, List(Of RoleIntervenant))

        If listeAvant IsNot Nothing Then
            For Each role As RoleIntervenant In listeAvant
                idsAvant.Add(role.IdRoleIntervenant)
            Next
        End If

        ' Rôle actuellement sélectionné, à préserver si aucun ajout n'est effectué.
        Dim idSelectionCourante As ULong? = RoleSelectionne()

        Dim affiche As Boolean = _fenetrePrincipale.OuvrirReferentielModal(New UC_RolesIntervenant(), "Rôles d'intervenant")

        If Not affiche Then
            Return
        End If

        Try

            Dim rolesApres As List(Of RoleIntervenant) = GestionRolesIntervenant.GetRolesIntervenantActifs()

            ' Recherche d'un identifiant nouvellement créé (présent après, absent avant).
            Dim idNouveau As ULong? = Nothing
            For Each role As RoleIntervenant In rolesApres
                If Not idsAvant.Contains(role.IdRoleIntervenant) Then
                    idNouveau = role.IdRoleIntervenant
                    Exit For
                End If
            Next

            ChargerRoles(If(idNouveau.HasValue, idNouveau, idSelectionCourante))

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur rechargement des rôles d'intervenant après ajout (IntervenantEdition).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAjouterTherapeute_Click
    ' Version   : V2.0.0
    ' Date      : 01/07/2026
    '
    ' Rôle      :
    ' Ouvre directement la Form de création d'un thérapeute (via la fenêtre principale, qui gère droits et élévation),
    ' sans passer par l'écran de liste. Au retour, le thérapeute créé est ajouté au combo, sélectionné,
    ' et son snapshot (lecture seule) est appliqué : l'utilisateur revient sur la saisie de l'intervenant
    ' avec le nouveau thérapeute prêt à l'emploi.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterTherapeute_Click(sender As Object, e As EventArgs) Handles btnAjouterTherapeute.Click

        If _fenetrePrincipale Is Nothing Then
            Return
        End If

        Try

            Dim therapeuteCree = _fenetrePrincipale.OuvrirCreationTherapeuteModal

            If therapeuteCree Is Nothing Then
                Return
            End If

            ' Nouveau thérapeute : on le sélectionne et on applique son snapshot (lecture seule).
            ChargerTherapeutes(therapeuteCree.IdTherapeute)
            AppliquerSnapshotTherapeute(TryCast(cboTherapeute.SelectedItem, Therapeute))

        Catch ex As Exception

            EcrireLog(
                "Erreur ajout direct d'un thérapeute (IntervenantEdition).",
                LogLevel.Succinct,
                LogCategory.UI,
                ex
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnVoirTherapeutes_Click
    ' Version   : V1.0.0
    ' Date      : 01/07/2026
    '
    ' Rôle      :
    ' Ouvre l'écran de liste des thérapeutes (UC_Therapeutes) dans une fenêtre modale (via la fenêtre principale, qui
    ' gère droits et élévation), pour consulter ou compléter une fiche existante. Un bouton « Fermer »
    ' de l'hôte modal permet de revenir proprement à la saisie de l'intervenant. Au retour, le combo
    ' des thérapeutes est rechargé (une fiche complétée peut changer le libellé affiché) en préservant
    ' la sélection courante.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnVoirTherapeutes_Click(sender As Object, e As EventArgs) Handles btnVoirTherapeutes.Click

        If _fenetrePrincipale Is Nothing Then
            Return
        End If

        ' Thérapeute actuellement sélectionné, à préserver au rechargement.
        Dim idSelectionCourante = TherapeuteSelectionne()

        Dim affiche = _fenetrePrincipale.OuvrirReferentielModal(New UC_Therapeutes, "Thérapeutes")

        If Not affiche Then
            Return
        End If

        Try

            Dim idCouranteLong = If(idSelectionCourante.HasValue,
                                             CLng(idSelectionCourante.Value),
                                             CType(Nothing, Long?))

            ChargerTherapeutes(idCouranteLong)

        Catch ex As Exception

            EcrireLog(
                "Erreur rechargement des thérapeutes après consultation (IntervenantEdition).",
                LogLevel.Succinct,
                LogCategory.UI,
                ex
            )

        End Try

    End Sub


#End Region

End Class
