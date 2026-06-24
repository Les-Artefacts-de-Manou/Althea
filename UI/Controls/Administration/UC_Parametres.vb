' -------------------------------------------------------------------------------------------------
' UserControl : UC_Parametres
' Projet      : Althéa
' Version     : V1.7.0
' Date        : 14/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' UserControl dédié à la gestion des paramètres applicatifs.
'
' Responsabilités :
' - Charger les paramètres actifs depuis la couche métier (GestionParametres)
' - Afficher les paramètres regroupés par catégorie (groupe_parametre) dans des onglets dynamiques
' - Permettre la consultation, création, modification des paramètres selon le mode d'accès (Admin/SuperUser)
' - Gérer l'affichage dynamique selon le type de paramètre (type_valeur : Texte, Entier, Booléen, etc.)
' - Valider les saisies selon les contraintes (valeur_min, valeur_max, longueur_max)
' - Afficher les paramètres désactivés en option (chkAfficherInactifs)
' - Gérer les modes d'édition (Consultation, Nouveau, Modification) avec gestion dirty
' - Journaliser les actions via GestionLog
'
' Remarques :
' Remarques   :
' - Chargé dynamiquement dans le panneau central de Home via NavigateToAdminView()
' - Implémente IContextAwareUserControl pour injection du contexte UI partagé
' - Aucun accès direct à la base de données (tout passe par GestionParametres)
' - L''affichage est dynamique : TabPages et DataGridViews créés à la volée
' - Les paramètres techniques (non modifiables) sont affichés en lecture seule
' - Le mode Admin donne accès à tous les paramètres, le mode SuperUser est limité
'
' Dépendances :
' - GestionParametres (chargement/sauvegarde des paramètres)
' - ParametreApplication (modèle de données)
' - UserControlContext (contexte UI injecté par Home)
' - ModeAccesParametres (Enum : Admin, SuperUser)
' - UtilsButtons (thématisation des boutons)
' - UITheme (couleurs et styles)
' - GestionLog (journalisation)
'
' Interface   :
' - IContextAwareUserControl : Pour injection du contexte UI partagé
'
' Imports     :
' - System.Drawing.Imaging
' - System.Linq
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_Parametres
    Implements IContextAwareUserControl


#Region "Variables / Constantes"

    ' -------------------------------------------------------------------------------------------------
    ' Variables privées du UserControl
    ' Version     : V1.7.0
    ' Date        : 14/05/2026
    '
    ' Rôle        :
    ' Stockent l'état du UserControl, le contexte UI, et les données de paramètres.
    '
    ' Remarques   :
    ' - _parametres : Liste des paramètres chargés depuis GestionParametres
    ' - _modeAcces : Mode d'accès Admin ou SuperUser (détermine les droits)
    ' - _modeEdition : OBSOLETE (remplacé par _modeEditionParametre)
    ' - _context : Contexte UI injecté par Home via SetContext()
    ' - _modeEditionParametre : Mode d'édition courant (Consultation, Nouveau, Modification)
    ' - _isDirty : Indique si des modifications non enregistrées sont en cours
    ' - _parametreCourant : Paramètre actuellement sélectionné ou en cours d'édition
    ' - _suspendSelectionChanged : Flag pour éviter les événements SelectionChanged en cascade
    ' -------------------------------------------------------------------------------------------------
    Private _parametres As List(Of ParametreApplication)
    Private _modeAcces As ModeAccesParametres = ModeAccesParametres.SuperUser
    Private _modeEdition As Boolean = False

    Private _context As UserControlContext

    Private _modeEditionParametre As ModeEditionParametre = ModeEditionParametre.Consultation
    Private _isDirty As Boolean = False
    Private _parametreCourant As ParametreApplication = Nothing
    Private _suspendSelectionChanged As Boolean = False

#End Region

#Region "Enums"

    ' -------------------------------------------------------------------------------------------------
    ' Enum       : ModeEditionParametre
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Définit les modes d'édition possibles pour un paramètre.
    '
    ' Valeurs    :
    ' - Consultation : Mode lecture seule (par défaut)
    ' - Nouveau      : Création d'un nouveau paramètre
    ' - Modification : Modification d'un paramètre existant
    '
    ' Remarques  :
    ' - Utilisé pour contrôler l'état des boutons et l'éditabilité des champs
    ' -------------------------------------------------------------------------------------------------
    Private Enum ModeEditionParametre
        Consultation
        Nouveau
        Modification
    End Enum

#End Region

#Region "Constructeurs"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version      : V1.0.0
    ' Date         : 14/05/2026
    '
    ' Rôle         :
    ' Constructeur par défaut requis par le WinForms Designer.
    '
    ' Paramètres   :
    ' - Aucun
    '
    ' Remarques    :
    ' - Ne pas supprimer : obligatoire pour le Designer
    ' - _modeAcces est initialisé à ModeAccesParametres.SuperUser par défaut
    ' - En production, utiliser le constructeur surchargé New(modeAcces)
    '
    ' Exceptions   :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub New()

        InitializeComponent()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version      : V1.0.0
    ' Date         : 14/05/2026
    '
    ' Rôle         :
    ' Initialise UC_Parametres avec le mode d'accès spécifié (Admin ou SuperUser).
    '
    ' Paramètres   :
    ' - modeAcces : Mode d'accès (Admin : tous les paramètres, SuperUser : limité)
    '
    ' Remarques    :
    ' - Constructeur utilisé en production par Home via NavigateToAdminView()
    ' - _context sera injecté ultérieurement via SetContext()
    '
    ' Exceptions   :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(modeAcces As ModeAccesParametres)

        InitializeComponent()

        _modeAcces = modeAcces

    End Sub

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetContext
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Injecte le contexte UI partagé fourni par Home (implémentation IContextAwareUserControl).
    '
    ' Paramètres :
    ' - context : Instance de UserControlContext partagée entre Home et tous les UserControls
    '
    ' Remarques  :
    ' - Appelé automatiquement par Home après instanciation du UserControl
    ' - Permet d'accéder aux contrôles partagés : StatusStrip, ErrorProvider, ToolTip
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(context As UserControlContext) _
    Implements IContextAwareUserControl.SetContext

        _context = context

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InitializeToolTips
    ' Version    : V1.0.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Initialise les infobulles de tous les contrôles du UserControl.
    '
    ' Paramètres :
    ' - Aucun
    '
    ' Remarques  :
    ' - Si _context est Nothing, l'initialisation est ignorée sans erreur
    ' - Configure les infobulles pour tabParametres, chkAfficherInactifs, btnNouveau,
    '   btnModifier, btnEnregistrer, btnAnnuler
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitializeToolTips()

        If _context Is Nothing Then Return

        _context.SetToolTip(tabParametres, "Paramètres applicatifs regroupés par catégorie.")
        _context.SetToolTip(chkAfficherInactifs, "Afficher aussi les paramètres désactivés pour pouvoir les consulter ou les réactiver.")
        _context.SetToolTip(btnNouveau, "Créer un nouveau paramètre applicatif.")
        _context.SetToolTip(btnModifier, "Modifier le paramètre sélectionné.")
        _context.SetToolTip(btnEnregistrer, "Enregistrer les modifications.")
        _context.SetToolTip(btnAnnuler, "Annuler les modifications en cours.")

    End Sub

#End Region

