' -------------------------------------------------------------------------------------------------
' UserControl : UC_PatientFiche
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 12/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Fiche patient plein panneau : bandeau identité persistant (photo + alerte) et onglets
' (Identité, Famille/Contacts, Intervenants, Dossiers). Permet la consultation, la création
' et la modification d'un patient (D-Q11, D-Q12, D-Q13, D-Q14).
'
' Responsabilités :
' - Afficher l'identité, les coordonnées et les données administratives d'un patient
' - Orchestrer les modes Consultation / Création / Modification et l'état des contrôles
' - Activer progressivement les onglets dépendant de l'id_patient (D-Q14)
' - Valider les saisies (champs requis, unicité NISS, détection de doublon)
' - Enregistrer le patient via GestionPatients (création / mise à jour)
' - Afficher le bandeau identité (photo via CheminsPatientHelper, nom complet, code, alerte)
' - Permettre le retour vers l'écran précédent (mini-pile de navigation, D-Q15)
'
' Remarques   :
' - Implémente IContextAwareUserControl (injection du contexte UI partagé par Home)
' - Aucun accès direct à la base de données : tout passe par GestionPatients
' - Pour cette première brique, seul l'onglet Identité est fonctionnel ; les onglets
'   Famille/Contacts, Intervenants et Dossiers affichent un message « à venir » et restent
'   désactivés tant qu'aucun id_patient n'est obtenu (activation progressive)
'
' Dépendances :
' - GestionPatients (couche métier patients)
' - GestionSituationsFamiliales (alimentation du combo situation familiale)
' - Patient (modèle métier)
' - CheminsPatientHelper (chemin déterministe de la photo)
' - UserControlContext / IContextAwareUserControl (contexte UI partagé)
' - UtilsButtons (style des boutons standard)
' - DialogChoix (boîtes de dialogue, en remplacement de MessageBox)
' - GestionLog (journalisation)
'
' Interface   :
' - IContextAwareUserControl : injection du contexte UI partagé
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.IO

Public Class UC_PatientFiche
    Implements IContextAwareUserControl

#Region "Types"

    ' -------------------------------------------------------------------------------------------------
    ' Énumération : ModeFiche
    ' Rôle        : Décrit le mode courant de la fiche patient (pattern UC_ReferentielBase, D-Q14).
    ' -------------------------------------------------------------------------------------------------
    Public Enum ModeFiche
        Consultation
        Creation
        Modification
    End Enum

#End Region

#Region "Variables privées"

    ' Injecté par Home via SetContext() (implémentation IContextAwareUserControl)
    Private _context As UserControlContext

    ' Patient actuellement chargé dans la fiche (Nothing tant qu'aucun patient n'est chargé en création)
    Private _patient As Patient

    ' Mode courant de la fiche (consultation par défaut)
    Private _mode As ModeFiche = ModeFiche.Consultation

    ' Identifiant du patient en cours d'édition (0 = création, aucun id encore attribué)
    Private _idPatient As Long = 0

    ' Garde anti-réentrance lors du remplissage programmatique des contrôles
    Private _chargementEnCours As Boolean = False

    ' Indique que l'onglet Anamnèse est en édition locale (indépendante du mode fiche global).
    ' L'anamnèse possède son propre cycle Modifier / Enregistrer / Annuler avec persistance ciblée.
    Private _modeEditionAnamnese As Boolean = False

    ' Suivi des modifications de l'anamnèse pendant l'édition locale (dirty-tracking) : permet de
    ' n'avertir l'utilisateur d'une perte potentielle que si le contenu a réellement été modifié.
    Private _anamneseModifiee As Boolean = False

    ' Extensions d'images acceptées à l'upload (formats décodables par PictureBox / GDI+).
    Private Shared ReadOnly ExtensionsPhotoAutorisees As String() =
        {".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff"}

    ' Liste complète des contacts du patient courant (source mémoire pour la recherche rapide)
    Private _contacts As List(Of FamilleContact)

    ' Liste complète des intervenants du patient courant (source mémoire pour la recherche rapide)
    Private _intervenants As List(Of SuiviIntervenant)

    ' Pays proposé par défaut à la création d'un patient (cabinet situé en Belgique)
    Private Const PaysParDefaut As String = "Belgique"

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SetContext
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Reçoit le contexte UI partagé injecté par Home.
    '
    ' Paramètres :
    ' - context : Contexte UI partagé de l'application.
    '
    ' Remarques :
    ' - Obligatoire pour accéder au Status, ToolTip, ErrorProvider et contexte Home.
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(context As UserControlContext) _
        Implements IContextAwareUserControl.SetContext

        _context = context

        ' Propage le contexte aux éditeurs riches (infobulles), Home n'injectant que le contrôle racine.
        rteAlerte?.SetContext(context)
        rteAnamnese?.SetContext(context)

    End Sub

#End Region

#Region "Initialisation publique"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserPourCreation
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Prépare la fiche pour la création d'un nouveau patient.
    '
    ' Remarques :
    ' - À appeler par l'appelant (UC_PatientHome) avant la navigation.
    ' - Le chargement effectif a lieu au Load du UserControl.
    ' -------------------------------------------------------------------------------------------------
    Public Sub InitialiserPourCreation()

        _idPatient = 0
        _mode = ModeFiche.Creation

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserPourPatient
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Prépare la fiche pour la consultation ou la modification d'un patient existant.
    '
    ' Paramètres :
    ' - idPatient : Identifiant du patient à charger
    ' - enModification : True pour ouvrir directement en modification, False pour consultation
    '
    ' Remarques :
    ' - À appeler par l'appelant (UC_PatientHome) avant la navigation.
    ' - Le chargement effectif a lieu au Load du UserControl.
    ' -------------------------------------------------------------------------------------------------
    Public Sub InitialiserPourPatient(idPatient As Long, enModification As Boolean)

        _idPatient = idPatient
        _mode = If(enModification, ModeFiche.Modification, ModeFiche.Consultation)

    End Sub

#End Region

#Region "Chargement UserControl"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : UC_PatientFiche_Load
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Initialise le UserControl : boutons, combos, infobulles, puis charge le patient selon le mode.
    '
    ' Remarques :
    ' - Aucune logique métier ni accès direct à la base de données.
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_PatientFiche_Load(
        sender As Object,
        e As EventArgs
    ) Handles MyBase.Load

        InitialiserBoutons()
        InitialiserCombos()
        InitialiserGrilleContacts()
        InitialiserGrilleIntervenants()
        InitialiserToolTips()

        If _mode = ModeFiche.Creation Then
            DemarrerCreation()
        Else
            ChargerPatient()
        End If

    End Sub

#End Region

#Region "Initialisation UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserBoutons
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Initialise les boutons standards du panneau d'actions.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserBoutons()

        UtilsButtons.InitStandardButton(btnNouveau)
        UtilsButtons.InitStandardButton(btnModifier)
        UtilsButtons.InitStandardButton(btnEnregistrer)
        UtilsButtons.InitStandardButton(btnAnnuler)
        UtilsButtons.InitStandardButton(btnFermer)
        UtilsButtons.InitStandardButton(btnReinitialiserContacts)

        UtilsButtons.InitStandardButton(btnModifierAnamnese)
        UtilsButtons.InitStandardButton(btnEnregistrerAnamnese)
        UtilsButtons.InitStandardButton(btnAnnulerAnamnese)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserCombos
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Alimente les listes déroulantes (latéralité, situation familiale).
    '
    ' Remarques :
    ' - La latéralité est une liste fixe (pas de référentiel dédié en V1).
    ' - La situation familiale provient du référentiel ref_situations_familiales (valeurs actives).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserCombos()

        cboLateralite.Items.Clear()
        cboLateralite.Items.AddRange(New Object() {"", "Droitier", "Gaucher", "Ambidextre"})

        cboPays.Items.Clear()
        cboPays.Items.AddRange(UtilsTelephone.LibellesPays().ToArray())

        Try

            LierSituationsFamiliales(ChargerSituationsFamilialesDepuisReferentiel(), Nothing)

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur chargement des situations familiales (UC_PatientFiche).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ChargerSituationsFamilialesDepuisReferentiel
    ' Version  : V1.0.0
    ' Date     : 13/06/2026
    '
    ' Rôle     :
    ' Charge les situations familiales actives depuis le référentiel et insère l'entrée vide
    ' en tête (Id = 0) pour permettre l'état « non renseigné » sur un combo lié par DataSource.
    '
    ' Retour   :
    ' - List(Of SituationFamiliale) : liste prête à lier (entrée vide incluse en tête)
    ' -------------------------------------------------------------------------------------------------
    Private Function ChargerSituationsFamilialesDepuisReferentiel() As List(Of SituationFamiliale)

        Dim situations As List(Of SituationFamiliale) =
            GestionSituationsFamiliales.GetSituationsFamilialesActives()

        ' Entrée vide en tête : permet l'état « non renseigné » sur un combo lié par
        ' DataSource (le CurrencyManager interdit une position -1 quand la liste n'est pas vide).
        situations.Insert(0, New SituationFamiliale With {
            .IdSituationFamiliale = 0,
            .LibelleSituationFamiliale = String.Empty
        })

        Return situations

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : LierSituationsFamiliales
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' (Re)lie le combo Situation familiale à la liste fournie, puis sélectionne la situation
    ' demandée (ou l'entrée vide à défaut).
    '
    ' Paramètres :
    ' - situations  : liste à lier (entrée vide en tête attendue)
    ' - idASelectionner : identifiant à resélectionner après liaison, ou Nothing pour l'entrée vide
    ' -------------------------------------------------------------------------------------------------
    Private Sub LierSituationsFamiliales(
        situations As List(Of SituationFamiliale),
        idASelectionner As ULong?
    )

        cboSituationFamiliale.DataSource = situations
        cboSituationFamiliale.DisplayMember = NameOf(SituationFamiliale.LibelleSituationFamiliale)
        cboSituationFamiliale.ValueMember = NameOf(SituationFamiliale.IdSituationFamiliale)

        Dim index As Integer = 0

        If idASelectionner.HasValue Then
            index = situations.FindIndex(
                Function(s) s.IdSituationFamiliale = idASelectionner.Value)
            If index < 0 Then
                index = 0
            End If
        End If

        cboSituationFamiliale.SelectedIndex = index

    End Sub

    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Initialise les aides contextuelles des boutons d'action.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserToolTips()

        If _context Is Nothing Then Exit Sub

        _context.SetToolTip(btnNouveau, "Créer un nouveau patient.")
        _context.SetToolTip(btnModifier, "Passer la fiche en modification.")
        _context.SetToolTip(btnEnregistrer, "Enregistrer les modifications.")
        _context.SetToolTip(btnAnnuler, "Annuler les modifications en cours.")
        _context.SetToolTip(btnFermer, "Revenir à la liste des patients (retour à l'écran précédent).")

        _context.SetToolTip(btnModifierAnamnese, "Modifier l'anamnèse du patient.")
        _context.SetToolTip(btnEnregistrerAnamnese, "Enregistrer uniquement l'anamnèse.")
        _context.SetToolTip(btnAnnulerAnamnese, "Annuler les modifications de l'anamnèse.")

        _context.SetToolTip(btnUploadPhoto,
            "Importer la photo d'identité du patient." & Environment.NewLine &
            "Formats acceptés : JPG, PNG, BMP, GIF, TIFF.")

        _context.SetToolTip(txtTelephone,
            "Saisissez le numéro simplement (ex. 0475 12 34 56)." & Environment.NewLine &
            "Il sera mis en forme automatiquement selon le pays indiqué.")

    End Sub

