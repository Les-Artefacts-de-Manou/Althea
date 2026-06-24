' -------------------------------------------------------------------------------------------------
' Formulaire : IntervenantEdition
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 15/06/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Form modale permettant la création et la modification d'un intervenant du réseau de suivi
' d'un patient (table autres_suivis_patient, liaison N-N — D-Q1bis), depuis l'onglet
' « Intervenants » de la fiche patient.
'
' Responsabilités :
' - Saisir le rôle de l'intervenant (référentiel ref_roles_intervenant, optionnel)
' - Saisir l'identité texte libre (nom / praticien, spécialité, lieu)
' - Saisir la période de suivi (date de début, date de fin), toutes deux optionnelles
' - Saisir un commentaire enrichi (RTF + texte) via UC_RichTextEditorSimple (D-Q7bis)
' - Valider la saisie (nom requis, cohérence des dates)
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
' - En V1, id_therapeute reste NULL (saisie texte libre via le nom)
' - Réutilise Home.OuvrirReferentielModal (droits + élévation) pour le bouton [+]
'
' Dépendances :
' - GestionSuivisIntervenants / SuiviIntervenant (persistance et modèle)
' - GestionRolesIntervenant / RoleIntervenant (combo Rôle)
' - UC_RolesIntervenant (référentiel ouvert via le bouton [+])
' - Home (ouverture du référentiel en modal avec gestion des droits)
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

    ' Référence à la fenêtre Home (pour ouvrir le référentiel des rôles via le bouton [+]).
    Private ReadOnly _homeForm As Home

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

    ' Valeur sentinelle du combo des rôles signifiant « aucun rôle précisé » (rôle optionnel).
    Private Const IdRoleNonPrecise As ULong = 0UL

    ' Valeur sentinelle du combo des thérapeutes signifiant « aucun thérapeute sélectionné »
    ' (le thérapeute est obligatoire : cette entrée invalide la saisie tant qu'elle est choisie).
    Private Const IdTherapeuteNonSelectionne As Long = 0L

#End Region

#Region "Constructeur"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version       : V1.1.0
    ' Date          : 18/06/2026
    '
    ' Rôle          :
    ' Initialise le formulaire pour la création, la consultation ou la modification d'un intervenant.
    '
    ' Paramètres    :
    ' - idPatient            : Identifiant du patient (obligatoire, > 0)
    ' - homeForm             : Référence Home pour le bouton [+] (peut être Nothing : le [+] sera masqué)
    ' - suiviAEditer         : Suivi à consulter/modifier, ou Nothing pour une création
    ' - ouvrirEnConsultation : True pour ouvrir un suivi existant en lecture seule (consultation
    '                          d'abord) ; ignoré en création
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(idPatient As Long, homeForm As Home, suiviAEditer As SuiviIntervenant, Optional ouvrirEnConsultation As Boolean = False)

        InitializeComponent()

        _idPatient = idPatient
        _homeForm = homeForm
        _suivi = suiviAEditer
        _creation = suiviAEditer Is Nothing

        ' La consultation ne s'applique qu'à un suivi existant (jamais en création).
        _consultation = ouvrirEnConsultation AndAlso Not _creation

        UtilsButtons.InitStandardButton(btnModifier)
        UtilsButtons.InitStandardButton(btnFermer)
        '    UtilsButtons.InitDiversIconButton(btnVoirTherapeutes)
        UtilsButtons.InitDiversIconButton(btnAjouterTherapeute)
        UtilsButtons.InitDiversIconButton(btnAjouterRole)

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
    ' Procédure : IntervenantEdition_Load
    ' Version   : V1.0.0
    ' Date      : 01/07/2026
    '
    ' Rôle      :
    ' Initialise les infobulles une fois le contexte injecté (les tooltips passent par _context
    ' lorsqu'il est disponible, sinon par le ToolTip local).
    ' -------------------------------------------------------------------------------------------------
    Private Sub IntervenantEdition_Load(sender As Object, e As EventArgs) Handles MyBase.Load

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

        ' Les boutons [+] n'ont de sens que si Home est disponible (gestion des droits/élévation).
        btnAjouterTherapeute.Visible = _homeForm IsNot Nothing
        btnAjouterRole.Visible = _homeForm IsNot Nothing

        ' La consultation de la liste des thérapeutes passe aussi par Home (droits/élévation).
        btnVoirTherapeutes.Visible = _homeForm IsNot Nothing

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
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Applique le titre selon le mode et remplit les champs en modification.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserFenetre()

        If _creation Then

            Text = "Nouvel intervenant"
            lblTitreForm.Text = "Nouvel intervenant"
            dtpDateDebut.Checked = False
            dtpDateFin.Checked = False

        Else

            RemplirChamps(_suivi)

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

            Text = "Consulter l'intervenant"
            lblTitreForm.Text = "Consulter l'intervenant"

        ElseIf _creation Then

            Text = "Nouvel intervenant"
            lblTitreForm.Text = "Nouvel intervenant"

        Else

            Text = "Modifier l'intervenant"
            lblTitreForm.Text = "Modifier l'intervenant"

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
    ' Active ou désactive les contrôles de saisie et les boutons annexes selon que la fenêtre
    ' est en édition (editable = True) ou en consultation (editable = False).
    '
    ' Paramètres :
    ' - editable : True pour autoriser la saisie, False pour verrouiller en lecture seule
    '
    ' Remarques :
    ' - Les champs snapshot (nom professionnel, spécialité, lieu, téléphone) restent ReadOnly en
    '   permanence (trace pilotée par la sélection du thérapeute) : non touchés ici.
    ' -------------------------------------------------------------------------------------------------
    Private Sub DefinirEtatChamps(editable As Boolean)

        cboTherapeute.Enabled = editable
        cboRole.Enabled = editable
        dtpDateDebut.Enabled = editable
        dtpDateFin.Enabled = editable

        rteCommentaire.ReadOnlyMode = Not editable

        ' Boutons annexes : verrouillés en consultation, sinon selon leur disponibilité de base.
        btnAjouterTherapeute.Enabled = editable AndAlso _homeForm IsNot Nothing
        btnAjouterRole.Enabled = editable AndAlso _homeForm IsNot Nothing

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplirChamps
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Renseigne les contrôles depuis un suivi existant.
    '
    ' Paramètres :
    ' - suivi : Suivi source
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
        txtNomProfessionnel.Text = suivi.NomProfessionnel
        txtSpecialite.Text = suivi.Specialite
        txtLieu.Text = suivi.Lieu

        If suivi.DateDebut.HasValue Then
            dtpDateDebut.Value = suivi.DateDebut.Value
            dtpDateDebut.Checked = True
        Else
            dtpDateDebut.Checked = False
        End If

        If suivi.DateFin.HasValue Then
            dtpDateFin.Value = suivi.DateFin.Value
            dtpDateFin.Checked = True
        Else
            dtpDateFin.Checked = False
        End If

        rteCommentaire.ChargerContenu(suivi.CommentaireRtf, suivi.CommentaireTxt)

        _chargementEnCours = False

    End Sub