#Region "Initialisation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UC_Parametres_Load
    ' Projet     : Althéa
    ' Version    : V1.0.0
    ' Date       : 28/04/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       :
    ' Initialise l’écran de gestion des paramètres applicatifs.
    '
    ' Remarques  :
    ' - Charge les paramètres actifs depuis la couche métier.
    ' - Aucun accès direct à la base de données depuis l’UI.
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_Parametres_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        UtilsButtons.InitStandardButton(btnNouveau)
        UtilsButtons.InitStandardButton(btnModifier)
        UtilsButtons.InitStandardButton(btnEnregistrer)
        UtilsButtons.InitStandardButton(btnAnnuler)

        InitializeToolTips()

        UpdateContextInfo("Gestion des paramètres applicatifs prête.")

        GestionLog.EcrireLog(
    "UI: ouverture de l'écran des paramètres applicatifs.",
    GestionLog.LogLevel.Rapide,
    GestionLog.LogCategory.UI
)

        ChargerParametres()

        UpdateButtonsState()

    End Sub

#End Region

#Region "Chargement des données"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ChargerParametres
    ' Projet     : Althéa
    ' Version    : V1.6.0
    ' Date       : 02/05/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       :
    ' Charge les paramètres actifs depuis la couche métier et crée les onglets
    ' dynamiquement selon le groupe_parametre.
    '
    ' Remarques  :
    ' - Aucun accès direct à la base de données depuis l’UI.
    ' - Les TabPages sont générées depuis les données de tec_parametres.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerParametres()

        Try
            _parametres = GestionParametres.GetParametres(_modeAcces, chkAfficherInactifs.Checked)
            _suspendSelectionChanged = True

            tabParametres.TabPages.Clear()

            Dim groupes = _parametres _
            .GroupBy(Function(p) p.GroupeParametre) _
            .OrderBy(Function(g) g.Key)

            For Each groupe In groupes

                Dim tab As New TabPage With {
        .Text = groupe.Key,
        .BackColor = UITheme.DynamicTabBack
    }

                Dim tlp As New TableLayoutPanel With {
        .Dock = DockStyle.Fill,
        .ColumnCount = 2
    }

                tlp.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))
                tlp.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))

                ' -----------------------------
                ' DataGridView (liste)
                ' -----------------------------
                Dim dgv As DataGridView = CreerDataGridParametres()

                dgv.DataSource = groupe.OrderBy(Function(p) p.OrdreAffichage).ToList()

                ' -----------------------------
                ' Panel détail
                ' -----------------------------
                Dim pnlDetail As New Panel With {
    .Dock = DockStyle.Fill,
    .BackColor = UITheme.DynamicPanelDetailBack
}
                ConfigurerDataGridParametres(dgv, pnlDetail)

                ' Assemblage
                tlp.Controls.Add(dgv, 0, 0)
                tlp.Controls.Add(pnlDetail, 1, 0)

                tab.Controls.Add(tlp)
                tabParametres.TabPages.Add(tab)

                InitialiserSelection(dgv, pnlDetail)

            Next

            If _parametres.Count = 0 Then
                Dim tabVide As New TabPage With {
                .Text = "Paramètres",
                .BackColor = UITheme.DynamicTabBack
            }

                Dim lblVide As New Label With {
                .Text = "Aucun paramètre actif trouvé.",
                .AutoSize = True,
                .Location = New Point(16, 16),
                .ForeColor = UITheme.DynamicLabelFore
            }
                tabVide.Controls.Add(lblVide)
                tabParametres.TabPages.Add(tabVide)
            End If

            _suspendSelectionChanged = False

            If chkAfficherInactifs.Checked Then
                UpdateContextInfo("Paramètres actifs et désactivés chargés.")
            Else
                UpdateContextInfo("Paramètres actifs chargés.")
            End If

        Catch ex As Exception

            _suspendSelectionChanged = False
            tabParametres.TabPages.Clear()

            Dim tabErreur As New TabPage With {
            .Text = "Erreur",
            .BackColor = UITheme.DynamicTabBack
        }

            Dim lblErreur As New Label With {
            .Text = "Erreur lors du chargement des paramètres.",
            .AutoSize = True,
            .Location = New Point(16, 16),
            .ForeColor = UITheme.DynamicErrorFore
        }

            tabErreur.Controls.Add(lblErreur)
            tabParametres.TabPages.Add(tabErreur)

            GestionLog.EcrireLog(
            "UI: erreur lors du chargement des paramètres applicatifs.",
            GestionLog.LogLevel.Succinct,
            GestionLog.LogCategory.UI,
            ex
        )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : RestaurerSelection
    ' Projet     : Althéa
    ' Version    : V1.1.0
    ' Date       : 30/04/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       :
    ' Restaure l’onglet, la ligne sélectionnée et le panneau détail après rechargement
    ' des paramètres.
    '
    ' Paramètres :
    ' - groupeParametre : groupe du paramètre à retrouver
    ' - idParametre     : identifiant du paramètre à resélectionner
    ' -------------------------------------------------------------------------------------------------
    Private Sub RestaurerSelection(groupeParametre As String, idParametre As ULong)

        For Each tab As TabPage In tabParametres.TabPages

            If Not String.Equals(tab.Text, groupeParametre, StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If

            tabParametres.SelectedTab = tab

            If tab.Controls.Count = 0 Then Return

            Dim tlp As TableLayoutPanel = TryCast(tab.Controls(0), TableLayoutPanel)
            If tlp Is Nothing Then Return

            Dim dgv As DataGridView = TryCast(tlp.GetControlFromPosition(0, 0), DataGridView)
            Dim pnlDetail As Panel = TryCast(tlp.GetControlFromPosition(1, 0), Panel)

            If dgv Is Nothing OrElse pnlDetail Is Nothing Then Return

            For Each row As DataGridViewRow In dgv.Rows

                Dim parametre As ParametreApplication =
                TryCast(row.DataBoundItem, ParametreApplication)

                If parametre Is Nothing Then Continue For

                If parametre.IdParametre = idParametre Then

                    dgv.ClearSelection()

                    row.Selected = True
                    dgv.CurrentCell = row.Cells(0)

                    _parametreCourant = parametre

                    AfficherDetailParametre(pnlDetail, parametre)
                    UpdateButtonsState()
                    Return

                End If

            Next


            Return

        Next

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : RestaurerOnglet
    ' Projet     : Althéa
    ' Version    : V1.0.0
    ' Date       : 30/04/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       :
    ' Restaure l’onglet correspondant au groupe indiqué.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RestaurerOnglet(groupeParametre As String)

        For Each tab As TabPage In tabParametres.TabPages

            If String.Equals(tab.Text, groupeParametre, StringComparison.OrdinalIgnoreCase) Then
                tabParametres.SelectedTab = tab
                Return
            End If

        Next

    End Sub

#End Region

#Region "Gestion des événements UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnModifier_Click
    ' Projet     : Althéa
    ' Version    : V1.0.0
    ' Date       : 29/04/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       : Mode édition du paramètre sélectionné
    ' -------------------------------------------------------------------------------------------------
    ' 
    Private Sub btnModifier_Click(sender As Object, e As EventArgs) Handles btnModifier.Click

        If _parametreCourant Is Nothing Then

            UpdateContextInfo("Sélectionne d’abord un paramètre à modifier.")

            GestionLog.EcrireLog(
                "UI: demande de modification sans paramètre sélectionné.",
                GestionLog.LogLevel.Rapide,
                GestionLog.LogCategory.UI
            )

            Return
        End If

        _modeEditionParametre = ModeEditionParametre.Modification
        _modeEdition = True
        _isDirty = False

        RafraichirDetail()
        UpdateContextInfo("Modification du paramètre en cours.")

        GestionLog.EcrireLog(
    $"UI: modification du paramètre initialisée ({_parametreCourant.CodeParametre}).",
    GestionLog.LogLevel.Succinct,
    GestionLog.LogCategory.UI
)
        UpdateButtonsState()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnNouveau_Click
    ' Projet     : Althéa
    ' Version    : V1.0.0
    ' Date       : 30/04/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       :
    ' Initialise la création d’un nouveau paramètre.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnNouveau_Click(sender As Object, e As EventArgs) Handles btnNouveau.Click

        If _modeAcces <> ModeAccesParametres.Admin Then

            If _context IsNot Nothing Then
                _context.ClearAllErrors()
            End If

            UpdateContextInfo("Création réservée au mode administrateur.")

            GestionLog.EcrireLog(
        "UI: tentative de création de paramètre sans droit Admin.",
        GestionLog.LogLevel.Succinct,
        GestionLog.LogCategory.UI
    )

            Return

        End If

        _modeEditionParametre = ModeEditionParametre.Nouveau
        _modeEdition = True
        _isDirty = False

        ' Nouveau paramètre vide
        Dim groupeCourant As String = GetGroupeParametreCourant()

        _parametreCourant = New ParametreApplication With {
    .GroupeParametre = groupeCourant,
    .TypeValeur = "STRING",
    .Actif = True,
    .ModifiableUtilisateur = True
}

        AfficherDetailParametre(GetPanelDetailCourant(), _parametreCourant)
        UpdateContextInfo($"Création d’un nouveau paramètre dans le groupe {groupeCourant}.")

        GestionLog.EcrireLog(
    $"UI: création d'un nouveau paramètre initialisée dans le groupe {groupeCourant}.",
    GestionLog.LogLevel.Succinct,
    GestionLog.LogCategory.UI
)

        UpdateButtonsState()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnEnregistrer_Click
    ' Projet     : Althéa
    ' Version    : V1.2.0
    ' Date       : 30/04/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       :
    ' Enregistre le paramètre courant, en création ou en modification.
    '
    ' Remarques  :
    ' - Les valeurs sont relues depuis l’UI avant sauvegarde.
    ' - Les champs obligatoires sont validés avant appel DB.
    ' - En modification, la sélection est restaurée sur la ligne modifiée.
    ' - En création, la sélection sera restaurée ensuite via la clé technique.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEnregistrer_Click(sender As Object, e As EventArgs) Handles btnEnregistrer.Click

        If _parametreCourant Is Nothing Then Return

        UpdateContextInfo("Enregistrement en cours...")

        Dim tab = tabParametres.SelectedTab
        If tab Is Nothing OrElse tab.Controls.Count = 0 Then Return

        Dim tlp = TryCast(tab.Controls(0), TableLayoutPanel)
        If tlp Is Nothing Then Return

        Dim pnl = TryCast(tlp.GetControlFromPosition(1, 0), Panel)
        If pnl Is Nothing Then Return

        ' Mémorisation avant sauvegarde
        Dim groupeActuel As String = _parametreCourant.GroupeParametre
        Dim idParametreActuel As ULong = _parametreCourant.IdParametre
        Dim cleParametreActuelle As String = _parametreCourant.CleParametre

        ' Lecture UI -> objet métier
        LireValeursDepuisUI(pnl, _parametreCourant)

        ' Normalisation des codes techniques avant validation / sauvegarde
        _parametreCourant.CleParametre =
    UtilsString.NormalizeTechnicalCode(_parametreCourant.CleParametre)

        ' Validation avant DB
        If Not ValiderParametreAvantSauvegarde(_parametreCourant, pnl) Then
            Return
        End If

        Try

            If _modeEditionParametre = ModeEditionParametre.Nouveau Then
                GestionParametres.InsertParametre(_parametreCourant)
                ' En création, id_parametre est généré par la DB.
                ' On garde donc la clé pour retrouver la ligne après rechargement.
                groupeActuel = _parametreCourant.GroupeParametre
                cleParametreActuelle = _parametreCourant.CleParametre
            Else
                GestionParametres.UpdateParametre(_parametreCourant)
                groupeActuel = _parametreCourant.GroupeParametre
                idParametreActuel = _parametreCourant.IdParametre
            End If

            _isDirty = False
            _modeEdition = False
            _modeEditionParametre = ModeEditionParametre.Consultation

            ChargerParametres()

            UpdateContextInfo("Paramètre enregistré avec succès.")

            GestionLog.EcrireLog(
    $"UI: paramètre enregistré ({_parametreCourant.CodeParametre}).",
    GestionLog.LogLevel.Succinct,
    GestionLog.LogCategory.UI
)

            If _modeEditionParametre = ModeEditionParametre.Consultation Then

                If idParametreActuel > 0 Then
                    RestaurerSelection(groupeActuel, idParametreActuel)
                Else
                    RestaurerOnglet(groupeActuel)
                End If

            End If

            UpdateButtonsState()

        Catch ex As Exception

            UpdateContextInfo("Erreur lors de l’enregistrement du paramètre.")

            GestionLog.EcrireLog(
            "UI: erreur lors de l'enregistrement d'un paramètre applicatif.",
            GestionLog.LogLevel.Succinct,
            GestionLog.LogCategory.UI,
            ex
        )

            DialogChoix.Erreur(
            "Erreur lors de l'enregistrement du paramètre." & Environment.NewLine & ex.Message,
            "Althéa"
        )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnAnnuler_Click
    ' Projet     : Althéa
    ' Version    : V1.4.0
    ' Date       : 30/04/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       :
    ' Annule une modification en cours sans recharger toute l’interface.
    '
    ' Remarques  :
    ' - En mode Modification, les changements non enregistrés sont uniquement présents dans les contrôles UI.
    ' - On revient donc simplement au détail du paramètre courant.
    ' - Cela conserve l’onglet et la ligne sélectionnés.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAnnuler_Click(sender As Object, e As EventArgs) Handles btnAnnuler.Click

        If _context IsNot Nothing Then
            _context.ClearAllErrors()
        End If

        _isDirty = False
        _modeEdition = False
        _modeEditionParametre = ModeEditionParametre.Consultation

        UpdateContextInfo("Modifications annulées.")
        RafraichirDetail()

        GestionLog.EcrireLog(
    "UI: annulation des modifications en cours.",
    GestionLog.LogLevel.Rapide,
    GestionLog.LogCategory.UI
)
        UpdateButtonsState()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InitializeActionButtons
    ' Projet     : Althéa
    ' Version    : V1.0.0
    ' Date       : 30/04/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       :
    ' Initialise les boutons standards de UC_Parametres.
    '
    ' Remarques  :
    ' - Les Tags doivent être définis dans le Designer.
    ' - Les états Normal / Hover / Disabled sont gérés par UtilsForm.
    ' - La logique Enabled reste gérée par UpdateButtonsState.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitializeActionButtons()

        UtilsButtons.InitStandardButton(btnNouveau)
        UtilsButtons.InitStandardButton(btnModifier)
        UtilsButtons.InitStandardButton(btnEnregistrer)
        UtilsButtons.InitStandardButton(btnAnnuler)


    End Sub

    Private Sub tabParametres_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tabParametres.SelectedIndexChanged

        If tabParametres.SelectedTab Is Nothing Then
            Return
        End If

        UpdateContextInfo("Groupe de paramètres affiché.")

        GestionLog.EcrireLog(
        $"UI: changement d'onglet vers {tabParametres.SelectedTab.Text}.",
        GestionLog.LogLevel.Rapide,
        GestionLog.LogCategory.UI
    )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : chkAfficherInactifs_CheckedChanged
    ' Projet     : Althéa
    ' Version    : V1.2
    ' Date       : 02/05/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       :
    ' Recharge la liste des paramètres selon l’option d’affichage
    ' des paramètres désactivés.
    ' -------------------------------------------------------------------------------------------------
    Private Sub chkAfficherInactifs_CheckedChanged(sender As Object, e As EventArgs) Handles chkAfficherInactifs.CheckedChanged

        Dim groupeAvant As String = Nothing
        Dim idAvant As ULong = 0

        If _parametreCourant IsNot Nothing Then
            groupeAvant = _parametreCourant.GroupeParametre
            idAvant = _parametreCourant.IdParametre
        End If

        GestionLog.EcrireLog(
        $"UI: affichage paramètres désactivés = {chkAfficherInactifs.Checked}.",
        GestionLog.LogLevel.Rapide,
        GestionLog.LogCategory.UI
    )

        ChargerParametres()

        If Not String.IsNullOrWhiteSpace(groupeAvant) AndAlso idAvant > 0 Then
            RestaurerSelection(groupeAvant, idAvant)
        End If

    End Sub


#End Region

#Region "Affichage / Détail"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AfficherDetailParametre
    ' Projet     : Althéa
    ' Version    : V1.3.0
    ' Date       : 03/05/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       :
    ' Affiche le détail d’un paramètre sélectionné dans le panneau de détail.
    '
    ' Paramètres :
    ' - pnlDetail : panel cible du détail
    ' - parametre : paramètre à afficher
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherDetailParametre(pnlDetail As Panel, parametre As ParametreApplication)

        pnlDetail.Controls.Clear()
        pnlDetail.Padding = New Padding(16)

        Dim tlpDetail As New TableLayoutPanel With {
        .Dock = DockStyle.Top,
        .AutoSize = True,
        .ColumnCount = 2,
        .BackColor = UITheme.DynamicPanelDetailBack
    }

        tlpDetail.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 160))
        tlpDetail.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))

        Dim isAdmin As Boolean = _modeAcces = ModeAccesParametres.Admin

        Dim canEditStructure As Boolean =
        isAdmin AndAlso
        (_modeEditionParametre = ModeEditionParametre.Nouveau OrElse
         _modeEditionParametre = ModeEditionParametre.Modification)

        Dim canEditCle As Boolean =
        isAdmin AndAlso _modeEditionParametre = ModeEditionParametre.Nouveau

        Dim canEditValeur As Boolean =
        _modeEditionParametre <> ModeEditionParametre.Consultation AndAlso
        (
            isAdmin OrElse
            (_modeAcces = ModeAccesParametres.SuperUser AndAlso parametre.ModifiableUtilisateur)
        )

        AjouterChamp(tlpDetail, "Libellé", parametre.LibelleParametre, canEditStructure, "txtLibelle")
        AjouterChamp(tlpDetail, "Clé technique", parametre.CleParametre, canEditCle, "txtCleParametre")
        AjouterChamp(tlpDetail, "Groupe", parametre.GroupeParametre, canEditStructure, "txtGroupe")
        AjouterChamp(tlpDetail, "Type", parametre.TypeValeur, canEditStructure, "cmbType", "TYPE")

        AjouterChampValeur(tlpDetail, parametre, canEditValeur)

        AjouterChampDescription(tlpDetail, parametre, canEditStructure)

        AjouterCheckBox(tlpDetail, "Actif", parametre.Actif, canEditStructure, "chkActif")
        AjouterCheckBox(tlpDetail, "Modifiable", parametre.ModifiableUtilisateur, canEditStructure, "chkModifiable")

        pnlDetail.Controls.Add(tlpDetail)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : RafraichirDetail
    ' Projet     : Althéa
    ' Version    : V1.0.0
    ' Date       : 29/04/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       : 
    ' Re-déclenche l'affichage du paramètre sélectionné
    ' -------------------------------------------------------------------------------------------------

    Private Sub RafraichirDetail()

        ' Re-déclenche l'affichage du paramètre sélectionné
        Dim tab = tabParametres.SelectedTab

        If tab Is Nothing Then Return

        Dim tlp = TryCast(tab.Controls(0), TableLayoutPanel)
        If tlp Is Nothing Then Return

        Dim dgv = TryCast(tlp.GetControlFromPosition(0, 0), DataGridView)
        Dim pnl = TryCast(tlp.GetControlFromPosition(1, 0), Panel)

        If dgv Is Nothing OrElse pnl Is Nothing Then Return
        If dgv.CurrentRow Is Nothing Then Return

        Dim parametre = TryCast(dgv.CurrentRow.DataBoundItem, ParametreApplication)
        If parametre Is Nothing Then Return

        AfficherDetailParametre(pnl, parametre)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AppliquerStyleControleDetail
    ' Projet     : Althéa
    ' Version    : V1.0.0
    ' Date       : 16/05/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       :
    ' Applique le style Althéa aux contrôles générés par code
    ' dans le panneau détail de UC_Parametres.
    '
    ' Remarque   :
    ' Certains contrôles WinForms natifs, notamment DateTimePicker
    ' et ComboBox désactivée, ne respectent pas toujours totalement
    ' BackColor / ForeColor.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerStyleControleDetail(
    ctrl As Control,
    editable As Boolean
)

        If ctrl Is Nothing Then
            Return
        End If

        ctrl.BackColor = UITheme.ColorBeigeClair
        ctrl.ForeColor = UITheme.DynamicTextFore

        If TypeOf ctrl Is TextBox Then

            Dim txt As TextBox =
            DirectCast(ctrl, TextBox)

            txt.BackColor = UITheme.ColorBeigeClair
            txt.ForeColor = UITheme.DynamicTextFore

            txt.BorderStyle =
            If(editable, BorderStyle.Fixed3D, BorderStyle.None)

            Return

        End If

        If TypeOf ctrl Is ComboBox Then

            Dim cmb As ComboBox =
            DirectCast(ctrl, ComboBox)

            UtilsControls.InitComboBoxTheme(cmb)

            Return

        End If

        If TypeOf ctrl Is NumericUpDown Then

            Dim num As NumericUpDown =
            DirectCast(ctrl, NumericUpDown)

            num.BackColor = UITheme.ColorBeigeClair
            num.ForeColor = UITheme.DynamicTextFore

            Return

        End If

        If TypeOf ctrl Is DateTimePicker Then

            Dim dtp As DateTimePicker =
            DirectCast(ctrl, DateTimePicker)

            dtp.CalendarMonthBackground = UITheme.ColorBeigeClair
            dtp.CalendarForeColor = UITheme.DynamicTextFore
            dtp.CalendarTitleBackColor = UITheme.ColorSaugeClair
            dtp.CalendarTitleForeColor = UITheme.DynamicTextFore

            Return

        End If

        If TypeOf ctrl Is CheckBox Then

            Dim chk As CheckBox =
            DirectCast(ctrl, CheckBox)

            chk.BackColor = UITheme.ColorBeigeClair
            chk.ForeColor = UITheme.DynamicTextFore
            chk.UseVisualStyleBackColor = False
            chk.FlatStyle = FlatStyle.Flat

            chk.FlatAppearance.CheckedBackColor = UITheme.ColorBeigeClair
            chk.FlatAppearance.MouseOverBackColor = UITheme.ColorBeigeClair
            chk.FlatAppearance.MouseDownBackColor = UITheme.ColorBeigeClair

            Return

        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AjouterChamp
    ' Projet     : Althéa
    ' Version    : V1.4.0
    ' Date       : 02/05/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       :
    ' Ajoute un champ dans le panneau de détail.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AjouterChamp(tlp As TableLayoutPanel,
                         libelle As String,
                         valeur As String,
                         editable As Boolean,
                         nomControle As String,
                         Optional typeChamp As String = "TEXT")

        Dim rowIndex As Integer = tlp.RowCount

        tlp.RowCount += 1
        tlp.RowStyles.Add(New RowStyle(SizeType.AutoSize))

        Dim lbl As New Label With {
        .Text = libelle & " :",
        .AutoSize = True,
        .ForeColor = UITheme.DynamicLabelFore,
        .Font = New Font("Calibri", 10, FontStyle.Bold),
        .Margin = New Padding(4, 8, 12, 8)
    }

        Dim ctrl As Control

        If typeChamp = "TYPE" Then

            Dim cmb As New ComboBox With {
            .Name = nomControle,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Width = 400,
            .Margin = New Padding(4, 5, 4, 5),
            .Enabled = editable
        }

            AppliquerStyleControleDetail(cmb, editable)

            cmb.Items.AddRange(New String() {
            "STRING",
            "BOOL",
            "INT",
            "DECIMAL",
            "DATE",
            "PATH"
        })

            If Not String.IsNullOrWhiteSpace(valeur) AndAlso cmb.Items.Contains(valeur) Then
                cmb.SelectedItem = valeur
            Else
                cmb.SelectedItem = "STRING"
            End If

            If editable Then
                AddHandler cmb.SelectedIndexChanged,
                Sub()
                    _isDirty = True
                    UpdateButtonsState()
                End Sub
            End If

            ctrl = cmb

        ElseIf nomControle = "txtGroupe" Then

            Dim cmbGroupe As New ComboBox With {
    .Name = nomControle,
    .Text = If(valeur, ""),
    .DropDownStyle = ComboBoxStyle.DropDown,
    .Width = 400,
    .Enabled = editable,
    .TabStop = editable,
    .Margin = New Padding(4, 5, 4, 5)
}
            AppliquerStyleControleDetail(cmbGroupe, editable)

            Dim groupesExistants = _parametres _
                .Select(Function(p) p.GroupeParametre) _
                .Where(Function(g) Not String.IsNullOrWhiteSpace(g)) _
                .Distinct() _
                .OrderBy(Function(g) g) _
                .ToArray()

            cmbGroupe.Items.AddRange(groupesExistants)

            If editable Then
                AddHandler cmbGroupe.TextChanged,
                    Sub()
                        _isDirty = True
                        UpdateButtonsState()

                        If _context IsNot Nothing Then
                            _context.ClearError(cmbGroupe)
                        End If
                    End Sub
            End If

            ctrl = cmbGroupe

        Else

            Dim txt As New TextBox With {
                .Name = nomControle,
                .Text = If(valeur, ""),
                .ReadOnly = Not editable,
                .Width = 400,
                .Margin = New Padding(4, 5, 4, 5)
            }

            AppliquerStyleControleDetail(txt, editable)

            If nomControle = "txtCleParametre" Then
                txt.CharacterCasing = CharacterCasing.Upper
            End If

            If editable Then
                AddHandler txt.TextChanged,
                    Sub()
                        _isDirty = True
                        UpdateButtonsState()

                        If _context IsNot Nothing Then
                            _context.ClearError(txt)
                        End If
                    End Sub
            End If

            ctrl = txt

        End If

        tlp.Controls.Add(lbl, 0, rowIndex)
        tlp.Controls.Add(ctrl, 1, rowIndex)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AjouterChampValeur
    ' Projet     : Althéa
    ' Version    : V1.3.0
    ' Date       : 03/05/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       :
    ' Ajoute la ligne Valeur dans le détail du paramètre, en fonction du type de valeur du paramètre.
    ' Branche la détection de modification si le champ est éditable.
    '
    ' Paramètres : 
    ' - tlp : TableLayoutPanel contenant le panneau de détail
    ' - parametre : ParametreApplication contenant les données du paramètre
    ' - editable : Boolean indiquant si le champ est editable
    ' -------------------------------------------------------------------------------------------------
    Private Sub AjouterChampValeur(tlp As TableLayoutPanel,
                               parametre As ParametreApplication,
                               editable As Boolean)

        Dim rowIndex As Integer = tlp.RowCount

        tlp.RowCount += 1
        tlp.RowStyles.Add(New RowStyle(SizeType.AutoSize))

        Dim lbl As New Label With {
        .Text = "Valeur :",
        .AutoSize = True,
        .ForeColor = UITheme.DynamicLabelFore,
        .Font = New Font("Calibri", 10, FontStyle.Bold),
        .Margin = New Padding(4, 8, 12, 8)
    }

        Dim ctrl As Control
        Dim typeValeur As String = If(parametre.TypeValeur, "STRING").Trim().ToUpperInvariant()
        Dim valeur As String = If(parametre.ValeurParametre, "")

        Select Case typeValeur

            Case "BOOL"

                Dim chk As New CheckBox With {
                .Name = "chkValeur",
                .Checked = valeur = "1" OrElse valeur.Equals("TRUE", StringComparison.OrdinalIgnoreCase),
                .Enabled = editable,
                .Margin = New Padding(4, 8, 4, 8)
               }
                AppliquerStyleControleDetail(chk, editable)

                If editable Then
                    AddHandler chk.CheckedChanged,
                    Sub()
                        _isDirty = True
                        UpdateButtonsState()

                        If _context IsNot Nothing Then
                            _context.ClearError(chk)
                        End If
                    End Sub
                End If

                ctrl = chk

            Case "INT"

                Dim num As New NumericUpDown With {
                .Name = "numValeur",
                .Width = 200,
                .DecimalPlaces = 0,
                .Minimum = -1000000000D,
                .Maximum = 1000000000D,
                .Enabled = editable,
                .Margin = New Padding(4, 5, 4, 5)
            }

                AppliquerStyleControleDetail(num, editable)

                Dim parsed As Decimal
                If Decimal.TryParse(valeur, parsed) Then
                    num.Value = Math.Max(num.Minimum, Math.Min(num.Maximum, parsed))
                End If

                If editable Then
                    AddHandler num.ValueChanged,
                    Sub()
                        _isDirty = True
                        UpdateButtonsState()

                        If _context IsNot Nothing Then
                            _context.ClearError(num)
                        End If
                    End Sub
                End If

                ctrl = num

            Case "DECIMAL"

                Dim num As New NumericUpDown With {
                .Name = "numValeur",
                .Width = 200,
                .DecimalPlaces = 2,
                .Minimum = -1000000000D,
                .Maximum = 1000000000D,
                .Enabled = editable,
                .Margin = New Padding(4, 5, 4, 5)
            }
                AppliquerStyleControleDetail(num, editable)

                Dim parsed As Decimal
                If Decimal.TryParse(valeur, parsed) Then
                    num.Value = Math.Max(num.Minimum, Math.Min(num.Maximum, parsed))
                End If

                If editable Then
                    AddHandler num.ValueChanged,
                    Sub()
                        _isDirty = True
                        UpdateButtonsState()

                        If _context IsNot Nothing Then
                            _context.ClearError(num)
                        End If
                    End Sub
                End If

                ctrl = num

            Case "DATE"

                Dim dtp As New DateTimePicker With {
                .Name = "dtpValeur",
                .Format = DateTimePickerFormat.Short,
                .Width = 200,
                .Enabled = editable,
                .Margin = New Padding(4, 5, 4, 5)
            }
                AppliquerStyleControleDetail(dtp, editable)

                Dim parsed As Date
                If Date.TryParse(valeur, parsed) Then
                    dtp.Value = parsed
                End If

                If editable Then
                    AddHandler dtp.ValueChanged,
                    Sub()
                        _isDirty = True
                        UpdateButtonsState()

                        If _context IsNot Nothing Then
                            _context.ClearError(dtp)
                        End If
                    End Sub
                End If

                ctrl = dtp

            Case Else

                Dim txt As New TextBox With {
                .Name = "txtValeur",
                .Text = valeur,
                .ReadOnly = Not editable,
                .Width = 400,
                .Margin = New Padding(4, 5, 4, 5)
            }
                AppliquerStyleControleDetail(txt, editable)

                If editable Then
                    AddHandler txt.TextChanged,
                    Sub()
                        _isDirty = True
                        UpdateButtonsState()

                        If _context IsNot Nothing Then
                            _context.ClearError(txt)
                        End If
                    End Sub
                End If

                ctrl = txt

        End Select

        ctrl.Tag = parametre

        tlp.Controls.Add(lbl, 0, rowIndex)
        tlp.Controls.Add(ctrl, 1, rowIndex)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AjouterChampDescription
    ' Projet     : Althéa
    ' Version    : V1.0.0
    ' Date       : 30/04/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       :
    ' Ajoute le champ Description dans le panneau détail avec une TextBox multiligne.
    '
    ' Paramètres :
    ' - tlp       : TableLayoutPanel cible
    ' - parametre : paramètre affiché
    ' - editable  : indique si le champ peut être modifié
    ' -------------------------------------------------------------------------------------------------
    Private Sub AjouterChampDescription(tlp As TableLayoutPanel, parametre As ParametreApplication, editable As Boolean)

        Dim rowIndex As Integer = tlp.RowCount

        tlp.RowCount += 1
        tlp.RowStyles.Add(New RowStyle(SizeType.Absolute, 95))

        Dim lbl As New Label With {
        .Text = "Description :",
        .AutoSize = True,
        .ForeColor = UITheme.DynamicLabelFore,
        .Font = New Font("Calibri", 10, FontStyle.Bold),
        .Margin = New Padding(4, 8, 12, 8)
    }

        Dim txt As New TextBox With {
        .Name = "txtDescription",
        .Text = parametre.DescriptionParametre,
        .Multiline = True,
        .ScrollBars = ScrollBars.Vertical,
        .ReadOnly = Not editable,
        .Dock = DockStyle.Fill,
        .Margin = New Padding(4, 5, 4, 5),
        .BackColor = UITheme.ColorBeigeClair,
        .ForeColor = UITheme.DynamicTextFore
    }

        If editable Then
            AddHandler txt.TextChanged,
            Sub()
                _isDirty = True
                UpdateButtonsState()
            End Sub
        End If

        txt.Tag = parametre

        tlp.Controls.Add(lbl, 0, rowIndex)
        tlp.Controls.Add(txt, 1, rowIndex)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AjouterCheckBox
    ' Projet     : Althéa
    ' Version    : V1.2.0
    ' Date       : 30/04/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       :
    ' Ajoute une case à cocher dans le panneau de détail.
    ' 
    ' -------------------------------------------------------------------------------------------------
    Private Sub AjouterCheckBox(tlp As TableLayoutPanel, libelle As String, valeur As Boolean, editable As Boolean, nomControle As String)

        Dim rowIndex As Integer = tlp.RowCount

        tlp.RowCount += 1
        tlp.RowStyles.Add(New RowStyle(SizeType.AutoSize))

        Dim lbl As New Label With {
        .Text = libelle & " :",
        .AutoSize = True,
        .ForeColor = UITheme.DynamicLabelFore,
        .Font = New Font("Calibri", 10, FontStyle.Bold),
        .Margin = New Padding(4, 8, 12, 8)
    }

        Dim chk As New CheckBox With {
        .Name = nomControle,
        .Checked = valeur,
        .Enabled = editable,
        .Margin = New Padding(4, 8, 4, 8),
        .BackColor = UITheme.ColorBeigeClair,
        .ForeColor = UITheme.DynamicTextFore
    }

        If editable Then
            AddHandler chk.CheckedChanged,
            Sub()
                _isDirty = True
                UpdateButtonsState()
            End Sub
        End If

        tlp.Controls.Add(lbl, 0, rowIndex)
        tlp.Controls.Add(chk, 1, rowIndex)

    End Sub

