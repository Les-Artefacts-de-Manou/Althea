' -------------------------------------------------------------------------------------------------
' UserControl : UC_PatientHome
' Projet      : Althéa
' Version     : V1.7.2
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' UserControl d'accueil des patients permettant de rechercher et de lister les patients.
' Chargé dynamiquement dans Home via NavigationManager.
'
' Responsabilités :
' - Afficher la liste des patients (triés par date de modification décroissante)
' - Permettre la recherche multi-critères en mémoire (nom, prénom, NISS, code, contact)
' - Filtrer les patients ayant au moins un dossier actif
' - Préparer l'ouverture de la fiche patient (création / modification / consultation)
' - Utiliser le contexte UI partagé fourni par Home
' - Journaliser les actions via GestionLog
'
' Remarques   :
' - Implémente IContextAwareUserControl
' - Aucun accès direct à la base de données
' - Toute logique métier passe par GestionPatients
' - V1.1.0 (17/10/2026) : colonne Paiements — infobulles « tous dossiers » ; avoir rappelé même
'   quand le patient doit de l'argent.
' - V1.1.1 (17/10/2026) : codes de statut de dossier repris de GestionDossiers (filtre, icônes)
'   au lieu d'être écrits en dur.
' - V1.2.0 (17/10/2026, grille « hub de la journée », étape 2) : colonnes Âge, Suivi (pastille +
'   domaine), Prochain RDV, Dernière séance, Paiements et Abs. en pastilles, Alerte en icône avec le
'   texte en infobulle ; ordre par défaut en trois groupes séparés (aujourd'hui / à venir / sans
'   rendez-vous) ; ligne du jour teintée ; patient inactif grisé. Naissance, NISS, Email, Photo,
'   Actif et Dossiers retirés de l'affichage.
' - V1.3.0 (17/10/2026) : double-clic sur « Prochain RDV » ou « Dernière séance » = ouvrir cette
'   séance (prise de notes) ; ailleurs, la fiche patient comme avant.
' - V1.4.0 (17/10/2026, étape 3) : résumé cliquable au-dessus de la grille (aujourd'hui, sous 7 jours,
'   suivis sans rendez-vous, à régler, avoir, absences, alertes — ResumeFiltre) ; filtre « Dossier »
'   limité au statut du dossier ; patients inactifs masqués par défaut (« Afficher les inactifs »).
' - V1.4.1 (17/10/2026, étape 4) : icônes de la liste « Dossier » dessinées (UtilsIcons V1.1.0).
' - V1.4.2 (17/10/2026) : polices standard des grilles (Calibri 10, en-têtes 10 gras), au lieu des
'   tailles réduites qu'imposait l'ancien nombre de colonnes.
' - V1.5.0 (17/10/2026, étape 5) : panneau d'aperçu à droite de la grille (UC_ApercuPatient), ouvert
'   par défaut, rempli au clic sur une ligne ; ouverture de la fiche, du dossier ou d'une séance
'   depuis le panneau ; Âge, Dernière séance, Abs. et Téléphone masqués quand il est ouvert.
' - V1.5.1 (19/10/2026) : l'aperçu ne se recharge que si le patient sélectionné change (clignotement
'   en boucle constaté le 19/10/2026 : la grille relève SelectionChanged sans changement de ligne,
'   le rechargement de l'aperçu relançait la minuterie — même cause que Tarifs / Natures le 16/10).
'   Pagination : place réservée aux deux repères de groupe possibles d'une page (HauteurReservee).
' - Les actions Nouveau / Modifier / Ouvrir sont des points d'entrée vers UC_PatientFiche
'   (écran non encore implémenté : statut informatif en attendant)
'
' Dépendances :
' - GestionPatients
' - PatientListeItem
' - UserControlContext
' - UtilsButtons
' - UtilsDataGrid
' - GestionLog
'
' Interface   :
' - IContextAwareUserControl : injection du contexte UI partagé
' - V1.6.0 (23/10/2026, audit C.2) : recherche sur le NISS sous sa forme normalisée (celle de la base
'   depuis v2.54.0) : « 85.07.30 » trouve le patient comme « 850730 ».
' - V1.6.1 (24/10/2026, audit C.3) : codes de statut de dossier lus dans
'   CodesSysteme.StatutsDossier.
' - V1.7.0 (24/10/2026, relevé de Joëlle) : patient inactif marqué d'une pastille « Inactif » devant
'   son nom (la mention « (inactif) » après le prénom était coupée) ; la case « Afficher les inactifs »
'   devient la liste « Patients : Actifs / Inactifs / Tous » (on peut ne voir que les inactifs).
' - V1.7.1 (25/10/2026, relevé aux captures) : texte d'une ligne inactive sélectionnée en
'   ColorGrisDiscret, comme les autres grilles (ColorGrisClair était illisible sur la sélection).
' - V1.7.2 (25/10/2026, audit C.4, ADR-55) : la fenêtre principale est lue dans le contexte
'   (_context.FenetrePrincipale, IFenetrePrincipale) ; plus aucune référence à Home.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_PatientHome
    Implements IContextAwareUserControl

