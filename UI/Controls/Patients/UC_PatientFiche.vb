' -------------------------------------------------------------------------------------------------
' UserControl : UC_PatientFiche
' Projet      : Althéa
' Version     : V1.13.0
' Date        : 25/10/2026
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
' - V1.4.0 (14/10/2026, UI-7) : listes de l'onglet Identité en AltheaListe, date de naissance en
'   AltheaDate facultative (vide = inconnue, au lieu de la case à cocher Windows) ; consultation
'   lisible des trois cartes (UtilsControls.DefinirLectureSeule).
' - V1.5.0 (15/10/2026) : boutons [+] (situation familiale) et loupe (mutualité) en AltheaBouton
'   Outil à icône seule, sans UtilsButtons (icône du Designer, survol du kit).
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
' - UtilsDataGrid, UtilsPastilles (grilles : pastilles, alignements)
'
' Interface   :
' - IContextAwareUserControl : injection du contexte UI partagé
' - IUnsavedChangesAware : avertit avant de quitter la fiche avec des modifications non enregistrées
'
' Historique  :
' - V1.7.0 (19/10/2026) : modifications non enregistrées — quitter la fiche (Retour, menu de Home,
'   fermeture d'Althéa : IUnsavedChangesAware) ou l'onglet Anamnèse en cours de modification propose
'   Enregistrer / Quitter sans enregistrer / Rester ; la fiche en création ou en modification est
'   surveillée par une photo de ses champs (GardeModifications), l'anamnèse par son suivi local.
'   Annuler (fiche en modification, anamnèse) demande confirmation s'il y a des modifications
'   (DialogChoix.ConfirmerAbandon, même question dans toute l'application).
' - V1.7.1 (19/10/2026) : éditeur titré pour « Ouvrir en grand » (TitreDocument = « Alerte / Notes
'   »).
' - V1.7.2 (19/10/2026) : anamnèse : patient dans l'en-tête de ses documents (PatientDocument).
' - V1.7.3 (19/10/2026) : éditeur(s) de texte : textes types de leur usage (UsageTextesTypes, temps 2).
' - V1.7.4 (19/10/2026) : l'alerte propose aussi ses textes types (usage « Alerte du patient »).
' - V1.8.0 (20/10/2026, v2.46.0) : latéralité lue dans le référentiel ref_lateralites (elle était
'   écrite dans le code) ; une latéralité désactivée reste affichée chez le patient qui l'a.
' - V1.9.0 (23/10/2026, audit C.2 étape 2, décisions de Joëlle) : la fiche est jugée par
'   GestionPatients.ValiderPatient (règles sorties de ValiderSaisie, appliquées aussi à
'   l'enregistrement) ; erreurs sur leurs champs et dans une fenêtre récapitulative
'   (AffichageErreursSaisie) au lieu de la seule barre d'état ; type d'identifiant transmis au métier
'   (Patient.TypeIdentifiantNiss) ; NISS affiché mis en forme (UtilsNiss.Formater), enregistré
'   normalisé ; refus du métier affiché dans une fenêtre. Relevés de Joëlle : homonyme sans date de
'   naissance signalé ; icônes d'erreur effacées à l'enregistrement réussi (et dès la correction du
'   champ, AffichageErreursSaisie).
' - V1.9.1 (23/10/2026, audit C.2 étape 2b) : grille Intervenants — « terminé » dit par le suivi
'   (SuiviIntervenant.EstTerminee) au lieu d'une relecture de la cellule Date de fin.
' - V1.10.0 (23/10/2026, recherche transversale R1) : la fiche peut s'ouvrir sur l'onglet Famille /
'   Contacts, Intervenants ou Dossiers (SelectionnerOngletAuChargement) ; SelectionnerOngletDossiers
'   en est un cas.
' - V1.11.0 (24/10/2026, relevé de Joëlle) : colonne « Code » (FC…, AS…) dans les grilles des
'   contacts et des intervenants (Designer).
' - V1.6.1 (19/10/2026) : éditeur de texte refondu (UC_RichTextEditor V2.0.0 ; alerte en
'   profil Compact) ; l'anamnèse marque l'enregistrement (ligne d'état) et titre ses impressions « Anamnèse ».
' - V1.6.0 (18/10/2026, UI-9.2) : grille Dossiers — statut en pastille (couleurs de la colonne Suivi
'   des Patients), dossier clôturé ou archivé atténué ; grille Intervenants — intervention terminée
'   atténuée ; dates centrées dans les deux grilles.
' - V1.12.0 (24/10/2026, v2.56.0) : code postal belge contrôlé par le référentiel, localité proposée,
'   suggérée et confirmée si elle ne correspond pas au code (SaisieLocaliteBelge).
' - V1.12.1 (24/10/2026, audit C.3) : types et sources de document lus dans CodesSysteme (plus de
'   code en clair).
' - V1.12.2 (25/10/2026, audit C.4, ADR-55) : la fenêtre principale est lue dans le contexte
'   (_context.FenetrePrincipale, IFenetrePrincipale) ; plus aucune référence à Home.
' - V1.13.0 (25/10/2026, audit C.5, décision de Joëlle) : fiche répartie en fichiers partiels, un
'   par onglet, code déplacé tel quel : UC_PatientFiche.Famille.vb, .Intervenants.vb, .Dossiers.vb ;
'   ici le reste (identité, anamnèse, bandeau, modes, validation, garde).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.IO