#End Region

#Region "Lecture / Écriture UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : LireValeursDepuisUI
    ' Projet     : Althéa
    ' Version    : V1.4.0
    ' Date       : 03/05/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       :
    ' Lit la valeur actuellement affichée dans le panneau détail et met à jour
    ' l’objet ParametreApplication courant avant sauvegarde.
    '
    ' Paramètres :
    ' - pnlDetail : panel contenant les contrôles de détail
    ' - param     : paramètre courant à mettre à jour
    ' -------------------------------------------------------------------------------------------------
    Private Sub LireValeursDepuisUI(pnlDetail As Panel, param As ParametreApplication)

        Dim txtCle = TryCast(pnlDetail.Controls.Find("txtCleParametre", True).FirstOrDefault(), TextBox)
        Dim txtLibelle = TryCast(pnlDetail.Controls.Find("txtLibelle", True).FirstOrDefault(), TextBox)
        'Dim txtGroupe = TryCast(pnlDetail.Controls.Find("txtGroupe", True).FirstOrDefault(), TextBox)

        Dim ctrlGroupe = pnlDetail.Controls.Find("txtGroupe", True).FirstOrDefault()
        Dim txtGroupe = TryCast(ctrlGroupe, TextBox)
        Dim cmbGroupe = TryCast(ctrlGroupe, ComboBox)

        Dim cmbType = TryCast(pnlDetail.Controls.Find("cmbType", True).FirstOrDefault(), ComboBox)

        Dim txtValeur = TryCast(pnlDetail.Controls.Find("txtValeur", True).FirstOrDefault(), TextBox)
        Dim chkValeur = TryCast(pnlDetail.Controls.Find("chkValeur", True).FirstOrDefault(), CheckBox)
        Dim numValeur = TryCast(pnlDetail.Controls.Find("numValeur", True).FirstOrDefault(), NumericUpDown)
        Dim dtpValeur = TryCast(pnlDetail.Controls.Find("dtpValeur", True).FirstOrDefault(), DateTimePicker)

        Dim txtDescription = TryCast(pnlDetail.Controls.Find("txtDescription", True).FirstOrDefault(), TextBox)
        Dim chkActif = TryCast(pnlDetail.Controls.Find("chkActif", True).FirstOrDefault(), CheckBox)
        Dim chkModifiable = TryCast(pnlDetail.Controls.Find("chkModifiable", True).FirstOrDefault(), CheckBox)

        If txtCle IsNot Nothing Then
            param.CleParametre = UtilsString.NormalizeTechnicalCode(txtCle.Text)
        End If

        If txtLibelle IsNot Nothing Then
            param.LibelleParametre = txtLibelle.Text.Trim()
        End If

        If cmbGroupe IsNot Nothing Then
            param.GroupeParametre = UtilsString.NormalizeTechnicalCode(cmbGroupe.Text)
        ElseIf txtGroupe IsNot Nothing Then
            param.GroupeParametre = UtilsString.NormalizeTechnicalCode(txtGroupe.Text)
        End If

        If cmbType IsNot Nothing AndAlso cmbType.SelectedItem IsNot Nothing Then
            param.TypeValeur = cmbType.SelectedItem.ToString()
        End If

        If txtValeur IsNot Nothing Then
            param.ValeurParametre = txtValeur.Text
        ElseIf chkValeur IsNot Nothing Then
            param.ValeurParametre = If(chkValeur.Checked, "1", "0")
        ElseIf numValeur IsNot Nothing Then
            param.ValeurParametre = numValeur.Value.ToString(Globalization.CultureInfo.InvariantCulture)
        ElseIf dtpValeur IsNot Nothing Then
            param.ValeurParametre = dtpValeur.Value.ToString("yyyy-MM-dd")
        End If

        If txtDescription IsNot Nothing Then
            param.DescriptionParametre = txtDescription.Text
        End If

        If chkActif IsNot Nothing Then
            param.Actif = chkActif.Checked
        End If

        If chkModifiable IsNot Nothing Then
            param.ModifiableUtilisateur = chkModifiable.Checked
        End If

    End Sub

