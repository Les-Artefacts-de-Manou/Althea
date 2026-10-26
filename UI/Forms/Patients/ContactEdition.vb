' -------------------------------------------------------------------------------------------------
' Formulaire : ContactEdition
' Projet     : Althéa
' Version    : V1.9.0
' Date       : 25/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Form modale permettant la création et la modification d'un contact de l'entourage d'un patient
' (table famille_contacts), depuis l'onglet « Famille / Contacts » de la fiche patient.
'
' Responsabilités :
' - Saisir le lien (référentiel ref_liens_patient), l'identité et les coordonnées du contact
' - Gérer les indicateurs de rôle (autorité parentale, représentant légal, personne autorisée, urgence)
' - Saisir un commentaire enrichi (RTF + texte) via UC_RichTextEditor, profil Compact (D-Q7bis)
' - Faire valider la saisie par le métier (GestionFamilleContacts.ValiderContact) et montrer les erreurs
' - Persister le contact via GestionFamilleContacts (création / mise à jour)
' - Proposer l'ajout d'un lien manquant via le bouton [+] (référentiel en modal, auto-sélection)
'
' Fonctionnement :
' - Mode Création  : le constructeur ne reçoit pas de contact existant
' - Mode Modification : le constructeur reçoit le contact à éditer
'
' Remarques  :
' - Retour DialogResult.OK après enregistrement réussi
' - Le patient doit être déjà enregistré (idPatient > 0) ; garanti par l'activation progressive
' - Réutilise UtilsTelephone (téléphone/pays) et IFenetrePrincipale.OuvrirReferentielModal (droits + élévation)
' - V1.2.0 (14/10/2026, UI-7) : cartes du kit, listes AltheaListe, date de naissance AltheaDate
'   facultative, consultation lisible (UtilsControls.DefinirLectureSeule) ; fond AltheaFondFenetre,
'   boutons [+] et copie d'adresse en AltheaBouton Outil (icône seule)
' - V1.3.0 (15/10/2026) : petits boutons du kit sans UtilsButtons (icône du Designer, survol du kit)
' - V1.3.1 (19/10/2026) : éditeur de texte refondu (UC_RichTextEditor V2.0.0, profil
'   Compact) ; le commentaire reçoit le contexte (horodatage signé).
' - V1.4.0 (19/10/2026) : modifications non enregistrées — fermer la fenêtre (Annuler, croix, Échap)
'   après une modification propose Enregistrer / Quitter sans enregistrer / Rester (GardeModifications).
' - V1.4.1 (19/10/2026) : éditeur titré pour « Ouvrir en grand » (TitreDocument = « Commentaire du
'   contact »).
' - V1.4.2 (19/10/2026) : éditeur(s) de texte : textes types de leur usage (UsageTextesTypes, temps 2).
' - V1.4.3 (19/10/2026) : textes types de l'usage « Commentaire d'un contact » (v2.43.0).
' - V1.5.0 (23/10/2026, audit C.2 étape 2b) : saisie jugée par GestionFamilleContacts.ValiderContact
'   (règles sorties de ValiderSaisie, appliquées aussi à l'enregistrement) ; erreurs sur leurs champs
'   et dans une fenêtre récapitulative (AffichageErreursSaisie) ; refus du métier affiché.
' - V1.6.0 (24/10/2026, relevé de Joëlle) : le code du contact (FC…) au titre de la fenêtre et dans
'   l'annonce de l'enregistrement.
' - V1.7.0 (24/10/2026, v2.56.0) : code postal belge contrôlé par le référentiel, localité proposée,
'   suggérée et confirmée si elle ne correspond pas au code (SaisieLocaliteBelge).
' - V1.8.0 (24/10/2026, audit C.3 étape 2, v2.57.0) : lien et rôle légal présélectionnés à la
'   création lus dans le référentiel (case « Par défaut ») au lieu des codes MERE et AUTORITE_PARENTALE.
' - V1.8.1 (25/10/2026, relevé aux captures) : consultation ouverte avec le focus sur « Fermer »,
'   plus de texte sélectionné dans le premier champ ; en-tête d'AppliquerMode replacé au-dessus d'elle.
' - V1.9.0 (25/10/2026, audit C.4, ADR-55) : fenêtre principale reçue comme IFenetrePrincipale (et non
'   plus Home) ; boutons [+] inchangés.
'
' Dépendances :
' - GestionFamilleContacts / FamilleContact (persistance et modèle)
' - GestionLiensPatient / LienPatient (combo Lien)
' - UtilsTelephone (formatage/normalisation du téléphone, libellés pays)
' - UtilsValidation, UtilsCodePostal (signal immédiat d'un format douteux à la sortie du champ)
' - AffichageErreursSaisie (erreurs de saisie rendues par le métier)
' - UC_LiensPatient (référentiel ouvert via le bouton [+])
' - IFenetrePrincipale (ouverture du référentiel en modal avec gestion des droits)
' - GestionLog (journalisation)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class ContactEdition
    Implements IContextAwareForm

