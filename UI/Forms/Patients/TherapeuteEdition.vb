' -------------------------------------------------------------------------------------------------
' Formulaire : TherapeuteEdition
' Projet     : Althéa
' Version    : V1.9.0
' Date       : 25/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Form modale permettant la création et la modification d'un thérapeute (table therapeutes),
' depuis l'écran de gestion du référentiel des thérapeutes (UC_Therapeutes).
'
' Responsabilités :
' - Saisir l'identité (nom/prénom, spécialité choisie dans le référentiel) et les coordonnées du thérapeute
' - Proposer l'ajout d'une spécialité manquante via le bouton [+] (référentiel en modal, auto-sélection)
' - Saisir un commentaire libre (texte simple)
' - Gérer l'indicateur d'activité (Actif)
' - Faire valider la saisie par le métier (GestionTherapeutes.ValiderTherapeute), montrer les erreurs
' - Signaler un homonyme (même nom + prénom) et faire confirmer l'enregistrement
' - Persister le thérapeute via GestionTherapeutes (création / mise à jour)
'
' Fonctionnement :
' - Mode Création     : le constructeur ne reçoit pas de thérapeute existant (Actif coché par défaut)
' - Mode Modification : le constructeur reçoit le thérapeute à éditer
'
' Remarques  :
' - Retour DialogResult.OK après enregistrement réussi
' - Réutilise UtilsTelephone (téléphone/pays) et UtilsValidation (e-mail), comme ContactEdition
' - V1.2.0 (14/10/2026, UI-7) : cartes du kit, listes AltheaListe, case AltheaCase ; zones de texte
'   blanches en saisie (fenêtre toujours en saisie : pas de mode consultation) ; fond AltheaFondFenetre
' - V1.2.1 (19/10/2026) : éditeur de texte refondu (UC_RichTextEditor V2.0.0, profil
'   Compact) ; le commentaire reçoit le contexte (horodatage signé).
' - V1.3.0 (19/10/2026) : modifications non enregistrées — fermer la fenêtre (Annuler, croix, Échap)
'   après une modification propose Enregistrer / Quitter sans enregistrer / Rester (GardeModifications).
' - V1.3.1 (19/10/2026) : éditeur titré pour « Ouvrir en grand » (TitreDocument = « Commentaire du
'   thérapeute »).
' - V1.3.2 (19/10/2026) : éditeur(s) de texte : textes types de leur usage (UsageTextesTypes, temps 2).
' - V1.3.3 (19/10/2026) : textes types de l'usage « Commentaire d'un thérapeute » (v2.43.0).
' - V1.4.0 (22/10/2026, audit C.1, étape 3) : écritures transmises avec le ContexteOperation, droit
'   vérifié par le métier ; refus de droit affiché tel quel.
' - V1.5.0 (23/10/2026, audit C.2 étape 2b) : saisie jugée par GestionTherapeutes.ValiderTherapeute
'   (règles sorties de ValiderSaisie, appliquées aussi à l'enregistrement) ; erreurs sur leurs champs et
'   dans une fenêtre récapitulative (AffichageErreursSaisie) ; homonyme signalé, enregistrement après
'   confirmation (décision de Joëlle : il était refusé) ; refus du métier affiché.
' - V1.6.0 (23/10/2026, v2.55.0) : spécialité choisie dans une liste (référentiel ref_specialites, une
'   seule par thérapeute — décisions de Joëlle), complétée par le bouton [+] ; la fenêtre reçoit Home.
' - V1.7.0 (24/10/2026, relevé de Joëlle) : le code du thérapeute (TH…) au titre de la fenêtre et
'   dans l'annonce de l'enregistrement.
' - V1.8.0 (24/10/2026, v2.56.0) : code postal belge contrôlé par le référentiel, localité proposée,
'   suggérée et confirmée si elle ne correspond pas au code (SaisieLocaliteBelge).
' - V1.9.0 (25/10/2026, audit C.4, ADR-55) : fenêtre principale reçue comme IFenetrePrincipale (et non
'   plus Home) ; boutons [+] inchangés.
'
' Dépendances :
' - GestionTherapeutes / Therapeute (persistance et modèle)
' - UtilsTelephone (formatage/normalisation/validation du téléphone, libellés pays)
' - UtilsValidation, UtilsCodePostal (signal immédiat d'un format douteux à la sortie du champ)
' - AffichageErreursSaisie (erreurs de saisie rendues par le métier)
' - GestionSpecialites / UC_Specialites (liste et bouton [+]), IFenetrePrincipale (ouverture du référentiel en modal)
' - DialogChoix (messages d'erreur)
' - GestionLog (journalisation)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class TherapeuteEdition
    Implements IContextAwareForm