#End Region

#Region "Validation"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ValiderParametreAvantSauvegarde
    ' Projet     : Althéa
    ' Version    : V1.4.0
    ' Date       : 02/05/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       :
    ' Valide les champs obligatoires d’un paramètre avant sauvegarde.
    ' Affiche les erreurs via le contexte UI global, sans MessageBox.
    '
    ' Paramètres :
    ' - param     : paramètre courant à valider
    ' - pnlDetail : panel contenant les contrôles dynamiques du détail
    '
    ' Retour     :
    ' - True si la validation est correcte
    ' - False si un ou plusieurs champs obligatoires sont manquants
    ' -------------------------------------------------------------------------------------------------
    Private Function ValiderParametreAvantSauvegarde(param As ParametreApplication,
                                                 pnlDetail As Panel) As Boolean

        Dim isValid As Boolean = True
        Dim messagesErreur As New List(Of String)

        Dim txtCle = TryCast(pnlDetail.Controls.Find("txtCleParametre", True).FirstOrDefault(), TextBox)
        Dim txtLibelle = TryCast(pnlDetail.Controls.Find("txtLibelle", True).FirstOrDefault(), TextBox)
        Dim txtGroupe = TryCast(pnlDetail.Controls.Find("txtGroupe", True).FirstOrDefault(), TextBox)
        Dim cmbType = TryCast(pnlDetail.Controls.Find("cmbType", True).FirstOrDefault(), ComboBox)
        Dim txtValeur = TryCast(pnlDetail.Controls.Find("txtValeur", True).FirstOrDefault(), TextBox)
        Dim chkValeur = TryCast(pnlDetail.Controls.Find("chkValeur", True).FirstOrDefault(), CheckBox)

        If _context IsNot Nothing Then
            If txtCle IsNot Nothing Then _context.ClearError(txtCle)
            If txtLibelle IsNot Nothing Then _context.ClearError(txtLibelle)
            If txtGroupe IsNot Nothing Then _context.ClearError(txtGroupe)
            If cmbType IsNot Nothing Then _context.ClearError(cmbType)
            If txtValeur IsNot Nothing Then _context.ClearError(txtValeur)
            If chkValeur IsNot Nothing Then _context.ClearError(chkValeur)
        End If

        If String.IsNullOrWhiteSpace(param.CleParametre) Then
            If _context IsNot Nothing AndAlso txtCle IsNot Nothing Then
                _context.SetError(txtCle, "La clé technique est obligatoire.")
            End If

            messagesErreur.Add("La clé technique est obligatoire.")
            isValid = False
        End If

        ' Vérification unicité de la clé uniquement en création
        If _modeEditionParametre = ModeEditionParametre.Nouveau AndAlso
   Not String.IsNullOrWhiteSpace(param.CleParametre) Then

            If GestionParametres.CleParametreExiste(param.CleParametre, 0) Then

                If _context IsNot Nothing AndAlso txtCle IsNot Nothing Then
                    _context.SetError(txtCle, "Cette clé existe déjà.")
                End If

                messagesErreur.Add("La clé technique existe déjà.")
                isValid = False

                GestionLog.EcrireLog(
            $"UI: création refusée - clé paramètre déjà existante ({param.CleParametre}).",
            GestionLog.LogLevel.Rapide,
            GestionLog.LogCategory.UI
        )

            End If

        End If

        If String.IsNullOrWhiteSpace(param.LibelleParametre) Then
            If _context IsNot Nothing AndAlso txtLibelle IsNot Nothing Then
                _context.SetError(txtLibelle, "Le libellé est obligatoire.")
            End If

            messagesErreur.Add("Le libellé est obligatoire.")
            isValid = False
        End If

        If String.IsNullOrWhiteSpace(param.GroupeParametre) Then
            If _context IsNot Nothing AndAlso txtGroupe IsNot Nothing Then
                _context.SetError(txtGroupe, "Le groupe est obligatoire.")
            End If

            messagesErreur.Add("Le groupe est obligatoire.")
            isValid = False
        End If

        If String.IsNullOrWhiteSpace(param.TypeValeur) Then
            If _context IsNot Nothing AndAlso cmbType IsNot Nothing Then
                _context.SetError(cmbType, "Le type de valeur est obligatoire.")
            End If

            messagesErreur.Add("Le type de valeur est obligatoire.")
            isValid = False
        End If

        If Not String.IsNullOrWhiteSpace(param.TypeValeur) Then

            Dim messageType As String = String.Empty

            If Not UtilsValidation.IsValidValueForType(param.ValeurParametre, param.TypeValeur, messageType) Then

                Dim ctrlValeur As Control = Nothing

                If txtValeur IsNot Nothing Then
                    ctrlValeur = txtValeur
                ElseIf chkValeur IsNot Nothing Then
                    ctrlValeur = chkValeur
                End If

                If _context IsNot Nothing AndAlso ctrlValeur IsNot Nothing Then
                    _context.SetError(ctrlValeur, messageType)
                End If

                messagesErreur.Add(messageType)
                isValid = False

            End If

        End If

        If Not isValid Then

            Dim messageFinal As String =
            String.Join(Environment.NewLine, messagesErreur.Distinct())

            UpdateContextInfo("Enregistrement refusé : corrige les champs signalés.")

            GestionLog.EcrireLog(
            "UI: validation du paramètre refusée - " & messageFinal.Replace(Environment.NewLine, " | "),
            GestionLog.LogLevel.Rapide,
            GestionLog.LogCategory.UI
        )

            DialogChoix.Avertissement(
            "L'enregistrement est refusé :" & Environment.NewLine & Environment.NewLine &
            messageFinal,
            "Althéa - Validation"
        )

        End If

        Return isValid

    End Function

