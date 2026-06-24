' -------------------------------------------------------------------------------------------------
' Formulaire : ContactEdition
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 13/06/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Form modale permettant la création et la modification d'un contact de l'entourage d'un patient
' (table famille_contacts), depuis l'onglet « Famille / Contacts » de la fiche patient.
'
' Responsabilités :
' - Saisir le lien (référentiel ref_liens_patient), l'identité et les coordonnées du contact
' - Gérer les indicateurs de rôle (autorité parentale, représentant légal, personne autorisée, urgence)
' - Saisir un commentaire enrichi (RTF + texte) via UC_RichTextEditorSimple (D-Q7bis)
' - Valider la saisie (nom/prénom requis, téléphone/e-mail country-aware)
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
' - Réutilise UtilsTelephone (téléphone/pays) et Home.OuvrirReferentielModal (droits + élévation)
'
' Dépendances :
' - GestionFamilleContacts / FamilleContact (persistance et modèle)
' - GestionLiensPatient / LienPatient (combo Lien)
' - UtilsTelephone (formatage/normalisation/validation du téléphone, libellés pays)
' - UtilsValidation (validation e-mail)
' - UC_LiensPatient (référentiel ouvert via le bouton [+])
' - Home (ouverture du référentiel en modal avec gestion des droits)
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

    ' Référence à la fenêtre Home (pour ouvrir le référentiel des liens via le bouton [+]).
    Private ReadOnly _homeForm As Home

    ' Contact en cours d'édition (Nothing en création).
    Private _contact As FamilleContact

    ' Indique si l'on est en mode création (True) ou modification (False).
    Private ReadOnly _creation As Boolean

    ' Vrai tant que la fenêtre est ouverte en consultation (lecture seule) : passe à False
    ' dès que l'utilisateur clique sur « Modifier » pour entrer en édition.
    Private _consultation As Boolean

    ' Code du lien présélectionné à la création (cas le plus fréquent : la mère).
    Private Const CodeLienParDefaut As String = "MERE"

    ' Code du rôle légal présélectionné à la création (cas le plus fréquent : l'autorité parentale).
    Private Const CodeRoleParDefaut As String = "AUTORITE_PARENTALE"

#End Region

#Region "Constructeur"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version       : V1.2.0
    ' Date          : 18/06/2026
    '
    ' Rôle          :
    ' Initialise le formulaire pour la création, la consultation ou la modification d'un contact.
    '
    ' Paramètres    :
    ' - idPatient            : Identifiant du patient (obligatoire, > 0)
    ' - homeForm             : Référence Home pour le bouton [+] (peut être Nothing : le [+] sera masqué)
    ' - contactAEditer       : Contact à consulter/modifier, ou Nothing pour une création
    ' - patientCourant       : Patient courant, pour la copie rapide de l'adresse (peut être Nothing)
    ' - ouvrirEnConsultation : True pour ouvrir un contact existant en lecture seule (consultation
    '                          d'abord) ; ignoré en création
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(idPatient As Long, homeForm As Home, contactAEditer As FamilleContact, patientCourant As Patient, Optional ouvrirEnConsultation As Boolean = False)

        InitializeComponent()

        _idPatient = idPatient
        _homeForm = homeForm
        _contact = contactAEditer
        _creation = contactAEditer Is Nothing
        _patientCourant = patientCourant

        ' La consultation ne s'applique qu'à un contact existant (jamais en création).
        _consultation = ouvrirEnConsultation AndAlso Not _creation

        UtilsButtons.InitStandardButton(btnModifier)
        UtilsButtons.InitStandardButton(btnFermer)

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

    End Sub

#End Region