#Region "Variables privées"

    ' Contexte UI partagé fourni par Home (barre de statut, infobulles). Peut être Nothing si la
    ' fenêtre est ouverte sans contexte : un repli local (ttMain) garantit alors les infobulles.
    ' Remarque : errProvider reste local (les icônes d'erreur doivent se positionner sur la modale).
    Private _context As UserControlContext

    ' Thérapeute en cours d'édition (Nothing en création).
    Private _therapeute As Therapeute

    ' Indique si l'on est en mode création (True) ou modification (False).
    Private ReadOnly _creation As Boolean

    ' Fenêtre principale (ouverture du référentiel des spécialités par le bouton [+], droits et élévation
    ' compris) ; Nothing si l'appelant ne la fournit pas : le bouton [+] est alors grisé.
    Private ReadOnly _fenetrePrincipale As IFenetrePrincipale

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
    ' Initialise le formulaire pour la création ou la modification d'un thérapeute.
    '
    ' Paramètres    :
    ' - therapeuteAEditer : Thérapeute à modifier, ou Nothing pour une création
    ' - fenetrePrincipale : Fenêtre principale, pour le bouton [+] des spécialités (facultative)
    '
    ' Remarques     :
    ' - V1.2.0 (23/10/2026, v2.55.0) : reçoit Home (bouton [+] de la liste des spécialités).
    ' - V1.3.0 (24/10/2026, v2.56.0) : aide à la saisie du code postal et de la localité (SaisieLocaliteBelge).
    ' - V1.9.0 (25/10/2026, audit C.4, ADR-55) : reçoit la fenêtre principale (IFenetrePrincipale)
    '   au lieu de Home.
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(therapeuteAEditer As Therapeute, Optional fenetrePrincipale As IFenetrePrincipale = Nothing)

        InitializeComponent()

        ' Fond du kit : beige clair et petite feuille dessinée (remplace l'image étirée).
        AltheaFondFenetre.Appliquer(Me)

        _therapeute = therapeuteAEditer
        _creation = therapeuteAEditer Is Nothing
        _fenetrePrincipale = fenetrePrincipale

        _saisieLocalite = New SaisieLocaliteBelge(txtCodePostal, txtLocalite, AddressOf CodePaysSaisi)
        AddHandler Disposed, Sub() _saisieLocalite.Dispose()

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
    ' - Doit être appelée par l'appelant AVANT ShowDialog (cf. Home, UC_Therapeutes).
    ' - errProvider reste local (positionnement des icônes sur la modale).
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(context As UserControlContext) Implements IContextAwareForm.SetContext

        _context = context
        rteCommentaire.SetContext(context)

    End Sub

#End Region