#End Region

#Region "Gestion état UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateButtonsState
    ' Projet     : Althéa
    ' Version    : V1.0.0
    ' Date       : 29/04/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       : Met à jour l'état des boutons en fonction du mode d'édition et des droits d'accès
    ' -------------------------------------------------------------------------------------------------
    Private Sub UpdateButtonsState()

        Dim isAdmin As Boolean = _modeAcces = ModeAccesParametres.Admin

        Select Case _modeEditionParametre

            Case ModeEditionParametre.Consultation

                btnNouveau.Enabled = isAdmin
                btnModifier.Enabled = _parametreCourant IsNot Nothing
                btnEnregistrer.Enabled = False
                btnAnnuler.Enabled = False

            Case ModeEditionParametre.Modification

                btnNouveau.Enabled = False
                btnModifier.Enabled = False
                btnEnregistrer.Enabled = _isDirty
                btnAnnuler.Enabled = True

            Case ModeEditionParametre.Nouveau

                btnNouveau.Enabled = False
                btnModifier.Enabled = False
                btnEnregistrer.Enabled = _isDirty
                btnAnnuler.Enabled = True

        End Select

    End Sub

#End Region

#Region "Helpers / Utilitaires"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetLibelleModeAcces
    ' Projet     : Althéa
    ' Version    : V1.5.1
    ' Date       : 01/05/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       :
    ' Retourne le libellé utilisateur du mode d'accès courant.
    ' -------------------------------------------------------------------------------------------------
    Private Function GetLibelleModeAcces() As String

        Select Case _modeAcces
            Case ModeAccesParametres.Admin
                Return "mode Admin"
            Case ModeAccesParametres.SuperUser
                Return "mode SuperUser"
            Case Else
                Return "mode utilisateur"
        End Select

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateContextInfo
    ' Projet     : Althéa
    ' Version    : V1.5.1
    ' Date       : 01/05/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       :
    ' Met à jour le contexte affiché dans Home selon l'onglet,
    ' le paramètre courant et le mode d'accès.
    ' -------------------------------------------------------------------------------------------------
    Private Sub UpdateContextInfo(Optional statusMessage As String = Nothing)

        If _context Is Nothing Then Return

        Dim elements As New List(Of String) From {
            "Administration",
            "Paramètres"
        }

        If tabParametres.SelectedTab IsNot Nothing Then
            elements.Add(tabParametres.SelectedTab.Text)
        End If

        _context.SetHeader(String.Join(" > ", elements) & " (" & GetLibelleModeAcces() & ")")

        If Not String.IsNullOrWhiteSpace(statusMessage) Then
            _context.SetStatus(statusMessage)
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : GetPanelDetailCourant
    ' Projet     : Althéa
    ' Version    : V1.1
    ' Date       : 01/05/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       : Retourne le panel de détail actuellement affiché pour le paramètre sélectionné.
    ' Return : Panel de détail ou Nothing si non trouvé
    ' -------------------------------------------------------------------------------------------------
    Private Function GetPanelDetailCourant() As Panel

        Dim tab = tabParametres.SelectedTab
        If tab Is Nothing OrElse tab.Controls.Count = 0 Then Return Nothing

        Dim tlp As TableLayoutPanel = TryCast(tab.Controls(0), TableLayoutPanel)
        If tlp Is Nothing Then Return Nothing

        Return TryCast(tlp.GetControlFromPosition(1, 0), Panel)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : GetGroupeParametreCourant
    ' Projet     : Althéa
    ' Version    : V1.0
    ' Date       : 02/05/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       : Retourne le groupe actuellement affiché pour le paramètre sélectionné.
    ' Return : Groupe de paramètres ou "GENERAL" si non trouvé
    ' -------------------------------------------------------------------------------------------------
    Private Function GetGroupeParametreCourant() As String

        If tabParametres IsNot Nothing AndAlso
           tabParametres.SelectedTab IsNot Nothing AndAlso
           Not String.IsNullOrWhiteSpace(tabParametres.SelectedTab.Text) Then

            Return tabParametres.SelectedTab.Text.Trim()

        End If

        Return "GENERAL"

    End Function