#End Region

#Region "Validation et enregistrement"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ValiderSaisie
    ' Version  : V1.0.0
    ' Date     : 15/06/2026
    '
    ' Rôle     :
    ' Valide les champs obligatoires et la cohérence des dates avant enregistrement.
    '
    ' Retour   :
    ' - Boolean : True si la saisie est valide, False sinon (ErrorProvider + focus du 1er fautif)
    ' -------------------------------------------------------------------------------------------------
    Private Function ValiderSaisie() As Boolean

        errProvider.Clear()

        Dim premierChampFautif As Control = Nothing

        ' Thérapeute (obligatoire) : un intervenant doit référencer un thérapeute du référentiel.
        ' S'il n'existe pas réellement d'intervenant, on n'en crée pas (décision actée).
        If Not TherapeuteSelectionne().HasValue Then
            errProvider.SetError(cboTherapeute, "Le thérapeute est obligatoire.")
            If premierChampFautif Is Nothing Then premierChampFautif = cboTherapeute
        End If

        ' Cohérence des dates : la fin ne peut pas précéder le début (si les deux sont renseignées)
        If dtpDateDebut.Checked AndAlso dtpDateFin.Checked AndAlso
           dtpDateFin.Value.Date < dtpDateDebut.Value.Date Then

            errProvider.SetError(dtpDateFin, "La date de fin ne peut pas précéder la date de début.")
            If premierChampFautif Is Nothing Then premierChampFautif = dtpDateFin
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
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Valide la saisie, construit le suivi et le persiste (création ou mise à jour),
    ' puis ferme la fenêtre avec DialogResult.OK.
    ' -------------------------------------------------------------------------------------------------
    Private Sub Enregistrer()

        If Not ValiderSaisie() Then
            Return
        End If

        Try

            Dim suivi As SuiviIntervenant = If(_suivi, New SuiviIntervenant())

            suivi.IdPatient = _idPatient
            suivi.IdRoleIntervenant = RoleSelectionne()
            suivi.IdTherapeute = TherapeuteSelectionne()
            suivi.NomProfessionnel = txtNomProfessionnel.Text.Trim()
            suivi.Specialite = NormaliserTexte(txtSpecialite.Text)
            suivi.Lieu = NormaliserTexte(txtLieu.Text)
            suivi.DateDebut = If(dtpDateDebut.Checked, CType(dtpDateDebut.Value.Date, Date?), Nothing)
            suivi.DateFin = If(dtpDateFin.Checked, CType(dtpDateFin.Value.Date, Date?), Nothing)
            suivi.CommentaireRtf = NormaliserTexte(rteCommentaire.RtfContent)
            suivi.CommentaireTxt = NormaliserTexte(rteCommentaire.TextContent)

            If _creation Then
                GestionSuivisIntervenants.CreateSuivi(suivi)
            Else
                GestionSuivisIntervenants.UpdateSuivi(suivi)
            End If

            _suivi = suivi

            _context?.SetStatus(If(_creation, "Intervenant créé.", "Intervenant mis à jour."))

            DialogResult = DialogResult.OK
            Close()

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
    ' (Nom / praticien, Spécialité, Lieu) afin de figer la trace historique. Les champs snapshot sont
    ' en lecture seule : l'utilisateur n'a pas à les modifier.
    '
    ' Remarques :
    ' - Neutralisé pendant le chargement programmatique (_chargementEnCours) pour ne pas écraser
    '   le snapshot d'un suivi existant lors de l'initialisation.
    ' - Le Lieu est déduit de la localité du thérapeute (vide si la localité est vide).
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
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Renseigne les champs snapshot (lecture seule) depuis le thérapeute fourni, ou les vide si le
    ' thérapeute est absent / correspond à l'entrée sentinelle « (sélectionner…) ».
    '
    ' Paramètres :
    ' - therapeute : Thérapeute source du snapshot, ou Nothing
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerSnapshotTherapeute(therapeute As Therapeute)

        If therapeute Is Nothing OrElse therapeute.IdTherapeute = IdTherapeuteNonSelectionne Then
            txtNomProfessionnel.Clear()
            txtSpecialite.Clear()
            txtLieu.Clear()
            txtTelephone.Clear()
            Return
        End If

        txtNomProfessionnel.Text = therapeute.NomComplet
        txtSpecialite.Text = If(therapeute.Specialite, String.Empty)
        txtLieu.Text = If(therapeute.Localite, String.Empty)
        txtTelephone.Text = If(therapeute.Telephone, String.Empty)

        Dim saisieTel As String = txtTelephone.Text.Trim()
        If saisieTel.Length > 0 Then
            txtTelephone.Text = UtilsTelephone.FormaterAffichage(saisieTel, therapeute.Pays)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAjouterRole_Click
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Ouvre le référentiel des rôles d'intervenant dans une fenêtre modale (via Home, qui gère droits
    ' et élévation), puis recharge le combo et sélectionne automatiquement la valeur nouvellement créée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterRole_Click(sender As Object, e As EventArgs) Handles btnAjouterRole.Click

        If _homeForm Is Nothing Then
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

        Dim affiche As Boolean = _homeForm.OuvrirReferentielModal(New UC_RolesIntervenant(), "Rôles d'intervenant")

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
    ' Ouvre directement la Form de création d'un thérapeute (via Home, qui gère droits et élévation),
    ' sans passer par l'écran de liste. Au retour, le thérapeute créé est ajouté au combo, sélectionné,
    ' et son snapshot (lecture seule) est appliqué : l'utilisateur revient sur la saisie de l'intervenant
    ' avec le nouveau thérapeute prêt à l'emploi.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterTherapeute_Click(sender As Object, e As EventArgs) Handles btnAjouterTherapeute.Click

        If _homeForm Is Nothing Then
            Return
        End If

        Try

            Dim therapeuteCree = _homeForm.OuvrirCreationTherapeuteModal

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
    ' Ouvre l'écran de liste des thérapeutes (UC_Therapeutes) dans une fenêtre modale (via Home, qui
    ' gère droits et élévation), pour consulter ou compléter une fiche existante. Un bouton « Fermer »
    ' de l'hôte modal permet de revenir proprement à la saisie de l'intervenant. Au retour, le combo
    ' des thérapeutes est rechargé (une fiche complétée peut changer le libellé affiché) en préservant
    ' la sélection courante.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnVoirTherapeutes_Click(sender As Object, e As EventArgs) Handles btnVoirTherapeutes.Click

        If _homeForm Is Nothing Then
            Return
        End If

        ' Thérapeute actuellement sélectionné, à préserver au rechargement.
        Dim idSelectionCourante = TherapeuteSelectionne()

        Dim affiche = _homeForm.OuvrirReferentielModal(New UC_Therapeutes, "Thérapeutes")

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