#Region "Variables privées"

    ' Contexte UI partagé fourni par Home (barre de statut, infobulles). Peut être Nothing si la
    ' fenêtre est ouverte sans contexte : un repli local (ttMain) garantit alors les infobulles.
    ' Remarque : errProvider reste local (les icônes d'erreur doivent se positionner sur la modale).
    Private _context As UserControlContext

    ' Identifiant du patient auquel le contact est rattaché.
    Private ReadOnly _idPatient As Long

    ' Patient courant (sert à la copie rapide de l'adresse vers le contact ; peut être Nothing).
    Private ReadOnly _patientCourant As Patient

    ' Fenêtre principale (pour ouvrir le référentiel des liens via le bouton [+]).
    Private ReadOnly _fenetrePrincipale As IFenetrePrincipale

    ' Contact en cours d'édition (Nothing en création).
    Private _contact As FamilleContact

    ' Indique si l'on est en mode création (True) ou modification (False).
    Private ReadOnly _creation As Boolean

    ' Vrai tant que la fenêtre est ouverte en consultation (lecture seule) : passe à False
    ' dès que l'utilisateur clique sur « Modifier » pour entrer en édition.
    Private _consultation As Boolean

    ' Aide à la saisie du code postal et de la localité d'une adresse belge (v2.56.0).
    Private ReadOnly _saisieLocalite As SaisieLocaliteBelge

#End Region