#End Region

#Region "Création UI dynamique"

    Private Function CreerDataGridParametres() As DataGridView

        Dim dgv As New DataGridView With {
            .Dock = DockStyle.Fill,
            .ReadOnly = True,
            .AutoGenerateColumns = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .MultiSelect = False,
            .BackgroundColor = Color.White,
            .BorderStyle = BorderStyle.None
        }

        UtilsDataGrid.InitDataGridBasique(dgv)

        Dim colEtat As New DataGridViewImageColumn With {
            .Name = "colEtat",
            .HeaderText = "",
            .Width = 36,
            .ImageLayout = DataGridViewImageCellLayout.Normal
        }

        colEtat.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        dgv.Columns.Add(colEtat)

        dgv.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "LibelleParametre",
            .HeaderText = "Paramètre",
            .Width = 200
        })

        dgv.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "ValeurParametre",
            .HeaderText = "Valeur",
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        })

        Return dgv

    End Function

    Private Sub ConfigurerDataGridParametres(dgv As DataGridView,
                                         pnlDetail As Panel)

        AddHandler dgv.SelectionChanged,
            Sub()

                If _suspendSelectionChanged Then Return

                If _isDirty Then

                    Dim result = DialogChoix.Confirmer(
                        "Des modifications n'ont pas été enregistrées." & Environment.NewLine &
                        "Voulez-vous quitter sans enregistrer ?",
                        "Attention"
                    )

                    If result = DialogResult.No Then
                        Return
                    End If

                End If

                If dgv.CurrentRow Is Nothing Then Return

                Dim parametre As ParametreApplication =
                    TryCast(dgv.CurrentRow.DataBoundItem, ParametreApplication)

                If parametre Is Nothing Then Return

                If _context IsNot Nothing Then
                    _context.ClearAllErrors()
                End If

                _parametreCourant = parametre
                AfficherDetailParametre(pnlDetail, parametre)

                UpdateContextInfo($"Paramètre sélectionné : {_parametreCourant.LibelleParametre}")
                UpdateButtonsState()

            End Sub

        AddHandler dgv.CellFormatting,
            Sub(sender, e)

                If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
                If dgv.Columns(e.ColumnIndex).Name <> "colEtat" Then Return

                Dim row = dgv.Rows(e.RowIndex)
                Dim param = TryCast(row.DataBoundItem, ParametreApplication)

                If param Is Nothing Then Return

                If param.Actif Then
                    e.Value = UtilsIcons.IconOK(16)
                Else
                    e.Value = UtilsIcons.IconOFF(16)
                End If

                row.Cells("colEtat").ToolTipText =
                    If(param.Actif, "Paramètre actif", "Paramètre désactivé")

                If Not param.Actif Then
                    row.DefaultCellStyle.ForeColor = UITheme.DynamicTextDisabledFore
                Else
                    row.DefaultCellStyle.ForeColor = UITheme.DynamicTextFore
                End If

            End Sub

    End Sub

    Private Sub InitialiserSelection(dgv As DataGridView, pnlDetail As Panel)

        If dgv.Rows.Count = 0 Then Return

        dgv.ClearSelection()
        dgv.Rows(0).Selected = True
        dgv.CurrentCell = dgv.Rows(0).Cells(0)

        Dim parametreInitial As ParametreApplication =
            TryCast(dgv.Rows(0).DataBoundItem, ParametreApplication)

        If parametreInitial Is Nothing Then Return

        _parametreCourant = parametreInitial

        If _context IsNot Nothing Then
            _context.ClearAllErrors()
        End If

        AfficherDetailParametre(pnlDetail, parametreInitial)
        UpdateButtonsState()

    End Sub




#End Region

End Class