#End Region

#Region "Chargement des données"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DemarrerCreation
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Prépare la fiche vierge pour la création d'un nouveau patient.
    ' -------------------------------------------------------------------------------------------------
    Private Sub DemarrerCreation()

        _patient = New Patient()
        _idPatient = 0

        ViderChamps()
        AfficherBandeau()
        AppliquerMode(ModeFiche.Creation)
        RafraichirContexteNavigation()

        If _context IsNot Nothing Then
            _context.SetStatus("Création d'un nouveau patient.")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerPatient
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Charge le patient courant depuis la base via GestionPatients et alimente les contrôles.
    '
    ' Remarques :
    ' - En cas d'introuvable ou d'erreur, la fiche revient à un état neutre et informe l'utilisateur.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerPatient()

        Try
            Cursor = Cursors.WaitCursor

            _patient = GestionPatients.GetPatientById(_idPatient)

            If _patient Is Nothing Then

                If _context IsNot Nothing Then
                    _context.SetStatus("Patient introuvable.")
                End If

                DialogChoix.Erreur("Le patient demandé est introuvable.")
                Return

            End If

            RemplirChamps(_patient)
            AfficherBandeau()
            AppliquerMode(_mode)
            RafraichirContexteNavigation()

            If _context IsNot Nothing Then
                _context.SetStatus($"Patient chargé : {_patient.NomComplet}.")
            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur chargement du patient (UC_PatientFiche, id={_idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors du chargement du patient.")
            End If

            DialogChoix.Erreur("Une erreur est survenue lors du chargement du patient.")

        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplirChamps
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Recopie les propriétés d'un patient dans les contrôles de l'onglet Identité.
    '
    ' Paramètres :
    ' - patient : Patient source
    ' -------------------------------------------------------------------------------------------------
    Private Sub RemplirChamps(patient As Patient)

        _chargementEnCours = True

        Try

            txtNom.Text = If(patient.Nom, String.Empty)
            txtPrenom.Text = If(patient.Prenom, String.Empty)

            If patient.DateNaissance.HasValue Then
                dtpDateNaissance.Value = patient.DateNaissance.Value
                dtpDateNaissance.Checked = True
            Else
                dtpDateNaissance.Checked = False
            End If

            RafraichirAge()

            txtNiss.Text = If(patient.Niss, String.Empty)
            cboLateralite.Text = If(patient.Lateralite, String.Empty)

            txtAdresseLigne1.Text = If(patient.AdresseLigne1, String.Empty)
            txtAdresseLigne2.Text = If(patient.AdresseLigne2, String.Empty)
            txtCodePostal.Text = If(patient.CodePostal, String.Empty)
            txtLocalite.Text = If(patient.Localite, String.Empty)
            SelectionnerPays(patient.Pays)
            txtTelephone.Text = UtilsTelephone.FormaterAffichage(patient.Telephone, patient.Pays)
            txtEmail.Text = If(patient.Email, String.Empty)

            txtMutualite.Text = If(patient.Mutualite, String.Empty)

            If patient.IdSituationFamiliale.HasValue Then
                cboSituationFamiliale.SelectedValue = CULng(patient.IdSituationFamiliale.Value)
            Else
                ReinitialiserSituationFamiliale()
            End If

            rteAlerte.ChargerContenu(patient.AlerteRtf, patient.AlerteTxt)
            rteAnamnese.ChargerContenu(patient.AnamneseRtf, patient.AnamneseTxt)

        Finally
            _chargementEnCours = False
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ViderChamps
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Réinitialise tous les contrôles de saisie de l'onglet Identité.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ViderChamps()

        _chargementEnCours = True

        Try

            txtNom.Clear()
            txtPrenom.Clear()
            dtpDateNaissance.Value = Date.Today
            dtpDateNaissance.Checked = False
            RafraichirAge()
            txtNiss.Clear()
            cboLateralite.SelectedIndex = -1
            cboLateralite.Text = String.Empty

            txtAdresseLigne1.Clear()
            txtAdresseLigne2.Clear()
            txtCodePostal.Clear()
            txtLocalite.Clear()
            SelectionnerPays(PaysParDefaut)
            txtTelephone.Clear()
            txtEmail.Clear()

            txtMutualite.Clear()
            ReinitialiserSituationFamiliale()

            rteAlerte.RtfContent = String.Empty
            rteAnamnese.RtfContent = String.Empty

        Finally
            _chargementEnCours = False
        End Try

        _context?.ClearAllErrors()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SelectionnerPays
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Sélectionne le pays dans la liste déroulante à partir d'un libellé (canonique ou variante).
    ' À défaut de correspondance, sélectionne le pays par défaut (Belgique).
    '
    ' Paramètres :
    ' - libellePays : Libellé du pays (peut être Nothing, ancien ou localisé).
    ' -------------------------------------------------------------------------------------------------
    Private Sub SelectionnerPays(libellePays As String)

        Dim canonique As String = UtilsTelephone.NormaliserLibellePays(libellePays)

        If canonique Is Nothing Then
            canonique = UtilsTelephone.LibellePaysParDefaut
        End If

        cboPays.SelectedItem = canonique

        If cboPays.SelectedIndex < 0 AndAlso cboPays.Items.Count > 0 Then
            cboPays.SelectedIndex = 0
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ReinitialiserSituationFamiliale
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Vide réellement la sélection du combo Situation familiale (lié par DataSource).
    '
    ' Remarques :
    ' - Le combo est lié par DataSource et contient une entrée vide en tête (Id = 0).
    '   Sélectionner cette entrée (index 0) affiche l'état « non renseigné » sans déclencher
    '   la resynchronisation du CurrencyManager (qui ramènerait la position à 0 sur une tentative -1).
    ' -------------------------------------------------------------------------------------------------
    Private Sub ReinitialiserSituationFamiliale()

        If cboSituationFamiliale.Items.Count > 0 Then
            cboSituationFamiliale.SelectedIndex = 0
        End If

    End Sub

#End Region

#Region "Bandeau identité"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherBandeau
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Met à jour le bandeau identité persistant : nom complet, code, photo et alerte (D-Q12, D-Q13).
    '
    ' Remarques :
    ' - En création, le bandeau affiche un libellé générique et le placeholder de photo.
    ' - La photo est chargée depuis le chemin déterministe (CheminsPatientHelper) si elle existe.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherBandeau()

        If _patient Is Nothing OrElse _idPatient <= 0 Then
            lblNomComplet.Text = "Nouveau patient"
            lblCodePatient.Text = "-"
            AfficherAlerte(Nothing, Nothing)
            AfficherPhoto(Nothing)
            Return
        End If

        lblNomComplet.Text = If(String.IsNullOrWhiteSpace(_patient.NomComplet), "Patient", _patient.NomComplet)
        lblCodePatient.Text = If(_patient.CodePatient, "-")

        AfficherAlerte(_patient.AlerteRtf, _patient.AlerteTxt)
        AfficherPhoto(_patient.PhotoFichier)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherAlerte
    ' Version   : V2.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Affiche les notes/alerte du patient dans le bandeau en conservant le formatage RTF (D-Q12).
    ' À défaut de contenu, affiche un libellé neutre.
    '
    ' Paramètres :
    ' - alerteRtf : Contenu RTF de l'alerte (peut être Nothing)
    ' - alerteTxt : Texte brut de l'alerte (repli si le RTF est absent ; peut être Nothing)
    '
    ' Remarques :
    ' - rtbAlerte est en lecture seule, avec ascenseur vertical si le texte dépasse.
    ' - Pas de coloration "rouge" : il s'agit avant tout de notes pour le thérapeute.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherAlerte(alerteRtf As String, alerteTxt As String)

        If String.IsNullOrWhiteSpace(alerteRtf) AndAlso String.IsNullOrWhiteSpace(alerteTxt) Then
            rtbAlerte.Clear()
            rtbAlerte.Text = "Aucune note."
            rtbAlerte.SelectAll()
            rtbAlerte.SelectionColor = Color.FromArgb(150, 150, 150)
            rtbAlerte.SelectionStart = 0
            rtbAlerte.SelectionLength = 0
            Return
        End If

        RichTextEditorHelper.ChargerContenu(rtbAlerte, alerteRtf, alerteTxt)
        rtbAlerte.SelectionStart = 0
        rtbAlerte.SelectionLength = 0

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherPhoto
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Charge la photo d'identité dans le bandeau depuis son chemin déterministe (D-Q13).
    '
    ' Paramètres :
    ' - photoFichier : Nom seul du fichier photo (peut être Nothing)
    '
    ' Remarques :
    ' - La photo est lue sans verrouiller le fichier (copie en mémoire) pour permettre son remplacement.
    ' - En l'absence de photo (ou en cas d'erreur), un placeholder neutre est affiché.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherPhoto(photoFichier As String)

        LibererPhoto()

        If _patient Is Nothing OrElse String.IsNullOrWhiteSpace(_patient.CodePatient) OrElse String.IsNullOrWhiteSpace(photoFichier) Then
            Return
        End If

        Try

            Dim chemin As String = CheminsPatientHelper.GetCheminFichierPatient(_patient.CodePatient, photoFichier)

            If Not String.IsNullOrEmpty(chemin) AndAlso File.Exists(chemin) Then

                Using fs As New FileStream(chemin, FileMode.Open, FileAccess.Read)
                    Using img As Image = Image.FromStream(fs)
                        picPhoto.Image = New Bitmap(img)
                    End Using
                End Using

            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur chargement de la photo patient (UC_PatientFiche, id={_idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : LibererPhoto
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Libère l'image courante du bandeau (évite les fuites de handles GDI+).
    ' -------------------------------------------------------------------------------------------------
    Private Sub LibererPhoto()

        If picPhoto.Image IsNot Nothing Then
            Dim ancienne As Image = picPhoto.Image
            picPhoto.Image = Nothing
            ancienne.Dispose()
        End If

    End Sub

#End Region

#Region "Gestion des modes"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerMode
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Applique le mode courant : état lecture seule / éditable des contrôles, visibilité des boutons,
    ' et activation progressive des onglets dépendant de l'id_patient (D-Q14).
    '
    ' Paramètres :
    ' - mode : Mode à appliquer
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerMode(mode As ModeFiche)

        _mode = mode

        Dim editable As Boolean = mode = ModeFiche.Creation OrElse mode = ModeFiche.Modification

        DefinirEtatChampsIdentite(editable)

        ' Actions « niveau fiche » : visibles uniquement sur l'onglet Identité, selon le mode courant.
        AppliquerVisibiliteActionsFiche()

        ' Modifier n'est possible que sur un patient déjà enregistré.
        btnModifier.Enabled = _idPatient > 0

        ' Activation progressive : les onglets dépendant de l'id ne sont actifs qu'avec un patient enregistré (D-Q14).
        Dim patientEnregistre As Boolean = _idPatient > 0
        tabPageFamille.Enabled = patientEnregistre
        tabPageIntervenants.Enabled = patientEnregistre
        tabPageDossiers.Enabled = patientEnregistre

        ' L'upload de la photo nécessite un patient déjà enregistré (code patient et dossier disponibles).
        btnUploadPhoto.Enabled = patientEnregistre

        ' Un changement de mode global réinitialise systématiquement l'édition locale de l'anamnèse,
        ' afin d'éviter tout état incohérent (ex. édition locale active alors que la fiche passe en modification).
        _modeEditionAnamnese = False
        _anamneseModifiee = False
        AppliquerModeAnamnese()

        ' En création, on démarre la saisie sur l'onglet Identité.
        ' En modification, on conserve l'onglet courant (l'utilisateur édite ce qu'il consulte).
        If mode = ModeFiche.Creation Then
            tabFiche.SelectedTab = tabPageIdentite
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DefinirEtatChampsIdentite
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Active ou désactive l'édition des contrôles de l'onglet Identité.
    '
    ' Paramètres :
    ' - editable : True pour autoriser la saisie, False pour la lecture seule
    ' -------------------------------------------------------------------------------------------------
    Private Sub DefinirEtatChampsIdentite(editable As Boolean)

        txtNom.ReadOnly = Not editable
        txtPrenom.ReadOnly = Not editable
        txtNiss.ReadOnly = Not editable
        txtAdresseLigne1.ReadOnly = Not editable
        txtAdresseLigne2.ReadOnly = Not editable
        txtCodePostal.ReadOnly = Not editable
        txtLocalite.ReadOnly = Not editable
        txtTelephone.ReadOnly = Not editable
        txtEmail.ReadOnly = Not editable
        txtMutualite.ReadOnly = Not editable

        dtpDateNaissance.Enabled = editable
        cboLateralite.Enabled = editable
        cboPays.Enabled = editable
        cboSituationFamiliale.Enabled = editable

        ' Le bouton d'ajout de référentiel n'est proposé qu'en saisie (Création / Modification).
        btnAjouterSituationFamiliale.Enabled = editable

        rteAlerte.ReadOnlyMode = Not editable

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : tabFiche_Selecting
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Empêche la sélection des onglets dépendant de l'id_patient tant que le patient n'est pas
    ' enregistré (activation progressive D-Q14).
    '
    ' Remarques :
    ' - En WinForms, TabPage.Enabled = False grise le contenu mais ne bloque PAS le clic sur l'onglet :
    '   il faut annuler la sélection ici (e.Cancel) pour un vrai verrouillage.
    ' -------------------------------------------------------------------------------------------------
    Private Sub tabFiche_Selecting(
        sender As Object,
        e As TabControlCancelEventArgs
    ) Handles tabFiche.Selecting

        If e.TabPage Is Nothing Then
            Return
        End If

        Dim ongletDependantId As Boolean =
            e.TabPage Is tabPageFamille OrElse
            e.TabPage Is tabPageIntervenants OrElse
            e.TabPage Is tabPageDossiers

        If ongletDependantId AndAlso _idPatient <= 0 Then

            e.Cancel = True

            If _context IsNot Nothing Then
                _context.SetStatus("Enregistrez d'abord le patient pour accéder à cet onglet.")
            End If

        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : tabFiche_SelectedIndexChanged
    ' Version   : V1.0.0
    ' Date      : 18/06/2026
    '
    ' Rôle      :
    ' Met à jour le fil d'Ariane de navigation (lblContexte de Home) à chaque changement d'onglet,
    ' afin que l'utilisateur sache toujours où il se trouve (ex. « Patients > Dupont Jean > Anamnèse »).
    '
    ' Remarques :
    ' - Aligné sur le comportement de UC_Parametres (suivi du contexte au déplacement entre onglets).
    ' -------------------------------------------------------------------------------------------------
    Private Sub tabFiche_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles tabFiche.SelectedIndexChanged

        RafraichirContexteNavigation()
        AppliquerVisibiliteActionsFiche()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RafraichirContexteNavigation
    ' Version   : V1.0.0
    ' Date      : 18/06/2026
    '
    ' Rôle      :
    ' (Re)construit le fil d'Ariane affiché dans lblContexte (header de Home) au format
    ' « Patients > {patient} > {onglet} », pour refléter en continu l'emplacement de l'utilisateur.
    '
    ' Remarques :
    ' - Le segment patient vaut « Nouveau patient » tant qu'aucun patient n'est enregistré.
    ' - Le segment onglet provient du libellé de l'onglet actif (tabFiche.SelectedTab.Text).
    ' - N'écrit pas dans la barre de statut (stsStatus) : celle-ci porte les messages d'action.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RafraichirContexteNavigation()

        If _context Is Nothing Then
            Return
        End If

        Dim segmentPatient As String =
            If(_patient IsNot Nothing AndAlso _idPatient > 0 AndAlso Not String.IsNullOrWhiteSpace(_patient.NomComplet),
               _patient.NomComplet,
               "Nouveau patient")

        Dim segmentOnglet As String =
            If(tabFiche.SelectedTab IsNot Nothing, tabFiche.SelectedTab.Text, String.Empty)

        Dim contexte As String = $"Patients > {segmentPatient}"

        If Not String.IsNullOrWhiteSpace(segmentOnglet) Then
            contexte &= $" > {segmentOnglet}"
        End If

        _context.SetHeader(contexte)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerVisibiliteActionsFiche
    ' Version   : V1.0.0
    ' Date      : 18/06/2026
    '
    ' Rôle      :
    ' Affiche les boutons d'action « niveau fiche » (Nouveau / Modifier / Enregistrer / Annuler)
    ' uniquement sur l'onglet Identité, qui est la fiche maître. Sur les autres onglets, seules les
    ' actions propres à l'onglet (Ajouter/Modifier/Supprimer un contact, etc.) restent visibles,
    ' afin d'éviter la confusion d'un double panneau d'actions (règle user-friendly).
    '
    ' Remarques :
    ' - Le bouton « Retour liste » (btnFermer) reste visible sur tous les onglets.
    ' - La visibilité dépend à la fois de l'onglet actif et du mode courant (consultation/édition).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerVisibiliteActionsFiche()

        Dim surIdentite As Boolean = tabFiche.SelectedTab Is tabPageIdentite
        Dim editable As Boolean = _mode = ModeFiche.Creation OrElse _mode = ModeFiche.Modification

        ' Nouveau / Modifier : uniquement sur l'onglet Identité, en consultation.
        btnNouveau.Visible = surIdentite AndAlso _mode = ModeFiche.Consultation
        btnModifier.Visible = surIdentite AndAlso _mode = ModeFiche.Consultation

        ' Enregistrer / Annuler : uniquement sur l'onglet Identité, en édition.
        btnEnregistrer.Visible = surIdentite AndAlso editable
        btnAnnuler.Visible = surIdentite AndAlso editable

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerModeAnamnese
    ' Version   : V1.0.0
    ' Date      : 18/06/2026
    '
    ' Rôle      :
    ' Applique l'état de l'onglet Anamnèse selon son cycle d'édition local (Modifier / Enregistrer /
    ' Annuler), indépendant du mode global de la fiche. L'éditeur passe en lecture seule ou éditable,
    ' et les boutons du panneau local sont affichés en conséquence.
    '
    ' Remarques :
    ' - L'édition locale n'est proposée que sur un patient déjà enregistré et en mode Consultation.
    '   En Création / Modification globale, l'anamnèse est éditable directement (flux global), et le
    '   panneau local (Modifier / Enregistrer / Annuler) reste masqué.
    ' - btnModifier (édition) et btnEnregistrer (sauvegarde) ne sont jamais visibles simultanément.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerModeAnamnese()

        ' En Création / Modification globale, l'anamnèse est éditable directement (flux global),
        ' sans bouton local : elle est enregistrée avec le reste de la fiche via btnEnregistrer.
        Dim editionGlobale As Boolean =
            _mode = ModeFiche.Creation OrElse _mode = ModeFiche.Modification

        ' L'édition locale n'a de sens qu'en consultation d'un patient déjà enregistré.
        Dim editionLocaleDisponible As Boolean =
            _idPatient > 0 AndAlso _mode = ModeFiche.Consultation

        rteAnamnese.ReadOnlyMode =
            Not (editionGlobale OrElse (editionLocaleDisponible AndAlso _modeEditionAnamnese))

        ' Modifier : proposé hors édition, quand l'édition locale est disponible.
        btnModifierAnamnese.Visible = editionLocaleDisponible AndAlso Not _modeEditionAnamnese

        ' Enregistrer / Annuler : proposés uniquement pendant l'édition locale.
        btnEnregistrerAnamnese.Visible = editionLocaleDisponible AndAlso _modeEditionAnamnese
        btnAnnulerAnamnese.Visible = editionLocaleDisponible AndAlso _modeEditionAnamnese

    End Sub

#End Region

#Region "Âge"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dtpDateNaissance_ValueChanged
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Recalcule l'âge affiché lorsque la date de naissance change (ou que la case est cochée/décochée).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dtpDateNaissance_ValueChanged(
        sender As Object,
        e As EventArgs
    ) Handles dtpDateNaissance.ValueChanged

        RafraichirAge()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RafraichirAge
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Met à jour le libellé d'âge à partir de la date de naissance saisie (vide si non renseignée).
    ' -------------------------------------------------------------------------------------------------
    Private Sub RafraichirAge()

        Dim dateNaissance As Date? = SaisieDateNaissance()

        If Not dateNaissance.HasValue Then
            lblAge.Text = String.Empty
            Return
        End If

        Dim age As Integer = CalculerAge(dateNaissance.Value, Date.Today)

        If age < 0 Then
            lblAge.Text = String.Empty
        ElseIf age = 0 Then
            lblAge.Text = "(moins d'un an)"
        ElseIf age = 1 Then
            lblAge.Text = "(1 an)"
        Else
            lblAge.Text = $"({age} ans)"
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CalculerAge
    ' Version  : V1.0.0
    ' Date     : 12/06/2026
    '
    ' Rôle     :
    ' Calcule l'âge en années révolues à une date de référence donnée.
    '
    ' Paramètres :
    ' - dateNaissance : Date de naissance
    ' - reference     : Date de référence (généralement aujourd'hui)
    '
    ' Retour   :
    ' - Integer : Âge en années révolues (peut être négatif si date future)
    ' -------------------------------------------------------------------------------------------------
    Private Function CalculerAge(dateNaissance As Date, reference As Date) As Integer

        Dim age As Integer = reference.Year - dateNaissance.Year

        If dateNaissance.Date > reference.AddYears(-age) Then
            age -= 1
        End If

        Return age

    End Function

#End Region

#Region "Validation et sauvegarde"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ValiderSaisie
    ' Version  : V1.1.0
    ' Date     : 12/06/2026
    '
    ' Rôle     :
    ' Valide les champs obligatoires, l'unicité du NISS, et le format du téléphone et de l'e-mail
    ' avant enregistrement. Toutes les erreurs sont bloquantes.
    '
    ' Retour   :
    ' - Boolean : True si la saisie est valide, False sinon (ErrorProvider + message explicite dans le statut)
    '
    ' Remarques :
    ' - Chaque champ invalide est signalé via l'ErrorProvider ; un message récapitulatif explicite
    '   est affiché dans le statut Home (convivialité), et le premier champ fautif reçoit le focus.
    ' -------------------------------------------------------------------------------------------------
    Private Function ValiderSaisie() As Boolean

        _context?.ClearAllErrors()

        Dim erreurs As New List(Of String)
        Dim premierChampFautif As Control = Nothing

        ' Nom (obligatoire)
        If String.IsNullOrWhiteSpace(txtNom.Text) Then
            _context?.SetError(txtNom, "Le nom est obligatoire.")
            erreurs.Add("le nom est obligatoire")
            If premierChampFautif Is Nothing Then premierChampFautif = txtNom
        End If

        ' Prénom (obligatoire)
        If String.IsNullOrWhiteSpace(txtPrenom.Text) Then
            _context?.SetError(txtPrenom, "Le prénom est obligatoire.")
            erreurs.Add("le prénom est obligatoire")
            If premierChampFautif Is Nothing Then premierChampFautif = txtPrenom
        End If

        ' Téléphone (format souple, si renseigné)
        Dim messageTel As String = String.Empty
        If Not UtilsValidation.IsValidTelephone(txtTelephone.Text, messageTel) Then
            _context?.SetError(txtTelephone, messageTel)
            erreurs.Add("le téléphone est invalide")
            If premierChampFautif Is Nothing Then premierChampFautif = txtTelephone
        End If

        ' E-mail (format, si renseigné)
        Dim messageEmail As String = String.Empty
        If Not UtilsValidation.IsValidEmail(txtEmail.Text, messageEmail) Then
            _context?.SetError(txtEmail, messageEmail)
            erreurs.Add("l'adresse e-mail est invalide")
            If premierChampFautif Is Nothing Then premierChampFautif = txtEmail
        End If

        ' Unicité du NISS (si renseigné)
        Dim niss As String = txtNiss.Text.Trim()
        If Not String.IsNullOrEmpty(niss) AndAlso GestionPatients.NissExiste(niss, _idPatient) Then
            _context?.SetError(txtNiss, "Ce NISS est déjà utilisé par un autre patient.")
            erreurs.Add("ce NISS est déjà utilisé par un autre patient")
            If premierChampFautif Is Nothing Then premierChampFautif = txtNiss
        End If

        If erreurs.Count = 0 Then
            Return True
        End If

        If _context IsNot Nothing Then
            _context.SetStatus("Veuillez corriger : " & String.Join(" ; ", erreurs) & ".")
        End If

        premierChampFautif?.Focus()

        Return False

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ConfirmerDoublonEventuel
    ' Version  : V1.0.0
    ' Date     : 12/06/2026
    '
    ' Rôle     :
    ' Détecte un doublon potentiel (nom + prénom + date de naissance) et demande confirmation.
    '
    ' Retour   :
    ' - Boolean : True si l'enregistrement peut se poursuivre, False si l'utilisateur renonce
    ' -------------------------------------------------------------------------------------------------
    Private Function ConfirmerDoublonEventuel() As Boolean

        Dim dateNaissance As Date? = SaisieDateNaissance()

        Dim doublon As Boolean = GestionPatients.DoublonExiste(
            txtNom.Text.Trim(),
            txtPrenom.Text.Trim(),
            dateNaissance,
            _idPatient
        )

        If Not doublon Then
            Return True
        End If

        Dim reponse As DialogResult = DialogChoix.Confirmer(
            "Un patient portant les mêmes nom, prénom et date de naissance existe déjà." & Environment.NewLine &
            "Voulez-vous quand même enregistrer ce patient ?",
            "Doublon potentiel"
        )

        Return reponse = DialogResult.Yes

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Enregistrer
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Valide la saisie, alimente le patient depuis les contrôles, puis crée ou met à jour le patient.
    '
    ' Remarques :
    ' - En création réussie, la fiche bascule en consultation sur le patient nouvellement créé.
    ' - Les onglets dépendant de l'id deviennent alors actifs (activation progressive D-Q14).
    ' -------------------------------------------------------------------------------------------------
    Private Sub Enregistrer()

        If Not ValiderSaisie() Then
            ' Le message explicite (champs fautifs) est déjà posé dans le statut par ValiderSaisie().
            Return
        End If

        If Not ConfirmerDoublonEventuel() Then
            If _context IsNot Nothing Then
                _context.SetStatus("Enregistrement annulé.")
            End If
            Return
        End If

        Try
            Cursor = Cursors.WaitCursor

            If _patient Is Nothing Then
                _patient = New Patient()
            End If

            AlimenterPatientDepuisChamps(_patient)

            If _mode = ModeFiche.Creation Then

                Dim nouvelId As Long = GestionPatients.CreatePatient(_patient)

                If nouvelId <= 0 Then
                    DialogChoix.Erreur("La création du patient a échoué.")
                    Return
                End If

                _idPatient = nouvelId
                _patient = GestionPatients.GetPatientById(_idPatient)

                DialogChoix.Succes("Le patient a été créé avec succès.")

            Else

                GestionPatients.UpdatePatient(_patient)
                _patient = GestionPatients.GetPatientById(_idPatient)

                DialogChoix.Succes("Les modifications ont été enregistrées.")

            End If

            RemplirChamps(_patient)
            AfficherBandeau()
            AppliquerMode(ModeFiche.Consultation)
            RafraichirContexteNavigation()

            If _context IsNot Nothing Then
                _context.SetStatus($"Patient enregistré : {_patient.NomComplet}.")
            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur enregistrement du patient (UC_PatientFiche, id={_idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur("Une erreur est survenue lors de l'enregistrement du patient.")

        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AlimenterPatientDepuisChamps
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Recopie les valeurs des contrôles de l'onglet Identité dans l'objet patient.
    '
    ' Paramètres :
    ' - patient : Patient cible à alimenter
    '
    ' Remarques :
    ' - Les champs vides sont normalisés à Nothing pour rester cohérents avec le stockage.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AlimenterPatientDepuisChamps(patient As Patient)

        patient.Nom = txtNom.Text.Trim()
        patient.Prenom = txtPrenom.Text.Trim()
        patient.DateNaissance = SaisieDateNaissance()
        patient.Niss = NormaliserTexte(txtNiss.Text)
        patient.Lateralite = NormaliserTexte(cboLateralite.Text)

        patient.AdresseLigne1 = NormaliserTexte(txtAdresseLigne1.Text)
        patient.AdresseLigne2 = NormaliserTexte(txtAdresseLigne2.Text)
        patient.CodePostal = NormaliserTexte(txtCodePostal.Text)
        patient.Localite = NormaliserTexte(txtLocalite.Text)
        patient.Pays = NormaliserTexte(cboPays.Text)
        patient.Telephone = NormaliserTexte(UtilsTelephone.NormaliserE164(txtTelephone.Text, cboPays.Text))
        patient.Email = NormaliserTexte(txtEmail.Text)

        patient.Mutualite = NormaliserTexte(txtMutualite.Text)
        patient.IdSituationFamiliale = SaisieSituationFamiliale()

        patient.AlerteRtf = NormaliserTexte(rteAlerte.RtfContent)
        patient.AlerteTxt = NormaliserTexte(rteAlerte.TextContent)

        patient.AnamneseRtf = NormaliserTexte(rteAnamnese.RtfContent)
        patient.AnamneseTxt = NormaliserTexte(rteAnamnese.TextContent)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : SaisieDateNaissance
    ' Version  : V1.0.0
    ' Date     : 12/06/2026
    '
    ' Rôle     :
    ' Retourne la date de naissance saisie, ou Nothing si la case du DateTimePicker est décochée.
    '
    ' Retour   :
    ' - Date? : Date de naissance ou Nothing
    ' -------------------------------------------------------------------------------------------------
    Private Function SaisieDateNaissance() As Date?

        If dtpDateNaissance.Checked Then
            Return dtpDateNaissance.Value.Date
        End If

        Return Nothing

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : SaisieSituationFamiliale
    ' Version  : V1.0.0
    ' Date     : 12/06/2026
    '
    ' Rôle     :
    ' Retourne l'identifiant de situation familiale sélectionné, ou Nothing si aucune sélection.
    '
    ' Retour   :
    ' - Long? : Identifiant de la situation familiale ou Nothing
    ' -------------------------------------------------------------------------------------------------
    Private Function SaisieSituationFamiliale() As Long?

        If cboSituationFamiliale.SelectedValue Is Nothing Then
            Return Nothing
        End If

        Dim idSituation As Long
        If Long.TryParse(cboSituationFamiliale.SelectedValue.ToString(), idSituation) AndAlso idSituation > 0 Then
            Return idSituation
        End If

        Return Nothing

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : NormaliserTexte
    ' Version  : V1.0.0
    ' Date     : 12/06/2026
    '
    ' Rôle     :
    ' Normalise une saisie texte : Nothing si vide après nettoyage, sinon la valeur nettoyée.
    '
    ' Paramètres :
    ' - valeur : Texte saisi
    '
    ' Retour   :
    ' - String : Texte nettoyé, ou Nothing si vide
    ' -------------------------------------------------------------------------------------------------
    Private Function NormaliserTexte(valeur As String) As String

        Dim nettoye As String = If(valeur, String.Empty).Trim()
        Return If(nettoye.Length = 0, Nothing, nettoye)

    End Function

#End Region

#Region "Événements UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : txtTelephone_Leave
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Met en forme le numéro de téléphone à la sortie du champ, en déduisant le pays du combo Pays
    ' (brique réutilisable UtilsTelephone). Reformatage souple et non bloquant.
    '
    ' Remarques :
    ' - Ignoré pendant le chargement programmatique des contrôles (anti-réentrance).
    ' - Si le pays est inconnu, le numéro n'est pas dénaturé (repli souple).
    ' -------------------------------------------------------------------------------------------------
    Private Sub txtTelephone_Leave(
        sender As Object,
        e As EventArgs
    ) Handles txtTelephone.Leave

        If _chargementEnCours Then
            Return
        End If

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
    '
    ' Rôle      :
    ' Reformate le numéro de téléphone lorsque le pays sélectionné change,
    ' afin de réappliquer l'indicatif et le groupement corrects du nouveau pays.
    '
    ' Remarques :
    ' - Ignoré pendant le chargement programmatique des contrôles (anti-réentrance).
    ' - Si le téléphone est vide, aucune action.
    ' -------------------------------------------------------------------------------------------------
    Private Sub cboPays_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles cboPays.SelectedIndexChanged

        If _chargementEnCours Then
            Return
        End If

        Dim saisie As String = txtTelephone.Text.Trim()

        If saisie.Length = 0 Then
            Return
        End If

        txtTelephone.Text = UtilsTelephone.FormaterAffichage(saisie, cboPays.Text)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnNouveau_Click
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Bascule la fiche en création d'un nouveau patient.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnNouveau_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnNouveau.Click

        DemarrerCreation()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnModifier_Click
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Passe la fiche du patient courant en mode modification.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnModifier_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnModifier.Click

        If _idPatient <= 0 Then
            Return
        End If

        AppliquerMode(ModeFiche.Modification)

        If _context IsNot Nothing Then
            _context.SetStatus("Modification de la fiche patient.")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnUploadPhoto_Click
    ' Version   : V1.0.0
    ' Date      : 14/06/2026
    '
    ' Rôle      :
    ' Importe la photo d'identité du patient : sélection libre par l'utilisateur, validation du
    ' format, copie déterministe dans le dossier du patient (nom fixe « identite.ext »), mise à jour
    ' du nom de fichier en base (patients.photo_fichier) et affichage immédiat dans le bandeau.
    '
    ' Remarques :
    ' - L'action n'est possible que sur un patient enregistré (bouton désactivé sinon).
    ' - Le dossier du patient est créé à la demande (AssurerDossierPatient).
    ' - La photo garde un nom fixe : une nouvelle photo remplace la précédente (D-Q13 / §8.2).
    ' - Les formats proposés et acceptés se limitent à ceux décodables par PictureBox / GDI+.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnUploadPhoto_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnUploadPhoto.Click

        If _patient Is Nothing OrElse _idPatient <= 0 OrElse String.IsNullOrWhiteSpace(_patient.CodePatient) Then
            Return
        End If

        Dim cheminSource As String = SelectionnerFichierPhoto()

        If String.IsNullOrEmpty(cheminSource) Then
            Return
        End If

        Dim extension As String = Path.GetExtension(cheminSource)

        If Not ExtensionPhotoEstAutorisee(extension) Then
            DialogChoix.Erreur(
                "Le format sélectionné n'est pas pris en charge." & Environment.NewLine &
                "Formats acceptés : JPG, PNG, BMP, GIF, TIFF.")
            Return
        End If

        Try

            ' Garantit l'existence du dossier patient avant toute écriture.
            CheminsPatientHelper.AssurerDossierPatient(_patient.CodePatient)

            ' Nom de fichier déterministe (identite + extension d'origine) et chemin cible.
            Dim nomFichier As String = CheminsPatientHelper.GetNomFichierPhotoIdentite(extension)
            Dim cheminCible As String = CheminsPatientHelper.GetCheminFichierPatient(_patient.CodePatient, nomFichier)

            ' Libère l'image courante avant la copie pour ne pas verrouiller le fichier cible.
            LibererPhoto()

            File.Copy(cheminSource, cheminCible, overwrite:=True)

            ' Met à jour le check en base (nom seul) puis l'objet patient en mémoire.
            GestionPatients.UpdatePhotoPatient(_idPatient, nomFichier)
            _patient.PhotoFichier = nomFichier

            ' Affiche la nouvelle photo dans le bandeau.
            AfficherPhoto(nomFichier)

            ' TODO : Traçabilité documents (brique Documents) — enregistrer ici une trace de la photo
            ' d'identité dans la table documents : nom de fichier (nomFichier), type/modèle de document
            ' (photo d'identité), code_patient, date, et NIVEAU = patient.
            ' Règle générale : tout document créé OU uploadé dans l'appli doit avoir une trace dans documents.
            ' Voir la décision niveau document (Enum publique vs type de paramètre) à trancher à ce moment.
            ' Ne pas oublier de revenir ici. Voir Docs\Todo\ToDo.md §D1 (Points différés).

            If _context IsNot Nothing Then
                _context.SetStatus("Photo d'identité mise à jour.")
            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur upload de la photo patient (UC_PatientFiche, id={_idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur("Une erreur est survenue lors de l'import de la photo.")

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : SelectionnerFichierPhoto
    ' Version  : V1.0.0
    ' Date     : 14/06/2026
    '
    ' Rôle     :
    ' Ouvre un sélecteur de fichier filtré sur les formats d'image acceptés et retourne le chemin
    ' choisi (ou Nothing si l'utilisateur annule).
    ' -------------------------------------------------------------------------------------------------
    Private Function SelectionnerFichierPhoto() As String

        Using dlg As New OpenFileDialog()

            dlg.Title = "Sélectionner la photo d'identité"
            dlg.Filter =
                "Images (*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tif;*.tiff)|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tif;*.tiff"
            dlg.CheckFileExists = True
            dlg.Multiselect = False

            If dlg.ShowDialog() = DialogResult.OK Then
                Return dlg.FileName
            End If

        End Using

        Return Nothing

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ExtensionPhotoEstAutorisee
    ' Version  : V1.0.0
    ' Date     : 14/06/2026
    '
    ' Rôle     :
    ' Valide (défensivement) que l'extension du fichier choisi fait partie des formats acceptés,
    ' indépendamment du filtre du dialogue (l'utilisateur peut forcer « Tous les fichiers »).
    '
    ' Paramètres :
    ' - extension : Extension du fichier (ex : ".png")
    ' -------------------------------------------------------------------------------------------------
    Private Shared Function ExtensionPhotoEstAutorisee(extension As String) As Boolean

        If String.IsNullOrWhiteSpace(extension) Then
            Return False
        End If

        Return Array.IndexOf(ExtensionsPhotoAutorisees, extension.Trim().ToLowerInvariant()) >= 0

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : rteAnamnese_ExportRequested
    ' Version   : V1.0.0
    ' Date      : 14/06/2026
    '
    ' Rôle      :
    ' Fournit (par délégation) le chemin de destination déterministe pour l'export de l'anamnèse :
    ' dossier du patient ({…}/Documents/{code_patient}) et nom horodaté
    ' (anamnese_{code}_{yyyyMMdd_HHmmss}.{ext}).
    '
    ' Remarques :
    ' - Uniquement pour un patient enregistré ; sinon on laisse le contrôle retomber sur son dialogue.
    ' - Le dossier patient est créé à la demande (AssurerDossierPatient) avant l'écriture.
    ' - Le contrôle reste générique : c'est le conteneur qui porte la règle de nommage métier.
    ' -------------------------------------------------------------------------------------------------
    Private Sub rteAnamnese_ExportRequested(
        sender As Object,
        e As ExportRequestedEventArgs
    ) Handles rteAnamnese.ExportRequested

        ' En création (patient non encore enregistré), pas de code patient : fallback dialogue.
        If _patient Is Nothing OrElse _idPatient <= 0 OrElse String.IsNullOrWhiteSpace(_patient.CodePatient) Then
            Return
        End If

        Try

            ' Garantit l'existence du dossier patient avant toute écriture.
            CheminsPatientHelper.AssurerDossierPatient(_patient.CodePatient)

            Dim extension As String = If(e.Format = ExportFormat.Word, ".docx", ".pdf")
            Dim horodatage As String = DateTime.Now.ToString("yyyyMMdd_HHmmss")
            Dim nomFichier As String = $"anamnese_{_patient.CodePatient}_{horodatage}{extension}"

            e.DestinationPath = CheminsPatientHelper.GetCheminFichierPatient(_patient.CodePatient, nomFichier)

        Catch ex As Exception

            ' En cas d'échec du calcul de chemin, on laisse le contrôle retomber sur son dialogue.
            GestionLog.EcrireLog(
                $"Erreur préparation export anamnèse (UC_PatientFiche, id={_idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : rteAnamnese_ExportCompleted
    ' Version   : V1.0.0
    ' Date      : 14/06/2026
    '
    ' Rôle      :
    ' Ouvre automatiquement le fichier exporté (PDF/Word) après un export contextuel réussi, l'export
    ' étant déclenché parce que l'utilisateur en a un besoin immédiat.
    '
    ' Remarques :
    ' - Ouverture via l'association de fichiers du système (ShellExecute).
    ' - La traçabilité du document (table documents) sera ajoutée ultérieurement.
    ' -------------------------------------------------------------------------------------------------
    Private Sub rteAnamnese_ExportCompleted(
        sender As Object,
        e As ExportCompletedEventArgs
    ) Handles rteAnamnese.ExportCompleted

        If Not e.Success OrElse String.IsNullOrWhiteSpace(e.DestinationPath) Then
            Return
        End If

        ' TODO : Traçabilité documents (brique Documents) — enregistrer ici une trace du fichier exporté
        ' dans la table documents : nom de fichier (e.DestinationPath), type/modèle de document
        ' (export anamnèse PDF/Word), code_patient, date, et NIVEAU = patient.
        ' Règle générale : tout document créé OU uploadé dans l'appli doit avoir une trace dans documents.
        ' À trancher à ce moment : niveau document = Enum publique (Patient/Dossier/Seance...) OU
        ' nouveau type de paramètre dans tec_parametres (plus cohérent avec le reste de l'appli).
        ' Ne pas oublier de revenir ici. Voir Docs\Todo\ToDo.md §D1 (Points différés).

        Try

            Dim psi As New ProcessStartInfo(e.DestinationPath) With {
                .UseShellExecute = True
            }
            Process.Start(psi)

            If _context IsNot Nothing Then
                _context.SetStatus("Anamnèse exportée et ouverte.")
            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur ouverture du fichier exporté (UC_PatientFiche, id={_idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : rteAnamnese_ContentChanged
    ' Version   : V1.0.0
    ' Date      : 18/06/2026
    '
    ' Rôle      :
    ' Marque l'anamnèse comme modifiée pendant l'édition locale (dirty-tracking), afin de n'avertir
    ' l'utilisateur d'une perte éventuelle que si le contenu a réellement changé.
    '
    ' Remarques :
    ' - L'événement n'est pas déclenché lors du chargement initial du contenu (ChargerContenu).
    ' - Seul le mode d'édition locale est concerné : en consultation pure, l'éditeur est en lecture seule.
    ' -------------------------------------------------------------------------------------------------
    Private Sub rteAnamnese_ContentChanged(
        sender As Object,
        e As EventArgs
    ) Handles rteAnamnese.ContentChanged

        If _modeEditionAnamnese Then
            _anamneseModifiee = True
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnModifierAnamnese_Click
    ' Version   : V1.0.0
    ' Date      : 18/06/2026
    '
    ' Rôle      :
    ' Active l'édition locale de l'anamnèse : l'éditeur devient saisissable (toolbar visible) et les
    ' boutons Enregistrer / Annuler remplacent Modifier, sans toucher au mode global de la fiche.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnModifierAnamnese_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnModifierAnamnese.Click

        If _idPatient <= 0 OrElse _mode <> ModeFiche.Consultation Then
            Return
        End If

        _modeEditionAnamnese = True
        _anamneseModifiee = False
        AppliquerModeAnamnese()

        rteAnamnese.Focus()

        If _context IsNot Nothing Then
            _context.SetStatus("Modification de l'anamnèse.")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnEnregistrerAnamnese_Click
    ' Version   : V1.0.0
    ' Date      : 18/06/2026
    '
    ' Rôle      :
    ' Enregistre uniquement l'anamnèse du patient courant (persistance ciblée), met à jour le patient
    ' en mémoire, puis revient en consultation locale (éditeur en lecture seule).
    '
    ' Remarques :
    ' - Utilise GestionPatients.UpdateAnamnesePatient (mise à jour ciblée des colonnes d'anamnèse).
    ' - N'altère pas les autres champs de la fiche ni le mode global.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEnregistrerAnamnese_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnEnregistrerAnamnese.Click

        If _idPatient <= 0 OrElse _patient Is Nothing Then
            Return
        End If

        Try
            Cursor = Cursors.WaitCursor

            Dim anamneseRtf As String = NormaliserTexte(rteAnamnese.RtfContent)
            Dim anamneseTxt As String = NormaliserTexte(rteAnamnese.TextContent)

            GestionPatients.UpdateAnamnesePatient(_idPatient, anamneseRtf, anamneseTxt)

            ' Synchronise le patient en mémoire pour rester cohérent avec la base.
            _patient.AnamneseRtf = anamneseRtf
            _patient.AnamneseTxt = anamneseTxt

            _modeEditionAnamnese = False
            _anamneseModifiee = False
            AppliquerModeAnamnese()

            If _context IsNot Nothing Then
                _context.SetStatus("Anamnèse enregistrée.")
            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur enregistrement de l'anamnèse (UC_PatientFiche, id={_idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur("Une erreur est survenue lors de l'enregistrement de l'anamnèse.")

        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAnnulerAnamnese_Click
    ' Version   : V1.0.0
    ' Date      : 18/06/2026
    '
    ' Rôle      :
    ' Annule l'édition locale de l'anamnèse : restaure le contenu d'origine (depuis le patient en
    ' mémoire) et revient en consultation locale (éditeur en lecture seule).
    '
    ' Remarques :
    ' - Demande confirmation seulement si des modifications ont réellement été saisies (dirty-tracking).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAnnulerAnamnese_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnAnnulerAnamnese.Click

        If _anamneseModifiee Then

            Dim message As String =
                "Annuler les modifications de l'anamnèse ?" &
                Environment.NewLine & "Les changements non enregistrés seront perdus."

            If DialogChoix.Confirmer(message, "Annulation de l'anamnèse") <> DialogResult.Yes Then
                Return
            End If

        End If

        ' Restaure le contenu d'origine depuis le patient en mémoire.
        If _patient IsNot Nothing Then
            rteAnamnese.ChargerContenu(_patient.AnamneseRtf, _patient.AnamneseTxt)
        End If

        _modeEditionAnamnese = False
        _anamneseModifiee = False
        AppliquerModeAnamnese()

        If _context IsNot Nothing Then
            _context.SetStatus("Modifications de l'anamnèse annulées.")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAjouterSituationFamiliale_Click
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Ouvre le référentiel des situations familiales dans une fenêtre modale (via Home, qui gère
    ' droits et élévation), puis recharge le combo et sélectionne automatiquement la valeur
    ' nouvellement créée le cas échéant.
    '
    ' Remarques :
    ' - L'action n'est proposée qu'en saisie (bouton désactivé en consultation).
    ' - La sélection courante est préservée si aucune nouvelle valeur n'est ajoutée.
    ' - La détection du « nouvel » élément repose sur la comparaison des identifiants avant/après.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterSituationFamiliale_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnAjouterSituationFamiliale.Click

        Dim homeForm As Home = TryCast(FindForm(), Home)

        If homeForm Is Nothing Then
            Return
        End If

        ' Identifiants présents avant l'ouverture du référentiel (détection des ajouts).
        Dim idsAvant As New List(Of ULong)
        Dim listeAvant As List(Of SituationFamiliale) =
            TryCast(cboSituationFamiliale.DataSource, List(Of SituationFamiliale))

        If listeAvant IsNot Nothing Then
            For Each situation As SituationFamiliale In listeAvant
                idsAvant.Add(situation.IdSituationFamiliale)
            Next
        End If

        ' Sélection courante à préserver si aucun ajout n'est effectué.
        Dim idSelectionCourante As ULong? = Nothing
        Dim selectionLue As Long? = SaisieSituationFamiliale()
        If selectionLue.HasValue Then
            idSelectionCourante = CULng(selectionLue.Value)
        End If

        Dim affiche As Boolean =
            homeForm.OuvrirReferentielModal(New UC_SituationsFamiliales(), "Situations familiales")

        If Not affiche Then
            Return
        End If

        Try

            Dim situationsApres As List(Of SituationFamiliale) =
                ChargerSituationsFamilialesDepuisReferentiel()

            ' Recherche d'un identifiant nouvellement créé (présent après, absent avant).
            Dim idNouveau As ULong? = Nothing
            For Each situation As SituationFamiliale In situationsApres
                If situation.IdSituationFamiliale > 0UL AndAlso
                   Not idsAvant.Contains(situation.IdSituationFamiliale) Then
                    idNouveau = situation.IdSituationFamiliale
                    Exit For
                End If
            Next

            Dim idASelectionner As ULong? =
                If(idNouveau.HasValue, idNouveau, idSelectionCourante)

            LierSituationsFamiliales(situationsApres, idASelectionner)

            If idNouveau.HasValue AndAlso _context IsNot Nothing Then
                _context.SetStatus("Nouvelle situation familiale ajoutée et sélectionnée.")
            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur rechargement des situations familiales après ajout (UC_PatientFiche).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnEnregistrer_Click
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Déclenche l'enregistrement (création ou mise à jour) du patient.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEnregistrer_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnEnregistrer.Click

        Enregistrer()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAnnuler_Click
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Annule la saisie en cours et revient à l'état consultation (ou retourne à la liste en création).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAnnuler_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnAnnuler.Click

        If _idPatient > 0 Then

            ChargerPatient()
            AppliquerMode(ModeFiche.Consultation)

            If _context IsNot Nothing Then
                _context.SetStatus("Modifications annulées.")
            End If

        Else
            RetournerListe()
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnFermer_Click
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Revient à l'écran précédent (liste des patients) via la mini-pile de navigation.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnFermer_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnFermer.Click

        RetournerListe()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RetournerListe
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Demande à Home de revenir à l'écran précédent (D-Q15) ; à défaut, recharge l'accueil patients.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RetournerListe()

        ' Garde-fou : quitter la fiche pendant une édition locale de l'anamnèse non enregistrée
        ' entraînerait la perte de la saisie. On en avertit l'utilisateur avant de naviguer.
        If _modeEditionAnamnese AndAlso _anamneseModifiee Then

            Dim message As String =
                "Quitter la fiche sans enregistrer l'anamnèse ?" &
                Environment.NewLine & "Les modifications non enregistrées seront perdues."

            If DialogChoix.Confirmer(message, "Anamnèse non enregistrée") <> DialogResult.Yes Then
                Return
            End If

        End If

        Dim homeForm As Home = TryCast(FindForm(), Home)

        If homeForm Is Nothing Then
            Return
        End If

        If Not homeForm.NavigateRetour() Then
            homeForm.NavigateToPatients()
        End If

    End Sub

#End Region

#Region "Famille / Contacts"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserGrilleContacts
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Applique la mise en forme commune (brique réutilisable UtilsDataGrid) à la grille des contacts
    ' et fige ses propriétés de base, à l'identique de la liste patients (UC_PatientHome).
    '
    ' Remarques :
    ' - Les colonnes et leurs DataPropertyName sont définis au Designer (AutoGenerateColumns = False).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserGrilleContacts()

        UtilsDataGrid.InitDataGridBasique(dgvContacts)

        dgvContacts.AutoGenerateColumns = False

        dgvContacts.ReadOnly = True
        dgvContacts.MultiSelect = False
        dgvContacts.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvContacts.AllowUserToAddRows = False
        dgvContacts.AllowUserToDeleteRows = False

        ' Le bouton Réinitialiser est désactivé par défaut (aucun filtre au chargement)
        btnReinitialiserContacts.Enabled = False

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : tabFiche_Selected
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Charge (à la demande) la liste des contacts du patient lorsque l'onglet Famille / Contacts
    ' est sélectionné.
    '
    ' Remarques :
    ' - L'onglet n'est accessible qu'avec un patient enregistré (activation progressive D-Q14).
    ' -------------------------------------------------------------------------------------------------
    Private Sub tabFiche_Selected(
        sender As Object,
        e As TabControlEventArgs
    ) Handles tabFiche.Selected

        If e.TabPage Is tabPageFamille AndAlso _idPatient > 0 Then
            ChargerContacts()
        ElseIf e.TabPage Is tabPageIntervenants AndAlso _idPatient > 0 Then
            ChargerIntervenants()
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerContacts
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Recharge la grille des contacts du patient courant depuis la couche métier et met à jour
    ' l'état des boutons d'action.
    '
    ' Remarques :
    ' - AutoGenerateColumns = False : les colonnes (dont les pictos de rôle) sont définies au Designer.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerContacts()

        If _idPatient <= 0 Then
            _contacts = Nothing
            dgvContacts.DataSource = Nothing
            RafraichirEtatBoutonsContacts()
            Return
        End If

        Try

            dgvContacts.AutoGenerateColumns = False

            _contacts = GestionFamilleContacts.GetContactsParPatient(_idPatient)

            AppliquerFiltresContacts()

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur chargement des contacts du patient (UC_PatientFiche, id={_idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors du chargement des contacts.")
            End If

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvContacts_CellFormatting
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Met en forme l'affichage du téléphone du contact (masque international lisible) sans altérer
    ' la donnée sous-jacente (brique réutilisable UtilsTelephone).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvContacts_CellFormatting(
        sender As Object,
        e As DataGridViewCellFormattingEventArgs
    ) Handles dgvContacts.CellFormatting

        If e.RowIndex < 0 OrElse e.ColumnIndex <> colContactTelephone.Index Then
            Return
        End If

        Dim valeur As String = TryCast(e.Value, String)

        If String.IsNullOrWhiteSpace(valeur) Then
            Return
        End If

        e.Value = UtilsTelephone.FormaterAffichage(valeur, Nothing)
        e.FormattingApplied = True

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvContacts_SelectionChanged
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Met à jour l'état des boutons Modifier / Supprimer selon la sélection courante.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvContacts_SelectionChanged(
        sender As Object,
        e As EventArgs
    ) Handles dgvContacts.SelectionChanged

        RafraichirEtatBoutonsContacts()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvContacts_CellDoubleClick
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Ouvre le contact sélectionné en consultation (double-clic sur une ligne).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvContacts_CellDoubleClick(
        sender As Object,
        e As DataGridViewCellEventArgs
    ) Handles dgvContacts.CellDoubleClick

        If e.RowIndex < 0 Then
            Return
        End If

        Dim contact As FamilleContact = ContactSelectionne()

        If contact IsNot Nothing Then
            OuvrirEditionContact(contact, ouvrirEnConsultation:=True)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerFiltresContacts
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Filtre la liste mémoire des contacts selon le texte de recherche (nom, prénom, lien) et
    ' réaffiche le résultat dans la grille, à l'identique du fonctionnement de UC_PatientHome.
    '
    ' Remarques :
    ' - Recherche simple multi-champs, insensible à la casse, déclenchée par Entrée ou bouton.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerFiltresContacts()

        If _contacts Is Nothing Then
            dgvContacts.DataSource = Nothing
            RafraichirEtatBoutonsContacts()
            Return
        End If

        Try

            Dim contactsFiltres = _contacts.AsEnumerable()

            Dim texteRecherche As String = txtRechercheContact.Text.Trim()
            If Not String.IsNullOrEmpty(texteRecherche) Then
                contactsFiltres = contactsFiltres.Where(
                    Function(c) CorrespondRechercheContact(c, texteRecherche)
                )
            End If

            Dim resultats = contactsFiltres.ToList()

            dgvContacts.DataSource = Nothing
            dgvContacts.DataSource = resultats

            dgvContacts.ClearSelection()

            btnReinitialiserContacts.Enabled = Not String.IsNullOrEmpty(texteRecherche)

            If _context IsNot Nothing Then
                _context.SetStatus($"{resultats.Count} contact(s) affiché(s).")
            End If

        Finally

            RafraichirEtatBoutonsContacts()

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CorrespondRechercheContact
    ' Version  : V1.0.0
    ' Date     : 13/06/2026
    '
    ' Rôle     :
    ' Indique si un contact correspond au texte de recherche (comparaison insensible à la casse).
    '
    ' Paramètres :
    ' - contact : Le contact à tester
    ' - texte   : Le texte recherché (déjà nettoyé)
    '
    ' Retour   :
    ' - Boolean : True si l'un des champs (nom, prénom, lien, rôle légal) contient le texte recherché.
    '
    ' Remarques :
    ' - Les champs optionnels peuvent être Nothing : protégés par If(..., "").
    ' -------------------------------------------------------------------------------------------------
    Private Function CorrespondRechercheContact(contact As FamilleContact, texte As String) As Boolean

        Return If(contact.Nom, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(contact.Prenom, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(contact.LibelleLienPatient, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(contact.LibelleRoleLegal, "").Contains(texte, StringComparison.OrdinalIgnoreCase)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnRechercherContact_Click
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Applique le filtre de recherche sur la liste mémoire des contacts.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnRechercherContact_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnRechercherContact.Click

        AppliquerFiltresContacts()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : txtRechercheContact_KeyDown
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Permet de déclencher la recherche des contacts par la touche Entrée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub txtRechercheContact_KeyDown(
        sender As Object,
        e As KeyEventArgs
    ) Handles txtRechercheContact.KeyDown

        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            AppliquerFiltresContacts()
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnReinitialiserContacts_Click
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Réinitialise le critère de recherche et réaffiche la liste complète des contacts.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnReinitialiserContacts_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnReinitialiserContacts.Click

        txtRechercheContact.Clear()

        AppliquerFiltresContacts()

        txtRechercheContact.Focus()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAjouterContact_Click
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Ouvre la fenêtre modale de création d'un nouveau contact pour le patient courant.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterContact_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnAjouterContact.Click

        OuvrirEditionContact(Nothing)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnModifierContact_Click
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Ouvre la fenêtre modale de modification du contact sélectionné.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnModifierContact_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnModifierContact.Click

        Dim contact As FamilleContact = ContactSelectionne()

        If contact IsNot Nothing Then
            OuvrirEditionContact(contact)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnSupprimerContact_Click
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Supprime physiquement le contact sélectionné après confirmation explicite de l'utilisateur.
    '
    ' Remarques :
    ' - La table famille_contacts ne comporte pas d'indicateur d'activité : la suppression est physique.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnSupprimerContact_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnSupprimerContact.Click

        Dim contact As FamilleContact = ContactSelectionne()

        If contact Is Nothing Then
            Return
        End If

        Dim message As String =
            $"Supprimer définitivement le contact « {contact.NomComplet} » ?" &
            Environment.NewLine & "Cette action est irréversible."

        If DialogChoix.Confirmer(message, "Suppression d'un contact") <> DialogResult.Yes Then
            Return
        End If

        Try

            GestionFamilleContacts.DeleteContact(contact.IdFamilleContact)

            ChargerContacts()

            If _context IsNot Nothing Then
                _context.SetStatus("Contact supprimé.")
            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur suppression d'un contact (UC_PatientFiche, idContact={contact.IdFamilleContact}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors de la suppression du contact.")
            End If

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : OuvrirEditionContact
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Ouvre la fenêtre modale d'édition d'un contact (création si contact = Nothing, sinon
    ' consultation ou modification) et rafraîchit la grille en cas d'enregistrement.
    '
    ' Paramètres :
    ' - contact              : Contact à consulter/modifier, ou Nothing pour une création.
    ' - ouvrirEnConsultation : True pour ouvrir un contact existant en lecture seule (double-clic).
    ' -------------------------------------------------------------------------------------------------
    Private Sub OuvrirEditionContact(contact As FamilleContact, Optional ouvrirEnConsultation As Boolean = False)

        Dim homeForm As Home = TryCast(FindForm(), Home)

        If homeForm Is Nothing Then
            Return
        End If

        Using dlg As New ContactEdition(_idPatient, homeForm, contact, _patient, ouvrirEnConsultation)

            dlg.SetContext(_context)

            If dlg.ShowDialog(Me) = DialogResult.OK Then

                ChargerContacts()

                If _context IsNot Nothing Then
                    _context.SetStatus(If(contact Is Nothing, "Contact ajouté.", "Contact modifié."))
                End If

            End If

        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RafraichirEtatBoutonsContacts
    ' Version   : V1.0.0
    ' Date      : 13/06/2026
    '
    ' Rôle      :
    ' Active ou désactive les boutons d'action de l'onglet Famille / Contacts selon la présence
    ' d'un patient enregistré et d'une ligne sélectionnée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RafraichirEtatBoutonsContacts()

        Dim patientEnregistre As Boolean = _idPatient > 0
        Dim aContactSelectionne As Boolean = ContactSelectionne() IsNot Nothing

        btnAjouterContact.Enabled = patientEnregistre
        btnModifierContact.Enabled = patientEnregistre AndAlso aContactSelectionne
        btnSupprimerContact.Enabled = patientEnregistre AndAlso aContactSelectionne

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ContactSelectionne
    ' Version  : V1.0.0
    ' Date     : 13/06/2026
    '
    ' Rôle     :
    ' Retourne le contact actuellement sélectionné dans la grille, ou Nothing si aucune ligne.
    '
    ' Retour   :
    ' - FamilleContact? : Contact ou Nothing
    ' -------------------------------------------------------------------------------------------------
    Private Function ContactSelectionne() As FamilleContact

        If dgvContacts.CurrentRow Is Nothing Then
            Return Nothing
        End If

        Return TryCast(dgvContacts.CurrentRow.DataBoundItem, FamilleContact)

    End Function

#End Region

#Region "Intervenants"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserGrilleIntervenants
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Applique la mise en forme commune (brique réutilisable UtilsDataGrid) à la grille des
    ' intervenants et fige ses propriétés de base, à l'identique de la grille des contacts.
    '
    ' Remarques :
    ' - Les colonnes et leurs DataPropertyName sont définis au Designer (AutoGenerateColumns = False).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserGrilleIntervenants()

        UtilsDataGrid.InitDataGridBasique(dgvIntervenants)

        dgvIntervenants.AutoGenerateColumns = False

        dgvIntervenants.ReadOnly = True
        dgvIntervenants.MultiSelect = False
        dgvIntervenants.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvIntervenants.AllowUserToAddRows = False
        dgvIntervenants.AllowUserToDeleteRows = False

        ' Le bouton Réinitialiser est désactivé par défaut (aucun filtre au chargement)
        btnReinitialiserIntervenants.Enabled = False

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerIntervenants
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Recharge la grille des intervenants du patient courant depuis la couche métier et met à jour
    ' l'état des boutons d'action.
    '
    ' Remarques :
    ' - AutoGenerateColumns = False : les colonnes sont définies au Designer.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerIntervenants()

        If _idPatient <= 0 Then
            _intervenants = Nothing
            dgvIntervenants.DataSource = Nothing
            RafraichirEtatBoutonsIntervenants()
            Return
        End If

        Try

            dgvIntervenants.AutoGenerateColumns = False

            _intervenants = GestionSuivisIntervenants.GetSuivisParPatient(_idPatient)

            AppliquerFiltresIntervenants()

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur chargement des intervenants du patient (UC_PatientFiche, id={_idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors du chargement des intervenants.")
            End If

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvIntervenants_CellFormatting
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Affiche les dates de période en format court et masque les valeurs nulles (suivi sans date),
    ' sans altérer la donnée sous-jacente.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvIntervenants_CellFormatting(
        sender As Object,
        e As DataGridViewCellFormattingEventArgs
    ) Handles dgvIntervenants.CellFormatting

        If e.RowIndex < 0 Then
            Return
        End If

        If e.ColumnIndex <> colIntervenantDateDebut.Index AndAlso
           e.ColumnIndex <> colIntervenantDateFin.Index Then
            Return
        End If

        If e.Value Is Nothing OrElse e.Value Is DBNull.Value Then
            e.Value = String.Empty
            e.FormattingApplied = True
            Return
        End If

        Dim valeur As Date
        If Date.TryParse(e.Value.ToString(), valeur) Then
            e.Value = valeur.ToShortDateString()
            e.FormattingApplied = True
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvIntervenants_SelectionChanged
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Met à jour l'état des boutons Modifier / Supprimer selon la sélection courante.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvIntervenants_SelectionChanged(
        sender As Object,
        e As EventArgs
    ) Handles dgvIntervenants.SelectionChanged

        RafraichirEtatBoutonsIntervenants()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvIntervenants_CellDoubleClick
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Ouvre l'intervenant sélectionné en consultation (double-clic sur une ligne).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvIntervenants_CellDoubleClick(
        sender As Object,
        e As DataGridViewCellEventArgs
    ) Handles dgvIntervenants.CellDoubleClick

        If e.RowIndex < 0 Then
            Return
        End If

        Dim suivi As SuiviIntervenant = IntervenantSelectionne()

        If suivi IsNot Nothing Then
            OuvrirEditionIntervenant(suivi, ouvrirEnConsultation:=True)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerFiltresIntervenants
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Filtre la liste mémoire des intervenants selon le texte de recherche (rôle, nom, spécialité,
    ' lieu) et réaffiche le résultat dans la grille, à l'identique des contacts.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerFiltresIntervenants()

        If _intervenants Is Nothing Then
            dgvIntervenants.DataSource = Nothing
            RafraichirEtatBoutonsIntervenants()
            Return
        End If

        Try

            Dim intervenantsFiltres = _intervenants.AsEnumerable()

            Dim texteRecherche As String = txtRechercheIntervenant.Text.Trim()
            If Not String.IsNullOrEmpty(texteRecherche) Then
                intervenantsFiltres = intervenantsFiltres.Where(
                    Function(s) CorrespondRechercheIntervenant(s, texteRecherche)
                )
            End If

            Dim resultats = intervenantsFiltres.ToList()

            dgvIntervenants.DataSource = Nothing
            dgvIntervenants.DataSource = resultats

            dgvIntervenants.ClearSelection()

            btnReinitialiserIntervenants.Enabled = Not String.IsNullOrEmpty(texteRecherche)

            If _context IsNot Nothing Then
                _context.SetStatus($"{resultats.Count} intervenant(s) affiché(s).")
            End If

        Finally

            RafraichirEtatBoutonsIntervenants()

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CorrespondRechercheIntervenant
    ' Version  : V1.0.0
    ' Date     : 15/06/2026
    '
    ' Rôle     :
    ' Indique si un intervenant correspond au texte de recherche (comparaison insensible à la casse).
    '
    ' Paramètres :
    ' - suivi : L'intervenant à tester
    ' - texte : Le texte recherché (déjà nettoyé)
    '
    ' Retour   :
    ' - Boolean : True si l'un des champs (rôle, nom, spécialité, lieu) contient le texte recherché.
    '
    ' Remarques :
    ' - Les champs optionnels peuvent être Nothing : protégés par If(..., "").
    ' -------------------------------------------------------------------------------------------------
    Private Function CorrespondRechercheIntervenant(suivi As SuiviIntervenant, texte As String) As Boolean

        Return If(suivi.LibelleRoleIntervenant, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(suivi.NomProfessionnel, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(suivi.Specialite, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(suivi.Lieu, "").Contains(texte, StringComparison.OrdinalIgnoreCase)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnRechercherIntervenant_Click
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Applique le filtre de recherche sur la liste mémoire des intervenants.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnRechercherIntervenant_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnRechercherIntervenant.Click

        AppliquerFiltresIntervenants()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : txtRechercheIntervenant_KeyDown
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Permet de déclencher la recherche des intervenants par la touche Entrée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub txtRechercheIntervenant_KeyDown(
        sender As Object,
        e As KeyEventArgs
    ) Handles txtRechercheIntervenant.KeyDown

        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            AppliquerFiltresIntervenants()
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnReinitialiserIntervenants_Click
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Réinitialise le critère de recherche et réaffiche la liste complète des intervenants.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnReinitialiserIntervenants_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnReinitialiserIntervenants.Click

        txtRechercheIntervenant.Clear()

        AppliquerFiltresIntervenants()

        txtRechercheIntervenant.Focus()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAjouterIntervenant_Click
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Ouvre la fenêtre modale de création d'un nouvel intervenant pour le patient courant.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterIntervenant_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnAjouterIntervenant.Click

        OuvrirEditionIntervenant(Nothing)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnModifierIntervenant_Click
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Ouvre la fenêtre modale de modification de l'intervenant sélectionné.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnModifierIntervenant_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnModifierIntervenant.Click

        Dim suivi As SuiviIntervenant = IntervenantSelectionne()

        If suivi IsNot Nothing Then
            OuvrirEditionIntervenant(suivi)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnSupprimerIntervenant_Click
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Supprime physiquement l'intervenant sélectionné après confirmation explicite de l'utilisateur.
    '
    ' Remarques :
    ' - La liaison autres_suivis_patient ne comporte pas d'indicateur d'activité : suppression physique.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnSupprimerIntervenant_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnSupprimerIntervenant.Click

        Dim suivi As SuiviIntervenant = IntervenantSelectionne()

        If suivi Is Nothing Then
            Return
        End If

        Dim message As String =
            $"Supprimer définitivement l'intervenant « {suivi.LibelleAffichage} » ?" &
            Environment.NewLine & "Cette action est irréversible."

        If DialogChoix.Confirmer(message, "Suppression d'un intervenant") <> DialogResult.Yes Then
            Return
        End If

        Try

            GestionSuivisIntervenants.DeleteSuivi(suivi.IdAutreSuiviPatient)

            ChargerIntervenants()

            If _context IsNot Nothing Then
                _context.SetStatus("Intervenant supprimé.")
            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur suppression d'un intervenant (UC_PatientFiche, idSuivi={suivi.IdAutreSuiviPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors de la suppression de l'intervenant.")
            End If

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : OuvrirEditionIntervenant
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Ouvre la fenêtre modale d'édition d'un intervenant (création si suivi = Nothing, sinon
    ' consultation ou modification) et rafraîchit la grille en cas d'enregistrement.
    '
    ' Paramètres :
    ' - suivi                : Intervenant à consulter/modifier, ou Nothing pour une création.
    ' - ouvrirEnConsultation : True pour ouvrir un intervenant existant en lecture seule (double-clic).
    ' -------------------------------------------------------------------------------------------------
    Private Sub OuvrirEditionIntervenant(suivi As SuiviIntervenant, Optional ouvrirEnConsultation As Boolean = False)

        Dim homeForm As Home = TryCast(FindForm(), Home)

        If homeForm Is Nothing Then
            Return
        End If

        Using dlg As New IntervenantEdition(_idPatient, homeForm, suivi, ouvrirEnConsultation)

            dlg.SetContext(_context)

            If dlg.ShowDialog(Me) = DialogResult.OK Then

                ChargerIntervenants()

                If _context IsNot Nothing Then
                    _context.SetStatus(If(suivi Is Nothing, "Intervenant ajouté.", "Intervenant modifié."))
                End If

            End If

        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RafraichirEtatBoutonsIntervenants
    ' Version   : V1.0.0
    ' Date      : 15/06/2026
    '
    ' Rôle      :
    ' Active ou désactive les boutons d'action de l'onglet Intervenants selon la présence
    ' d'un patient enregistré et d'une ligne sélectionnée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RafraichirEtatBoutonsIntervenants()

        Dim patientEnregistre As Boolean = _idPatient > 0
        Dim aIntervenantSelectionne As Boolean = IntervenantSelectionne() IsNot Nothing

        btnAjouterIntervenant.Enabled = patientEnregistre
        btnModifierIntervenant.Enabled = patientEnregistre AndAlso aIntervenantSelectionne
        btnSupprimerIntervenant.Enabled = patientEnregistre AndAlso aIntervenantSelectionne

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : IntervenantSelectionne
    ' Version  : V1.0.0
    ' Date     : 15/06/2026
    '
    ' Rôle     :
    ' Retourne l'intervenant actuellement sélectionné dans la grille, ou Nothing si aucune ligne.
    ' -------------------------------------------------------------------------------------------------
    Private Function IntervenantSelectionne() As SuiviIntervenant

        If dgvIntervenants.CurrentRow Is Nothing Then
            Return Nothing
        End If

        Return TryCast(dgvIntervenants.CurrentRow.DataBoundItem, SuiviIntervenant)

    End Function


#End Region

End Class