#Region "Constructeur"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version       : V1.9.0
    ' Date          : 25/10/2026
    '
    ' Rôle          :
    ' Initialise le formulaire pour la création, la consultation ou la modification d'un contact.
    '
    ' Paramètres    :
    ' - idPatient            : Identifiant du patient (obligatoire, > 0)
    ' - fenetrePrincipale : Fenêtre principale pour le bouton [+] (peut être Nothing : le [+] sera masqué)
    ' - contactAEditer       : Contact à consulter/modifier, ou Nothing pour une création
    ' - patientCourant       : Patient courant, pour la copie rapide de l'adresse (peut être Nothing)
    ' - ouvrirEnConsultation : True pour ouvrir un contact existant en lecture seule (consultation
    '                          d'abord) ; ignoré en création
    ' - V1.5.0 (24/10/2026, v2.56.0) : aide à la saisie du code postal et de la localité (SaisieLocaliteBelge).
    ' - V1.9.0 (25/10/2026, audit C.4, ADR-55) : reçoit la fenêtre principale (IFenetrePrincipale)
    '   au lieu de Home.
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(idPatient As Long, fenetrePrincipale As IFenetrePrincipale, contactAEditer As FamilleContact, patientCourant As Patient, Optional ouvrirEnConsultation As Boolean = False)

        InitializeComponent()

        ' Fond du kit : beige clair et petite feuille dessinée (remplace l'image étirée).
        AltheaFondFenetre.Appliquer(Me)

        _idPatient = idPatient
        _fenetrePrincipale = fenetrePrincipale
        _contact = contactAEditer
        _creation = contactAEditer Is Nothing
        _patientCourant = patientCourant

        ' La consultation ne s'applique qu'à un contact existant (jamais en création).
        _consultation = ouvrirEnConsultation AndAlso Not _creation

        UtilsButtons.InitStandardButton(btnModifier)
        UtilsButtons.InitStandardButton(btnFermer)
        UtilsButtons.InitStandardButton(btnEnregistrer)
        UtilsButtons.InitStandardButton(btnAnnuler)

        ' Boutons [+] / loupe / copie : AltheaBouton Outil à icône seule, sans UtilsButtons (ses icônes de
        ' survol, claires, disparaîtraient sur le survol vert pâle du kit) : l'icône du Designer reste.

        _saisieLocalite = New SaisieLocaliteBelge(txtCodePostal, txtLocalite, AddressOf CodePaysSaisi)
        AddHandler Disposed, Sub() _saisieLocalite.Dispose()

        InitialiserCombos()
        InitialiserFenetre()
        _saisieLocalite.Actualiser()

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
    ' Procédure : ContactEdition_Load
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
    Private Sub ContactEdition_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        GardeModifications.SurveillerFenetre(Me, btnEnregistrer, "du contact")

        ' Textes types proposés dans l'éditeur (temps 2 de l'éditeur, UsagesTexteType).
        rteCommentaire.UsageTextesTypes = UsagesTexteType.CommentaireContact

        InitialiserToolTips()

    End Sub

#End Region

#Region "Initialisation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserCombos
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Alimente le combo des liens et celui des rôles légaux (pays et civilité : à l'affichage du contact).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserCombos()

        ChargerLiens(Nothing)
        ChargerRolesLegaux(Nothing)

        ' Le bouton [+] n'a de sens que si la fenêtre principale est disponible (gestion des droits/élévation).
        btnAjouterLien.Visible = _fenetrePrincipale IsNot Nothing
        btnAjouterRole.Visible = _fenetrePrincipale IsNot Nothing

        ' La copie d'adresse n'a de sens que si le patient courant est connu.
        btnCopierAdressePatient.Visible = _patientCourant IsNot Nothing

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerLiens
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' (Re)charge le combo des liens depuis le référentiel actif et sélectionne le lien demandé.
    '
    ' Paramètres :
    ' - idASelectionner : identifiant du lien à sélectionner, ou Nothing pour conserver le 1er
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerLiens(idASelectionner As ULong?)

        Dim liens As List(Of LienPatient) = GestionLiensPatient.GetLiensPatientActifs()

        cboLien.DataSource = liens
        cboLien.DisplayMember = NameOf(LienPatient.LibelleLienPatient)
        cboLien.ValueMember = NameOf(LienPatient.IdLienPatient)

        If idASelectionner.HasValue Then

            Dim index As Integer = liens.FindIndex(
                Function(l) l.IdLienPatient = idASelectionner.Value)

            If index >= 0 Then
                cboLien.SelectedIndex = index
            End If

        ElseIf liens.Count > 0 Then
            cboLien.SelectedIndex = 0
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerRolesLegaux
    ' Version   : V1.0.0
    ' Date      : 14/06/2026
    '
    ' Rôle      :
    ' (Re)charge le combo des rôles légaux depuis le référentiel actif et sélectionne le rôle demandé.
    '
    ' Paramètres :
    ' - idASelectionner : identifiant du rôle à sélectionner, ou Nothing pour conserver le 1er
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerRolesLegaux(idASelectionner As ULong?)

        Dim roles As List(Of RoleLegal) = GestionRoleLegal.GetRolesLegauxActifs()

        cboRoleLegal.DataSource = roles
        cboRoleLegal.DisplayMember = NameOf(RoleLegal.LibelleRoleLegal)
        cboRoleLegal.ValueMember = NameOf(RoleLegal.IdRoleLegal)

        If idASelectionner.HasValue Then

            Dim index As Integer = roles.FindIndex(
                Function(r) r.IdRoleLegal = idASelectionner.Value)

            If index >= 0 Then
                cboRoleLegal.SelectedIndex = index
            End If

        ElseIf roles.Count > 0 Then
            cboRoleLegal.SelectedIndex = 0
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SelectionnerLienParDefaut
    ' Version   : V2.0.0
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Présélectionne, à la création d'un contact, le lien coché « Par défaut » dans le référentiel
    ' (au départ Mère, le cas le plus fréquent).
    '
    ' Remarques :
    ' - Aucun lien par défaut (ou désactivé) : la sélection courante est conservée.
    ' - V2.0.0 (24/10/2026, audit C.3 étape 2, v2.57.0) : case du référentiel au lieu du code « MERE ».
    ' -------------------------------------------------------------------------------------------------
    Private Sub SelectionnerLienParDefaut()

        Dim liens As List(Of LienPatient) = TryCast(cboLien.DataSource, List(Of LienPatient))

        If liens Is Nothing Then
            Return
        End If

        Dim index As Integer = liens.FindIndex(Function(l) l.ParDefaut)

        If index >= 0 Then
            cboLien.SelectedIndex = index
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SelectionnerRoleParDefaut
    ' Version   : V2.0.0
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Présélectionne, à la création d'un contact, le rôle légal coché « Par défaut » dans le
    ' référentiel (au départ Autorité parentale, le cas le plus fréquent).
    '
    ' Remarques :
    ' - Aucun rôle par défaut (ou désactivé) : la sélection courante est conservée.
    ' - V2.0.0 (24/10/2026, audit C.3 étape 2, v2.57.0) : case du référentiel au lieu du code
    '   « AUTORITE_PARENTALE ».
    ' -------------------------------------------------------------------------------------------------
    Private Sub SelectionnerRoleParDefaut()

        Dim roles As List(Of RoleLegal) = TryCast(cboRoleLegal.DataSource, List(Of RoleLegal))

        If roles Is Nothing Then
            Return
        End If

        Dim index As Integer = roles.FindIndex(Function(r) r.ParDefaut)

        If index >= 0 Then
            cboRoleLegal.SelectedIndex = index
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserToolTips
    ' Version   : V1.0.0
    ' Date      : 14/06/2026
    '
    ' Rôle      :
    ' Initialise les aides contextuelles (infobulles) des boutons d'action du formulaire.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserToolTips()

        DefinirToolTip(btnAjouterLien, "Ajouter un lien manquant au référentiel.")
        DefinirToolTip(btnAjouterRole, "Ajouter un rôle légal manquant au référentiel.")
        DefinirToolTip(btnCopierAdressePatient, "Reprendre l'adresse du patient pour ce contact.")
        DefinirToolTip(btnModifier, "Passer en modification pour éditer ce contact.")
        DefinirToolTip(btnFermer, "Fermer la consultation sans modifier.")
        DefinirToolTip(btnEnregistrer, "Enregistrer ce contact.")
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

            Text = "Nouveau contact"
            enTete.Text = "Nouveau contact"
            SelectionnerLienParDefaut()
            SelectionnerRoleParDefaut()
            UtilsSaisieReferentiels.ChargerPays(cboPays, Nothing)
            UtilsSaisieReferentiels.ChargerCivilites(cboCivilite, Nothing)
            dtpDateNaissance.Valeur = Nothing

        Else

            RemplirChamps(_contact)

        End If

        AppliquerMode()

    End Sub

    ' Code du contact affiché au titre (« — FC000004 ») ; vide pour un nouveau contact.
    Private Function SuffixeCode() As String
        Dim code As String = _contact?.CodeFamilleContact
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

            Text = "Consulter le contact" & SuffixeCode()
            enTete.Text = Text

        ElseIf _creation Then

            Text = "Nouveau contact"
            enTete.Text = "Nouveau contact"

        Else

            Text = "Modifier le contact" & SuffixeCode()
            enTete.Text = Text

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
    ' Version   : V2.0.0
    ' Date      : 14/10/2026
    '
    ' Rôle      :
    ' Bascule les champs entre édition (editable = True) et consultation (editable = False).
    '
    ' Paramètres :
    ' - editable : True pour autoriser la saisie, False pour la consultation
    '
    ' Remarques :
    ' - V2.0.0 (UI-7) : règle « Consultation lisible » — UtilsControls.DefinirLectureSeule sur les
    '   trois cartes de saisie (texte normal, fond teinté) au lieu de champs grisés un par un.
    ' - L'éditeur du commentaire garde son propre mode lecture seule (UserControl ignoré par
    '   DefinirLectureSeule).
    ' -------------------------------------------------------------------------------------------------
    Private Sub DefinirEtatChamps(editable As Boolean)

        For Each carte As Control In {grpIdentite, grpCoordonnees, grpRoles}
            UtilsControls.DefinirLectureSeule(carte, Not editable)
        Next

        rteCommentaire.ReadOnlyMode = Not editable

        ' Boutons annexes : verrouillés en consultation, sinon selon leur disponibilité de base.
        btnAjouterLien.Enabled = editable AndAlso _fenetrePrincipale IsNot Nothing
        btnAjouterRole.Enabled = editable AndAlso _fenetrePrincipale IsNot Nothing
        btnCopierAdressePatient.Enabled = editable AndAlso _patientCourant IsNot Nothing

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplirChamps
    ' Version   : V1.1.0
    ' Date      : 14/10/2026
    '
    ' Rôle      :
    ' Renseigne les contrôles depuis un contact existant.
    '
    ' Paramètres :
    ' - contact : Contact source
    ' -------------------------------------------------------------------------------------------------
    Private Sub RemplirChamps(contact As FamilleContact)

        ChargerLiens(contact.IdLienPatient)
        ChargerRolesLegaux(contact.IdRoleLegal)

        UtilsSaisieReferentiels.ChargerCivilites(cboCivilite, contact.IdCivilite)
        txtNom.Text = contact.Nom
        txtPrenom.Text = contact.Prenom

        dtpDateNaissance.Valeur = contact.DateNaissance

        UtilsSaisieReferentiels.ChargerPays(cboPays, contact.IdPays)
        txtTelephone.Text = UtilsTelephone.FormaterAffichage(contact.Telephone, contact.CodePays)
        txtEmail.Text = contact.Email

        txtAdresseLigne1.Text = contact.AdresseLigne1
        txtAdresseLigne2.Text = contact.AdresseLigne2
        txtCodePostal.Text = contact.CodePostal
        txtLocalite.Text = contact.Localite

        rteCommentaire.ChargerContenu(contact.CommentaireRtf, contact.CommentaireTxt)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CodePaysSaisi
    ' Version  : V1.0.0
    ' Date     : 12/10/2026
    '
    ' Rôle     :
    ' Code ISO du pays choisi (ex. « BE »), transmis aux règles de validation et de mise en forme
    ' du téléphone et du code postal. Nothing si aucun pays n'est choisi (pas de contrôle de format).
    ' -------------------------------------------------------------------------------------------------
    Private Function CodePaysSaisi() As String
        Return UtilsSaisieReferentiels.CodePaysSaisi(cboPays)
    End Function

#End Region

#Region "Validation et enregistrement"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ValiderSaisie
    ' Version  : V2.0.0
    ' Date     : 23/10/2026
    '
    ' Rôle     :
    ' Fait juger la saisie par le métier (GestionFamilleContacts.ValiderContact : lien et rôle légal
    ' choisis, nom et prénom, téléphone et code postal selon le pays, e-mail) et montre les erreurs.
    '
    ' Retour   :
    ' - Boolean : True si la saisie est valide, False sinon
    '
    ' Remarques :
    ' - V2.0.0 (23/10/2026, audit C.2, ADR-51) : plus de copie des règles ici ; icône sur chaque champ
    '   en cause, fenêtre récapitulative et focus sur le premier (AffichageErreursSaisie).
    ' - L'icône d'une liste (lien, rôle légal) se place sur son bouton [+] : à côté de la liste, elle le
    '   recouvrirait.
    ' -------------------------------------------------------------------------------------------------
    Private Function ValiderSaisie() As Boolean

        errProvider.Clear()

        Dim saisie As New FamilleContact()
        AlimenterContactDepuisChamps(saisie)

        Dim erreurs As List(Of ErreurSaisie) = GestionFamilleContacts.ValiderContact(saisie)

        Return Not AffichageErreursSaisie.Signaler(erreurs, ChampsDuContact(),
                                                   Sub(c, m) errProvider.SetError(SupportIcone(c), m),
                                                   "Contact")

    End Function

    ' Contrôle de chaque propriété du contact jugée par ValiderContact (ErreurSaisie.Champ).
    Private Function ChampsDuContact() As Dictionary(Of String, Control)
        Return New Dictionary(Of String, Control) From {
            {NameOf(FamilleContact.IdLienPatient), cboLien},
            {NameOf(FamilleContact.IdRoleLegal), cboRoleLegal},
            {NameOf(FamilleContact.Nom), txtNom},
            {NameOf(FamilleContact.Prenom), txtPrenom},
            {NameOf(FamilleContact.Telephone), txtTelephone},
            {NameOf(FamilleContact.CodePostal), txtCodePostal},
            {NameOf(FamilleContact.Email), txtEmail}
        }
    End Function

    ' Contrôle qui porte l'icône d'erreur d'un champ : le bouton [+] pour les listes lien et rôle légal.
    Private Function SupportIcone(champ As Control) As Control
        If champ Is cboLien Then Return btnAjouterLien
        If champ Is cboRoleLegal Then Return btnAjouterRole
        Return champ
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AlimenterContactDepuisChamps
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Recopie la saisie dans un contact (sans l'enregistrer) : sert à la validation comme à
    ' l'enregistrement, pour que le métier juge exactement ce qui sera écrit.
    '
    ' Paramètres :
    ' - contact : Contact à alimenter
    '
    ' Remarques :
    ' - Liste sans choix : identifiant 0, refusé par ValiderContact.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AlimenterContactDepuisChamps(contact As FamilleContact)

        contact.IdPatient = _idPatient
        contact.IdLienPatient = If(cboLien.SelectedValue Is Nothing, 0UL, CULng(cboLien.SelectedValue))
        contact.IdRoleLegal = If(cboRoleLegal.SelectedValue Is Nothing, 0UL, CULng(cboRoleLegal.SelectedValue))
        contact.IdCivilite = UtilsSaisieReferentiels.IdCiviliteSaisie(cboCivilite)
        contact.Nom = txtNom.Text.Trim()
        contact.Prenom = txtPrenom.Text.Trim()
        contact.DateNaissance = dtpDateNaissance.Valeur
        contact.IdPays = UtilsSaisieReferentiels.IdPaysSaisi(cboPays)
        contact.Telephone = NormaliserTexte(UtilsTelephone.NormaliserE164(txtTelephone.Text, CodePaysSaisi()))
        contact.Email = NormaliserTexte(txtEmail.Text)
        contact.AdresseLigne1 = NormaliserTexte(txtAdresseLigne1.Text)
        contact.AdresseLigne2 = NormaliserTexte(txtAdresseLigne2.Text)
        contact.CodePostal = NormaliserTexte(txtCodePostal.Text)
        contact.Localite = NormaliserTexte(txtLocalite.Text)
        contact.CommentaireRtf = NormaliserTexte(rteCommentaire.RtfContent)
        contact.CommentaireTxt = NormaliserTexte(rteCommentaire.TextContent)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Enregistrer
    ' Version   : V1.3.0
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Valide la saisie, construit le contact et le persiste (création ou mise à jour),
    ' puis ferme la fenêtre avec DialogResult.OK.
    '
    ' Remarques :
    ' - V1.2.0 (23/10/2026, audit C.2) : contact alimenté par AlimenterContactDepuisChamps ; un refus du
    '   métier (ErreurValidationException…) s'affiche dans une fenêtre, la saisie reste ouverte.
    ' - V1.3.0 (24/10/2026, v2.56.0) : localité qui ne correspond pas au code postal belge : confirmation.
    ' -------------------------------------------------------------------------------------------------
    Private Sub Enregistrer()

        If Not ValiderSaisie() Then
            Return
        End If

        If Not _saisieLocalite.ConfirmerLocalite() Then Return

        Try

            Dim contact As FamilleContact = If(_contact, New FamilleContact())
            AlimenterContactDepuisChamps(contact)

            Dim code As String
            If _creation Then
                code = GestionRecherche.CodeElement(NatureResultat.Contact, GestionFamilleContacts.CreateContact(contact))
            Else
                GestionFamilleContacts.UpdateContact(contact)
                code = contact.CodeFamilleContact
            End If

            _contact = contact

            _context?.SetStatus(If(_creation, $"Contact {code} créé.", $"Contact {code} mis à jour."))

            DialogResult = DialogResult.OK
            Close()

        Catch exRefus As InvalidOperationException

            ' Refus du métier : message rédigé pour l'utilisatrice ; la fenêtre reste en saisie.
            DialogChoix.Information(exRefus.Message, "Enregistrement impossible")

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur enregistrement contact (ContactEdition).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur(
                "Impossible d'enregistrer le contact.",
                "Erreur"
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : NormaliserTexte
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Convertit une chaîne en valeur stockable : Nothing si vide/blanc, sinon la chaîne nettoyée.
    '
    ' Paramètres : - valeur : Chaîne source
    ' Retour : Chaîne nettoyée ou Nothing
    ' -------------------------------------------------------------------------------------------------
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
    ' Date      : 13/06/2026

    ' Rôle      : Déclenche la validation et l'enregistrement du contact.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEnregistrer_Click_1(sender As Object, e As EventArgs) Handles btnEnregistrer.Click
        Enregistrer()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAnnuler_Click
    ' Version   : V1.0.0
    ' Date      : 13/06/2026

    ' Rôle      : Ferme la fenêtre sans enregistrer.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAnnuler_Click(sender As Object, e As EventArgs) Handles btnAnnuler.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnModifier_Click
    ' Version   : V1.0.0
    ' Date      : 13/06/2026

    ' Rôle      : Bascule la fenêtre de la consultation (lecture seule) vers l'édition.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnModifier_Click(sender As Object, e As EventArgs) Handles btnModifier.Click
        _consultation = False
        AppliquerMode()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnFermer_Click
    ' Version   : V1.0.0
    ' Date      : 13/06/2026

    ' Rôle      : Ferme la fenêtre de consultation sans modification.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnFermer_Click(sender As Object, e As EventArgs) Handles btnFermer.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : txtTelephone_Leave
    ' Version   : V1.0.0
    ' Date      : 13/06/2026

    ' Rôle      : Reformate le téléphone selon le pays sélectionné à la sortie du champ.
    ' -------------------------------------------------------------------------------------------------
    Private Sub txtTelephone_Leave(sender As Object, e As EventArgs) Handles txtTelephone.Leave

        Dim saisie As String = txtTelephone.Text.Trim()

        If saisie.Length > 0 Then
            txtTelephone.Text = UtilsTelephone.FormaterAffichage(saisie, CodePaysSaisi())
        End If

        ControlerChampsPays()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : cboPays_SelectedIndexChanged
    ' Version   : V1.0.0
    ' Date      : 13/06/2026

    ' Rôle      : Reformate le téléphone selon le nouveau pays sélectionné.
    ' -------------------------------------------------------------------------------------------------
    Private Sub cboPays_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboPays.SelectedIndexChanged

        Dim saisie As String = txtTelephone.Text.Trim()

        If saisie.Length > 0 Then
            txtTelephone.Text = UtilsTelephone.FormaterAffichage(saisie, CodePaysSaisi())
        End If

        ControlerChampsPays()

    End Sub

    Private Sub txtCodePostal_Leave(sender As Object, e As EventArgs) Handles txtCodePostal.Leave
        ControlerChampsPays()
    End Sub

    Private Sub txtEmail_Leave(sender As Object, e As EventArgs) Handles txtEmail.Leave
        Dim message As String = String.Empty
        UtilsValidation.IsValidEmail(txtEmail.Text, message)
        errProvider.SetError(txtEmail, message)
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ControlerChampsPays
    ' Version   : V1.1.0
    ' Date      : 24/10/2026
    '
    ' Rôle      : Signale immédiatement les anomalies de téléphone et de code postal selon le pays.
    '             Le blocage intervient à l'enregistrement (ValiderSaisie).
    ' - V1.1.0 (24/10/2026, v2.56.0) : code postal belge inexistant signalé ; localité qui ne correspond
    '   pas au code : avertissement (SaisieLocaliteBelge).
    ' -------------------------------------------------------------------------------------------------
    Private Sub ControlerChampsPays()

        Dim message As String = String.Empty
        UtilsTelephone.Valider(txtTelephone.Text, CodePaysSaisi(), message)
        errProvider.SetError(txtTelephone, message)

        errProvider.SetError(txtCodePostal, _saisieLocalite.MessageCodePostal())
        _saisieLocalite.Actualiser()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAjouterLien_Click
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Ouvre le référentiel des liens patient dans une fenêtre modale (via la fenêtre principale, qui gère droits et
    ' élévation), puis recharge le combo et sélectionne automatiquement la valeur nouvellement créée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterLien_Click(sender As Object, e As EventArgs) Handles btnAjouterLien.Click

        If _fenetrePrincipale Is Nothing Then
            Return
        End If

        ' Identifiants présents avant l'ouverture du référentiel (détection des ajouts).
        Dim idsAvant As New List(Of ULong)
        Dim listeAvant As List(Of LienPatient) = TryCast(cboLien.DataSource, List(Of LienPatient))

        If listeAvant IsNot Nothing Then
            For Each lien As LienPatient In listeAvant
                idsAvant.Add(lien.IdLienPatient)
            Next
        End If

        ' Lien actuellement sélectionné, à préserver si aucun ajout n'est effectué.
        Dim idSelectionCourante As ULong? = Nothing
        If cboLien.SelectedValue IsNot Nothing Then
            idSelectionCourante = CULng(cboLien.SelectedValue)
        End If

        Dim affiche As Boolean = _fenetrePrincipale.OuvrirReferentielModal(New UC_LiensPatient(), "Liens du patient")

        If Not affiche Then
            Return
        End If

        Try

            Dim liensApres As List(Of LienPatient) = GestionLiensPatient.GetLiensPatientActifs()

            ' Recherche d'un identifiant nouvellement créé (présent après, absent avant).
            Dim idNouveau As ULong? = Nothing
            For Each lien As LienPatient In liensApres
                If Not idsAvant.Contains(lien.IdLienPatient) Then
                    idNouveau = lien.IdLienPatient
                    Exit For
                End If
            Next

            ChargerLiens(If(idNouveau.HasValue, idNouveau, idSelectionCourante))

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur rechargement des liens après ajout (ContactEdition).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAjouterRole_Click
    ' Version   : V1.0.0
    ' Date      : 14/06/2026
    '
    ' Rôle      :
    ' Ouvre le référentiel des rôles légaux dans une fenêtre modale (via la fenêtre principale, qui gère droits et
    ' élévation), puis recharge le combo et sélectionne automatiquement la valeur nouvellement créée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterRole_Click(sender As Object, e As EventArgs) Handles btnAjouterRole.Click

        If _fenetrePrincipale Is Nothing Then
            Return
        End If

        ' Identifiants présents avant l'ouverture du référentiel (détection des ajouts).
        Dim idsAvant As New List(Of ULong)
        Dim listeAvant As List(Of RoleLegal) = TryCast(cboRoleLegal.DataSource, List(Of RoleLegal))

        If listeAvant IsNot Nothing Then
            For Each role As RoleLegal In listeAvant
                idsAvant.Add(role.IdRoleLegal)
            Next
        End If

        ' Rôle actuellement sélectionné, à préserver si aucun ajout n'est effectué.
        Dim idSelectionCourante As ULong? = Nothing
        If cboRoleLegal.SelectedValue IsNot Nothing Then
            idSelectionCourante = CULng(cboRoleLegal.SelectedValue)
        End If

        Dim affiche As Boolean = _fenetrePrincipale.OuvrirReferentielModal(New UC_RoleLegal(), "Rôles légaux")

        If Not affiche Then
            Return
        End If

        Try

            Dim rolesApres As List(Of RoleLegal) = GestionRoleLegal.GetRolesLegauxActifs()

            ' Recherche d'un identifiant nouvellement créé (présent après, absent avant).
            Dim idNouveau As ULong? = Nothing
            For Each role As RoleLegal In rolesApres
                If Not idsAvant.Contains(role.IdRoleLegal) Then
                    idNouveau = role.IdRoleLegal
                    Exit For
                End If
            Next

            ChargerRolesLegaux(If(idNouveau.HasValue, idNouveau, idSelectionCourante))

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur rechargement des rôles légaux après ajout (ContactEdition).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnCopierAdressePatient_Click
    ' Version   : V1.1.0
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Reprend l'adresse complète du patient courant (adresse, complément, code postal, localité, pays)
    ' dans les champs du contact. Pratique pour un enfant partageant l'adresse d'un parent.
    '
    ' Remarques :
    ' - Le pays est repris avant le téléphone afin de reformater correctement le numéro existant.
    ' - Sans patient courant disponible, l'action est sans effet.
    ' - V1.1.0 (24/10/2026, v2.56.0) : suggestions et avertissement de la localité actualisés.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnCopierAdressePatient_Click(sender As Object, e As EventArgs) Handles btnCopierAdressePatient.Click

        If _patientCourant Is Nothing Then
            Return
        End If

        txtAdresseLigne1.Text = _patientCourant.AdresseLigne1
        txtAdresseLigne2.Text = _patientCourant.AdresseLigne2
        txtCodePostal.Text = _patientCourant.CodePostal
        txtLocalite.Text = _patientCourant.Localite

        UtilsSaisieReferentiels.ChargerPays(cboPays, _patientCourant.IdPays)
        _saisieLocalite.Actualiser()

        '  on réaligne l'affichage d'un éventuel téléphone déjà saisi.
        Dim saisieTel As String = txtTelephone.Text.Trim()
        If saisieTel.Length > 0 Then
            txtTelephone.Text = UtilsTelephone.FormaterAffichage(saisieTel, CodePaysSaisi())
        End If

    End Sub

#End Region

End Class
