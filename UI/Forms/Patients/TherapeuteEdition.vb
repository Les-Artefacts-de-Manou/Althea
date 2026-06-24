' -------------------------------------------------------------------------------------------------
' Formulaire : TherapeuteEdition
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 16/06/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Form modale permettant la création et la modification d'un thérapeute (table therapeutes),
' depuis l'écran de gestion du référentiel des thérapeutes (UC_Therapeutes).
'
' Responsabilités :
' - Saisir l'identité (nom/prénom/spécialité) et les coordonnées du thérapeute
' - Saisir un commentaire libre (texte simple)
' - Gérer l'indicateur d'activité (Actif)
' - Valider la saisie (nom requis, téléphone/e-mail country-aware, unicité nom + prénom)
' - Persister le thérapeute via GestionTherapeutes (création / mise à jour)
'
' Fonctionnement :
' - Mode Création     : le constructeur ne reçoit pas de thérapeute existant (Actif coché par défaut)
' - Mode Modification : le constructeur reçoit le thérapeute à éditer
'
' Remarques  :
' - Retour DialogResult.OK après enregistrement réussi
' - Réutilise UtilsTelephone (téléphone/pays) et UtilsValidation (e-mail), comme ContactEdition
'
' Dépendances :
' - GestionTherapeutes / Therapeute (persistance et modèle)
' - UtilsTelephone (formatage/normalisation/validation du téléphone, libellés pays)
' - UtilsValidation (validation e-mail)
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

#End Region

#Region "Constructeur"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version       : V1.0.0
    ' Date          : 16/06/2026
    '
    ' Rôle          :
    ' Initialise le formulaire pour la création ou la modification d'un thérapeute.
    '
    ' Paramètres    :
    ' - therapeuteAEditer : Thérapeute à modifier, ou Nothing pour une création
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(therapeuteAEditer As Therapeute)

        InitializeComponent()

        _therapeute = therapeuteAEditer
        _creation = therapeuteAEditer Is Nothing

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
    ' - Doit être appelée par l'appelant AVANT ShowDialog (cf. Home, UC_Therapeutes).
    ' - errProvider reste local (positionnement des icônes sur la modale).
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(context As UserControlContext) Implements IContextAwareForm.SetContext

        _context = context

    End Sub

#End Region

#Region "Chargement Form"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : TherapeuteEdition_Load
    ' Version   : V1.0.0
    ' Date      : 01/07/2026
    '
    ' Rôle      :
    ' Initialise les infobulles une fois le contexte injecté (les tooltips passent par _context
    ' lorsqu'il est disponible, sinon par le ToolTip local).
    ' -------------------------------------------------------------------------------------------------
    Private Sub TherapeuteEdition_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        InitialiserToolTips()

    End Sub

#End Region