#Region "Chargement Form"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : TherapeuteEdition_Load
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
    Private Sub TherapeuteEdition_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        GardeModifications.SurveillerFenetre(Me, btnEnregistrer, "du thérapeute")

        ' Textes types proposés dans l'éditeur (temps 2 de l'éditeur, UsagesTexteType).
        rteCommentaire.UsageTextesTypes = UsagesTexteType.CommentaireTherapeute

        UtilsButtons.InitStandardButton(btnEnregistrer)
        UtilsButtons.InitStandardButton(btnAnnuler)

        InitialiserToolTips()

    End Sub

#End Region

#Region "Initialisation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserToolTips
    ' Version   : V1.0.0
    ' Date      : 16/06/2026
    '
    ' Rôle      :
    ' Initialise les aides contextuelles (infobulles) des contrôles du formulaire.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserToolTips()

        DefinirToolTip(chkActif, "Un thérapeute inactif n'apparaît plus dans les listes de sélection.")
        DefinirToolTip(btnAjouterSpecialite, "Ajouter une spécialité manquante au référentiel.")
        DefinirToolTip(btnEnregistrer, "Enregistrer ce thérapeute.")
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
    ' Version   : V1.2.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Applique le titre selon le mode et remplit les champs en modification.
    '
    ' Remarques :
    ' - V1.1.0 (UI-7) : la fenêtre est toujours en saisie ; DefinirLectureSeule(..., False) donne aux
    '   zones de texte le fond blanc de la saisie, comme dans les autres fiches et fenêtres.
    ' - V1.2.0 (23/10/2026, v2.55.0) : liste des spécialités ; bouton [+] actif si Home est fournie.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserFenetre()

        For Each carte As Control In {grpIdentite, grpCoordonnees}
            UtilsControls.DefinirLectureSeule(carte, False)
        Next

        If _creation Then

            Text = "Nouveau thérapeute"
            enTete.SousTitre = "Nouveau thérapeute"
            UtilsSaisieReferentiels.ChargerPays(cboPays, Nothing)
            UtilsSaisieReferentiels.ChargerCivilites(cboCivilite, Nothing)
            UtilsSaisieReferentiels.ChargerSpecialites(cboSpecialite, Nothing)
            chkActif.Checked = True

        Else

            Dim code As String = _therapeute?.CodeTherapeute
            Text = "Modifier le thérapeute" & If(String.IsNullOrWhiteSpace(code), String.Empty, $" — {code}")
            enTete.SousTitre = Text
            RemplirChamps(_therapeute)

        End If

        btnAjouterSpecialite.Enabled = _fenetrePrincipale IsNot Nothing

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplirChamps
    ' Version   : V1.1.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Renseigne les contrôles depuis un thérapeute existant.
    '
    ' Paramètres :
    ' - therapeute : Thérapeute source
    '
    ' Remarques :
    ' - V1.1.0 (23/10/2026, v2.55.0) : spécialité choisie dans la liste (référentiel ref_specialites).
    ' -------------------------------------------------------------------------------------------------
    Private Sub RemplirChamps(therapeute As Therapeute)

        UtilsSaisieReferentiels.ChargerCivilites(cboCivilite, therapeute.IdCivilite)
        txtNom.Text = therapeute.Nom
        txtPrenom.Text = therapeute.Prenom
        UtilsSaisieReferentiels.ChargerSpecialites(cboSpecialite, therapeute.IdSpecialite)
        chkActif.Checked = therapeute.Actif

        UtilsSaisieReferentiels.ChargerPays(cboPays, therapeute.IdPays)
        txtTelephone.Text = UtilsTelephone.FormaterAffichage(therapeute.Telephone, therapeute.CodePays)
        txtEmail.Text = therapeute.Email

        txtAdresseLigne1.Text = therapeute.AdresseLigne1
        txtAdresseLigne2.Text = therapeute.AdresseLigne2
        txtCodePostal.Text = therapeute.CodePostal
        txtLocalite.Text = therapeute.Localite

        rteCommentaire.ChargerContenu(therapeute.Commentaire_rtf, therapeute.Commentaire_txt)

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
    ' Fait juger la saisie par le métier (GestionTherapeutes.ValiderTherapeute : nom, téléphone et code
    ' postal selon le pays, e-mail) et montre les erreurs.
    '
    ' Retour   :
    ' - Boolean : True si la saisie est valide, False sinon
    '
    ' Remarques :
    ' - V2.0.0 (23/10/2026, audit C.2, ADR-51) : plus de copie des règles ici ; icône sur chaque champ
    '   en cause, fenêtre récapitulative et focus sur le premier (AffichageErreursSaisie). L'homonymie
    '   n'est plus un refus : elle est confirmée ensuite (ConfirmerHomonymeEventuel).
    ' -------------------------------------------------------------------------------------------------
    Private Function ValiderSaisie() As Boolean

        errProvider.Clear()

        Dim saisie As New Therapeute()
        AlimenterTherapeuteDepuisChamps(saisie)

        Dim erreurs As List(Of ErreurSaisie) = GestionTherapeutes.ValiderTherapeute(saisie)

        Return Not AffichageErreursSaisie.Signaler(erreurs, ChampsDuTherapeute(),
                                                   Sub(c, m) errProvider.SetError(c, m),
                                                   "Thérapeute")

    End Function

    ' Contrôle de chaque propriété du thérapeute jugée par ValiderTherapeute (ErreurSaisie.Champ).
    Private Function ChampsDuTherapeute() As Dictionary(Of String, Control)
        Return New Dictionary(Of String, Control) From {
            {NameOf(Therapeute.Nom), txtNom},
            {NameOf(Therapeute.Telephone), txtTelephone},
            {NameOf(Therapeute.CodePostal), txtCodePostal},
            {NameOf(Therapeute.Email), txtEmail}
        }
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ConfirmerHomonymeEventuel
    ' Version  : V1.0.0
    ' Date     : 23/10/2026
    '
    ' Rôle     :
    ' Si un autre thérapeute porte déjà ces nom et prénom, le dit et demande s'il faut enregistrer
    ' quand même — à la création, ou quand la modification change le nom ou le prénom.
    '
    ' Retour   :
    ' - Boolean : True si l'enregistrement peut se poursuivre, False si l'utilisatrice renonce
    '
    ' Remarques :
    ' - Décision de Joëlle (23/10/2026) : les homonymes sont fréquents parmi les thérapeutes ; on les
    '   signale, on ne les empêche pas (comme les doublons potentiels de patients).
    ' - Un homonyme déjà accepté n'est pas redemandé à chaque modification de sa fiche : _therapeute
    '   porte encore le nom et le prénom enregistrés (il n'est alimenté qu'après cette question).
    ' -------------------------------------------------------------------------------------------------
    Private Function ConfirmerHomonymeEventuel() As Boolean

        If _therapeute IsNot Nothing AndAlso
           MemeTexte(_therapeute.Nom, txtNom.Text) AndAlso MemeTexte(_therapeute.Prenom, txtPrenom.Text) Then
            Return True
        End If

        Dim idExclu As Long = If(_therapeute IsNot Nothing, _therapeute.IdTherapeute, 0L)

        If Not GestionTherapeutes.TherapeuteExiste(txtNom.Text, txtPrenom.Text, idExclu) Then
            Return True
        End If

        Dim reponse As DialogResult = DialogChoix.Confirmer(
            "Un thérapeute portant les mêmes nom et prénom existe déjà." & Environment.NewLine &
            "Voulez-vous quand même enregistrer celui-ci ?",
            "Homonyme"
        )

        Return reponse = DialogResult.Yes

    End Function

    ' Même texte à la casse et aux espaces de bord près (comparaison de la base, insensible à la casse).
    Private Shared Function MemeTexte(enregistre As String, saisi As String) As Boolean
        Return String.Equals(If(enregistre, String.Empty).Trim(), If(saisi, String.Empty).Trim(),
                             StringComparison.CurrentCultureIgnoreCase)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AlimenterTherapeuteDepuisChamps
    ' Version   : V1.1.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Recopie la saisie dans un thérapeute (sans l'enregistrer) : sert à la validation comme à
    ' l'enregistrement, pour que le métier juge exactement ce qui sera écrit.
    '
    ' Paramètres :
    ' - therapeute : Thérapeute à alimenter
    '
    ' Remarques :
    ' - V1.1.0 (23/10/2026, v2.55.0) : spécialité par son identifiant ; le libellé suit, pour que
    '   l'appelant (fiche Intervenant) puisse le recopier sans relire la base.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AlimenterTherapeuteDepuisChamps(therapeute As Therapeute)

        therapeute.IdCivilite = UtilsSaisieReferentiels.IdCiviliteSaisie(cboCivilite)
        therapeute.Nom = txtNom.Text.Trim()
        therapeute.Prenom = NormaliserTexte(txtPrenom.Text)
        therapeute.IdSpecialite = UtilsSaisieReferentiels.IdSpecialiteSaisie(cboSpecialite)
        therapeute.Specialite = If(therapeute.IdSpecialite.HasValue, cboSpecialite.Text, Nothing)
        therapeute.Actif = chkActif.Checked
        therapeute.IdPays = UtilsSaisieReferentiels.IdPaysSaisi(cboPays)
        therapeute.Telephone = NormaliserTexte(UtilsTelephone.NormaliserE164(txtTelephone.Text, CodePaysSaisi()))
        therapeute.Email = NormaliserTexte(txtEmail.Text)
        therapeute.AdresseLigne1 = NormaliserTexte(txtAdresseLigne1.Text)
        therapeute.AdresseLigne2 = NormaliserTexte(txtAdresseLigne2.Text)
        therapeute.CodePostal = NormaliserTexte(txtCodePostal.Text)
        therapeute.Localite = NormaliserTexte(txtLocalite.Text)

        therapeute.Commentaire_rtf = NormaliserTexte(rteCommentaire.RtfContent)
        therapeute.Commentaire_txt = NormaliserTexte(rteCommentaire.TextContent)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Enregistrer
    ' Version   : V1.2.0
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Valide la saisie, construit le thérapeute et le persiste (création ou mise à jour),
    ' puis ferme la fenêtre avec DialogResult.OK.
    '
    ' Remarques :
    ' - V1.1.0 (23/10/2026, audit C.2) : thérapeute alimenté par AlimenterTherapeuteDepuisChamps ;
    '   homonyme confirmé au lieu d'être refusé ; un refus du métier (ErreurValidationException…)
    '   s'affiche dans une fenêtre, la saisie reste ouverte.
    ' - V1.2.0 (24/10/2026, v2.56.0) : localité qui ne correspond pas au code postal belge : confirmation.
    ' -------------------------------------------------------------------------------------------------
    Private Sub Enregistrer()

        If Not ValiderSaisie() Then
            Return
        End If

        Try

            If Not ConfirmerHomonymeEventuel() Then
                _context?.SetStatus("Enregistrement annulé.")
                Return
            End If

            If Not _saisieLocalite.ConfirmerLocalite() Then Return

            Dim therapeute As Therapeute = If(_therapeute, New Therapeute())
            AlimenterTherapeuteDepuisChamps(therapeute)

            Dim code As String
            If _creation Then
                ' Code généré par la base (TH…) : relu avec la fiche créée.
                Dim idCree As Long = GestionTherapeutes.CreateTherapeute(_context?.ContexteOperation, therapeute)
                code = GestionTherapeutes.GetTherapeuteById(idCree)?.CodeTherapeute
            Else
                GestionTherapeutes.UpdateTherapeute(_context?.ContexteOperation, therapeute)
                code = therapeute.CodeTherapeute
            End If

            _therapeute = therapeute

            Dim libelle As String = If(String.IsNullOrWhiteSpace(code), "Thérapeute", $"Thérapeute {code}")
            _context?.SetStatus(If(_creation, $"{libelle} créé.", $"{libelle} mis à jour."))

            DialogResult = DialogResult.OK
            Close()

        Catch ex As UnauthorizedAccessException

            ' Droit vérifié par le métier (audit C.1) : message rédigé pour l'utilisatrice.
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")

        Catch exRefus As InvalidOperationException

            ' Refus du métier : message rédigé pour l'utilisatrice ; la fenêtre reste en saisie.
            DialogChoix.Information(exRefus.Message, "Enregistrement impossible")

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur enregistrement thérapeute (TherapeuteEdition).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur(
                "Impossible d'enregistrer le thérapeute.",
                "Erreur"
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : NormaliserTexte
    ' Version  : V1.0.0
    ' Date     : 16/06/2026
    '
    ' Rôle     :
    ' Convertit une chaîne en valeur stockable : Nothing si vide/blanc, sinon la chaîne nettoyée.
    ' -------------------------------------------------------------------------------------------------
    Private Function NormaliserTexte(valeur As String) As String

        If String.IsNullOrWhiteSpace(valeur) Then
            Return Nothing
        End If

        Return valeur.Trim()

    End Function

#End Region

#Region "Propriétés publiques"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : TherapeuteEnregistre
    ' Version   : V1.0.0
    ' Date      : 16/06/2026
    '
    ' Rôle      :
    ' Expose le thérapeute après enregistrement (utile à l'appelant pour la resélection en liste).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property TherapeuteEnregistre As Therapeute
        Get
            Return _therapeute
        End Get
    End Property