#Region "Chargement Form"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ContactEdition_Load
    ' Version   : V1.0.0
    ' Date      : 01/07/2026
    '
    ' Rôle      :
    ' Initialise les infobulles une fois le contexte injecté (les tooltips passent par _context
    ' lorsqu'il est disponible, sinon par le ToolTip local).
    ' -------------------------------------------------------------------------------------------------
    Private Sub ContactEdition_Load(sender As Object, e As EventArgs) Handles MyBase.Load

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
    ' Alimente le combo des liens (référentiel actif) et le combo des pays.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserCombos()

        cboPays.Items.Clear()
        cboPays.Items.AddRange(UtilsTelephone.LibellesPays().ToArray())

        ChargerLiens(Nothing)
        ChargerRolesLegaux(Nothing)

        ' Le bouton [+] n'a de sens que si Home est disponible (gestion des droits/élévation).
        btnAjouterLien.Visible = _homeForm IsNot Nothing
        btnAjouterRole.Visible = _homeForm IsNot Nothing

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
    ' Version   : V1.0.0
    ' Date      : 14/06/2026
    '
    ' Rôle      :
    ' Présélectionne le lien « Mère » à la création d'un contact (cas le plus fréquent).
    '
    ' Remarques :
    ' - Le lien est recherché par son code (CodeLienPatient = "MERE"), insensible à la casse.
    ' - Si le lien n'existe pas (référentiel modifié), la sélection courante est conservée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub SelectionnerLienParDefaut()

        Dim liens As List(Of LienPatient) = TryCast(cboLien.DataSource, List(Of LienPatient))

        If liens Is Nothing Then
            Return
        End If

        Dim index As Integer = liens.FindIndex(
            Function(l) String.Equals(l.CodeLienPatient, CodeLienParDefaut, StringComparison.OrdinalIgnoreCase))

        If index >= 0 Then
            cboLien.SelectedIndex = index
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SelectionnerRoleParDefaut
    ' Version   : V1.0.0
    ' Date      : 14/06/2026
    '
    ' Rôle      :
    ' Présélectionne le rôle « Autorité parentale » à la création d'un contact (cas le plus fréquent).
    '
    ' Remarques :
    ' - Le rôle est recherché par son code (CodeRoleLegal = "AUTORITE_PARENTALE"), insensible à la casse.
    ' - Si le rôle n'existe pas (référentiel modifié), la sélection courante est conservée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub SelectionnerRoleParDefaut()

        Dim roles As List(Of RoleLegal) = TryCast(cboRoleLegal.DataSource, List(Of RoleLegal))

        If roles Is Nothing Then
            Return
        End If

        Dim index As Integer = roles.FindIndex(
            Function(r) String.Equals(r.CodeRoleLegal, CodeRoleParDefaut, StringComparison.OrdinalIgnoreCase))

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
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Applique le titre selon le mode et remplit les champs en modification.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserFenetre()

        If _creation Then

            Text = "Nouveau contact"
            lblTitreForm.Text = "Nouveau contact"
            SelectionnerLienParDefaut()
            SelectionnerRoleParDefaut()
            SelectionnerPays(UtilsTelephone.LibellePaysParDefaut)
            dtpDateNaissance.Checked = False

        Else

            RemplirChamps(_contact)

        End If

        AppliquerMode()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerMode
    ' Version   : V1.0.0
    ' Date      : 18/06/2026
    '
    ' Rôle      :
    ' Applique le titre, l'état éditable des contrôles et la visibilité des boutons d'action
    ' selon le mode courant (création, consultation ou modification).
    '
    ' Remarques :
    ' - Consultation : champs verrouillés ; boutons « Modifier » et « Fermer » visibles.
    ' - Création / Modification : champs éditables ; boutons « Enregistrer » et « Annuler » visibles.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerMode()

        If _consultation Then

            Text = "Consulter le contact"
            lblTitreForm.Text = "Consulter le contact"

        ElseIf _creation Then

            Text = "Nouveau contact"
            lblTitreForm.Text = "Nouveau contact"

        Else

            Text = "Modifier le contact"
            lblTitreForm.Text = "Modifier le contact"

        End If

        DefinirEtatChamps(Not _consultation)

        btnEnregistrer.Visible = Not _consultation
        btnAnnuler.Visible = Not _consultation
        btnModifier.Visible = _consultation
        btnFermer.Visible = _consultation

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DefinirEtatChamps
    ' Version   : V1.0.0
    ' Date      : 18/06/2026
    '
    ' Rôle      :
    ' Active ou désactive l'ensemble des contrôles de saisie et des boutons annexes selon que
    ' la fenêtre est en édition (editable = True) ou en consultation (editable = False).
    '
    ' Paramètres :
    ' - editable : True pour autoriser la saisie, False pour verrouiller en lecture seule
    ' -------------------------------------------------------------------------------------------------
    Private Sub DefinirEtatChamps(editable As Boolean)

        cboLien.Enabled = editable
        txtNom.ReadOnly = Not editable
        txtPrenom.ReadOnly = Not editable
        dtpDateNaissance.Enabled = editable

        txtTelephone.ReadOnly = Not editable
        txtEmail.ReadOnly = Not editable
        cboPays.Enabled = editable
        txtAdresseLigne1.ReadOnly = Not editable
        txtAdresseLigne2.ReadOnly = Not editable
        txtCodePostal.ReadOnly = Not editable
        txtLocalite.ReadOnly = Not editable

        cboRoleLegal.Enabled = editable

        rteCommentaire.ReadOnlyMode = Not editable

        ' Boutons annexes : verrouillés en consultation, sinon selon leur disponibilité de base.
        btnAjouterLien.Enabled = editable AndAlso _homeForm IsNot Nothing
        btnAjouterRole.Enabled = editable AndAlso _homeForm IsNot Nothing
        btnCopierAdressePatient.Enabled = editable AndAlso _patientCourant IsNot Nothing

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplirChamps
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
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

        txtNom.Text = contact.Nom
        txtPrenom.Text = contact.Prenom

        If contact.DateNaissance.HasValue Then
            dtpDateNaissance.Value = contact.DateNaissance.Value
            dtpDateNaissance.Checked = True
        Else
            dtpDateNaissance.Checked = False
        End If

        SelectionnerPays(contact.Pays)
        txtTelephone.Text = UtilsTelephone.FormaterAffichage(contact.Telephone, contact.Pays)
        txtEmail.Text = contact.Email

        txtAdresseLigne1.Text = contact.AdresseLigne1
        txtAdresseLigne2.Text = contact.AdresseLigne2
        txtCodePostal.Text = contact.CodePostal
        txtLocalite.Text = contact.Localite

        rteCommentaire.ChargerContenu(contact.CommentaireRtf, contact.CommentaireTxt)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SelectionnerPays
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Sélectionne le libellé de pays canonique correspondant à la valeur fournie.
    '
    ' Paramètres :
    ' - libellePays : Libellé de pays (éventuellement ancien/localisé)
    ' -------------------------------------------------------------------------------------------------
    Private Sub SelectionnerPays(libellePays As String)

        Dim canonique As String = UtilsTelephone.NormaliserLibellePays(libellePays)

        If String.IsNullOrWhiteSpace(canonique) Then
            canonique = UtilsTelephone.LibellePaysParDefaut
        End If

        Dim index As Integer = cboPays.Items.IndexOf(canonique)

        If index >= 0 Then
            cboPays.SelectedIndex = index
        Else
            cboPays.Text = canonique
        End If

    End Sub