#Region "Initialisation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserCombos
    ' Version   : V1.0.0
    ' Date      : 16/06/2026
    '
    ' Rôle      :
    ' Alimente le combo des pays.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserCombos()

        cboPays.Items.Clear()
        cboPays.Items.AddRange(UtilsTelephone.LibellesPays().ToArray())

    End Sub

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
    ' Date      : 16/06/2026
    '
    ' Rôle      :
    ' Applique le titre selon le mode et remplit les champs en modification.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserFenetre()

        If _creation Then

            Text = "Nouveau thérapeute"
            lblTitreForm.Text = "Nouveau thérapeute"
            SelectionnerPays(UtilsTelephone.LibellePaysParDefaut)
            chkActif.Checked = True

        Else

            Text = "Modifier le thérapeute"
            lblTitreForm.Text = "Modifier le thérapeute"
            RemplirChamps(_therapeute)

        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplirChamps
    ' Version   : V1.0.0
    ' Date      : 17/06/2026
    '
    ' Rôle      :
    ' Renseigne les contrôles depuis un thérapeute existant.
    '
    ' Paramètres :
    ' - therapeute : Thérapeute source
    ' -------------------------------------------------------------------------------------------------
    Private Sub RemplirChamps(therapeute As Therapeute)

        txtNom.Text = therapeute.Nom
        txtPrenom.Text = therapeute.Prenom
        ' TODO (idée à creuser) : si la spécialité passe en table (référentiel), remplacer txtSpecialite
        ' par un combo type-ahead + bouton [+]. Voir Docs\Todo\ToDo.md § Thérapeutes.
        txtSpecialite.Text = therapeute.Specialite
        chkActif.Checked = therapeute.Actif

        SelectionnerPays(therapeute.Pays)
        txtTelephone.Text = UtilsTelephone.FormaterAffichage(therapeute.Telephone, therapeute.Pays)
        txtEmail.Text = therapeute.Email

        txtAdresseLigne1.Text = therapeute.AdresseLigne1
        txtAdresseLigne2.Text = therapeute.AdresseLigne2
        txtCodePostal.Text = therapeute.CodePostal
        txtLocalite.Text = therapeute.Localite

        rteCommentaire.ChargerContenu(therapeute.Commentaire_rtf, therapeute.Commentaire_txt)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SelectionnerPays
    ' Version   : V1.0.0
    ' Date      : 16/06/2026
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
    ' Date     : 16/06/2026
    '
    ' Rôle     :
    ' Valide les champs obligatoires et les formats (téléphone/e-mail) avant enregistrement,
    ' puis vérifie l'unicité du couple nom + prénom.
    '
    ' Retour   :
    ' - Boolean : True si la saisie est valide, False sinon (ErrorProvider + focus du 1er fautif)
    ' -------------------------------------------------------------------------------------------------
    Private Function ValiderSaisie() As Boolean

        errProvider.Clear()

        Dim premierChampFautif As Control = Nothing

        ' Nom (obligatoire)
        If String.IsNullOrWhiteSpace(txtNom.Text) Then
            errProvider.SetError(txtNom, "Le nom est obligatoire.")
            If premierChampFautif Is Nothing Then premierChampFautif = txtNom
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

        ' Unicité du couple nom + prénom (exclut l'enregistrement courant en modification)
        Dim idExclu As Long = If(_therapeute IsNot Nothing, _therapeute.IdTherapeute, 0L)

        If GestionTherapeutes.TherapeuteExiste(txtNom.Text, txtPrenom.Text, idExclu) Then
            errProvider.SetError(txtNom, "Un thérapeute portant ce nom et ce prénom existe déjà.")
            txtNom.Focus()
            Return False
        End If

        Return True

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Enregistrer
    ' Version   : V1.0.0
    ' Date      : 16/06/2026
    '
    ' Rôle      :
    ' Valide la saisie, construit le thérapeute et le persiste (création ou mise à jour),
    ' puis ferme la fenêtre avec DialogResult.OK.
    ' -------------------------------------------------------------------------------------------------
    Private Sub Enregistrer()

        If Not ValiderSaisie() Then
            Return
        End If

        Try

            Dim therapeute As Therapeute = If(_therapeute, New Therapeute())

            therapeute.Nom = txtNom.Text.Trim()
            therapeute.Prenom = NormaliserTexte(txtPrenom.Text)
            therapeute.Specialite = NormaliserTexte(txtSpecialite.Text)
            therapeute.Actif = chkActif.Checked
            therapeute.Pays = NormaliserTexte(cboPays.Text)
            therapeute.Telephone = NormaliserTexte(UtilsTelephone.NormaliserE164(txtTelephone.Text, cboPays.Text))
            therapeute.Email = NormaliserTexte(txtEmail.Text)
            therapeute.AdresseLigne1 = NormaliserTexte(txtAdresseLigne1.Text)
            therapeute.AdresseLigne2 = NormaliserTexte(txtAdresseLigne2.Text)
            therapeute.CodePostal = NormaliserTexte(txtCodePostal.Text)
            therapeute.Localite = NormaliserTexte(txtLocalite.Text)

            therapeute.Commentaire_rtf = NormaliserTexte(rteCommentaire.RtfContent)
            therapeute.Commentaire_txt = NormaliserTexte(rteCommentaire.TextContent)

            If _creation Then
                GestionTherapeutes.CreateTherapeute(therapeute)
            Else
                GestionTherapeutes.UpdateTherapeute(therapeute)
            End If

            _therapeute = therapeute

            _context?.SetStatus(If(_creation, "Thérapeute créé.", "Thérapeute mis à jour."))

            DialogResult = DialogResult.OK
            Close()

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

        If saisie.Length = 0 Then
            Return
        End If

        txtTelephone.Text = UtilsTelephone.FormaterAffichage(saisie, cboPays.Text)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : cboPays_SelectedIndexChanged
    ' Version   : V1.0.0
    ' Date      : 16/06/2026

    ' Rôle      : Reformate le téléphone selon le nouveau pays sélectionné.
    ' -------------------------------------------------------------------------------------------------
    Private Sub cboPays_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboPays.SelectedIndexChanged

        Dim saisie As String = txtTelephone.Text.Trim()

        If saisie.Length = 0 Then
            Return
        End If

        txtTelephone.Text = UtilsTelephone.FormaterAffichage(saisie, cboPays.Text)

    End Sub

#End Region

End Class