Public Class UC_PatientFiche
    Implements IContextAwareUserControl, IUnsavedChangesAware

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

    ' Onglet sur lequel ouvrir la fiche (retour du hub dossier, recherche transversale).
    Public Enum OngletPatient
        Identite
        Famille
        Intervenants
        Dossiers
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

    ' Photo des champs de la fiche (Identité, Anamnèse) en création ou en modification : dit si des
    ' modifications seraient perdues en quittant (créée au chargement, onglets de listes exclus).
    Private _gardeFiche As GardeModifications

    ' Aide à la saisie du code postal et de la localité d'une adresse belge (v2.56.0).
    Private _saisieLocalite As SaisieLocaliteBelge

    ' Onglet à présélectionner au chargement (retour depuis le hub dossier D-Q15, recherche) ; Nothing :
    ' onglet par défaut.
    Private _ongletAuChargement As OngletPatient?

    ' Liste complète des contacts du patient courant (source mémoire pour la recherche rapide)
    Private _contacts As List(Of FamilleContact)

    ' Liste complète des intervenants du patient courant (source mémoire pour la recherche rapide)
    Private _intervenants As List(Of SuiviIntervenant)

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

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SelectionnerOngletDossiers
    ' Version   : V1.1.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Demande la présélection de l'onglet Dossiers au prochain chargement de la fiche.
    '
    ' Remarques :
    ' - Utilisée par la fabrique de retour (mini-pile D-Q15) pour restaurer l'onglet Dossiers
    '   lorsque l'utilisateur revient du hub dossier.
    ' - À appeler avant l'ajout du contrôle (l'application effective a lieu au Load).
    ' - V1.1.0 (23/10/2026) : cas particulier de SelectionnerOngletAuChargement.
    ' -------------------------------------------------------------------------------------------------
    Public Sub SelectionnerOngletDossiers()

        SelectionnerOngletAuChargement(OngletPatient.Dossiers)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SelectionnerOngletAuChargement
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Demande la présélection d'un onglet au prochain chargement de la fiche (patient existant).
    '
    ' Remarques :
    ' - À appeler avant l'ajout du contrôle (l'application effective a lieu au Load) ; utilisée par
    '   la recherche transversale (contact → Famille / Contacts, intervenant → Intervenants).
    ' -------------------------------------------------------------------------------------------------
    Public Sub SelectionnerOngletAuChargement(onglet As OngletPatient)

        _ongletAuChargement = onglet

    End Sub

#End Region

#Region "Chargement UserControl"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : UC_PatientFiche_Load
    ' Version   : V1.5.0
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Initialise le UserControl : boutons, combos, infobulles, puis charge le patient selon le mode.
    '
    ' Remarques :
    ' - Aucune logique métier ni accès direct à la base de données.
    ' - V1.1.0 (19/10/2026) : crée la garde des modifications de la fiche (GardeModifications).
    ' - V1.2.0 (19/10/2026) : textes types proposés dans l'éditeur (UsageTextesTypes).
    ' - V1.3.0 (19/10/2026) : usage propre (alerte, commentaire séparé), v2.43.0.
    ' - V1.4.0 (23/10/2026) : onglet présélectionné au choix (SelectionnerOngletAuChargement).
    ' - V1.5.0 (24/10/2026, v2.56.0) : aide à la saisie du code postal et de la localité (SaisieLocaliteBelge).
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_PatientFiche_Load(
        sender As Object,
        e As EventArgs
    ) Handles MyBase.Load

        _gardeFiche = New GardeModifications(tabFiche, tabPageFamille, tabPageIntervenants, tabPageDossiers)

        _saisieLocalite = New SaisieLocaliteBelge(txtCodePostal, txtLocalite, AddressOf CodePaysSaisi)
        AddHandler Disposed, Sub() _saisieLocalite?.Dispose()

        ' Textes types proposés dans l'éditeur (temps 2 de l'éditeur, UsagesTexteType).
        rteAnamnese.UsageTextesTypes = UsagesTexteType.Anamnese
        rteAlerte.UsageTextesTypes = UsagesTexteType.AlertePatient

        InitialiserBoutons()
        InitialiserCombos()
        InitialiserGrilleContacts()
        InitialiserGrilleIntervenants()
        InitialiserGrilleDossiers()
        InitialiserToolTips()

        If _mode = ModeFiche.Creation Then
            DemarrerCreation()
        Else
            ChargerPatient()
        End If

        If _ongletAuChargement.HasValue AndAlso _idPatient > 0 Then
            Select Case _ongletAuChargement.Value
                Case OngletPatient.Famille : tabFiche.SelectedTab = tabPageFamille
                Case OngletPatient.Intervenants : tabFiche.SelectedTab = tabPageIntervenants
                Case OngletPatient.Dossiers : tabFiche.SelectedTab = tabPageDossiers
                Case Else : tabFiche.SelectedTab = tabPageIdentite
            End Select
        End If

    End Sub

#End Region

#Region "Initialisation UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserBoutons
    ' Version   : V1.1.0
    ' Date      : 15/10/2026
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

        UtilsButtons.InitStandardButton(btnAjouterContact)
        UtilsButtons.InitStandardButton(btnModifierContact)
        UtilsButtons.InitStandardButton(btnSupprimerContact)

        UtilsButtons.InitStandardButton(btnModifierAnamnese)
        UtilsButtons.InitStandardButton(btnEnregistrerAnamnese)
        UtilsButtons.InitStandardButton(btnAnnulerAnamnese)

        ' Boutons [+] et loupe : AltheaBouton Outil à icône seule, sans UtilsButtons (ses icônes de
        ' survol, claires, disparaîtraient sur le survol vert pâle du kit) : l'icône du Designer reste.
        UtilsButtons.InitDiversIconButton(btnRechercherContact)
        UtilsButtons.InitDiversIconButton(btnReinitialiserIntervenants)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserCombos
    ' Version   : V1.1.0
    ' Date      : 20/10/2026
    '
    ' Rôle      :
    ' Alimente les listes déroulantes (latéralité, situation familiale).
    '
    ' Remarques :
    ' - La latéralité provient du référentiel ref_lateralites (v2.46.0 ; elle était écrite dans le code).
    ' - La situation familiale provient du référentiel ref_situations_familiales (valeurs actives).
    ' - V1.1.0 (20/10/2026) : latéralités du référentiel (UtilsSaisieReferentiels.ChargerLateralites).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserCombos()

        UtilsSaisieReferentiels.ChargerLateralites(cboLateralite, Nothing)

        cboTypeNiss.Items.Clear()
        cboTypeNiss.Items.AddRange(UtilsNiss.TypesIdentifiant())
        cboTypeNiss.SelectedItem = UtilsNiss.TYPE_BE

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

        Try

            LierPrixSeancesDefaut()

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur chargement des tarifs par défaut (UC_PatientFiche).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : LierPrixSeancesDefaut
    ' Version   : V1.0.0
    ' Date      : 20/01/2025
    '
    ' Rôle      :
    ' Alimente la liste déroulante du tarif par défaut avec les tarifs séance actifs.
    '
    ' Remarques :
    ' - Charge tous les tarifs depuis la table prix_seance ordonnés par montant
    ' - Insère une entrée vide en tête pour 'non renseigné'
    ' -------------------------------------------------------------------------------------------------
    Private Sub LierPrixSeancesDefaut()

        cboPrixSeanceDefaut.DataSource = Nothing
        cboPrixSeanceDefaut.Items.Clear()

        Dim prixSeances As List(Of PrixSeance) = GestionPrixSeance.GetPrixSeanceActifs()

        ' Ajouter une entrée vide en tête
        If prixSeances Is Nothing Then prixSeances = New List(Of PrixSeance)
        prixSeances.Insert(0, New PrixSeance With {.IdPrixSeance = 0, .CodePrixSeance = "", .LibellePrixSeance = "", .Montant = 0, .Actif = True})

        cboPrixSeanceDefaut.DataSource = prixSeances
        cboPrixSeanceDefaut.DisplayMember = "MontantAffichage"
        cboPrixSeanceDefaut.ValueMember = "IdPrixSeance"

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

        _context.SetToolTip(btnAjouterContact, "Rattacher un contact à ce patient.")
        _context.SetToolTip(btnModifierContact, "Modifier le contact sélectionné.")
        _context.SetToolTip(btnSupprimerContact, "Détacher le contact sélectionné du patient.")
        _context.SetToolTip(btnRechercherContact, "Rechercher un contact existant à rattacher.")
        _context.SetToolTip(btnReinitialiserContacts, "Réinitialiser la recherche de contacts.")
        _context.SetToolTip(btnReinitialiserIntervenants, "Réinitialiser la recherche d'intervenants.")
        _context.SetToolTip(btnAjouterSituationFamiliale, "Ajouter une situation familiale manquante au référentiel.")
        _context.SetToolTip(btnFicheMutualite, "Consulter la fiche de la mutualité choisie (coordonnées, plateforme), ou ajouter une mutualité.")
        _context.SetToolTip(txtNumeroAffiliation, "Numéro d'affiliation du patient auprès de sa mutualité.")
        _context.SetToolTip(txtCodeTitulaire1, "Code titulaire 1 (CT1) : 3 chiffres, figurant sur la vignette de mutualité.")
        _context.SetToolTip(txtCodeTitulaire2, "Code titulaire 2 (CT2) : 3 chiffres, figurant sur la vignette de mutualité.")

        ' Onglet Intervenants : boutons du haut (à ne pas confondre avec ceux du bas de la fiche)
        _context.SetToolTip(btnAjouterIntervenant, "Rattacher un intervenant (médecin, thérapeute…) à ce patient.")
        _context.SetToolTip(btnModifierIntervenant, "Modifier l'intervenant sélectionné dans la liste.")
        _context.SetToolTip(btnSupprimerIntervenant, "Détacher l'intervenant sélectionné de ce patient (l'intervenant reste dans le référentiel).")
        _context.SetToolTip(btnRechercherIntervenant, "Rechercher un intervenant existant à rattacher.")

        ' Onglet Dossiers : boutons du haut
        _context.SetToolTip(btnOuvrirDossier, "Ouvrir le dossier de suivi clinique sélectionné.")
        _context.SetToolTip(btnAjouterDossier, "Créer un nouveau dossier de suivi clinique pour ce patient.")

        ' Bandeau : bascule d'activation manuelle du patient (libellé dynamique)
        MettreAJourToolTipBasculeActif()

        _context.SetToolTip(btnUploadPhoto,
            "Importer la photo d'identité du patient." & Environment.NewLine &
            "Formats acceptés : JPG, PNG, BMP, GIF, TIFF.")

        _context.SetToolTip(txtTelephone,
            "Saisissez le numéro simplement (ex. 0475 12 34 56)." & Environment.NewLine &
            "Il sera mis en forme automatiquement selon le pays indiqué.")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreAJourToolTipBasculeActif
    '
    ' Rôle      :
    ' Ajuste l'infobulle du bouton de bascule d'activation en fonction de l'état courant du patient,
    ' afin que l'utilisateur comprenne l'effet exact du clic (activer ou désactiver manuellement).
    ' -------------------------------------------------------------------------------------------------
    Private Sub MettreAJourToolTipBasculeActif()

        If _context Is Nothing Then Exit Sub

        Dim message As String = If(_patient IsNot Nothing AndAlso _patient.IsActif,
            "Désactiver manuellement ce patient (le rendre inactif sans supprimer ses données).",
            "Réactiver manuellement ce patient (le rendre à nouveau actif).")

        _context.SetToolTip(btnBasculeActif, message)

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
    ' Version   : V1.3.0
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Recopie les propriétés d'un patient dans les contrôles de l'onglet Identité.
    '
    ' Paramètres :
    ' - patient : Patient source
    ' - V1.1.0 (19/10/2026) : l'anamnèse reçoit le patient pour l'en-tête de ses documents (PatientDocument).
    ' - V1.2.0 (20/10/2026) : latéralité par son identifiant.
    ' - V1.3.0 (24/10/2026, v2.56.0) : suggestions et avertissement de la localité actualisés.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RemplirChamps(patient As Patient)

        _chargementEnCours = True

        Try

            UtilsSaisieReferentiels.ChargerCivilites(cboCivilite, patient.IdCivilite)
            txtNom.Text = If(patient.Nom, String.Empty)
            txtPrenom.Text = If(patient.Prenom, String.Empty)

            dtpDateNaissance.Valeur = patient.DateNaissance

            RafraichirAge()

            Dim typeNiss As String = UtilsNiss.DeduireType(patient.Niss)
            txtNiss.Text = UtilsNiss.Formater(patient.Niss, typeNiss)
            cboTypeNiss.SelectedItem = typeNiss
            UtilsSaisieReferentiels.ChargerLateralites(cboLateralite,
                If(patient.IdLateralite.HasValue, CULng(patient.IdLateralite.Value), CType(Nothing, ULong?)))

            txtAdresseLigne1.Text = If(patient.AdresseLigne1, String.Empty)
            txtAdresseLigne2.Text = If(patient.AdresseLigne2, String.Empty)
            txtCodePostal.Text = If(patient.CodePostal, String.Empty)
            txtLocalite.Text = If(patient.Localite, String.Empty)
            UtilsSaisieReferentiels.ChargerPays(cboPays, patient.IdPays)
            txtTelephone.Text = UtilsTelephone.FormaterAffichage(patient.Telephone, patient.CodePays)
            txtEmail.Text = If(patient.Email, String.Empty)

            UtilsSaisieReferentiels.ChargerMutualites(cboMutualite, patient.IdOrganismeMutualite)
            txtNumeroAffiliation.Text = If(patient.NumeroAffiliation, String.Empty)
            txtCodeTitulaire1.Text = If(patient.CodeTitulaire1, String.Empty)
            txtCodeTitulaire2.Text = If(patient.CodeTitulaire2, String.Empty)

            If patient.IdSituationFamiliale.HasValue Then
                cboSituationFamiliale.SelectedValue = CULng(patient.IdSituationFamiliale.Value)
            Else
                ReinitialiserSituationFamiliale()
            End If

            If patient.IdPrixSeanceDefaut.HasValue Then
                cboPrixSeanceDefaut.SelectedValue = patient.IdPrixSeanceDefaut.Value
            Else
                cboPrixSeanceDefaut.SelectedIndex = 0
            End If

            rteAlerte.ChargerContenu(patient.AlerteRtf, patient.AlerteTxt)
            rteAnamnese.PatientDocument = EnTeteDocument.DesignerPatient(patient)
            rteAnamnese.ChargerContenu(patient.AnamneseRtf, patient.AnamneseTxt)

        Finally
            _chargementEnCours = False
        End Try

        _saisieLocalite?.Actualiser()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ViderChamps
    ' Version   : V1.2.0
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Réinitialise tous les contrôles de saisie de l'onglet Identité.
    ' - V1.1.0 (20/10/2026) : latéralité : liste du référentiel, « — » sélectionné.
    ' - V1.2.0 (24/10/2026, v2.56.0) : suggestions et avertissement de la localité actualisés.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ViderChamps()

        _chargementEnCours = True

        Try

            UtilsSaisieReferentiels.ChargerCivilites(cboCivilite, Nothing)
            txtNom.Clear()
            txtPrenom.Clear()
            dtpDateNaissance.Valeur = Nothing
            RafraichirAge()
            txtNiss.Clear()
            cboTypeNiss.SelectedItem = UtilsNiss.TYPE_BE
            UtilsSaisieReferentiels.ChargerLateralites(cboLateralite, Nothing)

            txtAdresseLigne1.Clear()
            txtAdresseLigne2.Clear()
            txtCodePostal.Clear()
            txtLocalite.Clear()
            UtilsSaisieReferentiels.ChargerPays(cboPays, Nothing)
            txtTelephone.Clear()
            txtEmail.Clear()

            UtilsSaisieReferentiels.ChargerMutualites(cboMutualite, Nothing)
            txtNumeroAffiliation.Clear()
            txtCodeTitulaire1.Clear()
            txtCodeTitulaire2.Clear()
            ReinitialiserSituationFamiliale()
            cboPrixSeanceDefaut.SelectedIndex = 0

            rteAlerte.RtfContent = String.Empty
            rteAnamnese.RtfContent = String.Empty

        Finally
            _chargementEnCours = False
        End Try

        _saisieLocalite?.Actualiser()

        _context?.ClearAllErrors()

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
    ' Version   : V1.1.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Met à jour le bandeau identité persistant : nom complet, code, statut, naissance, photo et
    ' alerte (D-Q12, D-Q13 ; carte du kit depuis UI-5).
    '
    ' Remarques :
    ' - En création, le bandeau affiche un libellé générique et le placeholder de photo.
    ' - La photo est chargée depuis le chemin déterministe (CheminsPatientHelper) si elle existe.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherBandeau()

        If _patient Is Nothing OrElse _idPatient <= 0 Then
            lblNomComplet.Text = "Nouveau patient"
            lblCodePatient.Text = "-"
            lblStatutPatient.Text = ""
            lblStatutPatient.Visible = False
            lblNaissanceBandeau.Text = String.Empty
            btnBasculeActif.Visible = False
            PlacerPastillesBandeau()
            AfficherAlerte(Nothing, Nothing)
            AfficherPhoto(Nothing)
            Return
        End If

        lblNomComplet.Text = If(String.IsNullOrWhiteSpace(_patient.NomComplet), "Patient", _patient.NomComplet)
        lblCodePatient.Text = If(_patient.CodePatient, "-")
        lblNaissanceBandeau.Text = LibelleNaissanceBandeau()

        AfficherStatutPatient()

        AfficherAlerte(_patient.AlerteRtf, _patient.AlerteTxt)
        AfficherPhoto(_patient.PhotoFichier)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherStatutPatient
    ' Version   : V1.1.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Affiche de façon succincte les deux statuts du patient dans le bandeau :
    ' l'activation manuelle (Actif/Inactif) et l'état du dernier dossier, ex. « Actif · dossier Actif ».
    ' Met à jour le libellé et la couleur du bouton de bascule (Désactiver en terre, Activer en sauge).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherStatutPatient()

        If _patient Is Nothing Then
            lblStatutPatient.Text = ""
            lblStatutPatient.Visible = False
            btnBasculeActif.Visible = False
            PlacerPastillesBandeau()
            Return
        End If

        Dim libelleActif As String = If(_patient.IsActif, "Actif", "Inactif")
        Dim libelleDossier As String = LibelleStatutDossierBandeau()

        lblStatutPatient.Text = $"{libelleActif} · {libelleDossier}"
        lblStatutPatient.Visible = True
        ' Patient inactif : pastille neutre (beige) plutôt que sauge.
        lblStatutPatient.CouleurFond = If(_patient.IsActif, UITheme.ColorGrisVertClair, UITheme.ColorBeige)
        lblStatutPatient.ForeColor = If(_patient.IsActif, UITheme.ColorSaugeTresFonce, UITheme.ColorGrisFonce)
        PlacerPastillesBandeau()

        btnBasculeActif.Visible = True
        btnBasculeActif.Text = If(_patient.IsActif, "Désactiver", "Activer")
        btnBasculeActif.Variante = If(_patient.IsActif, VarianteBouton.OutilDanger, VarianteBouton.Outil)

        MettreAJourToolTipBasculeActif()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : LibelleNaissanceBandeau
    ' Version   : V1.0.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Libellé « Né(e) le jj/mm/aaaa · N ans » du bandeau (vide si la date n'est pas renseignée).
    ' -------------------------------------------------------------------------------------------------
    Private Function LibelleNaissanceBandeau() As String

        If _patient Is Nothing OrElse Not _patient.DateNaissance.HasValue Then Return String.Empty

        Dim age As Integer = Patient.CalculerAge(_patient.DateNaissance.Value, Date.Today)
        Return $"Né(e) le {_patient.DateNaissance.Value:dd/MM/yyyy} · {age} an{If(age > 1, "s", String.Empty)}"

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : PlacerPastillesBandeau
    ' Version   : V1.0.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Aligne, sur la ligne sous le nom, la pastille du code, celle du statut puis la naissance :
    ' leurs largeurs dépendent du texte, leur position ne peut donc pas être figée dans le Designer.
    ' -------------------------------------------------------------------------------------------------
    Private Sub PlacerPastillesBandeau()

        Const Ecart As Integer = 6

        lblStatutPatient.Location = New Point(lblCodePatient.Right + Ecart, lblCodePatient.Top)

        Dim avant As Control = If(lblStatutPatient.Visible, CType(lblStatutPatient, Control), lblCodePatient)
        lblNaissanceBandeau.Location = New Point(avant.Right + 2 * Ecart,
                                                 lblCodePatient.Top + (lblCodePatient.Height - lblNaissanceBandeau.Height) \ 2)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : pnlAlerteBandeau_Paint
    ' Version   : V1.0.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Filet vertical séparant l'identité du bloc Alerte dans la carte du bandeau.
    ' -------------------------------------------------------------------------------------------------
    Private Sub pnlAlerteBandeau_Paint(sender As Object, e As PaintEventArgs) Handles pnlAlerteBandeau.Paint

        Using stylo As New Pen(UITheme.ColorBeige)
            e.Graphics.DrawLine(stylo, 0, 0, 0, pnlAlerteBandeau.Height)
        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : LibelleStatutDossierBandeau
    ' Version   : V1.0.0
    ' Date      : 14/07/2026
    '
    ' Rôle      :
    ' Fournit un libellé court de l'état du dernier dossier du patient pour le bandeau.
    ' -------------------------------------------------------------------------------------------------
    Private Function LibelleStatutDossierBandeau() As String

        Dim dossier As DossierListeItem = GestionDossiers.GetDossierDominant(_idPatient)

        If dossier Is Nothing Then
            Return "sans dossier"
        End If

        Dim libelle As String = If(String.IsNullOrWhiteSpace(dossier.LibelleStatutDossier),
                                   dossier.CodeStatutDossier,
                                   dossier.LibelleStatutDossier)

        Return $"dossier {libelle}"

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnBasculeActif_Click
    ' Version   : V1.0.0
    ' Date      : 14/07/2026
    '
    ' Rôle      :
    ' Bascule l'activation manuelle du patient (is_actif) puis rafraîchit le bandeau.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnBasculeActif_Click(sender As Object, e As EventArgs) Handles btnBasculeActif.Click

        If _patient Is Nothing OrElse _idPatient <= 0 Then
            Return
        End If

        Dim nouvelEtat As Boolean = Not _patient.IsActif

        Try

            GestionPatients.ChangerStatutActif(_idPatient, nouvelEtat)
            _patient.IsActif = nouvelEtat

            AfficherStatutPatient()
            RafraichirContexteNavigation()

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur bascule activation patient (UC_PatientFiche, id={_idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            MessageBox.Show(
                "Impossible de modifier l'état d'activation du patient.",
                "Erreur",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherAlerte
    ' Version   : V2.1.0
    ' Date      : 11/10/2026
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
    ' - Fond terre très léger quand une note existe (maquette « Fiche patient » validée le 11/10,
    '   en écho au récapitulatif de la fiche Dossier), beige clair sinon ; pas de texte rouge.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherAlerte(alerteRtf As String, alerteTxt As String)

        Dim aNote As Boolean = Not (String.IsNullOrWhiteSpace(alerteRtf) AndAlso String.IsNullOrWhiteSpace(alerteTxt))
        Dim fond As Color = If(aNote, AltheaDessin.Melanger(UITheme.ColorBlanc, UITheme.ColorRougeMoyenClair, 0.15F), UITheme.ColorBeigeClair)
        pnlAlerteFond.BackColor = fond
        rtbAlerte.BackColor = fond

        If Not aNote Then
            rtbAlerte.Clear()
            rtbAlerte.Text = "Aucune note."
            rtbAlerte.SelectAll()
            rtbAlerte.SelectionColor = UITheme.ColorGrisDiscret
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

        If _patient Is Nothing OrElse String.IsNullOrWhiteSpace(_patient.CodePatient) OrElse String.IsNullOrWhiteSpace(photoFichier) Then
            UtilsImages.RemplacerImage(picPhoto, Nothing)
            Return
        End If

        ' Le helper journalise lui-même un fichier illisible et renvoie Nothing.
        UtilsImages.RemplacerImage(picPhoto, CheminsPatientHelper.ChargerImagePatient(_patient.CodePatient, photoFichier))

    End Sub

#End Region

#Region "Gestion des modes"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerMode
    ' Version   : V1.1.0
    ' Date      : 19/10/2026
    '
    ' Rôle      :
    ' Applique le mode courant : état lecture seule / éditable des contrôles, visibilité des boutons,
    ' et activation progressive des onglets dépendant de l'id_patient (D-Q14).
    '
    ' Paramètres :
    ' - mode : Mode à appliquer
    ' - V1.1.0 (19/10/2026) : photo des champs en création / modification (GardeModifications).
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

        ' Création / modification : photo des champs une fois la fiche remplie ; consultation : rien à perdre.
        If editable Then
            _gardeFiche?.PhotographierApresChargement()
        Else
            _gardeFiche?.Oublier()
        End If

        ' En création, on démarre la saisie sur l'onglet Identité.
        ' En modification, on conserve l'onglet courant (l'utilisateur édite ce qu'il consulte).
        If mode = ModeFiche.Creation Then
            tabFiche.SelectedTab = tabPageIdentite
        End If

        RafraichirContexteNavigation()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DefinirEtatChampsIdentite
    ' Version   : V2.0.0
    ' Date      : 14/10/2026
    '
    ' Rôle      :
    ' Bascule les trois cartes de l'onglet Identité entre consultation et saisie.
    '
    ' Paramètres :
    ' - editable : True pour autoriser la saisie, False pour la lecture seule
    '
    ' Remarques :
    ' - V2.0.0 (UI-7) : règle « Consultation lisible » (UtilsControls.DefinirLectureSeule) au lieu
    '   d'un réglage champ par champ ; les listes et la date du kit restent lisibles en consultation,
    '   les zones de texte prennent le fond teinté (blanc en saisie).
    ' - Le bouton d'ajout de situation familiale suit la saisie (bouton de la carte) ; la fiche de la
    '   mutualité reste consultable dans tous les modes.
    ' -------------------------------------------------------------------------------------------------
    Private Sub DefinirEtatChampsIdentite(editable As Boolean)

        For Each carte As Control In {grpIdentite, grpCoordonnees, grpAdministratif}
            UtilsControls.DefinirLectureSeule(carte, Not editable)
        Next

        btnFicheMutualite.Enabled = True

        rteAlerte.ReadOnlyMode = Not editable

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : tabFiche_Selecting
    ' Version   : V1.1.0
    ' Date      : 19/10/2026
    '
    ' Rôle      :
    ' Empêche la sélection des onglets dépendant de l'id_patient tant que le patient n'est pas
    ' enregistré (activation progressive D-Q14).
    '
    ' Remarques :
    ' - En WinForms, TabPage.Enabled = False grise le contenu mais ne bloque PAS le clic sur l'onglet :
    '   il faut annuler la sélection ici (e.Cancel) pour un vrai verrouillage.
    ' - V1.1.0 (19/10/2026) : quitter l'onglet Anamnèse en cours de modification passe par PeutQuitterAnamnese.
    ' -------------------------------------------------------------------------------------------------
    Private Sub tabFiche_Selecting(
        sender As Object,
        e As TabControlCancelEventArgs
    ) Handles tabFiche.Selecting

        If e.TabPage Is Nothing Then
            Return
        End If

        ' Quitter l'onglet Anamnèse en cours de modification locale : ne rien perdre sans le dire.
        If tabFiche.SelectedTab Is tabPageAnamnese AndAlso e.TabPage IsNot tabPageAnamnese AndAlso
           Not PeutQuitterAnamnese() Then
            e.Cancel = True
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
    Private Sub RafraichirContexteNavigation(Optional action As String = Nothing)

        If _context Is Nothing Then
            Return
        End If

        Dim segmentPatient As String =
            If(_patient IsNot Nothing AndAlso _idPatient > 0 AndAlso Not String.IsNullOrWhiteSpace(_patient.NomComplet),
               _patient.NomComplet,
               "Nouveau patient")

        If _patient IsNot Nothing AndAlso _idPatient > 0 AndAlso Not String.IsNullOrWhiteSpace(_patient.CodePatient) Then
            segmentPatient &= $" [{_patient.CodePatient}]"
        End If

        ' Enrichit le segment patient avec les deux statuts succincts (activation + dernier dossier)
        If _patient IsNot Nothing AndAlso _idPatient > 0 Then
            Dim libelleActif As String = If(_patient.IsActif, "Actif", "Inactif")
            segmentPatient &= $" ({libelleActif}, {LibelleStatutDossierBandeau()})"
        End If

        Dim segmentOnglet As String =
            If(tabFiche.SelectedTab IsNot Nothing, tabFiche.SelectedTab.Text, String.Empty)

        Dim contexte As String = $"Patients > {segmentPatient}"

        If Not String.IsNullOrWhiteSpace(segmentOnglet) Then
            contexte &= $" > {segmentOnglet}"
        End If

        If _mode = ModeFiche.Modification Then
            contexte &= " > Modification"
        End If

        If Not String.IsNullOrWhiteSpace(action) Then
            contexte &= $" > {action}"
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
    ' Procédure : dtpDateNaissance_ValeurChangee
    ' Version   : V1.0.0
    ' Date      : 12/06/2026
    '
    ' Rôle      :
    ' Recalcule l'âge affiché lorsque la date de naissance change ou est effacée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dtpDateNaissance_ValeurChangee(
        sender As Object,
        e As EventArgs
    ) Handles dtpDateNaissance.ValeurChangee

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

        Dim age As Integer = Patient.CalculerAge(dateNaissance.Value, Date.Today)

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

#End Region

#Region "Validation et sauvegarde"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ValiderSaisie
    ' Version  : V2.0.0
    ' Date     : 23/10/2026
    '
    ' Rôle     :
    ' Fait juger la fiche par le métier (GestionPatients.ValiderPatient : champs obligatoires, téléphone
    ' et code postal selon le pays, e-mail, NISS et son unicité, CT1/CT2) et montre les erreurs.
    '
    ' Retour   :
    ' - Boolean : True si la saisie est valide, False sinon
    '
    ' Remarques :
    ' - V2.0.0 (23/10/2026, audit C.2, ADR-51) : plus de copie des règles ici ; chaque champ en cause
    '   reçoit son erreur (contexte de Home), une fenêtre les récapitule (AffichageErreursSaisie, décision
    '   de Joëlle) et le premier champ reçoit le focus.
    ' -------------------------------------------------------------------------------------------------
    Private Function ValiderSaisie() As Boolean

        _context?.ClearAllErrors()

        Dim saisie As New Patient With {.IdPatient = _idPatient}
        AlimenterPatientDepuisChamps(saisie)

        Dim erreurs As List(Of ErreurSaisie) = GestionPatients.ValiderPatient(saisie)

        If Not AffichageErreursSaisie.Signaler(erreurs, ChampsDeLaFiche(), Sub(c, m) _context?.SetError(c, m), "Fiche patient") Then
            Return True
        End If

        _context?.SetStatus($"Fiche patient : {erreurs.Count} point(s) à corriger.")
        Return False

    End Function

    ' Contrôle de chaque propriété du patient jugée par ValiderPatient (ErreurSaisie.Champ).
    Private Function ChampsDeLaFiche() As Dictionary(Of String, Control)
        Return New Dictionary(Of String, Control) From {
            {NameOf(Patient.Nom), txtNom},
            {NameOf(Patient.Prenom), txtPrenom},
            {NameOf(Patient.Telephone), txtTelephone},
            {NameOf(Patient.CodePostal), txtCodePostal},
            {NameOf(Patient.Email), txtEmail},
            {NameOf(Patient.Niss), txtNiss},
            {NameOf(Patient.CodeTitulaire1), txtCodeTitulaire1},
            {NameOf(Patient.CodeTitulaire2), txtCodeTitulaire2}
        }
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ConfirmerDoublonEventuel
    ' Version  : V1.1.0
    ' Date     : 23/10/2026
    '
    ' Rôle     :
    ' Détecte un doublon potentiel (mêmes nom et prénom, date de naissance identique ou inconnue d'un
    ' côté ou de l'autre) et demande confirmation.
    ' - V1.1.0 (23/10/2026, relevé de Joëlle) : un homonyme sans date de naissance n'était pas signalé.
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
            "Un patient portant les mêmes nom et prénom existe déjà (même date de naissance, ou date non renseignée)." & Environment.NewLine &
            "Voulez-vous quand même enregistrer ce patient ?",
            "Doublon potentiel"
        )

        Return reponse = DialogResult.Yes

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : Enregistrer
    ' Version   : V1.3.0
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Valide la saisie, alimente le patient depuis les contrôles, puis crée ou met à jour le patient.
    '
    ' Remarques :
    ' - En création réussie, la fiche bascule en consultation sur le patient nouvellement créé.
    ' - Les onglets dépendant de l'id deviennent alors actifs (activation progressive D-Q14).
    ' - V1.1.0 (19/10/2026) : renvoie True si le patient est enregistré (choix « Enregistrer » avant de
    '   quitter la fiche).
    ' - V1.2.0 (23/10/2026, audit C.2) : un refus du métier (ErreurValidationException…) s'affiche dans
    '   une fenêtre, la fiche reste en saisie.
    ' - V1.3.0 (24/10/2026, v2.56.0) : localité qui ne correspond pas au code postal belge : confirmation
    '   (« Enregistrer quand même » / « Corriger »).
    ' -------------------------------------------------------------------------------------------------
    Private Function Enregistrer() As Boolean

        If Not ValiderSaisie() Then
            ' Le message explicite (champs fautifs) est déjà posé dans le statut par ValiderSaisie().
            Return False
        End If

        If Not _saisieLocalite.ConfirmerLocalite() Then
            _context?.SetStatus("Enregistrement annulé : localité à corriger.")
            Return False
        End If

        If Not ConfirmerDoublonEventuel() Then
            If _context IsNot Nothing Then
                _context.SetStatus("Enregistrement annulé.")
            End If
            Return False
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
                    Return False
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
            _context?.ClearAllErrors()
            AfficherBandeau()
            AppliquerMode(ModeFiche.Consultation)
            RafraichirContexteNavigation()

            If _context IsNot Nothing Then
                _context.SetStatus($"Patient enregistré : {_patient.NomComplet}.")
            End If

            Return True

        Catch exRefus As InvalidOperationException

            ' Refus du métier (fiche modifiée entre-temps, NISS pris par un autre poste…) : message
            ' rédigé pour l'utilisatrice ; la fiche reste en saisie.
            DialogChoix.Information(exRefus.Message, "Enregistrement impossible")
            _context?.SetStatus("Enregistrement du patient refusé.")
            Return False

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur enregistrement du patient (UC_PatientFiche, id={_idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur("Une erreur est survenue lors de l'enregistrement du patient.")
            Return False

        Finally
            Cursor = Cursors.Default
        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AlimenterPatientDepuisChamps
    ' Version   : V1.2.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Recopie les valeurs des contrôles de l'onglet Identité dans l'objet patient.
    '
    ' Paramètres :
    ' - patient : Patient cible à alimenter
    '
    ' Remarques :
    ' - Les champs vides sont normalisés à Nothing pour rester cohérents avec le stockage.
    ' - V1.1.0 (20/10/2026) : latéralité par son identifiant.
    ' - V1.2.0 (23/10/2026, audit C.2) : NISS normalisé (UtilsNiss.Normaliser) et type d'identifiant
    '   transmis au métier (Patient.TypeIdentifiantNiss).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AlimenterPatientDepuisChamps(patient As Patient)

        patient.IdCivilite = UtilsSaisieReferentiels.IdCiviliteSaisie(cboCivilite)
        patient.Nom = txtNom.Text.Trim()
        patient.Prenom = txtPrenom.Text.Trim()
        patient.DateNaissance = SaisieDateNaissance()
        patient.Niss = UtilsNiss.Normaliser(txtNiss.Text)
        patient.TypeIdentifiantNiss = cboTypeNiss.Text
        Dim idLateralite As ULong? = UtilsSaisieReferentiels.IdLateraliteSaisie(cboLateralite)
        patient.IdLateralite = If(idLateralite.HasValue, CLng(idLateralite.Value), CType(Nothing, Long?))

        patient.AdresseLigne1 = NormaliserTexte(txtAdresseLigne1.Text)
        patient.AdresseLigne2 = NormaliserTexte(txtAdresseLigne2.Text)
        patient.CodePostal = NormaliserTexte(txtCodePostal.Text)
        patient.Localite = NormaliserTexte(txtLocalite.Text)
        patient.IdPays = UtilsSaisieReferentiels.IdPaysSaisi(cboPays)
        patient.Telephone = NormaliserTexte(UtilsTelephone.NormaliserE164(txtTelephone.Text, CodePaysSaisi()))
        patient.Email = NormaliserTexte(txtEmail.Text)

        patient.IdOrganismeMutualite = UtilsSaisieReferentiels.IdMutualiteSaisie(cboMutualite)
        patient.NumeroAffiliation = NormaliserTexte(txtNumeroAffiliation.Text)
        patient.CodeTitulaire1 = NormaliserTexte(txtCodeTitulaire1.Text)
        patient.CodeTitulaire2 = NormaliserTexte(txtCodeTitulaire2.Text)
        patient.IdSituationFamiliale = SaisieSituationFamiliale()
        patient.IdPrixSeanceDefaut = SaisiePrixSeanceDefaut()

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
    ' Retourne la date de naissance saisie, ou Nothing si le champ (facultatif) est vide.
    '
    ' Retour   :
    ' - Date? : Date de naissance ou Nothing
    ' -------------------------------------------------------------------------------------------------
    Private Function SaisieDateNaissance() As Date?

        Return dtpDateNaissance.Valeur

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
    ' Fonction : SaisiePrixSeanceDefaut
    ' Version  : V1.0.0
    ' Date     : 20/01/2025
    '
    ' Rôle     :
    ' Récupère l'identifiant du tarif séance sélectionné dans le combo, ou Nothing si non renseigné.
    '
    ' Retour   :
    ' - ULong? : Identifiant du prix séance if > 0, sinon Nothing
    ' -------------------------------------------------------------------------------------------------
    Private Function SaisiePrixSeanceDefaut() As ULong?

        ' Quand le combobox est bindé par DataSource, SelectedValue ne peut pas être Nothing
        ' mais peut être un objet PrixSeance avec IdPrixSeance = 0 (entrée vide)
        If cboPrixSeanceDefaut.SelectedValue Is Nothing Then
            Return Nothing
        End If

        ' Essayer de récupérer l'ID depuis la valeur sélectionnée
        Dim idPrix As ULong = 0
        Dim selectedValueStr = cboPrixSeanceDefaut.SelectedValue.ToString()

        ' SelectedValue est un ULong (ValueMember="IdPrixSeance")
        If ULong.TryParse(selectedValueStr, idPrix) Then
            ' Si idPrix > 0, c'est un vrai tarif ; sinon c'est l'entrée vide (0)
            If idPrix > 0 Then
                Return idPrix
            End If
        End If

        ' Aucune sélection ou entrée vide sélectionnée
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

        If saisie.Length > 0 Then
            txtTelephone.Text = UtilsTelephone.FormaterAffichage(saisie, CodePaysSaisi())
        End If

        ControlerChampsPays()

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

        If saisie.Length > 0 Then
            txtTelephone.Text = UtilsTelephone.FormaterAffichage(saisie, CodePaysSaisi())
        End If

        ControlerChampsPays()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ControlerChampsPays
    ' Version   : V1.1.0
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Signale immédiatement (ErrorProvider) les anomalies de téléphone et de code postal selon le pays
    ' de l'adresse. Non bloquant ici : le blocage intervient à l'enregistrement (ValiderSaisie).
    ' - V1.1.0 (24/10/2026, v2.56.0) : code postal belge inexistant signalé ; localité qui ne correspond
    '   pas au code : avertissement (SaisieLocaliteBelge).
    ' -------------------------------------------------------------------------------------------------
    Private Sub ControlerChampsPays()

        If _chargementEnCours OrElse _context Is Nothing Then
            Return
        End If

        Dim message As String = String.Empty

        UtilsTelephone.Valider(txtTelephone.Text, CodePaysSaisi(), message)
        _context.SetError(txtTelephone, message)

        _context.SetError(txtCodePostal, _saisieLocalite.MessageCodePostal())
        _saisieLocalite.Actualiser()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ControlerNiss
    ' Version   : V1.1.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Signale immédiatement (ErrorProvider) un NISS invalide selon le type d'identifiant choisi ; un
    ' NISS valide est remis en forme (« 85.07.30-033.28 », V1.1.0 du 23/10/2026).
    ' -------------------------------------------------------------------------------------------------
    Private Sub ControlerNiss()

        If _chargementEnCours OrElse _context Is Nothing Then
            Return
        End If

        Dim message As String = String.Empty
        If UtilsNiss.Valider(txtNiss.Text, cboTypeNiss.Text, message) Then
            Dim forme As String = UtilsNiss.Formater(txtNiss.Text, cboTypeNiss.Text)
            If forme <> txtNiss.Text Then txtNiss.Text = forme
        End If
        _context.SetError(txtNiss, message)

    End Sub

    Private Sub txtCodePostal_Leave(sender As Object, e As EventArgs) Handles txtCodePostal.Leave
        ControlerChampsPays()
    End Sub

    Private Sub txtNiss_Leave(sender As Object, e As EventArgs) Handles txtNiss.Leave
        ControlerNiss()
    End Sub

    Private Sub txtEmail_Leave(sender As Object, e As EventArgs) Handles txtEmail.Leave

        If _chargementEnCours OrElse _context Is Nothing Then
            Return
        End If

        Dim message As String = String.Empty
        UtilsValidation.IsValidEmail(txtEmail.Text, message)
        _context.SetError(txtEmail, message)

    End Sub

    Private Sub cboTypeNiss_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTypeNiss.SelectedIndexChanged
        ControlerNiss()
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
    ' Version   : V1.1.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Importe la photo d'identité du patient : choix et contrôle du fichier (import d'image unifié,
    ' UtilsImages), confirmation si une photo existe, copie sous le nom fixe « Identite.ext », mise
    ' à jour du nom de fichier en base (patients.photo_fichier) et affichage immédiat dans le bandeau.
    '
    ' Remarques :
    ' - L'action n'est possible que sur un patient enregistré (bouton désactivé sinon).
    ' - Le dossier du patient est créé à la demande (AssurerDossierPatient).
    ' - La photo garde un nom fixe : une nouvelle photo remplace la précédente (D-Q13 / §8.2).
    ' - Formats et contrôle du fichier communs à toute l'application (ImagesHelper).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnUploadPhoto_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnUploadPhoto.Click

        If _patient Is Nothing OrElse _idPatient <= 0 OrElse String.IsNullOrWhiteSpace(_patient.CodePatient) Then
            Return
        End If

        Dim cheminSource As String = UtilsImages.ChoisirFichierImage(FindForm(), "Sélectionner la photo d'identité")

        If String.IsNullOrEmpty(cheminSource) Then
            Return
        End If

        ' Une photo existante n'est jamais écrasée sans confirmation.
        If Not UtilsImages.ConfirmerRemplacement(_patient.PhotoFichier, "la photo d'identité de ce patient") Then
            Return
        End If

        Try

            ' Libère l'image affichée avant la copie pour ne pas verrouiller le fichier cible.
            UtilsImages.RemplacerImage(picPhoto, Nothing)

            ' Copie sous le nom fixe « Identite » (anciennes versions supprimées, dossier créé au besoin).
            Dim nomFichier As String = CheminsPatientHelper.ImporterPhotoIdentite(_patient.CodePatient, cheminSource)

            ' Met à jour le check en base (nom seul) puis l'objet patient en mémoire.
            GestionPatients.UpdatePhotoPatient(_idPatient, nomFichier)
            _patient.PhotoFichier = nomFichier

            ' Affiche la nouvelle photo dans le bandeau.
            AfficherPhoto(nomFichier)

            ' Trace la photo d'identité dans la table documents (niveau patient, type PHOTO_IDENTITE).
            TracerPhotoIdentite(nomFichier, Path.GetExtension(cheminSource))

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
            ' Nom déterministe SANS horodatage : l'anamnèse est unique par patient.
            ' Chaque export écrase la version précédente (une seule dernière version conservée).
            Dim nomFichier As String = $"anamnese_{_patient.CodePatient}{extension}"

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
    ' - Trace le document exporté dans la table documents (niveau patient, type ANAMNESE).
    ' -------------------------------------------------------------------------------------------------
    Private Sub rteAnamnese_ExportCompleted(
        sender As Object,
        e As ExportCompletedEventArgs
    ) Handles rteAnamnese.ExportCompleted

        If Not e.Success OrElse String.IsNullOrWhiteSpace(e.DestinationPath) Then
            Return
        End If

        TracerAnamneseExportee(e)

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
    ' Procédure : TracerAnamneseExportee
    ' Version   : V1.0.0
    ' Date      : 09/06/2026
    '
    ' Rôle      :
    ' Insère une trace dans la table documents pour l'export de l'anamnèse (niveau patient).
    '
    ' Remarques :
    ' - Type de document : ANAMNESE.
    ' - chemin_relatif : relatif à PATH_GENERAL (Documents/{code_patient}/{fichier}).
    ' - type_source_document : PDF ou LOCAL_DOCX selon le format exporté.
    ' - Best-effort : une erreur de traçabilité ne doit pas empêcher l'ouverture du fichier exporté.
    ' -------------------------------------------------------------------------------------------------
    Private Sub TracerAnamneseExportee(e As ExportCompletedEventArgs)

        If _patient Is Nothing OrElse _idPatient <= 0 OrElse String.IsNullOrWhiteSpace(_patient.CodePatient) Then
            Return
        End If

        Try

            Dim idType As ULong = GestionDocuments.ResoudreIdTypeDocumentParCode(CodesSysteme.TypesDocument.Anamnese)

            If idType = 0UL Then
                GestionLog.EcrireLog(
                    "Type de document ANAMNESE introuvable : trace documentaire ignorée.",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.UI)
                Return
            End If

            Dim nomFichier As String = System.IO.Path.GetFileName(e.DestinationPath)
            Dim extension As String = System.IO.Path.GetExtension(e.DestinationPath).TrimStart("."c)

            Dim doc As New Document With {
                .IdPatient = CULng(_idPatient),
                .IdTypeDocument = idType,
                .TypeSourceDocument = If(e.Format = ExportFormat.Word, CodesSysteme.SourcesDocument.LocalDocx, CodesSysteme.SourcesDocument.Pdf),
                .NomDocument = "Anamnèse",
                .NomFichier = nomFichier,
                .ExtensionFichier = extension,
                .CheminRelatif = CheminsPatientHelper.GetCheminRelatifFichierPatient(_patient.CodePatient, nomFichier),
                .DateDocument = Date.Today
            }

            GestionDocuments.TracerDocument(doc)

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur traçabilité document export anamnèse (UC_PatientFiche, id={_idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : TracerPhotoIdentite
    ' Version   : V1.0.0
    ' Date      : 13/07/2026
    '
    ' Rôle      :
    ' Insère (ou met à jour) une trace dans la table documents pour la photo d'identité du patient
    ' (niveau patient, type PHOTO_IDENTITE).
    '
    ' Paramètres :
    ' - nomFichier : Nom du fichier déterministe (ex. "identite.jpg").
    ' - extension  : Extension du fichier source (ex. ".jpg").
    '
    ' Remarques :
    ' - Photo d'identité unique par patient : nom déterministe + upsert (cf. ADR-23).
    ' - est_document_image_metier = True (image, non document rédactionnel).
    ' - chemin_relatif relatif à PATH_GENERAL (Documents/{code_patient}/{fichier}).
    ' - Best-effort : une erreur de traçabilité ne doit pas empêcher l'import de la photo.
    ' -------------------------------------------------------------------------------------------------
    Private Sub TracerPhotoIdentite(nomFichier As String, extension As String)

        If _patient Is Nothing OrElse _idPatient <= 0 OrElse String.IsNullOrWhiteSpace(_patient.CodePatient) Then
            Return
        End If

        Try

            Dim idType As ULong = GestionDocuments.ResoudreIdTypeDocumentParCode(CodesSysteme.TypesDocument.PhotoIdentite)

            If idType = 0UL Then
                GestionLog.EcrireLog(
                    "Type de document PHOTO_IDENTITE introuvable : trace documentaire ignorée.",
                    GestionLog.LogLevel.Succinct,
                    GestionLog.LogCategory.UI)
                Return
            End If

            Dim ext As String = If(extension, String.Empty).TrimStart("."c).ToLowerInvariant()

            Dim doc As New Document With {
                .IdPatient = CULng(_idPatient),
                .IdTypeDocument = idType,
                .TypeSourceDocument = CodesSysteme.SourcesDocument.Local,
                .EstDocumentImageMetier = True,
                .NomDocument = "Photo d'identité",
                .NomFichier = nomFichier,
                .ExtensionFichier = ext,
                .CheminRelatif = CheminsPatientHelper.GetCheminRelatifFichierPatient(_patient.CodePatient, nomFichier),
                .DateDocument = Date.Today
            }

            GestionDocuments.TracerDocumentParPatientEtType(doc)

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur traçabilité photo d'identité (UC_PatientFiche, id={_idPatient}).",
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
    ' Version   : V1.1.0
    ' Date      : 19/10/2026
    '
    ' Rôle      :
    ' Enregistre uniquement l'anamnèse du patient courant (persistance ciblée), met à jour le patient
    ' en mémoire, puis revient en consultation locale (éditeur en lecture seule).
    '
    ' Remarques :
    ' - Utilise GestionPatients.UpdateAnamnesePatient (mise à jour ciblée des colonnes d'anamnèse).
    ' - N'altère pas les autres champs de la fiche ni le mode global.
    ' - V1.1.0 (19/10/2026) : enregistrement extrait dans EnregistrerAnamnese (réutilisé avant de quitter).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEnregistrerAnamnese_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnEnregistrerAnamnese.Click

        EnregistrerAnamnese()

    End Sub

    ' Enregistre l'anamnèse seule ; True si elle est enregistrée (bouton, ou « Enregistrer » avant de quitter).
    Private Function EnregistrerAnamnese() As Boolean

        If _idPatient <= 0 OrElse _patient Is Nothing Then
            Return False
        End If

        Try
            Cursor = Cursors.WaitCursor

            Dim anamneseRtf As String = NormaliserTexte(rteAnamnese.RtfContent)
            Dim anamneseTxt As String = NormaliserTexte(rteAnamnese.TextContent)

            GestionPatients.UpdateAnamnesePatient(_idPatient, anamneseRtf, anamneseTxt)

            ' Synchronise le patient en mémoire pour rester cohérent avec la base.
            _patient.AnamneseRtf = anamneseRtf
            _patient.AnamneseTxt = anamneseTxt
            rteAnamnese.MarquerEnregistre()

            _modeEditionAnamnese = False
            _anamneseModifiee = False
            AppliquerModeAnamnese()

            If _context IsNot Nothing Then
                _context.SetStatus("Anamnèse enregistrée.")
            End If

            Return True

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur enregistrement de l'anamnèse (UC_PatientFiche, id={_idPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur("Une erreur est survenue lors de l'enregistrement de l'anamnèse.")
            Return False

        Finally
            Cursor = Cursors.Default
        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAnnulerAnamnese_Click
    ' Version   : V1.1.0
    ' Date      : 19/10/2026
    '
    ' Rôle      :
    ' Annule l'édition locale de l'anamnèse : restaure le contenu d'origine (depuis le patient en
    ' mémoire) et revient en consultation locale (éditeur en lecture seule).
    '
    ' Remarques :
    ' - Demande confirmation seulement si des modifications ont réellement été saisies (dirty-tracking).
    ' - V1.1.0 (19/10/2026) : question commune DialogChoix.ConfirmerAbandon ; abandon extrait dans AbandonnerAnamnese.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAnnulerAnamnese_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnAnnulerAnamnese.Click

        If _anamneseModifiee AndAlso Not DialogChoix.ConfirmerAbandon("de l'anamnèse") Then
            Return
        End If

        AbandonnerAnamnese()

    End Sub

    ' Rend à l'anamnèse son contenu enregistré et revient en consultation locale (sans question).
    Private Sub AbandonnerAnamnese()

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
    ' Version   : V1.0.1
    ' Date      : 25/10/2026
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
    ' - V1.0.1 (25/10/2026, audit C.4, ADR-55) : fenêtre principale lue dans le contexte
    '   (IFenetrePrincipale) au lieu de TryCast(FindForm(), Home).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterSituationFamiliale_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnAjouterSituationFamiliale.Click

        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale

        If fenetre Is Nothing Then
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
            fenetre.OuvrirReferentielModal(New UC_SituationsFamiliales(), "Situations familiales")

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
    ' Procédure : btnFicheMutualite_Click
    ' Version   : V1.0.1
    ' Date      : 25/10/2026
    '
    ' Rôle      :
    ' Ouvre le référentiel des mutualités en fenêtre modale (via Home, qui gère droits et élévation),
    ' positionné sur la mutualité choisie : consultation de sa fiche complète (coordonnées, plateforme,
    ' procédure) ou ajout d'une nouvelle mutualité, sans quitter la fiche Patient.
    '
    ' Remarques :
    ' - Disponible aussi en consultation (lecture de la fiche) ; la liste est alors rechargée sans
    '   changer la mutualité du patient.
    ' - En saisie, une mutualité nouvellement ajoutée est sélectionnée automatiquement.
    ' - V1.0.1 (25/10/2026, audit C.4, ADR-55) : fenêtre principale lue dans le contexte
    '   (IFenetrePrincipale) au lieu de TryCast(FindForm(), Home).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnFicheMutualite_Click(sender As Object, e As EventArgs) Handles btnFicheMutualite.Click

        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale
        If fenetre Is Nothing Then Return

        Dim idCourant As ULong? = UtilsSaisieReferentiels.IdMutualiteSaisie(cboMutualite)
        Dim idsAvant As HashSet(Of ULong) = UtilsSaisieReferentiels.IdsMutualites(cboMutualite)

        If Not fenetre.OuvrirReferentielModal(New UC_Organismes(TypeOrganisme.Mutualite, idCourant), "Mutualités") Then
            Return
        End If

        ' Une mutualité a pu être ajoutée, renommée ou désactivée pendant la fenêtre modale.
        UtilsSaisieReferentiels.ChargerMutualites(cboMutualite, idCourant)

        If cboMutualite.Enabled Then
            Dim nouvelles = UtilsSaisieReferentiels.IdsMutualites(cboMutualite).Except(idsAvant).ToList()
            If nouvelles.Count = 1 Then
                cboMutualite.SelectedValue = nouvelles(0)
                _context?.SetStatus("Nouvelle mutualité ajoutée et sélectionnée.")
            End If
        End If

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
    ' Version   : V1.1.0
    ' Date      : 19/10/2026
    '
    ' Rôle      :
    ' Annule la saisie en cours et revient à l'état consultation (ou retourne à la liste en création).
    ' - V1.1.0 (19/10/2026) : en modification, Abandonner / Continuer la saisie s'il y a des modifications.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAnnuler_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnAnnuler.Click

        If _idPatient > 0 Then

            ' Même question que tout bouton Annuler d'une saisie modifiée (rien demandé sans modification).
            If _gardeFiche IsNot Nothing AndAlso Not _gardeFiche.ConfirmerAnnulation(DeQuoiFiche()) Then
                Return
            End If

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
    ' Version   : V1.1.1
    ' Date      : 25/10/2026
    '
    ' Rôle      :
    ' Demande à Home de revenir à l'écran précédent (D-Q15) ; à défaut, recharge l'accueil patients.
    ' - V1.1.0 (19/10/2026) : la question des modifications non enregistrées est posée par Home (PeutQuitter).
    ' - V1.1.1 (25/10/2026, audit C.4, ADR-55) : fenêtre principale lue dans le contexte
    '   (IFenetrePrincipale) au lieu de TryCast(FindForm(), Home).
    ' -------------------------------------------------------------------------------------------------
    Private Sub RetournerListe()

        ' Les modifications non enregistrées sont contrôlées par Home avant de quitter la fiche
        ' (PeutQuitter, IUnsavedChangesAware), comme pour le menu et la fermeture d'Althéa.

        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale

        If fenetre Is Nothing Then
            Return
        End If

        If Not fenetre.NavigateRetour() Then
            fenetre.NavigateToPatients()
        End If

    End Sub

#End Region

#Region "Modifications non enregistrées"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : PeutQuitter
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       :
    ' Appelée par Home avant de quitter la fiche (Retour, menu, fermeture d'Althéa) : propose
    ' d'enregistrer l'anamnèse en cours de modification locale, puis la fiche en création ou en
    ' modification.
    '
    ' Retour     :
    ' - True  : rien à perdre, enregistré, ou « Quitter sans enregistrer »
    ' - False : « Rester », ou l'enregistrement n'a pas abouti (saisie refusée, erreur)
    '
    ' Remarques  :
    ' - « Quitter sans enregistrer » oublie les modifications : une seconde demande pendant la même
    '   navigation (Retour sans historique puis accueil Patients) ne repose pas la question.
    ' -------------------------------------------------------------------------------------------------
    Public Function PeutQuitter() As Boolean Implements IUnsavedChangesAware.PeutQuitter

        If Not PeutQuitterAnamnese() Then Return False

        If _gardeFiche Is Nothing Then Return True

        Return _gardeFiche.PeutQuitter(DeQuoiFiche(), AddressOf Enregistrer)

    End Function

    ' Désignation de la fiche dans les questions (« Les modifications de la fiche du patient… »).
    Private Function DeQuoiFiche() As String
        Return If(_mode = ModeFiche.Creation, "du nouveau patient", "de la fiche du patient")
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : PeutQuitterAnamnese
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Anamnèse en cours de modification locale et modifiée : Enregistrer (enregistrement
    '              ciblé), Quitter sans enregistrer (contenu enregistré rétabli) ou Rester.
    ' Retour     : True si l'on peut quitter l'anamnèse.
    ' -------------------------------------------------------------------------------------------------
    Private Function PeutQuitterAnamnese() As Boolean

        If Not (_modeEditionAnamnese AndAlso _anamneseModifiee) Then Return True

        Select Case DialogChoix.DemanderAvantDepart("de l'anamnèse")
            Case ChoixAvantDepart.Enregistrer
                Return EnregistrerAnamnese()
            Case ChoixAvantDepart.QuitterSansEnregistrer
                AbandonnerAnamnese()
                Return True
            Case Else
                Return False
        End Select

    End Function

#End Region

End Class