#Region "Variables privées"

    ' Index des items du filtre « Dossier » (cboFiltreSuivi) : statut du dossier dominant. L'ordre doit
    ' correspondre à InitialiserFiltres(). Les autres axes (situation, inactifs) ont leurs propres
    ' contrôles depuis la V1.4.0.
    Private Const FiltreTous As Integer = 0
    Private Const FiltreDossierActif As Integer = 1
    Private Const FiltreDossierPause As Integer = 2
    Private Const FiltreDossierCloture As Integer = 3
    Private Const FiltreDossierArchive As Integer = 4
    Private Const FiltreSansDossier As Integer = 5

    ' Index des items du filtre « Patients » (cboFiltrePatients) : actifs (par défaut), inactifs
    ' seulement, tous (décision de Joëlle, 24/10/2026 ; remplace la case « Afficher les inactifs »).
    Private Const FiltrePatientsActifs As Integer = 0
    Private Const FiltrePatientsInactifs As Integer = 1
    Private Const FiltrePatientsTous As Integer = 2

    ' Libellé de l'item neutre du filtre par domaine (aucun domaine imposé)
    Private Const FiltreDomaineTous As String = "Tous les domaines"

    ' Situations du résumé cliquable (une seule à la fois, axe indépendant des filtres ci-dessus).
    Private Const SituationToutes As Integer = 0
    Private Const SituationAujourdhui As Integer = 1
    Private Const SituationProchainsJours As Integer = 2
    Private Const SituationSansRendezVous As Integer = 3
    Private Const SituationARegler As Integer = 4
    Private Const SituationAvoir As Integer = 5
    Private Const SituationAbsences As Integer = 6
    Private Const SituationAlertes As Integer = 7

    ' Horizon de la situation « sous 7 jours » (aujourd'hui compris).
    Private Const NB_JOURS_PROCHAINS As Integer = 7

    ' Injecté par Home via SetContext() (implémentation IContextAwareUserControl)
    Private _context As UserControlContext

    ' Liste complète des patients chargée depuis la base de données. Utilisée pour le filtrage en mémoire.
    Private _patients As List(Of PatientListeItem)

    ' Filtre de recherche à restaurer au retour depuis la fiche patient (mini-pile D-Q15).
    ' Renseigné par RestaurerFiltre() avant le Load ; appliqué aux contrôles avant le chargement.
    Private _filtreInitialTexte As String = Nothing
    Private _filtreInitialSuivi As Integer = FiltreTous
    Private _filtreInitialSituation As Integer = SituationToutes
    Private _filtreInitialPatients As Integer = FiltrePatientsActifs

    ' Situation choisie dans le résumé cliquable, et le résumé lui-même.
    Private _filtreSituation As Integer = SituationToutes
    Private _resume As ResumeFiltre

    ' Page à réafficher au retour depuis la fiche patient (consommée au premier filtrage).
    Private _pageInitiale As Integer = 1

    ' Hauteur des lignes à pastilles et marge titrée entre deux groupes de la liste (mêmes valeurs
    ' que les grilles du dossier).
    Private Const HAUTEUR_LIGNE_PASTILLES As Integer = 30
    Private Const HAUTEUR_SEPARATEUR_GROUPE As Integer = 24

    ' Longueur au-delà de laquelle le texte de l'alerte est coupé dans l'infobulle (la fiche le montre en entier).
    Private Const LONGUEUR_MAX_ALERTE_INFOBULLE As Integer = 400

    ' Titre de séparateur par ligne de la page affichée (index de la ligne au-dessus de la marge).
    Private ReadOnly _titresSeparateurs As New Dictionary(Of Integer, String)

    ' Délai entre la sélection d'une ligne et le chargement de l'aperçu : parcourir la liste au
    ' clavier ne relance pas une lecture à chaque ligne survolée.
    Private Const DELAI_APERCU_MS As Integer = 200
    Private WithEvents _minuterieApercu As New System.Windows.Forms.Timer With {.Interval = DELAI_APERCU_MS}

    ' Patient montré dans l'aperçu (AUCUN_PATIENT_APERCU = invite « Cliquez sur un patient » ;
    ' Nothing = à relire) : la grille relève parfois SelectionChanged sans que la ligne change ; sans
    ' ce repère, l'aperçu se rechargeait en boucle.
    Private _idPatientApercu As Long?
    Private Const AUCUN_PATIENT_APERCU As Long = -1

    ' Patient que l'aperçu devrait montrer pour la sélection courante.
    Private Function IdPatientAttendu() As Long
        Return If(PatientPourApercu()?.IdPatient, AUCUN_PATIENT_APERCU)
    End Function

    ' Taille des icônes d'état de la liste « Dossier » (dessinées à cette taille : pas d'agrandissement flou).
    Private Const TAILLE_ICONE_FILTRE As Integer = 18

    ' Rendus partagés de la grille : « Sans dossier » en italique, icône d'alerte du kit.
    Private Shared ReadOnly PoliceSansDossier As New Font("Calibri", 10.0F, FontStyle.Italic)
    Private Shared ReadOnly IconeAlerte As Image = AltheaIconeEtat.CreerImage(EtatIcone.Avertissement, 18)

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SetContext
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
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

    End Sub

#End Region

#Region "Chargement UserControl"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : UC_PatientHome_Load
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Initialise le UserControl et charge la liste des patients.
    ' - Configure les éléments UI (boutons, DataGridView, ToolTips).
    ' - Charge les patients depuis la base de données via GestionPatients.
    '
    ' Remarques :
    ' - Aucune logique métier ni accès direct à la base de données (tout passe par les couches inférieures).
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_PatientHome_Load(
        sender As Object,
        e As EventArgs
    ) Handles MyBase.Load

        If _context IsNot Nothing Then
            _context.SetHeader("Patients")
            _context.SetStatus("Chargement de la liste des patients...")
        End If

        InitialiserBoutons()
        InitialiserDataGridView()
        InitialiserFiltres()
        InitialiserResume()
        InitialiserApercu()
        InitialiserToolTips()
        AppliquerFiltreInitial()
        ChargerPatients()

    End Sub

#End Region

#Region "Initialisation UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserBoutons
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Initialise les boutons standards du UserControl.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserBoutons()

        UtilsButtons.InitStandardButton(btnNouveau)
        UtilsButtons.InitStandardButton(btnModifier)
        UtilsButtons.InitStandardButton(btnOuvrir)
        UtilsButtons.InitStandardButton(btnActualiser)
        UtilsButtons.InitStandardButton(btnRechercher)
        UtilsButtons.InitStandardButton(btnReinitialiserFiltres)

        ' Le bouton Réinitialiser est désactivé par défaut (aucun filtre au chargement)
        btnReinitialiserFiltres.Enabled = False

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserDataGridView
    ' Version   : V2.1.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Configure le DataGridView des patients : propriétés de base, liaison des colonnes aux propriétés
    ' de PatientListeItem, polices, hauteur des lignes à pastilles, tri et pagination.
    '
    ' Remarques :
    ' - V2.0.0 (grille « hub de la journée », maquette « Patients — proposition » du 17/10/2026) :
    '   colonnes Âge, Suivi, Prochain RDV, Dernière séance ; Naissance, NISS, Email, Photo, Actif et
    '   Dossiers retirés de l'affichage (NISS et email restent dans la recherche).
    ' - Les colonnes à pastille sont liées à une valeur triable (date, montant, compteur) : le texte
    '   et la pastille sont produits à l'affichage (CellFormatting, CellPainting).
    ' - V2.1.0 : polices des autres grilles rétablies (Calibri 10, en-têtes 10 gras sur une ligne).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserDataGridView()

        UtilsDataGrid.InitDataGridBasique(dgvPatients)

        dgvPatients.AutoGenerateColumns = False

        dgvPatients.ReadOnly = True
        dgvPatients.MultiSelect = False
        dgvPatients.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvPatients.AllowUserToAddRows = False
        dgvPatients.AllowUserToDeleteRows = False
        dgvPatients.RowTemplate.Height = HAUTEUR_LIGNE_PASTILLES

        colCodePatient.DataPropertyName = NameOf(PatientListeItem.CodePatient)
        colNom.DataPropertyName = NameOf(PatientListeItem.Nom)
        colPrenom.DataPropertyName = NameOf(PatientListeItem.Prenom)
        colAge.DataPropertyName = NameOf(PatientListeItem.Age)
        colSuivi.DataPropertyName = NameOf(PatientListeItem.SuiviAffichage)
        colProchainRdv.DataPropertyName = NameOf(PatientListeItem.RendezVousTri)
        colDerniereSeance.DataPropertyName = NameOf(PatientListeItem.DerniereSeanceTri)
        colStatutPaiement.DataPropertyName = NameOf(PatientListeItem.MontantDu)
        colNbNoShow.DataPropertyName = NameOf(PatientListeItem.NbNoShow)
        colTelephone.DataPropertyName = NameOf(PatientListeItem.Telephone)
        colIdPatient.DataPropertyName = NameOf(PatientListeItem.IdPatient)

        ' Colonne Alerte : non liée (icône posée dans CellFormatting) ; NullValue = Nothing évite le
        ' glyphe « image manquante » sur les patients sans alerte.
        colAlerte.DefaultCellStyle.NullValue = Nothing
        colAlerte.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        colAge.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        colNbNoShow.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        colDerniereSeance.DefaultCellStyle.ForeColor = UITheme.ColorGrisDiscret

        ' Polices standard des grilles (InitDataGridBasique : Calibri 10, en-têtes 10 gras) : les
        ' polices réduites et les en-têtes sur deux lignes ne s'imposent plus depuis que la grille
        ' compte moins de colonnes (V2.1.0, demande de Joëlle du 17/10/2026).

        ' Nom et prénom en gras : ce sont les deux clés de lecture d'une ligne patient.
        Dim policeIdentite As New Font("Calibri", 10.0F, FontStyle.Bold)
        colNom.DefaultCellStyle.Font = policeIdentite
        colPrenom.DefaultCellStyle.Font = policeIdentite

        ' Tri par clic sur l'en-tête de colonne (ascendant/descendant), colonnes non liées exclues.
        UtilsDataGrid.ActiverTriColonnes(dgvPatients)

        ' Pagination : la liste filtrée complète reste en mémoire, seule la page courante est affichée.
        ' Le tri par en-tête porte sur toute la liste filtrée.
        pagPatients.Attacher(dgvPatients, "patient(s)")
        ' Une page peut contenir deux repères de groupe (« À venir », « Sans rendez-vous à venir »).
        pagPatients.HauteurReservee = 2 * HAUTEUR_SEPARATEUR_GROUPE

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserFiltres
    ' Version   : V1.1.0
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Initialise les filtres de la liste patients, avec les libellés des statuts de dossier paramétrés.
    '
    ' Remarques :
    ' - L'ordre des items doit correspondre aux constantes FiltreSuivi* (index).
    ' - L'item par défaut est « Tous ».
    ' - V1.1.0 (24/10/2026) : liste « Patients » (actifs, inactifs, tous).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserFiltres()

        Dim statuts As List(Of StatutDossier) = Nothing

        Try
            statuts = GestionStatutsDossier.GetStatutsDossier(True)
        Catch ex As Exception
            GestionLog.EcrireLog(
                "Impossible de charger les libellés des statuts de dossier pour les filtres patients : repli sur les codes.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )
        End Try

        cboFiltreSuivi.Items.Clear()
        cboFiltreSuivi.Items.Add("Tous")
        cboFiltreSuivi.Items.Add(LibelleStatutFiltre(statuts, CodesSysteme.StatutsDossier.Actif))
        cboFiltreSuivi.Items.Add(LibelleStatutFiltre(statuts, CodesSysteme.StatutsDossier.Pause))
        cboFiltreSuivi.Items.Add(LibelleStatutFiltre(statuts, CodesSysteme.StatutsDossier.Cloture))
        cboFiltreSuivi.Items.Add(LibelleStatutFiltre(statuts, CodesSysteme.StatutsDossier.Archive))
        cboFiltreSuivi.Items.Add("Sans dossier")

        ' Libellés sans « Dossier : » : l'étiquette de la liste le dit déjà (V1.0.3).
        ' Dessin personnalisé : icône d'état à gauche de chaque libellé.
        cboFiltreSuivi.DrawMode = DrawMode.OwnerDrawFixed
        cboFiltreSuivi.ItemHeight = 24

        cboFiltreSuivi.SelectedIndex = FiltreTous

        cboFiltrePatients.Items.Clear()
        cboFiltrePatients.Items.Add("Actifs")
        cboFiltrePatients.Items.Add("Inactifs")
        cboFiltrePatients.Items.Add("Tous")
        cboFiltrePatients.SelectedIndex = FiltrePatientsActifs

        InitialiserFiltreDomaine()

    End Sub

    ' Retourne le libellé paramétré du statut, ou son code si la ligne manque dans le référentiel.
    Private Function LibelleStatutFiltre(statuts As List(Of StatutDossier), code As String) As String

        Dim statut As StatutDossier = If(statuts, New List(Of StatutDossier)).
            FirstOrDefault(Function(s) String.Equals(s.CodeStatutDossier, code, StringComparison.OrdinalIgnoreCase))

        If statut IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(statut.LibelleStatutDossier) Then
            Return statut.LibelleStatutDossier
        End If

        GestionLog.EcrireLog(
            $"Statut de dossier {code} sans libellé dans le référentiel : repli sur le code pour le filtre patients.",
            GestionLog.LogLevel.Succinct,
            GestionLog.LogCategory.UI
        )

        Return code

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserFiltreDomaine
    ' Version   : V1.0.0
    ' Date      : 20/09/2026
    '
    ' Rôle      :
    ' Alimente la liste déroulante de filtrage par domaine à partir du référentiel ref_domaines.
    '
    ' Remarques :
    ' - Les domaines ne sont jamais codés en dur : la liste est construite depuis le référentiel, si
    '   bien qu'un nouveau domaine créé en administration devient filtrable sans recompilation.
    ' - L'item « Tous les domaines » porte une chaîne vide en Tag pour représenter l'absence de filtre.
    ' - En cas d'échec de chargement, le filtre reste utilisable (réduit à « Tous ») plutôt que de
    '   bloquer l'écran patients.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserFiltreDomaine()

        cboFiltreDomaine.Items.Clear()
        cboFiltreDomaine.Items.Add(FiltreDomaineTous)

        Try

            For Each domaine As Domaine In GestionDomaines.GetDomainesActifs()
                cboFiltreDomaine.Items.Add(domaine.CodeDomaine)
            Next

        Catch ex As Exception

            If _context IsNot Nothing Then
                _context.SetStatus("Filtre par domaine indisponible : référentiel non chargé.")
            End If

        End Try

        cboFiltreDomaine.SelectedIndex = 0

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : IconeFiltreSuivi
    ' Version  : V1.0.3
    ' Date     : 17/10/2026
    '
    ' Rôle     :
    ' Retourne l'icône d'état associée à un index du filtre cboFiltreSuivi, pour établir une liaison
    ' visuelle avec les icônes affichées dans la grille (statut de dossier, activation patient).
    '
    ' Paramètres :
    ' - index : Index de l'item du filtre (constantes Filtre*)
    '
    ' Retour :
    ' - Image : L'icône correspondante, ou Nothing pour « Tous » (pas d'icône).
    ' -------------------------------------------------------------------------------------------------
    Private Function IconeFiltreSuivi(index As Integer) As Image

        Select Case index
            Case FiltreDossierActif
                Return UtilsIcons.IconStatutDossierDominant(CodesSysteme.StatutsDossier.Actif, TAILLE_ICONE_FILTRE)
            Case FiltreDossierPause
                Return UtilsIcons.IconStatutDossierDominant(CodesSysteme.StatutsDossier.Pause, TAILLE_ICONE_FILTRE)
            Case FiltreDossierCloture
                Return UtilsIcons.IconStatutDossierDominant(CodesSysteme.StatutsDossier.Cloture, TAILLE_ICONE_FILTRE)
            Case FiltreDossierArchive
                Return UtilsIcons.IconStatutDossierDominant(CodesSysteme.StatutsDossier.Archive, TAILLE_ICONE_FILTRE)
            Case FiltreSansDossier
                Return UtilsIcons.IconPatientSansDossier(TAILLE_ICONE_FILTRE)
            Case Else
                Return Nothing
        End Select

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : cboFiltreSuivi_DrawItem
    ' Version   : V1.1.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Dessine chaque item du filtre de suivi avec son icône d'état à gauche du libellé (owner-draw).
    '
    ' Remarques :
    ' - L'icône (20x20) est dessinée en 18x18 pour tenir dans la hauteur d'item.
    ' - « Tous » n'a pas d'icône : le texte est simplement décalé pour rester aligné.
    ' - V1.1.0 : texte sur une ligne (TextRenderer, « … » si besoin) — il se coupait en deux dans le
    '   champ fermé, étroit après l'étiquette « Dossier » ; couleurs du kit dans la liste ouverte.
    ' -------------------------------------------------------------------------------------------------
    Private Sub cboFiltreSuivi_DrawItem(sender As Object, e As DrawItemEventArgs) Handles cboFiltreSuivi.DrawItem

        If e.Index < 0 Then Return

        ' Champ fermé : fond fourni par la liste ; liste ouverte : couleurs du kit (comme AltheaListe).
        Dim champFerme As Boolean = (e.State And DrawItemState.ComboBoxEdit) = DrawItemState.ComboBoxEdit
        Dim pointe As Boolean = Not champFerme AndAlso (e.State And DrawItemState.Selected) = DrawItemState.Selected

        Using pinceau As New SolidBrush(If(champFerme, e.BackColor, If(pointe, UITheme.ColorGrisVertClair, Color.White)))
            e.Graphics.FillRectangle(pinceau, e.Bounds)
        End Using

        Const marge As Integer = 4
        Const tailleIcone As Integer = TAILLE_ICONE_FILTRE

        Dim icone As Image = IconeFiltreSuivi(e.Index)
        If icone IsNot Nothing Then
            Dim yIcone As Integer = e.Bounds.Top + Math.Max(0, (e.Bounds.Height - tailleIcone) \ 2)
            e.Graphics.DrawImage(icone, e.Bounds.Left + marge, yIcone, tailleIcone, tailleIcone)
        End If

        ' Une seule ligne, coupée par « … » si la place manque (le champ fermé est étroit après
        ' l'étiquette) : DrawString revenait à la ligne au milieu d'un mot.
        Dim xTexte As Integer = e.Bounds.Left + marge + tailleIcone + marge
        Dim zoneTexte As New Rectangle(xTexte, e.Bounds.Top, Math.Max(0, e.Bounds.Right - xTexte - 2), e.Bounds.Height)
        Dim couleur As Color = If(champFerme, e.ForeColor, If(pointe, UITheme.ColorSaugeTresFonce, UITheme.ColorGrisFonce))

        TextRenderer.DrawText(e.Graphics, cboFiltreSuivi.Items(e.Index).ToString(), cboFiltreSuivi.Font, zoneTexte, couleur,
                              TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.SingleLine Or
                              TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPadding)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserToolTips
    ' Version   : V1.0.1
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Initialise les aides contextuelles du UserControl.
    '
    ' Remarques :
    ' - Utilise le ToolTip du contexte pour fournir des descriptions d'actions sur les boutons.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserToolTips()

        If _context Is Nothing Then Exit Sub

        _context.SetToolTip(btnNouveau, "Créer un nouveau patient.")
        _context.SetToolTip(btnModifier, "Modifier le patient sélectionné.")
        _context.SetToolTip(btnOuvrir, "Ouvrir la fiche du patient sélectionné.")
        _context.SetToolTip(btnActualiser, "Recharger la liste des patients.")
        _context.SetToolTip(btnRechercher, "Appliquer les critères de recherche.")
        _context.SetToolTip(btnReinitialiserFiltres, "Réinitialiser tous les filtres de recherche.")
        _context.SetToolTip(cboFiltreSuivi, "Filtrer les patients selon le statut de leur dossier courant (actif, en pause, clôturé, archivé, sans dossier).")
        _context.SetToolTip(cboFiltrePatients, "Patients actifs (par défaut), inactifs seulement, ou tous. Un patient inactif est marqué « Inactif » devant son nom.")
        _context.SetToolTip(btnApercu, "Afficher ou masquer l'aperçu du patient sélectionné.")
        _context.SetToolTip(cboFiltreDomaine, "Filtrer les patients selon le domaine de leur dossier courant.")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserResume
    ' Version   : V1.0.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Crée le résumé cliquable au-dessus de la grille (étape 3 du hub de la journée) : ses pastilles
    ' choisissent une situation (_filtreSituation) ; un nouveau clic revient à tous les patients.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserResume()

        _resume = New ResumeFiltre(pnlResumePatients,
                                   Function() _filtreSituation,
                                   AddressOf ChoisirSituation,
                                   SituationToutes, "tous les patients", "ces patients",
                                   AddressOf DefinirInfobulle)

    End Sub

    ' Infobulle des pastilles du résumé (ResumeFiltre ne connaît pas le contexte de l'écran).
    Private Sub DefinirInfobulle(controle As Control, texte As String)
        _context?.SetToolTip(controle, texte)
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserApercu
    ' Version   : V1.0.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Prépare le panneau d'aperçu (étape 5) : ouvert par défaut (décision de Joëlle du 17/10/2026,
    ' à revoir à l'usage), minuterie libérée avec l'écran.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserApercu()

        AppliquerEtatApercu(True)
        AddHandler Disposed, Sub() _minuterieApercu.Dispose()

    End Sub

    ' Colonnes reprises par le panneau d'aperçu, masquées dans la grille quand il est ouvert.
    Private ReadOnly Property ColonnesRepriseParApercu As DataGridViewColumn()
        Get
            Return {colAge, colDerniereSeance, colNbNoShow, colTelephone}
        End Get
    End Property

    ' Ouvre ou ferme le panneau d'aperçu et ajuste les colonnes de la grille.
    Private Sub AppliquerEtatApercu(ouvert As Boolean)

        _idPatientApercu = Nothing
        ucApercu.Visible = ouvert
        btnApercu.Text = If(ouvert, "Masquer l'aperçu", "Afficher l'aperçu")
        For Each colonne As DataGridViewColumn In ColonnesRepriseParApercu
            colonne.Visible = Not ouvert
        Next

    End Sub

    ' Patient dont la ligne est sélectionnée (et non la seule ligne courante, qui reste posée sur la
    ' première ligne après un rechargement).
    Private Function PatientPourApercu() As PatientListeItem
        If dgvPatients.SelectedRows.Count = 0 Then Return Nothing
        Return TryCast(dgvPatients.SelectedRows(0).DataBoundItem, PatientListeItem)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerApercu
    ' Version   : V1.1.0
    ' Date      : 19/10/2026
    '
    ' Rôle      :
    ' Charge et affiche l'aperçu du patient sélectionné (GestionApercuPatient) ; sans sélection,
    ' l'invite « Cliquez sur un patient ».
    '
    ' Remarques :
    ' - Une erreur de lecture est journalisée et dite dans le panneau, sans fenêtre : l'écran reste
    '   utilisable.
    ' - V1.1.0 : rien n'est relu si le patient montré est déjà celui de la sélection (_idPatientApercu,
    '   remis à Nothing quand la liste est rechargée ou le panneau rouvert).
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerApercu()

        If Not ucApercu.Visible Then Return

        Dim attendu As Long = IdPatientAttendu()
        If _idPatientApercu.HasValue AndAlso _idPatientApercu.Value = attendu Then Return
        _idPatientApercu = attendu

        Dim patient As PatientListeItem = PatientPourApercu()
        If patient Is Nothing Then
            ucApercu.Vider()
            Return
        End If

        Try
            ucApercu.AfficherApercu(GestionApercuPatient.GetApercuPatient(patient.IdPatient))
        Catch ex As Exception
            GestionLog.EcrireLog(
                $"Erreur chargement de l'aperçu (UC_PatientHome, idPatient={patient.IdPatient}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )
            ucApercu.Vider("L'aperçu n'a pas pu être chargé.")
        End Try

    End Sub

    Private Sub dgvPatients_SelectionChanged(sender As Object, e As EventArgs) Handles dgvPatients.SelectionChanged
        ' Même patient déjà montré : signal sans changement de ligne, rien à relancer.
        If _idPatientApercu.HasValue AndAlso _idPatientApercu.Value = IdPatientAttendu() Then Return
        _minuterieApercu.Stop()
        _minuterieApercu.Start()
    End Sub

    Private Sub _minuterieApercu_Tick(sender As Object, e As EventArgs) Handles _minuterieApercu.Tick
        _minuterieApercu.Stop()
        ChargerApercu()
    End Sub

    Private Sub btnApercu_Click(sender As Object, e As EventArgs) Handles btnApercu.Click
        AppliquerEtatApercu(Not ucApercu.Visible)
        ChargerApercu()
    End Sub

    Private Sub ucApercu_OuvrirFicheDemandee(sender As Object, e As EventArgs) Handles ucApercu.OuvrirFicheDemandee
        OuvrirFichePatientSelectionne("Consultation")
    End Sub

    Private Sub ucApercu_OuvrirDossierDemande(idDossier As Long, codeDossier As String) Handles ucApercu.OuvrirDossierDemande
        OuvrirDossier(idDossier, codeDossier)
    End Sub

    Private Sub ucApercu_OuvrirSeanceDemandee(seance As RepereSeance) Handles ucApercu.OuvrirSeanceDemandee
        OuvrirSeance(seance)
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : OuvrirDossier
    ' Version   : V1.0.1
    ' Date      : 25/10/2026
    '
    ' Rôle      :
    ' Ouvre le hub d'un dossier du patient sélectionné, en empilant l'écran Patients (retour avec
    ' ses filtres, D-Q15) — comme depuis l'onglet Dossiers de la fiche patient.
    ' - V1.0.1 (25/10/2026, audit C.4, ADR-55) : fenêtre principale lue dans le contexte
    '   (IFenetrePrincipale) au lieu de TryCast(FindForm(), Home).
    ' -------------------------------------------------------------------------------------------------
    Private Sub OuvrirDossier(idDossier As Long, codeDossier As String)

        Dim patient As PatientListeItem = GetPatientSelectionne()
        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale
        If patient Is Nothing OrElse fenetre Is Nothing Then Return

        Dim hub As New UC_DossierFiche()
        hub.InitialiserPourDossier(idDossier)

        fenetre.NavigateToDossierFiche(hub, $"Patients > {patient.NomComplet} > {codeDossier}", CreerEcranCourant())

    End Sub

    ' Resélectionne un patient dans la page affichée (après un rechargement), ce qui relance l'aperçu.
    Private Sub SelectionnerPatient(idPatient As Long)

        For Each ligne As DataGridViewRow In dgvPatients.Rows
            Dim item As PatientListeItem = TryCast(ligne.DataBoundItem, PatientListeItem)
            If item IsNot Nothing AndAlso item.IdPatient = idPatient Then
                dgvPatients.CurrentCell = ligne.Cells(colCodePatient.Index)
                ligne.Selected = True
                Return
            End If
        Next

    End Sub

#End Region

#Region "Chargement des données"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerPatients
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Charge la liste complète des patients depuis la base de données via GestionPatients,
    ' puis applique les filtres actifs pour alimenter le DataGridView.
    '
    ' Remarques :
    ' - La liste complète est conservée dans _patients pour le filtrage en mémoire.
    ' - Les erreurs sont journalisées via GestionLog et signalées dans le statut.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerPatients()

        Try
            Cursor = Cursors.WaitCursor

            _patients = GestionPatients.GetPatients()
            ' Données relues : l'aperçu doit l'être aussi, même si la sélection n'a pas changé.
            _idPatientApercu = Nothing

            AppliquerFiltres()

            If _context IsNot Nothing Then
                _context.SetStatus($"{_patients.Count} patient(s) chargé(s).")
            End If

        Catch ex As Exception

            GestionLog.EcrireLog(
                "Erreur chargement des patients (UC_PatientHome).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors du chargement des patients.")
            End If

        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerFiltres
    ' Version   : V1.2.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Filtre la liste _patients en mémoire selon :
    ' - Le texte de recherche (nom, prénom, NISS, code, téléphone, email)
    ' - Le statut de suivi sélectionné (en cours / clôturé / tous)
    '
    ' Remarques :
    ' - Lie le résultat au DataGridView.
    ' - Met à jour l'état du bouton de réinitialisation des filtres.
    ' - V1.1.0 : ordre par défaut « agenda » (décision du 17/10/2026) — patients attendus aujourd'hui
    '   (par heure), puis rendez-vous à venir (par date), puis sans rendez-vous (par nom) ; un tri par
    '   en-tête antérieur est oublié à chaque nouveau filtrage.
    ' - V1.2.0 : patients inactifs masqués sauf « Afficher les inactifs » ; résumé cliquable recalculé,
    '   puis situation choisie appliquée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerFiltres()

        If _patients Is Nothing Then
            Return
        End If

        Try
            Cursor = Cursors.WaitCursor

            Dim patientsFiltres = _patients.AsEnumerable()

            ' Filtre : texte de recherche multi-champs
            Dim texteRecherche As String = txtRecherchePatient.Text.Trim()
            If Not String.IsNullOrEmpty(texteRecherche) Then
                patientsFiltres = patientsFiltres.Where(
                    Function(p) CorrespondRecherche(p, texteRecherche)
                )
            End If

            ' Filtre : statut du dossier dominant
            Select Case cboFiltreSuivi.SelectedIndex
                Case FiltreDossierActif
                    patientsFiltres = patientsFiltres.Where(Function(p) EstStatutDossier(p, CodesSysteme.StatutsDossier.Actif))
                Case FiltreDossierPause
                    patientsFiltres = patientsFiltres.Where(Function(p) EstStatutDossier(p, CodesSysteme.StatutsDossier.Pause))
                Case FiltreDossierCloture
                    patientsFiltres = patientsFiltres.Where(Function(p) EstStatutDossier(p, CodesSysteme.StatutsDossier.Cloture))
                Case FiltreDossierArchive
                    patientsFiltres = patientsFiltres.Where(Function(p) EstStatutDossier(p, CodesSysteme.StatutsDossier.Archive))
                Case FiltreSansDossier
                    patientsFiltres = patientsFiltres.Where(Function(p) p.SansDossier)
                Case Else
                    ' FiltreTous : aucun filtre
            End Select

            ' Filtre : domaine du dossier courant (axe indépendant du statut de suivi)
            Dim domaineFiltre As String = DomaineFiltreSelectionne()
            If Not String.IsNullOrEmpty(domaineFiltre) Then
                patientsFiltres = patientsFiltres.Where(
                    Function(p) String.Equals(If(p.CodeDomaineDominant, "").Trim(), domaineFiltre, StringComparison.OrdinalIgnoreCase)
                )
            End If

            ' Patients : actifs par défaut (décision du 17/10/2026), inactifs seulement ou tous
            ' (24/10/2026). Avec les actifs, on compte les inactifs qui correspondraient pour le
            ' signaler, sinon une recherche « vide » serait trompeuse.
            Dim nbInactifsMasques As Integer = 0
            Select Case cboFiltrePatients.SelectedIndex
                Case FiltrePatientsInactifs
                    patientsFiltres = patientsFiltres.Where(Function(p) Not p.IsActif)
                Case FiltrePatientsTous
                    ' aucun filtre
                Case Else
                    Dim tous As List(Of PatientListeItem) = patientsFiltres.ToList()
                    nbInactifsMasques = tous.Where(Function(p) Not p.IsActif).Count()
                    patientsFiltres = tous.Where(Function(p) p.IsActif)
            End Select

            ' Le résumé compte ce que donnerait chaque situation sur la liste déjà filtrée, puis la
            ' situation choisie restreint l'affichage.
            Dim avantSituation As List(Of PatientListeItem) = patientsFiltres.ToList()
            MettreAJourResume(avantSituation)

            Dim resultats = avantSituation.
                Where(Function(p) CorrespondSituation(p, _filtreSituation)).
                OrderBy(Function(p) p.Groupe).
                ThenBy(Function(p) p.OrdreAgenda).
                ThenBy(Function(p) p.Nom, StringComparer.CurrentCultureIgnoreCase).
                ThenBy(Function(p) p.Prenom, StringComparer.CurrentCultureIgnoreCase).
                ToList()

            UtilsDataGrid.ReinitialiserTri(dgvPatients)

            ' Nouveau jeu de résultats : retour en page 1, sauf page à restaurer au retour de fiche.
            pagPatients.DefinirSource(resultats, _pageInitiale)
            _pageInitiale = 1

            ' Le bouton de réinitialisation n'est actif que si un filtre s'écarte de l'état par défaut
            btnReinitialiserFiltres.Enabled =
                Not String.IsNullOrEmpty(texteRecherche) OrElse
                cboFiltreSuivi.SelectedIndex <> FiltreTous OrElse
                Not String.IsNullOrEmpty(domaineFiltre) OrElse
                _filtreSituation <> SituationToutes OrElse
                cboFiltrePatients.SelectedIndex <> FiltrePatientsActifs

            If _context IsNot Nothing Then
                Dim mention As String = If(nbInactifsMasques > 0,
                                           $" — {nbInactifsMasques} patient(s) inactif(s) masqué(s) : choisir « Patients : Tous » ou « Inactifs ».",
                                           String.Empty)
                _context.SetStatus($"{resultats.Count} patient(s) affiché(s).{mention}")
            End If

        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CorrespondRecherche
    ' Version  : V1.1.0
    ' Date     : 23/10/2026
    '
    ' Rôle     :
    ' Indique si un patient correspond au texte de recherche (comparaison insensible à la casse).
    '
    ' Paramètres :
    ' - patient : Le patient à tester
    ' - texte   : Le texte recherché (déjà nettoyé)
    '
    ' Retour   :
    ' - Boolean : True si l'un des champs (nom, prénom, NISS, code, téléphone, email, domaine) contient le texte
    '
    ' Remarques :
    ' - Les champs optionnels (NISS, téléphone, email) peuvent être Nothing : protégés par If(..., "").
    ' - Le domaine courant est inclus afin que saisir « PSY » ramène directement la patientèle concernée.
    ' - V1.1.0 (23/10/2026) : NISS comparé sous sa forme normalisée (séparateurs ignorés).
    ' -------------------------------------------------------------------------------------------------
    Private Function CorrespondRecherche(patient As PatientListeItem, texte As String) As Boolean

        Dim nissCherche As String = UtilsNiss.Normaliser(texte)

        Return If(patient.Nom, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(patient.Prenom, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               (nissCherche IsNot Nothing AndAlso If(patient.Niss, "").Contains(nissCherche, StringComparison.OrdinalIgnoreCase)) OrElse
               If(patient.CodePatient, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(patient.Telephone, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(patient.Email, "").Contains(texte, StringComparison.OrdinalIgnoreCase) OrElse
               If(patient.CodeDomaineDominant, "").Contains(texte, StringComparison.OrdinalIgnoreCase)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreAJourResume
    ' Version   : V1.0.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Reconstruit le résumé cliquable sur la liste déjà filtrée (recherche, dossier, domaine,
    ' inactifs) : chaque pastille dit combien de patients la situation ramènerait.
    '
    ' Remarques :
    ' - Situations du jour d'abord (aujourd'hui, sous 7 jours, suivis sans rendez-vous), puis les
    '   points d'attention (à régler, avoir, absences, alertes) ; une pastille à zéro n'apparaît pas,
    '   sauf celle de la situation choisie.
    ' - Couleurs des pastilles de la grille (UtilsPastilles), pour que l'œil fasse le lien.
    ' -------------------------------------------------------------------------------------------------
    Private Sub MettreAJourResume(patients As List(Of PatientListeItem))

        If _resume Is Nothing Then Return

        _resume.Reconstruire(
            Sub()
                Dim compter = Function(situation As Integer) patients.Where(Function(p) CorrespondSituation(p, situation)).Count()

                _resume.AjouterNombre(compter(SituationAujourdhui), "aujourd'hui", "aujourd'hui",
                                      UtilsPastilles.Teinte(FamilleRendezVousPatient.AujourdhuiAVenir), SituationAujourdhui)
                _resume.AjouterNombre(compter(SituationProchainsJours), $"sous {NB_JOURS_PROCHAINS} jours", $"sous {NB_JOURS_PROCHAINS} jours",
                                      UtilsPastilles.Teinte(FamilleRendezVousPatient.AVenir), SituationProchainsJours)
                _resume.AjouterNombre(compter(SituationSansRendezVous), "suivi sans rendez-vous", "suivis sans rendez-vous",
                                      UtilsPastilles.Teinte(FamilleRendezVousPatient.SansRendezVous), SituationSansRendezVous)

                _resume.AjouterSeparateur()

                _resume.AjouterMontant(patients.Where(Function(p) p.ADesImpayes).Sum(Function(p) p.MontantDu), "À régler",
                                       UtilsPastilles.Teinte(FamillePaiementSeance.ARegler), SituationARegler)
                _resume.AjouterMontant(patients.Sum(Function(p) p.MontantAvoir), "Avoir",
                                       UtilsPastilles.Teinte(FamillePaiementSeance.Avoir), SituationAvoir)
                _resume.AjouterNombre(compter(SituationAbsences), "avec absences", "avec absences",
                                      UtilsPastilles.Teinte(FamilleStatutSeance.AbsenceNonExcusee), SituationAbsences)
                _resume.AjouterNombre(compter(SituationAlertes), "alerte", "alertes",
                                      New UtilsPastilles.TeintePastille(AltheaIconeEtat.Teintes(EtatIcone.Avertissement).Pale,
                                                                       AltheaIconeEtat.Teintes(EtatIcone.Avertissement).Foncee),
                                      SituationAlertes)
            End Sub)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CorrespondSituation
    ' Version  : V1.0.0
    ' Date     : 17/10/2026
    '
    ' Rôle     :
    ' Indique si un patient relève d'une situation du résumé cliquable.
    '
    ' Remarques :
    ' - « Sous 7 jours » : séance aujourd'hui ou prochain rendez-vous avant J+7 (aujourd'hui compris).
    ' - « Avoir » : tout patient qui a un avoir, même s'il doit aussi de l'argent (pour le retrouver
    '   et l'utiliser), alors que la grille ne montre l'avoir en pastille que sans dette.
    ' -------------------------------------------------------------------------------------------------
    Private Function CorrespondSituation(patient As PatientListeItem, situation As Integer) As Boolean

        Select Case situation
            Case SituationAujourdhui
                Return patient.ASeanceAujourdhui
            Case SituationProchainsJours
                Return patient.ASeanceAujourdhui OrElse
                       (patient.ProchaineSeanceDate.HasValue AndAlso
                        patient.ProchaineSeanceDate.Value.Date < Date.Today.AddDays(NB_JOURS_PROCHAINS))
            Case SituationSansRendezVous
                Return patient.EstSuiviSansRendezVous
            Case SituationARegler
                Return patient.ADesImpayes
            Case SituationAvoir
                Return patient.MontantAvoir > 0D
            Case SituationAbsences
                Return patient.NbNoShow > 0
            Case SituationAlertes
                Return patient.AAlerte
            Case Else
                Return True
        End Select

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : DomaineFiltreSelectionne
    ' Version  : V1.0.0
    ' Date     : 20/09/2026
    '
    ' Rôle     :
    ' Retourne le code de domaine sélectionné dans le filtre, ou une chaîne vide si aucun n'est imposé.
    '
    ' Retour   :
    ' - String : Code du domaine (PSY, GRA…) ou String.Empty
    ' -------------------------------------------------------------------------------------------------
    Private Function DomaineFiltreSelectionne() As String

        Dim selection As String = TryCast(cboFiltreDomaine.SelectedItem, String)

        If String.IsNullOrWhiteSpace(selection) OrElse selection = FiltreDomaineTous Then
            Return String.Empty
        End If

        Return selection.Trim()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : EstStatutDossier
    ' Version  : V1.0.0
    ' Date     : 14/07/2026
    '
    ' Rôle     :
    ' Indique si le statut du dernier dossier (dominant) d'un patient correspond au code attendu.
    '
    ' Paramètres :
    ' - patient : Élément de liste à tester
    ' - code    : Code de statut attendu (ACTIF/PAUSE/CLOTURE/ARCHIVE)
    '
    ' Retour   :
    ' - Boolean : True si le patient a un dossier dont le statut dominant correspond, sinon False.
    ' -------------------------------------------------------------------------------------------------
    Private Function EstStatutDossier(patient As PatientListeItem, code As String) As Boolean

        If patient.SansDossier OrElse String.IsNullOrWhiteSpace(patient.CodeStatutDossierDominant) Then
            Return False
        End If

        Return String.Equals(patient.CodeStatutDossierDominant.Trim(), code, StringComparison.OrdinalIgnoreCase)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : GetPatientSelectionne
    ' Version  : V1.0.0
    ' Date     : 11/06/2026
    '
    ' Rôle     :
    ' Retourne le patient actuellement sélectionné dans le DataGridView.
    '
    ' Retour   :
    ' - PatientListeItem : patient sélectionné, ou Nothing si aucune sélection valide.
    '
    ' Remarques :
    ' - Ne déclenche aucune action métier.
    ' -------------------------------------------------------------------------------------------------
    Private Function GetPatientSelectionne() As PatientListeItem

        If dgvPatients.CurrentRow Is Nothing Then
            Return Nothing
        End If

        Return TryCast(
            dgvPatients.CurrentRow.DataBoundItem,
            PatientListeItem
        )

    End Function

#End Region

#Region "Événements UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnRechercher_Click
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Applique les critères de recherche/filtrage sur la liste en mémoire.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnRechercher_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnRechercher.Click

        AppliquerFiltres()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : txtRecherchePatient_KeyDown
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Permet de déclencher la recherche par la touche Entrée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub txtRecherchePatient_KeyDown(
        sender As Object,
        e As KeyEventArgs
    ) Handles txtRecherchePatient.KeyDown

        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            AppliquerFiltres()
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : cboFiltreSuivi_SelectedIndexChanged
    ' Version   : V1.0.0
    ' Date      : 19/07/2026
    '
    ' Rôle      :
    ' Réapplique les filtres lorsque le statut de suivi sélectionné change.
    ' -------------------------------------------------------------------------------------------------
    Private Sub cboFiltreSuivi_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles cboFiltreSuivi.SelectedIndexChanged

        AppliquerFiltres()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : cboFiltreDomaine_SelectedIndexChanged
    ' Version   : V1.0.0
    ' Date      : 20/09/2026
    '
    ' Rôle      :
    ' Réapplique les filtres lorsque le domaine sélectionné change.
    ' -------------------------------------------------------------------------------------------------
    Private Sub cboFiltreDomaine_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles cboFiltreDomaine.SelectedIndexChanged

        AppliquerFiltres()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : cboFiltrePatients_SelectedIndexChanged
    ' Version   : V1.0.0
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Patients actifs, inactifs ou tous (remplace la case « Afficher les inactifs », décision de
    ' Joëlle) ; réapplique les filtres.
    ' -------------------------------------------------------------------------------------------------
    Private Sub cboFiltrePatients_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboFiltrePatients.SelectedIndexChanged
        AppliquerFiltres()
    End Sub

    ' Choix d'une situation dans le résumé cliquable (ou retour à « tous »).
    Private Sub ChoisirSituation(situation As Integer)
        _filtreSituation = situation
        AppliquerFiltres()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnReinitialiserFiltres_Click
    ' Version   : V1.0.2
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Réinitialise tous les critères de recherche et réaffiche la liste complète.
    ' - V1.0.2 (24/10/2026) : filtre « Patients » remis sur « Actifs ».
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnReinitialiserFiltres_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnReinitialiserFiltres.Click

        txtRecherchePatient.Clear()
        _filtreSituation = SituationToutes
        cboFiltrePatients.SelectedIndex = FiltrePatientsActifs
        cboFiltreSuivi.SelectedIndex = FiltreTous

        If cboFiltreDomaine.Items.Count > 0 Then
            cboFiltreDomaine.SelectedIndex = 0
        End If

        AppliquerFiltres()

        txtRecherchePatient.Focus()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnActualiser_Click
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Recharge la liste des patients depuis la base de données.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnActualiser_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnActualiser.Click

        ChargerPatients()
        dgvPatients.ClearSelection()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnNouveau_Click
    ' Version   : V1.1.1
    ' Date      : 25/10/2026
    '
    ' Rôle      :
    ' Ouvre la fiche patient en mode création (UC_PatientFiche).
    '
    ' Remarques :
    ' - Empile l'écran courant (liste + filtre) dans la mini-pile de navigation pour le retour (D-Q15).
    ' - V1.1.1 (25/10/2026, audit C.4, ADR-55) : fenêtre principale lue dans le contexte
    '   (IFenetrePrincipale) au lieu de TryCast(FindForm(), Home).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnNouveau_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnNouveau.Click

        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale

        If fenetre Is Nothing Then
            Return
        End If

        Dim fiche As New UC_PatientFiche()
        fiche.InitialiserPourCreation()

        fenetre.NavigateToPatientFiche(
            fiche,
            "Patients > Nouveau patient",
            CreerEcranCourant()
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnModifier_Click
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Ouvrira la fiche patient sélectionnée en mode modification (UC_PatientFiche, à implémenter).
    '
    ' Remarques :
    ' - L'écran de fiche patient n'est pas encore disponible : statut informatif en attendant.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnModifier_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnModifier.Click

        OuvrirFichePatientSelectionne("Modification")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnOuvrir_Click
    ' Version   : V1.0.0
    ' Date      : 11/06/2026
    '
    ' Rôle      :
    ' Ouvrira la fiche patient sélectionnée en mode consultation (UC_PatientFiche, à implémenter).
    '
    ' Remarques :
    ' - L'écran de fiche patient n'est pas encore disponible : statut informatif en attendant.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnOuvrir_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnOuvrir.Click

        OuvrirFichePatientSelectionne("Consultation")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvPatients_CellFormatting
    ' Version   : V2.1.1
    ' Date      : 25/10/2026
    '
    ' Rôle      :
    ' Mise en forme des cellules ordinaires de la grille Patients :
    ' - ligne d'un patient attendu aujourd'hui : fond bleu très léger ;
    ' - patient inactif : texte grisé (la pastille « Inactif » devant le nom est dessinée par
    '   dgvPatients_CellPainting) ;
    ' - Âge : « 42 ans » ; Suivi « Sans dossier » : gris en italique ;
    ' - Prochain RDV, Dernière séance, Paiements, Abs. : texte (la pastille éventuelle est dessinée
    '   par dgvPatients_CellPainting) ;
    ' - Alerte : icône d'avertissement ; Téléphone : masque international lisible.
    '
    ' Remarques :
    ' - V2.0.0 : grille « hub de la journée » (maquette du 17/10/2026). Les infobulles sont données
    '   par dgvPatients_CellToolTipTextNeeded.
    ' - Le numéro est stocké au format E.164 : le pays est déduit de l'indicatif présent dans le numéro.
    ' - V2.1.0 (24/10/2026) : « (inactif) » après le prénom retiré (coupé par la largeur de la colonne) :
    '   la pastille « Inactif » le remplace.
    ' - V2.1.1 (25/10/2026, relevé aux captures) : texte d'un patient inactif sélectionné en
    '   ColorGrisDiscret (ColorGrisClair était illisible sur le fond de la sélection).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvPatients_CellFormatting(
        sender As Object,
        e As DataGridViewCellFormattingEventArgs
    ) Handles dgvPatients.CellFormatting

        If e.RowIndex < 0 Then
            Return
        End If

        Dim patient As PatientListeItem = TryCast(dgvPatients.Rows(e.RowIndex).DataBoundItem, PatientListeItem)
        If patient Is Nothing Then
            Return
        End If

        If patient.ASeanceAujourdhui Then
            e.CellStyle.BackColor = UITheme.LigneAujourdhuiBack
        End If

        If Not patient.IsActif Then
            e.CellStyle.ForeColor = UITheme.ColorGrisMoyenFonce
            e.CellStyle.SelectionForeColor = UITheme.ColorGrisDiscret
        End If

        Select Case e.ColumnIndex

            Case colAge.Index
                e.Value = If(patient.Age.HasValue, $"{patient.Age.Value} an{If(patient.Age.Value > 1, "s", "")}", String.Empty)
                e.FormattingApplied = True

            Case colSuivi.Index
                If patient.SansDossier Then
                    e.CellStyle.ForeColor = UITheme.ColorGrisMoyenFonce
                    e.CellStyle.Font = PoliceSansDossier
                End If

            Case colProchainRdv.Index
                e.Value = patient.RendezVousAffichage
                e.FormattingApplied = True

            Case colDerniereSeance.Index
                e.Value = patient.DerniereSeanceAffichage
                e.FormattingApplied = True

            Case colStatutPaiement.Index
                ' Cas sain en texte discret ; une dette ou un avoir sont dessinés en pastille.
                e.Value = If(patient.ADesImpayes OrElse patient.AUnAvoir, String.Empty, "À jour")
                e.CellStyle.ForeColor = UITheme.ColorSaugeFonce
                e.FormattingApplied = True

            Case colNbNoShow.Index
                e.Value = If(patient.NbNoShow > 0, patient.NbNoShow.ToString(), String.Empty)
                e.FormattingApplied = True

            Case colAlerte.Index
                e.Value = If(patient.AAlerte, IconeAlerte, Nothing)
                e.FormattingApplied = True

            Case colTelephone.Index
                Dim valeur As String = TryCast(e.Value, String)
                If Not String.IsNullOrWhiteSpace(valeur) Then
                    e.Value = UtilsTelephone.FormaterAffichage(valeur, Nothing)
                    e.FormattingApplied = True
                End If

        End Select

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvPatients_CellPainting
    ' Version   : V1.1.0
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Dessine les pastilles de la grille Patients (UtilsDataGrid.DessinerPastilleCellule, couleurs
    ' UtilsPastilles) : Suivi (« Actif · PSY », « +1 »), Prochain RDV (aujourd'hui en bleu plein,
    ' séance du jour passée atténuée, à venir bleu pâle, « Aucun RDV » au trait), Paiements
    ' (« À régler 60 € », « Avoir 30 € ») et absences. Pastilles atténuées pour un patient inactif,
    ' marqué d'une pastille « Inactif » devant son nom (V1.1.0, 24/10/2026).
    '
    ' Remarques :
    ' - Sans pastille (sans dossier, à jour, aucune absence…), la cellule garde le rendu ordinaire.
    ' - V1.1.0 (24/10/2026) : pastille « Inactif » devant le nom d'un patient inactif (relevé de Joëlle).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvPatients_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) _
        Handles dgvPatients.CellPainting

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return

        Dim patient As PatientListeItem = TryCast(dgvPatients.Rows(e.RowIndex).DataBoundItem, PatientListeItem)
        If patient Is Nothing Then Return

        Dim attenuee As Boolean = Not patient.IsActif

        Select Case e.ColumnIndex

            Case colNom.Index
                If Not patient.IsActif Then
                    UtilsDataGrid.DessinerPastilleAvantTexte(dgvPatients, e, "Inactif", UtilsPastilles.Teinte(TonEtat.Inactif))
                End If

            Case colSuivi.Index
                If patient.SansDossier Then Return
                UtilsDataGrid.DessinerPastilleCellule(dgvPatients, e, patient.SuiviAffichage,
                                                      UtilsPastilles.TeinteSuiviListe(patient.CodeStatutDossierDominant),
                                                      patient.SuiviMention, attenuee)

            Case colProchainRdv.Index
                Dim famille As FamilleRendezVousPatient = patient.FamilleRendezVous
                If famille = FamilleRendezVousPatient.Aucune Then Return
                UtilsDataGrid.DessinerPastilleCellule(dgvPatients, e, patient.RendezVousAffichage,
                                                      UtilsPastilles.Teinte(famille), Nothing,
                                                      attenuee OrElse famille = FamilleRendezVousPatient.AujourdhuiPassee,
                                                      famille = FamilleRendezVousPatient.SansRendezVous)

            Case colStatutPaiement.Index
                If patient.ADesImpayes Then
                    UtilsDataGrid.DessinerPastilleCellule(dgvPatients, e, $"À régler {SeanceListeItem.MontantCourt(patient.MontantDu)}",
                                                          UtilsPastilles.Teinte(FamillePaiementSeance.ARegler), Nothing, attenuee)
                ElseIf patient.AUnAvoir Then
                    UtilsDataGrid.DessinerPastilleCellule(dgvPatients, e, $"Avoir {SeanceListeItem.MontantCourt(patient.MontantAvoir)}",
                                                          UtilsPastilles.Teinte(FamillePaiementSeance.Avoir), Nothing, attenuee)
                End If

            Case colNbNoShow.Index
                If patient.NbNoShow > 0 Then
                    UtilsDataGrid.DessinerPastilleCellule(dgvPatients, e, patient.NbNoShow.ToString(),
                                                          UtilsPastilles.Teinte(FamilleStatutSeance.AbsenceNonExcusee), Nothing, attenuee)
                End If

        End Select

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvPatients_CellToolTipTextNeeded
    ' Version   : V1.1.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Infobulles de la grille Patients : naissance, dossiers, rendez-vous (dont le suivant quand la
    ' séance du jour est passée), dernière séance, paiements (tous dossiers), absences, alerte complète.
    ' V1.1.0 : « Double-clic : ouvrir la séance SE… » sur les colonnes de rendez-vous.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvPatients_CellToolTipTextNeeded(sender As Object, e As DataGridViewCellToolTipTextNeededEventArgs) _
        Handles dgvPatients.CellToolTipTextNeeded

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return

        Dim patient As PatientListeItem = TryCast(dgvPatients.Rows(e.RowIndex).DataBoundItem, PatientListeItem)
        If patient Is Nothing Then Return

        Select Case e.ColumnIndex

            Case colAge.Index
                If patient.DateNaissance.HasValue Then
                    e.ToolTipText = $"Né(e) le {patient.DateNaissance.Value:dd/MM/yyyy}"
                End If

            Case colSuivi.Index
                e.ToolTipText = If(patient.SansDossier,
                                   "Aucun dossier",
                                   $"{patient.NbDossiers} dossier(s) — dossier courant : {patient.SuiviAffichage}")

            Case colProchainRdv.Index
                If patient.ProchaineSeanceDate.HasValue Then
                    e.ToolTipText = $"Prochain rendez-vous : {DateHeureLongue(patient.ProchaineSeanceDate.Value, patient.ProchaineSeanceHeure)}"
                ElseIf patient.EstSuiviSansRendezVous Then
                    e.ToolTipText = "Suivi actif sans rendez-vous à venir"
                End If
                e.ToolTipText = AjouterAideDoubleClic(e.ToolTipText, patient.SeanceDuRendezVous)

            Case colDerniereSeance.Index
                If patient.DerniereSeanceDate.HasValue Then
                    e.ToolTipText = $"Dernière séance où le patient est venu : {DateHeureLongue(patient.DerniereSeanceDate.Value, patient.DerniereSeanceHeure)}"
                End If
                e.ToolTipText = AjouterAideDoubleClic(e.ToolTipText, patient.DerniereSeance)

            Case colStatutPaiement.Index
                If patient.ADesImpayes Then
                    e.ToolTipText = $"Séances réalisées non payées (tous dossiers) : {patient.MontantDu:N2} €" &
                                    If(patient.MontantAvoir > 0D, $"{Environment.NewLine}Avoir disponible : {patient.MontantAvoir:N2} €", String.Empty)
                ElseIf patient.AUnAvoir Then
                    e.ToolTipText = $"Avoir disponible (tous dossiers) : {patient.MontantAvoir:N2} €"
                Else
                    e.ToolTipText = "Aucune séance réalisée n'est due (les séances planifiées ne sont pas comptées)"
                End If

            Case colNbNoShow.Index
                If patient.NbNoShow > 0 Then
                    e.ToolTipText = $"{patient.NbNoShow} absence(s) sans prévenir"
                End If

            Case colAlerte.Index
                If patient.AAlerte Then
                    Dim texte As String = patient.AlerteTxt.Trim()
                    If texte.Length > LONGUEUR_MAX_ALERTE_INFOBULLE Then
                        texte = texte.Substring(0, LONGUEUR_MAX_ALERTE_INFOBULLE).TrimEnd() & "…"
                    End If
                    e.ToolTipText = $"Alerte : {texte}"
                End If

        End Select

    End Sub

    ' Ajoute à une infobulle la ligne « Double-clic : ouvrir la séance SE… » quand une séance est désignée.
    Private Shared Function AjouterAideDoubleClic(texte As String, seance As RepereSeance) As String
        If seance Is Nothing Then
            Return texte
        End If
        Dim aide As String = $"Double-clic : ouvrir la séance {seance.CodeSeance}"
        Return If(String.IsNullOrEmpty(texte), aide, texte & Environment.NewLine & aide)
    End Function

    ' « mardi 21 octobre 2026 à 10:00 » (sans « à … » si l'heure est inconnue).
    Private Shared Function DateHeureLongue(jour As Date, heure As TimeSpan?) As String
        Dim texte As String = jour.ToString("dddd d MMMM yyyy")
        Return If(heure.HasValue, $"{texte} à {heure.Value:hh\:mm}", texte)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Événement : dgvPatients_DataBindingComplete
    ' Version   : V1.0.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Dans l'ordre par défaut (pas de tri par en-tête), sépare les groupes de la liste — patients
    ' attendus aujourd'hui, rendez-vous à venir, sans rendez-vous à venir — par une marge titrée sous
    ' la dernière ligne d'un groupe (« À venir », « Sans rendez-vous à venir »).
    '
    ' Remarques :
    ' - Le premier groupe n'a pas de titre : ses lignes se distinguent déjà (fond, pastilles du jour).
    ' - Même mécanisme que la ligne « Aujourd'hui » de la grille des séances (DividerHeight).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvPatients_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) _
        Handles dgvPatients.DataBindingComplete

        _titresSeparateurs.Clear()
        Dim ordreParDefaut As Boolean = Not UtilsDataGrid.TriUtilisateurActif(dgvPatients)

        For i As Integer = 0 To dgvPatients.Rows.Count - 1

            Dim marge As Integer = 0

            If ordreParDefaut AndAlso i < dgvPatients.Rows.Count - 1 Then
                Dim actuel = TryCast(dgvPatients.Rows(i).DataBoundItem, PatientListeItem)
                Dim suivant = TryCast(dgvPatients.Rows(i + 1).DataBoundItem, PatientListeItem)
                If actuel IsNot Nothing AndAlso suivant IsNot Nothing AndAlso actuel.Groupe <> suivant.Groupe Then
                    marge = HAUTEUR_SEPARATEUR_GROUPE
                    _titresSeparateurs(i) = TitreGroupe(suivant.Groupe)
                End If
            End If

            dgvPatients.Rows(i).DividerHeight = marge
            dgvPatients.Rows(i).Height = HAUTEUR_LIGNE_PASTILLES + marge

        Next

    End Sub

    Private Sub dgvPatients_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) _
        Handles dgvPatients.RowPostPaint

        Dim titre As String = Nothing
        If _titresSeparateurs.TryGetValue(e.RowIndex, titre) Then
            UtilsDataGrid.DessinerSeparateurSousLigne(e, dgvPatients, titre)
        End If

    End Sub

    ' Titre du séparateur placé au-dessus d'un groupe de la liste.
    Private Shared Function TitreGroupe(groupe As GroupeListePatient) As String
        Select Case groupe
            Case GroupeListePatient.Aujourdhui
                Return $"Aujourd'hui — {Date.Today:dddd d MMMM yyyy}"
            Case GroupeListePatient.AVenir
                Return "À venir"
            Case Else
                Return "Sans rendez-vous à venir"
        End Select
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvPatients_CellDoubleClick
    ' Version   : V1.1.0
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Ouvre la fiche du patient sur double-clic dans la grille ; sur « Prochain RDV » ou « Dernière
    ' séance », ouvre la séance désignée (V1.1.0, raccourci de prise de notes).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvPatients_CellDoubleClick(
        sender As Object,
        e As DataGridViewCellEventArgs
    ) Handles dgvPatients.CellDoubleClick

        If e.RowIndex < 0 Then
            Return
        End If

        ' Raccourci de prise de notes : double-clic sur une séance désignée par la ligne.
        Dim patient As PatientListeItem = TryCast(dgvPatients.Rows(e.RowIndex).DataBoundItem, PatientListeItem)
        Dim seance As RepereSeance = Nothing

        If patient IsNot Nothing Then
            If e.ColumnIndex = colProchainRdv.Index Then
                seance = patient.SeanceDuRendezVous
            ElseIf e.ColumnIndex = colDerniereSeance.Index Then
                seance = patient.DerniereSeance
            End If
        End If

        If seance IsNot Nothing Then
            OuvrirSeance(seance)
        Else
            OuvrirFichePatientSelectionne("Consultation")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : OuvrirSeance
    ' Version   : V1.1.1
    ' Date      : 25/10/2026
    '
    ' Rôle      :
    ' Ouvre une séance dans la fenêtre Séance depuis la grille Patients (raccourci de prise de notes,
    ' demande de Joëlle du 17/10/2026), puis recharge la liste en gardant la page courante.
    '
    ' Remarques :
    ' - Même fenêtre et même règle que l'onglet Séances du dossier : consultation seulement si le
    '   dossier est clôturé ou archivé.
    ' - La liste est rechargée même sans enregistrement : le statut de la séance a pu changer.
    ' - V1.1.0 : le patient reste sélectionné après le rechargement (aperçu mis à jour) ; appelée
    '   aussi depuis le panneau d'aperçu.
    ' - V1.1.1 (25/10/2026, audit C.4, ADR-55) : fenêtre principale lue dans le contexte
    '   (IFenetrePrincipale) au lieu de TryCast(FindForm(), Home).
    ' -------------------------------------------------------------------------------------------------
    Private Sub OuvrirSeance(seance As RepereSeance)

        Try

            Dim dossier As Dossier = GestionDossiers.GetDossier(seance.IdDossier)
            Dim lectureSeule As Boolean = dossier Is Nothing OrElse dossier.EstCloture OrElse dossier.EstArchive

            Using editeur As New SeanceEditor(seance.IdDossier, seance.IdSeance, lectureSeule, _context?.FenetrePrincipale)

                editeur.SetContext(_context)
                _context?.SetStatus($"{If(lectureSeule, "Consultation", "Ouverture")} de la séance {seance.CodeSeance}.")
                editeur.ShowDialog(FindForm())

            End Using

            Dim idPatient As Long? = PatientPourApercu()?.IdPatient
            _pageInitiale = pagPatients.PageCourante
            ChargerPatients()
            If idPatient.HasValue Then SelectionnerPatient(idPatient.Value)

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur ouverture de la séance depuis la liste des patients (UC_PatientHome, idSeance={seance.IdSeance}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur("La séance n'a pas pu être ouverte. Veuillez réessayer.")

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : OuvrirFichePatientSelectionne
    ' Version   : V1.1.1
    ' Date      : 25/10/2026
    '
    ' Rôle      :
    ' Point d'entrée commun vers la fiche patient (modes Consultation / Modification).
    '
    ' Paramètres :
    ' - mode : Libellé du mode demandé ("Consultation" ou "Modification")
    '
    ' Remarques :
    ' - Vérifie qu'un patient est sélectionné.
    ' - Ouvre UC_PatientFiche dans le mode demandé en empilant l'écran courant pour le retour (D-Q15).
    ' - V1.1.1 (25/10/2026, audit C.4, ADR-55) : fenêtre principale lue dans le contexte
    '   (IFenetrePrincipale) au lieu de TryCast(FindForm(), Home).
    ' -------------------------------------------------------------------------------------------------
    Private Sub OuvrirFichePatientSelectionne(mode As String)

        Dim patient As PatientListeItem = GetPatientSelectionne()

        If patient Is Nothing Then
            If _context IsNot Nothing Then
                _context.SetStatus("Aucun patient sélectionné.")
            End If
            Return
        End If

        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale

        If fenetre Is Nothing Then
            Return
        End If

        Dim enModification As Boolean =
            String.Equals(mode, "Modification", StringComparison.OrdinalIgnoreCase)

        Dim fiche As New UC_PatientFiche()
        fiche.InitialiserPourPatient(patient.IdPatient, enModification)

        Dim libelleActif As String = If(patient.IsActif, "Actif", "Inactif")
        Dim libelleDossier As String =
            If(patient.SansDossier, "sans dossier", $"dossier {patient.LibelleStatutDossierDominant}")

        fenetre.NavigateToPatientFiche(
            fiche,
            $"Patients > {patient.NomComplet} ({libelleActif}, {libelleDossier})",
            CreerEcranCourant()
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CreerEcranCourant
    ' Version  : V1.0.1
    ' Date     : 17/10/2026
    '
    ' Rôle     :
    ' Construit une fabrique recréant l'écran patients courant à l'identique au retour (mini-pile D-Q15).
    '
    ' Retour   :
    ' - Func(Of UserControl) : fabrique d'un nouveau UC_PatientHome restaurant le filtre courant
    '
    ' Remarques :
    ' - Le filtre courant (texte + statut de suivi) est capturé par closure et réinjecté via
    '   RestaurerFiltre() afin que la liste revienne avec ses critères de recherche.
    ' -------------------------------------------------------------------------------------------------
    Private Function CreerEcranCourant() As Func(Of UserControl)

        Dim texteFiltre As String = txtRecherchePatient.Text
        Dim filtreSuivi As Integer = cboFiltreSuivi.SelectedIndex
        Dim page As Integer = pagPatients.PageCourante
        Dim situation As Integer = _filtreSituation
        Dim filtrePatients As Integer = cboFiltrePatients.SelectedIndex

        Return Function()
                   Dim ecran As New UC_PatientHome()
                   ecran.RestaurerFiltre(texteFiltre, filtreSuivi, page, situation, filtrePatients)
                   Return ecran
               End Function

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RestaurerFiltre
    ' Version   : V1.3.0
    ' Date      : 24/10/2026
    '
    ' Rôle      :
    ' Mémorise le filtre à restaurer au prochain chargement de l'écran (retour depuis la fiche, D-Q15).
    '
    ' Paramètres :
    ' - texteRecherche : Texte de recherche à réappliquer
    ' - filtreSuivi    : Index du statut de suivi (cboFiltreSuivi) à réappliquer
    ' - page           : Page de la grille à réafficher (1 par défaut)
    ' - situation      : Situation du résumé cliquable (V1.2.0)
    ' - filtrePatients : Index du filtre « Patients » (actifs, inactifs, tous ; V1.3.0, remplace l'état
    '                    de la case « Afficher les inactifs » de la V1.2.0)
    '
    ' Remarques :
    ' - Appelée par la fabrique CreerEcranCourant() avant l'ajout du contrôle ; les valeurs sont
    '   réinjectées dans les contrôles par AppliquerFiltreInitial() au Load.
    ' - V1.3.0 (24/10/2026) : filtre « Patients » (index) au lieu de la case « Afficher les inactifs ».
    ' -------------------------------------------------------------------------------------------------
    Public Sub RestaurerFiltre(texteRecherche As String, filtreSuivi As Integer, Optional page As Integer = 1,
                               Optional situation As Integer = SituationToutes,
                               Optional filtrePatients As Integer = FiltrePatientsActifs)

        _filtreInitialTexte = texteRecherche
        _filtreInitialSuivi = filtreSuivi
        _pageInitiale = Math.Max(1, page)
        _filtreInitialSituation = situation
        _filtreInitialPatients = filtrePatients

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerFiltreInitial
    ' Version   : V1.0.1
    ' Date      : 17/10/2026
    '
    ' Rôle      :
    ' Réinjecte le filtre mémorisé (RestaurerFiltre) dans les contrôles avant le chargement de la liste.
    '
    ' Remarques :
    ' - Sans appel préalable à RestaurerFiltre, les contrôles conservent leur état par défaut.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerFiltreInitial()

        If _filtreInitialTexte IsNot Nothing Then
            txtRecherchePatient.Text = _filtreInitialTexte
        End If

        If _filtreInitialSuivi >= 0 AndAlso _filtreInitialSuivi < cboFiltreSuivi.Items.Count Then
            cboFiltreSuivi.SelectedIndex = _filtreInitialSuivi
        End If

        _filtreSituation = _filtreInitialSituation
        If _filtreInitialPatients >= 0 AndAlso _filtreInitialPatients < cboFiltrePatients.Items.Count Then
            cboFiltrePatients.SelectedIndex = _filtreInitialPatients
        End If

    End Sub

#End Region

End Class