#End Region

#Region "Gestionnaires d'événements"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnEnregistrer_Click
    ' Version   : V1.0.0
    ' Date      : 16/06/2026

    ' Rôle      : Déclenche la validation et l'enregistrement du thérapeute.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEnregistrer_Click(sender As Object, e As EventArgs) Handles btnEnregistrer.Click
        Enregistrer()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAnnuler_Click
    ' Version   : V1.0.0
    ' Date      : 16/06/2026

    ' Rôle      : Ferme la fenêtre sans enregistrer.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAnnuler_Click(sender As Object, e As EventArgs) Handles btnAnnuler.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : txtTelephone_Leave
    ' Version   : V1.0.0
    ' Date      : 16/06/2026

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
    ' Date      : 16/06/2026

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
    ' Procédure : btnAjouterSpecialite_Click
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Ouvre le référentiel des spécialités dans une fenêtre modale (via la fenêtre principale, qui gère droits et
    ' élévation), puis recharge la liste et sélectionne la spécialité nouvellement créée (sinon garde
    ' la sélection en cours).
    '
    ' Remarques :
    ' - Même fonctionnement que les boutons [+] de ContactEdition et IntervenantEdition.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterSpecialite_Click(sender As Object, e As EventArgs) Handles btnAjouterSpecialite.Click

        If _fenetrePrincipale Is Nothing Then
            Return
        End If

        ' Identifiants présents avant l'ouverture du référentiel (détection des ajouts).
        Dim idsAvant As New HashSet(Of ULong)
        Dim listeAvant As List(Of Specialite) = TryCast(cboSpecialite.DataSource, List(Of Specialite))
        If listeAvant IsNot Nothing Then
            idsAvant.UnionWith(listeAvant.Select(Function(s) s.IdSpecialite))
        End If

        Dim idSelectionCourante As ULong? = UtilsSaisieReferentiels.IdSpecialiteSaisie(cboSpecialite)

        If Not _fenetrePrincipale.OuvrirReferentielModal(New UC_Specialites(), "Spécialités") Then
            Return
        End If

        Try

            Dim nouvelle As Specialite = GestionSpecialites.GetSpecialites(False).
                FirstOrDefault(Function(s) Not idsAvant.Contains(s.IdSpecialite))

            UtilsSaisieReferentiels.ChargerSpecialites(cboSpecialite,
                If(nouvelle IsNot Nothing, nouvelle.IdSpecialite, idSelectionCourante))

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur rechargement des spécialités après ajout (TherapeuteEdition).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

#End Region

End Class