#End Region

#Region "Validation et enregistrement"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ValiderSaisie
    ' Version  : V1.0.0
    ' Date     : 13/06/2026
    '
    ' Rôle     :
    ' Valide les champs obligatoires et les formats (téléphone/e-mail) avant enregistrement.
    '
    ' Retour   :
    ' - Boolean : True si la saisie est valide, False sinon (ErrorProvider + focus du 1er fautif)
    ' -------------------------------------------------------------------------------------------------
    Private Function ValiderSaisie() As Boolean

        errProvider.Clear()

        Dim premierChampFautif As Control = Nothing

        ' Lien (obligatoire : FK NOT NULL)
        If cboLien.SelectedValue Is Nothing Then
            errProvider.SetError(btnAjouterLien, "Sélectionnez un lien (ou ajoutez-en un via +).")
            If premierChampFautif Is Nothing Then premierChampFautif = cboLien
        End If

        ' Rôle légal (obligatoire : FK NOT NULL)
        If cboRoleLegal.SelectedValue Is Nothing Then
            errProvider.SetError(btnAjouterRole, "Sélectionnez un rôle légal (ou ajoutez-en un via +).")
            If premierChampFautif Is Nothing Then premierChampFautif = cboRoleLegal
        End If

        ' Nom (obligatoire)
        If String.IsNullOrWhiteSpace(txtNom.Text) Then
            errProvider.SetError(txtNom, "Le nom est obligatoire.")
            If premierChampFautif Is Nothing Then premierChampFautif = txtNom
        End If

        ' Prénom (obligatoire)
        If String.IsNullOrWhiteSpace(txtPrenom.Text) Then
            errProvider.SetError(txtPrenom, "Le prénom est obligatoire.")
            If premierChampFautif Is Nothing Then premierChampFautif = txtPrenom
        End If

        ' Téléphone (optionnel mais validé selon le pays s'il est renseigné)
        Dim messageTel As String = String.Empty
        If Not UtilsTelephone.Valider(txtTelephone.Text, cboPays.Text, messageTel) Then
            errProvider.SetError(txtTelephone, messageTel)
            If premierChampFautif Is Nothing Then premierChampFautif = txtTelephone
        End If

        ' E-mail (optionnel mais validé s'il est renseigné)
        Dim messageEmail As String = String.Empty
        If Not UtilsValidation.IsValidEmail(txtEmail.Text, messageEmail) Then
            errProvider.SetError(txtEmail, messageEmail)
            If premierChampFautif Is Nothing Then premierChampFautif = txtEmail
        End If

        If premierChampFautif IsNot Nothing Then
            premierChampFautif.Focus()
            Return False
        End If

        Return True

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Enregistrer
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Valide la saisie, construit le contact et le persiste (création ou mise à jour),
    ' puis ferme la fenêtre avec DialogResult.OK.
    ' -------------------------------------------------------------------------------------------------
    Private Sub Enregistrer()

        If Not ValiderSaisie() Then
            Return
        End If

        Try

            Dim contact As FamilleContact = If(_contact, New FamilleContact())

            contact.IdPatient = _idPatient
            contact.IdLienPatient = CULng(cboLien.SelectedValue)
            contact.IdRoleLegal = CULng(cboRoleLegal.SelectedValue)
            contact.Nom = txtNom.Text.Trim()
            contact.Prenom = txtPrenom.Text.Trim()
            contact.DateNaissance = If(dtpDateNaissance.Checked, CType(dtpDateNaissance.Value.Date, Date?), Nothing)
            contact.Pays = NormaliserTexte(cboPays.Text)
            contact.Telephone = NormaliserTexte(UtilsTelephone.NormaliserE164(txtTelephone.Text, cboPays.Text))
            contact.Email = NormaliserTexte(txtEmail.Text)
            contact.AdresseLigne1 = NormaliserTexte(txtAdresseLigne1.Text)
            contact.AdresseLigne2 = NormaliserTexte(txtAdresseLigne2.Text)
            contact.CodePostal = NormaliserTexte(txtCodePostal.Text)
            contact.Localite = NormaliserTexte(txtLocalite.Text)
            contact.CommentaireRtf = NormaliserTexte(rteCommentaire.RtfContent)
            contact.CommentaireTxt = NormaliserTexte(rteCommentaire.TextContent)

            If _creation Then
                GestionFamilleContacts.CreateContact(contact)
            Else
                GestionFamilleContacts.UpdateContact(contact)
            End If

            _contact = contact

            _context?.SetStatus(If(_creation, "Contact créé.", "Contact mis à jour."))

            DialogResult = DialogResult.OK
            Close()

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

        If saisie.Length = 0 Then
            Return
        End If

        txtTelephone.Text = UtilsTelephone.FormaterAffichage(saisie, cboPays.Text)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : cboPays_SelectedIndexChanged
    ' Version   : V1.0.0
    ' Date      : 13/06/2026

    ' Rôle      : Reformate le téléphone selon le nouveau pays sélectionné.
    ' -------------------------------------------------------------------------------------------------
    Private Sub cboPays_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboPays.SelectedIndexChanged

        Dim saisie As String = txtTelephone.Text.Trim()

        If saisie.Length = 0 Then
            Return
        End If

        txtTelephone.Text = UtilsTelephone.FormaterAffichage(saisie, cboPays.Text)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAjouterLien_Click
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Ouvre le référentiel des liens patient dans une fenêtre modale (via Home, qui gère droits et
    ' élévation), puis recharge le combo et sélectionne automatiquement la valeur nouvellement créée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterLien_Click(sender As Object, e As EventArgs) Handles btnAjouterLien.Click

        If _homeForm Is Nothing Then
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

        Dim affiche As Boolean = _homeForm.OuvrirReferentielModal(New UC_LiensPatient(), "Liens du patient")

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
    ' Ouvre le référentiel des rôles légaux dans une fenêtre modale (via Home, qui gère droits et
    ' élévation), puis recharge le combo et sélectionne automatiquement la valeur nouvellement créée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterRole_Click(sender As Object, e As EventArgs) Handles btnAjouterRole.Click

        If _homeForm Is Nothing Then
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

        Dim affiche As Boolean = _homeForm.OuvrirReferentielModal(New UC_RoleLegal(), "Rôles légaux")

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
    ' Version   : V1.0.0
    ' Date      : 14/06/2026
    '
    ' Rôle      :
    ' Reprend l'adresse complète du patient courant (adresse, complément, code postal, localité, pays)
    ' dans les champs du contact. Pratique pour un enfant partageant l'adresse d'un parent.
    '
    ' Remarques :
    ' - Le pays est repris avant le téléphone afin de reformater correctement le numéro existant.
    ' - Sans patient courant disponible, l'action est sans effet.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnCopierAdressePatient_Click(sender As Object, e As EventArgs) Handles btnCopierAdressePatient.Click

        If _patientCourant Is Nothing Then
            Return
        End If

        txtAdresseLigne1.Text = _patientCourant.AdresseLigne1
        txtAdresseLigne2.Text = _patientCourant.AdresseLigne2
        txtCodePostal.Text = _patientCourant.CodePostal
        txtLocalite.Text = _patientCourant.Localite

        SelectionnerPays(_patientCourant.Pays)

        '  on réaligne l'affichage d'un éventuel téléphone déjà saisi.
        Dim saisieTel As String = txtTelephone.Text.Trim()
        If saisieTel.Length > 0 Then
            txtTelephone.Text = UtilsTelephone.FormaterAffichage(saisieTel, cboPays.Text)
        End If

    End Sub

#End Region

End Class
